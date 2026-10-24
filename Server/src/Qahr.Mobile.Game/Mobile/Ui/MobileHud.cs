using System;
using System.Collections.Generic;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: server side of the mobile main-screen (HUD) editor (Mobile UiExplorer, "Screen layout" page).
    /// Settings: Data\mobile_hud.txt (re-read when the file changes, no restart):
    ///   hide_functions = 16,17        function ids of ini\luacfg\newfuncopen (16 Arena [1812-1001], 17 WebHub [1812-1007],
    ///                                 11 Trials [1812-6163], 5 Guild, 6 Forge ...). Their bit (id-1) is cleared in the
    ///                                 MsgAction 471 function mask (MobileFunctionUnlock.MaskFor), so the button stays
    ///                                 locked/hidden. Empty = everything open.
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

        sealed class Config
        {
            public long Mask = long.MaxValue;
            public readonly Dictionary<string, string> Buttons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            public static Config Parse(string[] lines)
            {
                var cfg = new Config();
                foreach (var line in DataFile.Clean(lines))
                {
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                        continue;
                    string key = line.Substring(0, eq).Trim(), value = line.Substring(eq + 1).Trim();
                    if (key.Equals("hide_functions", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var s in value.Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            int id;
                            if (int.TryParse(s, out id) && id >= 1 && id <= 63)
                                cfg.Mask &= ~(1L << (id - 1));
                        }
                    }
                    else if (key.StartsWith("button.", StringComparison.OrdinalIgnoreCase) && key.Length > 7)
                        cfg.Buttons[key.Substring(7)] = value;
                }
                return cfg;
            }
        }

        static readonly DataFile<Config> Settings = new DataFile<Config>("mobile_hud.txt", "Hud", Config.Parse);

        /// <summary>long.MaxValue minus the functions the owner hid with hide_functions.</summary>
        public static long HiddenMask()
        {
            return Settings.Value.Mask;
        }

        /// <summary>LuaPush.HandleCallback (msg 19991). True = answered.</summary>
        public static bool OnLua(Client.GameClient c, string func, string[] args)
        {
            if (c == null || c.Player == null || func != "QahrHud_Cmd")
                return false;
            string key = args != null && args.Length > 0 ? args[0].Trim() : "";
            string action;
            if (!Settings.Value.Buttons.TryGetValue(key, out action))
                return true;
            try
            {
                int sp = action.IndexOf(' ');
                string kind = (sp < 0 ? action : action.Substring(0, sp)).ToLowerInvariant();
                string rest = sp < 0 ? "" : action.Substring(sp + 1).Trim();
                switch (kind)
                {
                    case "lua":
                        LuaUi.LuaPush.Send(c, rest);
                        break;
                    case "msg":
                        PlayerMsg.Tell(c, rest);
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
                MyConsole.WriteLine("[Hud] button " + key + ": " + e.Message, ConsoleColor.Red);
            }
            return true;
        }
    }
}
