using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// The mobile friend panel asks for its lists when it opens: CHeroFriendAndEnemyMgr::QueryFriendList sends
    /// CMsgFriend 10139 action 22 (0x16) and the panel then shows only what arrives after that query. The PC server sends
    /// friends / enemies once at login (Associate.cs) and ignored action 22, so the panel always showed 0/0. Here the
    /// same AddFriend (15) / AddEnemy (19) packets as at login are sent again in answer to the query.
    /// </summary>
    public static class MobileFriends
    {
        public const byte QueryList = 22;

        public static void SendList(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client == null || client.Player == null || client.Player.Associate == null)
                return;
            var a = client.Player.Associate;
            Send(client, stream, a, Role.Instance.Associate.Friends, MsgKnowPersons.Action.AddFriend);
            Send(client, stream, a, Role.Instance.Associate.Enemy, MsgKnowPersons.Action.AddEnemy);
        }

        static void Send(Client.GameClient client, ServerSockets.Packet stream, Role.Instance.Associate.MyAsociats a,
            byte mode, MsgKnowPersons.Action action)
        {
            if (!a.Associat.ContainsKey(mode))
                return;
            foreach (var mem in a.Associat[mode].Values)
            {
                Client.GameClient t;
                if (Pool.GamePoll.TryGetValue(mem.UID, out t) && t.Player != null)
                    client.Send(stream.KnowPersonsCreate(action, mem.UID, true, mem.Name, (uint)t.Player.NobilityRank, t.Player.Mesh / 1000, t.Player.Level, t.Player.Description));
                else
                    client.Send(stream.KnowPersonsCreate(action, mem.UID, false, mem.Name, 0, 0, mem.Level, mem.Description));
            }
        }
    }
}
