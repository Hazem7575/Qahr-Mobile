// [Poker 2026-10-14] New Texas Sit & Go.
// Official data (read-only): Data\newtexas_sng_race.txt (buy-in, blind level seconds, return rate, players),
// newtexas_sng_stage.txt (row 0 = starting chips, rows 1.. = big blind / small blind), newtexas_sng_prize.txt
// (rank -> per-10000 share of the pool); client lua newtexas_sng (max_player 5, hall "type" 5..8 per race).
// Protocol (spec_tourney.txt): sign-up / leave arrive as 9119 CMsgDominoAct with f5 = 5..8; replies 9119 f1=1/4,
// 2534 (sign-up result / state / result / leave), 2535 (blinds + eliminated players); the table itself is a normal
// New Texas board (2471 f9 = 0x10) with tournament chips.
// Money: the buy-in is escrowed like a cash seat (journal + marker saved with Money); a crash refunds it; the prize is
// credited and the marker cleared in one save when the player finishes.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Poker
{
    public static class PokerSng
    {
        public const ushort IdDominoAct = 9119, IdSngAct = 2534, IdSngTableInfo = 2535;

        public sealed class Race
        {
            public uint Id; public string Name = ""; public ulong Cost; public int StageSeconds; public uint ReturnRate;
            public int MinPlayers, MaxPlayers; public uint Variant; public uint HallType; public bool Enabled;
            public Dictionary<uint, uint> PrizeByRank = new Dictionary<uint, uint>();   // rank -> per 10000
        }

        internal sealed class Entry
        {
            public uint Uid; public Client.GameClient Client; public Race Race; public ulong Session;
            public Game Game; public bool Out; public uint Rank;
        }

        internal sealed class Game : PokerModule.ITourney
        {
            public uint Id; public Race Race; public PokerModule.TableCtx Ctx;
            public readonly List<Entry> Players = new List<Entry>();
            public int Level = 1; public long NextLevelAt; public long StartAt; public bool Started; public bool Finished;
            public ulong Pool;

            public Client.GameClient ClientOf(uint uid)
            {
                var e = Players.FirstOrDefault(x => x.Uid == uid && !x.Out);
                return e != null ? e.Client : null;
            }

            public void AfterResult(PokerModule.TableCtx ctx, PokerEvent ev, long now) { OnHandEnd(this, now); }
            public void Housekeeping(PokerModule.TableCtx ctx, long now) { Tick(this, now); }
        }

        static readonly ConcurrentDictionary<uint, Entry> entries = new ConcurrentDictionary<uint, Entry>();
        static readonly Dictionary<uint, List<Entry>> queues = new Dictionary<uint, List<Entry>>();   // race -> waiting
        static readonly object gate = new object();
        static Dictionary<uint, Race> races;
        static List<ulong[]> stages;     // [bb, sb]
        static ulong startChips = 50000;
        static DateTime loadedStamp;

        public static bool IsIn(uint uid) { return entries.ContainsKey(uid); }
        internal static PokerModule.TableCtx TableOf(uint uid)
        {
            Entry e;
            return entries.TryGetValue(uid, out e) && e.Game != null && !e.Out ? e.Game.Ctx : null;
        }

        // ------------------------------------------------------------------ data
        static string DbRoot
        {
            get
            {
                string f = PokerConfig.Folder;
                return Path.GetDirectoryName(f.TrimEnd('\\', '/'));
            }
        }

        static void EnsureData()
        {
            var cfgStamp = File.Exists(Path.Combine(PokerConfig.Folder, "config.json")) ? File.GetLastWriteTimeUtc(Path.Combine(PokerConfig.Folder, "config.json")) : DateTime.MinValue;
            if (races != null && cfgStamp == loadedStamp) return;
            lock (gate)
            {
                var r = new Dictionary<uint, Race>();
                var sng = PokerConfig.Obj(PokerConfig.Current.Raw, "sng");
                var enabledIds = new HashSet<uint>();
                var arr = PokerConfig.Arr(sng, "enabledRaces");
                if (arr != null) foreach (var o in arr) enabledIds.Add((uint)PokerConfig.Num(o));
                int maxPlayers = (int)PokerConfig.D(sng, "maxPlayers", 5);           // client lua max_player
                int minPlayers = (int)PokerConfig.D(sng, "minPlayers", 3);
                foreach (var line in ReadTable("newtexas_sng_race.txt"))
                {
                    if (line.Length < 11) continue;
                    var x = new Race
                    {
                        Id = UI(line[0]), Name = line[1], Cost = UL(line[2]), StageSeconds = (int)UI(line[3]), ReturnRate = UI(line[5]),
                        MinPlayers = minPlayers, MaxPlayers = maxPlayers, Variant = UI(line[8])
                    };
                    if (x.Id == 0 || x.Cost == 0) continue;
                    x.HallType = x.Id <= 36 ? 5u : x.Id <= 42 ? 6u : x.Id <= 48 ? 7u : 8u;   // client lua newtexas_sng "type"
                    x.Enabled = enabledIds.Contains(x.Id);
                    if (x.StageSeconds < 30) x.StageSeconds = 120;
                    if (x.ReturnRate == 0 || x.ReturnRate > 10000) x.ReturnRate = 9500;
                    r[x.Id] = x;
                }
                foreach (var line in ReadTable("newtexas_sng_prize.txt"))
                {
                    if (line.Length < 4) continue;
                    Race x;
                    if (r.TryGetValue(UI(line[1]), out x)) x.PrizeByRank[UI(line[2])] = UI(line[3]);
                }
                var st = new List<ulong[]>();
                foreach (var line in ReadTable("newtexas_sng_stage.txt"))
                {
                    if (line.Length < 3) continue;
                    uint lv = UI(line[0]);
                    if (lv == 0) { startChips = UL(line[1]); continue; }
                    st.Add(new[] { UL(line[1]), UL(line[2]) });
                }
                if (st.Count == 0) st.Add(new ulong[] { 200, 100 });
                stages = st;
                races = r;
                loadedStamp = cfgStamp;
            }
        }

        static IEnumerable<string[]> ReadTable(string name)
        {
            string p = Path.Combine(DbRoot, name);
            if (!File.Exists(p)) yield break;
            foreach (var l in File.ReadAllLines(p, Encoding.UTF8))
                if (l.Trim().Length > 0) yield return l.Split(new[] { "@@" }, StringSplitOptions.None);
        }
        static uint UI(string s) { uint v; return uint.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }
        static ulong UL(string s) { ulong v; return ulong.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        // ------------------------------------------------------------------ packets
        static void Domino(Client.GameClient c, uint action, uint[] f2, int f5)
        {
            var w = new PbW().U(1, action).Rep(2, f2).I32(5, f5);
            PokerModule.Send(c, w, IdDominoAct);
        }
        static void SngAct(Client.GameClient c, uint action, uint[] f2, ulong[] f3 = null)
        {
            var w = new PbW().U(1, action).Rep(2, f2);
            if (f3 != null) w.Rep(3, f3);
            PokerModule.Send(c, w, IdSngAct);
        }

        [PacketAttribute(IdDominoAct)]
        static void OnDominoAct(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!PokerModule.Ready(client)) return;
                var r = PbR.FromPacket(stream);
                int f5 = (int)r.U(5);
                if (f5 < 5 || f5 > 8) return;                         // Domino game types 0..4: not this system
                uint action = r.U32(1);
                var f2 = r.List(2);
                if (action != 2 && !PokerModule.Throttle(client, PokerConfig.Current.RequestCooldownMs)) return;
                PokerModule.EnsureTicker();
                PokerLedger.ApplyPendingRefund(client);
                switch (action)
                {
                    case 1: SignUp(client, f2.Count > 0 ? (uint)f2[0] : 0, f5); break;
                    case 2: EnterBoard(client, f2.Count > 0 ? (uint)f2[0] : 0); break;
                    case 4: Leave(client, "leave"); break;
                    case 5: Leave(client, "re-sign"); break;          // "play again": leave the old table; the hall signs up again
                }
            }
            catch (Exception e) { PokerLog.Write(client, "9119 error: " + e.Message); }
        }

        static void SignUp(Client.GameClient client, uint raceId, int hallType)
        {
            EnsureData();
            uint uid = client.Player.UID;
            Race race;
            if (!PokerConfig.Current.Enabled || !races.TryGetValue(raceId, out race) || !race.Enabled || race.HallType != (uint)hallType)
            { SngAct(client, 1, new[] { raceId, 1u }); PokerLog.Write(client, "SNG sign-up refused: race " + raceId); return; }
            if (PokerModule.IsSeated(uid)) { SngAct(client, 1, new[] { raceId, 2u }); return; }
            if (PokerMoney.Balance(client) < race.Cost) { Domino(client, 7, new[] { 0u, 1u }, hallType); return; }
            lock (gate)
            {
                if (entries.ContainsKey(uid)) return;
                ulong sid = PokerRng.NextId();
                PokerLedger.SetChips(sid, uid, raceId, race.Cost, true);
                if (!PokerMoney.Debit(client, race.Cost, "SNG buy-in race " + raceId)) { PokerLedger.SetChips(sid, uid, raceId, 0, false); Domino(client, 7, new[] { 0u, 1u }, hallType); return; }
                PokerLedger.SetMarker(uid, sid);
                PokerMoney.Save(client);
                var e = new Entry { Uid = uid, Client = client, Race = race, Session = sid };
                entries[uid] = e;
                List<Entry> q;
                if (!queues.TryGetValue(raceId, out q)) queues[raceId] = q = new List<Entry>();
                q.Add(e);
                PokerLog.Write(client, "SNG SIGN-UP race " + raceId + " cost " + race.Cost + " session " + sid + " (" + q.Count + "/" + race.MaxPlayers + ")");
                SngAct(client, 1, new[] { raceId, 0u });
                foreach (var w in q) SngAct(w.Client, 2, new[] { raceId, 0u, (uint)q.Count });
                if (q.Count >= race.MaxPlayers) StartGame(race, q.Take(race.MaxPlayers).ToList());
            }
        }

        /// <summary>Caller holds gate.</summary>
        static void StartGame(Race race, List<Entry> list)
        {
            foreach (var e in list) queues[race.Id].Remove(e);
            ulong pot = race.Cost * (ulong)list.Count;
            var cfg = PokerConfig.Current;
            var def = new PokerTableDef
            {
                Id = (uint)System.Threading.Interlocked.Increment(ref PokerModule.nextTableId),
                LevelId = race.Id, GameType = 5, Variant = race.Variant == 2 ? PokerVariant.Omaha : PokerVariant.Texas,
                Seats = race.MaxPlayers, SmallBlind = stages[0][1], BigBlind = stages[0][0], MinBuyIn = 0, MaxBuyIn = ulong.MaxValue,
                ActionSeconds = cfg.ActionSeconds, NextHandSeconds = cfg.NextHandSeconds, RakePercent = 0, Tournament = true
            };
            var g = new Game { Id = def.Id, Race = race, Pool = pot * race.ReturnRate / 10000 };
            var ctx = new PokerModule.TableCtx { T = new PokerTable(def), Level = new PokerLevel { Id = race.Id, Type = 0, MinBuyIn = 0, MaxBuyIn = ulong.MaxValue, BetUnit = def.BigBlind }, RaceType = race.Variant == 2 ? 2u : 0u, Tour = g, SurfaceF9 = 0x10 };
            g.Ctx = ctx;
            long now = PokerModule.Clock();
            lock (ctx.T.Gate)
            {
                int seat = 0;
                foreach (var e in list)
                {
                    e.Game = g; g.Players.Add(e);
                    ctx.T.Sit(seat++, e.Uid, e.Client.Player.Name, startChips, now);
                }
                ctx.T.Drain();
                g.StartAt = now + 10000;               // give every client time to open the board (9119 f1=2)
                ctx.T.HoldUntil(g.StartAt);
                g.NextLevelAt = g.StartAt + 1000L * race.StageSeconds;
            }
            PokerModule.tables[def.Id] = ctx;
            PokerLog.Write("SNG START table " + def.Id + " race " + race.Id + " players " + string.Join(",", list.Select(x => x.Uid)) + " pool " + g.Pool + " (fee " + (pot - g.Pool) + ")");
            foreach (var e in list) Domino(e.Client, 1, new[] { def.Id, 1u }, (int)race.HallType);
        }

        static void EnterBoard(Client.GameClient client, uint tableId)
        {
            Entry e;
            if (!entries.TryGetValue(client.Player.UID, out e) || e.Game == null || e.Out || e.Game.Id != tableId) return;
            var ctx = e.Game.Ctx;
            lock (ctx.T.Gate)
            {
                var me = ctx.T.SeatOf(e.Uid);
                if (me == null) return;
                PokerModule.Send(client, PokerModule.EnterFor(ctx, me, true, PokerModule.StatusOf(ctx, me)), PokerModule.IdEnter);
                foreach (var o in ctx.T.Seats)
                    if (!o.Empty && o.Uid != e.Uid) PokerModule.Send(client, PokerModule.EnterFor(ctx, o, false, PokerModule.StatusOf(ctx, o)), PokerModule.IdEnter);
                PokerModule.Send(client, PokerModule.Chips(ctx.T.Seats), PokerModule.IdTableChip);
                SendTableInfo(e.Game, client);
                SngAct(client, 2, new[] { e.Race.Id, 1u, (uint)e.Game.Players.Count(x => !x.Out) });
            }
        }

        static PbW TableInfo(Game g)
        {
            var t = g.Ctx.T;
            var w = new PbW().U(1, 0).U(2, t.Def.BigBlind).U(3, 0);
            foreach (var p in g.Players) w.Msg(4, new PbW().U(1, PokerModule.ServerId).U(2, p.Uid).U(3, p.Out ? 1u : 0u));
            return w;
        }
        static void SendTableInfo(Game g, Client.GameClient only = null)
        {
            var w = TableInfo(g);
            if (only != null) { PokerModule.Send(only, w, IdSngTableInfo); return; }
            PokerModule.Broadcast(g.Ctx, w, IdSngTableInfo);
        }

        // ------------------------------------------------------------------ game flow (callers hold the table gate)
        static void Tick(Game g, long now)
        {
            var t = g.Ctx.T;
            if (g.Finished) return;
            if (!g.Started)
            {
                if (now < g.StartAt) { t.HoldUntil(g.StartAt); return; }
                g.Started = true;
                SendTableInfo(g);
            }
            if (now >= g.NextLevelAt && g.Level < stages.Count)
            {
                g.Level++;
                g.NextLevelAt = now + 1000L * g.Race.StageSeconds;
                // the new blinds apply from the next hand
                t.Def.BigBlind = stages[g.Level - 1][0];
                t.Def.SmallBlind = stages[g.Level - 1][1];
                SendTableInfo(g);
                PokerLog.Write("SNG table " + g.Id + " level " + g.Level + " blinds " + t.Def.SmallBlind + "/" + t.Def.BigBlind);
            }
            // a player who went offline is out (DERIVED: no reconnect into a running SNG)
            foreach (var p in g.Players.Where(x => !x.Out).ToList())
                if (p.Client == null || (!PokerModule.TestMode && (p.Client.Socket == null || !p.Client.Socket.Alive)))
                    Eliminate(g, p, now, "offline");
        }

        static void OnHandEnd(Game g, long now)
        {
            var t = g.Ctx.T;
            // busted this hand: ranks by chips at the start of the hand are equal here, so the order is by seat (DERIVED tie rule)
            var busted = g.Players.Where(p => !p.Out && t.SeatOf(p.Uid) != null && t.SeatOf(p.Uid).Stack == 0).ToList();
            foreach (var p in busted) Eliminate(g, p, now, "busted");
        }

        static void Eliminate(Game g, Entry p, long now, string why)
        {
            var t = g.Ctx.T;
            int left = g.Players.Count(x => !x.Out);
            p.Rank = (uint)left;
            p.Out = true;
            t.Leave(p.Uid, now);                                       // tournament chips vanish; the silver pool is fixed
            PokerModule.Broadcast(g.Ctx, new PbW().U(1, 1).U(2, t.Def.Id).U(3, p.Uid).U(4, PokerModule.ServerId), PokerModule.IdExit, p.Uid);
            Finish(g, p, why);
            SendTableInfo(g);
            var alive = g.Players.Where(x => !x.Out).ToList();
            if (alive.Count == 1)
            {
                var w = alive[0];
                w.Rank = 1; w.Out = true;
                t.Leave(w.Uid, now);
                Finish(g, w, "winner");
                g.Finished = true;
                PokerModule.tables.TryRemove(g.Id, out _);
                PokerLog.Write("SNG END table " + g.Id + " winner " + w.Uid);
            }
            PokerModule.Flush(g.Ctx, now);
        }

        static void Finish(Game g, Entry p, string why)
        {
            uint share; g.Race.PrizeByRank.TryGetValue(p.Rank, out share);
            ulong prize = g.Pool * share / 10000;
            entries.TryRemove(p.Uid, out _);
            PokerLedger.Payout(p.Client, p.Uid, p.Session, g.Race.Id, prize, "SNG prize rank " + p.Rank + " table " + g.Id);
            if (PokerLedger.Online(p.Client)) SngAct(p.Client, 3, new[] { g.Race.Id, p.Rank, 0u }, new[] { prize });
            PokerLog.Write(p.Client, "SNG FINISH (" + why + ") table " + g.Id + " rank " + p.Rank + " prize " + prize);
        }

        static void Leave(Client.GameClient client, string why)
        {
            uint uid = client.Player.UID;
            Entry e;
            if (!entries.TryGetValue(uid, out e)) { Domino(client, 4, new uint[0], 5); return; }
            if (e.Game == null)
            {
                // still waiting: full refund of the buy-in
                lock (gate)
                {
                    if (!entries.TryRemove(uid, out e)) return;
                    List<Entry> q;
                    if (queues.TryGetValue(e.Race.Id, out q)) q.Remove(e);
                    PokerLedger.SetChips(e.Session, uid, e.Race.Id, e.Race.Cost, false);
                    PokerMoney.Credit(client, e.Race.Cost, "SNG sign-up cancelled race " + e.Race.Id);
                    PokerLedger.SetMarker(uid, 0);
                    PokerMoney.Save(client);
                    PokerLog.Write(client, "SNG CANCEL race " + e.Race.Id + " refund " + e.Race.Cost);
                    if (q != null) foreach (var w in q) SngAct(w.Client, 2, new[] { e.Race.Id, 0u, (uint)q.Count });
                }
                Domino(client, 4, new uint[0], (int)e.Race.HallType);
                SngAct(client, 4, new uint[0]);
                return;
            }
            var g = e.Game;
            lock (g.Ctx.T.Gate)
            {
                if (!e.Out) Eliminate(g, e, PokerModule.Clock(), why);   // forfeits; the chips are not refunded (STR_SNG_CONFIRM_LEAVE)
            }
            Domino(client, 4, new uint[0], (int)e.Race.HallType);
        }

        /// <summary>Disconnect: a waiting sign-up is refunded; a running game is forfeited.</summary>
        public static void OnDisconnect(Client.GameClient client)
        {
            Entry e;
            if (client == null || client.Player == null || !entries.TryGetValue(client.Player.UID, out e)) return;
            try { Leave(client, "disconnect"); } catch (Exception ex) { PokerLog.Write(client, "SNG disconnect error: " + ex.Message); }
        }
    }
}
