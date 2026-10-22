using System;
using System.Collections.Generic;
using System.IO;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: skills the mobile client knows under another magic type (Data\mobile_magic.txt,
    /// "serverSkill mobileSkill mobileMaxLevel name"). The mobile 1.1.2.3 has an older Wanderer (SwordDash 26210 ...)
    /// than the server (CliffCrusher 25670 ...): the server keeps its own skills and rules, the client is told the
    /// mobile id so it shows its own icon/animation, and the id the client casts is turned back into the server id.
    /// The two id sets do not overlap, so every conversion is safe to apply twice and needs no per-client state.
    /// Used by: SpellPacket.SpellCreate/GetSpell (2057), MsgAttackPacket.Interaction/InteractionCreate (2104),
    /// MsgSpellAnimation.CreateAnimation (2422), MsgMagicColdTime.MagicColdTimeCreate (2369), Spell.Remove (action 109).
    /// The file is re-read when it changes (checked at most every 2 s).
    /// </summary>
    public static class MobileMagic
    {
        class Entry
        {
            public ushort Server;
            public ushort Mobile;
            public ushort MaxLevel;
            public string Name;
        }

        static Dictionary<ushort, Entry> toMobile = new Dictionary<ushort, Entry>();
        static Dictionary<ushort, Entry> toServer = new Dictionary<ushort, Entry>();
        static DateTime loadedStamp = DateTime.MinValue;
        static DateTime nextCheck = DateTime.MinValue;
        static readonly object sync = new object();

        public static string Path
        {
            get { return System.IO.Path.Combine(Program.ServerConfig.DbLocation ?? "", "mobile_magic.txt"); }
        }

        static void Refresh()
        {
            DateTime now = DateTime.UtcNow;
            if (now < nextCheck)
                return;
            lock (sync)
            {
                if (now < nextCheck)
                    return;
                nextCheck = now.AddSeconds(2);
                try
                {
                    DateTime stamp = File.Exists(Path) ? File.GetLastWriteTimeUtc(Path) : DateTime.MinValue;
                    if (stamp != loadedStamp)
                    {
                        loadedStamp = stamp;
                        Load();
                    }
                }
                catch (Exception e)
                {
                    MyConsole.WriteLine("[Mobile] " + Path + ": " + e.Message, ConsoleColor.Red);
                }
            }
        }

        public static void Load()
        {
            var tm = new Dictionary<ushort, Entry>();
            var ts = new Dictionary<ushort, Entry>();
            try
            {
                if (File.Exists(Path))
                {
                    foreach (var raw in File.ReadAllLines(Path))
                    {
                        string line = raw;
                        int c = line.IndexOf('#');
                        if (c >= 0) line = line.Substring(0, c);
                        var p = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        ushort server, mobile, max = ushort.MaxValue;
                        if (p.Length < 2 || !ushort.TryParse(p[0], out server) || !ushort.TryParse(p[1], out mobile) || server == mobile)
                            continue;
                        int nameAt = 2;
                        if (p.Length > 2 && ushort.TryParse(p[2], out max))
                            nameAt = 3;
                        else
                            max = ushort.MaxValue;
                        var e = new Entry { Server = server, Mobile = mobile, MaxLevel = max, Name = p.Length > nameAt ? p[nameAt] : "" };
                        tm[server] = e;
                        ts[mobile] = e;
                    }
                }
                MyConsole.WriteLine("[Mobile] " + tm.Count + " skills translated from " + Path, ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[Mobile] " + Path + ": " + e.Message, ConsoleColor.Red);
            }
            toMobile = tm;
            toServer = ts;
        }

        /// <summary>Server magic type -> the mobile client's (unchanged when not in the table or already a mobile id).</summary>
        public static ushort ToMobile(ushort serverSkill)
        {
            Refresh();
            Entry e;
            return toMobile.TryGetValue(serverSkill, out e) ? e.Mobile : serverSkill;
        }

        public static uint ToMobile(uint serverSkill)
        {
            return serverSkill <= ushort.MaxValue ? ToMobile((ushort)serverSkill) : serverSkill;
        }

        public static ulong ToMobile(ulong serverSkill)
        {
            return serverSkill <= ushort.MaxValue ? ToMobile((ushort)serverSkill) : serverSkill;
        }

        /// <summary>Mobile magic type (as the client casts it) -> the server's.</summary>
        public static ushort ToServer(ushort mobileSkill)
        {
            Refresh();
            Entry e;
            return toServer.TryGetValue(mobileSkill, out e) ? e.Server : mobileSkill;
        }

        /// <summary>The level to show the mobile client for a server skill at this level (its magictype may have fewer
        /// levels). serverSkill is the server id (a mobile id is accepted too).</summary>
        public static ushort LevelToMobile(ushort serverSkill, ushort level)
        {
            Refresh();
            Entry e;
            if (!toMobile.TryGetValue(serverSkill, out e) && !toServer.TryGetValue(serverSkill, out e))
                return level;
            return level > e.MaxLevel ? e.MaxLevel : level;
        }

        /// <summary>True when the server skill is shown to the mobile client under another id.</summary>
        public static bool IsTranslated(ushort serverSkill)
        {
            Refresh();
            return toMobile.ContainsKey(serverSkill);
        }
    }
}
