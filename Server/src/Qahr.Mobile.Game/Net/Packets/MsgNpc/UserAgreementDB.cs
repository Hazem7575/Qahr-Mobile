using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Game.MsgNpc
{
    /// <summary>
    /// Persistent store for which characters have accepted the End User Agreement.
    /// Backed by a simple flat file (one character UID per line) in the DB folder, so
    /// the acceptance survives restarts without touching the player-save schema.
    /// </summary>
    public static class UserAgreementDB
    {
        private static readonly object _lock = new object();
        private static HashSet<uint> _agreed;

        private static string FilePath
        {
            get { return Program.ServerConfig.DbLocation + "Agreed.txt"; }
        }

        private static void EnsureLoaded()
        {
            if (_agreed != null)
                return;
            lock (_lock)
            {
                if (_agreed != null)
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
                    MyConsole.WriteLine("[UserAgreement] load failed: " + e);
                }
                _agreed = set;
            }
        }

        // [AgreementFix 2026-10-15] the reply to the login agreement is matched by "pending" instead of ActiveNpc,
        // which other login pushes can overwrite before the player clicks (the click was lost, Agreed.txt never written).
        private static readonly HashSet<uint> _pending = new HashSet<uint>();
        public static void MarkPending(uint uid) { lock (_lock) _pending.Add(uid); }
        public static bool TryHandlePending(Client.GameClient user, ServerSockets.Packet stream, byte option)
        {
            if (user == null || user.Player == null || (option != 1 && option != 2)) return false;
            lock (_lock) { if (!_pending.Remove(user.Player.UID)) return false; }
            NpcHandler.UserAgreementDialog(user, stream, option, "", (uint)NpcID.UserAgreement);
            return true;
        }

        public static bool HasAgreed(uint uid)
        {
            EnsureLoaded();
            lock (_lock)
                return _agreed.Contains(uid);
        }

        public static void SetAgreed(uint uid)
        {
            EnsureLoaded();
            lock (_lock)
            {
                if (!_agreed.Add(uid))
                    return;
                try
                {
                    File.AppendAllText(FilePath, uid + Environment.NewLine);
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[UserAgreement] save failed for " + uid + ": " + e);
                }
            }
        }
    }
}
