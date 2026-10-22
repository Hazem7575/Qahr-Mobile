// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Qahr.Mobile.Login.Database
{
    public unsafe class ServerInfo
    {
        public string Name;
        public string IP;
        public ushort Port;
        public string TransferKey;
        public string TransferSalt;
    }
    public unsafe class Server
    {
        public static Dictionary<string, ServerInfo> Servers = new Dictionary<string, ServerInfo>();
        public static void Load()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("Servers"))
            using (var reader = cmd.CreateReader())
            {
                while (reader.Read())
                {
                    ServerInfo serverinfo = new ServerInfo();
                    serverinfo.Name = reader.ReadString("Name");
                    serverinfo.IP = reader.ReadString("IP");
                    serverinfo.Port = reader.ReadUInt16("Port");
                    Servers.Add(serverinfo.Name, serverinfo);
                    string format = "{0} [{1}:{2}]";
                    Console.WriteLine(string.Format(format, serverinfo.Name, serverinfo.IP, serverinfo.Port));
                }
            }
        }
    }
}
