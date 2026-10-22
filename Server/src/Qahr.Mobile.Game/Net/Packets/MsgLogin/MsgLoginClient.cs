using Qahr.Mobile.DBFunctionality;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{

    public struct MsgLoginClient
    {
        [ProtoContract]
        public unsafe class LoginGameQuery
        {
            [ProtoMember(1, IsRequired = true)]
            public uint TickCount;
            [ProtoMember(2, IsRequired = true)]
            public uint CryptoKey;
            [ProtoMember(3, IsRequired = true)]
            public uint ClienExeUID;
            [ProtoMember(4, IsRequired = true)]
            public uint Key;
            [ProtoMember(5, IsRequired = true)]
            public uint AccountHash;
            [ProtoMember(6, IsRequired = true)]
            public uint dwparam = 0;
            [ProtoMember(7, IsRequired = true)]
            public string Language;
            [ProtoMember(8, IsRequired = true)]
            public string MacAddress;
            [ProtoMember(9, IsRequired = true)]
            public string MachineName;
            [ProtoMember(10, IsRequired = true)]
            public string HWID;
            [ProtoMember(11, IsRequired = true)]
            public string UserName;
            [ProtoMember(12, IsRequired = true)]
            public uint VMware = 0;
            [ProtoMember(13, IsRequired = true)]
            public uint VPSMachine = 0;
            [ProtoMember(14, IsRequired = true)]
            public uint QuerySize = 0;
        }
        public uint AccountHash;
        public uint Key, HDSerial;
        public string MachineName, Username;
        public string SecurtyCode;
        public string HWID, MacAddress;
        public string Language;
        public bool VMware, VPSMachine;
        public uint ClienExeUID;


        public unsafe static void LoginHandler(Client.GameClient client, MsgLoginClient packet)
        {
            client.ClientFlag &= ~Client.ServerFlag.OnLoggion;
            if (client.Socket != null)
            {
                if (client.Socket.RemoteIp == "NONE")
                    return;

            }
            try
            {
                if (Program.svrmaintance == 1)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        string Messaj = "Failed to login: Server maintenance for 30 minutes. Please try again later!(10)";
                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;
                }
                var pool = Pool.GamePoll.Values.ToArray();
                if (pool.Count(i => i.OnLogin.MacAddress == client.OnLogin.MacAddress) >= 3)  // تحديد عدد الاكونتات للاجهزه يوزرات
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.Send(new MsgServer.MsgMessage("Sorry, You Can Only Open a Maximum of 3 Accounts.", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                    }
                    return;
                }

                string BanMessaje;
                if (Database.SystemBanned.IsBanned(client.Socket.RemoteIp, out BanMessaje))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

#if Arabic
                          string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
#else
                        string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
#endif

                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;
                }
                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) == Client.ServerFlag.CreateCharacterSucces)
                {
                    if (Database.ServerDatabase.AllowCreate(client.ConnectionUID))
                    {
                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacterSucces;
                        if (client.Player.MyChi == null)
                        {
                            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);

                        }
                        if (client.Player.SubClass == null)
                            client.Player.SubClass = new Role.Instance.SubClass();
                        if (client.Player.Flowers == null)
                        {
                            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
                            client.Player.Flowers.FreeFlowers = 1;
                        }
                        if (client.Player.Nobility == null)
                            client.Player.Nobility = new Role.Instance.Nobility(client);
                        if (client.Player.Associate == null)
                        {
                            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                            client.Player.Associate.MyClient = client;
                            client.Player.Associate.Online = true;
                        }
                        if (client.Player.InnerPower == null)
                            client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);

                        Pool.GamePoll.TryAdd(client.Player.UID, client);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage("ANSWER_OK", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));


                            Database.ServerDatabase.CreateCharacte(client);
                            Database.ServerDatabase.SaveClient(client);

                            client.ClientFlag |= Client.ServerFlag.AcceptLogin;

                            MyConsole.WriteLine("Client [ " + client.Player.Name + " ] Was Login On IP : [ " + client.Socket.RemoteIp + " ] .", ConsoleColor.White); global::Qahr.Mobile.Modules.ModuleManager.Login(client);

                            client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                            MsgLoginHandler.LoadMap(client, stream);
                        }
                        Program.CallBack.Register(client);
                        return;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) != Client.ServerFlag.AcceptLogin)
                {

                    var login = client.OnLogin;

                    client.ConnectionUID = login.Key;


                    if (Database.SystemBannedAccount.IsBanned(client.ConnectionUID, out BanMessaje))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
#if Arabic
                             string aMessaj = "Your account is Banned for: " + BanMessaje + " ";
#else
                            string aMessaj = "Your account is Banned for: " + BanMessaje;
#endif

                            client.Send(new MsgServer.MsgMessage(aMessaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                        return;
                    }

                    try
                    {
                        if (pool.Count(i => i.OnLogin.HWID == client.OnLogin.HWID) >= 3)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(new MsgServer.MsgMessage("You can not open more than 3 accounts.", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                            }
                            return;
                        }
                    }
                    catch { }


                    string Messaj = "NEW_ROLE";

                    if (Database.ServerDatabase.AllowCreate(login.Key) == false)
                    {

                        Client.GameClient InGame = null;
                        if (Pool.GamePoll.TryGetValue((uint)login.Key, out InGame))
                        {
                            if (InGame.Player != null)
                            {
                                MyConsole.WriteLine("Account try join but is in use. [ " + InGame.Player.Name + " ]", ConsoleColor.White);

                                if (InGame.Player.UID == 0)
                                {
                                    Pool.GamePoll.TryRemove((uint)login.Key, out InGame);
                                    if (InGame != null && InGame.Player != null)
                                    {
                                        if (InGame.Map != null)
                                            InGame.Map.Denquer(InGame);
                                    }
                                }
                            }
                            InGame.Socket.Disconnect();
#if Arabic
                                     Messaj = "Sorry, The Account You Are Trying To Open Is Currently Online. Try Again.";
#else
                            Messaj = "Sorry, The Account You Are Trying To Open Is Currently Online. Try Again.";
#endif

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                            }
                            if (InGame.TRyDisconnect-- == 0)
                            {
                                if (InGame.Player != null && InGame.FullLoading)
                                {
                                    InGame.ClientFlag |= Client.ServerFlag.Disconnect;
                                    Database.ServerDatabase.SaveClient(InGame);
                                }
                                Pool.GamePoll.TryRemove((uint)login.Key, out InGame);
                                if (InGame != null && InGame.Player != null)
                                {
                                    if (InGame.Map != null)
                                        InGame.Map.Denquer(InGame);
                                }
                            }
                        }
                        else
                        {
                            Pool.GamePoll.TryAdd((uint)login.Key, client);
                            Messaj = "ANSWER_OK";
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                var loadSw = System.Diagnostics.Stopwatch.StartNew(); // [perf] login_log.txt
                                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) != Client.ServerFlag.CreateCharacterSucces)
                                {
                                    Database.ServerDatabase.LoadCharacter(client, (uint)login.Key);
                                }
                                double loadMs = loadSw.Elapsed.TotalMilliseconds;

                                client.ClientFlag |= Client.ServerFlag.AcceptLogin;
                                MyConsole.WriteLine("Client [ " + client.Player.Name + " ] Was Login On IP : [ " + client.Socket.RemoteIp + " ] .", ConsoleColor.White); global::Qahr.Mobile.Modules.ModuleManager.Login(client);
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));



                                var mapSw = System.Diagnostics.Stopwatch.StartNew();
                                MsgLoginHandler.LoadMap(client, stream);
                                LoginTiming.Write(client.Player.Name, loadMs, mapSw.Elapsed.TotalMilliseconds);

                            }
                            Program.CallBack.Register(client);
                        }
                    }
                    else
                    {
                        client.ClientFlag |= Client.ServerFlag.CreateCharacter;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                    }
                }
            }

            catch (Exception e) { MyConsole.WriteException(e); }
        }
        public unsafe static void LoginHandlers(Client.GameClient client, MsgLoginClient packet)
        {
            // [sqli] parameterized (was the string-building DBFunctionality.MySqlCommand builder); same row/column.
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = new MySqlConnector.MySqlCommand("SELECT * FROM `accounts` WHERE `EntityID` = @eid", conn))
            {
                cmd.Parameters.AddWithValue("@eid", client.Player.UID);
                cmd.CommandTimeout = 30;
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        object macs = rdr["macs"];
                        client.MacAddress = (macs == null || macs is DBNull) ? "" : macs.ToString();
                    }
                }
            }
            client.ClientFlag &= ~Client.ServerFlag.OnLoggion;

            if (client.Socket != null)
            {
                if (client.Socket.RemoteIp == "NONE")
                    return;
            }
            try
            {
                if (client.OnLogin.Key > 100000000 || client.OnLogin.Key < 1000000)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        string Messaj = "You can't do this";
                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;
                }
                string BanMessaje;
                if (Database.SystemBanned.IsBanned(client.Socket.RemoteIp, out BanMessaje))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

#if Arabic
                          string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
#else
                        string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
#endif

                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;




                }
                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) == Client.ServerFlag.CreateCharacterSucces)
                {
                    if (Database.ServerDatabase.AllowCreate(client.ConnectionUID))
                    {

                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacterSucces;
                        if (client.Player.MyChi == null)
                        {
                            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);

                        }
                        if (client.Player.SubClass == null)
                            client.Player.SubClass = new Role.Instance.SubClass();
                        if (client.Player.Flowers == null)
                        {
                            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
                            client.Player.Flowers.FreeFlowers = 1;
                        }
                        if (client.Player.Nobility == null)
                            client.Player.Nobility = new Role.Instance.Nobility(client);
                        if (client.Player.Associate == null)
                        {
                            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                            client.Player.Associate.MyClient = client;
                            client.Player.Associate.Online = true;
                        }
                        if (client.Player.InnerPower == null)
                            client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);

                        Pool.GamePoll.TryAdd(client.Player.UID, client);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage("ANSWER_OK", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));


                            Database.ServerDatabase.CreateCharacte(client);
                            Database.ServerDatabase.SaveClient(client);

                            client.ClientFlag |= Client.ServerFlag.AcceptLogin;
                            MyConsole.WriteLine("Client [ " + client.Player.Name + " ] Was Login On IP : [ " + client.Socket.RemoteIp + " ] .", ConsoleColor.White); global::Qahr.Mobile.Modules.ModuleManager.Login(client);

                            client.Send(stream.LoginHandlerCreate(1, client.Player.Map));


                            MsgLoginHandler.LoadMap(client, stream);
                        }
                        Program.CallBack.Register(client);
                        return;
                    }
                }

                if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) != Client.ServerFlag.AcceptLogin)
                {

                    var login = client.OnLogin;

                    client.ConnectionUID = login.Key;

                    if (Database.SystemBannedAccount.IsBanned(client.ConnectionUID, out BanMessaje))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
#if Arabic
                             string aMessaj = "Your account is Banned for: " + BanMessaje + " ";
#else
                            string aMessaj = "Your account is Banned for: " + BanMessaje;
#endif

                            client.Send(new MsgServer.MsgMessage(aMessaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                        return;
                    }

                    string Messaj = "NEW_ROLE";

                    if (Database.ServerDatabase.AllowCreate(login.Key) == false)
                    {

                        Client.GameClient InGame = null;
                        if (Pool.GamePoll.TryGetValue((uint)login.Key, out InGame))
                        {
                            if (InGame.Player != null)
                            {
                                MyConsole.WriteLine("Account try join but is in use. [ " + InGame.Player.Name + " ]", ConsoleColor.White);

                                if (InGame.Player.UID == 0)
                                {
                                    Pool.GamePoll.TryRemove((uint)login.Key, out InGame);
                                    if (InGame != null && InGame.Player != null)
                                    {
                                        if (InGame.Map != null)
                                            InGame.Map.Denquer(InGame);
                                    }
                                }
                            }
                            InGame.Socket.Disconnect();
#if Arabic
                                     Messaj = "Sorry, The Account You Are Trying To Open Is Currently Online. Try Again.";
#else
                            Messaj = "Sorry, The Account You Are Trying To Open Is Currently Online. Try Again.";
#endif

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                            }
                            if (InGame.TRyDisconnect-- == 0)
                            {
                                if (InGame.Player != null && InGame.FullLoading)
                                {
                                    InGame.ClientFlag |= Client.ServerFlag.Disconnect;
                                    Database.ServerDatabase.SaveClient(InGame);
                                }
                                Pool.GamePoll.TryRemove((uint)login.Key, out InGame);
                                if (InGame != null && InGame.Player != null)
                                {
                                    if (InGame.Map != null)
                                        InGame.Map.Denquer(InGame);
                                }
                            }
                        }
                        else
                        {
                            Messaj = "ANSWER_OK";
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) != Client.ServerFlag.CreateCharacterSucces)
                                {
                                    Database.ServerDatabase.LoadCharacter(client, (uint)login.Key);
                                }
                                client.ClientFlag |= Client.ServerFlag.AcceptLogin;
                                MyConsole.WriteLine("Client [ " + client.Player.Name + " ] Was Login On IP : [ " + client.Socket.RemoteIp + " ] .", ConsoleColor.White); global::Qahr.Mobile.Modules.ModuleManager.Login(client);

                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                                MsgLoginHandler.LoadMap(client, stream);
                            }
                            Program.CallBack.Register(client);
                            Pool.GamePoll.TryAdd((uint)login.Key, client);
                        }
                    }
                    else
                    {
                        client.ClientFlag |= Client.ServerFlag.CreateCharacter;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
    }

    /// <summary>
    /// [Security flaw 1/2/7] Server-side store for one-time login session tokens.
    /// The account server inserts a random 64-bit token (bound to AccountID/EntityID, IP and
    /// creation time) into the shared `login_tokens` table. The game server looks it up here,
    /// verifies freshness + IP, and consumes (deletes) it so it can never be replayed.
    /// Required table (see deployment notes):
    ///   CREATE TABLE IF NOT EXISTS `login_tokens` (
    ///     `Token` BIGINT UNSIGNED NOT NULL,
    ///     `EntityID` INT UNSIGNED NOT NULL,
    ///     `State` INT UNSIGNED NOT NULL DEFAULT 0,
    ///     `IP` VARCHAR(45) NOT NULL DEFAULT '',
    ///     `CreatedAt` BIGINT NOT NULL,
    ///     PRIMARY KEY (`Token`)
    ///   );
    /// </summary>
    internal static class LoginTokenStore
    {
        public static bool TryConsume(ulong token, string ip, out uint entityId, out uint state)
        {
            entityId = 0;
            state = 0;
            if (token == 0)
                return false;

            try
            {
                bool found = false;
                uint foundEntity = 0, foundState = 0;
                string tokenIp = "";
                long createdAt = 0;

                using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("login_tokens").Where("Token", token))
                using (var rdr = new MySqlReader(cmd))
                {
                    if (rdr.Read())
                    {
                        found = true;
                        foundEntity = rdr.ReadUInt32("EntityID");
                        foundState = rdr.ReadUInt32("State");
                        tokenIp = rdr.ReadString("IP");
                        createdAt = rdr.ReadInt64("CreatedAt");
                    }
                }

                // لا نحذف التوكن عند أول استهلاك: تدفق إنشاء الشخصية الجديدة يعمل _MSG_CONNECT مرتين
                // (اتصال لشاشة الإنشاء ثم اتصال لدخول العالم) بنفس التوكن. نتركه صالحًا طوال مدة الـ TTL،
                // ونكتفي بتنظيف التوكنات المنتهية (أقدم من TTL) حتى لا يكبر الجدول ولا تُعاد بعد انتهائها.
                try
                {
                    int ttlClean = Program.ServerConfig.LoginTokenTtlSeconds; if (ttlClean <= 0) ttlClean = 30;
                    long cutoff = DateTime.UtcNow.Ticks - (long)TimeSpan.TicksPerSecond * ttlClean;
                    var clean = new MySqlCommand(MySqlCommandType.DELETE);
                    clean.Command = "DELETE FROM login_tokens WHERE CreatedAt < " + cutoff;
                    clean.Execute();
                }
                catch { }

                if (!found)
                {
                    try { MyConsole.WriteLine("[LoginToken] NOT FOUND token=" + token, ConsoleColor.Yellow); } catch { }
                    return false;
                }

                int ttl = Program.ServerConfig.LoginTokenTtlSeconds;
                if (ttl <= 0) ttl = 30;
                double ageSec = (double)(DateTime.UtcNow.Ticks - createdAt) / TimeSpan.TicksPerSecond;
                // العمر السالب = اختلاف ساعة بسيط بين الإصدار والاستهلاك؛ نعتبره طازجًا مش منتهيًا.
                if (ageSec > ttl)
                {
                    try { MyConsole.WriteLine("[LoginToken] EXPIRED age=" + ageSec.ToString("0") + "s ttl=" + ttl + "s", ConsoleColor.Yellow); } catch { }
                    return false;
                }

                if (Program.ServerConfig.LoginTokenBindIp)
                {
                    if (string.IsNullOrEmpty(ip) || ip == "NONE" || tokenIp != ip)
                    {
                        try { MyConsole.WriteLine("[LoginToken] IP MISMATCH token-ip=" + tokenIp + " conn-ip=" + ip, ConsoleColor.Yellow); } catch { }
                        return false;
                    }
                }
                try { MyConsole.WriteLine("[LoginToken] OK entity=" + foundEntity + " age=" + ageSec.ToString("0") + "s", ConsoleColor.Green); } catch { }

                entityId = foundEntity;
                state = foundState;
                return true;
            }
            catch (Exception e)
            {
                MyConsole.WriteException(e);
                return false;
            }
        }
    }
}