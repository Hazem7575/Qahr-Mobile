using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetTitle(this ServerSockets.Packet stream, out  byte Title
            , out MsgTitle.QueueTitle Action)
        {
            uint UID = stream.ReadUInt32();
            Title = stream.ReadUInt8();
            Action = (MsgTitle.QueueTitle)stream.ReadUInt8();
        }

        public static unsafe ServerSockets.Packet TitleCreate(this ServerSockets.Packet stream, uint UID, byte Title
            , MsgTitle.QueueTitle Action)
        {
            stream.InitWriter();

            stream.Write(UID);
            stream.Write(Title);
            stream.Write((byte)Action);

            stream.Finalize(GamePackets.MsgTitle);

            return stream;
        }


    }
    public unsafe struct MsgTitle
    {
        public enum QueueTitle : uint
        {
            Enqueue = 1,
            Dequeue = 2,
            Change = 3,
            List = 4
        }
        [PacketAttribute(GamePackets.MsgTitle)]
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            byte Title;
            MsgTitle.QueueTitle Action;

            stream.GetTitle(out Title, out Action);

            switch (Action)
            {
                case QueueTitle.List:
                    {
                        foreach (var title in user.Player.Titles)
                            user.Send(stream.TitleCreate(user.Player.UID, title, QueueTitle.Enqueue));
                        break;
                    }
                case QueueTitle.Change:
                    {
                        user.Player.SwitchTitle(Title);
                        break;
                    }
            }
        }
    }
}
