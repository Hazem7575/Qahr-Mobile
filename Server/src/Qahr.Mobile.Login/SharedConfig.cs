// SharedConfig - reads the ONE central ServerConfig.ini (found by walking up from the exe folder to
// the source root) that the game server and this account server both share. Self-contained simple INI
// parser (AccServer's IniFile prepends the working dir, so it can't open an absolute path). Every lookup
// falls back to the given value if the central file or key is missing, so nothing breaks.
using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Login
{
    public static class SharedConfig
    {
        private static Dictionary<string, string> _map;
        private static bool _loaded;
        public static bool Found { get; private set; }

        private static void Load()
        {
            if (_loaded) return;
            _loaded = true;
            _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string file = null;
                for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir) && file == null; i++)
                {
                    // Canonical home is the shared folder (src\Qahr.Mobile.Shared\ServerConfig.ini); root is the fallback.
                    foreach (string p in new[]
                    {
                        Path.Combine(dir, "Qahr.Mobile.Shared", "ServerConfig.ini"),
                        Path.Combine(dir, "Shared", "ServerConfig.ini"),
                        Path.Combine(dir, "ServerConfig.ini"),
                    })
                    { if (File.Exists(p)) { file = p; break; } }
                    DirectoryInfo parent = Directory.GetParent(dir);
                    dir = parent != null ? parent.FullName : null;
                }
                if (file == null) return;

                string section = "";
                foreach (string raw in File.ReadAllLines(file))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;
                    if (line[0] == '[' && line.EndsWith("]")) { section = line.Substring(1, line.Length - 2).Trim(); continue; }
                    int eq = line.IndexOf('=');
                    if (eq <= 0) continue;
                    _map[section + "|" + line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
                }
                Found = true;
            }
            catch { }
        }

        public static string String(string section, string key, string fallback)
        {
            Load();
            if (!Found) return fallback;
            string v;
            return (_map.TryGetValue(section + "|" + key, out v) && !string.IsNullOrEmpty(v)) ? v : fallback;
        }

        public static int Int(string section, string key, int fallback)
        {
            int v;
            return int.TryParse(String(section, key, null), out v) ? v : fallback;
        }
    }
}
