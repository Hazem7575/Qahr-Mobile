using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// Team helpers for the mobile client.
    /// - The mobile player menu has one "Team Up" button; it always sends MsgTeam JoinRequest (CMsgTeam 10243 action 1,
    ///   target uid @8). When the target has no team the PC server just said "doesn't have a team"; here the requester
    ///   gets a team (same packets as MsgTeam Create) and the target an invitation (same as MsgTeam InviteRequest).
    /// - The invitation / join-request popups (CMsgTeam::Process cases 1 and 4) print the other player's name from
    ///   msg+0xc (char[32]); the server's MsgTeam struct is only action + uid, so for a mobile receiver the packet is
    ///   built here as the mobile 88-byte struct (action u16 @4, uid @8, name @12) and sent under the mobile id.
    /// </summary>
    public static class MobileTeam
    {
        public const ushort MobileMsgTeam = 10243;

        public static void CreateAndInvite(Client.GameClient user, Client.GameClient target, ServerSockets.Packet stream)
        {
            if (user == null || target == null || target.Team != null)
                return;
            if (user.Team == null)
            {
                user.Team = new Role.Instance.Team(user);
                user.Send(stream.TeamCreate(MsgTeam.TeamTypes.Send, user.Player.UID));
                user.Send(stream.TeamCreate(MsgTeam.TeamTypes.Create, user.Player.UID));
                user.Send(stream.TeamCreate(MsgTeam.TeamTypes.Send2, user.Player.UID));
            }
            if (!user.Team.CkeckToAdd() || !user.Team.TeamLider(user))
                return;
            target.Send(stream.RelationCreate(user.Player, target.Player));
            SendTeam(target, stream, MsgTeam.TeamTypes.InviteRequest, user);
            MobileOnly.SendTeamInvite(target, user, false);
        }

        /// <summary>MsgTeam (typ, uid of "from") to "to"; a mobile receiver also gets the name of "from".</summary>
        public static void SendTeam(Client.GameClient to, ServerSockets.Packet stream, MsgTeam.TeamTypes typ, Client.GameClient from)
        {
            if (to == null || from == null || from.Player == null)
                return;
            if (!MobileEvents.IsMobile(to))
            {
                to.Send(stream.TeamCreate(typ, from.Player.UID));
                return;
            }
            stream.InitWriter();
            stream.Write((ushort)typ);           // @4 action
            stream.Write((ushort)0);
            stream.Write(from.Player.UID);       // @8 uid
            var name = new byte[32];             // @12 name
            var n = Encoding.UTF8.GetBytes(from.Player.Name ?? "");
            System.Buffer.BlockCopy(n, 0, name, 0, System.Math.Min(n.Length, 31));
            foreach (byte b in name) stream.Write(b);
            for (int i = 44; i < 88; i++) stream.Write((byte)0);
            stream.Finalize(MobileMsgTeam);
            to.Send(stream);
        }
    }
}
