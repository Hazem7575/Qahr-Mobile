// ============================================================================
//  QahrMobile: quest status request of the mobile client (CMsgTaskStatus 10120 action 3).
//
//  The mobile keeps its own quest table (questinfo.ini in the APK) and, when the quest panel loads, asks the
//  server for the status of ITS quest ids: MsgTaskStatus Review with up to 80 units (id u32, status, left time),
//  several packets in a row (CMissionSystemData::RequestTaskStatus). The PC client sends one Review with no ids
//  and gets the whole server list back (Quests.SendFullGUI), which ignored every request with 80 units.
//
//  Here every request is answered with exactly the ids it carries: Accepted (0) / Finished (1) / Available (2)
//  plus the left time. The mobile stores the status over the quest's CompleteFlag (2 = not taken, 1 = finished,
//  0 = accepted -> "already accepted" list) and decides by itself which quests can be accepted (level,
//  profession, Prequest finished), so a correct answer is what fills its quest panel.
// ============================================================================
using System;
using System.Collections.Generic;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.QuestEngine
{
    public static class MobileQuestStatus
    {
        const int UnitsOffset = 24;   // server MsgTaskStatus: action u16 4, count u16 6, 16 zero bytes, units at 24
        const int UnitSize = 12;      // id u32, status u32, left time u32
        const int MaxUnits = 80;

        /// <summary>Reads the requested ids of a Review packet. Null when the packet carries none (PC style request).</summary>
        public static List<uint> ReadRequest(ServerSockets.Packet stream, ushort count)
        {
            if (count == 0)
                return null;
            int n = Math.Min((int)count, MaxUnits);
            if (stream.Size < UnitsOffset + UnitSize)
                return null;
            n = Math.Min(n, (stream.Size - UnitsOffset) / UnitSize);
            var ids = new List<uint>(n);
            stream.Seek(UnitsOffset);
            for (int i = 0; i < n; i++)
            {
                uint id = stream.ReadUInt32();
                stream.ReadUInt32();
                stream.ReadUInt32();
                if (id != 0)
                    ids.Add(id);
            }
            return ids.Count > 0 ? ids : null;
        }

        /// <summary>Answers a Review request with the status of exactly these ids (one packet, at most 80 units).</summary>
        public static void Answer(Client.GameClient user, ServerSockets.Packet stream, List<uint> ids)
        {
            var gui = user.Player.QuestGUI;
            var units = new List<MsgQuestList.QuestListItem>(ids.Count);
            foreach (uint id in ids)
            {
                MsgQuestList.QuestListItem item = null;
                try { gui.src.TryGetValue(id, out item); } catch { item = null; }
                units.Add(item ?? new MsgQuestList.QuestListItem { UID = id, Status = MsgQuestList.QuestListItem.QuestStatus.Available, Time = 0 });
            }
            stream.QuestListCreate(MsgQuestList.QuestMode.Review, (ushort)units.Count);
            foreach (var u in units)
                stream.AddItemQuestList(u);
            user.Send(stream.QuestListFinalize());

            // progress counters of the accepted ones, so the tracker shows "Kill x (3/10)" after a relog
            foreach (var u in units)
            {
                if (u.Status != MsgQuestList.QuestListItem.QuestStatus.Accepted || !gui.IsActiveQuest(u.UID))
                    continue;
                uint[] cur;
                gui.GetQuestObjectives(u.UID, out cur);
                if (cur != null && cur.Length > 0)
                    user.Send(stream.MsgQuestDataCreate(0, u.UID, cur));
            }
        }
    }
}
