// [TeamArena 2026-10-14] Team Arena (Team Qualifier) - registration, matchmaking, matches, results, rewards.
//
// Flow (client side read from Conquer.exe, see TeamArenaPackets.cs):
//   leader: Team Arena dialog -> "Join" -> STR_TEAM_ARENA_JOIN_CONFIRM -> 2218 action 0
//   server: 2218 action 6 status 1 (Waiting) + 2296 to every member
//   matchmaking every N s: teams with the closest Team Arena Points (= average of the members' points)
//   server: 2218 action 2 (accept dialog) + 2168 type 1 (opponent members) to both teams
//   leader: accept (2218 action 3 param 1) / give up (action 3 param 2); no answer in AcceptSeconds = gives up
//   both accepted: members teleported to the arena (map 700, own dynamic id), 2218 action 8, status 5, 2239 score
//   fight ends when one team has nobody alive in the arena, or on time (more damage wins)
//   server: 2218 action 9 (victory / failure effect), points, W/L, TeamArenaPack + Champion Points,
//           after EndDelay: back to where they were, status 0, 2218 action 10 (continue dialog -> action 11)
//   daily: honor by rank from TeamArenaHonorRewards.ini (client file), yesterday top 10, points reset.
//
// Security: the uid is always client.Player.UID; no amount/id from the client is trusted (2218 param is only
// compared to 1/2); every request is rate limited; results/rewards are applied once per match under one lock;
// silver is taken before points are given; the settlement date is saved before honor is credited.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Qahr.Mobile.TeamQualifier
{
    public static class TeamArenaCore
    {
        // ------------------------------------------------------------------ texts (client Cn_Res.ini, OFFICIAL)
        public const string TxtTime = "Team Qualifier is open from 08:00-12:00 and 13:00-23:00 everyday.";
        public const string TxtNotLeader = "Only team leaders can sign up for the Team Qualifier.";
        public const string TxtLevel = "Only level 40+ players can sign up for the Team Qualifier.";
        public const string TxtRepeat = "You`ve signed up for the Team Qualifier!";
        public const string TxtMark = "Your Team Arena Points are too low to sign up for the Team Qualifier.";
        public const string TxtBooth = "Quit running a booth before you sign up for the Team Qualifier.";
        public const string TxtBuyFail = "You don`t have enough Silver!";
        // server-only texts (DERIVED wording)
        public const string TxtTeamSize = "A team of {0}-{1} players is needed for the Team Qualifier.";
        public const string TxtBusy = "{0} is in another qualifier or event and can't join the Team Qualifier now.";
        public const string TxtPlace = "{0} can't join the Team Qualifier from the current map.";
        public const string TxtOffline = "Your team changed, so it has left the Team Qualifier. Sign up again.";
        public const string TxtDead = "{0} must be alive to join the Team Qualifier.";

        // ------------------------------------------------------------------ state
        public enum MatchState { Pending, Fighting, Ending, Done }

        public sealed class Fighter
        {
            public uint Uid; public bool Imported, Out, Declined; public int LastHp;
            public uint RetMap, RetDyn; public ushort RetX, RetY;
        }

        public sealed class Side
        {
            public uint Leader; public string LeaderName = "";
            public readonly List<Fighter> Fighters = new List<Fighter>();
            public bool Accepted; public uint Damage; public uint AvgPoints;
            public IEnumerable<Fighter> Active { get { return Fighters.Where(f => !f.Declined); } }
        }

        public sealed class Match
        {
            public uint Id; public MatchState State; public Side A, B; public uint Dyn;
            public DateTime Created, Started, EndAt; public bool Applied; public Side Winner;
            public bool ScoreDirty; public DateTime LastScore;
            public Side SideOf(uint uid) { return A.Fighters.Any(f => f.Uid == uid) ? A : (B.Fighters.Any(f => f.Uid == uid) ? B : null); }
            public Side Other(Side s) { return s == A ? B : A; }
            public Fighter FighterOf(uint uid) { return A.Fighters.Concat(B.Fighters).FirstOrDefault(f => f.Uid == uid); }
        }

        public sealed class Entry { public uint Leader; public List<uint> Members = new List<uint>(); public DateTime Since; public Match Match; }

        static readonly object Sync = TeamArenaStore.Sync;
        static readonly Dictionary<uint, Entry> entries = new Dictionary<uint, Entry>();          // leader uid -> entry
        static readonly Dictionary<uint, Match> matches = new Dictionary<uint, Match>();
        static readonly ConcurrentDictionary<uint, uint> memberOf = new ConcurrentDictionary<uint, uint>(); // uid -> leader uid
        static readonly ConcurrentDictionary<uint, Match> inMatch = new ConcurrentDictionary<uint, Match>();
        static DateTime lastMatchmaking = DateTime.MinValue, lastSave = DateTime.MinValue, lastHonorPay = DateTime.MinValue;

        // ------------------------------------------------------------------ seams (tests replace these; null = real server)
        public static bool DisableTimer;
        public static Func<DateTime> Clock = () => DateTime.Now;
        public static Action<uint, byte[]> Capture;                                   // every packet sent, by uid
        public static Func<uint, Client.GameClient> Resolve;                          // uid -> online client
        public static Action<Client.GameClient, ushort, ushort, uint, uint> TeleportHook; // x, y, map, dynamic id
        public static Func<Client.GameClient, uint, uint, bool> GrantHook;            // item, champion points
        public static Func<uint> NewDynamicId;

        static Timer timer;
        static int started;

        public static void EnsureStarted()
        {
            TeamArenaStore.EnsureLoaded();
            if (DisableTimer) return;
            if (Interlocked.Exchange(ref started, 1) == 0)
                timer = new Timer(_ => { try { Tick(); } catch (Exception e) { TeamArenaStore.Log("TICK " + e); } }, null, 2000, 1000);
        }

        static Client.GameClient Online(uint uid)
        {
            if (Resolve != null) return Resolve(uid);
            Client.GameClient c;
            if (Pool.GamePoll != null && Pool.GamePoll.TryGetValue(uid, out c) && c != null && c.Player != null && c.FullLoading
                && (c.Fake || (c.Socket != null && c.Socket.Alive))) return c;
            return null;
        }

        // ------------------------------------------------------------------ hooks used by core files (lock free)
        /// <summary>Hooked into GameClient.InQualifier(): true while the player is in a Team Arena match.</summary>
        public static bool InMatch(Client.GameClient client)
        {
            if (client == null || client.Player == null) return false;
            Match m;
            return inMatch.TryGetValue(client.Player.UID, out m) && m.State != MatchState.Done;
        }

        /// <summary>Hooked into CanAttackPlayer.Verified(): null = not a Team Arena case; otherwise the answer.</summary>
        public static bool? CanAttack(Client.GameClient attacker, Role.Player attacked)
        {
            if (attacker == null || attacker.Player == null || attacked == null) return null;
            Match ma, mb;
            bool a = inMatch.TryGetValue(attacker.Player.UID, out ma) && InArena(attacker.Player, ma);
            bool b = inMatch.TryGetValue(attacked.UID, out mb) && InArena(attacked, mb);
            if (!a && !b) return null;
            if (!a || !b || ma != mb || ma.State != MatchState.Fighting) return false;
            var sa = ma.SideOf(attacker.Player.UID); var sb = ma.SideOf(attacked.UID);
            if (sa == null || sb == null || sa == sb) return false;
            var fa = ma.FighterOf(attacker.Player.UID);
            return fa != null && fa.Imported && !fa.Out;
        }

        static bool InArena(Role.Player p, Match m)
        {
            return m != null && m.Dyn != 0 && p.Map == (uint)TeamArenaConfig.Current.ArenaMap && p.DynamicID == m.Dyn;
        }

        // ------------------------------------------------------------------ send helpers
        static void Send(Client.GameClient c, Func<ServerSockets.Packet, ServerSockets.Packet> build)
        {
            if (c == null || c.Player == null) return;
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = build(rec.GetStream());
                    if (Capture != null)
                    {
                        var b = new byte[s.Size];
                        unsafe { for (int i = 0; i < b.Length; i++) b[i] = s.Memory[i]; }
                        Capture(c.Player.UID, b);
                    }
                    else c.Send(s);
                }
            }
            catch (Exception e) { TeamArenaStore.Log("SEND " + e.Message); }
        }

        static void Msg(Client.GameClient c, string text)
        {
            if (c == null) return;
            if (Capture != null) { Capture(c.Player.UID, System.Text.Encoding.UTF8.GetBytes("MSG:" + text)); return; }
            try { c.SendSysMesage(text, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch { }
        }

        static void Interact(uint uid, ushort action, uint param)
        {
            var c = Online(uid);
            if (c != null) Send(c, s => TeamArenaPackets.Interactive(s, action, param));
        }

        public static uint StatusOf(uint uid)
        {
            Match m;
            if (inMatch.TryGetValue(uid, out m))
                return m.State == MatchState.Pending ? TeamArenaPackets.StWaiting : (m.State == MatchState.Done ? TeamArenaPackets.StNone : TeamArenaPackets.StFighting);
            return memberOf.ContainsKey(uid) ? TeamArenaPackets.StWaiting : TeamArenaPackets.StNone;
        }

        static void SendHero(Client.GameClient c)
        {
            if (c == null) return;
            var r = TeamArenaStore.Get(c);
            uint rank = TeamArenaStore.TodayRank(c.Player.UID);
            uint hist = 0, cur = 0;
            try { if (c.ArenaStatistic != null && c.ArenaStatistic.Info != null) { hist = c.ArenaStatistic.Info.HistoryHonor; cur = c.ArenaStatistic.Info.CurrentHonor; } } catch { }
            uint st = StatusOf(c.Player.UID);
            Send(c, s => TeamArenaPackets.HeroData(s, rank, st, r, hist, cur));
        }

        static void SendStatus(uint uid)
        {
            var c = Online(uid);
            if (c == null) return;
            Interact(uid, TeamArenaPackets.ActStatus, StatusOf(uid));
            SendHero(c);
        }

        // ------------------------------------------------------------------ throttle
        sealed class Th { public readonly int[] Last = new int[8]; public readonly bool[] Used = new bool[8]; }
        static readonly ConditionalWeakTable<Client.GameClient, Th> th = new ConditionalWeakTable<Client.GameClient, Th>();
        public static bool Allow(Client.GameClient c, int slot, int ms)
        {
            if (c == null || slot < 0 || slot >= 8) return false;
            var t = th.GetOrCreateValue(c);
            int now = Environment.TickCount;
            lock (t)
            {
                if (t.Used[slot] && unchecked(now - t.Last[slot]) < ms) return false;
                t.Used[slot] = true; t.Last[slot] = now; return true;
            }
        }

        static bool Ready(Client.GameClient c) { return c != null && c.Player != null && c.FullLoading; }

        // ------------------------------------------------------------------ packet entry points (B2 delegates here)
        public static void OnRank(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Ready(client)) return;
            EnsureStarted();
            var cfg = TeamArenaConfig.Current;
            if (!Allow(client, 0, cfg.RequestCooldownMs)) return;
            ushort page = 1;
            if (stream.Size >= 6) { stream.Seek(4); page = stream.ReadUInt16(); }
            if (page == 0 || page > 100) page = 1;
            var list = TeamArenaStore.TodayRanking();
            Send(client, s => TeamArenaPackets.Rank(s, page, list, cfg.PageRank));
        }

        public static void OnYTop10(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Ready(client)) return;
            EnsureStarted();
            if (!Allow(client, 1, TeamArenaConfig.Current.RequestCooldownMs)) return;
            var top = TeamArenaStore.YesterdayTop10();
            Send(client, s => TeamArenaPackets.YTop10(s, top));
        }

        public static void OnFightingList(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Ready(client)) return;
            EnsureStarted();
            var cfg = TeamArenaConfig.Current;
            if (!Allow(client, 2, cfg.RequestCooldownMs)) return;
            uint first = 1;
            if (stream.Size >= 8) { stream.Seek(4); first = stream.ReadUInt32(); }
            if (first == 0 || first > 100000) first = 1;
            List<TeamArenaPackets.FightRow> rows; uint teams;
            lock (Sync)
            {
                rows = matches.Values.Where(m => m.State == MatchState.Fighting).OrderBy(m => m.Id)
                    .Select(m => new TeamArenaPackets.FightRow
                    {
                        LeaderA = m.A.Leader, NameA = m.A.LeaderName, CountA = (uint)m.A.Fighters.Count(f => f.Imported),
                        LeaderB = m.B.Leader, NameB = m.B.LeaderName, CountB = (uint)m.B.Fighters.Count(f => f.Imported)
                    }).ToList();
                teams = (uint)entries.Count;
            }
            Send(client, s => TeamArenaPackets.FightingList(s, first, rows, teams, cfg.PageFighting));
        }

        public static void OnHeroData(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Ready(client)) return;
            EnsureStarted();
            if (!Allow(client, 3, TeamArenaConfig.Current.RequestCooldownMs)) return;
            SendHero(client);
        }

        [PacketAttribute(TeamArenaPackets.IdMembers)]
        private static void OnMembersPacket(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!Ready(client)) return;
                EnsureStarted();
                if (!Allow(client, 5, TeamArenaConfig.Current.RequestCooldownMs)) return;
                uint leader = 0;
                if (stream.Size >= 12) { stream.Seek(8); leader = stream.ReadUInt32(); }
                List<TeamArenaPackets.MemberRow> rows = null;
                lock (Sync)
                {
                    var m = matches.Values.FirstOrDefault(x => x.State == MatchState.Fighting && (x.A.Leader == leader || x.B.Leader == leader));
                    if (m != null) rows = Rows(m.A.Leader == leader ? m.A : m.B, true);
                }
                Send(client, s => TeamArenaPackets.Members(s, 0, leader, rows));
            }
            catch (Exception e) { TeamArenaStore.Log("2168 " + e); }
        }

        static List<TeamArenaPackets.MemberRow> Rows(Side side, bool importedOnly)
        {
            var rows = new List<TeamArenaPackets.MemberRow>();
            foreach (var f in side.Fighters.OrderBy(f => f.Uid == side.Leader ? 0 : 1))
            {
                if (f.Declined || (importedOnly && !f.Imported)) continue;
                var r = TeamArenaStore.Find(f.Uid);
                if (r == null) continue;
                rows.Add(new TeamArenaPackets.MemberRow
                {
                    Uid = r.Uid, Level = r.Level, Class = r.Class, BodyType = r.BodyType, Rank = TeamArenaStore.TodayRank(r.Uid), Points = r.Points, Name = r.Name
                });
            }
            return rows;
        }

        public static void OnInteractive(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!Ready(client)) return;
            EnsureStarted();
            if (!Allow(client, 4, TeamArenaConfig.Current.InteractCooldownMs)) return;
            ushort action = 0; uint param = 0;
            if (stream.Size >= 12) { stream.Seek(4); action = stream.ReadUInt16(); stream.ReadUInt16(); param = stream.ReadUInt32(); }
            else if (stream.Size >= 6) { stream.Seek(4); action = stream.ReadUInt16(); }
            Interactive(client, action, param);
        }

        /// <summary>2218 from the client. Only actions the client really sends are handled; anything else is ignored.</summary>
        public static void Interactive(Client.GameClient client, ushort action, uint param)
        {
            switch (action)
            {
                case TeamArenaPackets.ActSignUp:
                case TeamArenaPackets.ActContinue: SignUp(client); break;
                case TeamArenaPackets.ActQuit:
                case TeamArenaPackets.ActRivalGaveUp: Quit(client); break;
                case TeamArenaPackets.ActAnswer:
                    if (param == 1) Accept(client);
                    else if (param == 2) GiveUp(client);
                    break;
                case TeamArenaPackets.ActBuyPoints: BuyPoints(client); break;
            }
        }

        // ------------------------------------------------------------------ sign up / quit
        static List<Client.GameClient> TeamOf(Client.GameClient leader)
        {
            var t = leader.Team;
            if (t == null || t.Members == null) return null;
            return t.Members.Values.Where(mi => mi != null && mi.client != null && mi.client.Player != null).Select(mi => mi.client).ToList();
        }

        static bool IsLeader(Client.GameClient c)
        {
            var t = c.Team;
            return t != null && t.Leader != null && t.Leader.Player != null && t.Leader.Player.UID == c.Player.UID;
        }

        /// <summary>Why this player can't be in a Team Arena team right now (null = fine).</summary>
        static string Blocker(Client.GameClient m, TeamArenaConfig cfg, uint ownLeader)
        {
            var p = m.Player;
            if (Online(p.UID) == null) return string.Format(TxtBusy, p.Name);
            if (p.Level < cfg.MinLevel) return TxtLevel;
            var r = TeamArenaStore.Get(m);
            if (r == null || r.Points < cfg.MinPointsToJoin) return TxtMark;
            uint other;
            if (memberOf.TryGetValue(p.UID, out other) && other != ownLeader) return TxtRepeat;
            if (inMatch.ContainsKey(p.UID)) return TxtRepeat;
            bool busy = false;
            try
            {
                busy = m.InQualifier() || m.IsWatching()
                    || (m.ArenaStatistic != null && m.ArenaStatistic.Info != null && m.ArenaStatistic.Info.Status != Game.MsgServer.MsgArenaInfo.Action.NotSignedUp);
            }
            catch { }
            if (busy) return string.Format(TxtBusy, p.Name);
            try { if (m.MyBooth != null) return TxtBooth; } catch { }
            if (!p.Alive) return string.Format(TxtDead, p.Name);
            bool badMap = false;
            try
            {
                var solo = Game.MsgTournaments.MsgSchedules.Arena;
                if (solo != null && solo.BlockArenaMaps.Contains(p.Map)) badMap = true;
                if (Program.IDFixBug.Contains(p.Map)) badMap = true;
            }
            catch { }
            if (p.Map == (uint)cfg.ArenaMap || p.DynamicID != 0) badMap = true;
            if (badMap) return string.Format(TxtPlace, p.Name);
            return null;
        }

        public static void SignUp(Client.GameClient client)
        {
            var cfg = TeamArenaConfig.Current;
            lock (Sync)
            {
                uint uid = client.Player.UID;
                if (memberOf.ContainsKey(uid) || inMatch.ContainsKey(uid)) { Msg(client, TxtRepeat); return; }
                if (!cfg.IsOpen(Clock())) { Msg(client, TxtTime); return; }
                if (!IsLeader(client)) { Msg(client, TxtNotLeader); return; }
                var team = TeamOf(client);
                if (team == null || team.Count < cfg.MinTeamSize || team.Count > cfg.MaxTeamSize)
                { Msg(client, string.Format(TxtTeamSize, cfg.MinTeamSize, cfg.MaxTeamSize)); return; }
                foreach (var m in team)
                {
                    string why = Blocker(m, cfg, uid);
                    if (why != null) { Msg(client, why); return; }
                }
                var e = new Entry { Leader = uid, Since = Clock() };
                foreach (var m in team) { e.Members.Add(m.Player.UID); memberOf[m.Player.UID] = uid; }
                entries[uid] = e;
                TeamArenaStore.Log("SIGNUP leader " + uid + " " + client.Player.Name + " members " + string.Join(",", e.Members));
                foreach (var m in e.Members) SendStatus(m);
            }
        }

        public static void Quit(Client.GameClient client)
        {
            lock (Sync)
            {
                uint uid = client.Player.UID;
                Match m;
                if (inMatch.TryGetValue(uid, out m))
                {
                    if (m.State == MatchState.Pending) { GiveUp(client); return; }
                    if (m.State == MatchState.Fighting)
                    {
                        var f = m.FighterOf(uid);
                        if (f != null && f.Imported && !f.Out)
                        {
                            f.Out = true;
                            ReturnHome(client, f, m);
                            TeamArenaStore.Log("LEFT match " + m.Id + " uid " + uid);
                        }
                    }
                    return;
                }
                uint leader;
                if (!memberOf.TryGetValue(uid, out leader)) { Interact(uid, TeamArenaPackets.ActQuit, 0); SendStatus(uid); return; }
                Entry e;
                if (entries.TryGetValue(leader, out e)) Unregister(e, "QUIT by " + uid);
                else { uint x; memberOf.TryRemove(uid, out x); SendStatus(uid); }
            }
        }

        static void Unregister(Entry e, string why)
        {
            entries.Remove(e.Leader);
            foreach (var u in e.Members)
            {
                uint x; memberOf.TryRemove(u, out x);
                Interact(u, TeamArenaPackets.ActQuit, 0);
                SendStatus(u);
            }
            TeamArenaStore.Log("UNREGISTER leader " + e.Leader + " (" + why + ")");
        }

        // ------------------------------------------------------------------ buy points (2218 action 5)
        public static void BuyPoints(Client.GameClient client)
        {
            var cfg = TeamArenaConfig.Current;
            lock (Sync)
            {
                if (inMatch.ContainsKey(client.Player.UID)) return;
                var r = TeamArenaStore.Get(client);
                if (r == null || r.Points >= cfg.PointsCap) return;
                uint need = cfg.PointsCap - r.Points;
                ulong cost = (ulong)need * cfg.SilverPerPoint;
                ulong before = client.Player.Money;
                if (before < cost) { Msg(client, TxtBuyFail); return; }
                client.Player.Money = before - cost;                     // spend first (the setter logs + updates the client)
                if (client.Player.Money != before - cost) { TeamArenaStore.Log("BUY ABORT uid " + r.Uid + " money did not change"); return; }
                r.Points = cfg.PointsCap;                                // then grant
                TeamArenaStore.MarkDirty();
                TeamArenaStore.Log("BUY uid " + r.Uid + " " + r.Name + " +" + need + " points for " + cost + " silver (" + before + " -> " + client.Player.Money + ")");
                TeamArenaStore.Save();
                SendHero(client);
            }
        }

        // ------------------------------------------------------------------ accept / give up
        public static void Accept(Client.GameClient client)
        {
            lock (Sync)
            {
                Match m;
                if (!inMatch.TryGetValue(client.Player.UID, out m) || m.State != MatchState.Pending) return;
                var side = m.SideOf(client.Player.UID);
                if (side == null || side.Leader != client.Player.UID) return;   // the leader answers for the team
                side.Accepted = true;
                if (m.A.Accepted && m.B.Accepted) StartFight(m);
            }
        }

        public static void GiveUp(Client.GameClient client)
        {
            lock (Sync)
            {
                Match m;
                uint uid = client.Player.UID;
                if (!inMatch.TryGetValue(uid, out m) || m.State != MatchState.Pending) return;
                var side = m.SideOf(uid);
                if (side == null) return;
                if (side.Leader == uid) { Forfeit(m, side, "leader gave up"); return; }
                var f = m.FighterOf(uid);
                if (f != null) f.Declined = true;                                 // a member only sits this match out
                Match x; inMatch.TryRemove(uid, out x);
                Interact(uid, TeamArenaPackets.ActCloseDlg, 0);
                SendStatus(uid);
                if (!side.Active.Any()) Forfeit(m, side, "no member left");
            }
        }

        // ------------------------------------------------------------------ tick
        public static void Tick()
        {
            var now = Clock();
            var cfg = TeamArenaConfig.Current;
            lock (Sync)
            {
                if (TeamArenaStore.SettleIfNewDay(now)) lastHonorPay = DateTime.MinValue;
                if ((now - lastHonorPay).TotalSeconds >= 10) { lastHonorPay = now; TeamArenaStore.PayPendingHonor(); }

                foreach (var m in matches.Values.ToList())
                {
                    try
                    {
                        if (m.State == MatchState.Pending) TickPending(m, now, cfg);
                        else if (m.State == MatchState.Fighting) TickFight(m, now, cfg);
                        else if (m.State == MatchState.Ending && now >= m.EndAt) Finish(m);
                    }
                    catch (Exception e) { TeamArenaStore.Log("MATCH " + m.Id + " " + e); }
                }

                if ((now - lastMatchmaking).TotalSeconds >= cfg.MatchmakingSeconds)
                {
                    lastMatchmaking = now;
                    Matchmake(now, cfg);
                }

                if ((now - lastSave).TotalSeconds >= 5) { lastSave = now; TeamArenaStore.Save(); }
            }
        }

        /// <summary>Re-checks a waiting team. Returns false (and unregisters it) when it can't play anymore.</summary>
        static bool Validate(Entry e, TeamArenaConfig cfg)
        {
            var leader = Online(e.Leader);
            if (leader == null || !IsLeader(leader)) { Unregister(e, "leader offline / not leader"); return false; }
            var team = TeamOf(leader);
            var now = team == null ? new List<uint>() : team.Select(c => c.Player.UID).ToList();
            if (now.Count != e.Members.Count || now.Except(e.Members).Any())
            {
                foreach (var u in e.Members) { var c = Online(u); if (c != null) Msg(c, TxtOffline); }
                Unregister(e, "team changed");
                return false;
            }
            foreach (var c in team)
            {
                string why = Blocker(c, cfg, e.Leader);
                if (why == null) continue;
                if (why == TxtMark) Interact(c.Player.UID, TeamArenaPackets.ActHeroOut, 0);
                Msg(leader, why);
                Unregister(e, "blocked: " + why);
                return false;
            }
            return true;
        }

        static uint Avg(Entry e)
        {
            ulong sum = 0; int n = 0;
            foreach (var u in e.Members) { var r = TeamArenaStore.Find(u); if (r != null) { sum += r.Points; n++; } }
            return n == 0 ? 0 : (uint)(sum / (ulong)n);
        }

        static void Matchmake(DateTime now, TeamArenaConfig cfg)
        {
            if (!cfg.IsOpen(now))
            {
                foreach (var e in entries.Values.Where(x => x.Match == null).ToList()) { Unregister(e, "closed"); }
                return;
            }
            var waiting = entries.Values.Where(e => e.Match == null).ToList();
            waiting = waiting.Where(e => Validate(e, cfg)).OrderBy(e => Avg(e)).ThenBy(e => e.Since).ToList();
            for (int i = 0; i + 1 < waiting.Count; i += 2) CreateMatch(waiting[i], waiting[i + 1], now);
        }

        static Side MakeSide(Entry e)
        {
            var s = new Side { Leader = e.Leader, AvgPoints = Avg(e) };
            var r = TeamArenaStore.Find(e.Leader);
            s.LeaderName = r != null ? r.Name : "";
            foreach (var u in e.Members) s.Fighters.Add(new Fighter { Uid = u });
            return s;
        }

        static void CreateMatch(Entry x, Entry y, DateTime now)
        {
            var m = new Match { Id = ++TeamArenaStore.MatchCounter, State = MatchState.Pending, A = MakeSide(x), B = MakeSide(y), Created = now };
            x.Match = y.Match = m;
            matches[m.Id] = m;
            TeamArenaStore.MarkDirty();
            foreach (var f in m.A.Fighters.Concat(m.B.Fighters)) inMatch[f.Uid] = m;
            TeamArenaStore.Log("MATCH " + m.Id + " found: " + m.A.Leader + " (" + m.A.AvgPoints + ") vs " + m.B.Leader + " (" + m.B.AvgPoints + ")");
            foreach (var side in new[] { m.A, m.B })
            {
                var opp = Rows(m.Other(side), false);
                uint oppLeader = m.Other(side).Leader;
                foreach (var f in side.Fighters)
                {
                    var c = Online(f.Uid);
                    if (c == null) continue;
                    Send(c, s => TeamArenaPackets.Members(s, 1, oppLeader, opp));
                    Send(c, s => TeamArenaPackets.Interactive(s, TeamArenaPackets.ActMatchFound, 0));
                }
            }
        }

        static void TickPending(Match m, DateTime now, TeamArenaConfig cfg)
        {
            foreach (var side in new[] { m.A, m.B })
                if (Online(side.Leader) == null) { Forfeit(m, side, "leader offline"); return; }
            if ((now - m.Created).TotalSeconds < cfg.AcceptSeconds) return;
            if (!m.A.Accepted && !m.B.Accepted) { Cancel(m, "nobody accepted"); return; }
            Forfeit(m, m.A.Accepted ? m.B : m.A, "did not accept in time");
        }

        // ------------------------------------------------------------------ fight
        static void Teleport(Client.GameClient c, ushort x, ushort y, uint map, uint dyn)
        {
            if (TeleportHook != null) { TeleportHook(c, x, y, map, dyn); return; }
            c.Teleport(x, y, map, dyn);
        }

        static void StartFight(Match m)
        {
            var cfg = TeamArenaConfig.Current;
            uint map = (uint)cfg.ArenaMap;
            Role.GameMap gm = null;
            uint dyn = 0;
            try
            {
                if (NewDynamicId != null) dyn = NewDynamicId();
                else { gm = Pool.ServerMaps != null ? Pool.ServerMaps[map] : null; if (gm != null) dyn = gm.GenerateDynamicID(); }
            }
            catch (Exception e) { TeamArenaStore.Log("MAP " + map + " " + e.Message); }
            if (dyn == 0) { Cancel(m, "arena map " + map + " not available"); return; }
            m.Dyn = dyn;
            m.State = MatchState.Fighting;
            m.Started = Clock();
            foreach (var side in new[] { m.A, m.B })
            {
                foreach (var f in side.Fighters)
                {
                    if (f.Declined) continue;
                    var c = Online(f.Uid);
                    if (c == null || !c.Player.Alive || c.Player.Map == map) { f.Out = true; continue; }
                    f.RetMap = c.Player.Map; f.RetDyn = c.Player.DynamicID; f.RetX = c.Player.X; f.RetY = c.Player.Y;
                    ushort x = 50, y = 50;
                    if (gm != null) gm.GetRandCoord(ref x, ref y);
                    try
                    {
                        if (c.Player.MyJiangHu != null && c.Player.JiangHuActive != 0)
                        {
                            c.Player.PkMode = Role.Flags.PKMode.Capture;
                            c.Player.MyJiangHu.OnJiangMode = false;
                            c.Player.MyJiangHu.DisableJiang(c);
                        }
                    }
                    catch { }
                    Teleport(c, x, y, map, m.Dyn);
                    if (c.Player.Map != map || c.Player.DynamicID != m.Dyn) { f.Out = true; continue; }  // teleport refused
                    f.Imported = true;
                    f.LastHp = c.Player.HitPoints;
                    if (TeleportHook == null)
                    {
                        try { c.Player.ProtectJumpAttack(10); } catch { }
                        try { c.Player.SetPkMode(Role.Flags.PKMode.PK); } catch { }
                    }
                }
            }
            TeamArenaStore.Log("MATCH " + m.Id + " start dyn " + m.Dyn + " A " + string.Join(",", m.A.Fighters.Where(f => f.Imported).Select(f => f.Uid))
                + " B " + string.Join(",", m.B.Fighters.Where(f => f.Imported).Select(f => f.Uid)));
            foreach (var f in m.A.Fighters.Concat(m.B.Fighters).Where(f => f.Imported))
            {
                Interact(f.Uid, TeamArenaPackets.ActStartFight, 0);
                SendStatus(f.Uid);
            }
            SendScore(m);
            bool aOut = !m.A.Fighters.Any(f => f.Imported), bOut = !m.B.Fighters.Any(f => f.Imported);
            if (aOut || bOut) End(m, aOut && bOut ? m.B : (aOut ? m.B : m.A), "no fighters");
        }

        static void SendScore(Match m)
        {
            m.ScoreDirty = false; m.LastScore = Clock();
            foreach (var f in m.A.Fighters.Concat(m.B.Fighters).Where(f => f.Imported))
            {
                var c = Online(f.Uid);
                if (c != null) Send(c, s => TeamArenaPackets.Score(s, m.A.Leader, m.A.LeaderName, m.A.Damage, m.B.Leader, m.B.LeaderName, m.B.Damage));
            }
        }

        static void TickFight(Match m, DateTime now, TeamArenaConfig cfg)
        {
            foreach (var side in new[] { m.A, m.B })
            {
                var other = m.Other(side);
                foreach (var f in side.Fighters)
                {
                    if (!f.Imported || f.Out) continue;
                    var c = Online(f.Uid);
                    if (c == null || !InArena(c.Player, m)) { f.Out = true; continue; }
                    int hp = c.Player.HitPoints;
                    if (hp < f.LastHp) { other.Damage = TeamArenaStore.SatAdd(other.Damage, (uint)(f.LastHp - hp)); m.ScoreDirty = true; }
                    f.LastHp = hp;
                    if (!c.Player.Alive) f.Out = true;
                }
            }
            if (m.ScoreDirty && (now - m.LastScore).TotalMilliseconds >= 900) SendScore(m);
            bool aDown = !m.A.Fighters.Any(f => f.Imported && !f.Out), bDown = !m.B.Fighters.Any(f => f.Imported && !f.Out);
            if (aDown || bDown)
            {
                Side w = aDown && bDown ? (m.A.Damage > m.B.Damage ? m.A : m.B) : (aDown ? m.B : m.A);
                End(m, w, "knockout");
                return;
            }
            if ((now - m.Started).TotalSeconds >= cfg.FightSeconds)
                End(m, m.A.Damage > m.B.Damage ? m.A : m.B, "time");   // tie -> A loses (same as the solo Qualifier)
        }

        static void End(Match m, Side winner, string why)
        {
            if (m.State != MatchState.Fighting) return;
            m.State = MatchState.Ending;
            m.Winner = winner;
            m.EndAt = Clock().AddSeconds(TeamArenaConfig.Current.EndDelaySeconds);
            SendScore(m);
            ApplyResult(m, winner, true, why);
            foreach (var f in m.A.Fighters.Concat(m.B.Fighters).Where(f => f.Imported))
            {
                var c = Online(f.Uid);
                if (c == null) continue;
                try { if (TeleportHook == null) c.Player.ProtectAttack(TeamArenaConfig.Current.EndDelaySeconds * 1000 + 1000); } catch { }
                Interact(f.Uid, TeamArenaPackets.ActResult, m.SideOf(f.Uid) == winner ? 1u : 0u);
            }
            Announce(winner, m.Other(winner));
        }

        static void Announce(Side w, Side l)
        {
            string text = w.LeaderName + "'s team has defeated " + l.LeaderName + "'s team in the Team Qualifier.";
            if (Capture != null) { Capture(0, System.Text.Encoding.UTF8.GetBytes("GLOBAL:" + text)); return; }
            try
            {
                using (var rec = new ServerSockets.RecycledPacket())
                    Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(text, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Qualifier).GetArray(rec.GetStream()));
            }
            catch { }
        }

        static void ReturnHome(Client.GameClient c, Fighter f, Match m)
        {
            if (c == null || f == null || !f.Imported) return;
            try
            {
                if (InArena(c.Player, m) && f.RetMap != 0)
                    Teleport(c, f.RetX, f.RetY, f.RetMap, f.RetDyn);
                if (TeleportHook == null) c.Player.RestorePkMode();
            }
            catch (Exception e) { TeamArenaStore.Log("RETURN " + f.Uid + " " + e.Message); }
        }

        static void Finish(Match m)
        {
            m.State = MatchState.Done;
            foreach (var side in new[] { m.A, m.B })
            {
                foreach (var f in side.Fighters)
                {
                    var c = Online(f.Uid);
                    if (c != null) ReturnHome(c, f, m);
                }
            }
            Close(m);
            foreach (var side in new[] { m.A, m.B }) Interact(side.Leader, TeamArenaPackets.ActContinueDlg, 0);
        }

        /// <summary>Removes the match and both registrations; everybody back to "not joined".</summary>
        static void Close(Match m)
        {
            m.State = MatchState.Done;
            matches.Remove(m.Id);
            foreach (var side in new[] { m.A, m.B })
            {
                Entry e;
                if (entries.TryGetValue(side.Leader, out e) && e.Match == m) entries.Remove(side.Leader);
                foreach (var f in side.Fighters)
                {
                    Match x; uint y;
                    if (inMatch.TryGetValue(f.Uid, out x) && x == m) inMatch.TryRemove(f.Uid, out x);
                    if (memberOf.TryGetValue(f.Uid, out y) && y == side.Leader) memberOf.TryRemove(f.Uid, out y);
                    SendStatus(f.Uid);
                }
            }
        }

        static void Forfeit(Match m, Side loser, string why)
        {
            if (m.State != MatchState.Pending) return;
            var winner = m.Other(loser);
            m.State = MatchState.Ending;
            m.Winner = winner;
            ApplyResult(m, winner, false, "forfeit: " + why);
            foreach (var f in loser.Fighters) Interact(f.Uid, TeamArenaPackets.ActQuit, 0);
            foreach (var f in winner.Fighters) Interact(f.Uid, TeamArenaPackets.ActRivalGaveUp, 0);
            Close(m);
            Interact(winner.Leader, TeamArenaPackets.ActContinueDlg, 0);
        }

        static void Cancel(Match m, string why)
        {
            if (m.State != MatchState.Pending) return;
            TeamArenaStore.Log("MATCH " + m.Id + " cancelled: " + why);
            foreach (var f in m.A.Fighters.Concat(m.B.Fighters)) Interact(f.Uid, TeamArenaPackets.ActQuit, 0);
            Close(m);
        }

        // ------------------------------------------------------------------ results + rewards (once per match)
        public static int PointsDelta(uint winnerAvg, uint loserAvg, TeamArenaConfig cfg)
        {
            long d = cfg.PointsBase + ((long)loserAvg - winnerAvg) / cfg.PointsDivisor;
            return (int)Math.Max(cfg.PointsMin, Math.Min(cfg.PointsMax, d));
        }

        static void ApplyResult(Match m, Side winner, bool fought, string why)
        {
            if (m.Applied) return;
            m.Applied = true;
            var cfg = TeamArenaConfig.Current;
            var loser = m.Other(winner);
            int delta = PointsDelta(winner.AvgPoints, loser.AvgPoints, cfg);
            TeamArenaStore.Log("RESULT match " + m.Id + " winner " + winner.Leader + " loser " + loser.Leader + " delta " + delta + " dmg " + m.A.Damage + "/" + m.B.Damage + " (" + why + ")");
            foreach (var side in new[] { winner, loser })
            {
                bool won = side == winner;
                foreach (var f in side.Fighters)
                {
                    if (f.Declined) continue;
                    if (fought && !f.Imported) continue;              // only players who were in the arena
                    var r = TeamArenaStore.Find(f.Uid);
                    if (r == null) continue;
                    if (won) { r.TodayWin++; r.TotalWin++; r.Points = TeamArenaStore.SatAdd(r.Points, (uint)delta); }
                    else { r.TodayLose++; r.TotalLose++; r.Points = r.Points > (uint)delta ? r.Points - (uint)delta : 0; }
                    if (fought)
                    {
                        r.TodayFights++;
                        global::Qahr.Mobile.Phone.MobileDaily.EventUid(f.Uid, "teamarena");// QahrMobile: mobile daily Team Qualifier
                        bool reward = (won && r.TodayWin <= (uint)cfg.RewardWinsUpTo) || cfg.RewardJoinCounts.Contains((int)r.TodayFights);
                        if (reward) Reward(f.Uid, r, m, cfg);
                    }
                    TeamArenaStore.Log("  uid " + r.Uid + " " + r.Name + (won ? " WIN" : " LOSE") + " points " + r.Points + " today " + r.TodayWin + "/" + r.TodayLose + " fights " + r.TodayFights);
                }
            }
            TeamArenaStore.MarkDirty();
            TeamArenaStore.Save();
        }

        static void Reward(uint uid, TeamArenaRecord r, Match m, TeamArenaConfig cfg)
        {
            var c = Online(uid);
            if (c == null) { TeamArenaStore.Log("  REWARD skipped (offline) uid " + uid + " match " + m.Id); return; }
            bool ok = false;
            try
            {
                if (GrantHook != null) ok = GrantHook(c, cfg.RewardItem, cfg.RewardChampionPoints);
                else
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var s = rec.GetStream();
                        if (cfg.RewardItem != 0 && Pool.ItemsBase != null && Pool.ItemsBase.ContainsKey(cfg.RewardItem))
                        {
                            if (c.Inventory.HaveSpace(1)) c.Inventory.AddItemWitchStack(cfg.RewardItem, 0, 1, s);
                            else c.Inventory.AddReturnedItem(s, cfg.RewardItem);
                        }
                        if (cfg.RewardChampionPoints > 0) c.Player.AddChampionPoints(cfg.RewardChampionPoints);
                        ok = true;
                    }
                }
            }
            catch (Exception e) { TeamArenaStore.Log("  REWARD FAILED uid " + uid + " " + e.Message); }
            if (ok)
            {
                r.TodayPacks++;
                TeamArenaStore.Log("  REWARD uid " + uid + " " + r.Name + " item " + cfg.RewardItem + " + " + cfg.RewardChampionPoints + " CP (match " + m.Id + ", packs today " + r.TodayPacks + ")");
                Msg(c, "You received a TeamArenaPack and " + cfg.RewardChampionPoints + " Champion Points.");
            }
        }

        // ------------------------------------------------------------------ tests
        public static void ResetForTests()
        {
            lock (Sync)
            {
                entries.Clear(); matches.Clear(); memberOf.Clear(); inMatch.Clear();
                lastMatchmaking = lastSave = lastHonorPay = DateTime.MinValue;
            }
        }
        public static int MatchCount { get { lock (Sync) return matches.Count; } }
        public static int EntryCount { get { lock (Sync) return entries.Count; } }
        public static Match MatchOf(uint uid) { Match m; return inMatch.TryGetValue(uid, out m) ? m : null; }
    }
}
