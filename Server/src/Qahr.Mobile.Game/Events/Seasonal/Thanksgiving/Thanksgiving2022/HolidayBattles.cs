// ============================================================================
//  [Thanksgiving2022 2026-10-15] HolidayBattles - private battle copies for the holiday festival group (see HolidayKit.cs).
//
//  A battle = one player, a private DynamicID copy of a REAL map (Pool.ServerMaps, loaded lazily), waves of monsters
//  (festival monster types registered by the festival while it runs, or normal types), a time limit. Every second the
//  host checks the copy: all monsters of the wave dead -> next wave / success; time over, player dead too long, player
//  left the copy or logged out -> failure. On the end the remaining monsters are removed, Done(run, success) runs (the
//  caller grants / records), then the player is sent back to where he entered from.
//  The monsters are RemoveOnDead (never respawn). Kills are polled, so no shared kill hook is needed.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Thanksgiving2022
{
    public sealed class BattleWave
    {
        public uint Monster; public int Count = 1; public string Text;
        public BattleWave(uint monster, int count, string text = null) { Monster = monster; Count = count; Text = text; }
    }
    public sealed class BattleSpec
    {
        public string Festival, Name;
        public uint Map; public ushort X, Y; public int Spread = 10;
        public int Seconds = 300;
        public string StartText, FailText;
        public List<BattleWave> Waves = new List<BattleWave>();
    }
    public sealed class BattleRun
    {
        public uint Owner, Dyn; public BattleSpec Spec; public object Tag;
        public int Wave = -1, Kills;
        public DateTime Deadline, Started;
        public uint BackMap; public ushort BackX, BackY;
        public readonly List<object> Mobs = new List<object>();
        public Action<BattleRun, bool> Done;
        public bool Over;
        public DateTime DeadSince = DateTime.MaxValue;
    }

    public static class HolidayBattles
    {
        /// <summary>Everything that touches the world (harness seam).</summary>
        public interface IWorld
        {
            Client.GameClient Online(uint uid);
            bool Prepare(uint map, ref ushort x, ref ushort y, out uint dyn);
            void Move(Client.GameClient c, uint map, ushort x, ushort y, uint dyn);
            bool Inside(Client.GameClient c, uint map, uint dyn);
            bool Alive(Client.GameClient c);
            object Spawn(uint map, uint dyn, uint monster, ushort x, ushort y, int spread);
            bool MobAlive(object mob);
            void Remove(object mob);
        }
        public static IWorld World = new RealWorld();
        static readonly ConcurrentDictionary<uint, BattleRun> _runs = new ConcurrentDictionary<uint, BattleRun>();
        static Timer _timer;
        static readonly object _tickLock = new object();
        public static readonly HashSet<uint> CityMaps = new HashSet<uint> { 1002, 1000, 1011, 1015, 1020, 1036, 3954 };

        public static bool InBattle(uint uid) { return _runs.ContainsKey(uid); }
        /// <summary>Harness seam: festival-running check (default FestivalManager.IsRunning).</summary>
        public static Func<string, bool> RunningOverride;
        static bool FestivalRunning(string key) { var o = RunningOverride; if (o != null) return o(key); try { return Festivals.FestivalManager.IsRunning(key); } catch { return false; } }
        public static BattleRun Of(uint uid) { BattleRun r; return _runs.TryGetValue(uid, out r) ? r : null; }

        /// <summary>Starts a battle. Returns null on success, else the reason (nothing was taken).</summary>
        public static string CanStart(Client.GameClient c)
        {
            if (c == null || c.Player == null) return "Please try again.";
            if (_runs.ContainsKey(c.Player.UID)) return "You are already in a battle.";
            if (!World.Alive(c)) return "You can`t do this while you are dead.";
            if (c.Player.DynamicID != 0) return "Please leave this place first.";
            return null;
        }
        public static string Start(Client.GameClient c, BattleSpec spec, object tag, Action<BattleRun, bool> done)
        {
            string why = CanStart(c);
            if (why != null) return why;
            ushort x = spec.X, y = spec.Y; uint dyn;
            if (!World.Prepare(spec.Map, ref x, ref y, out dyn)) return "This place is not open yet.";
            var run = new BattleRun
            {
                Owner = c.Player.UID, Dyn = dyn, Spec = spec, Tag = tag, Done = done,
                Started = DateTime.Now, Deadline = DateTime.Now.AddSeconds(Math.Max(30, spec.Seconds)),
                BackMap = c.Player.Map, BackX = c.Player.X, BackY = c.Player.Y,
            };
            if (!_runs.TryAdd(c.Player.UID, run)) return "You are already in a battle.";
            World.Move(c, spec.Map, x, y, dyn);
            if (!string.IsNullOrEmpty(spec.StartText)) HKit.Center(c, spec.StartText);
            NextWave(run, c);
            EnsureTimer();
            return null;
        }

        static void EnsureTimer()
        {
            if (_timer != null) return;
            lock (_tickLock) { if (_timer == null) _timer = new Timer(_ => Tick(), null, 1000, 1000); }
        }

        static void NextWave(BattleRun run, Client.GameClient c)
        {
            run.Wave++;
            if (run.Wave >= run.Spec.Waves.Count) return;
            var w = run.Spec.Waves[run.Wave];
            lock (run.Mobs)
                for (int i = 0; i < w.Count; i++)
                {
                    var m = World.Spawn(run.Spec.Map, run.Dyn, w.Monster, run.Spec.X, run.Spec.Y, run.Spec.Spread);
                    if (m != null) run.Mobs.Add(m);
                }
            if (c != null && !string.IsNullOrEmpty(w.Text)) HKit.Center(c, w.Text);
        }

        /// <summary>One pass over every battle (the timer; the harness calls it directly).</summary>
        public static void Tick()
        {
            if (!Monitor.TryEnter(_tickLock)) return;
            try
            {
                foreach (var run in _runs.Values.ToArray())
                {
                    try { Step(run); }
                    catch (Exception e) { MyConsole.SaveException(e); Finish(run, false); }
                }
            }
            finally { Monitor.Exit(_tickLock); }
        }
        static void Step(BattleRun run)
        {
            if (run.Over) return;
            var c = World.Online(run.Owner);
            if (c == null || c.Player == null || !World.Inside(c, run.Spec.Map, run.Dyn)) { Finish(run, false); return; }
            if (!string.IsNullOrEmpty(run.Spec.Festival) && !FestivalRunning(run.Spec.Festival)) { Finish(run, false); return; }// the festival ended
            if (!World.Alive(c))
            {
                if (run.DeadSince == DateTime.MaxValue) run.DeadSince = DateTime.Now;
                else if (DateTime.Now - run.DeadSince > TimeSpan.FromSeconds(20)) { Finish(run, false); return; }
            }
            else run.DeadSince = DateTime.MaxValue;
            int alive;
            lock (run.Mobs)
            {
                int before = run.Mobs.Count;
                run.Mobs.RemoveAll(m => !World.MobAlive(m));
                run.Kills += before - run.Mobs.Count;
                alive = run.Mobs.Count;
            }
            if (alive == 0)
            {
                if (run.Wave + 1 < run.Spec.Waves.Count) { NextWave(run, c); return; }
                Finish(run, true);
                return;
            }
            if (DateTime.Now >= run.Deadline) Finish(run, false);
        }

        /// <summary>Ends a battle (idempotent): monsters removed, Done(run, ok), player sent back.</summary>
        public static void Finish(BattleRun run, bool ok)
        {
            lock (run)
            {
                if (run.Over) return;
                run.Over = true;
            }
            BattleRun cur;
            if (_runs.TryGetValue(run.Owner, out cur) && cur == run) _runs.TryRemove(run.Owner, out cur);
            lock (run.Mobs) { foreach (var m in run.Mobs) { try { World.Remove(m); } catch { } } run.Mobs.Clear(); }
            var c = World.Online(run.Owner);
            if (!ok && c != null && !string.IsNullOrEmpty(run.Spec.FailText)) HKit.Center(c, run.Spec.FailText);
            try { if (run.Done != null) run.Done(run, ok); } catch (Exception e) { MyConsole.SaveException(e); }
            if (c != null && c.Player != null && World.Inside(c, run.Spec.Map, run.Dyn))
            {
                uint map = run.BackMap; ushort x = run.BackX, y = run.BackY;
                if (!CityMaps.Contains(map)) { map = 1002; x = 300; y = 278; }
                try { World.Move(c, map, x, y, 0); } catch { }
            }
        }

        /// <summary>Owner command / festival end: stop every battle of a festival (no reward).</summary>
        public static int StopAll(string festival)
        {
            int n = 0;
            foreach (var r in _runs.Values.ToArray()) if (festival == null || r.Spec.Festival == festival) { Finish(r, false); n++; }
            return n;
        }

        // ------------------------------------------------------------------ the real world
        sealed class RealWorld : IWorld
        {
            public Client.GameClient Online(uint uid) { Client.GameClient c; return Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out c) ? c : null; }
            public bool Alive(Client.GameClient c) { return c.Player.Alive; }
            public bool Inside(Client.GameClient c, uint map, uint dyn) { return c.Player.Map == map && c.Player.DynamicID == dyn; }
            public bool Prepare(uint map, ref ushort x, ref ushort y, out uint dyn)
            {
                dyn = 0;
                Role.GameMap gm = null;
                try { gm = Pool.ServerMaps[map]; } catch { }
                if (gm == null) return false;
                if (!Nearest(gm, ref x, ref y)) return false;
                dyn = gm.GenerateDynamicID();
                return true;
            }
            static bool Nearest(Role.GameMap gm, ref ushort x, ref ushort y)
            {
                if (gm.ValidLocation(x, y)) return true;
                for (int r = 1; r <= 12; r++)
                    for (int dx = -r; dx <= r; dx++)
                        for (int dy = -r; dy <= r; dy++)
                        {
                            if (Math.Max(Math.Abs(dx), Math.Abs(dy)) != r) continue;
                            int nx = x + dx, ny = y + dy;
                            if (nx < 0 || ny < 0) continue;
                            if (gm.ValidLocation((ushort)nx, (ushort)ny)) { x = (ushort)nx; y = (ushort)ny; return true; }
                        }
                return false;
            }
            public void Move(Client.GameClient c, uint map, ushort x, ushort y, uint dyn)
            {
                var o = QuestEngine.QuestEngineCore.TeleportOverride;
                if (o != null) o(c, x, y, map, dyn); else c.Teleport(x, y, map, dyn);
            }
            public object Spawn(uint mapId, uint dyn, uint monster, ushort x, ushort y, int spread)
            {
                Game.MsgMonster.MonsterFamily fam;
                if (Pool.MonsterFamilies == null || !Pool.MonsterFamilies.TryGetValue(monster, out fam) || fam == null) return null;
                var map = Pool.ServerMaps[mapId];
                if (map == null) return null;
                if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
                if (!map.MonstersColletion.ReadMap()) return null;
                var copy = fam.Copy();
                int s = Math.Max(1, spread);
                copy.SpawnX = (ushort)Math.Max(1, x - s); copy.SpawnY = (ushort)Math.Max(1, y - s);
                copy.MaxSpawnX = (ushort)(x + s); copy.MaxSpawnY = (ushort)(y + s);
                copy.MapID = map.ID; copy.SpawnCount = 1;
                Game.MsgMonster.MonsterRole mob = null;
                for (int tries = 0; tries < 6 && mob == null; tries++) mob = map.MonstersColletion.Add(copy, true, dyn, true);
                if (mob == null) return null;
                mob.RemoveOnDead = true;
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        mob.Send(stream.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                        mob.Send(mob.GetArray(stream, false));
                    }
                }
                catch { }
                return mob;
            }
            public bool MobAlive(object o) { var m = o as Game.MsgMonster.MonsterRole; return m != null && m.Alive; }
            public void Remove(object o)
            {
                var mob = o as Game.MsgMonster.MonsterRole;
                if (mob == null || !mob.Alive) return;
                mob.RemoveOnDead = true;
                mob.HitPoints = 0;
                using (var rec = new ServerSockets.RecycledPacket())
                    mob.Send(rec.GetStream().ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.RemoveEntity }));
                var gm = mob.GMap;
                if (gm != null) { gm.View.LeaveMap<Role.IMapObj>(mob); gm.SetMonsterOnTile(mob.X, mob.Y, false); }
            }
        }
    }
}
