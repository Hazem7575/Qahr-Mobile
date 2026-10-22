// QahrMobile: answer of the mobile's CMsgTaskDetailInfo request (10136 = server 2339) when it asks for several quest
// ids at once (count u16 at 6, 32-byte units from 8, all carried in by Data\mobile_layouts\quest-protocol.txt).
// Called from MsgQuestData.Process; a request with one id (every PC request) keeps the old single answer there.
using System;

namespace Qahr.Mobile.QuestEngine
{
    public static class MobileQuestDataAnswer
    {
        /// <summary>QahrMobile: several ids in one request -> one answer with one 32-byte unit per id.</summary>
        public static unsafe bool TryAnswer(Client.GameClient user, ServerSockets.Packet stream, uint UnKnow, uint firstId)
        {
            int count = (int)(UnKnow >> 16);
            if (count <= 1) return false;
            stream.Seek(0);
            int len = stream.ReadUInt16();
            int n = Math.Min(Math.Min(count, 31), Math.Max(0, (len - 8) / 32));
            if (n <= 1) return false;
            var ids = new uint[n];
            for (int k = 0; k < n; k++)
            {
                stream.Seek(8 + 32 * k);
                ids[k] = stream.ReadUInt32();
            }
            ids[0] = firstId;
            stream.InitWriter();
            stream.Write((ushort)(UnKnow & 0xFFFF));//4 action
            stream.Write((ushort)n);//6 count
            foreach (uint id in ids)
            {
                uint[] cur = null;
                if (id != 0 && !Database.QuestInfo.IsKingDomMission(id))
                    user.Player.QuestGUI.GetQuestObjectives(id, out cur);
                stream.Write(id);//+0
                for (int x = 0; x < 6; x++)
                    stream.Write(cur != null && x < cur.Length ? cur[x] : 0u);//+4..+27
                stream.Write(user.Player.UID);//+28 (the layout copies 28 bytes: not a counter)
            }
            stream.Finalize(Game.GamePackets.MsgTaskDetailInfo);
            user.Send(stream);
            return true;
        }
    }
}
