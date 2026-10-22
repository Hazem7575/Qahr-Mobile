using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgRefineEffect
    {
        [ProtoContract]
        public class RefineEffectProto
        {
            [ProtoMember(1, IsRequired = true)]
            public uint Id;
            [ProtoMember(2, IsRequired = true)]
            public uint dwParam;
            [ProtoMember(3, IsRequired = true)]
            public RefineEffects Effect;
        }
        [Flags]
        public enum RefineEffects : byte
        {
            None,
            ToxinEraserLevel,
            StrikeLockLevel,
            LuckyStrike,
            CalmWind,
            DrainingTouch,
            BloodSpawn,
            LightOfStamina,
            ShiledBreak,
            KillingFlash,
            MirrorOfSin,
            DivineGuard,
            CoreStrike,
            InvisbleArrow,
            FreeSoul,
            StraightLife,
            AbsoluteLuck
        }
        public static unsafe ServerSockets.Packet MsgRefineEffectCreate(this ServerSockets.Packet stream, RefineEffectProto obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgRefineEffect);
            return stream;

        }
    }
}
