// [GuildRecruit 2026-10-14] Union (League) join applications and league-order status.
// Official 7957 flow (reverse-engineered from Conquer.exe):
//   apply   : MsgLeagueOpt(2461) action 8 Pledge (hero, Index = union id)
//             MsgLeagueOpt action 11 GuildPledge (guild leader with the whole guild, Index = union id)
//   review  : 2384 CMsgLeagueApplyList - client sends an empty request right after it opens CDlgLeagueSynList (702)
//             (@0x10ce01d). Reply f2 bit0 = clear list, bit1 = open CDlgLeagueEnrolling (3822); rows f3 =
//             CMsgLeagueApplyListInfo {1 id, 2 name, 3 level, 4 class, 5 battle power, 6 guild flag, 7 guild name}.
//   decide  : MsgLeagueOpt Index = row id: 82 agree hero / 85 agree guild (row has f6 != 0, @0xaf9a30) / 83 refuse,
//             86 agree all / 87 refuse all (CDlgLeagueEnrolling buttons 6504/6505 @0xad252c/@0xad259f).
//   Only the union leader (Emperor rank) decides (derived: the client shows the dialog to whoever gets bit1).
// State: Infinity[DB]\GuildRecruit\league_apply.txt.
//
// 2393 CMsgTokenUpdate: Ids -> Infos {Token, Used, Seconds left}; the client stores cooldown = now + Seconds*1000.
// League orders (tokens 101/102/103/106/204/205/206) are issued through MsgLeagueToken 2305, whose server handler is an
// empty stub today, so no order has ever been issued: the status comes from LeagueOrders (Infinity[DB]\GuildRecruit\
// league_orders.txt) which is empty until an order system calls LeagueOrders.RecordUse.
using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Qahr.Mobile.GuildRecruit
{
    [ProtoContract]
    public class LeagueApplyInfoPB// CMsgLeagueApplyListInfo
    {
        [ProtoMember(1)] public uint Id;
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public uint Level;
        [ProtoMember(4)] public uint Class;
        [ProtoMember(5)] public uint BattlePower;
        [ProtoMember(6)] public uint IsGuild;
        [ProtoMember(7)] public string GuildName;
    }
    [ProtoContract]
    public class LeagueApplyListPB// 2384 CMsgLeagueApplyListPB
    {
        [ProtoMember(1)] public uint f1;
        [ProtoMember(2)] public uint Flags;
        [ProtoMember(3)] public List<LeagueApplyInfoPB> Items = new List<LeagueApplyInfoPB>();
    }

    [ProtoContract]
    public class TokenUpdateInfoPB// CMsgTokenUpdateInfoPB
    {
        [ProtoMember(1, IsRequired = true)] public uint Token;
        [ProtoMember(2, IsRequired = true)] public uint Used;
        [ProtoMember(3, IsRequired = true)] public uint Seconds;
    }
    [ProtoContract]
    public class TokenUpdatePB// 2393 CMsgTokenUpdatePB
    {
        [ProtoMember(1)] public List<uint> Ids = new List<uint>();
        [ProtoMember(2)] public List<TokenUpdateInfoPB> Infos = new List<TokenUpdateInfoPB>();
    }

    public sealed class LeagueApplication
    {
        public uint UnionId, Id;// Id = hero uid, or guild id when IsGuild
        public byte IsGuild;
        public string Name = "", GuildName = "";
        public uint Level, Class, BattlePower;
        public DateTime Time;

        public string ToLine()
        {
            return string.Join("\t", UnionId, Id, IsGuild, RecruitIO.Field(Name), RecruitIO.Field(GuildName), Level, Class, BattlePower, Time.Ticks);
        }
        public static LeagueApplication FromLine(string l)
        {
            var p = l.Split('\t');
            if (p.Length < 9) return null;
            var a = new LeagueApplication();
            long t;
            if (!uint.TryParse(p[0], out a.UnionId) || !uint.TryParse(p[1], out a.Id) || !byte.TryParse(p[2], out a.IsGuild) || !long.TryParse(p[8], out t)) return null;
            a.Name = p[3]; a.GuildName = p[4];
            uint.TryParse(p[5], out a.Level); uint.TryParse(p[6], out a.Class); uint.TryParse(p[7], out a.BattlePower);
            a.Time = new DateTime(t);
            return a;
        }
    }

    public static class LeagueApply
    {
        public const ushort IdApplyList = 2384;
        public const ushort ActPledge = 8, ActGuildPledge = 11, ActAgreeHero = 82, ActRefuse = 83, ActAgreeGuild = 85, ActAgreeAll = 86, ActRefuseAll = 87;

        static readonly object Sync = new object();
        static List<LeagueApplication> _apps;
        static string StoreFile { get { return RecruitIO.FilePath("league_apply.txt"); } }

        static List<LeagueApplication> Apps
        {
            get
            {
                if (_apps == null)
                {
                    _apps = new List<LeagueApplication>();
                    foreach (var l in RecruitIO.ReadLines(StoreFile))
                    {
                        var a = LeagueApplication.FromLine(l);
                        if (a != null) _apps.Add(a);
                    }
                }
                return _apps;
            }
        }
        public static void ResetForTests() { lock (Sync) _apps = null; }

        static void Save()
        {
            try { RecruitIO.WriteAtomic(StoreFile, Apps.Select(a => a.ToLine()).ToArray()); }
            catch (Exception e) { RecruitIO.Error(e); }
        }
        static void Purge_NoLock()
        {
            var cutoff = RecruitIO.Now.AddHours(-RecruitIO.Config.ApplyExpireHours);
            Apps.RemoveAll(a => a.Time < cutoff || !Role.Instance.Union.UnionPoll.ContainsKey(a.UnionId));
        }

        public static List<LeagueApplication> ListFor(uint unionId)
        {
            lock (Sync)
            {
                Purge_NoLock();
                return Apps.Where(a => a.UnionId == unionId).OrderBy(a => a.Time).ToList();
            }
        }

        public static bool IsManager(Client.GameClient c)
        {
            return c != null && c.Player != null && c.Player.IsUnionEmperror;
        }

        // ------------------------------------------------------------------ hook (MsgLeagueOpt, before its switch)
        /// <summary>true = consumed here. index = the packet's Index field (union id for 8/11, row id for 82/83/85).</summary>
        public static bool OnLeagueOpt(Client.GameClient user, ushort action, uint index, ServerSockets.Packet stream)
        {
            try
            {
                switch (action)
                {
                    case ActPledge:
                    case ActAgreeHero:
                    case ActRefuse:
                    case ActAgreeGuild:
                    case ActAgreeAll:
                    case ActRefuseAll:
                        break;
                    case ActGuildPledge:
                        if (RecruitIO.Config.GuildPledgeNeedsApproval == 0) return false;// old direct join
                        break;
                    default:
                        return false;
                }
                if (!RecruitUtil.Ready(user)) return true;
                if (!RecruitThrottle.Allow(user, 4)) return true;
                switch (action)
                {
                    case ActPledge: ApplyHero(user, index); break;
                    case ActGuildPledge: ApplyGuild(user, index); break;
                    case ActAgreeHero: Decide(user, index, false, true, stream); break;
                    case ActAgreeGuild: Decide(user, index, true, true, stream); break;
                    case ActRefuse: Refuse(user, index, stream); break;
                    case ActAgreeAll: DecideAll(user, true, stream); break;
                    case ActRefuseAll: DecideAll(user, false, stream); break;
                }
            }
            catch (Exception e) { RecruitIO.Error(e); }
            return true;
        }

        static bool AddApp(Client.GameClient user, LeagueApplication app)
        {
            var cfg = RecruitIO.Config;
            lock (Sync)
            {
                Purge_NoLock();
                if (Apps.Any(a => a.UnionId == app.UnionId && a.Id == app.Id && a.IsGuild == app.IsGuild))
                {
                    RecruitUtil.Msg(user, "You have already applied to join this union. Please wait for a reply.");
                    return false;
                }
                if (Apps.Count(a => a.Id == app.Id && a.IsGuild == app.IsGuild) >= cfg.MaxPendingPerPlayer)
                {
                    RecruitUtil.Msg(user, "You have too many union applications waiting for a reply.");
                    return false;
                }
                if (Apps.Count(a => a.UnionId == app.UnionId) >= cfg.MaxApplicationsPerGuild)
                {
                    RecruitUtil.Msg(user, "This union has too many applications. Please try again later.");
                    return false;
                }
                Apps.Add(app);
                Save();
            }
            return true;
        }

        public static void ApplyHero(Client.GameClient user, uint unionId)
        {
            var p = user.Player;
            Role.Instance.Union union;
            if (unionId == 0 || !Role.Instance.Union.UnionPoll.TryGetValue(unionId, out union) || union == null) return;
            if (p.InUnion || p.MyUnion != null) { RecruitUtil.Msg(user, "You already belong to a union."); return; }
            var app = new LeagueApplication
            {
                UnionId = unionId, Id = p.UID, IsGuild = 0, Name = p.Name ?? "",
                GuildName = p.MyGuild != null ? p.MyGuild.GuildName : "",
                Level = p.Level, Class = p.Class, BattlePower = (uint)Math.Max(0, p.BattlePower), Time = RecruitIO.Now
            };
            if (!AddApp(user, app)) return;
            RecruitIO.Log("league-apply hero union=" + unionId + " uid=" + p.UID);
            RecruitUtil.Msg(user, "Your application has been sent to the union " + union.Name + ".");
            NotifyLeader(union, p.Name + " applied to join the union.");
        }

        public static void ApplyGuild(Client.GameClient user, uint unionId)
        {
            var p = user.Player;
            Role.Instance.Union union;
            if (unionId == 0 || !Role.Instance.Union.UnionPoll.TryGetValue(unionId, out union) || union == null) return;
            if (p.InUnion || p.MyGuild == null || p.MyGuildMember == null || p.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader) return;
            if (p.MyGuild.UnionID != 0) { RecruitUtil.Msg(user, "Your guild already belongs to a union."); return; }
            var app = new LeagueApplication
            {
                UnionId = unionId, Id = p.MyGuild.Info.GuildID, IsGuild = 1, Name = p.Name ?? "",
                GuildName = p.MyGuild.GuildName ?? "", Level = p.MyGuild.Info.Level, Class = p.Class,
                BattlePower = (uint)Math.Max(0, p.BattlePower), Time = RecruitIO.Now
            };
            if (!AddApp(user, app)) return;
            RecruitIO.Log("league-apply guild union=" + unionId + " guild=" + app.Id + " leader=" + p.UID);
            RecruitUtil.Msg(user, "Your guild`s application has been sent to the union " + union.Name + ".");
            NotifyLeader(union, "Guild " + app.GuildName + " applied to join the union.");
        }

        static void NotifyLeader(Role.Instance.Union union, string text)
        {
            if (union.EmperrorUID != 0) RecruitUtil.Msg(RecruitUtil.Online(union.EmperrorUID), text);
        }

        static LeagueApplication Find(uint unionId, uint id, bool guild)
        {
            lock (Sync)
            {
                Purge_NoLock();
                return Apps.FirstOrDefault(a => a.UnionId == unionId && a.Id == id && (a.IsGuild != 0) == guild);
            }
        }
        static void Remove(LeagueApplication app)
        {
            lock (Sync) { if (Apps.Remove(app)) Save(); }
        }
        static void RemoveWhere(Func<LeagueApplication, bool> pred)
        {
            lock (Sync) { if (Apps.RemoveAll(a => pred(a)) > 0) Save(); }
        }

        public static void Decide(Client.GameClient leader, uint id, bool guild, bool agree, ServerSockets.Packet stream)
        {
            if (!IsManager(leader)) return;
            var union = leader.Player.MyUnion;
            var app = Find(union.UID, id, guild);
            if (app == null) return;
            DecideOne(leader, union, app, agree, stream);
        }

        static void Refuse(Client.GameClient leader, uint id, ServerSockets.Packet stream)
        {
            if (!IsManager(leader)) return;
            var union = leader.Player.MyUnion;
            var app = Find(union.UID, id, false) ?? Find(union.UID, id, true);
            if (app == null) return;
            DecideOne(leader, union, app, false, stream);
        }

        static void DecideOne(Client.GameClient leader, Role.Instance.Union union, LeagueApplication app, bool agree, ServerSockets.Packet stream)
        {
            if (!agree)
            {
                Remove(app);
                RecruitIO.Log("league-refuse union=" + union.UID + " id=" + app.Id + " guild=" + app.IsGuild + " by=" + leader.Player.UID);
                var who = app.IsGuild != 0 ? GuildLeaderOnline(app.Id) : RecruitUtil.Online(app.Id);
                if (who != null) RecruitUtil.Msg(who, "Your application to join the union " + union.Name + " was refused.");
                return;
            }
            if (app.IsGuild != 0)
            {
                Role.Instance.Guild g;
                if (!Role.Instance.Guild.GuildPoll.TryGetValue(app.Id, out g) || g == null || g.UnionID != 0)
                {
                    Remove(app);
                    return;
                }
                union.AddGuild(stream, g);
                RemoveWhere(a => a.IsGuild != 0 && a.Id == app.Id);
                RecruitIO.Log("league-agree guild union=" + union.UID + " guild=" + app.Id + " by=" + leader.Player.UID);
                g.SendMessajGuild("Our guild joined the union " + union.Name + ".");
                return;
            }
            var hero = RecruitUtil.Online(app.Id);
            if (hero == null)
            {
                RecruitUtil.Msg(leader, app.Name + " is offline. The application is kept until the hero is online.");
                return;
            }
            if (hero.Player.InUnion || hero.Player.MyUnion != null)
            {
                RemoveWhere(a => a.IsGuild == 0 && a.Id == app.Id);
                return;
            }
            union.AddOtherMember(stream, hero);
            RemoveWhere(a => a.IsGuild == 0 && a.Id == app.Id);
            RecruitIO.Log("league-agree hero union=" + union.UID + " uid=" + app.Id + " by=" + leader.Player.UID);
            RecruitUtil.Msg(hero, "You joined the union " + union.Name + ".");
        }

        static Client.GameClient GuildLeaderOnline(uint guildId)
        {
            Role.Instance.Guild g;
            if (!Role.Instance.Guild.GuildPoll.TryGetValue(guildId, out g) || g == null || g.Info == null) return null;
            return RecruitUtil.Online(g.Info.Leaderid);
        }

        static void DecideAll(Client.GameClient leader, bool agree, ServerSockets.Packet stream)
        {
            if (!IsManager(leader)) return;
            var union = leader.Player.MyUnion;
            foreach (var a in ListFor(union.UID))
                DecideOne(leader, union, a, agree, stream);
        }

        // ------------------------------------------------------------------ 2384
        public static LeagueApplyListPB Answer(Client.GameClient c, LeagueApplyListPB req)
        {
            var ans = new LeagueApplyListPB { Flags = 1 };// bit0: clear
            if (!IsManager(c)) return ans;
            var list = ListFor(c.Player.MyUnion.UID);
            foreach (var a in list.Take(RecruitIO.Config.MaxApplicationsPerGuild))
                ans.Items.Add(new LeagueApplyInfoPB
                {
                    Id = a.Id, Name = a.Name ?? "", Level = a.Level, Class = a.Class, BattlePower = a.BattlePower,
                    IsGuild = a.IsGuild, GuildName = a.GuildName ?? ""
                });
            if (ans.Items.Count > 0) ans.Flags |= 2;// open the application list for the union leader
            return ans;
        }

        public static ServerSockets.Packet ReplyList(Client.GameClient c, ServerSockets.Packet stream)
        {
            LeagueApplyListPB req;
            try { req = stream.ProtoBufferDeserialize(new LeagueApplyListPB()); } catch { return null; }
            stream.InitWriter();
            stream.ProtoBufferSerialize(Answer(c, req));
            stream.Finalize(IdApplyList);
            return stream;
        }
    }

    /// <summary>League order (token) usage + cooldown per union, for 2393.</summary>
    public static class LeagueOrders
    {
        public const ushort IdTokenUpdate = 2393;
        public const int MaxIds = 32;
        public const uint MaxTokenId = 100000;

        sealed class Use { public uint UnionId, Token, Used; public DateTime WeekStart, CooldownEnd; }
        static readonly object Sync = new object();
        static List<Use> _uses;
        static string StoreFile { get { return RecruitIO.FilePath("league_orders.txt"); } }

        static List<Use> Uses
        {
            get
            {
                if (_uses == null)
                {
                    _uses = new List<Use>();
                    foreach (var l in RecruitIO.ReadLines(StoreFile))
                    {
                        var p = l.Split('\t');
                        if (p.Length < 5) continue;
                        var u = new Use(); long w, cd;
                        if (!uint.TryParse(p[0], out u.UnionId) || !uint.TryParse(p[1], out u.Token) || !uint.TryParse(p[2], out u.Used)
                            || !long.TryParse(p[3], out w) || !long.TryParse(p[4], out cd)) continue;
                        u.WeekStart = new DateTime(w); u.CooldownEnd = new DateTime(cd);
                        _uses.Add(u);
                    }
                }
                return _uses;
            }
        }
        public static void ResetForTests() { lock (Sync) _uses = null; }

        static DateTime WeekStart(DateTime d)
        {
            int diff = ((int)d.DayOfWeek + 6) % 7;// Monday-based week
            return d.Date.AddDays(-diff);
        }

        /// <summary>For the order system: record one issue of a token by a union with its cooldown.</summary>
        public static void RecordUse(uint unionId, uint token, int cooldownSeconds)
        {
            lock (Sync)
            {
                var now = RecruitIO.Now;
                var u = Uses.FirstOrDefault(x => x.UnionId == unionId && x.Token == token);
                if (u == null) { u = new Use { UnionId = unionId, Token = token, WeekStart = WeekStart(now) }; Uses.Add(u); }
                if (u.WeekStart != WeekStart(now)) { u.WeekStart = WeekStart(now); u.Used = 0; }
                u.Used++;
                u.CooldownEnd = now.AddSeconds(Math.Max(0, cooldownSeconds));
                try { RecruitIO.WriteAtomic(StoreFile, Uses.Select(x => string.Join("\t", x.UnionId, x.Token, x.Used, x.WeekStart.Ticks, x.CooldownEnd.Ticks)).ToArray()); }
                catch (Exception e) { RecruitIO.Error(e); }
            }
        }

        public static TokenUpdatePB Answer(TokenUpdatePB req, uint unionId, bool inUnion)
        {
            var ans = new TokenUpdatePB();
            if (!inUnion || req == null || req.Ids == null) return ans;
            var seen = new HashSet<uint>();
            var now = RecruitIO.Now;
            lock (Sync)
            {
                foreach (var id in req.Ids)
                {
                    if (ans.Infos.Count >= MaxIds) break;
                    if (id == 0 || id > MaxTokenId || !seen.Add(id)) continue;
                    var u = Uses.FirstOrDefault(x => x.UnionId == unionId && x.Token == id);
                    uint used = 0, secs = 0;
                    if (u != null)
                    {
                        if (u.WeekStart == WeekStart(now)) used = u.Used;
                        if (u.CooldownEnd > now) secs = (uint)Math.Min(int.MaxValue, Math.Ceiling((u.CooldownEnd - now).TotalSeconds));
                    }
                    ans.Infos.Add(new TokenUpdateInfoPB { Token = id, Used = used, Seconds = secs });
                }
            }
            return ans;
        }

        public static ServerSockets.Packet Reply(Client.GameClient c, ServerSockets.Packet stream)
        {
            TokenUpdatePB req;
            try { req = stream.ProtoBufferDeserialize(new TokenUpdatePB()); } catch { return null; }
            bool inUnion = c.Player.InUnion;
            uint unionId = inUnion ? c.Player.MyUnion.UID : 0;
            stream.InitWriter();
            stream.ProtoBufferSerialize(Answer(req, unionId, inUnion));
            stream.Finalize(IdTokenUpdate);
            return stream;
        }
    }
}
