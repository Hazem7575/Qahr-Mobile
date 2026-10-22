// ============================================================================
//  [WenwenDetective 2026-10-14] the official Children's Day 2024 "Goosie Adventure" window (client msg 10406
//  tMsg_Info["DlgChildrenDay"], luaui\dialog\dlgchildrenday, Dialog_Info 1490, EN button "Goosie Adventure").
//  Shown while the festival WenwenDetective (quests 5847-5852, Titan Realm) runs. The button DlgChildrenDayBtn is pushed by
//  festival.json icon_show / icon_hide (FestivalManager / FestivalIcons). The button is shared with JourneyOfWonder, whose maps are
//  still unresolved, so WenwenDetective owns it for now.
//
//  Official client:
//    click     ChildrenDayNpc2024_OpenMain -> DlgChildrenDayMain_MainOpen|{Code=200,Data={Score,IslandList={4 x state},
//              ItemInfo='3600709 0 n',DuckList={3 x {Pos,State}},OpenIndex,ScoreTip}}
//              IslandList: 1 Goose Park, 2 Dinowolf Isle, 3 Titan Realm, 4 Titan Vault; state 1 locked, 2 unlocking, 3 unlocked, 4 red dot.
//    island    ChildrenDayNpc2024_Click</N>i ; DlgItemGet: TrialGround_EnterMainMap (Goose Park), GiantIsland_EnterGiantIsland (Titan Realm)
//    other     TheGoslingTrial_ExecuteTask</N>duck</N>state, ChildrenDayNpc2024_ClickTeng/ClickJuRen</N>1, ChildrenDayNpc2024_Msg,
//              ChildrenDayBigKu_OpenScoreGet</N>n (points ranking)
//  On this server: island 3 Titan Realm = the Wenwen quest chain 5847-5852 (Questinfo map name TitanRealm, maps 1002614-1002619).
//  Islands 1/2/4, egg snatching, goosie trials, points, ranking and the draw have no official server data: polite messages,
//  Score 0, no ducks. ItemInfo = the real Bubble Hammer (3600709) count of the bag.
//
//  Hook: Base\LuaUi\LuaPush.cs HandleCallback (msg 10406), tagged [WenwenDetective 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.WenwenDetective
{
    public static class WenwenDetectiveUi
    {
        public const uint LuaMsg = 10406;
        public const string Festival = "WenwenDetective";
        public const int MinLevel = 80;// official tChildrenDayNpc2024[33009][Text115] "Requirement: Level 80 or above"
        public const uint BubbleHammer = 3600709;
        public static readonly HashSet<uint> TeleportFrom = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020 };

        public sealed class Step
        {
            public uint Quest, Map; public ushort X, Y; public string Arrive;
        }
        /// <summary>The chain in official order: quest, its map (official id), an arrival cell next to its NPC, official MsgBox1.</summary>
        public static readonly Step[] Chain =
        {
            new Step { Quest = 5847, Map = 1002614, X = 193, Y = 116, Arrive = "That goosie seems puzzled, maybe it has encountered some trouble. Go and ask what`s wrong!" },
            new Step { Quest = 5848, Map = 1002615, X = 215, Y = 185, Arrive = "The goosie needs you urgently. Go and ask what`s wrong!" },
            new Step { Quest = 5849, Map = 1002616, X = 150, Y = 229, Arrive = "The gooise seems to have found new clues. Hurry up and chat with it!" },
            new Step { Quest = 5850, Map = 1002617, X = 150, Y = 229, Arrive = "The goosie has some results from the feather research. Go talk to it!" },
            new Step { Quest = 5851, Map = 1002618, X = 182, Y = 300, Arrive = "That mysterious figure has appeared! Go and take a look" },
            new Step { Quest = 5852, Map = 1002619, X = 176, Y = 293, Arrive = "Finally finished drawing this map home, now give it to the lost goosie!" },
        };

        /// <summary>[2026-10-15] Journey of Wonder (merged JourneyOfWonder): island 2 Dinowolf Isle, the two cave chapters (Questinfo 5861/5862).</summary>
        public static readonly Step[] Journey =
        {
            new Step { Quest = 5861, Map = 1002628, X = 33, Y = 47, Arrive = "This goosie actually fell into such a cave. Hurry up and find it to see what happened!" },
            new Step { Quest = 5862, Map = 1002629, X = 34, Y = 52, Arrive = "A goosie is lost in thought. Go and ask it what`s wrong!" },
        };

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static bool Finished(Client.GameClient c, uint q)
        {
            try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; }
        }
        static bool Taken(Client.GameClient c, uint q)
        {
            try { return c.Player.QuestGUI.src.ContainsKey(q); } catch { return false; }
        }
        static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        /// <summary>The first chain quest not finished yet (null = the whole case is solved).</summary>
        public static Step Current(Client.GameClient c) { return Chain.FirstOrDefault(s => !Finished(c, s.Quest)); }
        public static Step CurrentJourney(Client.GameClient c) { return Journey.FirstOrDefault(s => !Finished(c, s.Quest)); }

        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static readonly ConcurrentDictionary<uint, long> _lastGo = new ConcurrentDictionary<uint, long>();
        static bool Throttled(ConcurrentDictionary<uint, long> map, uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (map.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            map[uid] = now;
            return false;
        }
        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static void Lua(Client.GameClient c, string call) { LuaUi.LuaPush.Send(c, call, LuaMsg); }

        /// <summary>True = answered (LuaPush.HandleCallback, msgId 10406).</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
            if (func == "ChildrenDayNpc2024_Msg") return true;// the client asks for a tip after the vine animation: nothing to say
            if (Throttled(_last, c.Player.UID, 400)) return true;
            if (!Running)
            {
                Msg(c, "The happy Goosie Adventure has come to an end. Looking forward to all of them growing up into excellent adult geese!");
                return true;
            }
            int n;
            switch (func)
            {
                case "ChildrenDayNpc2024_OpenMain":
                    if (c.Player.Level < MinLevel) { Msg(c, "Requirement: Level " + MinLevel + " or above."); return true; }
                    Lua(c, MainData(c));
                    return true;
                case "ChildrenDayNpc2024_Click":
                    if (args.Length == 0 || !int.TryParse(args[0], out n)) n = 0;
                    if (n == 3) TitanRealm(c);
                    else if (n == 1) Msg(c, "Goose Park (egg snatching, Yoyo`s questions, goosie trials) isn`t open on this server: its NPCs and rules are not in the client.");
                    else if (n == 2) Go(c, Journey, CurrentJourney(c), "Dinowolf Isle", "You have helped the goosies of Dinowolf Isle. Thank you!");// [2026-10-15] Journey of Wonder
                    else if (n == 4) Lua(c, GoosieVault.DrawCall(c, "DlgChildrenDayDraw_MainOpen"));// [2026-10-15] Titan Vault
                    return true;
                case "GiantIsland_EnterGiantIsland":
                    TitanRealm(c);
                    return true;
                case "TrialGround_EnterMainMap":
                    Msg(c, "Goose Park isn`t open on this server yet.");
                    return true;
                case "TheGoslingTrial_ExecuteTask":
                    Msg(c, "The goosie trials on Dinowolf Isle aren`t open on this server yet.");
                    return true;
                case "ChildrenDayNpc2024_ClickTeng":
                    TitanRealm(c);
                    return true;
                case "ChildrenDayNpc2024_ClickJuRen":
                    if (c.Player.Level < MinLevel) { Msg(c, "Requirement: Level " + MinLevel + " or above."); return true; }
                    Lua(c, GoosieVault.DrawCall(c, "DlgChildrenDayDraw_MainOpen"));
                    return true;
                // [2026-10-15] Titan Vault golden eggs + Goosie Points stages (Base\WenwenDetective\GoosieVault.cs)
                case "ChildrenDayBigKu_Click":
                    if (c.Player.Level < MinLevel) { Msg(c, "Requirement: Level " + MinLevel + " or above."); return true; }
                    if (args.Length == 0 || !int.TryParse(args[0], out n)) n = 0;
                    GoosieVault.Smash(c, n);
                    return true;
                case "ChildrenDayBigKu_Refersh":
                    Lua(c, GoosieVault.DrawCall(c, "DlgChildrenDayDraw_UpdateView"));
                    return true;
                case "ChildrenDayBigKu_OpenScoreGet":
                    if (args.Length == 0 || !int.TryParse(args[0], out n)) n = 1;
                    Lua(c, "DlgMidAutumnRank_BindData|" + GoosieVault.StageData(c, n == 2 ? "DlgChildrenDayDraw" : "DlgChildrenDayMain"));
                    return true;
                case "ChildrenDayBigKu_GetStageReward":
                    {
                        int row; if (args.Length < 2 || !int.TryParse(args[1], out row)) { if (args.Length == 0 || !int.TryParse(args[0], out row)) row = 0; }
                        GoosieVault.ClaimStage(c, row);
                        return true;
                    }
                case "ChildrenDayBigKu_GoRank":
                    GoosieVault.Ranking(c);
                    return true;
                case "ChildrenDayBigKu_Close":
                    return true;
            }
            return false;
        }

        public static string MainData(Client.GameClient c)
        {
            var step = Current(c);
            // Titan Realm: 4 (red dot) while a Wenwen chapter waits for the player, 3 (unlocked) once the case is solved
            int titan = step != null ? 4 : 3;
            int dino = CurrentJourney(c) != null ? 4 : 3;// [2026-10-15] Dinowolf Isle (Journey of Wonder)
            GoosiePlayer gp; lock (GoosieVault.Sync) gp = GoosieVault.Get(c);
            return "DlgChildrenDayMain_MainOpen|{['Code']=200,['Data']={['Score']=" + gp.Score + ",['IslandList']={1," + dino + "," + titan + ",3},['ItemInfo']='"
                + BubbleHammer + " 0 " + Count(c, BubbleHammer) + "',['DuckList']={},['ScoreTip']=0}}";
        }

        /// <summary>Island 3: the player's current Wenwen chapter (accepted at its begin NPC if needed) and its map.</summary>
        static void TitanRealm(Client.GameClient c) { Go(c, Chain, Current(c), "Titan Realm", "You have helped Wenwen solve the whole case. Thank you, detective!"); }

        /// <summary>The player's current chapter of a chain (accepted at its begin NPC if needed) and its map.</summary>
        static void Go(Client.GameClient c, Step[] chain, Step step, string place, string done)
        {
            var p = c.Player;
            if (Throttled(_lastGo, p.UID, 3000)) return;
            if (p.Level < MinLevel) { Msg(c, "Requirement: Level " + MinLevel + " or above."); return; }
            if (step == null) { Msg(c, done); return; }
            bool fromOk = TeleportFrom.Contains(p.Map) || Chain.Any(s => s.Map == p.Map) || Journey.Any(s => s.Map == p.Map);
            if (!p.Alive || p.DynamicID != 0 || !fromOk) { Msg(c, "Please return to Twin City first."); return; }
            var t = QuestEngine.QuestEngineCore.Current;
            QuestEngine.EngineQuest q;
            if (t == null || !t.Quests.TryGetValue(step.Quest, out q) || q.Disabled) { Msg(c, place + " isn`t open on this server yet."); return; }
            if (!Taken(c, step.Quest))
            {
                uint before = p.Map;
                string name, why;
                if (!QuestEngine.QuestEngineCore.AcceptAtNpc(c, q.BeginNpc, step.Quest, out name, out why))
                {
                    Msg(c, "[Quest] " + (why ?? "This quest is not available."));
                    return;
                }
                Msg(c, "[Quest] New quest: " + name + ".");
                if (p.Map != before) { Msg(c, step.Arrive); return; }// its onAccept already moved the player
            }
            if (p.Map == step.Map && p.DynamicID == 0) return;
            if (Pool.ServerMaps[step.Map] == null) { Msg(c, place + " isn`t open on this server yet."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, step.X, step.Y, step.Map, 0); else c.Teleport(step.X, step.Y, step.Map, 0);
            Msg(c, step.Arrive);
        }
    }
}
