using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class BotJail
    {

        public class User
        {
            public uint UID;
            public uint CountJail;

            public override string ToString()
            {
                var writer = new DBActions.WriteLine('/');
                writer.Add(UID).Add(CountJail);
                return writer.Close();
            }
        }

        private static List<User> UsersPoll = new List<User>();

        // 2 Billion CPs
        public const ulong JailPrice = 2000000000;

        public static bool TryGetObject(uint UID, out User obj)
        {
            foreach (var _obj in UsersPoll)
            {
                if (_obj.UID == UID)
                {
                    obj = _obj;
                    return true;
                }
            }

            obj = null;
            return false;
        }

        public static bool CanOutBot(Client.GameClient client)
        {
            User _user;

            if (TryGetObject(client.Player.UID, out _user))
            {
                if (_user.CountJail < 3)
                    return true;

                return false;
            }

            return true;
        }

        public static bool CanOutBotJail(Client.GameClient client)
        {
            User _user;

            if (TryGetObject(client.Player.UID, out _user))
            {
                if (_user.CountJail < 3 && client.Player.CanOut)
                    return true;

                return false;
            }

            return true;
        }

        // خروج من البوت جيل
        public static void CheckUp(Client.GameClient client)
        {
            User _user;

            if (!TryGetObject(client.Player.UID, out _user))
            {
                _user = new User()
                {
                    UID = client.Player.UID,
                    CountJail = 1
                };

                UsersPoll.Add(_user);
            }

            // Permanent Jail
            if (_user.CountJail >= 3)
            {
                client.CreateBoxDialog("Your account is permanently jailed.");
                return;
            }

            // CPs Check
            if ((ulong)client.Player.ConquerPoints < JailPrice)
            {
                client.CreateBoxDialog("You need 2 Billion CPs to leave Bot Jail.");
                return;
            }

            // Remove CPs
            client.Player.ConquerPoints -= (uint)JailPrice;

            client.Player.CanOut = true;

            if (CanOutBotJail(client))
            {
                // خروج من الجيل
                client.Teleport(410, 354, 1002);

                Qahr.Mobile.Game.MsgTournaments.MsgSchedules.SendSysMesage(
                    "Player " + client.Player.Name + " has left Bot Jail.",
                    Qahr.Mobile.Game.MsgServer.MsgMessage.ChatMode.Center);
            }
        }

        // دخول البوت جيل
        public static void JoinJail(Client.GameClient client)
        {
            User _user;

            if (TryGetObject(client.Player.UID, out _user))
            {
                _user.CountJail++;
            }
            else
            {
                _user = new User()
                {
                    UID = client.Player.UID,
                    CountJail = 1
                };

                UsersPoll.Add(_user);
            }

            client.Player.CanOut = false;

            // دخول الجيل
            client.Teleport(35, 78, 10001);

            Qahr.Mobile.Game.MsgTournaments.MsgSchedules.SendSysMesage(
                "Player " + client.Player.Name +
                " has been sent to Bot Jail. Total Jail Count: " +
                _user.CountJail,
                Qahr.Mobile.Game.MsgServer.MsgMessage.ChatMode.Center);

            // لو وصل 3 مرات
            if (_user.CountJail >= 3)
            {
                client.CreateBoxDialog("Your account is now permanently jailed.");
            }
        }

        // استدعاء من نظام الحماية
        public static void DetectHack(Client.GameClient client, string Reason)
        {
            JoinJail(client);

            client.CreateBoxDialog(
                "Illegal tool detected: " + Reason);

            client.Socket.Disconnect();
        }

        public static void Save()
        {
            using (Database.DBActions.Write _wr =
                new Database.DBActions.Write("BotJail.txt"))
            {
                foreach (var _obj in UsersPoll)
                    _wr.Add(_obj.ToString());

                _wr.Execute(DBActions.Mode.Open);
            }
        }

        public static void Load()
        {
            using (Database.DBActions.Read r =
                new Database.DBActions.Read("BotJail.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;

                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine reader =
                            new DBActions.ReadLine(r.ReadString(""), '/');

                        User user = new User();

                        user.UID = reader.Read((uint)0);
                        user.CountJail = reader.Read((uint)0);

                        UsersPoll.Add(user);
                    }
                }
            }
        }
    }
}


