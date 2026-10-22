// ============================================================================
//  [EventData 2026-10-15] LegacyBackend: lets the old per-festival player stores (FoolsKit.Store, AnnivKit.Store,
//  QixiKit.PlayerStore, the two HolidayKit.PlayerStore, SpringStore, FestPlayerStore) keep their public API while
//  their data lives in EventStore (MySQL). The old file is imported once (then renamed *.migrated).
// ============================================================================
using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.EventData
{
    public enum LegacyFormat { KvText, KvTextSkip0, JsonPlayersStrings, JsonPlayersCounters, JsonRootCounters }

    public sealed class LegacyBackend
    {
        public readonly string Event;
        readonly Func<string> _path;
        readonly LegacyFormat _fmt;
        volatile bool _migrated;
        long _lastTry;

        public LegacyBackend(string ev, Func<string> legacyPath, LegacyFormat fmt)
        {
            Event = ev.Length > EventStore.MaxEvent ? ev.Substring(0, EventStore.MaxEvent) : ev;
            _path = legacyPath; _fmt = fmt;
        }

        /// <summary>Event key of an old store: the folder, plus ".file" when the file is not the default one.</summary>
        public static string KeyFor(string folder, string file, string defaultFile)
        {
            string k = folder ?? "";
            if (!string.IsNullOrEmpty(file) && !string.Equals(file, defaultFile, StringComparison.OrdinalIgnoreCase))
                k += "." + Path.GetFileNameWithoutExtension(file);
            return k;
        }
        public static string KeyForPath(string file, string defaultFile)
        {
            return KeyFor(Path.GetFileName(Path.GetDirectoryName(file) ?? ""), Path.GetFileName(file), defaultFile);
        }

        public string LegacyPath { get { try { return _path(); } catch { return null; } } }

        /// <summary>Imports the old file once (cheap after the first success; retried every 5 s while MySQL is down).</summary>
        public void EnsureMigrated()
        {
            if (_migrated) return;
            long now = Environment.TickCount & 0x7FFFFFFF;
            if (_lastTry != 0 && now - _lastTry < 5000 && now >= _lastTry) return;
            lock (this)
            {
                if (_migrated) return;
                _lastTry = now == 0 ? 1 : now;
                string p = LegacyPath;
                if (string.IsNullOrEmpty(p) || !File.Exists(p)) { _migrated = true; return; }
                int r;
                switch (_fmt)
                {
                    case LegacyFormat.KvText: r = EventStore.MigrateKvText(Event, p, false); break;
                    case LegacyFormat.KvTextSkip0: r = EventStore.MigrateKvText(Event, p, true); break;
                    case LegacyFormat.JsonPlayersStrings: r = EventStore.MigrateJsonPlayers(Event, p, "players", false); break;
                    case LegacyFormat.JsonPlayersCounters: r = EventStore.MigrateJsonPlayers(Event, p, "players", true); break;
                    default: r = EventStore.MigrateJsonPlayers(Event, p, null, true); break;
                }
                if (r >= 0) _migrated = true;
            }
        }
        /// <summary>Test seam: look for the old file again.</summary>
        public void Recheck() { _migrated = false; _lastTry = 0; }
    }
}
