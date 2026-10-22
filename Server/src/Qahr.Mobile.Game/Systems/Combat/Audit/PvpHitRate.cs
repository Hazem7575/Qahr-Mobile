using System;
using System.Globalization;
using System.IO;

namespace Qahr.Mobile.CombatAudit2
{
    /// <summary>
    /// [CombatAudit2 2026-10-14] PvP normal-attack (no skill) hit/miss roll used by Calculate.Base.Dodged(player, player).
    ///
    /// Before: return Rate(clamp(accuracy - 40 - dodge, 40, 100)) as "dodged", i.e. the clamped HIT rate of the TQ
    /// formula was used as the MISS chance: min 40% miss in PvP, and more Accuracy = MORE misses (Deadly Sight +200
    /// made every normal hit miss).
    ///
    /// Now:  hit% = clamp(BaseHit + AccWeight * accuracy - DodgeWeight * dodge, MinHit, MaxHit), miss = 100 - hit.
    ///   accuracy = attacker Status.Accuracy (weapon +N "Agility" column of ItemAdd.ini, ~0..46, + rune Deadly Sight 50..200)
    ///   dodge    = target Status.Dodge (boots itemtype dodge 0..~40 + boots +N 0..20)
    /// Every constant is DERIVED (the official server values are not in the client) and lives in
    /// Infinity[DB]\CombatAudit2\pvp_hitrate.txt (re-read when the file changes). Missing/bad file = the defaults below.
    /// </summary>
    public static class PvpHitRate
    {
        public sealed class Params
        {
            public bool Enabled = true;     // false = never miss (pure safety switch)
            public double BaseHit = 95;     // hit% at accuracy 0 vs dodge 0 (5% base miss)
            public double AccWeight = 0.5;  // +1 accuracy = +0.5 hit%
            public double DodgeWeight = 0.5;// +1 dodge    = -0.5 hit%
            public double MinHit = 50;      // worst case: 50% miss (only at very high dodge vs no accuracy)
            public double MaxHit = 100;     // enough accuracy = never miss
        }

        static volatile Params _p = new Params();
        static DateTime _stamp = DateTime.MinValue, _nextCheck = DateTime.MinValue;
        static readonly object _lock = new object();

        public static Params Current
        {
            get
            {
                DateTime now = DateTime.UtcNow;
                if (now < _nextCheck) return _p;
                lock (_lock)
                {
                    if (now < _nextCheck) return _p;
                    _nextCheck = now.AddSeconds(5);
                    try
                    {
                        string db = Program.ServerConfig.DbLocation ?? "";
                        if (db.Length > 0 && !db.EndsWith("\\") && !db.EndsWith("/")) db += "\\";
                        string path = db + "CombatAudit2\\pvp_hitrate.txt";
                        DateTime t = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
                        if (t != _stamp) { _p = t == DateTime.MinValue ? new Params() : Parse(File.ReadAllLines(path)); _stamp = t; }
                    }
                    catch { }
                }
                return _p;
            }
        }

        public static Params Parse(string[] lines)
        {
            var p = new Params();
            foreach (var raw in lines)
            {
                string l = raw.Trim();
                if (l.Length == 0 || l.StartsWith("#") || l.StartsWith(";")) continue;
                int eq = l.IndexOf('=');
                if (eq <= 0) continue;
                string k = l.Substring(0, eq).Trim().ToLowerInvariant();
                string v = l.Substring(eq + 1).Trim();
                int hash = v.IndexOf('#'); if (hash >= 0) v = v.Substring(0, hash).Trim();
                double d;
                if (k == "enabled") { p.Enabled = v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase); continue; }
                if (!double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out d) || double.IsNaN(d) || double.IsInfinity(d)) continue;
                switch (k)
                {
                    case "base_hit": p.BaseHit = d; break;
                    case "accuracy_weight": p.AccWeight = d; break;
                    case "dodge_weight": p.DodgeWeight = d; break;
                    case "min_hit": p.MinHit = d; break;
                    case "max_hit": p.MaxHit = d; break;
                }
            }
            // sanity: 0 <= min <= max <= 100, weights >= 0
            p.MaxHit = Math.Max(0, Math.Min(100, p.MaxHit));
            p.MinHit = Math.Max(0, Math.Min(p.MaxHit, p.MinHit));
            p.AccWeight = Math.Max(0, p.AccWeight);
            p.DodgeWeight = Math.Max(0, p.DodgeWeight);
            return p;
        }

        /// <summary>Hit chance in percent (0..100) for a normal PvP attack.</summary>
        public static double HitChance(long accuracy, long dodge, Params p = null)
        {
            p = p ?? Current;
            if (!p.Enabled) return 100;
            if (accuracy < 0) accuracy = 0;
            if (dodge < 0) dodge = 0;
            double hit = p.BaseHit + p.AccWeight * accuracy - p.DodgeWeight * dodge;
            return Math.Max(p.MinHit, Math.Min(p.MaxHit, hit));
        }

        /// <summary>True = the attack misses. roll01 is a uniform number in [0,1).</summary>
        public static bool Missed(long accuracy, long dodge, double roll01, Params p = null)
        {
            return roll01 * 100.0 >= HitChance(accuracy, dodge, p);
        }

        public static bool Dodged(Client.GameClient attacker, Client.GameClient target)
        {
            if (attacker == null || target == null || attacker.Status == null || target.Status == null) return false;
            return Missed(attacker.Status.Accuracy, target.Status.Dodge, Game.MsgServer.AttackHandler.Calculate.Base.FastRand.Next(0, 1000000) / 1000000.0);
        }
    }
}
