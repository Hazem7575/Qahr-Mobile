// ============================================================================
//  [NewYear2023 COMPLETE 2026-10-15] One entry point per server hook for the Spring Festival group
//  (LunarNewYear2023, SpringFestival2024, WhiteSnakeLegend, SpringFestival2026). Every method never throws.
//    LuaPush.HandleCallback      OnLua        (the NewYear2023 line: msg 10170 + the draw window's 10250 / 10139 calls)
//    Procesor (NPC in view)      OnNpc        before FestivalManager.OnNpc (daily NPCs of the group's festivals)
//    Procesor (item dialogs)     OnItemDialog before the in-screen NPC search (npcid = item id)
//    MsgItemUsuagePacket.UseItem OnUse        the group's boxes / event items
//    MonsterRole.Dead            OnKill       personal monsters (true = no loot) and hunting drops
//  A 15 s timer (started by the first hook call) keeps the dynamic NPCs in step with the festival schedule and removes
//  expired personal monsters.
// ============================================================================
using System;
using System.Threading;

namespace Qahr.Mobile.NewYear2023
{
    public static class SpringHooks
    {
        static Timer _timer;
        static int _started;
        /// <summary>Tests switch the timer off.</summary>
        public static bool NoTimer;

        static void EnsureTimer()
        {
            if (NoTimer || _started != 0 || Interlocked.Exchange(ref _started, 1) != 0) return;
            try { _timer = new Timer(_ => Tick(), null, 2000, 15000); } catch { }
        }
        static int _ticking;
        public static void Tick()
        {
            if (Interlocked.Exchange(ref _ticking, 1) != 0) return;
            try
            {
                try { SpringFestival2023.SyncWorld(); } catch (Exception e) { Log(e); }
                try { SpringWorld.Sweep(); } catch (Exception e) { Log(e); }
            }
            finally { Interlocked.Exchange(ref _ticking, 0); }
        }
        static void Log(Exception e) { MyConsole.SaveException(e); }

        /// <summary>LuaPush.HandleCallback. True = answered.</summary>
        public static bool OnLua(Client.GameClient c, uint msgId, string func, string[] args)
        {
            try
            {
                EnsureTimer();
                if (c == null || c.Player == null || string.IsNullOrEmpty(func)) return false;
                if (msgId == SpringFestival2023.LuaMsg) return SpringFestival2023.OnLua(c, func, args);
                if (msgId == SpringFestival2023Wish.MsgSummer23 || msgId == SpringFestival2023Wish.MsgValentines)
                    return SpringFestival2023Wish.OnLua(c, msgId, func, args);
                return false;
            }
            catch (Exception e) { Log(e); return false; }
        }

        /// <summary>Procesor, NPC in view, before FestivalManager.OnNpc. True = answered.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            try
            {
                EnsureTimer();
                if (c == null || c.Player == null) return false;
                if (SpringFestival2023.OnNpc(c, stream, npcid, option)) return true;
                return false;
            }
            catch (Exception e) { Log(e); return false; }
        }

        /// <summary>Procesor: answer of an item dialog (npcid = item id). True = answered.</summary>
        public static bool OnItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            try
            {
                if (c == null || c.Player == null) return false;
                if (SpringFestival2023Wish.OnItemDialog(c, stream, npcid, option, input)) return true;
                return false;
            }
            catch (Exception e) { Log(e); return true; }
        }

        /// <summary>MsgItemUsuagePacket.UseItem. True = handled.</summary>
        public static bool OnUse(Client.GameClient c, Game.MsgServer.MsgGameItem item, ServerSockets.Packet stream)
        {
            try
            {
                EnsureTimer();
                if (c == null || c.Player == null || item == null) return false;
                if (SpringFestival2023Wish.OnUse(c, item, stream)) return true;
                return false;
            }
            catch (Exception e) { Log(e); return false; }
        }

        /// <summary>MonsterRole.Dead after the quest credit. True = one of our personal monsters: no loot.</summary>
        public static bool OnKill(Game.MsgMonster.MonsterRole mob, Client.GameClient killer, ServerSockets.Packet stream)
        {
            try
            {
                EnsureTimer();
                if (mob == null) return false;
                var rec = SpringWorld.Consume(mob);
                if (rec != null)
                {
                    if (rec.Tag == "ny23pig") SpringFestival2023.OnPigKilled(rec);
                    return true;
                }
                if (killer == null || killer.Player == null) return false;
                SpringFestival2023.OnHunt(killer, mob);
                return false;
            }
            catch (Exception e) { Log(e); return false; }
        }
    }
}
