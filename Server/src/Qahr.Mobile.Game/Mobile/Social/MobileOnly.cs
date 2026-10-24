using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: messages of the Conquer Online Mobile client that have no server packet id (MOBILE_ONLY in
    /// conquer-mobile\docs\audit\protocol_coverage.txt). They arrive under the mobile id, which no server handler
    /// knows, so Program.Game_Receive offers them to Handle() before the normal dispatch. The answers are built
    /// here directly with the mobile id (the send path renumbers only known server ids). docs\MOBILE_ONLY.md lists
    /// every message and its state.
    /// Field numbers / offsets come from libCQ2ClientEn (CMsgXxxPB::k*FieldNumber, CMsgXxx::Create / Process).
    /// </summary>
    public static unsafe class MobileOnly
    {
        public const ushort FriendInfo = 10185;        // struct, server -> mobile (CMsgFriendInfo::Process)
        public const ushort FriendOnlineList = 10408;  // pb, both ways (CMsgFriendOnlineListPB)
        public const ushort FriendAcceptList = 10434;  // pb, mobile -> server (CFriendMgr::MsgAllMakeFriend / MsgAllMakeBuddy)
        public const ushort FriendSearch = 10433;      // pb, both ways (CMsgFriendSearchPB)
        public const ushort UserSearch = 10414;        // pb, both ways (CMsgUserSearchPB, guild recruit search)
        public const ushort BlackList = 10398;         // pb, both ways (CMsgQueryBlackListPB)
        public const ushort TeamInvite = 10432;        // pb, server -> mobile (CMsgTeamInvitePB)

        /// <summary>A mobile-only message from the client; true when it was handled here.</summary>
        public static bool Handle(Client.GameClient client, ServerSockets.Packet stream, ushort id)
        {
            switch (id)
            {
                case FriendOnlineList:
                case FriendAcceptList:
                case FriendSearch:
                case UserSearch:
                case BlackList:
                    break;
                default:
                    return false;
            }
            if (client == null || client.Player == null)
                return true;
            try
            {
                int len = Math.Min(*(ushort*)stream.Memory, stream.Size);
                var f = MobileWire.ParseProto(stream.Memory + 4, Math.Max(0, len - 4));
                switch (id)
                {
                    case FriendOnlineList: OnFriendOnlineList(client, f); break;
                    case FriendAcceptList: OnFriendAcceptList(client, f); break;
                    case FriendSearch: OnSearch(client, f, false); break;
                    case UserSearch: OnSearch(client, f, true); break;
                    case BlackList: OnBlackList(client, f); break;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
            }
            return true;
        }

        // ------------------------------------------------------------------------------------- friends

        /// <summary>CMsgFriendOnlineListPB: 1 = repeated CMsgFriendOnlinePB {1 UnUserID, 3 BOnline}. The client sends
        /// the uids (CHeroFriendAndEnemyMgr::QueryPlayerOnline), the answer carries the online flag
        /// (CMsgFriendOnlineList::Process -> SetPlayerOnline(uid, online)).</summary>
        static void OnFriendOnlineList(Client.GameClient client, Dictionary<int, List<object>> f)
        {
            var body = new MemoryStream();
            int n = 0;
            foreach (var sub in Messages(f, 1))
            {
                uint uid = (uint)U(sub, 1);
                if (uid == 0 || n++ >= 200)
                    continue;
                var e = new MemoryStream();
                MobileWire.WriteVarintField(e, 1, uid);
                MobileWire.WriteVarintField(e, 3, Online(uid) != null ? 1UL : 0UL);
                Sub(body, 1, e);
            }
            Send(client, FriendOnlineList, body.ToArray());
        }

        /// <summary>CMsgFriendAcceptListPB {1 UnAction, 2 IdList (repeated uint)}: "accept all" in the friend request
        /// list. Action 0 = friend requests (each uid accepted like MsgFriend 2254 AcceptRequest), 1 = trade buddy
        /// requests (not supported yet: the server keeps no list of pending trade partner requests).</summary>
        static void OnFriendAcceptList(Client.GameClient client, Dictionary<int, List<object>> f)
        {
            if (U(f, 1) != 0)
                return;
            foreach (uint uid in Uints(f, 2).Distinct().Take(50))
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var p = rec.GetStream();
                    p.InitWriter();
                    p.Write(uid);
                    p.Write((byte)Game.MsgServer.MsgKnowPersons.Action.AcceptRequest);
                    p.Write((byte)1);
                    p.Finalize(Game.GamePackets.MsgFriend);
                    p.Seek(4);
                    Game.MsgServer.MsgKnowPersons.HandlerKnowPersons(client, p);
                }
            }
        }

        /// <summary>CMsgFriendSearchPB {1 SzSearchName, 2 repeated CMsgFriendSearchResultPB {1 UnUserId, 2 SzUserName,
        /// 3 UnLookface, 4 UnLevel, 5 UnBattleEffect, 6 UnProf}} and CMsgUserSearchPB {1 SzSearchName, 2 repeated
        /// CMsgUserSearchResultPB {1 UnUserId, 2 SzUserName, 3 UnLookface, 4 UnAbilityScore, 5 IdSyn, 6 UnFaceFrame}}.
        /// No result = an empty list (the client shows "not found"). Online players only.</summary>
        static void OnSearch(Client.GameClient client, Dictionary<int, List<object>> f, bool user)
        {
            string name = Str(f, 1).Trim();
            var body = new MemoryStream();
            WriteString(body, 1, name);
            if (name.Length > 0)
            {
                var all = Pool.GamePoll.Values.Where(c => c != null && c.Player != null && c.Player.UID != client.Player.UID
                    && c.Player.Name != null && c.Player.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(c => string.Equals(c.Player.Name, name, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .ThenBy(c => c.Player.Name.Length)
                    .Take(20);
                foreach (var c in all)
                {
                    var p = c.Player;
                    var e = new MemoryStream();
                    MobileWire.WriteVarintField(e, 1, p.UID);
                    WriteString(e, 2, p.Name);
                    MobileWire.WriteVarintField(e, 3, p.Mesh);
                    if (user)
                    {
                        MobileWire.WriteVarintField(e, 4, (ulong)Math.Max(0, p.BattlePower));
                        MobileWire.WriteVarintField(e, 5, p.GuildID);
                    }
                    else
                    {
                        MobileWire.WriteVarintField(e, 4, p.Level);
                        MobileWire.WriteVarintField(e, 5, (ulong)Math.Max(0, p.BattlePower));
                        MobileWire.WriteVarintField(e, 6, p.Class);
                    }
                    Sub(body, 2, e);
                }
            }
            Send(client, user ? UserSearch : FriendSearch, body.ToArray());
        }

        /// <summary>
        /// CMsgFriendInfo 10185 (the details of a friend / enemy; the answer to MsgAction ViewFriendInfo /
        /// ViewEnemyInfo, which the PC client gets as MsgRelationInfo 2079 that the mobile does not know).
        /// CMsgFriendInfo::Process: uid 4, lookface 8, level 12, profession 16, PK points i16 20, guild id 24,
        /// u32 28 (unknown), guild rank u16 32, mate name 34 (32), enemy flag u8 66, u32 68 (unknown).
        /// </summary>
        public static void SendFriendInfo(Client.GameClient client, Role.Player target, bool enemy)
        {
            if (client == null || target == null)
                return;
            var b = new byte[72];
            MobileWire.WriteU32(b, 4, target.UID);
            MobileWire.WriteU32(b, 8, target.Mesh);
            MobileWire.WriteU32(b, 12, target.Level);
            MobileWire.WriteU32(b, 16, target.Class);
            MobileWire.WriteU16(b, 20, target.PKPoints);
            MobileWire.WriteU32(b, 24, target.GuildID);
            MobileWire.WriteU16(b, 32, (ushort)target.GuildRank);
            PutString(b, 34, 32, target.Spouse);
            b[66] = (byte)(enemy ? 1 : 0);
            SendStruct(client, FriendInfo, b);
        }

        // ------------------------------------------------------------------------------------- black list

        /// <summary>
        /// CMsgQueryBlackListPB {1 IdUser, 2 UnAction, 3 UnFrom, 4 repeated CBlackListInfo {1 IdBlack, 2 BOnline,
        /// 3 NLev, 4 DwLookFace, 5 SzName, 6 UnAbilityScore, 7 SzSynName, 8 DwFaceFrame}}.
        /// Actions (CMsgBlackList::Create(action, uid) / Process): 0 = add IdUser (answer: the new entry, the client
        /// adds it and shows a tip), 1 = remove IdUser (answer: same action and uid), 2 = query the list at login
        /// (IdUser = own uid; answer: every entry). Kept per player in Data\social_blacklist.txt.
        /// </summary>
        static void OnBlackList(Client.GameClient client, Dictionary<int, List<object>> f)
        {
            uint uid = (uint)U(f, 1);
            uint action = (uint)U(f, 2);
            uint me = client.Player.UID;
            switch (action)
            {
                case 2:
                    {
                        List<BlackEntry> list;
                        lock (blackSync)
                        {
                            LoadBlack();
                            list = black.TryGetValue(me, out var l) ? l.ToList() : new List<BlackEntry>();
                        }
                        SendBlack(client, 2, me, list);
                        break;
                    }
                case 0:
                    {
                        if (uid == 0 || uid == me)
                            return;
                        var target = Online(uid);
                        BlackEntry entry;
                        lock (blackSync)
                        {
                            LoadBlack();
                            if (!black.TryGetValue(me, out var l))
                                black[me] = l = new List<BlackEntry>();
                            entry = l.FirstOrDefault(x => x.Uid == uid);
                            if (entry == null)
                            {
                                if (target == null)
                                {
                                    client.SendSysMesage("The player must be online to be added to the black list.");
                                    return;
                                }
                                if (l.Count >= 50)
                                {
                                    client.SendSysMesage("Your black list is full.");
                                    return;
                                }
                                entry = new BlackEntry { Uid = uid };
                                l.Add(entry);
                            }
                            if (target != null)
                                entry.From(target.Player);
                            SaveBlack();
                        }
                        SendBlack(client, 0, uid, new List<BlackEntry> { entry });
                        break;
                    }
                case 1:
                    {
                        lock (blackSync)
                        {
                            LoadBlack();
                            if (black.TryGetValue(me, out var l) && l.RemoveAll(x => x.Uid == uid) > 0)
                                SaveBlack();
                        }
                        SendBlack(client, 1, uid, new List<BlackEntry>());
                        break;
                    }
            }
        }

        /// <summary>True when "owner" has "uid" on the mobile black list.</summary>
        public static bool IsBlocked(uint owner, uint uid)
        {
            lock (blackSync)
            {
                LoadBlack();
                return black.TryGetValue(owner, out var l) && l.Any(x => x.Uid == uid);
            }
        }

        class BlackEntry
        {
            public uint Uid, Level, Mesh;
            public string Name = "", Guild = "";
            public int Ability;
            public void From(Role.Player p)
            {
                Name = p.Name ?? "";
                Level = p.Level;
                Mesh = p.Mesh;
                Ability = Math.Max(0, p.BattlePower);
                Guild = p.MyGuild != null ? (p.MyGuild.GuildName ?? "") : "";
            }
        }

        static readonly object blackSync = new object();
        static Dictionary<uint, List<BlackEntry>> black;

        static string BlackPath
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "social_blacklist.txt"); }
        }

        /// <summary>"owner uid level mesh ability name\tguild" per line.</summary>
        static void LoadBlack()
        {
            if (black != null)
                return;
            black = new Dictionary<uint, List<BlackEntry>>();
            if (!File.Exists(BlackPath))
                return;
            foreach (var line in File.ReadAllLines(BlackPath))
            {
                var tab = line.Split('\t');
                var p = tab[0].Split(new[] { ' ' }, 6);
                uint owner;
                if (p.Length < 6 || !uint.TryParse(p[0], out owner))
                    continue;
                var e = new BlackEntry { Name = p[5], Guild = tab.Length > 1 ? tab[1] : "" };
                uint.TryParse(p[1], out e.Uid);
                uint.TryParse(p[2], out e.Level);
                uint.TryParse(p[3], out e.Mesh);
                int.TryParse(p[4], out e.Ability);
                if (!black.TryGetValue(owner, out var l))
                    black[owner] = l = new List<BlackEntry>();
                l.Add(e);
            }
        }

        static void SaveBlack()
        {
            var sb = new StringBuilder();
            foreach (var kv in black)
                foreach (var e in kv.Value)
                    sb.Append(kv.Key).Append(' ').Append(e.Uid).Append(' ').Append(e.Level).Append(' ').Append(e.Mesh).Append(' ')
                      .Append(e.Ability).Append(' ').Append((e.Name ?? "").Replace('\t', ' ').Replace('\n', ' '))
                      .Append('\t').Append((e.Guild ?? "").Replace('\t', ' ').Replace('\n', ' ')).Append('\n');
            File.WriteAllText(BlackPath, sb.ToString());
        }

        static void SendBlack(Client.GameClient client, uint action, uint uid, List<BlackEntry> list)
        {
            var body = new MemoryStream();
            MobileWire.WriteVarintField(body, 1, uid);
            MobileWire.WriteVarintField(body, 2, action);
            foreach (var x in list)
            {
                var online = Online(x.Uid);
                if (online != null)
                    x.From(online.Player);
                var e = new MemoryStream();
                MobileWire.WriteVarintField(e, 1, x.Uid);
                MobileWire.WriteVarintField(e, 2, online != null ? 1UL : 0UL);
                MobileWire.WriteVarintField(e, 3, x.Level);
                MobileWire.WriteVarintField(e, 4, x.Mesh);
                WriteString(e, 5, x.Name);
                MobileWire.WriteVarintField(e, 6, (ulong)x.Ability);
                WriteString(e, 7, x.Guild);
                Sub(body, 4, e);
            }
            Send(client, BlackList, body.ToArray());
        }

        // ------------------------------------------------------------------------------------- team

        /// <summary>
        /// CMsgTeamInvitePB {1 UnAction, 2 NData, 3 BLastPacket, 4 repeated CMsgTeamInviteInfoPB {1 IdTeam, 2 IdUser,
        /// 3 BLeader, 4 DwLookFace, 5 SzName, 6 DwBattleEffect, 7 DwLev, 8 DwProfession, 9 NTeamType, 10 IdActivity}}.
        /// CMsgTeam::Process has no invite / join request handling on the mobile: CMsgTeamInvite::Process puts
        /// action 0/1 entries (IdTeam 0) into the hero's team invitations (CHero::AddTeamInvite) and action 6 entries
        /// into the leader's join requests (CTeam::AddTeamJoin); BLastPacket refreshes the panel.
        /// Sent next to the server's MsgTeam InviteRequest / JoinRequest (MsgTeam.cs).
        /// </summary>
        public static void SendTeamInvite(Client.GameClient to, Client.GameClient from, bool joinRequest)
        {
            if (to == null || from == null || from.Player == null)
                return;
            var p = from.Player;
            var e = new MemoryStream();
            MobileWire.WriteVarintField(e, 2, p.UID);
            MobileWire.WriteVarintField(e, 3, joinRequest ? 0UL : 1UL);
            MobileWire.WriteVarintField(e, 4, p.Mesh);
            WriteString(e, 5, p.Name);
            MobileWire.WriteVarintField(e, 6, (ulong)Math.Max(0, p.BattlePower));
            MobileWire.WriteVarintField(e, 7, p.Level);
            MobileWire.WriteVarintField(e, 8, p.Class);
            var body = new MemoryStream();
            MobileWire.WriteVarintField(body, 1, joinRequest ? 6UL : 0UL);
            MobileWire.WriteVarintField(body, 3, 1);
            Sub(body, 4, e);
            Send(to, TeamInvite, body.ToArray());
        }

        // ------------------------------------------------------------------------------------- notices (not wired yet)

        // ------------------------------------------------------------------------------------- helpers

        static Client.GameClient Online(uint uid)
        {
            Client.GameClient c;
            return Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null ? c : null;
        }

        static List<Dictionary<int, List<object>>> Messages(Dictionary<int, List<object>> f, int field)
        {
            var r = new List<Dictionary<int, List<object>>>();
            List<object> vals;
            if (!f.TryGetValue(field, out vals))
                return r;
            foreach (var v in vals)
            {
                var b = v as byte[];
                if (b == null)
                    continue;
                fixed (byte* p = b)
                    r.Add(MobileWire.ParseProto(p, b.Length));
            }
            return r;
        }

        /// <summary>A repeated uint field, packed or not.</summary>
        static List<uint> Uints(Dictionary<int, List<object>> f, int field)
        {
            var r = new List<uint>();
            List<object> vals;
            if (!f.TryGetValue(field, out vals))
                return r;
            foreach (var v in vals)
            {
                var b = v as byte[];
                if (b == null)
                {
                    r.Add((uint)MobileWire.AsULong(v));
                    continue;
                }
                fixed (byte* p = b)
                {
                    int i = 0;
                    while (i < b.Length)
                        r.Add((uint)MobileWire.ReadVarint(p, b.Length, ref i));
                }
            }
            return r;
        }

        static ulong U(Dictionary<int, List<object>> f, int field)
        {
            List<object> vals;
            return f.TryGetValue(field, out vals) && vals.Count > 0 ? MobileWire.AsULong(vals[vals.Count - 1]) : 0;
        }

        static string Str(Dictionary<int, List<object>> f, int field)
        {
            List<object> vals;
            if (!f.TryGetValue(field, out vals) || vals.Count == 0)
                return "";
            var b = vals[vals.Count - 1] as byte[];
            return b == null ? "" : Encoding.UTF8.GetString(b);
        }

        static void WriteString(Stream s, int field, string v)
        {
            var b = Encoding.UTF8.GetBytes(v ?? "");
            MobileWire.WriteBytesField(s, field, b, 0, b.Length);
        }

        static void Sub(Stream s, int field, MemoryStream sub)
        {
            var b = sub.ToArray();
            MobileWire.WriteBytesField(s, field, b, 0, b.Length);
        }

        static void PutString(byte[] b, int offset, int size, string v)
        {
            var s = Encoding.UTF8.GetBytes(v ?? "");
            Buffer.BlockCopy(s, 0, b, offset, Math.Min(s.Length, size - 1));
        }

        /// <summary>A struct message: the bytes after the 4-byte header (b[0..3] is ignored).</summary>
        static void SendStruct(Client.GameClient client, ushort id, byte[] b)
        {
            var body = new byte[b.Length - 4];
            Buffer.BlockCopy(b, 4, body, 0, body.Length);
            Send(client, id, body);
        }

        static void Send(Client.GameClient client, ushort id, byte[] body)
        {
            if (client == null || body.Length > ServerSockets.Packet.MAX_SIZE - 64)
                return;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var p = rec.GetStream();
                p.InitWriter();
                foreach (byte x in body)
                    p.Write(x);
                p.Finalize(id);
                client.Send(p);
            }
        }
    }
}
