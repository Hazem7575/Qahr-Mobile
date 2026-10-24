using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Qahr.Mobile.Database
{
   public static class GroupServerList
    {
       public static Server MyServerInfo = null;
       public static Server InterServer = null;
       public class Server
       {
           public uint ID;
           public uint MapID;
           public uint X;
           public uint Y;
           public uint Group;
           public string Name;
           public byte TransferType = 0;

           public string IPAddress = "";
           public ushort Port = 0;
       }
       public static System.SafeDictionary<uint, Server> GroupServers = new System.SafeDictionary<uint, Server>();

       public static Server[] CanTransferServers()
       {
           return GroupServers.Values.Where(p => p.ID != MyServerInfo.ID && MyServerInfo.TransferType == p.TransferType).ToArray();
       }

       public static Server GetServer(uint ID)
       {
           return GroupServers.Values.Where(p => p.ID == ID).FirstOrDefault();
       }
       public static Server GetServer(string IpAddres, ushort Port)
       {
           Server server = null;
           foreach (var _server in GroupServers.Values)
           {
               if (_server.IPAddress == IpAddres && Port == _server.Port)
               {
                   server = _server;
                   break;
               }
           }
           return server;
       }
       public static void Load()
       {
           string[] baseText = File.ReadAllLines(Program.ServerConfig.DbLocation + "client_config.ini");
           foreach (var bas_line in baseText)
           {
               Database.DBActions.ReadLine line = new DBActions.ReadLine(bas_line, ' ');
               Server obj = new Server();
               obj.ID = line.Read((uint)0);
               obj.Name = line.Read("");
               obj.MapID = line.Read((uint)0);
               obj.X = line.Read((uint)0);
               obj.Y = line.Read((uint)0);
               obj.TransferType = line.Read((byte)0);

               obj.Group = line.Read((uint)0);
               line.Read((uint)0);
               line.Read((uint)0);
               obj.IPAddress = line.Read("");
               obj.Port = line.Read((ushort)0);
               GroupServers.Add(obj.ID, obj);

               if (obj.Name == Program.ServerConfig.ServerName)
                   MyServerInfo = obj;
               if (obj.Name.Contains("Realm"))
                   InterServer = obj;
           }
           // QahrMobile: a ServerName with no row here left MyServerInfo null and every login crashed (LoadMap)
           if (MyServerInfo == null)
           {
               MyServerInfo = GroupServers.Values.Where(s => !s.Name.Contains("Realm")).OrderBy(s => s.ID).FirstOrDefault();
               MyConsole.WriteLine("[Server] ServerConfig.ini ServerName '" + Program.ServerConfig.ServerName + "' is not in Data\\client_config.ini"
                   + (MyServerInfo != null ? ": using row '" + MyServerInfo.Name + "'. Put the same name in both files." : " and the file has no server row."),
                   ConsoleColor.Red);
           }
       }
    }
}
