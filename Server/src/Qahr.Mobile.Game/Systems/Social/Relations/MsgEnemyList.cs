using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static class MsgEnemyList
    {
        public static unsafe void GetEnemyList(this ServerSockets.Packet stream, out uint UID)
        {
            UID = stream.ReadUInt32();

        }
        public static unsafe ServerSockets.Packet EnemyListCreate(this ServerSockets.Packet stream, uint UID, bool online, uint UID2, string Name, uint Level, string Description, uint Mesh, uint Frame)
        {
            stream.InitWriter();

            stream.Write(UID);//4
            stream.Write((uint)1);//8
            stream.Write((uint)(online == true ? 1 : 0));//12
            stream.Write(UID2);//16
            stream.Write((uint)0);//20
            stream.Write((uint)2);//24
            stream.Write(Name, 32);//28
            stream.Write(Mesh);//60
            stream.Write(Level);//64
            stream.Write(Description, 64);//68
            stream.Write((uint)Frame);//132
            stream.Write((uint)1);//136
            stream.Finalize(GamePackets.MsgEnemyList);
            return stream;
        }
        [PacketAttribute(GamePackets.MsgEnemyList)]
        public unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (!user.Player.OnMyOwnServer)
                return;
            uint UID;
            stream.GetEnemyList(out UID);
            if (user.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Enemy))
            {
                foreach (var typ in user.Player.Associate.Associat)
                {
                    foreach (var mem in typ.Value.Values)
                    {
                        if (typ.Key == Role.Instance.Associate.Enemy)
                        {
                            Client.GameClient Target;
                            if (Pool.GamePoll.TryGetValue(mem.UID, out Target))
                            {
                                mem.Name = Target.Player.Name;
                                mem.Level = Target.Player.Level;
                                mem.Description = Target.Player.Description;
                                mem.Mesh = Target.Player.Mesh;
                                mem.Frame = Target.Player.FrameID;
                                user.Send(stream.EnemyListCreate(user.Player.UID, mem.IsOnline, Target.Player.UID, mem.Name, Target.Player.Level, Target.Player.Description, mem.Mesh, mem.Frame));
                            }
                            else
                                user.Send(stream.EnemyListCreate(user.Player.UID, mem.IsOnline, mem.UID, mem.Name, mem.Level, mem.Description, mem.Mesh, mem.Frame));
                        }
                    }
                }
            }
        }
    }
}
