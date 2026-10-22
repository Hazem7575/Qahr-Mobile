using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class VoteSystem
    {

        public class User
        {
            public uint UID;
            public string IP;
            public string HWID;
            public DateTime Timer = new DateTime();
            public override string ToString()
            {
                var writer = new DBActions.WriteLine('/');
                // نفس ترتيب Load: UID/IP/HWID/Timer (كان HWID ناقص فالوقت بيتقري غلط والكول-داون بيتلغي)
                writer.Add(UID).Add(IP ?? "").Add(HWID ?? "").Add(Timer.Ticks);
                return writer.Close();
            }
        }

        private static List<User> UsersPoll = new List<User>();


        public static bool TryGetObject(uint UID, string IP, out User obj,string HWID)
        {
            foreach (var _obj in UsersPoll)
            {
                if (_obj.UID == UID || _obj.IP == IP|| _obj.HWID == HWID)
                {
                    obj = _obj;
                    return true;
                }
            }
            obj = null;
            return false;
        }
        public static bool CanVote(Client.GameClient client)
        {
            User _user;
            if (TryGetObject(client.Player.UID, client.Socket.RemoteIp, out _user,client.OnLogin.HWID))
            {
                if (_user.Timer.AddHours(24) < DateTime.Now)
                    return true;
                else
                    return false;
            }
            return true;
        }
        public static void CheckUp(Client.GameClient client)
        {
            if (client.Player.StartVote)
            {
                User _user;
                if (TryGetObject(client.Player.UID, client.Socket.RemoteIp, out _user, client.OnLogin.HWID))
                {
                    _user.Timer = DateTime.Now;
                }
                else
                {
                    _user = new User();
                    _user.UID = client.Player.UID;
                    _user.IP = client.Socket.RemoteIp;
                    _user.HWID = client.OnLogin.HWID;
                    _user.Timer = DateTime.Now;
                    UsersPoll.Add(_user);
                }
                if (DateTime.Now > client.Player.StartVoteStamp)
                {
                  
                    client.Player.StartVote = false;
                    client.Player.ExpireVip = DateTime.Now.AddHours(2);
                    client.Player.VipLevel = 4;
                    client.Player.VotePoints += 1;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Player.SendUpdate(stream, client.Player.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);

                        client.Player.UpdateVip(stream);
                    }

                    client.CreateBoxDialog("Thank you for your support. You`ve received VIP6 (2 Hour) | 1 Points .");
                }

            }
        }
        public static void Save()
        {
            using (Database.DBActions.Write _wr = new Database.DBActions.Write("Votes.txt"))
            {
                foreach (var _obj in UsersPoll.ToArray())
                    _wr.Add(_obj.ToString());
                _wr.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {
            using (Database.DBActions.Read r = new Database.DBActions.Read("Votes.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine reader = new DBActions.ReadLine(r.ReadString(""), '/');
                        User user = new User();
                        user.UID = reader.Read((uint)0);
                        user.IP = reader.Read("");
                        user.HWID = reader.Read("");
                        user.Timer = DateTime.FromBinary(reader.Read((long)0));
                        UsersPoll.Add(user);
                    }
                }
            }

        }

    }
}
