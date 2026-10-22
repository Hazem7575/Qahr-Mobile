// ============================================================================
//  [JadeRabbit 2026-10-14] the Jade Rabbit voyage areas of the official Mid-Autumn 2024 "Lunar Voyage" window
//  (FuiZhongQiu2024Main, client msg 10459, owned by Base\MidAutumn2024\LunarVoyage.cs + festival MidAutumnVillage).
//
//  Official evidence (client):
//    area titles  Cn_Res STR_FUI_ZHONGQIU2024_TIP_TITLE_1..6 = Rainbow Bay, Ring Mountain, Stormy Ocean, Summer Lake,
//                 Galaxy Palace, Peak of Eternity
//    endings      STR_FGUI_150138_1_1 Dreamy Rainbow (paint Rainbow Bay)       = Questinfo 8066 "Rainbow Bay"   map 1003074
//                 STR_FGUI_150138_2_1 Bound Foes (help it against Flame Fox)  = Questinfo 8067 "Ring Mountain" map 1003075
//                 STR_FGUI_150138_6_1 Lunar Guardian (clean the trash)        = Questinfo 8073 "Clean Trash"   map 1003078 PeakOfEternity
//    areas 3-5    Server_Key tPlotEditorPlotEvent_DelText[8068..8072] exist, but Questinfo has no 8068-8072: not open here.
//    messages     Server_Key tMidAutumnAdventure_DelText[Sys][1] "Please return to Twin City first.",
//                 [Sys][4] "Jade Rabbit has already completed the exploration of this area. No need to go there again."
//  Window data: Cmd_FuiZhongQiu2024Main_Open List[i].Reward[1][1] = 1 when the area's quest is finished (ending achieved).
//  NOTE (client, fuizhongqiu2024mainproxy InitView): the client recomputes List[i].Lock itself - an area stays greyed until
//  every earlier area has its first ending. With areas 3-5 not open, area 6 can't be clicked in the window. GoTo 6 still
//  works on the server, and its quest can also be taken at JadeRabbit on 1003078.
//
//  Hook: Base\MidAutumn2024\LunarVoyage.cs (GoTo 1..6 and the Open data), tagged [JadeRabbit 2026-10-14].
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.JadeRabbit
{
    public static class JadeRabbitUi
    {
        public const string Festival = "JadeRabbit";

        public sealed class Area
        {
            public int Index; public uint Quest, Map; public ushort X, Y; public string Arrive;
        }
        /// <summary>Window area -> quest, map and arrival cell (walkable, a few cells from JadeRabbit). Official ids/points.</summary>
        public static readonly Area[] Areas =
        {
            new Area { Index = 1, Quest = 8066, Map = 1003074, X = 92, Y = 40, Arrive = "Jade Rabbit has come to the Flower Garden. Go ask it what it wants to do!" },
            new Area { Index = 2, Quest = 8067, Map = 1003075, X = 46, Y = 49, Arrive = "Jade Rabbit has encountered its nemesis, Flame Fox, but Jade Rabbit is no match for the fox. Go help it!" },
            new Area { Index = 6, Quest = 8073, Map = 1003078, X = 152, Y = 155, Arrive = "The once beautiful Peak of Eternity is littered with trash. Go discuss a solution with Jade Rabbit!" },
        };
        public static Area Find(int index) { return Areas.FirstOrDefault(a => a.Index == index); }

        public static bool Running { get { try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        static bool Finished(Client.GameClient c, uint q)
        {
            try { return c != null && c.Player != null && c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; }
        }
        static bool Taken(Client.GameClient c, uint q)
        {
            try { return c.Player.QuestGUI.src.ContainsKey(q); } catch { return false; }
        }

        /// <summary>LunarVoyage Open data: 1 = the first ending of this area (1-6) is achieved (its quest is finished).</summary>
        public static int EndingDone(Client.GameClient c, int area) { return EndingDone(c, area, 1); }
        /// <summary>[JadeRabbit 2026-10-15] ending j (1 first, 2 hidden) of the area: reached at the exploration terminus
        /// (JadeRabbitVoyage), or the area's story quest finished.</summary>
        public static int EndingDone(Client.GameClient c, int area, int ending)
        {
            if (c == null || c.Player == null) return 0;
            if (JadeRabbitVoyage.EndingDone(c.Player.UID, area, ending) == 1) return 1;
            var a = Find(area);
            return ending == 1 && a != null && Finished(c, a.Quest) ? 1 : 0;
        }
        /// <summary>[JadeRabbit 2026-10-15] the "Ending Reward Preview" item of the window (first reward of the ending).</summary>
        public static string EndingPreview(int area, int ending)
        {
            var cfg = JadeRabbitVoyage.Cfg;
            string[] l = ending == 2 ? (area >= 4 && area - 4 < cfg.HiddenRewards.Length ? cfg.HiddenRewards[area - 4] : null) : (area - 1 < cfg.EndingRewards.Length ? cfg.EndingRewards[area - 1] : null);
            return l != null && l.Length > 0 ? l[0] : "";
        }

        static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        static readonly ConcurrentDictionary<uint, long> _lastGo = new ConcurrentDictionary<uint, long>();

        /// <summary>LunarVoyage GoTo 1..6 (MainNpc2024_InterfaceGoto). Always answers.</summary>
        public static void Goto(Client.GameClient c, int area, int minLevel, HashSet<uint> teleportFrom)
        {
            var a = Find(area);
            if (a == null || !Running) { Msg(c, "The Lunar Voyage of Jade Rabbit isn`t open on this server yet."); return; }
            var p = c.Player;
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_lastGo.TryGetValue(p.UID, out prev) && now - prev >= 0 && now - prev < 3000) return;
            _lastGo[p.UID] = now;
            if (Finished(c, a.Quest)) { Msg(c, "Jade Rabbit has already completed the exploration of this area. No need to go there again."); return; }
            if (p.Level < minLevel) { Msg(c, "You need to reach Level " + minLevel + " to join the Mid-Autumn event."); return; }
            bool onArea = p.Map == a.Map && p.DynamicID == 0;
            bool fromOk = Areas.Any(x => x.Map == p.Map) || (teleportFrom != null && teleportFrom.Contains(p.Map));
            if (!p.Alive || p.DynamicID != 0 || !fromOk) { Msg(c, "Please return to Twin City first."); return; }

            var t = QuestEngine.QuestEngineCore.Current;
            QuestEngine.EngineQuest q;
            if (t == null || !t.Quests.TryGetValue(a.Quest, out q) || q.Disabled) { Msg(c, "The Lunar Voyage of Jade Rabbit isn`t open on this server yet."); return; }
            if (!Taken(c, a.Quest))
            {
                string name, why;
                if (!QuestEngine.QuestEngineCore.AcceptAtNpc(c, q.BeginNpc, a.Quest, out name, out why))
                {
                    Msg(c, "[Quest] " + (why ?? "This quest is not available."));
                    return;
                }
                Msg(c, "[Quest] New quest: " + name + ".");
            }
            if (onArea) return;
            if (Pool.ServerMaps[a.Map] == null) { Msg(c, "The Lunar Voyage of Jade Rabbit isn`t open on this server yet."); return; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, a.X, a.Y, a.Map, 0); else c.Teleport(a.X, a.Y, a.Map, 0);
            Msg(c, a.Arrive);
        }
    }
}
