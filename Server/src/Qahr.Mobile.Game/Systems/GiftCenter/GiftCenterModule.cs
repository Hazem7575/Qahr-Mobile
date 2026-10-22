// [GiftCenter 2026-10-14] Server side of the official Gift Center (CDlgGiftCenterMain#3663, GiftToFriend#3661, GiftToMe#3664,
// GiftRequirement#3665, GiftGiveLog#3667). Protocol reverse-engineered from Conquer.exe 7957 (Ghidra):
//
//   2423 C->S (builder FUN_00fc5334):
//     f1=0 send gift      f2 target uid, f3 kind (1 avatar / 2 frame), f4 hairface id, f5 text id 1..4
//     f1=3 reply an ask   f2 ask record id, f5 text id 1..4 (target / kind / item come from the stored ask)
//     f1=2 ask for gift   f2 target uid, f3 kind, f4 hairface id, f5 text id 5..13
//     f1=1 claim          f2 received record id
//     f1=5 refuse ask     f2 ask record id          (client removes the row itself)
//     f1=4 delete log     f2 sent-log record id     (client removes the row itself)
//   2423 S->C: f1=0 "Sent gift successfully"; f1=1 {f2 id} claimed; f1=100 {f2 id} claimed but already owned -> converted
//              to an item (Face.lua); f1=2 refresh; f1=3 {f2 ask id} "You have replied to the request".
//   2050 C->S f1 0/1/2 = list request (sent log / asks / received); f1=3 {f4[].f1} mark received read; f1=4 {f4[0].f1} mark
//        ask read. S->C f1 list, f2 clear, f3 last chunk, f4 records (chunked under the 1 KB send limit).
//   2090 C->S f1=1 {f2 0/1} "reject all asks". S->C f1=0 login {f2 reject, f3 asks used today, f4 bits (1 received, 2 ask)},
//        f1=1 {f2} ack, f1=2 {f3 used} ask success, f1=3 {f4 bits} live notify.
//
// Rules (official = client data/code, derived = GiftCenter.ini):
//   * Only the client's giftable catalog (hairface_storage_type: avatars >= 10M Silver, frames not "get-way 6").
//   * The giver pays the client price (CPs if money type 1, else Silver), deducted BEFORE the gift record exists.
//   * Claim: ledger line first, then the grant; already owned -> Face.lua conversion item (needs one free bag slot).
//   * Every id the client sends is looked up in the server's own lists of client.Player.UID; nothing else is trusted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Qahr.Mobile.GiftCenter
{
    public enum GcResult
    {
        Ok, Disabled, NotReady, RateLimited, BadRequest, BadItem, BadText, Self, TargetOffline, NotFriend, TargetOwns,
        TargetListFull, NoMoney, NoRecord, AlreadyClaimed, Expired, BagFull, AskLimit, AlreadyAsked, Rejected, OwnItem
    }

    public static class GiftCenterModule
    {
        public const ushort IdPresent = 2423, IdPresentList = 2050, IdPresentUser = 2090;
        const int RecordsPerChunk = 8;// 8 records ~ 8 * 60 bytes, well under the 1032-byte send limit

        // test seams
        public static Action<Client.GameClient, ServerSockets.Packet> SendHook = (c, p) => c.Send(p);
        public static Func<DateTime> Now = () => DateTime.Now;
        public static Func<uint, Client.GameClient> FindOnline = uid =>
        {
            Client.GameClient c;
            return Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading ? c : null;
        };

        static GcConfig Cfg { get { GiftCenterStore.EnsureLoaded(); return GiftCenterStore.Config; } }

        // ------------------------------------------------------------------ throttle
        sealed class Slots { public readonly int[] Last = new int[4]; public readonly bool[] Used = new bool[4]; public bool Pushed; }
        static readonly ConditionalWeakTable<Client.GameClient, Slots> slots = new ConditionalWeakTable<Client.GameClient, Slots>();
        static bool Allow(Client.GameClient c, int slot, int ms)
        {
            var s = slots.GetOrCreateValue(c);
            int now = Environment.TickCount;
            lock (s)
            {
                if (s.Used[slot] && unchecked(now - s.Last[slot]) < ms) return false;
                s.Used[slot] = true; s.Last[slot] = now; return true;
            }
        }
        public static void ResetThrottle(Client.GameClient c) { slots.Remove(c); }

        static bool Ready(Client.GameClient c) { return c != null && c.Player != null && c.FullLoading && c.Player.UID != 0; }

        // ------------------------------------------------------------------ helpers
        static long Unix(DateTime t) { return (long)(t.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }
        static int DayKey(DateTime t) { return t.Year * 10000 + t.Month * 100 + t.Day; }
        static ulong Stamp(long unix)
        {
            var t = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix).ToLocalTime();
            return (ulong)t.Year * 10000000000UL + (ulong)t.Month * 100000000UL + (ulong)t.Day * 1000000UL + (ulong)t.Hour * 10000UL + (ulong)t.Minute * 100UL + (ulong)t.Second;
        }

        static GcPlayer PlayerState(uint uid)
        {
            GcPlayer p;
            if (!GiftCenterStore.Players.TryGetValue(uid, out p)) GiftCenterStore.Players[uid] = p = new GcPlayer { Uid = uid };
            var today = DayKey(Now());
            if (p.AskDay != today) { p.AskDay = today; p.AskCount = 0; }
            return p;
        }

        static bool Expired(GcRecord r) { return Unix(Now()) - r.Created > (long)Cfg.RecordLifetimeDays * 86400L; }

        static IEnumerable<GcRecord> ListOf(uint owner, GcList list)
        {
            return GiftCenterStore.Records.Values.Where(r => r.Owner == owner && r.List == list && !Expired(r));
        }

        static GcRecord Find(uint owner, GcList list, uint id)
        {
            GcRecord r;
            return GiftCenterStore.Records.TryGetValue(id, out r) && r.Owner == owner && r.List == list ? r : null;
        }

        static bool Owns(Client.GameClient c, uint kind, uint item)
        {
            if (c == null || c.HairfaceStorage == null) return false;
            return c.HairfaceStorage.Exists(item, kind == 1 ? Game.MsgServer.MsgHairfaceStorage.Type.Avatar : Game.MsgServer.MsgHairfaceStorage.Type.Frame);
        }

        static bool IsFriend(Client.GameClient c, uint uid)
        {
            try { return c.Player.Associate != null && c.Player.Associate.Contain(Role.Instance.Associate.Friends, uid); } catch { return false; }
        }

        static uint Bits(uint uid)
        {
            uint b = 0;
            foreach (var r in GiftCenterStore.Records.Values)
            {
                if (r.Owner != uid || Expired(r)) continue;
                if (r.List == GcList.Received && (r.New || !r.Claimed)) b |= 1;
                if (r.List == GcList.Asks && r.New) b |= 2;
            }
            return b;
        }

        static void Tell(Client.GameClient c, string text)
        {
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static void Send(Client.GameClient c, object proto, ushort id)
        {
            if (c == null) return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                s.InitWriter();
                s.ProtoBufferSerialize(proto);
                s.Finalize(id);
                SendHook(c, s);
            }
        }

        static void Log(Client.GameClient c, string text)
        {
            GiftCenterStore.Log((c != null && c.Player != null ? c.Player.Name + "(" + c.Player.UID + ") " : "") + text);
        }

        static string ErrorText(GcResult r)
        {
            switch (r)
            {
                case GcResult.TargetOwns: return "Failed. The player has had it.";                 // STR_GIFT_TO_FRIEND_ERROR_1
                case GcResult.TargetListFull: return "Failed. The player`s gift list is full.";   // STR_GIFT_TO_FRIEND_ERROR_2
                case GcResult.AlreadyAsked: return "You have asked for the gift.";                 // STR_GIFT_TO_FRIEND_ERROR_3
                case GcResult.TargetOffline: return "The player is offline.";                      // STR_GIFT_TO_FRIEND_ERROR_4
                case GcResult.BagFull: return "The inventory is full.";                            // STR_GIFT_TO_FRIEND_ERROR_6
                case GcResult.NoMoney: return null;// the client already shows STR_GIFT_TO_FRIEND_NO_EMONEY / NO_MONEY itself
                case GcResult.NotFriend: return "You can only send gifts to your friends.";
                case GcResult.AskLimit: return "You have no asking chances left today.";
                case GcResult.Rejected: return "The player rejects all messages asking for gifts.";
                case GcResult.Expired: return "The gift has expired.";
                case GcResult.RateLimited: return "You clicked too fast! Please try later.";       // STR_PRESENT_MAIN_REFUSE_CD
                default: return null;
            }
        }

        // ================================================================== 2423
        public static GcResult Handle2423(Client.GameClient c, GcPresentPB req)
        {
            if (!Ready(c) || req == null) return GcResult.NotReady;
            var cfg = Cfg;
            if (!cfg.Enabled) return GcResult.Disabled;
            if (!Allow(c, 0, cfg.ActionIntervalMs)) return GcResult.RateLimited;
            GcResult r;
            switch (req.f1)
            {
                case 0: r = SendGift(c, req.f2, (uint)req.f3, req.f4, req.f5, 0); break;
                case 3: r = ReplyAsk(c, req.f2, req.f5); break;
                case 2: r = Ask(c, req.f2, (uint)req.f3, req.f4, req.f5); break;
                case 1: r = Claim(c, req.f2); break;
                case 5: r = Remove(c, GcList.Asks, req.f2, "refuse ask"); break;
                case 4: r = Remove(c, GcList.Sent, req.f2, "delete log"); break;
                default: r = GcResult.BadRequest; break;
            }
            if (r != GcResult.Ok) { var t = ErrorText(r); if (t != null) Tell(c, t); }
            return r;
        }

        static GcResult CheckTarget(Client.GameClient c, uint targetUid, out Client.GameClient target)
        {
            target = null;
            if (targetUid == 0 || targetUid == c.Player.UID) return GcResult.Self;
            var cfg = Cfg;
            target = FindOnline(targetUid);
            if (target == null && cfg.RequireTargetOnline) return GcResult.TargetOffline;
            if (cfg.RequireFriend && !IsFriend(c, targetUid)) return GcResult.NotFriend;
            if (target == null) return GcResult.TargetOffline;// the name / mesh / "already owned" checks need the live player
            return GcResult.Ok;
        }

        /// <summary>f1=0 (askId 0) or the reply path of f1=3 (askId = the ask record being answered).</summary>
        static GcResult SendGift(Client.GameClient c, uint targetUid, uint kind, uint item, uint text, uint askId)
        {
            var cfg = Cfg;
            if (text < 1 || text > 4) return GcResult.BadText;
            GcItem gi;
            if (kind < 1 || kind > 2 || !GiftCenterStore.Catalog.TryGetValue(GiftCenterStore.Key(kind, item), out gi)) return GcResult.BadItem;
            Client.GameClient target;
            var tr = CheckTarget(c, targetUid, out target);
            if (tr != GcResult.Ok) return tr;
            lock (GiftCenterStore.Sync)
            {
                if (askId != 0 && Find(c.Player.UID, GcList.Asks, askId) == null) return GcResult.NoRecord;
                if (Owns(target, kind, item)) return GcResult.TargetOwns;
                if (ListOf(targetUid, GcList.Received).Count() >= cfg.MaxReceived) return GcResult.TargetListFull;

                // 1) pay first (the client's price, never a client amount)
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    if (gi.PayCps)
                    {
                        if (gi.Price > uint.MaxValue || c.Player.ConquerPoints < gi.Price) return GcResult.NoMoney;
                        c.Player.ConquerPoints -= (uint)gi.Price;
                    }
                    else
                    {
                        if (c.Player.Money < gi.Price) return GcResult.NoMoney;
                        c.Player.Money -= gi.Price;
                        try { c.Player.SendUpdate(s, c.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money); } catch { }
                    }
                }

                // 2) records
                long now = Unix(Now());
                var got = new GcRecord
                {
                    Id = GiftCenterStore.NextId++, Owner = targetUid, List = GcList.Received, OtherUid = c.Player.UID, OtherName = c.Player.Name,
                    OtherMesh = c.Player.Mesh, Kind = kind, Item = item, Text = text, Created = now, New = true
                };
                var log = new GcRecord
                {
                    Id = GiftCenterStore.NextId++, Owner = c.Player.UID, List = GcList.Sent, OtherUid = targetUid, OtherName = target.Player.Name,
                    OtherMesh = target.Player.Mesh, Kind = kind, Item = item, Text = text, Created = now, New = false
                };
                GiftCenterStore.Records[got.Id] = got;
                GiftCenterStore.Records[log.Id] = log;
                TrimSentLog(c.Player.UID);
                if (askId != 0) GiftCenterStore.Records.Remove(askId);
                GiftCenterStore.Save();
                Log(c, "SEND " + (kind == 1 ? "avatar " : "frame ") + item + " (" + gi.Name + ") to " + target.Player.Name + "(" + targetUid + ") price "
                    + gi.Price + (gi.PayCps ? " CPs" : " Silver") + " record " + got.Id + (askId != 0 ? " (reply to ask " + askId + ")" : ""));
                if (askId != 0) Send(c, new GcPresentPB { f1 = 3, f2 = askId }, IdPresent);
                else Send(c, new GcPresentPB { f1 = 0 }, IdPresent);
                Send(target, new GcPresentUserPB { f1 = 3, f4 = Bits(targetUid) }, IdPresentUser);
                Tell(target, c.Player.Name + " sent you 1 " + (kind == 1 ? "Avatar" : "Avatar Frame") + ". Go check it!");
            }
            return GcResult.Ok;
        }

        static void TrimSentLog(uint uid)
        {
            var list = GiftCenterStore.Records.Values.Where(r => r.Owner == uid && r.List == GcList.Sent).OrderBy(r => r.Id).ToList();
            for (int i = 0; i < list.Count - Cfg.MaxSentLog; i++) GiftCenterStore.Records.Remove(list[i].Id);
        }

        static GcResult ReplyAsk(Client.GameClient c, uint askId, uint text)
        {
            GcRecord ask;
            lock (GiftCenterStore.Sync)
            {
                ask = Find(c.Player.UID, GcList.Asks, askId);
                if (ask == null) return GcResult.NoRecord;
                if (Expired(ask)) return GcResult.Expired;
            }
            return SendGift(c, ask.OtherUid, ask.Kind, ask.Item, text, askId);
        }

        static GcResult Ask(Client.GameClient c, uint targetUid, uint kind, uint item, uint text)
        {
            var cfg = Cfg;
            if (text < 5 || text > 13) return GcResult.BadText;
            GcItem gi;
            if (kind < 1 || kind > 2 || !GiftCenterStore.Catalog.TryGetValue(GiftCenterStore.Key(kind, item), out gi)) return GcResult.BadItem;
            if (Owns(c, kind, item)) return GcResult.OwnItem;
            Client.GameClient target;
            var tr = CheckTarget(c, targetUid, out target);
            if (tr != GcResult.Ok) return tr;
            lock (GiftCenterStore.Sync)
            {
                var me = PlayerState(c.Player.UID);
                if (me.AskCount >= cfg.AskLimitPerDay) return GcResult.AskLimit;
                var them = PlayerState(targetUid);
                if (them.RejectAsks) return GcResult.Rejected;
                var asks = ListOf(targetUid, GcList.Asks).ToList();
                if (asks.Any(a => a.OtherUid == c.Player.UID && a.Kind == kind && a.Item == item)) return GcResult.AlreadyAsked;
                if (asks.Count >= cfg.MaxAsks) return GcResult.TargetListFull;
                me.AskCount++;
                var a2 = new GcRecord
                {
                    Id = GiftCenterStore.NextId++, Owner = targetUid, List = GcList.Asks, OtherUid = c.Player.UID, OtherName = c.Player.Name,
                    OtherMesh = c.Player.Mesh, Kind = kind, Item = item, Text = text, Created = Unix(Now()), New = true
                };
                GiftCenterStore.Records[a2.Id] = a2;
                GiftCenterStore.Save();
                Log(c, "ASK " + (kind == 1 ? "avatar " : "frame ") + item + " from " + target.Player.Name + "(" + targetUid + ") record " + a2.Id + " used " + me.AskCount);
                Send(c, new GcPresentUserPB { f1 = 2, f3 = me.AskCount }, IdPresentUser);
                Send(target, new GcPresentUserPB { f1 = 3, f4 = Bits(targetUid) }, IdPresentUser);
            }
            return GcResult.Ok;
        }

        static GcResult Claim(Client.GameClient c, uint id)
        {
            var cfg = Cfg;
            lock (GiftCenterStore.Sync)
            {
                var r = Find(c.Player.UID, GcList.Received, id);
                if (r == null) return GcResult.NoRecord;
                if (r.Claimed || GiftCenterStore.IsClaimedInLedger(r.Id)) { r.Claimed = true; return GcResult.AlreadyClaimed; }
                if (Expired(r)) return GcResult.Expired;
                bool owned = Owns(c, r.Kind, r.Item);
                uint dupItem = r.Kind == 1 ? cfg.DuplicateAvatarItem : cfg.DuplicateFrameItem;
                Database.ItemType.DBItem dbi = null;
                if (owned)
                {
                    if (dupItem == 0 || !Pool.ItemsBase.TryGetValue(dupItem, out dbi)) return GcResult.BadItem;
                    if (!c.Inventory.HaveSpace(1)) return GcResult.BagFull;
                }
                Database.HairfaceStorageTable.Hairface tpl = null;
                if (!owned)
                {
                    var type = r.Kind == 1 ? Game.MsgServer.MsgHairfaceStorage.Type.Avatar : Game.MsgServer.MsgHairfaceStorage.Type.Frame;
                    tpl = Database.HairfaceStorageTable.Hairfaces.FirstOrDefault(h => h.ID == r.Item && h.Type == type);
                    if (tpl == null) return GcResult.BadItem;
                }

                // 1) claim is final before anything is granted (ledger + record)
                GiftCenterStore.LedgerClaim(r);
                r.Claimed = true; r.New = false;
                GiftCenterStore.Save();

                // 2) grant
                if (owned)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                        c.Inventory.AddItemWitchStack(dupItem, 0, 1, rec.GetStream(), false);
                    Log(c, "CLAIM record " + r.Id + " (already owned " + r.Item + ") -> item " + dupItem + " x1");
                    Send(c, new GcPresentPB { f1 = 100, f2 = r.Id }, IdPresent);
                }
                else
                {
                    var copy = new Database.HairfaceStorageTable.Hairface
                    {
                        Type = tpl.Type, ID = tpl.ID, Name = tpl.Name, RareLevel = tpl.RareLevel, RequiredVIPLevel = tpl.RequiredVIPLevel, Cost = tpl.Cost,
                        Classes = tpl.Classes == null ? new List<byte>() : new List<byte>(tpl.Classes), Sex = tpl.Sex,
                        Colors = new byte[7] { 1, 0, 0, 0, 0, 0, 0 }, Equiped = false, EquippedColor = 0
                    };
                    c.HairfaceStorage.Add(copy, true);
                    try { Database.ServerDatabase.SaveHairfaceStorage(c); } catch (Exception e) { Log(c, "hairface save error " + e.Message); }
                    Log(c, "CLAIM record " + r.Id + " -> " + (r.Kind == 1 ? "avatar " : "frame ") + r.Item + " (" + tpl.Name + ") from " + r.OtherName + "(" + r.OtherUid + ")");
                    Send(c, new GcPresentPB { f1 = 1, f2 = r.Id }, IdPresent);
                }
            }
            return GcResult.Ok;
        }

        static GcResult Remove(Client.GameClient c, GcList list, uint id, string what)
        {
            lock (GiftCenterStore.Sync)
            {
                var r = Find(c.Player.UID, list, id);
                if (r == null) return GcResult.NoRecord;
                GiftCenterStore.Records.Remove(id);
                GiftCenterStore.Save();
                Log(c, what.ToUpperInvariant() + " record " + id);
            }
            return GcResult.Ok;
        }

        // ================================================================== 2050
        public static GcResult Handle2050(Client.GameClient c, GcPresentListPB req)
        {
            if (!Ready(c) || req == null) return GcResult.NotReady;
            var cfg = Cfg;
            if (!cfg.Enabled) return GcResult.Disabled;
            if (req.f1 >= 0 && req.f1 <= 2)
            {
                if (!Allow(c, 1, cfg.ListIntervalMs)) return GcResult.RateLimited;
                List<GcRecord> list;
                lock (GiftCenterStore.Sync) list = ListOf(c.Player.UID, (GcList)req.f1).OrderByDescending(r => r.Id).ToList();
                SendList(c, req.f1, list);
                return GcResult.Ok;
            }
            if (req.f1 == 3 || req.f1 == 4)
            {
                if (!Allow(c, 2, 100)) return GcResult.RateLimited;
                var lt = req.f1 == 3 ? GcList.Received : GcList.Asks;
                bool changed = false;
                lock (GiftCenterStore.Sync)
                {
                    foreach (var x in req.f4.Take(8))
                    {
                        var r = x == null ? null : Find(c.Player.UID, lt, x.f1);
                        if (r != null && r.New) { r.New = false; changed = true; }
                    }
                    if (changed) GiftCenterStore.Save();
                }
                return GcResult.Ok;
            }
            return GcResult.BadRequest;
        }

        public static void SendList(Client.GameClient c, int type, List<GcRecord> list)
        {
            int i = 0;
            do
            {
                var pb = new GcPresentListPB { f1 = type, f2 = i == 0, f3 = i + RecordsPerChunk >= list.Count };
                foreach (var r in list.Skip(i).Take(RecordsPerChunk))
                    pb.f4.Add(new GcRecordPB
                    {
                        f1 = r.Id, f2 = r.OtherUid, f3 = r.OtherName, f4 = r.OtherMesh, f5 = r.Kind, f6 = r.Item, f7 = r.Text,
                        f8 = (ulong)r.Created, f9 = r.Claimed, f10 = r.New, f11 = Stamp(r.Created)
                    });
                Send(c, pb, IdPresentList);
                i += RecordsPerChunk;
            } while (i < list.Count);
        }

        // ================================================================== 2090
        public static GcResult Handle2090(Client.GameClient c, GcPresentUserPB req)
        {
            if (!Ready(c) || req == null) return GcResult.NotReady;
            var cfg = Cfg;
            if (!cfg.Enabled) return GcResult.Disabled;
            if (req.f1 != 1) return GcResult.BadRequest;
            if (!Allow(c, 3, cfg.ListIntervalMs)) return GcResult.RateLimited;
            bool on = req.f2 != 0;
            lock (GiftCenterStore.Sync)
            {
                var p = PlayerState(c.Player.UID);
                if (p.RejectAsks != on) { p.RejectAsks = on; GiftCenterStore.Save(); Log(c, "reject asks = " + on); }
            }
            Send(c, new GcPresentUserPB { f1 = 1, f2 = on ? 1u : 0u }, IdPresentUser);
            return GcResult.Ok;
        }

        /// <summary>Login state (2090 f1=0): reject flag, asks used today, red-dot bits.</summary>
        public static void LoginPush(Client.GameClient c)
        {
            if (!Ready(c) || !Cfg.Enabled) return;
            GcPresentUserPB pb;
            lock (GiftCenterStore.Sync)
            {
                var p = PlayerState(c.Player.UID);
                pb = new GcPresentUserPB { f1 = 0, f2 = p.RejectAsks ? 1u : 0u, f3 = p.AskCount, f4 = Bits(c.Player.UID) };
            }
            Send(c, pb, IdPresentUser);
        }

        // ================================================================== timer (login push + expiry)
        static Timer timer;
        static int lastPrune;
        public static void Start()
        {
            try
            {
                GiftCenterStore.EnsureLoaded();
                timer = new Timer(_ => Tick(), null, 5000, 3000);
                MyConsole.WriteLine("[GiftCenter] " + GiftCenterStore.Catalog.Count + " giftable item(s), " + GiftCenterStore.Records.Count + " record(s)"
                    + (GiftCenterStore.LoadError != null ? " - DISABLED: " + GiftCenterStore.LoadError : ""), ConsoleColor.Cyan);
            }
            catch (Exception e) { try { MyConsole.WriteLine("[GiftCenter] start failed: " + e.Message, ConsoleColor.Yellow); } catch { } }
        }

        public static void Tick()
        {
            try
            {
                if (!Cfg.Enabled || Pool.GamePoll == null) return;
                foreach (var c in Pool.GamePoll.Values)
                {
                    if (!Ready(c)) continue;
                    var s = slots.GetOrCreateValue(c);
                    if (s.Pushed) continue;
                    s.Pushed = true;
                    LoginPush(c);
                }
                if (unchecked(Environment.TickCount - lastPrune) > 600000 || lastPrune == 0)
                {
                    lastPrune = Environment.TickCount;
                    lock (GiftCenterStore.Sync)
                    {
                        var old = GiftCenterStore.Records.Values.Where(Expired).Select(r => r.Id).ToList();
                        if (old.Count > 0)
                        {
                            foreach (var id in old) GiftCenterStore.Records.Remove(id);
                            GiftCenterStore.Save();
                            GiftCenterStore.Log("expired " + old.Count + " record(s)");
                        }
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ================================================================== packet entry points
        [PacketAttribute(IdPresent)]
        private static void OnPresent(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                GcPresentPB req;
                try { req = stream.ProtoBufferDeserialize(new GcPresentPB()); } catch { return; }
                Handle2423(client, req);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        /// <summary>Called from the 2050 handler (MY System\Unanswered\B7\B7Social.cs).</summary>
        public static void OnPresentList(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                GcPresentListPB req;
                try { req = stream.ProtoBufferDeserialize(new GcPresentListPB()); } catch { return; }
                Handle2050(client, req);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        /// <summary>Called from the 2090 handler (MY System\Unanswered\B7\B7Social.cs).</summary>
        public static void OnPresentUser(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                GcPresentUserPB req;
                try { req = stream.ProtoBufferDeserialize(new GcPresentUserPB()); } catch { return; }
                Handle2090(client, req);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
    }
}
