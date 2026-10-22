using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Game.MsgServer
{
    /// <summary>
    /// Tracks which characters have already claimed the one-time reward for setting
    /// their secondary (account-security) password. Backed by a flat file in the DB
    /// folder so it survives restarts and can never be farmed via Forget/Set again.
    /// </summary>
    public static class SecondaryPasswordReward
    {
        private static readonly object _lock = new object();
        private static HashSet<uint> _rewarded;

        private static string FilePath
        {
            get { return Program.ServerConfig.DbLocation + "SecPassReward.txt"; }
        }

        private static void EnsureLoaded()
        {
            if (_rewarded != null)
                return;
            lock (_lock)
            {
                if (_rewarded != null)
                    return;
                var set = new HashSet<uint>();
                try
                {
                    if (File.Exists(FilePath))
                    {
                        foreach (var line in File.ReadAllLines(FilePath))
                        {
                            uint uid;
                            if (uint.TryParse(line.Trim(), out uid))
                                set.Add(uid);
                        }
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[SecPassReward] load failed: " + e);
                }
                _rewarded = set;
            }
        }

        public static bool HasRewarded(uint uid)
        {
            EnsureLoaded();
            lock (_lock)
                return _rewarded.Contains(uid);
        }

        public static void SetRewarded(uint uid)
        {
            EnsureLoaded();
            lock (_lock)
            {
                if (!_rewarded.Add(uid))
                    return;
                try
                {
                    File.AppendAllText(FilePath, uid + Environment.NewLine);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[SecPassReward] save failed for " + uid + ": " + e);
                }
            }
        }
    }
}
