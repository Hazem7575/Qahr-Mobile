// [Unanswered B4 2026-10-14] 2001 CMsgGuideInfo (legacy struct, client -> server request).
// Client builder 0x1019dc9: len 0x60, +4 u8 mode (1 = my mentor, 2 = my apprentices), +8 u32 uid (0 from every caller seen).
// Reply = the existing MsgApprenticeInformation struct (same fields as Associate.OnLoading sends at login).
// Read-only: only the caller's own mentor / apprentices are answered (no info leak), no battle-power or
// OnlineApprentice side effects (those stay in the login / accept paths).
using System;
using System.Linq;
using Qahr.Mobile.Game;
using Qahr.Mobile.Game.MsgServer;
using Assoc = Qahr.Mobile.Role.Instance.Associate;

namespace Qahr.Mobile.Unanswered.B4
{
    public static class B4GuideInfo
    {
        public const byte ModeMentor = 1, ModeApprentice = 2;
        public const int MaxApprenticeReplies = 20;

        [PacketAttribute(GamePackets.MsgGuideInfo)]
        public static void Handle(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!B4Util.Ready(client) || client.Player.Associate == null) return;
                if (stream.Size < 12) return;
                if (!B4Throttle.Allow(client, B4Throttle.Slot2001)) return;
                stream.Seek(4);
                byte mode = stream.ReadUInt8();
                stream.Seek(8);
                uint uid = stream.ReadUInt32(); // optional filter; must be the caller's own mentor / apprentice

                var me = client.Player;
                var assoc = me.Associate;
                if (mode == ModeMentor)
                {
                    System.Collections.Concurrent.ConcurrentDictionary<uint, Assoc.Member> list;
                    if (!assoc.Associat.TryGetValue(Assoc.Mentor, out list) || list == null) return;
                    foreach (var mem in list.Values.Take(1))
                    {
                        if (uid != 0 && mem.UID != uid) return;
                        B4Util.Send(client, BuildMentor(stream, client, mem));
                    }
                }
                else if (mode == ModeApprentice)
                {
                    System.Collections.Concurrent.ConcurrentDictionary<uint, Assoc.Member> list;
                    if (!assoc.Associat.TryGetValue(Assoc.Apprentice, out list) || list == null) return;
                    foreach (var mem in list.Values.Take(MaxApprenticeReplies))
                    {
                        if (uid != 0 && mem.UID != uid) continue;
                        B4Util.Send(client, BuildApprentice(stream, client, mem));
                    }
                }
            }
            catch (Exception e) { B4Util.Log(e); }
        }

        /// <summary>Mode 1: info about the caller's mentor (sent to the caller = apprentice).</summary>
        public static ServerSockets.Packet BuildMentor(ServerSockets.Packet stream, Client.GameClient client, Assoc.Member mem)
        {
            var me = client.Player;
            var info = MsgApprenticeInformation.Create();
            info.Mode = MsgApprenticeInformation.Action.Mentor;
            info.Mentor_ID = mem.UID;
            info.Apprentice_ID = me.UID;
            info.Enrole_date = (uint)mem.Timer;
            Client.GameClient other;
            if (Pool.GamePoll.TryGetValue(mem.UID, out other) && other != null && other.Player != null)
            {
                var t = other.Player;
                info.Level = (byte)t.Level;
                info.Class = t.Class;
                info.PkPoints = t.PKPoints;
                info.Mesh = t.Mesh;
                info.Online = 1;
                info.Shared_Battle_Power = t.GetShareBattlePowers((uint)me.RealBattlePower);
                info.WriteString(t.Name, me.Spouse, me.Name);
            }
            else
            {
                info.Online = 0;
                info.WriteString(mem.Name, "", "");
            }
            return info.GetArray(stream);
        }

        /// <summary>Mode 2: info about one of the caller's apprentices (sent to the caller = mentor).</summary>
        public static ServerSockets.Packet BuildApprentice(ServerSockets.Packet stream, Client.GameClient client, Assoc.Member mem)
        {
            var me = client.Player;
            var info = MsgApprenticeInformation.Create();
            info.Mode = MsgApprenticeInformation.Action.Apprentice;
            info.Mentor_ID = me.UID;
            info.Apprentice_ID = mem.UID;
            info.Enrole_date = (uint)mem.Timer;
            info.Apprentice_Blessing = (ushort)mem.Blessing;
            info.Apprentice_Composing = (ushort)mem.Stone;
            info.Apprentice_Experience = (ushort)mem.ExpBalls; // same width as Associate.OnLoading
            Client.GameClient other;
            if (Pool.GamePoll.TryGetValue(mem.UID, out other) && other != null && other.Player != null)
            {
                var t = other.Player;
                info.Level = (byte)t.Level;
                info.Class = t.Class;
                info.PkPoints = t.PKPoints;
                info.Mesh = t.Mesh;
                info.Online = 1;
                info.WriteString(me.Name, t.Spouse, t.Name);
            }
            else
            {
                info.Online = 0;
                info.WriteString(me.Name, "", mem.Name);
            }
            return info.GetArray(stream);
        }
    }
}
