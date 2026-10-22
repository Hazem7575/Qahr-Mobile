using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: NPC bodies the mobile client draws differently (Data\mobile_npclooks.txt, "serverLook mobileLook name").
    /// The look is the Npcs.txt mesh / 10 (the last digit is the direction). The mobile client has no body for some
    /// PC NPCs, or has it under another number; it also shows the name of the body it draws, so the PC name is sent
    /// with the NPC. Reloaded with the other Data\mobile_*.txt files (MobileProtocol.WatchFiles).
    /// </summary>
    public static class MobileNpcLooks
    {
        class Entry
        {
            public uint Look;
            public string Name;
        }

        static Dictionary<uint, Entry> looks = new Dictionary<uint, Entry>();

        public static string Path
        {
            get { return System.IO.Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_npclooks.txt"); }
        }

        public static void Load()
        {
            var map = new Dictionary<uint, Entry>();
            try
            {
                if (File.Exists(Path))
                {
                    foreach (var raw in File.ReadAllLines(Path))
                    {
                        string line = raw;
                        int c = line.IndexOf('#');
                        if (c >= 0) line = line.Substring(0, c);
                        var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        uint server, mobile;
                        if (p.Length < 2 || !uint.TryParse(p[0], out server) || !uint.TryParse(p[1], out mobile))
                            continue;
                        map[server] = new Entry { Look = mobile, Name = p.Length > 2 ? p[2] : null };
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] " + Path + ": " + e.Message, ConsoleColor.Red);
            }
            looks = map;
        }

        /// <summary>The mesh the mobile client draws for a server NPC mesh, and the name to send with it (or null).</summary>
        public static uint Mesh(uint mesh, out string name)
        {
            name = null;
            uint look = mesh / 10, dir = mesh % 10;
            // NPCs with a player-like body (npcx.ini) are 1000000 + look * 10 + direction on the PC client
            // and the same number without the 1000000 on the mobile client (1051596 -> npcx [5159])
            uint mobileLook = look >= 100000 ? look - 100000 : look;
            Entry e;
            if (looks.TryGetValue(look, out e))
            {
                mobileLook = e.Look;
                name = e.Name;
            }
            return mobileLook * 10 + dir;
        }
    }
}
