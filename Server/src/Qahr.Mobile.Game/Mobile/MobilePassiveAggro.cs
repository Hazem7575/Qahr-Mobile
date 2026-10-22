using System;
using System.Collections.Concurrent;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: passive monsters (attack_user=0 in Data\Monsters\*.ini) do not pick a target by themselves, but
    /// fight back against a player who hit them, like the beginner monsters on the official client.
    /// MobileMonsterTune marks the passive types (reloadmonsters applies it live), the receive-attack code records the
    /// attacker, and the monster AI asks Skip() before acting for a player.
    /// </summary>
    public static class MobilePassiveAggro
    {
        const int FightBackSeconds = 15;

        static readonly ConcurrentDictionary<uint, bool> passiveTypes = new ConcurrentDictionary<uint, bool>();
        static readonly ConcurrentDictionary<ulong, DateTime> hits = new ConcurrentDictionary<ulong, DateTime>();

        public static void MarkType(uint familyId, bool passive)
        {
            if (passive)
                passiveTypes[familyId] = true;
            else
                passiveTypes.TryRemove(familyId, out _);
        }

        public static bool IsPassive(uint familyId)
        {
            return passiveTypes.ContainsKey(familyId);
        }

        /// <summary>A player hit the monster.</summary>
        public static void Hit(Game.MsgMonster.MonsterRole monster, Client.GameClient attacker)
        {
            try
            {
                if (monster == null || attacker == null || attacker.Player == null)
                    return;
                if (!IsPassive(monster.Family.ID))
                    return;
                hits[Key(monster.UID, attacker.Player.UID)] = DateTime.Now;
                if (hits.Count > 20000)
                    Cleanup();
            }
            catch { }
        }

        /// <summary>True when a passive monster must ignore this player (the player did not hit it lately).</summary>
        public static bool Skip(Game.MsgMonster.MonsterRole monster, Role.Player client)
        {
            try
            {
                if (monster == null || client == null || !IsPassive(monster.Family.ID))
                    return false;
                DateTime at;
                if (hits.TryGetValue(Key(monster.UID, client.UID), out at) && (DateTime.Now - at).TotalSeconds <= FightBackSeconds)
                    return false;
                if (monster.Target == client)
                    monster.Target = null;
                return true;
            }
            catch { return false; }
        }

        static ulong Key(uint monster, uint player)
        {
            return ((ulong)monster << 32) | player;
        }

        static void Cleanup()
        {
            DateTime old = DateTime.Now.AddSeconds(-FightBackSeconds * 2);
            foreach (var kv in hits)
                if (kv.Value < old)
                    hits.TryRemove(kv.Key, out _);
        }
    }
}
