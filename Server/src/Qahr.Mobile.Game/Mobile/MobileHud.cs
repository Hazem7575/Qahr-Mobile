using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: server side of the mobile main-screen (HUD) editor (Mobile UiExplorer, "Screen layout" page).
    /// Settings: Data\mobile_hud.txt (re-read when the file changes, no restart):
    ///   hide_functions = 16,17        function ids of ini\luacfg\newfuncopen (16 Arena [1812-1001], 17 WebHub [1812-1007],
    ///                                 11 Trials [1812-6163], 5 Guild, 6 Forge ...). Their bit (id-1) is cleared in the
    ///                                 MsgAction 471 function mask, so the button stays locked/hidden for players below
    ///                                 the function's own level (newfuncopen enable_lev). Empty = everything open (long.MaxValue).
    ///   button.&lt;key&gt; = lua &lt;Func|arg&gt;      a Lua call sent back to the player
    ///   button.&lt;key&gt; = msg &lt;text&gt;          a system message to the player
    ///   button.&lt;key&gt; = teleport &lt;map&gt; &lt;x&gt; &lt;y&gt;
    ///   button.&lt;key&gt; = cmd &lt;@command&gt;     a chat command, staff accounts only (same check as GmStudio)
    /// A new HUD icon of kind "server" sends "&lt;/F&gt;ClientManager_Callback&lt;/N&gt;19991&lt;/S&gt;QahrHud_Cmd&lt;/N&gt;&lt;key&gt;"
    /// (Public_CallServerFunc(19991, ...); msg 19991 is not used by the client). Hook: LuaUi\LuaPush.HandleCallback.
    /// </summary>
    public static class MobileHud
    {
        public const uint LuaMsg = 19991;

        static readonly object sync = new object();
        static DateTime stamp = DateTime.MinValue;
        static long mask = long.MaxValue;
        static Dictionary<string, string> buttons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static string FilePath
        {
            get { return Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_hud.txt"); }
        }

        static void Refresh()
        {
            try
            {
                string p = FilePath;
                DateTime t = File.Exists(p) ? File.GetLastWriteTimeUtc(p) : DateTime.MinValue;
                lock (sync)
                {
                    if (t == stamp)
                        return;
                    stamp = t;
                    long m = long.MaxValue;
                    var b = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    if (t != DateTime.MinValue)
                    {
                        foreach (var raw in File.ReadAllLines(p))
                        {
                            string line = raw.Trim();
                            if (line.Length == 0 || line[0] == '#')
                                continue;
                            int eq = line.IndexOf('=');
                            if (eq <= 0)
                                continue;
                            string k = line.Substring(0, eq).Trim(), v = line.Substring(eq + 1).Trim();
                            if (k.Equals("hide_functions", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var s in v.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int id;
                                    if (int.TryParse(s, out id) && id >= 1 && id <= 63)
                                        m &= ~(1L << (id - 1));
                                }
                            }
                            else if (k.StartsWith("button.", StringComparison.OrdinalIgnoreCase) && k.Length > 7)
                                b[k.Substring(7)] = v;
                        }
                    }
                    mask = m;
                    buttons = b;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] mobile_hud.txt: " + e.Message, ConsoleColor.Red);
            }
        }

        /// <summary>MsgAction 471 dwParam3: long.MaxValue minus the hidden functions.</summary>
        public static long FunctionMask()
        {
            Refresh();
            return mask;
        }

        public static long FunctionMask(Client.GameClient c) { return MobileFunctionUnlock.MaskFor(c); }// QahrMobile: per player, Data\mobile_functions.txt (MobileFunctionUnlock.cs) minus hide_functions

        /// <summary>LuaPush.HandleCallback (msg 19991). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || func != "QahrHud_Cmd")
                return false;
            Refresh();
            string key = args != null && args.Length > 0 ? args[0].Trim() : "";
            string act;
            lock (sync)
            {
                if (!buttons.TryGetValue(key, out act))
                    act = null;
            }
            if (act == null)
                return true;
            try
            {
                int sp = act.IndexOf(' ');
                string kind = (sp < 0 ? act : act.Substring(0, sp)).ToLowerInvariant();
                string rest = sp < 0 ? "" : act.Substring(sp + 1).Trim();
                switch (kind)
                {
                    case "lua":
                        LuaUi.LuaPush.Send(c, rest);
                        break;
                    case "msg":
                        c.SendSysMesage(rest, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    case "teleport":
                        {
                            var p = rest.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                            uint map; ushort x, y;
                            if (p.Length >= 3 && uint.TryParse(p[0], out map) && ushort.TryParse(p[1], out x) && ushort.TryParse(p[2], out y))
                                c.Teleport(x, y, map);
                            break;
                        }
                    case "cmd":
                        if (AdminApi.AdminApiServer.CanUseGmCommands(c))
                            AdminApi.AdminApiServer.RunGmCommandCore(c, rest);
                        break;
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] HUD button " + key + ": " + e.Message, ConsoleColor.Red);
            }
            return true;
        }
    }
}
