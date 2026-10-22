// ============================================================================
//  QahrMobile: main-story steps finished by a plain NPC click (mobile players only).
//
//  When the player is far from the finish NPC, a tracker click auto-walks him there (MobileQuestGuide.QuestClick ->
//  SendAutoPatcher with the NPC id) and the mobile then CLICKS the NPC (MsgNpc). That click used to reach the quest
//  engine, which finished the step with its own generic dialog ("Complete: <name>") but paid no main_story.txt reward
//  and did not accept the next step: the tracker went empty (seen with 13508 "Twin City", 2026-10-21).
//
//  OnNpc (called first from QuestEngine.OnNpc, option 0): if the clicked NPC is the finish NPC of the player's ACTIVE
//  main-story step and the goal is done, the step goes the main-story way: the mobile story dialog (Dlg_OpenPlot) when
//  the step has one, else MobileQuestGuide.TryComplete (finish + reward + next step). Otherwise false: the engine /
//  hand-written handler answers as before.
//  OnEngineFinished (safety net, after the engine's own finish of an engine quest): a main-story step finished by the
//  engine still pays its reward and accepts the next step.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qahr.Mobile.Phone
{
    public static class MobileQuestStoryNpc
    {
        public static bool OnNpc(Client.GameClient c, uint npcid)
        {
            try
            {
                if (c == null || c.Player == null || !MobileEvents.IsMobile(c)) return false;
                var t = MobileQuestGuide.Table;
                var gui = c.Player.QuestGUI;
                foreach (var r in t.Values)
                {
                    if (r.Npc != npcid || !gui.IsActiveQuest(r.Id)) continue;
                    if (!GoalDone(c, r)) return false;// kills missing: the engine shows the progress
                    if (r.Dialog != 0)
                    {
                        string name = (c.Player.Name ?? "").Replace("'", "").Replace("\\", "");
                        LuaUi.LuaPush.Send(c, "Dlg_OpenPlot|{['id']=" + r.Dialog + ",['PlayerName']='" + name + "'}");
                        return true;// dialog end -> DlgOverSend -> MobileQuestGuide.TryComplete
                    }
                    return MobileQuestGuide.TryComplete(c, r.Id, false);
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return false;
        }

        /// <summary>
        /// Procesor.ExecuteNpcInvoker, before any range check: with the goal done, the mobile tracker click sends a
        /// plain NPC click (10084, option 0) to the finish NPC however far away it is. Far -> walk there (the client
        /// clicks again on arrival, and OnNpc finishes the step). True = handled.
        /// </summary>
        public static bool OnFarClick(Client.GameClient c, uint npcid, byte option)
        {
            try
            {
                // the tracker's click comes as option 1 (seen: 10084 {2: npc, 3: 1}); a real tap on the NPC is option 0
                if (option > 1 || c == null || c.Player == null || !MobileEvents.IsMobile(c)) return false;
                var gui = c.Player.QuestGUI;
                foreach (var r in MobileQuestGuide.Table.Values)
                {
                    if (r.Npc != npcid || !gui.IsActiveQuest(r.Id)) continue;
                    if (r.Map == c.Player.Map && Math.Max(Math.Abs((int)c.Player.X - (int)r.X), Math.Abs((int)c.Player.Y - (int)r.Y)) <= 16)
                        return option == 1 && OnNpc(c, npcid);// near: the tracker click finishes like a tap
                    c.SendSysMesage("[Quest] " + r.Name + ": go to " + r.NpcName + " (" + r.X + "," + r.Y + ").", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                    gui.SendAutoPatcher(r.Map, (ushort)r.X, (ushort)r.Y, r.Npc);
                    return true;
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return false;
        }

        /// <summary>
        /// Main-story "open this window" steps (14091-14093 Battle Goal = SevenDayTarget_Main, 13931 Lineage =
        /// BloodChallengeGuide_OpenWindow): the tracker click calls the step's own Lua function with no quest id. The
        /// server has no 7-day-goal / lineage window yet, so opening it counts as the goal: the step is finished and paid
        /// wherever the player is. MobileQuestGuide.OnLuaCall, before its own loop. True = a step was finished.
        /// </summary>
        public static bool OnWindowCall(Client.GameClient c, string[] strings)
        {
            try
            {
                if (c == null || c.Player == null || strings == null) return false;
                foreach (var s0 in strings)
                {
                    if (string.IsNullOrEmpty(s0)) continue;
                    string s = s0.Trim();
                    if (!s.StartsWith("</F>SevenDayTarget_Main", StringComparison.Ordinal)
                        && !s.StartsWith("</F>BloodChallengeGuide_OpenWindow", StringComparison.Ordinal)) continue;
                    var gui = c.Player.QuestGUI;
                    foreach (var r in MobileQuestGuide.Table.Values)
                    {
                        if (string.IsNullOrEmpty(r.Func) || !s.StartsWith("</F>" + r.Func, StringComparison.Ordinal) || !gui.IsActiveQuest(r.Id)) continue;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            gui.SetQuestObjectives(stream, r.Id, r.Count);
                            if (!MobileQuestStory.HasSpace(c, r.Id)) return true;
                            if (!gui.FinishQuest(r.Id)) return false;
                            MobileQuestStory.Pay(c, r.Id, stream);
                        }
                        MobileQuestGuide.EnsureMainQuest(c);
                        return true;
                    }
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return false;
        }

        /// <summary>
        /// The Chinju bounty board call (MobileQuestBounty.OnLuaCall) wrapped for the main story: 13920 "Bounty Hall"
        /// = complete 3 bounty quests. A bounty finish is seen as one more Finished quest after a CompleteTask call;
        /// each one adds 1 to 13920's counter, and at the goal the step is finished and paid like any other.
        /// </summary>
        public const uint BountyStep = 13920;
        public static bool BountyCall(Client.GameClient c, string[] strings)
        {
            int before = -1;
            try
            {
                if (c != null && c.Player != null && strings != null && c.Player.QuestGUI.IsActiveQuest(BountyStep)
                    && strings.Any(s => s != null && s.Contains("TaskAssemblyGfit_CompleteTask")))// "</F>ClientManager_Callback</N>10083</S>TaskAssemblyGfit_CompleteTask</N>id</N>page"
                    before = Finished(c);
            }
            catch { before = -1; }
            bool handled = MobileQuestBounty.OnLuaCall(c, strings);
            if (before < 0) return handled;
            try
            {
                if (Finished(c) <= before) return handled;
                MobileQuestGuide.Row r;
                if (!MobileQuestGuide.Table.TryGetValue(BountyStep, out r)) return handled;
                var gui = c.Player.QuestGUI;
                uint[] cur;
                gui.GetQuestObjectives(BountyStep, out cur);
                uint have = (cur != null && cur.Length > 0 ? cur[0] : 0) + 1;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    gui.SetQuestObjectives(stream, BountyStep, Math.Min(have, r.Count));
                    if (have < r.Count || !MobileQuestStory.HasSpace(c, BountyStep) || !gui.FinishQuest(BountyStep)) return handled;
                    MobileQuestStory.Pay(c, BountyStep, stream);
                }
                MobileQuestGuide.EnsureMainQuest(c);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
            return handled;
        }
        static int Finished(Client.GameClient c)
        {
            int n = 0;
            foreach (var q in c.Player.QuestGUI.src.Values)
                if (q.Status == Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished) n++;
            return n;
        }

        /// <summary>After the engine finished quest <paramref name="id"/> (FinishQuest succeeded): main-story reward + next step.</summary>
        public static void OnEngineFinished(Client.GameClient c, uint id, ServerSockets.Packet stream)
        {
            try
            {
                if (c == null || c.Player == null || !MobileEvents.IsMobile(c)) return;
                if (!MobileQuestGuide.Table.ContainsKey(id)) return;
                MobileQuestStory.Pay(c, id, stream);
                MobileQuestGuide.EnsureMainQuest(c);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static bool GoalDone(Client.GameClient c, MobileQuestGuide.Row r)
        {
            if (!IsMonster(r.Target)) return true;
            uint sm; ushort sx, sy; int sd;
            if (!MobileQuestStory.NearestSpot(r.Target, r.Map, (int)r.X, (int)r.Y, out sm, out sx, out sy, out sd)) return true;
            uint[] cur;
            c.Player.QuestGUI.GetQuestObjectives(r.Id, out cur);
            return cur != null && cur.Length > 0 && cur[0] >= r.Count;
        }

        static bool IsMonster(string name)
        {
            if (string.IsNullOrEmpty(name) || Pool.MonsterFamilies == null) return false;
            string n = name.Replace(" ", "").ToLowerInvariant();
            foreach (var f in Pool.MonsterFamilies.Values)
                if (f != null && f.Name != null && f.Name.Replace(" ", "").ToLowerInvariant() == n) return true;
            return false;
        }
    }
}
