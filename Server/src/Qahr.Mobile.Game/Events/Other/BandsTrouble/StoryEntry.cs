// ============================================================================
//  [BandsTrouble 2026-10-14] Story entry from a festival window: the next story quest of a chain is accepted at its
//  begin NPC (QuestEngineCore.AcceptAtNpc: every accept rule of the NPC menu) and its quests.json onAccept puts the
//  player into the private copy of the official instance; a player whose story quest is already running is sent back
//  into the same copy (QuestEngine EnterInstance: re-enter branch, same copy while it has not expired).
//  Used by Base\BandsTrouble\BandsTroubleUi.cs and Base\CastleMystery\CastleMysteryUi.cs.
//  Nothing comes from the client but the chain choice (validated by the caller); no item is given here.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qahr.Mobile.Game.MsgServer;
using QS = Qahr.Mobile.Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus;

namespace Qahr.Mobile.FestivalStory
{
    public static class StoryEntry
    {
        public const uint TwinCity = 1002;
        static MethodInfo _enter;

        public static void Msg(Client.GameClient c, string t) { try { c.SendSysMesage(t, MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow); } catch { } }
        public static bool Finished(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.CheckQuest(q, QS.Finished); } catch { return false; } }
        public static bool Active(Client.GameClient c, uint q) { try { return c.Player.QuestGUI.IsActiveQuest(q); } catch { return false; } }

        /// <summary>The first quest of the chain the player has not finished (0 = the whole chain is done).</summary>
        public static uint NextOf(Client.GameClient c, uint[] chain)
        {
            foreach (uint q in chain) if (!Finished(c, q)) return q;
            return 0;
        }

        /// <summary>Accept / resume the next quest of <paramref name="chain"/>. Returns a short result for logs and tests.</summary>
        public static string Enter(Client.GameClient c, uint[] chain, int minLevel, string notInTown, string done)
        {
            var p = c.Player;
            if (p.Level < minLevel) { Msg(c, "Your level is less than " + minLevel + ". Please level up first!"); return "level"; }
            uint id = NextOf(c, chain);
            if (id == 0) { Msg(c, done); return "done"; }
            if (!p.Alive || p.DynamicID != 0 || p.Map != TwinCity) { Msg(c, notInTown); return "town"; }
            var t = QuestEngine.QuestEngineCore.Current;
            QuestEngine.EngineQuest q;
            if (t == null || !t.Quests.TryGetValue(id, out q) || q.FestivalOff) { Msg(c, "This story isn`t open yet."); return "closed"; }
            if (Active(c, id))
            {
                QuestEngine.InstanceDef def;
                string inst = q.OnAccept != null ? q.OnAccept.Instance : null;
                if (inst == null || !t.Instances.TryGetValue(inst, out def)) { Msg(c, "This story isn`t open yet."); return "noinst"; }
                return Resume(c, def, id) ? "resume " + id : "resume failed";
            }
            string name, why;
            if (!QuestEngine.QuestEngineCore.AcceptAtNpc(c, q.BeginNpc, id, out name, out why))
            {
                Msg(c, "[Quest] " + (why ?? "This quest is not available."));
                return "refused " + id;
            }
            Msg(c, "[Quest] New quest: " + name + ".");
            return "accept " + id;
        }

        static bool Resume(Client.GameClient c, QuestEngine.InstanceDef def, uint quest)
        {
            try
            {
                if (_enter == null)
                    _enter = typeof(QuestEngine.QuestEngineCore).GetMethod("EnterInstance", BindingFlags.NonPublic | BindingFlags.Static);
                if (_enter == null) { Msg(c, "This story isn`t open yet."); return false; }
                using (var rec = new ServerSockets.RecycledPacket())
                    return (bool)_enter.Invoke(null, new object[] { c, def, quest, rec.GetStream() });
            }
            catch (Exception e)
            {
                try { MyConsole.SaveException(e); } catch { }
                return false;
            }
        }
    }
}
