// ============================================================================
//  QahrMobile: the mobile main story ("World of Conquer" 13507 -> 13508 -> ...), driven the way the mobile client
//  expects it (findings: D:\conquer\conquer-mobile\docs\quests\quest-guide.md).
//
//  Table: Data\mobile_quests\main_story.txt (tools\mainstory_table.py from the APK questinfo.ini + npcdialogueinfo.lua),
//  re-read when the file changes: id, next, finish NPC id/map/x/y, goal count, first dialog id, lua func/target.
//
//  1. Login (mobile only): the first main-story quest that is not finished is accepted if the player has not got it
//     -> MsgTaskStatus 2126 action 1 {id, status 0} -> the mobile puts it in the quest tracker (PostCmd 0xc42).
//  2. Tracker / goal link click: the mobile auto-runs to the quest NPC and calls the server Lua function
//     "</F>ClientTaskTemplate_QuestinfoFunc</N><id>" (MsgAction 258, CMsgMBAction 10046). Near the finish NPC with the
//     goal done -> "Dlg_OpenPlot|{['id']=<id*10+1>}" opens the quest's story dialog (DlgNpcDialogue); far -> auto-path
//     (AutoPatcher) + a hint.
//  3. "</F>ClientTaskTemplate_PlayPlot</N><plot>|<next>|<cur>": the scene script is skipped, the next dialog is opened.
//  4. Dialog closed ("</F>DlgOverSend</S>{Type=1,StartId=..}") or skipped ("</F>ClientTaskTemplate_CompleteMain</N><id>")
//     -> the quest is finished (2126 action 4) and the next one accepted (2126 action 1).
//  Never trusts the client: the quest must be active, the player near the finish NPC spot, and a kill goal counted
//  by the server (quest engine kill counters); talk / tutorial steps ("WearEquipment" ...) complete at the NPC.
//  Main-story prizes are empty in the mobile data: MobileQuestStory pays the reward columns of main_story.txt.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Qahr.Mobile.Phone
{
    public static class MobileQuestGuide
    {
        public const uint FirstQuest = 13507;
        const int NearCells = 18;

        public class Row
        {
            public uint Id, Next, Npc, Map, X, Y, Count, Dialog, LvMin;
            public string Func = "", Target = "", NpcName = "", Name = "";
        }

        static volatile Dictionary<uint, Row> table;
        static DateTime tableTime;
        static readonly object loadLock = new object();
        static string FindFile()
        {
            // DbLocation = the Data folder (as Data\mobile_npclooks.txt)
            string p = System.IO.Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_quests", "main_story.txt");
            return File.Exists(p) ? p : null;
        }

        public static Dictionary<uint, Row> Table
        {
            get
            {
                string f = FindFile();
                if (f == null) return table ?? new Dictionary<uint, Row>();
                DateTime t = File.GetLastWriteTimeUtc(f);
                if (table != null && t == tableTime) return table;
                lock (loadLock)
                {
                    if (table != null && t == tableTime) return table;
                    var d = new Dictionary<uint, Row>();
                    try
                    {
                        foreach (var line in File.ReadAllLines(f))
                        {
                            if (line.Length == 0 || line[0] == '#') continue;
                            var p = line.Split('\t');
                            if (p.Length < 13) continue;
                            var r = new Row
                            {
                                Id = U(p[0]), Next = U(p[1]), Npc = U(p[2]), Map = U(p[3]), X = U(p[4]), Y = U(p[5]),
                                Count = Math.Max(1, U(p[6])), Dialog = U(p[7]), Func = p[8], Target = p[9], NpcName = p[10],
                                LvMin = U(p[11]), Name = p[12]
                            };
                            if (r.Id != 0 && !d.ContainsKey(r.Id)) d[r.Id] = r;
                        }
                        MyConsole.WriteLine("[Mobile] main story: " + d.Count + " quests from " + f, ConsoleColor.Cyan);
                    }
                    catch (Exception e) { MyConsole.WriteLine("[Mobile] main story load failed: " + e.Message, ConsoleColor.Red); }
                    table = d; tableTime = t;
                    return d;
                }
            }
        }
        static uint U(string s) { uint v; return uint.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        // ------------------------------------------------------------------ 1. login
        /// <summary>MsgLoginHandler (LoginFull): mobile players always have the current main-story quest.</summary>
        public static void OnLoginFull(Client.GameClient client)
        {
            try
            {
                if (client == null || client.Player == null || !MobileEvents.IsMobile(client)) return;
                if (MobileQuestStory.Rewards.Count == 0) { }// loads the reward columns (+ main_story_check.txt) once
                if (MobileQuestStory.SkipAutoStart(client)) return;// level 70+ / reborn with no story quest: only on request
                EnsureMainQuest(client);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        /// <summary>Walks the chain from 13507: the first quest that is not finished is accepted (if not yet).
        /// Returns the current quest id (0 = chain done / blocked).</summary>
        public static uint EnsureMainQuest(Client.GameClient client)
        {
            var t = Table;
            var gui = client.Player.QuestGUI;
            uint id = FirstQuest;
            var seen = new HashSet<uint>();
            while (id != 0 && seen.Add(id))
            {
                Row r;
                if (!t.TryGetValue(id, out r)) return 0;
                if (gui.CheckQuest(id, Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished)) { id = r.Next; continue; }
                if (gui.IsActiveQuest(id)) return id;
                Database.QuestInfo.DBQuest dbq;
                if (!Database.QuestInfo.AllQuests.TryGetValue(id, out dbq)) return 0;// quest-server has not imported it
                if (r.LvMin > 0 && client.Player.Level < r.LvMin) return 0;
                if (gui.src.ContainsKey(id)) return 0;// another status (e.g. failed): leave it alone
                gui.Accept(dbq, 0);// -> 2126 action 1 {id, 0}: the tracker shows it
                return id;
            }
            return 0;
        }

        // ------------------------------------------------------------------ 2-4. Lua calls from the client
        /// <summary>MsgDataPacket AnimaCrest (MsgAction 258 from the client): true = handled here.</summary>
        public static bool OnLuaCall(Client.GameClient client, string[] strings)
        {
            if (client == null || client.Player == null || strings == null) return false;
            if (MobileQuestStoryNpc.BountyCall(client, strings)) return true;// Chinju daily bounty board (TaskAssemblyGfit_*) + 13920 counter
            if (MobileQuestStoryNpc.OnWindowCall(client, strings)) return true;// Battle Goal / Lineage "open window" steps
            foreach (var s0 in strings)
            {
                if (string.IsNullOrEmpty(s0)) continue;
                string s = s0.Trim();
                try
                {
                    if (s.StartsWith("</F>ClientTaskTemplate_QuestinfoFunc", StringComparison.Ordinal))
                    { QuestClick(client, Arg(s, 1)); return true; }
                    if (s.StartsWith("</F>ClientTaskTemplate_PlayPlot", StringComparison.Ordinal))
                    {
                        uint next = Arg(s, 2);
                        if (next != 0) OpenPlot(client, next);
                        else { uint cur = Arg(s, 3) / 10; if (Table.ContainsKey(cur)) TryComplete(client, cur, true); }// "PlayPlot</N>546</N>0</N>138921": scene is the last part of the step's dialog (13892)
                        return true;
                    }
                    if (s.StartsWith("</F>ClientTaskTemplate_CompleteMain", StringComparison.Ordinal))
                    {
                        uint id = Arg(s, 1);
                        if (Table.ContainsKey(id)) TryComplete(client, id, true);
                        return true;
                    }
                    if (s.StartsWith("</F>DlgOverSend", StringComparison.Ordinal))
                    {
                        uint type = Field(s, "Type"), start = Field(s, "StartId");
                        uint id = start / 10;
                        if (type == 1 && Table.ContainsKey(id)) { TryComplete(client, id, true); return true; }
                        return Table.ContainsKey(id);
                    }
                }
                catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return true; }
            }
            return false;
        }

        /// <summary>"</F>Func</N>a</N>b" -> argument n (1-based) as a number.</summary>
        static uint Arg(string s, int n)
        {
            var p = s.Split(new[] { "</N>" }, StringSplitOptions.None);
            if (p.Length <= n) return 0;
            var m = Regex.Match(p[n], @"\d+");
            return m.Success ? U(m.Value) : 0;
        }
        static uint Field(string s, string key)
        {
            var m = Regex.Match(s, @"\b" + key + @"\W{0,4}=\s*(\d+)");
            return m.Success ? U(m.Groups[1].Value) : 0;
        }

        static bool Near(Client.GameClient c, Row r)
        {
            if (r.Map == 0) return true;
            if (c.Player.Map != r.Map) return false;
            int dx = Math.Abs((int)c.Player.X - (int)r.X), dy = Math.Abs((int)c.Player.Y - (int)r.Y);
            return Math.Max(dx, dy) <= NearCells;
        }

        static bool IsMonster(string name)
        {
            if (string.IsNullOrEmpty(name) || Pool.MonsterFamilies == null) return false;
            string n = name.Replace(" ", "").ToLowerInvariant();
            foreach (var f in Pool.MonsterFamilies.Values)
                if (f != null && f.Name != null && f.Name.Replace(" ", "").ToLowerInvariant() == n) return true;
            return false;
        }

        /// <summary>Kill goals must be counted by the server; talk / tutorial steps are done at the NPC.</summary>
        static bool GoalDone(Client.GameClient c, Row r, out string left)
        {
            left = null;
            if (!IsMonster(r.Target)) return true;
            uint sm; ushort sx, sy; int sd;// a monster type that spawns nowhere (e.g. Demon 2420) cannot block the chain
            if (!MobileQuestStory.NearestSpot(r.Target, r.Map, (int)r.X, (int)r.Y, out sm, out sx, out sy, out sd)) return true;
            uint[] cur;
            c.Player.QuestGUI.GetQuestObjectives(r.Id, out cur);
            uint have = cur != null && cur.Length > 0 ? cur[0] : 0;
            if (have >= r.Count) return true;
            left = "Defeat " + r.Target + " " + have + "/" + r.Count + ".";
            return false;
        }

        static void QuestClick(Client.GameClient c, uint id)
        {
            Row r;
            if (!Table.TryGetValue(id, out r)) return;
            if (MobileQuestStory.Rewards.Count == 0) { }// reward columns (+ main_story_check.txt) up to date
            var gui = c.Player.QuestGUI;
            if (!gui.IsActiveQuest(id)) { EnsureMainQuest(c); return; }
            string left0;
            if (!GoalDone(c, r, out left0) && MobileQuestStory.KillHint(c, r.Name, r.Target, left0)) return;// kills missing: go to the monsters
            if (!Near(c, r))
            {
                c.SendSysMesage("[Quest] " + r.Name + ": go to " + r.NpcName + " (" + r.X + "," + r.Y + ").", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                try { gui.SendAutoPatcher(r.Map, (ushort)r.X, (ushort)r.Y, r.Npc); } catch { }
                return;
            }
            string left;
            if (!GoalDone(c, r, out left)) { c.SendSysMesage("[Quest] " + r.Name + ": " + left, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); return; }
            if (r.Dialog != 0) OpenPlot(c, r.Dialog);
            else TryComplete(c, id, false);
        }

        static void OpenPlot(Client.GameClient c, uint dialog)
        {
            string name = (c.Player.Name ?? "").Replace("'", "").Replace("\\", "");
            LuaUi.LuaPush.Send(c, "Dlg_OpenPlot|{['id']=" + dialog + ",['PlayerName']='" + name + "'}");
        }

        public static bool TryComplete(Client.GameClient c, uint id, bool fromDialog)
        {
            Row r;
            if (!Table.TryGetValue(id, out r)) return false;
            var gui = c.Player.QuestGUI;
            if (!gui.IsActiveQuest(id) || !Near(c, r)) return false;
            string left;
            if (!GoalDone(c, r, out left)) return false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                uint[] cur;
                gui.GetQuestObjectives(id, out cur);
                if (cur == null || cur.Length == 0 || cur[0] < r.Count)
                    gui.SetQuestObjectives(stream, id, r.Count);// tracker "1/1" before it goes
                if (!MobileQuestStory.HasSpace(c, id)) return false;// bag room for the step's items
                if (!gui.FinishQuest(id)) return false;// Accepted -> Finished once (2126 action 4)
                MobileQuestStory.Pay(c, id, stream);// main_story.txt reward columns
            }
            Log(c, id, fromDialog ? "dialog" : "click");
            EnsureMainQuest(c);// accepts r.Next (2126 action 1)
            return true;
        }

        static void Log(Client.GameClient c, uint id, string how)
        {
            try
            {
                File.AppendAllText(System.IO.Path.Combine(Program.ServerConfig.DbLocation, "questengine_log.txt"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | MAINSTORY | " + c.Player.UID + " " + c.Player.Name + " finished " + id + " (" + how + ")" + Environment.NewLine);
            }
            catch { }
        }
    }
}
