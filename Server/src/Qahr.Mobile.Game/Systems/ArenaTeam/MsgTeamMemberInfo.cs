using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe struct TeamMemberInfo
    {
        public string Name;
        public uint UID;
        public uint Mesh;
        public ushort MaxHitpoints;
        public ushort MinMHitpoints;
        public uint Frame;
    }
    public unsafe static class MsgTeamMemberInfo
    {

        public static unsafe ServerSockets.Packet TeamMemberInfoCreate(this ServerSockets.Packet stream, TeamMemberAction action, Role.Instance.Team.MemberInfo[] members)
        {
            stream.InitWriter();

            stream.Write((byte)action);//4

            if (members != null)
                stream.Write((byte)members.Length);//5
            else
                stream.Write((byte)0);//5


            stream.Write((ushort)0);//6
           
            for (int x = 0; x < 5; x++)
            {
                if (members != null && members.Length > x)
                {
                    var mem = members[x];//60


                    stream.Write(mem.Info.Name, 32);//8 
                    stream.Write(mem.Info.UID);//40
                    stream.Write(mem.Info.Mesh);//44

                    stream.Write((uint)mem.Info.MaxHitpoints);//48
                    stream.Write((uint)mem.Info.MinMHitpoints);//52
                    stream.Write(mem.Info.Frame);//44//56
                    stream.ZeroFill(12);//60
                }
                else
                    stream.ZeroFill(64);
            }
            stream.Finalize(GamePackets.MsgTeamMember);



            return stream;
        }


        public enum TeamMemberAction : byte
        {
            AddMember = 0,
            DropAddMember = 1
        }

    }
}
