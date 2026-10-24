// [GuildRecruit 2026-10-14] Guild join applications (official 7957 flow, reverse-engineered from Conquer.exe):
//   apply   : MsgSyndicate(2038) action 55 Join (UID = guild id)  or  2342 CMsgSynRecruitAdvertisingOpt action 1 (board "Join")
//   review  : 2351 CMsgSyndicateApplyListSyn (client sends an empty request when CDlgFactionEnrolling 3601 opens;
//             the reply list rows are CApplicationInfo {1 uid, 2 name, 3 level, 4 class, 5 battle power, 6 reborn})
//   decide  : MsgSyndicate action 56 agree one / 57 refuse one (UID = applicant uid, chat-link buttons @0x4a18d2)
//             action 58 agree all / 59 refuse all (CDlgFactionEnrolling buttons 6504/6505 @0x483f2a/0x483f9b)
// Recruitment flags already stored on the guild (MsgSyndicate 24 Requirements):
//   RecruitmentON  = "Recruitment on. Auto-recruit all heroes who meet requirements."  -> join at once
//   RecruitmentOFF = "Recruitment off. No application will be accepted."                -> refused
//   neither        -> the application waits for a leader / deputy / manager.
// State: Data\GuildRecruit\guild_apply.txt (one line per application).
using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Qahr.Mobile.GuildRecruit
{
    [ProtoContract]
    public class ApplicationInfoPB// CApplicationInfo (schema.json: all 6 required)
    {
        [ProtoMember(1, IsRequired = true)] public uint Uid;
        [ProtoMember(2, IsRequired = true)] public string Name = "";
        [ProtoMember(3, IsRequired = true)] public int Level;
        [ProtoMember(4, IsRequired = true)] public int Class;
        [ProtoMember(5, IsRequired = true)] public int BattlePower;
        [ProtoMember(6, IsRequired = true)] public int Reborn;
    }
    [ProtoContract]
    public class SyndicateApplyListPB// 2351 CMsgSyndicateApplyListSyncPB
    {
        [ProtoMember(1)] public List<ApplicationInfoPB> Items = new List<ApplicationInfoPB>();
    }

    public sealed class GuildApplication
    {
        public uint GuildId, Uid;
        public string Name = "";
        public int Level, Class, BattlePower, Reborn;
        public DateTime Time;

        public string ToLine()
        {
            return string.Join("\t", GuildId, Uid, RecruitIO.Field(Name), Level, Class, BattlePower, Reborn, Time.Ticks);
        }
        public static GuildApplication FromLine(string l)
        {
            var p = l.Split('\t');
            if (p.Length < 8) return null;
            var a = new GuildApplication();
            long ticks;
            if (!uint.TryParse(p[0], out a.GuildId) || !uint.TryParse(p[1], out a.Uid) || !long.TryParse(p[7], out ticks)) return null;
            a.Name = p[2];
            int.TryParse(p[3], out a.Level); int.TryParse(p[4], out a.Class); int.TryParse(p[5], out a.BattlePower); int.TryParse(p[6], out a.Reborn);
            a.Time = new DateTime(ticks);
            return a;
        }
    }

    public static class GuildApply
    {
        public const ushort IdApplyList = 2351;
        public const ushort IdRecruitOpt = 2342;
        public const uint ActJoin = 55, ActAgree = 56, ActRefuse = 57, ActAgreeAll = 58, ActRefuseAll = 59;

        static readonly object Sync = new object();
        static List<GuildApplication> _apps;

        static string StoreFile { get { return RecruitIO.FilePath("guild_apply.txt"); } }

        static List<GuildApplication> Apps
        {
            get
            {
                if (_apps == null)
                {
                    _apps = new List<GuildApplication>();
                    foreach (var l in RecruitIO.ReadLines(StoreFile))
                    {
                        var a = GuildApplication.FromLine(l);
                        if (a != null) _apps.Add(a);
                    }
                }
                return _apps;
            }
        }
        /// <summary>Test seam: forget the in-memory copy (reloads from disk on next use).</summary>
        public static void ResetForTests() { lock (Sync) _apps = null; }

        static void Save()
        {
            try { RecruitIO.WriteAtomic(StoreFile, Apps.Select(a => a.ToLine()).ToArray()); }
            catch (Exception e) { RecruitIO.Error(e); }
        }

        static void PurgeExpired_NoLock()
        {
            var cutoff = RecruitIO.Now.AddHours(-RecruitIO.Config.ApplyExpireHours);
            Apps.RemoveAll(a => a.Time < cutoff || !Role.Instance.Guild.GuildPoll.ContainsKey(a.GuildId));
        }

        public static List<GuildApplication> ListFor(uint guildId)
        {
            lock (Sync)
            {
                PurgeExpired_NoLock();
                return Apps.Where(a => a.GuildId == guildId).OrderBy(a => a.Time).ToList();
            }
        }

        // ------------------------------------------------------------------ hook (MsgGuildProces, before its switch)
        /// <summary>Handles MsgSyndicate actions 55..59. Returns true when the action was consumed here,
        /// false to let the original handler run (Join into a guild with auto-recruit on).</summary>
        public static bool OnSyndicate(Client.GameClient user, uint action, uint uid, ServerSockets.Packet stream)
        {
            try
            {
                if (action < ActJoin || action > ActRefuseAll) return false;
                if (!RecruitUtil.Ready(user)) return true;
                if (!RecruitThrottle.Allow(user, 0)) return true;
                switch (action)
                {
                    case ActJoin: return !Apply(user, uid, false, stream);
                    case ActAgree: Decide(user, uid, true, stream); return true;
                    case ActRefuse: Decide(user, uid, false, stream); return true;
                    case ActAgreeAll: DecideAll(user, true, stream); return true;
                    case ActRefuseAll: DecideAll(user, false, stream); return true;
                }
            }
            catch (Exception e) { RecruitIO.Error(e); return true; }
            return false;
        }

        /// <summary>Apply to a guild. Returns true when the caller should run the direct join (auto-recruit on),
        /// false when the request was handled (queued / refused).</summary>
        public static bool Apply(Client.GameClient user, uint guildId, bool fromBoard, ServerSockets.Packet stream)
        {
            var p = user.Player;
            Role.Instance.Guild guild;
            if (!Role.Instance.Guild.GuildPoll.TryGetValue(guildId, out guild) || guild == null || guild.Info == null) return false;
            if (p.MyGuild != null || p.MyGuildMember != null)
            {
                RecruitUtil.Msg(user, p.MyGuild == guild ? "You are already a member of this guild!" : "You already have a guild!");
                return false;
            }
            if (guild.Info.RecruitmentOFF)
            {
                RecruitUtil.Msg(user, "This Guild hasn`t started to recruit members.");
                return false;
            }
            if ((long)p.BattlePower < (long)guild.Info.Recruitment_Battle_Power || !guild.Profession(p.Class))
            {
                RecruitUtil.Msg(user, "Failed to apply. You are not eligible.");
                return false;
            }
            bool auto = guild.Info.RecruitmentON;
            if (!auto && fromBoard)
            {
                var ad = RecruitBoard.ActiveAd(guildId);
                if (ad != null && ad.AutoJoin != 0) auto = true;
            }
            if (auto)
            {
                if (fromBoard)
                {
                    try { guild.AddPlayer(p, stream); } catch (Exception e) { RecruitIO.Error(e); }// same call as the MsgSyndicate Join path (checks BP / class again)
                    if (p.MyGuild == guild) RemoveAllFor(p.UID);
                    return false;
                }
                RemoveAllFor(p.UID);
                return true;// let MsgGuildProces Join -> Guild.AddPlayer run
            }

            var cfg = RecruitIO.Config;
            lock (Sync)
            {
                PurgeExpired_NoLock();
                if (Apps.Any(a => a.GuildId == guildId && a.Uid == p.UID))
                {
                    RecruitUtil.Msg(user, "You have already applied to join this guild. Please wait for a reply.");
                    return false;
                }
                if (Apps.Count(a => a.Uid == p.UID) >= cfg.MaxPendingPerPlayer)
                {
                    RecruitUtil.Msg(user, "You have too many guild applications waiting for a reply.");
                    return false;
                }
                if (Apps.Count(a => a.GuildId == guildId) >= cfg.MaxApplicationsPerGuild)
                {
                    RecruitUtil.Msg(user, "This guild has too many applications. Please try again later.");
                    return false;
                }
                Apps.Add(new GuildApplication
                {
                    GuildId = guildId,
                    Uid = p.UID,
                    Name = p.Name ?? "",
                    Level = p.Level,
                    Class = (int)p.Class,
                    BattlePower = p.BattlePower,
                    Reborn = p.Reborn,
                    Time = RecruitIO.Now
                });
                Save();
            }
            RecruitIO.Log("guild-apply guild=" + guildId + " uid=" + p.UID + " name=" + p.Name);
            RecruitUtil.Msg(user, "Your application has been sent to the guild " + guild.GuildName + ".");
            foreach (var m in guild.Members.Values)
            {
                if (m == null || !m.IsOnline) continue;
                if (m.Rank != Role.Flags.GuildMemberRank.GuildLeader && m.Rank != Role.Flags.GuildMemberRank.DeputyLeader && m.Rank != Role.Flags.GuildMemberRank.Manager) continue;
                RecruitUtil.Msg(RecruitUtil.Online(m.UID), p.Name + " applied to join the guild. Check Guild > Applications.");
            }
            return false;
        }

        static void RemoveAllFor(uint uid)
        {
            lock (Sync)
            {
                if (Apps.RemoveAll(a => a.Uid == uid) > 0) Save();
            }
        }

        static GuildApplication Take(uint guildId, uint uid)
        {
            lock (Sync)
            {
                var a = Apps.FirstOrDefault(x => x.GuildId == guildId && x.Uid == uid);
                if (a != null) { Apps.Remove(a); Save(); }
                return a;
            }
        }

        /// <summary>Agree / refuse one applicant. The guild is the officer's own guild (never a client id).</summary>
        public static void Decide(Client.GameClient officer, uint applicantUid, bool agree, ServerSockets.Packet stream)
        {
            if (!RecruitUtil.IsGuildOfficer(officer)) return;
            var guild = officer.Player.MyGuild;
            int done = DecideOne(officer, guild, applicantUid, agree, stream);
            if (done >= 0) SendList(officer, stream);
        }

        /// <summary>-1 = no such application, 0 = kept (applicant offline), 1 = done.</summary>
        static int DecideOne(Client.GameClient officer, Role.Instance.Guild guild, uint applicantUid, bool agree, ServerSockets.Packet stream)
        {
            GuildApplication app;
            lock (Sync)
            {
                PurgeExpired_NoLock();
                app = Apps.FirstOrDefault(x => x.GuildId == guild.Info.GuildID && x.Uid == applicantUid);
            }
            if (app == null) return -1;
            var target = RecruitUtil.Online(applicantUid);
            if (!agree)
            {
                Take(guild.Info.GuildID, applicantUid);
                RecruitIO.Log("guild-refuse guild=" + guild.Info.GuildID + " uid=" + applicantUid + " by=" + officer.Player.UID);
                if (target != null) RecruitUtil.Msg(target, "Your application to join " + guild.GuildName + " was refused.");
                return 1;
            }
            if (target == null)
            {
                RecruitUtil.Msg(officer, app.Name + " is offline. The application is kept until the hero is online.");
                return 0;
            }
            if (target.Player.MyGuild != null || target.Player.MyGuildMember != null)
            {
                RemoveAllFor(applicantUid);
                RecruitUtil.Msg(officer, app.Name + " already joined another guild.");
                return 1;
            }
            try { guild.AddPlayer(target.Player, stream); }// checks BP / class requirement and that the hero is guild-less
            catch (Exception e) { RecruitIO.Error(e); }
            if (target.Player.MyGuild == guild)
            {
                RemoveAllFor(applicantUid);
                RecruitIO.Log("guild-agree guild=" + guild.Info.GuildID + " uid=" + applicantUid + " by=" + officer.Player.UID);
                return 1;
            }
            Take(guild.Info.GuildID, applicantUid);
            RecruitUtil.Msg(officer, app.Name + " does not meet the guild requirements any more.");
            return 1;
        }

        public static void DecideAll(Client.GameClient officer, bool agree, ServerSockets.Packet stream)
        {
            if (!RecruitUtil.IsGuildOfficer(officer)) return;
            var guild = officer.Player.MyGuild;
            foreach (var a in ListFor(guild.Info.GuildID))
                DecideOne(officer, guild, a.Uid, agree, stream);
            SendList(officer, stream);
        }

        // ------------------------------------------------------------------ 2351
        public static SyndicateApplyListPB AnswerList(Client.GameClient c)
        {
            var ans = new SyndicateApplyListPB();
            if (!RecruitUtil.IsGuildOfficer(c)) return ans;// only officers see applicants
            foreach (var a in ListFor(c.Player.MyGuild.Info.GuildID).Take(RecruitIO.Config.MaxApplicationsPerGuild))
                ans.Items.Add(new ApplicationInfoPB { Uid = a.Uid, Name = a.Name ?? "", Level = a.Level, Class = a.Class, BattlePower = a.BattlePower, Reborn = a.Reborn });
            return ans;
        }

        public static ServerSockets.Packet BuildList(ServerSockets.Packet stream, Client.GameClient c)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(AnswerList(c));
            stream.Finalize(IdApplyList);
            return stream;
        }

        static void SendList(Client.GameClient c, ServerSockets.Packet stream)
        {
            try { c.Send(BuildList(stream, c)); } catch (Exception e) { RecruitIO.Error(e); }
        }

        /// <summary>2351 request (empty message) -> the officer's applicant list.</summary>
        public static ServerSockets.Packet ReplyList(Client.GameClient c, ServerSockets.Packet stream)
        {
            return BuildList(stream, c);
        }

        // ------------------------------------------------------------------ 2342 CMsgSynRecruitAdvertisingOpt (c2s, len 0xc)
        // +4 u16 action (client only sends 1 = join from the recruitment board), +8 u32 guild id. Process is a no-op: no reply.
        [PacketAttribute(IdRecruitOpt)]
        private static void OnRecruitOpt(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                if (!RecruitUtil.Ready(user) || !RecruitThrottle.Allow(user, 1)) return;
                if (stream.Size < 12) return;
                stream.Seek(4);
                ushort action = stream.ReadUInt16();
                stream.Seek(8);
                uint guildId = stream.ReadUInt32();
                if (action != 1 || guildId == 0) return;
                Apply(user, guildId, true, stream);
            }
            catch (Exception e) { RecruitIO.Error(e); }
        }
    }
}
