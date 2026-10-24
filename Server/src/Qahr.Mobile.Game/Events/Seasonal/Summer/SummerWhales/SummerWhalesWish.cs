// ============================================================================
//  [SummerWhales 2026-10-15] The mini-games behind the Summer Adventure (2022) window buttons, with server-side checks:
//
//   Whale Wish     official draw window DlgValentinesDayMainUI, DlgID "1255-Darw" (luaui dlgvalentinesday\valentinesdayinfo):
//                  open  "</F>AnniversaryPageAct_KehuDataUpdate"            -> DlgValentinesDay_MainOpen|{Code=100,...}
//                  pray  10250 "AnniversaryPageAct_KehuChouJiang</N>1|10"    -> DlgValentinesDay_DrawBack|{Code=200,...}
//                  stage 10250 "AnniversaryPageAct_KehuAcculate</N>1..8"     -> DlgValentinesDay_GetRewardBack|{Code=400,...}
//                  after the wheel the client asks 10139 "Lover2025Kua_DrawCongratulation" -> DlgValentinesDay_RewardServer.
//                  OFFICIAL: cost 49 / 490 Wish Conch 3600409 (DlgDrawCostItem), "Pray 1 time to get 1 Ambergris, 49 Favor
//                  Points, etc." (luaUiRes 1255-Darw DrawInfo), the 20 wheel items (DlgDrawItemList) and the 8 stage packs
//                  (DlgRewardItemlist), level 80, all texts. DERIVED (config): wheel weights, stage thresholds.
//   Transformation DlgSummerMergence (1255-mergence): "</F>SummerActivePage_SynthesisView" (open),
//                  "</F>SummerActivePage_Synthesis</N>1|2" (transform).
//                  OFFICIAL: Garment = 15 Ambergris 3337922 -> Silver Bubble (Sunny) 199325 (B), +0.5% per 10 failures,
//                  reset on success, failure gives one of 500 Chi Points / Silver Bubble 199315 (B) / Permanent Stone (B) /
//                  Big Permanent Stone (B) / 25-100 Universal Rune Essences (B) (Merge_Fail_Info); Accessory = 10 Ambergris
//                  -> Cool Surfboard 3337923 (tSummerActivePage[3337922][Text115]). DERIVED: base success rate.
//   Items          Ambergris 3337922 (tSummerActivePage[3337922]: transform / unbind garment 299 / Chi 500 each),
//                  Cool Surfboard 3337923 (choose the 7-day 1% blessed 2-handed 350189 or 1-handed 360401),
//                  Whale Spirit 3337924 (summon Whale King (Holy) 200774 / Chi 1000 each).
//                  DERIVED: spirits needed for Whale King (Holy); CP prices are 0 = not sold (official prices unknown).
//  Favor (Whale Favor Points) = 49 per pray, shown in DlgSummerMain.
//  State per character: Data\SummerWhales\players.json (FestPlayerStore, saved before any grant).
//  Hooks: SummerWhalesUi.OnRaw (LuaPush, existing line), SummerWhalesWish.OnUse (MsgItemUsuagePacket.UseItem),
//         SummerWhalesWish.OnItemDialog (Procesor, before the in-screen NPC search).
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using FL = Qahr.Mobile.Festivals.FestivalLoader;
using G = Qahr.Mobile.SummerWhales.FestGive;

namespace Qahr.Mobile.SummerWhales
{
    public static class SummerWhalesWish
    {
        public const string DlgId = "1255-Darw";
        public const uint MsgSummer23 = 10250, MsgValentines = 10139;
        public const uint WishConch = 3600409, Ambergris = 3337922, Surfboard = 3337923, WhaleSpirit = 3337924;
        public const uint SunnyBubble = 199325, Surf2H = 350189, Surf1H = 360401, WhaleKingHoly = 200774;

        // ---- official tables (client valentinesdayinfo.dat 1255-Darw)
        public static readonly string[] Wheel =
        {
            "3337924 0 3", "730005 0 1 3 7200 1", "4060001 0 10 3", "196885 0 1 3 0 0 0 0 1", "4060001 0 100 3",
            "3314212 0 1 3", "3337925 0 1 3", "4060001 0 100 3", "3314213 0 1 3", "3314207 0 1 3",
            "3337924 0 1", "730005 0 1 3 7200 1", "3009001 0 1 0 2880 1", "194825 0 1 3 0 0 0 0 1", "3009002 0 1 0 2880 1",
            "3314213 0 1 3", "200677 0 1 3 0 0 0 0 1", "3314208 0 1 3", "3009002 0 1 0 2880 1", "730003 0 1 3 7200 1",
        };
        public static readonly string[][] Stages =
        {
            new[] { "730004 0 1 3 7200 1", "3337924 0 1" },
            new[] { "3009002 0 1 0 2880 1", "3337924 0 1" },
            new[] { "4060001 0 100 3", "3337924 0 2" },
            new[] { "3009002 0 2 0 2880 1", "3337924 0 3" },
            new[] { "730005 0 1 3 7200 1", "3337924 0 3" },
            new[] { "3335923 0 1", "3337924 0 5", "3314250 0 1 3" },
            new[] { "3335924 0 1", "3337924 0 10", "200677 0 1 0 0 0 0 0 1" },
            new[] { "3335922 0 1", "3337924 0 1" },
        };
        // wheel slot colours (DlgDrawItemBg 1255-Darw): bg15 yellow = top, bg19 red = good, bg14 white = common
        static readonly char[] Rarity = { 'Y', 'R', 'W', 'Y', 'R', 'W', 'R', 'R', 'R', 'W', 'Y', 'R', 'W', 'Y', 'R', 'R', 'Y', 'R', 'R', 'W' };

        public sealed class Config
        {
            public int CostOne = 49, CostTen = 490;                 // official
            public int AmbergrisPerPray = 1, FavorPerPray = 49;     // official DrawInfo
            public long[] Weights;                                  // derived (by slot colour)
            public int[] StageAt = { 10, 20, 30, 50, 80, 120, 160, 200 };// derived
            public int GarmentCost = 15, AccessoryCost = 10;        // official
            public int GarmentBasePermille = 50;                    // derived: 5.0 %
            public int GarmentStepPermille = 5, GarmentStepFails = 10;// official: +0.5 % per 10 failures
            public int UnbindCost = 299;                            // official Option112
            public int ChiPerAmbergris = 500, ChiPerSpirit = 1000;  // official
            public int AmbergrisCpPrice = 0;                        // 0 = not sold (official price unknown)
            public int SpiritsForHoly = 30;                         // derived
            public int SpiritCpPrice = 0;                           // 0 = not sold (official price unknown)
            public uint SurfLifeMinutes = 10080; public byte SurfBless = 1;// official "7D 1% Blessed"
            public int DrawCooldownMs = 1500;                       // derived
            public Config()
            {
                Weights = Rarity.Select(r => r == 'Y' ? 5L : r == 'R' ? 25L : 50L).ToArray();
            }
        }
        public static Config Cfg { get { var u = SummerWhalesUi.Cfg; return _cfgStamp == u ? _cfg : Build(u); } }
        static Config _cfg = new Config(); static object _cfgStamp;
        static Config Build(object stamp)
        {
            var c = new Config();
            try
            {
                var d = SummerWhalesUi.RawConfig;
                var w = FL.Obj(FL.Val(d, "wish"));
                if (w != null)
                {
                    var ws = FL.Arr(FL.Val(w, "weights"));
                    if (ws != null && ws.Count == Wheel.Length) c.Weights = ws.Select(x => (long)Math.Min(1000000, FL.U(x))).ToArray();
                    var st = FL.Arr(FL.Val(w, "stageAt"));
                    if (st != null && st.Count == Stages.Length) c.StageAt = st.Select(x => (int)Math.Max(1, Math.Min(100000, FL.U(x)))).ToArray();
                    var cd = FL.Val(w, "cooldownMs"); if (cd != null) c.DrawCooldownMs = (int)Math.Max(300, Math.Min(60000, FL.U(cd)));
                }
                var m = FL.Obj(FL.Val(d, "transform"));
                if (m != null)
                {
                    var b = FL.Val(m, "garmentBasePermille"); if (b != null) c.GarmentBasePermille = (int)Math.Min(1000, FL.U(b));
                }
                var it = FL.Obj(FL.Val(d, "items"));
                if (it != null)
                {
                    var s = FL.Val(it, "spiritsForHoly"); if (s != null) c.SpiritsForHoly = (int)Math.Max(1, Math.Min(10000, FL.U(s)));
                    var p1 = FL.Val(it, "ambergrisCpPrice"); if (p1 != null) c.AmbergrisCpPrice = (int)Math.Min(100000, FL.U(p1));
                    var p2 = FL.Val(it, "spiritCpPrice"); if (p2 != null) c.SpiritCpPrice = (int)Math.Min(100000, FL.U(p2));
                }
            }
            catch { }
            _cfg = c; _cfgStamp = stamp;
            return c;
        }

        static FestPlayerStore _store;
        public static FestPlayerStore Store { get { return _store ?? (_store = new FestPlayerStore("SummerWhales")); } }
        public static void ResetForTests() { _store = null; _pending.Clear(); _last.Clear(); }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }
        static readonly ConcurrentDictionary<uint, Tuple<long, string[]>> _pending = new ConcurrentDictionary<uint, Tuple<long, string[]>>();
        static readonly Random _rng = new Random();
        static readonly object _rngLock = new object();
        static int Pick(long[] w)
        {
            long total = w.Sum();
            if (total <= 0) return -1;
            long r; lock (_rngLock) r = (long)(_rng.NextDouble() * total);
            for (int i = 0; i < w.Length; i++) { if (r < w[i]) return i; r -= w[i]; }
            return w.Length - 1;
        }
        static int Rand(int lo, int hi) { lock (_rngLock) return _rng.Next(lo, hi + 1); }

        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call); }
        static string Q(string s) { return "'" + s.Replace("\\", "").Replace("'", "") + "'"; }

        // official texts
        const string TxtWishOver = "Summer Beach Adventure has come to an end, and Whale Wish is no longer available.";
        const string TxtNoConch = "You don`t have enough Wish Conchs. You may click `+` on the interface to check out the ways to get some.";
        const string TxtWishLevel = "Sorry, you haven`t reached level 80, so you can`t pray here.";
        const string TxtSpace = "Please prepare {0} space(s) in your inventory first.";
        const string TxtStageLow = "You can`t claim the reward. You praying times are not enough.";
        const string TxtNoAmber = "You don`t have enough Ambergris to transform. Do you want to get some from Whale Wish?";
        const string TxtMergeOk = "Congrats! You`ve transformed Ambergris into {0}!";
        const string TxtMergeRate = "Transformation failed, but the success rate has been increased to {0}%.";
        const string TxtMergeBroadcast = "{0} is blessed by Whale King, and has transformed Ambergris into Silver Bubble (Sunny) (B)!";

        public static int Bit(int i) { return 1 << i; }

        // ------------------------------------------------------------------ Whale Wish
        public static long[] EffectiveWeights(Config cfg)
        {
            var w = new long[Wheel.Length];
            for (int i = 0; i < w.Length; i++)
            {
                var g = G.Parse(Wheel[i]);
                w[i] = g != null && G.Exists(g.Id) && i < cfg.Weights.Length ? Math.Max(0, cfg.Weights[i]) : 0;// a slot whose item this server lacks never wins
            }
            return w;
        }

        static string RewardList(uint uid, Config cfg, out int flag)
        {
            long draws = Store.Get(uid, "draws"), got = Store.Get(uid, "stages");
            var sb = new StringBuilder("{");
            flag = 0;
            for (int i = 0; i < Stages.Length; i++)
            {
                bool claimed = (got & Bit(i)) != 0;
                if (!claimed && draws >= cfg.StageAt[i]) flag = 1;
                if (i > 0) sb.Append(',');
                sb.Append('{').Append(cfg.StageAt[i]).Append(',').Append(claimed ? 1 : 0).Append('}');
            }
            return sb.Append('}').ToString();
        }

        public static string OpenData(Client.GameClient c)
        {
            var cfg = Cfg; int flag;
            string rl = RewardList(c.Player.UID, cfg, out flag);
            return "DlgValentinesDay_MainOpen|{['Code']=100,['Data']={['DlgID']=" + Q(DlgId) + ",['ModelType']=0,['CostItem']=" + G.Count(c, WishConch)
                + ",['ChangeItem']=" + G.Count(c, Ambergris) + ",['DrawTimes']=" + Store.Get(c.Player.UID, "draws") + ",['Rewardflag']=" + flag
                + ",['RewardList']=" + rl + ",['DrawList']={}}}";
        }

        public static void OpenWish(Client.GameClient c) { Lua(c, OpenData(c)); }

        /// <summary>Pray n (1 or 10) times. Returns the error text or null.</summary>
        public static string Draw(Client.GameClient c, ServerSockets.Packet stream, int n)
        {
            var cfg = Cfg;
            if (n != 1 && n != 10) return "?";
            if (!SummerWhalesUi.Running) return TxtWishOver;
            if (c.Player.Level < SummerWhalesUi.Cfg.MinLevel) return TxtWishLevel;
            int cost = n == 1 ? cfg.CostOne : cfg.CostTen;
            if (G.Count(c, WishConch) < cost) return TxtNoConch;
            var w = EffectiveWeights(cfg);
            var picks = new List<int>();
            for (int i = 0; i < n; i++) { int k = Pick(w); if (k < 0) return "Whale Wish isn`t available right now."; picks.Add(k); }
            var items = picks.Select(k => G.Parse(Wheel[k])).ToList();
            items.Add(new G.GItem { Id = Ambergris, Count = cfg.AmbergrisPerPray * n, Bound = true });
            int slots = G.Slots(items);
            if (!c.Inventory.HaveSpace((byte)Math.Min(40, slots))) return string.Format(TxtSpace, slots);
            // 1. cost, 2. state, 3. grant
            if (!G.Take(c, WishConch, cost, stream)) return TxtNoConch;
            long draws = 0;
            Store.Update(c.Player.UID, get =>
            {
                draws = get("draws") + n;
                return new Dictionary<string, long> { { "draws", draws }, { "favor", get("favor") + (long)cfg.FavorPerPray * n } };
            });
            G.Give(c, items, stream, "SummerWhales/WhaleWish");
            _pending[c.Player.UID] = Tuple.Create((long)(Environment.TickCount & 0x7FFFFFFF), items.Select(i => i.ToString()).ToArray());
            int flag; RewardList(c.Player.UID, cfg, out flag);
            Lua(c, "DlgValentinesDay_DrawBack|{['Code']=200,['Data']={['DlgID']=" + Q(DlgId) + ",['CostItem']=" + G.Count(c, WishConch) + ",['DrawTimes']=" + draws
                + ",['DrawList']={" + string.Join(",", picks.Select(k => (k + 1).ToString())) + "},['ChangeItem']=" + G.Count(c, Ambergris) + ",['Rewardflag']=" + flag + "}}");
            return null;
        }

        /// <summary>Claim stage reward idx (1-based). Returns the error text or null.</summary>
        public static string ClaimStage(Client.GameClient c, ServerSockets.Packet stream, int idx)
        {
            var cfg = Cfg;
            if (idx < 1 || idx > Stages.Length) return "?";
            if (!SummerWhalesUi.Running) return TxtWishOver;
            if (c.Player.Level < SummerWhalesUi.Cfg.MinLevel) return "Sorry, you haven`t reached level 80, so you can`t claim the reward.";
            int i = idx - 1;
            var items = Stages[i].Select(G.Parse).Where(x => x != null && G.Exists(x.Id)).ToList();
            int slots = G.Slots(items);
            if (!c.Inventory.HaveSpace((byte)Math.Min(40, slots))) return string.Format(TxtSpace, slots);
            string err = null;
            bool ok = Store.Update(c.Player.UID, get =>
            {
                long got = get("stages");
                if ((got & Bit(i)) != 0) { err = "Claimed"; return null; }
                if (get("draws") < cfg.StageAt[i]) { err = TxtStageLow; return null; }
                return new Dictionary<string, long> { { "stages", got | Bit(i) } };
            });
            if (!ok) return err ?? TxtStageLow;
            G.Give(c, items, stream, "SummerWhales/WishStage" + idx);
            int flag; string rl = RewardList(c.Player.UID, cfg, out flag);
            Lua(c, "DlgValentinesDay_GetRewardBack|{['Code']=400,['Data']={['DlgID']=" + Q(DlgId) + ",['RewardList']=" + rl + ",['Rewardflag']=" + flag + "}}");
            return null;
        }

        /// <summary>The client's "show what I won" call after the wheel (10139 Lover2025Kua_DrawCongratulation).</summary>
        public static bool OnCongratulation(Client.GameClient c)
        {
            Tuple<long, string[]> p;
            if (!_pending.TryRemove(c.Player.UID, out p)) return false;
            long now = Environment.TickCount & 0x7FFFFFFF;
            if (now - p.Item1 < 0 || now - p.Item1 > 120000) return false;
            Lua(c, "DlgValentinesDay_RewardServer|{['Code']=200,['Data']={" + string.Join(",", p.Item2.Select(Q)) + "}}");
            return true;
        }

        // ------------------------------------------------------------------ Ambergris transformation
        public static int GarmentPermille(uint uid, Config cfg)
        {
            long fails = Store.Get(uid, "mfail");
            return (int)Math.Min(1000, cfg.GarmentBasePermille + (fails / Math.Max(1, cfg.GarmentStepFails)) * cfg.GarmentStepPermille);
        }
        static string Pct(int permille) { return (permille / 10) + (permille % 10 != 0 ? "." + (permille % 10) : ""); }

        public static string MergeData(Client.GameClient c, int page)
        {
            var cfg = Cfg;
            return "DlgSummerMergence_BindData|{['Code']=200,['Data']={['Coat']='" + SunnyBubble + " 0 1 3',['Weapon']='" + Surfboard + " 0 1 3',['Probability']="
                + Pct(GarmentPermille(c.Player.UID, cfg)) + ",['ItemNum']=" + G.Count(c, Ambergris) + ",['ItemPic']='" + Ambergris + " 0 1 0',['Page']=" + page + "}}";
        }
        public static void OpenMerge(Client.GameClient c) { Lua(c, MergeData(c, 1)); }

        /// <summary>page 1 = garment (chance), 2 = accessory. Returns the error text or null.</summary>
        public static string Merge(Client.GameClient c, ServerSockets.Packet stream, int page)
        {
            var cfg = Cfg;
            if (page != 1 && page != 2) return "?";
            if (!SummerWhalesUi.Running) return SummerWhalesUi.TxtOverPublic;
            if (c.Player.Level < SummerWhalesUi.Cfg.MinLevel) return "Please level up to 80 first.";
            int cost = page == 1 ? cfg.GarmentCost : cfg.AccessoryCost;
            if (G.Count(c, Ambergris) < cost) return TxtNoAmber;
            if (!c.Inventory.HaveSpace(1)) return string.Format(TxtSpace, 1);
            if (page == 2)
            {
                if (!G.Take(c, Ambergris, cost, stream)) return TxtNoAmber;
                var it = new G.GItem { Id = Surfboard, Count = 1, Bound = true };
                G.Give(c, new[] { it }, stream, "SummerWhales/TransformAccessory");
                Lua(c, "DlgSummerMergence_CallBack|{['MergeCode']=200,['MergeData']={['Code']=200,['Data']={'" + it + "'}}}");
                Msg(c, string.Format(TxtMergeOk, G.ItemName(Surfboard)));
                Lua(c, MergeData(c, 2));
                return null;
            }
            // garment: roll first, then cost, then state, then grant
            int rate = GarmentPermille(c.Player.UID, cfg);
            int roll = Rand(0, 999);
            bool win = roll < rate;
            G.GItem give = null; int chi = 0;
            if (win) give = new G.GItem { Id = SunnyBubble, Count = 1, Bound = true };
            else
            {
                int k = Rand(0, 4);
                if (k == 0 && c.Player.MyChi == null) k = 1;
                switch (k)
                {
                    case 0: chi = 500; break;
                    case 1: give = new G.GItem { Id = 199315, Count = 1, Bound = true }; break;
                    case 2: give = new G.GItem { Id = 723694, Count = 1, Bound = true }; break;
                    case 3: give = new G.GItem { Id = 723695, Count = 1, Bound = true }; break;
                    default: give = new G.GItem { Id = 4060001, Count = Rand(25, 100), Bound = true }; break;
                }
                if (give != null && !G.Exists(give.Id)) { give = null; chi = c.Player.MyChi != null ? 500 : 0; }
            }
            if (!G.Take(c, Ambergris, cost, stream)) return TxtNoAmber;
            Store.Update(c.Player.UID, get => new Dictionary<string, long> { { "mfail", win ? 0 : get("mfail") + 1 }, { "mtries", get("mtries") + 1 } });
            if (give != null) G.Give(c, new[] { give }, stream, win ? "SummerWhales/TransformGarment" : "SummerWhales/TransformFail");
            if (chi > 0) AddChi(c, chi, "SummerWhales/TransformFail");
            if (win)
            {
                Lua(c, "DlgSummerMergence_CallBack|{['MergeCode']=200,['MergeData']={['Code']=200,['Data']={'" + give + "'}}}");
                Msg(c, string.Format(TxtMergeOk, "Silver Bubble (Sunny) (B)"));
                Broadcast(string.Format(TxtMergeBroadcast, c.Player.Name));
            }
            else
            {
                string item = give != null ? give.ToString() : "StrengthValue";
                Lua(c, "DlgSummerMergence_CallBack|{['MergeCode']=201,['MergeData']={['Code']=200,['Data']={['Item']='" + item + "'}}}");
                int newRate = GarmentPermille(c.Player.UID, cfg);
                if (newRate > rate) Msg(c, string.Format(TxtMergeRate, Pct(newRate)));
            }
            Lua(c, MergeData(c, 1));
            return null;
        }

        static void AddChi(Client.GameClient c, int amount, string why)
        {
            if (c.Player.MyChi == null || amount <= 0) return;
            long v = (long)c.Player.MyChi.ChiPoints + amount;
            c.Player.MyChi.ChiPoints = (int)Math.Min(int.MaxValue, v);
            try { MsgChiInfo.MsgHandleChi.SendInfo(c, MsgChiInfo.Action.Upgrade); } catch { }
            try { Festivals.FestivalManager.Log("grant", why, c.Player.Name + "(" + c.Player.UID + ") " + amount + " Chi Points"); } catch { }
        }

        static void Broadcast(string text)
        {
            try
            {
                if (Pool.GamePoll == null) return;
                using (var rec = new ServerSockets.RecycledPacket())
                    Server.SendGlobalPacket(new MsgMessage(text, "ALLUSERS", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.System).GetArray(rec.GetStream()));
            }
            catch { }
        }

        // ------------------------------------------------------------------ items (use + dialog)
        static bool IsOurs(uint id) { return id == Ambergris || id == Surfboard || id == WhaleSpirit; }

        /// <summary>MsgItemUsuagePacket.UseItem hook. True = handled.</summary>
        public static bool OnUse(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            try
            {
                if (c == null || c.Player == null || item == null || !IsOurs(item.ITEM_ID)) return false;
                c.ActiveNpc = item.ITEM_ID;
                ShowItemDialog(c, stream, item.ITEM_ID, 0);
                return true;
            }
            catch (Exception e) { MyConsole.SaveException(e); return false; }
        }

        /// <summary>Procesor hook: the answer of one of our item dialogs (npcid = item id). True = handled.</summary>
        public static bool OnItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            try
            {
                if (c == null || c.Player == null || !IsOurs(npcid)) return false;
                if (option == 255) return true;
                if (G.Count(c, npcid) <= 0) { Msg(c, "You don`t have this item in your inventory."); return true; }
                if (Throttled(c.Player.UID, 400)) return true;
                ShowItemDialog(c, stream, npcid, option);
                return true;
            }
            catch (Exception e) { MyConsole.SaveException(e); return true; }
        }

        static void Box(Client.GameClient c, ServerSockets.Packet stream, string text)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText(text).AddOption("OK.", 255).AddAvatar(0).FinalizeDialog();
        }

        static void ShowItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint id, byte opt)
        {
            var cfg = Cfg;
            var d = new Game.MsgNpc.Dialog(c, stream);
            if (id == Surfboard)
            {
                if (opt == 1 || opt == 2)
                {
                    if (!c.Inventory.HaveSpace(1)) { Box(c, stream, string.Format(TxtSpace, 1)); return; }
                    if (!G.Take(c, Surfboard, 1, stream)) return;
                    var it = new G.GItem { Id = opt == 1 ? Surf2H : Surf1H, Count = 1, Bound = true, LifeMinutes = cfg.SurfLifeMinutes, Bless = cfg.SurfBless };
                    G.Give(c, new[] { it }, stream, "SummerWhales/CoolSurfboard");
                    Msg(c, "You received " + G.ItemName(it.Id) + ".");
                    return;
                }
                d.AddText("Please choose the reward you want.")
                 .AddOption("7D 1% Blessed 2-handed Cool Surfboard.", 1)
                 .AddOption("7D 1% Blessed 1-handed Cool Surfboard.", 2)
                 .AddOption("Let me think about it.", 255).AddAvatar(0).FinalizeDialog();
                return;
            }
            if (id == Ambergris)
            {
                long have = G.Count(c, Ambergris);
                switch (opt)
                {
                    case 1:
                        if (!SummerWhalesUi.Running) { Box(c, stream, SummerWhalesUi.TxtOverPublic); return; }
                        OpenMerge(c); return;
                    case 2:
                        d.AddText(" Are you sure you want to consume " + cfg.UnbindCost + " Ambergris to turn Silver Bubble (Sunny) into an unbound one?\n")
                         .AddOption("Yes.", 21).AddOption("Choose again.", 9).AddAvatar(0).FinalizeDialog(); return;
                    case 21:
                        {
                            var bub = c.Inventory.ClientItems.Values.FirstOrDefault(i => i.ITEM_ID == SunnyBubble && i.Bound != 0);
                            if (bub == null) { Box(c, stream, "Sorry, you haven`t got a Silver Bubble (Sunny) (B)."); return; }
                            if (have < cfg.UnbindCost) { Box(c, stream, "Sorry, you don`t have enough Ambergris."); return; }
                            if (!G.Take(c, Ambergris, cfg.UnbindCost, stream)) return;
                            bub.Bound = 0; bub.Mode = Role.Flags.ItemMode.Update; bub.Send(c, stream);
                            try { Festivals.FestivalManager.Log("unbind", "SummerWhales", c.Player.Name + "(" + c.Player.UID + ") Silver Bubble (Sunny) uid " + bub.UID); } catch { }
                            Box(c, stream, "Your Silver Bubble (Sunny) is unbound now."); return;
                        }
                    case 3:
                        Box(c, stream, "Buying Ambergris isn`t available on this server."); return;
                    case 4:
                        if (c.Player.MyChi == null) { Box(c, stream, "Please unlock Chi first."); return; }
                        d.AddText("Are you sure you want to turn " + have + " Ambergris into " + have * cfg.ChiPerAmbergris + " Chi Points?\n")
                         .AddOption("Yes.", 41).AddOption("Choose again.", 9).AddAvatar(0).FinalizeDialog(); return;
                    case 41:
                        {
                            if (c.Player.MyChi == null || have <= 0) return;
                            if (!G.Take(c, Ambergris, have, stream)) return;
                            AddChi(c, (int)Math.Min(int.MaxValue, have * cfg.ChiPerAmbergris), "SummerWhales/AmbergrisChi");
                            Box(c, stream, "You received " + have * cfg.ChiPerAmbergris + " Chi Points."); return;
                        }
                }
                d.AddText("------------------------------------------------------\n").AddText("              Ambergris Owned: " + have + "\n")
                 .AddText("------------------------------------------------------\n")
                 .AddText("Collect 15 Ambergris to transform into 1 Silver Bubble (Sunny).\n")
                 .AddText("Collect 10 Ambergris to transform into 1 7-day Weapon Accessory, Cool Surfboard.\n")
                 .AddText("Every Ambergris can be turned into 500 Chi Points.\n")
                 .AddOption("Carry out transformation.", 1).AddOption("Make garment unbound. (" + cfg.UnbindCost + " Ambergris)", 2);
                if (cfg.AmbergrisCpPrice > 0) d.AddOption("Buy Ambergris. (" + cfg.AmbergrisCpPrice + " CPs/each)", 3);
                d.AddOption("Turn into Chi Points.", 4).AddAvatar(0).FinalizeDialog();
                return;
            }
            // Whale Spirit
            long sp = G.Count(c, WhaleSpirit);
            switch (opt)
            {
                case 1:
                    {
                        long missing = Math.Max(0, cfg.SpiritsForHoly - sp);
                        if (missing > 0 && cfg.SpiritCpPrice <= 0) { Box(c, stream, "You don`t have enough Whale Spirits to summon [Whale King (Holy)]."); return; }
                        d.AddText("Are you sure you want to pay " + missing * cfg.SpiritCpPrice + " CPs to buy " + missing + " Whale Spirits and summon [Whale King (Holy)]?\n")
                         .AddOption("Yes.", 21).AddOption("Choose again.", 9).AddAvatar(0).FinalizeDialog(); return;
                    }
                case 21:
                    {
                        long missing = Math.Max(0, cfg.SpiritsForHoly - sp);
                        long cps = missing * cfg.SpiritCpPrice;
                        if (missing > 0 && cfg.SpiritCpPrice <= 0) { Box(c, stream, "You don`t have enough Whale Spirits to summon [Whale King (Holy)]."); return; }
                        if (cps > 0 && c.Player.ConquerPoints < cps) { Box(c, stream, "You don`t have enough CPs to summon."); return; }
                        if (!c.Inventory.HaveSpace(1)) { Box(c, stream, "Please clear up your inventory first."); return; }
                        long take = cfg.SpiritsForHoly - missing;
                        if (take > 0 && !G.Take(c, WhaleSpirit, take, stream)) return;
                        if (cps > 0) c.Player.ConquerPoints -= (uint)cps;
                        G.Give(c, new[] { new G.GItem { Id = WhaleKingHoly, Count = 1, Bound = true } }, stream, "SummerWhales/WhaleKingHoly");
                        Box(c, stream, "Congrats! You received a [Whale King (Holy)]."); return;
                    }
                case 2:
                    if (c.Player.MyChi == null) { Box(c, stream, "Please unlock Chi first."); return; }
                    d.AddText("Are you sure you want to turn " + sp + " Whale Spirit(s) into " + sp * cfg.ChiPerSpirit + " Chi Points?\n")
                     .AddOption("Yes.", 31).AddOption("Choose again.", 9).AddAvatar(0).FinalizeDialog(); return;
                case 31:
                    {
                        if (c.Player.MyChi == null || sp <= 0) return;
                        if (!G.Take(c, WhaleSpirit, sp, stream)) return;
                        AddChi(c, (int)Math.Min(int.MaxValue, sp * cfg.ChiPerSpirit), "SummerWhales/SpiritChi");
                        Box(c, stream, "You received " + sp * cfg.ChiPerSpirit + " Chi Points."); return;
                    }
            }
            d.AddText("Whale King:\n").AddText("---------------------------------\n").AddText("Whale Spirits Owned: " + sp + "\n")
             .AddText("---------------------------------\n")
             .AddText("You can pay some CPs to get the missing Whale Spirits and summon [Whale King (Holy)] or turn them into Chi Points. Every spirit can be turned into 1000 Chi Points.\n")
             .AddOption("Get the missing Whale Spirits.", 1).AddOption("Turn into Chi Points.", 2).AddOption("Let me think about it.", 255)
             .AddAvatar(0).FinalizeDialog();
        }

        // ------------------------------------------------------------------ lua routing (called from SummerWhalesUi)
        /// <summary>msgId callbacks of the Whale Wish window (10250) and its congratulation call (10139). True = answered.</summary>
        public static bool OnMsg(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (msgId == MsgValentinesDay && func == "Lover2025Kua_DrawCongratulation") return OnCongratulation(c);
            if (msgId != MsgSummer23) return false;
            if (!func.StartsWith("AnniversaryPageAct_", StringComparison.Ordinal)) return false;
            if (!SummerWhalesUi.Running) { Msg(c, TxtWishOver); return true; }
            if (Throttled(c.Player.UID, Cfg.DrawCooldownMs)) return true;
            int n;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var s = rec.GetStream();
                string err = null;
                switch (func)
                {
                    case "AnniversaryPageAct_KehuChouJiang":
                        err = args.Length == 1 && int.TryParse(args[0], out n) ? Draw(c, s, n) : "?"; break;
                    case "AnniversaryPageAct_KehuAcculate":
                        err = args.Length == 1 && int.TryParse(args[0], out n) ? ClaimStage(c, s, n) : "?"; break;
                    case "AnniversaryPageAct_RankNpc":
                        err = "The Summer Adventure rankings aren`t available on this server yet."; break;
                    case "AnniversaryPageAct_ExchangeNpc":
                        err = "The Wish Conch exchange isn`t available on this server yet."; break;
                    default: return false;
                }
                if (err != null && err != "?") Msg(c, err);
            }
            return true;
        }
        const uint MsgValentinesDay = MsgValentines;
    }
}
