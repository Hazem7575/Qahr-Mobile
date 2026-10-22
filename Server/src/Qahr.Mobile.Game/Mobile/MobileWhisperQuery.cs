using System.Linq;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// MsgName (2272) type 29 = the mobile's player "face" query (CMsgMBName 10036, type 0x1d). The client sends it with
    /// the target's name (data u32 @4 = 0) when a player is tapped on the map (CMyShellDlg::OnTap) or an avatar is tapped
    /// in the whisper window (CFDlgPrivateChat::OnBtnAvatar); the answer fills PLAYER_FACE_INFO and opens the player menu
    /// (CDlgPlayerFaceMenu: info, chat, trade, team, friend, guild invite...). The PC client has no such query.
    ///
    /// Answer (CMsgMBName::Process case 29): data @4 echoed (0x65 = ignore, 0x66 = PostCmd 0xbb9, 0x67 = leave a message,
    /// other = PostCmd 0xf23), strings [0] name, [1] 16-17 space separated tokens read with
    /// "%u %u %u %u %d %u %d %u %u %u %s %s %lu %d %d %d %d":
    ///   1 uid (+0)                 2 level (+0x44, menu "Lv")      3 lookface (+0x10 = lookface/10000 % 1000, avatar)
    ///   4 not read                 5 +0x48 = GetProfessionalName(i, j) i (1 = 2nd name column)   6 +0x4c = j, the profession id (professionalname.ini)
    ///   7 +0x08 (0 = take the guild name from the player in view) 8 max life (CPlayer vfunc 0xd0)
    ///   9 life (CPlayer::SetData(1, v))  10 +0x04               11 guild name (+0x38)   12 name (+0x2c)
    ///   13 face frame (+0x24, SelfStyle_GetFrameInfo)  14 +0x18  15 battle power (+0x0c, "BATTLE_LEVEL")
    ///   16 +0x28                   17 return level (+0x1c, CPlayer::SetPlayerReturnLevel)
    /// No tokens (string [1] empty) = the client's "player is offline / not found" message.
    /// </summary>
    public static class MobileWhisperQuery
    {
        public const byte QueryFace = 29;

        public static void Handle(Client.GameClient user, ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket str)
        {
            if (user == null || user.Player == null || str.Strings == null || str.Strings.Length == 0)
                return;
            string name = str.Strings[0] ?? "";
            Client.GameClient target = null;
            foreach (var c in Pool.GamePoll.Values)
            {
                if (c != null && c.Player != null && c.Player.Name == name) { target = c; break; }
            }

            // The client also sends this query when an NPC drawn with a player look (MsgPlayer spawn, e.g. Suyi,
            // MagicArtisan) is tapped. An empty answer makes it toast "The player is offline." on every such NPC click,
            // so answer only for real character names (online or offline).
            if (target == null)
            {
                bool known;
                lock (Pool.NameUsed) known = Pool.NameUsed.Contains(name.GetHashCode());
                if (!known) return;
            }

            string info = "";
            if (target != null)
            {
                var p = target.Player;
                string guild = p.MyGuild != null ? Token(p.MyGuild.GuildName) : "-";
                uint maxLife = target.Status != null ? (uint)target.Status.MaxHitpoints : (uint)p.HitPoints;
                info = string.Join(" ", new string[]
                {
                    p.UID.ToString(),                 // 1 uid
                    p.Level.ToString(),               // 2 level
                    p.Mesh.ToString(),                // 3 lookface
                    "0",                              // 4 not read
                    "0",                              // 5 name column flag (CDlgPlayerFaceMenu::OnOpenWindow passes +0x48, +0x4c)
                    p.Class.ToString(),               // 6 profession id ("Class:" line; was "?" with the class in token 5)
                    p.GuildID.ToString(),             // 7
                    maxLife.ToString(),               // 8 max life
                    ((uint)System.Math.Max(0, p.HitPoints)).ToString(), // 9 life
                    ((byte)p.NobilityRank).ToString(),// 10
                    guild,                            // 11 guild name
                    Token(p.Name),                    // 12 name
                    p.FrameID.ToString(),             // 13 face frame
                    p.VipLevel.ToString(),            // 14
                    p.BattlePower.ToString(),         // 15 battle power
                    "0",                              // 16
                    "0"                               // 17 return level
                });
                // No guild: the 16-token form ("%u %u %u %u %d %u %d %u %u %u %s %lu %d %d %d %d", no guild token) leaves
                // the guild name empty. A placeholder like "-" counts as a guild: the player menu then refuses the guild
                // invite (CDlgPlayerFaceMenu::OnBtnClickInviteFaction -> STR_FACTION_PLAYER_HAVE_SYN).
                if (p.MyGuild == null)
                    info = info.Replace(" - " + Token(p.Name) + " ", " " + Token(p.Name) + " ");
            }

            var reply = new Game.MsgServer.MsgStringPacket
            {
                ID = (Game.MsgServer.MsgStringPacket.StringID)QueryFace,
                UID = str.UID,                        // the data @4 is echoed
                Strings = new[] { name, info }
            };
            user.Send(stream.StringPacketCreate(reply));
        }

        // %s stops at a space and an empty token would change the token count
        static string Token(string s)
        {
            if (string.IsNullOrEmpty(s)) return "-";
            return new string(s.Select(ch => ch == ' ' ? '_' : ch).ToArray());
        }
    }
}
