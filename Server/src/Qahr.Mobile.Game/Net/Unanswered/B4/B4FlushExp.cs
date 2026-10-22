// [Unanswered B4 2026-10-14] 2274 CMsgFlushExp (client -> server refresh request, protobuf CMsgFlushExpPB).
// Client asks with f3 = id, f4 = kind. Process 0x10c1cc6: f4 == 0 -> weapon skill (proficiency) id f3, exp = f1, needExp = f2.
// f4 == 1 walks other hero lists (unknown owner system) -> not answered.
// Read-only: the reply comes from the server's own proficiency table; client f1/f2 values are never used.
using System;
using ProtoBuf;
using Qahr.Mobile.Game;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Unanswered.B4
{
    public static class B4FlushExp
    {
        [ProtoContract]
        public class Request
        {
            [ProtoMember(1)] public uint f1;
            [ProtoMember(2)] public uint f2;
            [ProtoMember(3)] public uint ID;
            [ProtoMember(4)] public uint Type;
        }

        [PacketAttribute(GamePackets.MsgFlushExp)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client) || client.MyProfs == null) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2274)) return;
                var req = B4Util.ReadProto<Request>(stream);
                if (req == null || req.Type != MsgFlushExpProto.TypeWeaponSkill) return;
                MsgProficiency prof;
                if (!client.MyProfs.ClientProf.TryGetValue(req.ID, out prof) || prof == null) return;
                uint need = BaseFunc.ProficiencyLevelExperience((byte)prof.Level); // same value UpdSpell sends
                B4Util.Send(client, stream.UpdateProfExperienceCreate(prof.Experience, need, prof.ID));
            }
            catch (Exception e) { B4Util.Log(e); }
        }
    }
}
