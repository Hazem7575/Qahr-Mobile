using System;
using System.Collections.Generic;

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
    /// </summary>
    public static class MobileMagic
    {
        sealed class Entry
        {
            public ushort Server;
            public ushort Mobile;
            public ushort MaxLevel;
            public string Name;
        }

        sealed class Table
        {
            public readonly Dictionary<ushort, Entry> ToMobile = new Dictionary<ushort, Entry>();
            public readonly Dictionary<ushort, Entry> ToServer = new Dictionary<ushort, Entry>();

            public static Table Parse(string[] lines)
            {
                var t = new Table();
                foreach (var line in DataFile.Clean(lines))
                {
                    var p = DataFile.Words(line);
                    ushort server, mobile, max;
                    if (p.Length < 2 || !ushort.TryParse(p[0], out server) || !ushort.TryParse(p[1], out mobile) || server == mobile)
                        continue;
                    int nameAt = 2;
                    if (p.Length > 2 && ushort.TryParse(p[2], out max))
                        nameAt = 3;
                    else
                        max = ushort.MaxValue;
                    var e = new Entry { Server = server, Mobile = mobile, MaxLevel = max, Name = p.Length > nameAt ? p[nameAt] : "" };
                    t.ToMobile[server] = e;
                    t.ToServer[mobile] = e;
                }
                MyConsole.WriteLine("[Mobile] " + t.ToMobile.Count + " skills translated (mobile_magic.txt)", ConsoleColor.Cyan);
                return t;
            }
        }

        static readonly DataFile<Table> Skills = new DataFile<Table>("mobile_magic.txt", "MobileMagic", Table.Parse);

        /// <summary>Server magic type -> the mobile client's (unchanged when not in the table or already a mobile id).</summary>
        public static ushort ToMobile(ushort serverSkill)
        {
            Entry e;
            return Skills.Value.ToMobile.TryGetValue(serverSkill, out e) ? e.Mobile : serverSkill;
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
            Entry e;
            return Skills.Value.ToServer.TryGetValue(mobileSkill, out e) ? e.Server : mobileSkill;
        }

        /// <summary>The level to show the mobile client for a server skill at this level (its magictype may have fewer
        /// levels). serverSkill is the server id (a mobile id is accepted too).</summary>
        public static ushort LevelToMobile(ushort serverSkill, ushort level)
        {
            var t = Skills.Value;
            Entry e;
            if (!t.ToMobile.TryGetValue(serverSkill, out e) && !t.ToServer.TryGetValue(serverSkill, out e))
                return level;
            return level > e.MaxLevel ? e.MaxLevel : level;
        }

        /// <summary>True when the server skill is shown to the mobile client under another id.</summary>
        public static bool IsTranslated(ushort serverSkill)
        {
            return Skills.Value.ToMobile.ContainsKey(serverSkill);
        }
    }
}
