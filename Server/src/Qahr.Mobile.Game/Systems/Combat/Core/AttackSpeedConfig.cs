// ============================================================================
//  [AttackSpeed] Data-driven melee/auto-attack pace, tuned from Infinity[DB]\attackspeed.txt - SWITCHABLE.
//
//  Background: Role\Instance\Equip.cs AttackSpeed(bool) returned the ms between swings. As written it was
//  effectively always 200 ms (a 200 floor clamp applied right after a 200 base), so weapon Frequency, the
//  ring and Agility had no effect. This class replaces that math with values read from a file, so the attack
//  pace can be tuned per class and per weapon type "like official", live, without a rebuild.
//
//  OFF by default (missing file OR atk_speed=0 = the old hard-coded behaviour runs instead). See
//  attackspeed.sample.txt. Equip.AttackSpeed only calls Compute() when Config.Enabled is true.
//
//  Config: Infinity[DB]\attackspeed.txt
//      atk_speed=0|1            master switch (0 = this file does nothing, old code runs)
//      base=800                 global base delay (ms) before any reduction
//      floor=200                never return less than this (ms); the skill overrides below ignore it
//      per_speed=1              ms removed per 1 point of weapon/ring Frequency (SpeedR+SpeedL+SpeedRing)
//      agility_div=2            delay -= Agility / agility_div   (0 = Agility ignored)
//      twohand_add=0            ms added when a two-handed weapon is equipped
//      cyclone=-150             ms delta while the Cyclone flag is up (0 = ignore). Added (keep it negative).
//      celestialdance=110       ABSOLUTE ms while CelestialDance is up (0 = ignore). Bypasses the floor.
//      spreadyourwings=100      ABSOLUTE ms while Spreadyourwings is up (0 = ignore). Bypasses the floor.
//      class.<name>.base/.per_speed/.floor     per-class override (ms). names: trojan warrior dune archer
//                                              ninja monk pirate dragonwarrior thunderstriker taoist water fire windwalker
//      weapon.<type>.base/.per_speed/.floor    per-weapon override (ms). types: unarmed blade sword backsword
//                                              dagger club scepter bow scythe katana rapier pistol knife solusblade monkweapon
//  Resolution for base / per_speed / floor: weapon override, else class override, else the global value.
//  Reload live: console "reloadatkspeed", in game "@atkspeed" (owner), file watch (hotreload.txt watch_atkspeed=1).
//
//  Safety:
//  - Pure read of the player's own stats; never touches client input and never throws into the attack path.
//  - A bad file changes nothing: Reload keeps the old Config and returns the parse error.
//  - The config object is immutable and swapped by reference (readers need no lock).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.Combat
{
    public sealed class AttackTuning
    {
        public int Base;
        public int PerSpeed;
        public int Floor;
        public bool HasBase, HasPer, HasFloor;
    }

    public sealed class AttackSpeedConfig
    {
        public bool Enabled;
        public bool FromFile;

        public int Base = 200;
        public int Floor = 200;
        public int PerSpeed = 1;
        public int AgilityDiv = 2;
        public int TwoHandAdd = 0;
        public int Cyclone = -150;
        public int CelestialDance = 110;
        public int SpreadWings = 100;

        public readonly Dictionary<string, AttackTuning> ByClass = new Dictionary<string, AttackTuning>(StringComparer.OrdinalIgnoreCase);
        public readonly Dictionary<string, AttackTuning> ByWeapon = new Dictionary<string, AttackTuning>(StringComparer.OrdinalIgnoreCase);

        public string Describe()
        {
            if (!Enabled)
                return "attack speed tuning OFF" + (FromFile ? "" : " (no attackspeed.txt)");
            return string.Format(CultureInfo.InvariantCulture,
                "attack speed ON: base {0}ms, floor {1}ms, per-speed {2}ms, agility/{3}, {4} class + {5} weapon overrides",
                Base, Floor, PerSpeed, AgilityDiv, ByClass.Count, ByWeapon.Count);
        }

        // ------------------------------------------------------------------ the math
        /// <summary>
        /// ms between swings for a player. Mirrors exactly what AttackStudio previews. Inputs come from Equip:
        /// the equipped weapon ids, the summed Frequency of both weapons + ring, Agility, and the three skill flags.
        /// Never throws (any lookup problem falls back to the global base).
        /// </summary>
        public int Compute(uint job, uint rightWeaponId, uint leftWeaponId, int speedR, int speedL, int speedRing,
                           int agility, bool cyclone, bool celestialDance, bool spreadWings)
        {
            int baseMs = Base, per = PerSpeed, floor = Floor;
            try
            {
                string cls = ClassKey(job);
                if (cls != null && ByClass.TryGetValue(cls, out var ct)) Apply(ct, ref baseMs, ref per, ref floor);
                string wp = WeaponKey(rightWeaponId != 0 ? rightWeaponId : leftWeaponId);
                if (wp != null && ByWeapon.TryGetValue(wp, out var wt)) Apply(wt, ref baseMs, ref per, ref floor);

                long delay = baseMs;
                delay -= (long)(speedR + speedL + speedRing) * per;
                if (AgilityDiv > 0) delay -= agility / AgilityDiv;
                if (TwoHandAdd != 0 && IsTwoHand(rightWeaponId)) delay += TwoHandAdd;
                if (cyclone && Cyclone != 0) delay += Cyclone;
                if (delay < floor) delay = floor;
                if (celestialDance && CelestialDance > 0) delay = CelestialDance;
                if (spreadWings && SpreadWings > 0) delay = SpreadWings;
                if (delay < 1) delay = 1;
                if (delay > 60000) delay = 60000;
                return (int)delay;
            }
            catch { return Math.Max(1, Base); }
        }

        static void Apply(AttackTuning t, ref int baseMs, ref int per, ref int floor)
        {
            if (t.HasBase) baseMs = t.Base;
            if (t.HasPer) per = t.PerSpeed;
            if (t.HasFloor) floor = t.Floor;
        }

        static bool IsTwoHand(uint id)
        {
            try { return id != 0 && Database.ItemType.IsTwoHand(id); } catch { return false; }
        }

        /// <summary>Config class key from the numeric Job id (see Database.AtributesStatus ranges).</summary>
        public static string ClassKey(uint job)
        {
            // explicit order: the Taoist band (10000-14058) contains Water (13xxx) and Fire (14xxx).
            if (Database.AtributesStatus.IsWater(job)) return "water";
            if (Database.AtributesStatus.IsFire(job)) return "fire";
            if (Database.AtributesStatus.IsTaoist(job)) return "taoist";
            if (Database.AtributesStatus.IsTrojan(job)) return "trojan";
            if (Database.AtributesStatus.IsWarrior(job)) return "warrior";
            if (Database.AtributesStatus.IsDune(job)) return "dune";
            if (Database.AtributesStatus.IsArcher(job)) return "archer";
            if (Database.AtributesStatus.IsNinja(job)) return "ninja";
            if (Database.AtributesStatus.IsMonk(job)) return "monk";
            if (Database.AtributesStatus.IsPirate(job)) return "pirate";
            if (Database.AtributesStatus.IsLee(job)) return "dragonwarrior";
            if (Database.AtributesStatus.IsThunderStriker(job)) return "thunderstriker";
            if (Database.AtributesStatus.IsWindWalker(job)) return "windwalker";
            return null;
        }

        /// <summary>Config weapon-type key from the equipped weapon item id (see Database.ItemType helpers).</summary>
        public static string WeaponKey(uint id)
        {
            if (id == 0) return "unarmed";
            try
            {
                if (Database.ItemType.IsBlade(id)) return "blade";
                if (Database.ItemType.IsBacksword(id)) return "backsword";
                if (Database.ItemType.IsSword(id)) return "sword";
                if (Database.ItemType.IsDagger(id)) return "dagger";
                if (Database.ItemType.IsScepter(id)) return "scepter";
                if (Database.ItemType.IsClub(id)) return "club";
                if (Database.ItemType.IsBow(id)) return "bow";
                if (Database.ItemType.IsScythe(id)) return "scythe";
                if (Database.ItemType.IsKatana(id)) return "katana";
                if (Database.ItemType.IsRapier(id)) return "rapier";
                if (Database.ItemType.IsPistol(id)) return "pistol";
                if (Database.ItemType.IsKnife(id)) return "knife";
                if (Database.ItemType.IsSolusBlade(id)) return "solusblade";
                if (Database.ItemType.IsMonkWeapon(id)) return "monkweapon";
            }
            catch { }
            return "other";
        }

        // ------------------------------------------------------------------ parsing
        public static AttackSpeedConfig Parse(string text, List<string> notes)
        {
            var c = new AttackSpeedConfig { FromFile = true };
            if (text == null) text = "";
            int lineNo = 0;
            foreach (string raw in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                lineNo++;
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#' || line[0] == ';') continue;
                int eq = line.IndexOf('=');
                if (eq <= 0) { notes?.Add($"line {lineNo}: ignored (no '='): {line}"); continue; }
                string key = line.Substring(0, eq).Trim().ToLowerInvariant();
                string val = line.Substring(eq + 1).Trim();

                try
                {
                    switch (key)
                    {
                        case "atk_speed":
                        case "enabled": c.Enabled = ParseBool(val); break;
                        case "base": c.Base = ParseInt(val, key); break;
                        case "floor": c.Floor = ParseInt(val, key); break;
                        case "per_speed": c.PerSpeed = ParseInt(val, key); break;
                        case "agility_div": c.AgilityDiv = ParseInt(val, key); break;
                        case "twohand_add": c.TwoHandAdd = ParseInt(val, key); break;
                        case "cyclone": c.Cyclone = ParseInt(val, key); break;
                        case "celestialdance": c.CelestialDance = ParseInt(val, key); break;
                        case "spreadyourwings": c.SpreadWings = ParseInt(val, key); break;
                        default:
                            if (!ParseOverride(c, key, val, key))
                                notes?.Add($"line {lineNo}: unknown key '{key}'");
                            break;
                    }
                }
                catch (FormatException) { throw; }
            }
            return c;
        }

        static bool ParseOverride(AttackSpeedConfig c, string key, string val, string full)
        {
            // class.<name>.<field>  or  weapon.<type>.<field>
            string[] p = key.Split('.');
            if (p.Length != 3) return false;
            Dictionary<string, AttackTuning> map;
            if (p[0] == "class") map = c.ByClass;
            else if (p[0] == "weapon") map = c.ByWeapon;
            else return false;
            if (!map.TryGetValue(p[1], out var t)) { t = new AttackTuning(); map[p[1]] = t; }
            switch (p[2])
            {
                case "base": t.Base = ParseInt(val, full); t.HasBase = true; return true;
                case "per_speed": t.PerSpeed = ParseInt(val, full); t.HasPer = true; return true;
                case "floor": t.Floor = ParseInt(val, full); t.HasFloor = true; return true;
                default: return false;
            }
        }

        static bool ParseBool(string v)
        {
            v = v.Trim().ToLowerInvariant();
            return v == "1" || v == "true" || v == "yes" || v == "on";
        }

        static int ParseInt(string v, string key)
        {
            if (int.TryParse(v.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int n)) return n;
            throw new FormatException($"'{key}': '{v}' is not a whole number");
        }

        // ------------------------------------------------------------------ live state + hot reload
        public static volatile AttackSpeedConfig Config = new AttackSpeedConfig();

        static string ConfigPath()
        {
            try { return global::Qahr.Mobile.HotReload.HotReloadManager.DbFile("attackspeed.txt"); }
            catch { return "attackspeed.txt"; }
        }

        /// <summary>Loaded once at startup (call from the same place FileDrops.Load is called, if any).</summary>
        public static void Load()
        {
            try
            {
                string path = ConfigPath();
                if (!File.Exists(path)) { Config = new AttackSpeedConfig(); return; }
                var notes = new List<string>();
                Config = Parse(File.ReadAllText(path, Encoding.UTF8), notes);
            }
            catch (Exception e)
            {
                Config = new AttackSpeedConfig();
                try { Console.WriteLine("[AttackSpeed] attackspeed.txt not loaded (tuning stays OFF): " + e.Message); } catch { }
            }
        }

        /// <summary>HotReload op "reloadatkspeed". A bad file changes nothing (the error is returned).</summary>
        public static void Reload(global::Qahr.Mobile.HotReload.ReloadResult result)
        {
            string path = ConfigPath();
            AttackSpeedConfig next;
            var notes = new List<string>();
            if (!File.Exists(path))
                next = new AttackSpeedConfig();
            else
            {
                byte[] data = global::Qahr.Mobile.HotReload.HotReloadManager.ReadStable(path, true);
                string text;
                using (var r = global::Qahr.Mobile.HotReload.HotReloadManager.TextOf(data)) text = r.ReadToEnd();
                try { next = Parse(text, notes); }
                catch (FormatException e) { throw new global::Qahr.Mobile.HotReload.HotReloadError("attackspeed.txt " + e.Message + ". Nothing was changed."); }
            }
            AttackSpeedConfig before = Config;
            Config = next;
            foreach (string s in notes) result.Note(s);
            if (before.Enabled != next.Enabled) result.Note("was: " + before.Describe());
            result.Counts["enabled"] = next.Enabled ? 1 : 0;
            result.Counts["classOverrides"] = next.ByClass.Count;
            result.Counts["weaponOverrides"] = next.ByWeapon.Count;
            result.Message = next.Describe() + ".";
        }
    }
}
