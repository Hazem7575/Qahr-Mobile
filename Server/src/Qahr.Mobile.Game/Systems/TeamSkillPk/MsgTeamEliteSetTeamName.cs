using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetEliteSetTeamName(this ServerSockets.Packet stream, out uint dwparam1, out uint dwparam2, out string name)
        {
            dwparam1 = stream.ReadUInt32();
            dwparam2 = stream.ReadUInt32();
            name = stream.ReadCString(32);
        }
        public static unsafe ServerSockets.Packet TeamEliteSetTeamName(this ServerSockets.Packet stream, ushort ID, MsgTeamEliteSetTeamName.State state
            , uint TeamUID, string name)
        {
            stream.InitWriter();

            stream.Write((uint)state);
            stream.Write((uint)TeamUID);
            stream.Write(name, 32);

            stream.Finalize(ID);

            return stream;
        }
    }
    public unsafe struct MsgTeamEliteSetTeamName
    {
        public enum State : uint
        {
            Apprend = 0, SuccessfulName = 1, RenameWasSuccessfulName = 2, Remove = 3
        }
        [PacketAttribute(GamePackets.MsgDominateTeamName)]
        private static void PorocesTeamPk(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint dwparam1, dwparam2;
            string name;
            stream.GetEliteSetTeamName(out dwparam1, out dwparam2, out name);
            if (user.Team != null)
            {
                if (user.Team.TeamLider(user))
                {
                    user.Team.TeamName = name;
                    stream.TeamEliteSetTeamName(GamePackets.MsgDominateTeamName, State.SuccessfulName, user.Team.UID, user.Team.TeamName);
                    foreach (var member in user.Team.Temates)
                        member.client.Send(stream);
                }
            }

        }
        [PacketAttribute(GamePackets.MsgDominateTeamPopPkName)]
        private static void PorocesSkillTeamPk(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint dwparam1, dwparam2;
            string name;
            stream.GetEliteSetTeamName(out dwparam1, out dwparam2, out name);
            if (user.Team != null)
            {
                if (user.Team.TeamLider(user))
                {
                    user.Team.TeamName = name;
                    stream.TeamEliteSetTeamName(GamePackets.MsgDominateTeamPopPkName, State.SuccessfulName, user.Team.UID, user.Team.TeamName);
                    foreach (var member in user.Team.Temates)
                        member.client.Send(stream);
                }
            }

        }
    }

}
