using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Qahr.Mobile.ServerTools
{
    /// <summary>
    /// Opens the ScriptHazem tools (tools\*\publish\*.exe) on the server PC.
    /// In game: "@tool" (owner account only) shows a dialog listing the tools; in the console: "tool" lists them,
    /// "tool &lt;number|name&gt;" opens one. The tools folder is found by walking up from the server exe
    /// (Sourse\tools), or set in Infinity[DB]\tools.ini: "Folder=D:\path\to\tools".
    /// A tool that is already running is brought to the front instead of opened twice.
    /// </summary>
    public static class ToolLauncher
    {
        public sealed class Tool
        {
            public string Name;
            public string Folder;
            public string Exe;
            public string Info;
        }

        // display order + short description (English: the game dialog font has no Arabic); missing tools are hidden
        private static readonly string[][] Known =
        {
            new[] { "PlayerStudio", "players: level, items, packages, teleport, ban" },
            new[] { "MapStudio", "maps, NPC positions, walkable cells, portals" },
            new[] { "ItemStudio", "items and shops" },
            new[] { "MonsterStudio", "monsters, drops, spawns" },
            new[] { "DialogStudio", "NPC dialogs" },
            new[] { "ConquerStudio", "client assets and 3D models" },
            new[] { "ServerControl", "server start/stop, logs, backups" },
            new[] { "RankStudio", "rankings web page" },
            new[] { "GmStudio", "GM commands: run or copy" },
            new[] { "PatchStudio", "official patch diff + what the server is missing" },
            new[] { "PacketStudio", "packet analyzer + world collector" },
            new[] { "LoaderStudio", "client loader + Server.dat" },
            new[] { "PatchBuilder", "build client patches + auto update" },
            new[] { "QuestStudio", "quest editor: objectives, rewards, chains" },
            new[] { "UiExplorer", "client windows: browse, preview, open live" },
            new[] { "DatCrypt", "encrypt/decrypt client .dat (items, skills, monsters)" },
            new[] { "ProtoStudio", "client protobuf schemas: browse, decode, build" },
            new[] { "FestivalStudio", "festivals: on/off, timed runs, schedules, timeline" },// [FestivalStudio 2026-10-15]
            new[] { "MallStudio", "CP Mall: tabs, Recommend, prices, client+server+patch" },// [MallStudio 2026-10-15]
            new[] { "CompensationStudio", "compensation mail: targets, rewards, history" },// [Compensation 2026-10-15]
        };

        public static string ToolsFolder()
        {
            try
            {
                string ini = Path.Combine(Program.ServerConfig.DbLocation ?? "", "tools.ini");
                if (File.Exists(ini))
                    foreach (var line in File.ReadAllLines(ini))
                    {
                        int eq = line.IndexOf('=');
                        if (eq > 0 && line.Substring(0, eq).Trim().Equals("Folder", StringComparison.OrdinalIgnoreCase))
                        {
                            string f = line.Substring(eq + 1).Trim().Trim('"');
                            if (Directory.Exists(f)) return f;
                        }
                    }
            }
            catch { }
            // ...\Sourse\Qahr\Qahr.GameServer\bin-net10\  ->  ...\Sourse\tools
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 6 && dir != null; i++, dir = dir.Parent)
            {
                string candidate = Path.Combine(dir.FullName, "tools");
                if (Directory.Exists(candidate) && Directory.Exists(Path.Combine(candidate, "_Brand")))
                    return candidate;
            }
            return null;
        }

        public static List<Tool> List()
        {
            var list = new List<Tool>();
            string root = ToolsFolder();
            if (root == null) return list;
            foreach (var k in Known)
            {
                string exe = Path.Combine(root, k[0], "publish", k[0] + ".exe");
                if (File.Exists(exe))
                    list.Add(new Tool { Name = k[0], Folder = Path.GetDirectoryName(exe), Exe = exe, Info = k[1] });
            }
            return list;
        }

        /// <summary>Find by 1-based number or (part of) the name.</summary>
        public static Tool Find(string key)
        {
            var list = List();
            if (int.TryParse(key, out int n) && n >= 1 && n <= list.Count) return list[n - 1];
            return list.FirstOrDefault(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                ?? list.FirstOrDefault(t => t.Name.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>Opens the tool (or focuses it if already running). Returns a short result message.</summary>
        public static string Open(Tool t)
        {
            if (t == null) return "Tool not found.";
            try
            {
                var running = Process.GetProcessesByName(t.Name)
                    .FirstOrDefault(p => { try { return p.MainWindowHandle != IntPtr.Zero; } catch { return false; } });
                if (running != null)
                {
                    IntPtr h = running.MainWindowHandle;
                    if (IsIconic(h)) ShowWindow(h, 9); // SW_RESTORE
                    SetForegroundWindow(h);
                    return t.Name + " is already open (brought to front).";
                }
                Process.Start(new ProcessStartInfo(t.Exe) { UseShellExecute = true, WorkingDirectory = t.Folder });
                return t.Name + " opened on the server PC.";
            }
            catch (Exception e)
            {
                return "Could not open " + t.Name + ": " + e.Message;
            }
        }

        // ---------------- console: "tool" / "tool <n|name>" ----------------
        public static void Console(string[] args)
        {
            var list = List();
            if (list.Count == 0)
            {
                MyConsole.WriteLine("[tool] No tools found. Put Folder=<tools path> in Infinity[DB]\\tools.ini", ConsoleColor.Yellow);
                return;
            }
            if (args.Length < 2)
            {
                MyConsole.WriteLine("[tool] ScriptHazem tools  ·  type: tool <number or name>", ConsoleColor.Yellow);
                for (int i = 0; i < list.Count; i++)
                    MyConsole.WriteLine("   " + (i + 1) + ". " + list[i].Name.PadRight(14) + " " + list[i].Info, ConsoleColor.Yellow);
                return;
            }
            MyConsole.WriteLine("[tool] " + Open(Find(args[1])), ConsoleColor.Green);
        }

        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] private static extern bool IsIconic(IntPtr hWnd);
    }
}
