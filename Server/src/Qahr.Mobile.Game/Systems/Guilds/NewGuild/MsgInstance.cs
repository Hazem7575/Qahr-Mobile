using Qahr.Mobile.Client;
using ProtoBuf;
using System;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgGuild
    {
        [ProtoContract]
        public class MsgInstance
        {
            public enum _Type : byte
            {
                Info = 10,
                Quit = 11,
            }
            [ProtoMember(1, IsRequired = true)]
            public uint Type;
            [ProtoMember(3, IsRequired = true)]
            public uint weather;
            [ProtoMember(4, IsRequired = true)]
            public uint user_id;


        }
        public static ServerSockets.Packet CreateMsgInstance(this ServerSockets.Packet stream, MsgInstance obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgInstance);
            return stream;
        }

        public static void GetMsgInstance(this ServerSockets.Packet stream, out MsgInstance pQuery)
        {
            pQuery = new MsgInstance();
            pQuery = stream.ProtoBufferDeserialize<MsgInstance>(pQuery);
        }
        [PacketAttribute(GamePackets.MsgInstance)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                MsgInstance pQuery = (MsgInstance)null;
                stream.GetMsgInstance(out pQuery);
                switch ((MsgInstance._Type)pQuery.Type)
                {
                    case MsgInstance._Type.Info:
                        ushort x = 0;
                        ushort y = 0;
                        Pool.ServerMaps[10428].GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, 10428, Pool.ServerMaps[10428].GenerateDynamicID());
                        Pool.ServerMaps[10428U].GetRandCoord(ref x, ref y);
                        user.Map.AddMapMonster(stream, 7481, x, y, (ushort)1, (ushort)1, (byte)1, user.Player.DynamicID);
                        break;
                    case MsgInstance._Type.Quit:
                        user.Teleport((ushort)410, (ushort)354, 1002);
                        break;
                    default:
                        MyConsole.WriteLine("Type not found ->" + pQuery.Type.ToString() );
                        break;
                }
            }
            catch (Exception ex)
            {
                MyConsole.SaveException(ex);
            }

        }
    }

}
