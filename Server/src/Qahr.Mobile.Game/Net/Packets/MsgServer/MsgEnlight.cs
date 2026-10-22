using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{

    public static unsafe partial class MsgBuilder
    {

        public static unsafe void GetEnlight(this ServerSockets.Packet stream, MsgEnlight* enlight)
        {
            stream.ReadUnsafe(enlight, sizeof(MsgEnlight));

        }

        public static unsafe ServerSockets.Packet EnlightCreate(this ServerSockets.Packet stream, MsgEnlight* enlight)
        {
            stream.InitWriter();

            stream.WriteUnsafe(enlight, sizeof(MsgEnlight));

            stream.Finalize(GamePackets.MsgMentorPlayer);

            return stream;
        }
    }

    public unsafe struct MsgEnlight
    {
        public uint TimerStamp;
        public uint UnKnow;
        public uint Enlighter;
        public uint Enlighted;
        public uint dwParam1;
        public uint dwParam2;
        public uint dwParam3;

        [PacketAttribute(GamePackets.MsgMentorPlayer)]
        private unsafe static void MsgEnlightHandler(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (!user.Player.OnMyOwnServer)
                return;
            MsgEnlight EnlightInfo;

            stream.GetEnlight(&EnlightInfo);

            Role.IMapObj obj;
            if (user.Player.View.TryGetValue(EnlightInfo.Enlighted, out obj, Role.MapObjectType.Player))
            {
                Role.Player target = (obj as Role.Player);
                if (user.Player.Enilghten >= 100 && target.EnlightenReceive < 5)
                {
                    if (target.Level + 20 < user.Player.Level)
                    {
                        user.Activeness.IncreaseTask(11);
                        user.Activeness.IncreaseTask(23);
                        user.Activeness.IncreaseTask(35);

                        user.HeroRewards.AddGoal(606);

                        if (target.EnlightenReceive == 0)
                            target.EnlightenTime= DateTime.Now;
                        target.EnlightenReceive += 1;
                        user.Player.Enilghten -= 100;
                        user.Player.SendUpdate(stream,user.Player.Enilghten, MsgUpdate.DataType.EnlightPoints);
                        target.Owner.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);

                        user.Player.View.SendView(stream.EnlightCreate(&EnlightInfo), true);
                    }
                }
            }
        }
    }
}
