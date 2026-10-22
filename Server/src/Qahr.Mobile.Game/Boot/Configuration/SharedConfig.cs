// ============================================================================
//  SharedConfig - the ONE central config file for the whole server.
//  Both the game server and the account server read  ServerConfig.ini  (found
//  by walking up from the exe folder to the source root). Edit that single
//  file to change the DB connection etc. - never the source, never two files.
//
//  Every lookup takes a fallback value: if ServerConfig.ini (or the key) is
//  missing, the old per-server config is used, so nothing breaks.
// ============================================================================
using System;
using System.IO;

namespace Qahr.Mobile
{
    public static class SharedConfig
    {
        private static WindowsAPI.IniFile _ini;
        private static bool _loaded;
        public static bool Found { get; private set; }
        public static string Path { get; private set; }

        private static void Load()
        {
            if (_loaded) return;
            _loaded = true;
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
                {
                    // Canonical home is the shared folder (src\Qahr.Mobile.Shared\ServerConfig.ini); the root next to the
                    // tree is kept as a fallback so older layouts still work.
                    string[] candidates =
                    {
                        System.IO.Path.Combine(dir, "Qahr.Mobile.Shared", "ServerConfig.ini"),
                        System.IO.Path.Combine(dir, "Shared", "ServerConfig.ini"),
                        System.IO.Path.Combine(dir, "ServerConfig.ini"),
                    };
                    foreach (string p in candidates)
                    {
                        if (File.Exists(p))
                        {
                            Path = p;
                            _ini = new WindowsAPI.IniFile(p, true);
                            Found = true;
                            return;
                        }
                    }
                    DirectoryInfo parent = Directory.GetParent(dir);
                    dir = parent != null ? parent.FullName : null;
                }
            }
            catch { }
        }

        /// <summary>Shared value if the central file has it, otherwise the given fallback.</summary>
        public static string String(string section, string key, string fallback)
        {
            Load();
            if (!Found) return fallback;
            try { string v = _ini.ReadString(section, key, null); return string.IsNullOrEmpty(v) ? fallback : v; }
            catch { return fallback; }
        }

        public static int Int(string section, string key, int fallback)
        {
            Load();
            if (!Found) return fallback;
            try { return _ini.ReadInt32(section, key, fallback); }
            catch { return fallback; }
        }
    }
}
