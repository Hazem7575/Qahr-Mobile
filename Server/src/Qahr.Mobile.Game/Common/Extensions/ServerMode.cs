using System;
using System.IO;

namespace Qahr.Mobile
{
    /// <summary>
    /// Server run mode: Development vs Production. Dangerous developer/test features (the packet injector, and any other
    /// "test tool" you gate on this) are only allowed in Development. Production is the SAFE DEFAULT: when the mode file
    /// is missing or unreadable, the server is treated as Production and those tools stay off.
    ///
    /// Config: Infinity[DB]\servermode.txt  — one line, either:
    ///   mode=development     (or dev)      -> test tools allowed
    ///   mode=production      (or prod)     -> test tools locked (default)
    /// A bare word "development"/"production" also works. Re-read at most every 5 seconds.
    /// </summary>
    public static class ServerMode
    {
        private static volatile bool _isDev;
        private static int _nextCheck;
        private static bool _checkedOnce;
        private static readonly object _lock = new object();

        /// <summary>True only when servermode.txt explicitly selects development. Default (missing/bad) = false (production).</summary>
        public static bool IsDevelopment
        {
            get { Refresh(); return _isDev; }
        }

        public static string Current { get { return IsDevelopment ? "development" : "production"; } }

        private static void Refresh()
        {
            int now = Environment.TickCount;
            if (_checkedOnce && now - _nextCheck < 0)
                return;
            lock (_lock)
            {
                if (_checkedOnce && now - _nextCheck < 0)
                    return;
                _checkedOnce = true;
                _nextCheck = now + 5000;
                bool dev = false;
                try
                {
                    string path = Program.ServerConfig.DbLocation + "servermode.txt";
                    if (File.Exists(path))
                    {
                        foreach (var raw in File.ReadAllLines(path))
                        {
                            string line = raw.Trim();
                            if (line.Length == 0 || line.StartsWith("#"))
                                continue;
                            int eq = line.IndexOf('=');
                            string val = (eq >= 0 ? line.Substring(eq + 1) : line).Trim().ToLowerInvariant();
                            if (val == "development" || val == "dev")
                                dev = true;
                            else if (val == "production" || val == "prod")
                                dev = false;
                            break; // first meaningful line wins
                        }
                    }
                }
                catch { dev = false; }
                _isDev = dev;
            }
        }
    }
}
