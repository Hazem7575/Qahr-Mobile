using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetQuestData(this ServerSockets.Packet stream, out uint UnKnow, out uint UID, out uint[] intents)
        {
            UnKnow = stream.ReadUInt32();//4
            UID = stream.ReadUInt32();//8 35034
            intents = new uint[1];
            intents[0] = stream.ReadUInt32();//12
        }
        public static unsafe ServerSockets.Packet MsgQuestDataCreate(this ServerSockets.Packet stream, uint UnKnow, uint UID, params uint[] intents)
        {
            stream.InitWriter();
            stream.Write((ushort)0);
            stream.Write((ushort)intents.Length);
            stream.Write(UID);
            for (int x = 0; x < intents.Length; x++)
                stream.Write(intents[x]);
            if (UID == 35024)
                for (int x = 0; x < intents.Length; x++)
                    stream.Write(intents[x]);
            stream.Write(0);
            if (UID == 35007)
            {
                stream.ZeroFill(12);
                for (int x = 0; x < intents.Length; x++)
                    stream.Write(intents[x]);
            }
            // QahrMobile: the mobile reads this packet as 32-byte units from offset 8 (quest-protocol.txt); the doubled
            // 35024 / 35007 tails are padded with zeros to whole units, so no unit is ever cut in half
            if (UID == 35024 || UID == 35007)
            {
                int body = stream.Position - 8;
                if (body > 32 && body % 32 != 0) stream.ZeroFill(32 - body % 32);
            }

            stream.Finalize(GamePackets.MsgTaskDetailInfo);
            return stream;
        }
        public static unsafe ServerSockets.Packet MsgQuestDataCreate(this ServerSockets.Packet stream, uint UnKnow, uint UID, uint OwnerUID, params uint[] intents)
        {
            stream.InitWriter();

            stream.Write(UnKnow);//4
            stream.Write(UID);//8
            for (int x = 0; x < intents.Length; x++)
                stream.Write(intents[x]);//12
            stream.ZeroFill(sizeof(uint) * Math.Min(6, (6 - intents.Length)));//14
            stream.Write(OwnerUID);//16
            stream.ZeroFill(sizeof(uint));
            stream.Finalize(GamePackets.MsgTaskDetailInfo);
            return stream;
        }
    }
    public class MsgQuestData
    {
        [PacketAttribute(GamePackets.MsgTaskDetailInfo)]
        private static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint UnKnow; uint UID;
            uint[] intents;
            stream.GetQuestData(out UnKnow, out UID, out intents);
            if (user.OnInterServer)
            {
                  if (Database.QuestInfo.IsKingDomMission(UID))
                  {
                      user.Player.QuestGUI.SetKingDomQuestObjectives(stream, UID, intents);
                  }
            }
            if (Database.QuestInfo.IsKingDomMission(UID))
            {
                user.Send(stream.MsgQuestDataCreate(UnKnow, UID, user.Player.UID));
                user.Send(stream.MsgQuestDataCreate(UnKnow, UID, user.Player.UID));
                return;
            }
            // QahrMobile: the mobile asks for up to 31 ids at once (count u16 at 6, 32-byte units from 8, all carried in by
            // the layout): every id gets its own unit {id, 6 counters, owner uid} with the real counters, count = units.
            // A request with one id (and every PC request) keeps the old single answer below.
            if (global::Qahr.Mobile.QuestEngine.MobileQuestDataAnswer.TryAnswer(user, stream, UnKnow, UID))
                return;
            // QahrMobile: answer with the real progress counters (the mobile asks after every quest list; the empty
            // answer reset its tracker to 0). At most 6 counters fit the answer (12..35, owner uid at 36).
            uint[] cur;
            user.Player.QuestGUI.GetQuestObjectives(UID, out cur);
            if (cur == null) cur = new uint[0];
            if (cur.Length > 6) { var six = new uint[6]; Array.Copy(cur, six, 6); cur = six; }
            user.Send(stream.MsgQuestDataCreate(UnKnow, UID, user.Player.UID, cur));
        }
    }
}
