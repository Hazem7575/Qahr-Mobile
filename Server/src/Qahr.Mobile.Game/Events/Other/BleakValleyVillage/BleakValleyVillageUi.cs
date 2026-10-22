// ============================================================================
//  [BleakValleyVillage 2026-10-14 / COMPLETE 2026-10-15] The official Global Anniversary 2023 "Celebration Realm"
//  (Anniversary Bonfire, Spark Logs, expedition zones incl. the Bleak Valley; client msg 10217 tMsg_Info["Anniversary"],
//  cumulative rewards msg 10218 tMsg_Info["AnniversaryCommon"]). Shown while the festival BleakValleyVillage runs.
//
//  Official client (luaui\dialog\dlganniversary, luagui\1359, dlgmidautumcommon MidAutumn_Config[10218]):
//    button    Dlg23AnniversaryBtn (festival.json icon_show / icon_hide); click AnniversaryInterface_MainView</N>0</N>0.
//    window    Dlg23AnniversaryMainUi_MainOpen / _UpdateView |{Code=100,Data={Score,Grade,Spirit,Actlist[1..12],ItemInfo}}
//              Actlist {State 0 fog / 2 open, ActTime = seconds, IsFinish = 1 (Dragonette back)}.
//    clicks    AnniversaryInterface_ChkDraw (bonfire) -> Dlg23AnniversaryMainUi_PlayFireEff / _PlayUplevEff + _UpdateView;
//              AnniversaryInterface_ClickMainView</N>2 (Celebration Monument) -> DlgMidAutumnRank_BindData (ActivityId 10218,
//              the official cumulative rewards); </N>3..12 = the "Go" of a zone panel; </N>7 also = claim the Dragonette.
//              Dlg23AnniversaryFrogUi_MainOpen|{Code=300,Data={ShowIndex 1..20,ItemList}} = the Dragonette's findings.
//    10218     AnniversaryInterface_Phase</N>10218</N>tier (claim), AnniversaryInterface_OpenMainView (back),
//              AniversaryNpc_LinkRank</N>n (rankings).
//  Bleak Valley = map 12100 (FestivalMaps maps.txt -> doc 11203): the villagers / objects of quests 7653-7667 are festival
//  NPCs (Festivals\BleakValleyVillage\npcs.txt, official ids + cells), the Reputation Officer and the Exit are placed by
//  this file (ids 8876001 / 8876002, scripted below). Rules and numbers: BleakValleyCore.cs.
//
//  Hooks: Base\LuaUi\LuaPush.cs HandleCallback (msg 10217, unchanged; msg 10218, one new tagged line);
//         FestivalBoxOpen.RegisterBoxes (Grit packs / breads, no extra hook).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Qahr.Mobile.AnnivKit;
using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.BleakValleyVillage
{
    public static class BleakValleyVillageUi
    {
        public const uint LuaMsg = 10217;              // tMsg_Info["Anniversary"]
        public const uint CommonMsg = 10218;           // tMsg_Info["AnniversaryCommon"]
        public const string Festival = "BleakValleyVillage";
        public const uint SparkLog = 3600479;
        public const uint BleakMap = 12100;
        public const uint OfficerId = 8876001, ExitId = 8876002;// [anniversary NPC block 8876000-8876999]
        public const ushort OfficerX = 187, OfficerY = 222, ExitX = 207, ExitY = 233;// client minimap.lua [11203] markers
        public const uint OfficerMesh = 1060, ExitMesh = 20020;// PROXY looks: npc.ini [NpcType106] PrizeOfficer, [NpcType2002] ExitGuide

        const string TxtOver = "Anniversary Revelry is over. Thanks for your participation! See you next year!";// [36553][Text142]
        const string TxtLevel = "Requirement: Level 80";
        static readonly string[] ZoneNames = { "", "Anniversary Bonfire", "Celebration Monument", "Anniversary Forest", "Viper Swamp", "Bleak Valley",
            "Fiery Desert", "Dragon Remains", "Collapsed Debris", "Timeless Shrine", "Dimensional Portal", "Annihilation Vortex", "Ascension Trail" };

        public static readonly Kit.CfgCache<BvCfg> Config = new Kit.CfgCache<BvCfg>(System.IO.Path.Combine("BleakValleyVillage", "bleakvalley.json"), BvCfg.Read, BvCfg.Default);
        public static BvCfg Cfg { get { return Config.Get(); } }
        public static bool Running { get { return Kit.Running(Cfg.Festival); } }

        static void Lua(Client.GameClient c, string call, uint msg = LuaMsg) { Kit.Lua(c, call, msg); }
        static void Msg(Client.GameClient c, string t) { Kit.Msg(c, t); }
        static int Arg(string[] a, int i, int def) { int v; return a != null && a.Length > i && int.TryParse(a[i], out v) ? v : def; }

        // ------------------------------------------------------------------ lifetime: officer/exit NPCs + quest credit
        static readonly Timer _timer;
        static BleakValleyVillageUi()
        {
            try { _timer = new Timer(_ => { try { Maintain(); } catch { } }, null, 5000, 30000); } catch { }
        }
        /// <summary>Called by FestivalBoxOpen at first use (starts the timer) - registers the Grit packs and breads.</summary>
        public static void RegisterBoxes()
        {
            foreach (uint id in new uint[] { 3600480, 3341478, 3341700, 3600492, 3600494, 3341509 })
                MayAdventure2023.FestivalBoxOpen.Register(id, c => OpenGritItem(c, id));
            Maintain();
        }
        static readonly Action<Client.GameClient, string, uint, string> _credit = OnQuestFinished;
        static readonly object _npcLock = new object();
        static Npc _officer, _exit;
        /// <summary>Keeps the Reputation Officer / Exit on map 12100 exactly while the festival runs, and the quest-credit
        /// listener chained on QuestEngine.FestivalQuestFinished (FestivalManager sets that delegate at start).</summary>
        public static void Maintain()
        {
            try
            {
                var cur = QuestEngine.QuestEngineCore.FestivalQuestFinished;
                if (cur == null || !cur.GetInvocationList().Contains(_credit)) QuestEngine.QuestEngineCore.FestivalQuestFinished = cur + _credit;
            }
            catch { }
            lock (_npcLock)
            {
                bool run = Running;
                if (run && (_officer == null || _exit == null)) { _officer = _officer ?? Place(OfficerId, OfficerMesh, OfficerX, OfficerY, "ReputationOfficer"); _exit = _exit ?? Place(ExitId, ExitMesh, ExitX, ExitY, "Exit"); }
                else if (!run && (_officer != null || _exit != null)) { Remove(_officer); Remove(_exit); _officer = _exit = null; }
            }
        }
        static Npc Place(uint id, uint mesh, ushort x, ushort y, string name)
        {
            try
            {
                if (Pool.ServerMaps == null) return null;
                var map = Pool.ServerMaps[BleakMap];
                if (map == null) return null;
                if (map.View.MapContain(Role.MapObjectType.Npc, id, p => p.DynamicID == 0)) return null;
                var np = Npc.Create();
                np.UID = id; np.NpcType = (Role.Flags.NpcType)2; np.Mesh = mesh; np.Map = BleakMap; np.X = x; np.Y = y; np.Name = name;
                map.AddNpc(np);
                return np;
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return null; }
        }
        static void Remove(Npc np)
        {
            if (np == null) return;
            try
            {
                Role.GameMap map;
                if (Pool.ServerMaps != null && Pool.ServerMaps.TryGetValue(BleakMap, out map) && map != null)
                    using (var rec = new ServerSockets.RecycledPacket()) map.RemoveNpc(np, rec.GetStream());
            }
            catch { }
        }
        static void OnQuestFinished(Client.GameClient c, string key, uint quest, string given)
        {
            try
            {
                if (c == null || c.Player == null || !string.Equals(key, Festival, StringComparison.OrdinalIgnoreCase)) return;
                if (BleakValleyCore.CreditQuest(Cfg, c.Player.UID, quest))
                    Msg(c, "Your Reputation in Foison Village rose by " + Cfg.RepPerQuest + " (now " + BleakValleyCore.Rep(c.Player.UID) + ").");
            }
            catch { }
        }

        // ------------------------------------------------------------------ window callbacks (10217 / 10218)
        /// <summary>LuaPush.HandleCallback (msg 10217 and 10218). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func) || (msgId != LuaMsg && msgId != CommonMsg)) return false;
            bool mine = func.StartsWith("AnniversaryInterface_", StringComparison.Ordinal) || func.StartsWith("AniversaryInterface_", StringComparison.Ordinal)
                        || (msgId == CommonMsg && func == "AniversaryNpc_LinkRank");
            if (!mine) return false;
            if (Kit.Throttled(Festival, c.Player.UID, 350)) return true;
            Maintain();
            var cfg = Cfg;
            if (!Running) { Msg(c, TxtOver); return true; }
            if (c.Player.Level < cfg.MinLevel) { Msg(c, TxtLevel); return true; }
            uint uid = c.Player.UID;
            try
            {
                if (msgId == CommonMsg)
                {
                    switch (func)
                    {
                        case "AnniversaryInterface_Phase":
                            {
                                int act = Arg(args, 0, 0), tier = Arg(args, 1, 0);
                                if (act != CommonMsg) return true;
                                var r = BleakValleyCore.ClaimCum(cfg, uid, c.Player.Name, new ClientBag(c), tier);
                                if (!r.Ok) { if (!string.IsNullOrEmpty(r.Error)) Msg(c, r.Error); }
                                else { Kit.Log(Festival, "points", c, "tier " + tier + " -> " + string.Join(", ", r.Given)); Msg(c, "Congrats! You got " + Kit.Names(r.Given) + "!"); }
                                Lua(c, "DlgMidAutumnRank_UpdateReward|" + RankData(cfg, uid, false), CommonMsg);
                                return true;
                            }
                        case "AnniversaryInterface_OpenMainView":
                            Open(c, cfg, false);
                            return true;
                        case "AniversaryNpc_LinkRank":
                            ShowRank(c, cfg);
                            return true;
                    }
                    return true;
                }
                switch (func)
                {
                    case "AnniversaryInterface_MainView":
                        Open(c, cfg, Arg(args, 0, 0) == 1);
                        return true;
                    case "AnniversaryInterface_ChkDraw":
                        Enhance(c, cfg);
                        return true;
                    case "AnniversaryInterface_ClickMainView":
                        Click(c, cfg, Arg(args, 0, 0));
                        return true;
                    case "AnniversaryInterface_OpenForestView":
                        Msg(c, "Anniversary Forest isn`t open on this server: its map is not available in the client.");
                        return true;
                    case "AniversaryInterface_EmoneyShop":
                        Msg(c, "The Crest Treasury (Anniversary Store) isn`t open on this server: its goods are server data the client does not have.");
                        return true;
                    case "AnniversaryInterface_ClientMsg":
                        Msg(c, "Daily Quest: talk to the Reputation Officer in Bleak Valley (Celebration Realm -> Bleak Valley -> Go).");
                        return true;
                    case "AnniversaryInterface_ShopItemView":
                        Msg(c, "This item isn`t sold on this server.");
                        return true;
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            Msg(c, "This part of the Celebration Realm isn`t open on this server yet.");
            return true;
        }

        /// <summary>Main window (Code 100) with this player's real state.</summary>
        public static string OpenData(BvCfg cfg, uint uid, long sparkLogs, bool refresh)
        {
            long pts, grit, left;
            lock (BleakValleyCore.Store.Sync) { pts = BleakValleyCore.Points(uid); grit = BleakValleyCore.Grit(uid); left = BleakValleyCore.DragonLeft(uid); }
            int grade = cfg.Grade(pts);
            var sb = new StringBuilder(refresh ? "Dlg23AnniversaryMainUi_UpdateView|" : "Dlg23AnniversaryMainUi_MainOpen|");
            sb.Append("{['Code']=100,['Data']={['Score']=").Append(pts).Append(",['Grade']=").Append(grade).Append(",['Spirit']=").Append(grit).Append(",['Actlist']={");
            for (int i = 1; i <= 12; i++)
            {
                if (i > 1) sb.Append(',');
                bool open = cfg.ZoneOpen(i, grade);
                sb.Append("{['State']=").Append(open ? 2 : 0);
                if (i == 7 && open && left > 0) sb.Append(",['ActTime']=").Append(left);
                if (i == 7 && open && left == 0) sb.Append(",['IsFinish']=1");
                sb.Append('}');
            }
            sb.Append("},['ItemInfo']='").Append(cfg.SparkLog).Append(" 0 ").Append(Math.Max(0, sparkLogs)).Append("'}}");
            return sb.ToString();
        }
        static void Open(Client.GameClient c, BvCfg cfg, bool refresh)
        {
            lock (BleakValleyCore.Store.Sync) { BleakValleyCore.Touch(cfg, c.Player.UID, c.Player.Name); BleakValleyCore.Store.Commit(); }
            Lua(c, OpenData(cfg, c.Player.UID, Kit.Count(c, cfg.SparkLog), refresh));
        }

        static void Enhance(Client.GameClient c, BvCfg cfg)
        {
            var r = BleakValleyCore.Enhance(cfg, c.Player.UID, c.Player.Name, new ClientBag(c));
            if (!r.Ok) { Msg(c, r.Error); Open(c, cfg, true); return; }
            Kit.Log(Festival, "bonfire", c, "45 Spark Logs -> " + r.Value + " points" + (r.LevelUp ? " LEVEL UP" : "") + (r.Given.Count > 0 ? " + " + string.Join(", ", r.Given) : ""));
            Lua(c, "Dlg23AnniversaryMainUi_PlayFireEff|");
            if (r.LevelUp) Lua(c, "Dlg23AnniversaryMainUi_PlayUplevEff|");
            Open(c, cfg, true);
            Msg(c, "You added 45 Spark Logs to the Anniversary Bonfire and got " + cfg.PointsPerTry + " Anniversary Points." + (r.Given.Count > 0 ? " The bonfire left a Faith Crystal!" : ""));
            if (r.LevelUp) Msg(c, "The Anniversary Bonfire reached level " + cfg.Grade(r.Value) + "!");
        }

        static readonly HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1036, BleakMap };
        static void Click(Client.GameClient c, BvCfg cfg, int zone)
        {
            uint uid = c.Player.UID;
            int grade; long left;
            lock (BleakValleyCore.Store.Sync) { BleakValleyCore.Touch(cfg, uid, c.Player.Name); grade = cfg.Grade(BleakValleyCore.Points(uid)); left = BleakValleyCore.DragonLeft(uid); }
            if (zone == 1) { Enhance(c, cfg); return; }
            if (zone == 2) { Lua(c, "DlgMidAutumnRank_BindData|" + RankData(cfg, uid, true), CommonMsg); return; }
            if (zone < 3 || zone > 12) return;
            if (!cfg.ZoneOpen(zone, grade)) { Msg(c, "The fog is so heavy here! Upgrade the Anniversary Bonfire to disperse the fog."); return; }
            if (zone == 7)
            {
                if (left == 0)
                {
                    var r = BleakValleyCore.DragonClaim(cfg, uid, c.Player.Name, new ClientBag(c));
                    if (!r.Ok) { Msg(c, r.Error); return; }
                    Kit.Log(Festival, "dragon", c, "claim -> " + string.Join(", ", r.Given));
                    Lua(c, "Dlg23AnniversaryFrogUi_MainOpen|{['Code']=300,['Data']={['ShowIndex']=" + r.Show + ",['ItemList']={" + string.Join(",", r.Given.Select(p => "'" + p.ClientString() + "'")) + "}}}");
                    Open(c, cfg, true);
                    return;
                }
                var g = BleakValleyCore.DragonGo(cfg, uid, c.Player.Name);
                if (!g.Ok) { Msg(c, g.Error); return; }
                Msg(c, "Dragonette set out on an expedition. It will be back in " + (g.Value / 60) + " minutes.");
                Open(c, cfg, true);
                return;
            }
            if (zone == 5)
            {
                var p = c.Player;
                if (!p.Alive || p.DynamicID != 0 || !TeleportFrom.Contains(p.Map)) { Msg(c, "You can`t go to Bleak Valley from here. Go back to a city first."); return; }
                if (Pool.ServerMaps == null || Pool.ServerMaps[BleakMap] == null) { Msg(c, "Bleak Valley isn`t open yet."); return; }
                Maintain();
                var o = QuestEngine.QuestEngineCore.TeleportOverride;
                if (o != null) o(c, OfficerX, (ushort)(OfficerY + 3), BleakMap, 0); else c.Teleport(OfficerX, (ushort)(OfficerY + 3), BleakMap, 0);
                return;
            }
            Msg(c, ZoneNames[zone] + " isn`t open on this server: its map is not available in the client (see the festival README).");
        }

        // ------------------------------------------------------------------ Celebration Monument (DlgMidAutumnRank 10218)
        public static string RankData(BvCfg cfg, uint uid, bool full)
        {
            var no = new List<int>(); var can = new List<int>(); var got = new List<int>();
            int c9 = 0, e9 = 0;
            lock (BleakValleyCore.Store.Sync)
                for (int t = 1; t <= 9; t++)
                {
                    int a, b;
                    int st = BleakValleyCore.CumState(cfg, uid, t, out a, out b);
                    if (t == 9) { c9 = a; e9 = b; }
                    if (st == 2 && t < 9) got.Add(t); else no.Add(t);
                    if (st == 1) can.Add(t);
                }
            var sb = new StringBuilder("{['Code']=200,['Data']={['ActivityId']=10218,['NoClaim']={").Append(string.Join(",", no)).Append("},['Claimed']={").Append(string.Join(",", got))
                .Append("},['CanClaim']={").Append(string.Join(",", can)).Append("},['Frequency']={{9,").Append(c9).Append(',').Append(Math.Max(c9, e9)).Append("}}");
            if (full)
                sb.Append(",['IsOpen']=1,['IsBack']=1,['IsTime']=1,['IsRule']=0,['RankNum']=4,['LastDialog']=1,['IsPoint']=1,['PointProgress']={")
                  .Append(BleakValleyCore.Points(uid)).Append(',').Append(cfg.CumThresholds[7]).Append('}');
            return sb.Append("}}").ToString();
        }
        static void ShowRank(Client.GameClient c, BvCfg cfg)
        {
            var top = BleakValleyCore.Top(cfg, 10);
            var sb = new StringBuilder("Anniversary Revelry Rankings (Anniversary Points)\n");
            if (top.Count == 0) sb.Append("No one has Anniversary Points yet.\n");
            for (int i = 0; i < top.Count; i++) sb.Append(i + 1).Append(". ").Append(top[i].Key).Append("  ").Append(top[i].Value).Append('\n');
            sb.Append("Your Anniversary Points: ").Append(BleakValleyCore.Points(c.Player.UID));
            c.CreateBoxDialog(sb.ToString());
        }

        // ------------------------------------------------------------------ Grit packs / breads
        static bool OpenGritItem(Client.GameClient c, uint item)
        {
            var cfg = Cfg;
            if (!Running) { Msg(c, TxtOver); return true; }
            var r = BleakValleyCore.OpenGritItem(cfg, c.Player.UID, c.Player.Name, new ClientBag(c), item);
            if (!r.Ok) { if (!string.IsNullOrEmpty(r.Error)) Msg(c, r.Error); return true; }
            Kit.Log(Festival, "grit-item", c, Kit.ItemName(item) + " -> " + (r.Given.Count > 0 ? string.Join(", ", r.Given) : "") + " grit=" + r.Value);
            Msg(c, r.Given.Count > 0 ? "Congrats! You got " + Kit.Names(r.Given) + "! Grit Points left: " + r.Value + "." : "Your Grit Points: " + r.Value + ".");
            return true;
        }

        // ------------------------------------------------------------------ NPC scripts (Bleak Valley)
        const byte OptAccept = 1, OptRep = 2, OptHome = 3, OptClaim = 4, OptBuyAsk = 5, OptBuyYes = 6, OptBack = 7;
        sealed class Stage { public int Menu; public DateTime Until; }
        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, Stage> _stage = new System.Collections.Concurrent.ConcurrentDictionary<uint, Stage>();
        static void Arm(uint uid, int menu) { _stage[uid] = new Stage { Menu = menu, Until = DateTime.UtcNow.AddMinutes(2) }; }
        static bool Armed(uint uid, int menu) { Stage s; return _stage.TryGetValue(uid, out s) && s.Menu == menu && s.Until > DateTime.UtcNow; }
        static void Texts(Dialog d, string text)
        {
            text = text ?? "";
            while (text.Length > 0) { int n = Math.Min(text.Length, 200); d.AddText(text.Substring(0, n)); text = text.Substring(n); }
        }

        [NpcAttribute((NpcID)OfficerId)]
        public static void ReputationOfficer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try { Officer(client, stream, Option); } catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
        [NpcAttribute((NpcID)ExitId)]
        public static void BleakExit(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            try
            {
                var d = new Dialog(client, stream);
                if (Option == 0) { d.AddText("Go back to Twin City?").AddOption("Yes.", 1).AddOption("Stay a while.", 255).AddAvatar(10).FinalizeDialog(); return; }
                if (Option == 1 && client.Player.Map == BleakMap) client.Teleport(368, 441, 1002, 0);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static string Status(Client.GameClient c, int v)
        {
            foreach (uint q in BleakValleyCore.VillagerQuests[v])
            {
                if (c.Player.QuestGUI.IsActiveQuest(q)) return "Accepted";
                if (BleakValleyCore.AcceptedToday(c.Player.UID, q) && c.Player.QuestGUI.CheckQuest(q, MsgQuestList.QuestListItem.QuestStatus.Finished)) return "Completed";
            }
            return "Available";
        }
        /// <summary>Fallback credit: a village quest accepted here today and finished (the listener may have been missing).</summary>
        static void CreditFinished(Client.GameClient c, BvCfg cfg)
        {
            foreach (var arr in BleakValleyCore.VillagerQuests)
                foreach (uint q in arr)
                    if (BleakValleyCore.AcceptedToday(c.Player.UID, q) && c.Player.QuestGUI.CheckQuest(q, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && BleakValleyCore.CreditQuest(cfg, c.Player.UID, q))
                        Msg(c, "Your Reputation in Foison Village rose by " + cfg.RepPerQuest + ".");
        }

        static void Officer(Client.GameClient c, ServerSockets.Packet stream, byte opt)
        {
            if (c == null || c.Player == null) return;
            var cfg = Cfg;
            uint uid = c.Player.UID;
            var d = new Dialog(c, stream);
            if (!Running) { d.AddText(TxtOver).AddOption("See you next year!", 255).AddAvatar(10).FinalizeDialog(); return; }
            if (c.Player.Level < cfg.MinLevel) { d.AddText("Reputation Officer:\n" + TxtLevel).AddOption("Level up first.", 255).AddAvatar(10).FinalizeDialog(); return; }
            Maintain();
            lock (BleakValleyCore.Store.Sync) { BleakValleyCore.Touch(cfg, uid, c.Player.Name); BleakValleyCore.Store.Commit(); }
            CreditFinished(c, cfg);
            long rep = BleakValleyCore.Rep(uid), grit = BleakValleyCore.Grit(uid);
            int lvl = cfg.RepLevel(rep);
            if (opt == 0 || opt == OptBack)
            {
                Texts(d, "Reputation Officer:\nBleak Valley, a place where strong winds blow all year round. But there is a century-old village in the valley called Foison Village.\n");
                Texts(d, "1. Complete the reputation quest issued by the villagers, and you will get the quest reward and reputation points.\n");
                Texts(d, "2. Accumulate reputation points to get reputation rewards. The higher the reputation level, the higher the purchase discount, which can be used to buy Log Sale Packs.\n");
                Texts(d, "3. Accepting 1 reputation quest will cost 100 Grit Points.\n");
                var sb = new StringBuilder("[Villager] [Status]\n");
                for (int v = 0; v < 5; v++) sb.Append(BleakValleyCore.Villagers[v]).Append(": ").Append(Status(c, v)).Append('\n');
                Texts(d, sb.ToString());
                Texts(d, lvl < cfg.RepLevels.Length
                    ? "Your Reputation Points: " + rep + ", Reputation Level: " + lvl + ". You need " + (cfg.RepLevels[lvl] - rep) + " more point(s) of Reputation to upgrade.\n"
                    : "Your Reputation Points: " + rep + ", Reputation Level: " + lvl + "\n");
                Texts(d, "Your Grit Points: " + grit + "\n");
                d.AddOption("Accept the quest.", OptAccept).AddOption("View Reputation and rewards.", OptRep).AddOption("Return to Twin City.", OptHome).AddOption("Leave.", 255).AddAvatar(10).FinalizeDialog();
                Arm(uid, 1);
                return;
            }
            if (opt == OptAccept && Armed(uid, 1)) { AcceptQuest(c, d, cfg); return; }
            if (opt == OptHome && Armed(uid, 1)) { c.Teleport(368, 441, 1002, 0); return; }
            if (opt == OptRep && Armed(uid, 1))
            {
                var sb = new StringBuilder("[Level] [Reputation Pts Needed] [Reward] [Claiming Status]\n");
                for (int i = 1; i <= cfg.RepLevels.Length; i++)
                    sb.Append("   ").Append(i).Append("        ").Append(cfg.RepLevels[i - 1]).Append(" points of Reputation    ").Append(cfg.RepLogs[i - 1]).Append(" Spark Logs    ")
                      .Append(BleakValleyCore.Store.GetI(uid, "rl" + i) == 1 ? "Claimed" : lvl >= i ? "Claimable" : "Not reached").Append('\n');
                Texts(d, sb.ToString());
                Texts(d, "Your Reputation Level: " + lvl + ".\nThe higher the reputation level, the higher the purchase discount, which can be used to buy Log Sale Packs.\n");
                d.AddOption("Claim the reward.", OptClaim).AddOption("Buy Log Sale Packs.", OptBuyAsk).AddOption("Previous.", OptBack).AddAvatar(10).FinalizeDialog();
                Arm(uid, 2);
                return;
            }
            if (opt == OptClaim && Armed(uid, 2))
            {
                var r = BleakValleyCore.ClaimRep(cfg, uid, c.Player.Name, new ClientBag(c));
                if (!r.Ok) { d.AddText(r.Error).AddOption("I see.", 255).AddAvatar(10).FinalizeDialog(); return; }
                Kit.Log(Festival, "reputation", c, "level " + lvl + " -> " + string.Join(", ", r.Given));
                d.AddText("Congrats! You got the reputation reward! (" + Kit.Names(r.Given) + ")").AddOption("I see.", 255).AddAvatar(10).FinalizeDialog();
                return;
            }
            if (opt == OptBuyAsk && Armed(uid, 2))
            {
                uint price = BleakValleyCore.LogSalePrice(cfg, uid);
                d.AddText("Log Sale Pack: open to randomly get 40-50 Spark Logs.\nPrice for Reputation Level " + lvl + ": " + price + " CPs. Buy 1?")
                 .AddOption("Yes.", OptBuyYes).AddOption("Think twice.", 255).AddAvatar(10).FinalizeDialog();
                Arm(uid, 3);
                return;
            }
            if (opt == OptBuyYes && Armed(uid, 3))
            {
                _stage.TryRemove(uid, out _);
                var r = BleakValleyCore.BuyLogSale(cfg, uid, c.Player.Name, new ClientBag(c));
                if (!r.Ok) { d.AddText(r.Error).AddOption("I see.", 255).AddAvatar(10).FinalizeDialog(); return; }
                Kit.Log(Festival, "logsale", c, r.Value + " CPs -> " + string.Join(", ", r.Given));
                d.AddText("You`ve got a Log Sale Pack for " + r.Value + " CPs.").AddOption("Buy another.", OptBuyAsk).AddOption("I see.", 255).AddAvatar(10).FinalizeDialog();
                Arm(uid, 2);
                return;
            }
        }

        static void AcceptQuest(Client.GameClient c, Dialog d, BvCfg cfg)
        {
            uint uid = c.Player.UID;
            _stage.TryRemove(uid, out _);
            lock (Kit.Lock(Festival, uid))
            {
                lock (BleakValleyCore.Store.Sync)
                {
                    if (BleakValleyCore.Grit(uid) < cfg.QuestGrit)
                    {
                        Texts(d, "You don`t have 100 Grit Points. Here are some ways to get Grit Points.\n1. You`ll get 1 point automatically every 30 minutes.\n2. You`ll get 200 points automatically at 0:00 every day.\n3. Eating Anniversary Breads.\n");
                        d.AddOption("I see.", 255).AddAvatar(10).FinalizeDialog();
                        return;
                    }
                    BleakValleyCore.TakeGrit(uid, cfg.QuestGrit);// paid first; refunded below when no quest could be accepted
                    BleakValleyCore.Store.Commit();
                }
                string name = null, why = null;
                uint got = 0;
                for (int v = 0; v < 5 && got == 0; v++)
                {
                    if (Status(c, v) != "Available") continue;
                    foreach (uint q in BleakValleyCore.VillagerQuests[v])
                    {
                        if (c.Player.QuestGUI.CheckQuest(q, MsgQuestList.QuestListItem.QuestStatus.Finished) && BleakValleyCore.AcceptedToday(uid, q)) continue;
                        if (QuestEngine.QuestEngineCore.AcceptAtNpc(c, 36555, q, out name, out why)) { got = q; break; }
                    }
                }
                if (got == 0)
                {
                    lock (BleakValleyCore.Store.Sync) { BleakValleyCore.Store.Add(uid, "gp", cfg.QuestGrit); BleakValleyCore.Store.Commit(); }
                    Texts(d, "Villagers have no quests at present. Please come back later.\n" + (string.IsNullOrEmpty(why) ? "" : "(" + why + ")"));
                    d.AddOption("I see.", 255).AddAvatar(10).FinalizeDialog();
                    return;
                }
                BleakValleyCore.MarkAccepted(uid, got);
                Kit.Log(Festival, "accept", c, "quest " + got + " for " + cfg.QuestGrit + " Grit Points");
                Texts(d, "Thank you! " + (name ?? "The quest") + " - the villager is waiting for you. (100 Grit Points used, " + BleakValleyCore.Grit(uid) + " left.)\n");
                d.AddOption("With pleasure.", 255).AddAvatar(10).FinalizeDialog();
            }
        }
    }
}
