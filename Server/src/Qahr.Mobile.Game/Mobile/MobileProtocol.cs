using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: the Conquer Online Mobile client (1.1.2.3) speaks the 7957 protocol with its own message ids
    /// (CMsgWalk 10205 instead of 2200, ...). Every packet is renumbered here, so the server code keeps its own ids:
    ///   incoming: Program.Game_Receive -> ToServer(), outgoing: SecuritySocket.Send -> ToMobile().
    /// The table is Data\mobile_ids.txt ("serverId mobileId name"), made from the client's CMsg* classes
    /// (D:\conquer\_private\comobile\out\idmap.txt); edits apply after "mobileids reload" or a restart.
    /// </summary>
    public static unsafe class MobileProtocol
    {
        public const ushort MsgConnect = 10124;

        static Dictionary<ushort, ushort> toServer = new Dictionary<ushort, ushort>();
        static Dictionary<ushort, ushort> toMobile = new Dictionary<ushort, ushort>();
        /// <summary>Mobile ids the client keeps as a C struct (Data\mobile_kinds.txt). A renumbered packet of this kind
        /// without a layout in Data\mobile_layouts\ is dropped (and reported once) instead of reaching the other side
        /// in the wrong format.</summary>
        static HashSet<ushort> structIds = new HashSet<ushort>();
        static readonly HashSet<string> reported = new HashSet<string>();

        /// <summary>CMsgMBAction 471 (CNewFunctionOpenMgr::SetFunctionFlag): a 64-bit mask of the opened menu functions
        /// (Guild, Lineage, Runes, Forge, Arena, ...). The mobile shows only the buttons whose bit is set; the server
        /// never sent it, so most buttons were missing. All functions are opened. Sent with every map entry.</summary>
        public const ushort ActionFunctionFlags = 471;

        public static void SendFunctionFlags(Client.GameClient client, ServerSockets.Packet stream)
        {
            client.Send(Game.MsgServer.MsgDataPacket.ActionCreate(stream, new Game.MsgServer.ActionQuery
            {
                ObjId = client.Player.UID,
                dwParam3 = (ulong)MobileHud.FunctionMask(client),// QahrMobile: per player (Data\mobile_functions.txt, MobileFunctionUnlock.cs) minus Data\mobile_hud.txt hide_functions
                Timestamp = 1,
                Type = ActionFunctionFlags
            }));
        }

        public static bool IsStruct(ushort mobileId)
        {
            return structIds.Contains(mobileId);
        }

        /// <summary>Server ids never sent to the mobile client: Data\mobile_block.txt, one id per line (optional; for
        /// finding which packet causes a client effect).</summary>
        static HashSet<ushort> blockedIds = new HashSet<ushort>();

        public static bool IsBlocked(ushort serverId)
        {
            return blockedIds.Count != 0 && blockedIds.Contains(serverId);
        }

        /// <summary>A packet that still needs a layout: dropped; written to the console once per id and direction.</summary>
        public static void ReportDrop(ushort serverId, ushort mobileId, string direction)
        {
            string key = direction + serverId;
            lock (reported)
            {
                if (!reported.Add(key))
                    return;
            }
            MyConsole.WriteLine("[Mobile] no layout, dropped " + direction + " " + serverId + " (mobile " + mobileId + ")", ConsoleColor.Yellow);
        }

        public static string TablePath
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_ids.txt"); }
        }

        static List<FileSystemWatcher> watchers;
        static System.Threading.Timer reloadTimer;

        /// <summary>Reload Data\mobile_*.txt and Data\mobile_layouts\*.txt about a second after one of them is saved.</summary>
        public static void WatchFiles()
        {
            if (watchers != null)
                return;
            try
            {
                reloadTimer = new System.Threading.Timer(_ =>
                {
                    Load();
                    MobileLayouts.Load();
                    MobileNpcLooks.Load();
                }, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                watchers = new List<FileSystemWatcher> { Watch(Program.ServerConfig.DbLocation, "mobile_*.txt") };
                if (Directory.Exists(MobileLayouts.Folder))
                    watchers.Add(Watch(MobileLayouts.Folder, "*.txt"));
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] file watch failed: " + e.Message, ConsoleColor.Red);
            }
        }

        static FileSystemWatcher Watch(string folder, string filter)
        {
            var w = new FileSystemWatcher(folder, filter);
            FileSystemEventHandler h = (s, e) => reloadTimer.Change(1000, System.Threading.Timeout.Infinite);
            w.Changed += h;
            w.Created += h;
            w.Deleted += h;
            w.Renamed += (s, e) => reloadTimer.Change(1000, System.Threading.Timeout.Infinite);
            w.EnableRaisingEvents = true;
            return w;
        }

        public static void Load()
        {
            var ts = new Dictionary<ushort, ushort>();
            var tm = new Dictionary<ushort, ushort>();
            try
            {
                foreach (var raw in File.ReadAllLines(TablePath))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line[0] == '#' || line[0] == ';')
                        continue;
                    var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    ushort s, m;
                    if (p.Length < 2 || !ushort.TryParse(p[0], out s) || !ushort.TryParse(p[1], out m) || s == m)
                        continue;
                    ts[m] = s;
                    // "serverId mobileId in ...": an incoming-only alias (a second mobile id the client sends for the
                    // same server packet, e.g. CMsgRouletteBet 10187 -> 2348); the server keeps sending its main id
                    if (p.Length >= 3 && p[2] == "in")
                        continue;
                    tm[s] = m;
                }
                MyConsole.WriteLine("[Mobile] " + tm.Count + " renumbered packets from " + TablePath, ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] " + TablePath + " could not be read: " + e.Message, ConsoleColor.Red);
            }
            toServer = ts;
            toMobile = tm;

            var st = new HashSet<ushort>();
            try
            {
                foreach (var raw in File.ReadAllLines(Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_kinds.txt")))
                {
                    var p = raw.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    ushort m;
                    if (p.Length >= 3 && ushort.TryParse(p[0], out m) && p[2] == "struct")
                        st.Add(m);
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] mobile_kinds.txt could not be read: " + e.Message, ConsoleColor.Red);
            }
            structIds = st;

            var bl = new HashSet<ushort>();
            string blockPath = Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_block.txt");
            if (File.Exists(blockPath))
            {
                foreach (var raw in File.ReadAllLines(blockPath))
                {
                    var p = raw.Split(new[] { ' ', '\t', '#' }, StringSplitOptions.RemoveEmptyEntries);
                    ushort s;
                    if (p.Length > 0 && ushort.TryParse(p[0], out s))
                        bl.Add(s);
                }
                if (bl.Count != 0)
                    MyConsole.WriteLine("[Mobile] " + bl.Count + " server packet ids blocked by " + blockPath, ConsoleColor.Yellow);
            }
            blockedIds = bl;
            lock (reported) reported.Clear();
        }

        /// <summary>Mobile id -> server id, written back into the packet.</summary>
        public static ushort ToServer(ServerSockets.Packet stream, ushort id)
        {
            ushort s;
            if (!toServer.TryGetValue(id, out s))
                return id;
            *(ushort*)(stream.Memory + 2) = s;
            return s;
        }

        /// <summary>The mobile client's id of a server packet id (the same id when it is not renumbered).</summary>
        public static ushort MobileId(ushort serverId)
        {
            ushort m;
            return toMobile.TryGetValue(serverId, out m) ? m : serverId;
        }

        /// <summary>Server id -> mobile id in place; returns the server id to restore after the send (or 0).</summary>
        public static ushort ToMobile(ServerSockets.Packet msg)
        {
            if (msg == null || msg.Size < 4)
                return 0;
            ushort id = *(ushort*)(msg.Memory + 2);
            ushort m;
            if (!toMobile.TryGetValue(id, out m))
                return 0;
            *(ushort*)(msg.Memory + 2) = m;
            return id;
        }

        public static void Restore(ServerSockets.Packet msg, ushort serverId)
        {
            if (serverId != 0)
                *(ushort*)(msg.Memory + 2) = serverId;
        }

        /// <summary>
        /// 10124 CMsgConnect: ticket halves at 4 and 8 (the two values of the account server's 1055 reply).
        /// Same checks as MsgLoginClient.LoginGameEx, without the PC client's TQProtect query.
        /// </summary>
        public static void Login(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.FullLoading || stream.Size < 12)
                return;
            stream.Seek(4);
            uint lo = stream.ReadUInt32();
            uint hi = stream.ReadUInt32();

            uint[] decrypted = Pool.transferCipher.Decrypt(new uint[] { lo, hi });
            ulong loginToken = ((ulong)decrypted[1] << 32) | decrypted[0];
            uint entityId, state;
            if (!Game.MsgServer.LoginTokenStore.TryConsume(loginToken, client.Socket != null ? client.Socket.RemoteIp : "NONE", out entityId, out state))
            {
                MyConsole.WriteLine("[Mobile] invalid or expired login token from " + (client.Socket != null ? client.Socket.RemoteIp : "?"), ConsoleColor.Yellow);
                client.Socket.Disconnect();
                return;
            }
            // The mobile client sends no machine name / MAC / HWID. Empty values were the same for every mobile player,
            // so the "3 accounts per device" limits (MsgLoginClient) allowed only 3 mobile players in total and a ban by
            // device (SystemBannedPC) would have hit all of them. Each mobile login gets its own id instead.
            string device = "MOBILE-" + entityId;
            client.OnLogin = new Game.MsgServer.MsgLoginClient()
            {
                Key = entityId,
                HDSerial = entityId,// SystemBannedPC.Save skips bans without a serial
                AccountHash = state,
                ClienExeUID = 1,
                MachineName = device,
                MacAddress = device,
                HWID = device,
                Language = "En",
            };
            client.ClientFlag |= Client.ServerFlag.OnLoggion;
            Game.MsgServer.MsgLoginClient.LoginHandler(client, client.OnLogin);
        }
    }
}
