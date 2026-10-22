// ============================================================================
//  [FileDrops] Per-monster drop table from Monsters\*.ini (drop_* fields + [SpecialDrop]) - SWITCHABLE.
//
//  History: the original kill handler called Family.ItemGenerator.GenerateItem() inside a 5% roll; that call
//  is commented out in MonsterRole.Dead (#region DropItem) and every real drop is hard-coded there instead.
//  This class brings the file table back, OFF by default, IN ADDITION to the hard-coded drops (never instead).
//
//  Config: Infinity[DB]\drops.txt (missing file = OFF = the old behaviour). See drops.sample.txt.
//      file_drops=0|1          master switch (0 = nothing here runs)
//      rate_multiplier=1.0     scales item_chance / money_chance and divides the [SpecialDrop] "every N" (0.01..20)
//      max_items_per_kill=3    hard cap on floor items this class puts down per kill (money included, 0..10)
//      special_drop=0|1        [SpecialDrop] counters (every N-th kill of that monster type, server wide)
//      item_chance=5           % per kill for one table item (equipment from drop_* levels, or drop_hp/drop_mp)
//      money_chance=20         % per kill for silver: random between drop_money/2 and drop_money (0 = never)
//      event_monsters=0        1 = also monsters spawned with RemoveOnDead (events/summons). Default skip.
//  Reload live: console "reloaddrops", in game "@reloaddrops" (owner), AdminApi op "reloaddrops".
//
//  Safety:
//  - No client input: runs only from MonsterRole.Dead (server side kill), after the hard-coded drops.
//  - Bosses (Family.Boss != 0) are skipped like the hard-coded DropItem region; floor/trap mobs return earlier.
//  - Only ids that exist in itemtype (Pool.ItemsBase) are dropped; equipment ids above the max level are refused.
//  - Items are owned by the killer with the normal protection (MsgItem ToMySelf), exactly like DropItemID.
//  - Never throws into the kill path: any error is caught and logged ONCE (then counted silently).
//  - Counters are Interlocked; the config is an immutable object swapped by reference.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace Qahr.Mobile.Game.MsgMonster
{
    public sealed class FileDropConfig
    {
        public bool Enabled;
        public double RateMultiplier = 1.0;
        public int MaxItemsPerKill = 3;
        public bool SpecialDrop;
        public double ItemChance = 5;
        public double MoneyChance = 20;
        public bool EventMonsters;
        public bool FromFile;

        public string Describe()
        {
            if (!Enabled)
                return "file drops OFF" + (FromFile ? "" : " (no drops.txt)");
            return string.Format(CultureInfo.InvariantCulture,
                "file drops ON: rate x{0}, max {1}/kill, item {2}%, money {3}%, special {4}, event mobs {5}",
                RateMultiplier, MaxItemsPerKill, Math.Min(100, ItemChance * RateMultiplier), Math.Min(100, MoneyChance * RateMultiplier),
                SpecialDrop ? "on" : "off", EventMonsters ? "on" : "off");
        }
    }

    public static class FileDrops
    {
        public static volatile FileDropConfig Config = new FileDropConfig();

        private static readonly ConcurrentDictionary<string, StrongBox> _special = new ConcurrentDictionary<string, StrongBox>();
        private static int _errorLogged;
        private static int _errors;
        public static long Kills, ItemsDropped, MoneyDropped, SpecialsDropped, MoneyTotal; // stats (tests / info)

        private sealed class StrongBox { public int Value; }

        private static string ConfigPath()
        {
            string db = Program.ServerConfig.DbLocation ?? "";
            if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
            return db + "drops.txt";
        }

        // ------------------------------------------------------------------ config
        /// <summary>Parses drops.txt text. Throws FormatException with a readable message on a bad value.</summary>
        public static FileDropConfig Parse(string text, List<string> notes)
        {
            var c = new FileDropConfig { FromFile = true };
            int n = 0;
            foreach (string raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                n++;
                string line = raw.Trim();
                int hash = line.IndexOf('#');
                if (hash >= 0) line = line.Substring(0, hash).Trim();
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("//")) continue;
                int eq = line.IndexOf('=');
                if (eq < 0) throw new FormatException("line " + n + ": expected key=value");
                string k = line.Substring(0, eq).Trim().ToLowerInvariant();
                string v = line.Substring(eq + 1).Trim();
                switch (k)
                {
                    case "file_drops": c.Enabled = Bool(v, k, n); break;
                    case "special_drop": c.SpecialDrop = Bool(v, k, n); break;
                    case "event_monsters": c.EventMonsters = Bool(v, k, n); break;
                    case "rate_multiplier": c.RateMultiplier = Num(v, k, n, 0.01, 20); break;
                    case "max_items_per_kill": c.MaxItemsPerKill = (int)Num(v, k, n, 0, 10); break;
                    case "item_chance": c.ItemChance = Num(v, k, n, 0, 100); break;
                    case "money_chance": c.MoneyChance = Num(v, k, n, 0, 100); break;
                    default:
                        if (notes != null) notes.Add("line " + n + ": unknown key '" + k + "' ignored");
                        break;
                }
            }
            return c;
        }

        private static bool Bool(string v, string k, int n)
        {
            v = v.ToLowerInvariant();
            if (v == "1" || v == "true" || v == "yes" || v == "on") return true;
            if (v == "0" || v == "false" || v == "no" || v == "off") return false;
            throw new FormatException("line " + n + ": " + k + " must be 0 or 1");
        }

        private static double Num(string v, string k, int n, double min, double max)
        {
            double d;
            if (!double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out d) || double.IsNaN(d) || double.IsInfinity(d))
                throw new FormatException("line " + n + ": " + k + " is not a number");
            if (d < min || d > max)
                throw new FormatException("line " + n + ": " + k + " must be between " + min.ToString(CultureInfo.InvariantCulture) + " and " + max.ToString(CultureInfo.InvariantCulture));
            return d;
        }

        /// <summary>Startup: missing drops.txt = OFF. A bad file keeps file drops OFF and prints why. Never throws.</summary>
        public static void Load()
        {
            try
            {
                string path = ConfigPath();
                if (!File.Exists(path))
                {
                    Config = new FileDropConfig();
                    MyConsole.WriteLine("[FileDrops] " + Config.Describe() + ".", ConsoleColor.DarkGray);
                    return;
                }
                var notes = new List<string>();
                Config = Parse(File.ReadAllText(path, Encoding.UTF8), notes);
                MyConsole.WriteLine("[FileDrops] " + Config.Describe() + ".", Config.Enabled ? ConsoleColor.Cyan : ConsoleColor.DarkGray);
                foreach (string s in notes) MyConsole.WriteLine("[FileDrops] " + s, ConsoleColor.Yellow);
            }
            catch (Exception e)
            {
                Config = new FileDropConfig();
                MyConsole.WriteLine("[FileDrops] drops.txt not loaded (file drops stay OFF): " + e.Message, ConsoleColor.Yellow);
            }
        }

        /// <summary>HotReload op "reloaddrops". A bad file changes nothing (the error is returned).</summary>
        public static void Reload(global::Qahr.Mobile.HotReload.ReloadResult result)
        {
            string path = ConfigPath();
            FileDropConfig next;
            var notes = new List<string>();
            if (!File.Exists(path))
                next = new FileDropConfig();
            else
            {
                byte[] data = global::Qahr.Mobile.HotReload.HotReloadManager.ReadStable(path, true);
                string text;
                using (var r = global::Qahr.Mobile.HotReload.HotReloadManager.TextOf(data)) text = r.ReadToEnd();
                try { next = Parse(text, notes); }
                catch (FormatException e) { throw new global::Qahr.Mobile.HotReload.HotReloadError("drops.txt " + e.Message + ". Nothing was changed."); }
            }
            FileDropConfig before = Config;
            Config = next;
            foreach (string s in notes) result.Note(s);
            if (before.Enabled != next.Enabled)
                result.Note("was: " + before.Describe());
            result.Counts["enabled"] = next.Enabled ? 1 : 0;
            result.Message = next.Describe() + ".";
        }

        // ------------------------------------------------------------------ kill hook
        /// <summary>Called from MonsterRole.Dead after the hard-coded drops. Never throws.</summary>
        public static void OnKill(MonsterRole mob, Client.GameClient killer, ServerSockets.Packet stream)
        {
            FileDropConfig cfg = Config;
            if (cfg == null || !cfg.Enabled || cfg.MaxItemsPerKill <= 0) return;
            try
            {
                if (mob == null || killer == null || killer.Player == null || stream == null) return;
                MonsterFamily fam = mob.Family;
                Role.GameMap map = mob.GMap;
                if (fam == null || map == null || mob.IsFloor) return;
                if (fam.Boss != 0 || mob.Boss != 0) return;                 // same as the hard-coded DropItem region
                if (mob.RemoveOnDead && !cfg.EventMonsters) return;         // event / summoned spawns
                Interlocked.Increment(ref Kills);

                // the master type carries the live (hot-reloaded) [SpecialDrop] table
                MonsterFamily master;
                if (!Pool.MonsterFamilies.TryGetValue(fam.ID, out master) || master == null) master = fam;

                int left = cfg.MaxItemsPerKill;
                double mult = cfg.RateMultiplier;

                // 1) [SpecialDrop]: every N-th kill of this monster type (N scaled by 1/rate_multiplier)
                if (cfg.SpecialDrop && master.DropSpecials != null)
                {
                    var specials = master.DropSpecials;
                    for (int i = 0; i < specials.Length && left > 0; i++)
                    {
                        uint id = specials[i].ID;
                        int tick = specials[i].Rate != null ? specials[i].Rate.Tick : 0;
                        if (id == 0 || tick <= 0) continue;
                        Database.ItemType.DBItem db;
                        if (!Pool.ItemsBase.TryGetValue(id, out db) || db == null) continue;
                        int every = Math.Max(1, (int)Math.Round(tick / mult, MidpointRounding.AwayFromZero));
                        var box = _special.GetOrAdd(fam.ID + ":" + i + ":" + id, _ => new StrongBox());
                        if ((Interlocked.Increment(ref box.Value) & int.MaxValue) % every != 0) continue;
                        if (Drop(mob, map, killer, stream, id, db, true, (byte)(id % 10)))
                        {
                            left--;
                            Interlocked.Increment(ref SpecialsDropped);
                        }
                    }
                }

                // 2) one table item (equipment by the drop_* levels, or the drop_hp / drop_mp potion)
                if (left > 0 && Roll(cfg.ItemChance * mult) && fam.ItemGenerator != null)
                {
                    byte quality;
                    Database.ItemType.DBItem db;
                    uint id = fam.ItemGenerator.GenerateFileItem(out quality, out db);
                    if (id != 0 && db != null && Drop(mob, map, killer, stream, id, db, false, quality))
                    {
                        left--;
                        Interlocked.Increment(ref ItemsDropped);
                    }
                }

                // 3) silver: drop_money/2 .. drop_money
                if (left > 0 && fam.DropMoney > 0 && Roll(cfg.MoneyChance * mult))
                {
                    int max = fam.DropMoney;
                    int amount = Program.GetRandom.Next(Math.Max(1, max / 2), max + 1);
                    if (amount > 0 && DropMoney(mob, map, killer, stream, (uint)amount))
                    {
                        left--;
                        Interlocked.Increment(ref MoneyDropped);
                        Interlocked.Add(ref MoneyTotal, amount);
                    }
                }
            }
            catch (Exception e)
            {
                Interlocked.Increment(ref _errors);
                if (Interlocked.Exchange(ref _errorLogged, 1) == 0)
                {
                    try
                    {
                        MyConsole.WriteLine("[FileDrops] error in the kill drop (logged once, the kill itself is fine): " + e.Message, ConsoleColor.Yellow);
                        MyConsole.SaveException(e);
                    }
                    catch { }
                }
            }
        }

        public static int Errors { get { return _errors; } }

        /// <summary>Test helper: forget the [SpecialDrop] counters and the stats.</summary>
        public static void ResetCounters()
        {
            _special.Clear();
            Kills = ItemsDropped = MoneyDropped = SpecialsDropped = MoneyTotal = 0;
        }

        private static bool Roll(double percent)
        {
            if (percent <= 0) return false;
            if (percent >= 100) return true;
            return Program.GetRandom.Next(0, 1000000) < (int)(percent * 10000);
        }

        private static bool Place(MonsterRole mob, Role.GameMap map, out ushort x, out ushort y)
        {
            x = mob.X;
            y = mob.Y;
            return map.AddGroundItem(ref x, ref y, 3);
        }

        private static void Unplace(Role.GameMap map, ushort x, ushort y)
        {
            try { map.cells[x, y] &= ~Role.MapFlagType.Item; } catch { }
        }

        private static bool Drop(MonsterRole mob, Role.GameMap map, Client.GameClient killer, ServerSockets.Packet stream,
            uint id, Database.ItemType.DBItem db, bool special, byte quality)
        {
            ushort x, y;
            if (!Place(mob, map, out x, out y)) return false;
            bool ok = false;
            try { ok = mob.DropFileItem(stream, killer, map, id, x, y, special, quality, db); }
            finally { if (!ok) Unplace(map, x, y); } // never leave a reserved cell behind (also on an exception)
            return ok;
        }

        private static bool DropMoney(MonsterRole mob, Role.GameMap map, Client.GameClient killer, ServerSockets.Packet stream, uint amount)
        {
            ushort x, y;
            if (!Place(mob, map, out x, out y)) return false;
            bool ok = false;
            try { ok = mob.DropFileMoney(stream, killer, map, amount, x, y); }
            finally { if (!ok) Unplace(map, x, y); }
            return ok;
        }
    }
}
