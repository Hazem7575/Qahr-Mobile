// [Poker 2026-10-14] BoYi (casino / "Poker Level") level and EXP - 2234 CMsgBetLevel.
// Client (spec_tourney 3.1): f1=0 info for uid f3 (reply f4 = level, f5 = EXP u64); f1=1 = hero update (level-up
// animation when f4 grows). Level count (249) is official (client BoYiTip.lua); the EXP needed per level and the EXP a
// hand gives are NOT in the client -> DERIVED, config "boyi" in Data\Poker\config.json.
// Saved in the character file ([Poker] BoYi=level|exp) by the same SaveClient hook as the escrow marker.
using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Qahr.Mobile.Poker
{
    public static class PokerBoYi
    {
        public const ushort IdBetLevel = 2234;
        public const int MaxLevel = 249;

        sealed class State { public uint Level = 1; public ulong Exp; }
        static readonly ConcurrentDictionary<uint, State> states = new ConcurrentDictionary<uint, State>();

        static System.Collections.Generic.Dictionary<string, object> Cfg { get { return PokerConfig.Obj(PokerConfig.Current.Raw, "boyi"); } }
        static ulong ExpToNext(uint level)
        {
            double baseExp = PokerConfig.D(Cfg, "expBase", 100), growth = PokerConfig.D(Cfg, "expPerLevel", 50);
            return (ulong)Math.Max(1, baseExp + growth * (level - 1));
        }

        public static uint LevelOf(uint uid) { State s; return states.TryGetValue(uid, out s) ? s.Level : 1; }

        public static void Load(Client.GameClient client, WindowsAPI.IniFile reader)
        {
            if (client == null || client.Player == null || reader == null) return;
            var s = new State();
            var raw = (reader.ReadString("Poker", "BoYi", "") ?? "").Split('|');
            uint lv; ulong ex;
            if (raw.Length >= 2 && uint.TryParse(raw[0], out lv) && ulong.TryParse(raw[1], out ex) && lv >= 1 && lv <= MaxLevel) { s.Level = lv; s.Exp = ex; }
            states[client.Player.UID] = s;
        }

        public static void Save(Client.GameClient client, WindowsAPI.IniFile writer)
        {
            if (client == null || client.Player == null || writer == null) return;
            State s;
            if (!states.TryGetValue(client.Player.UID, out s)) return;    // never loaded: leave the file alone
            lock (s) writer.WriteString("Poker", "BoYi", s.Level.ToString(CultureInfo.InvariantCulture) + "|" + s.Exp.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>Called from the existing 2234 handler (MY System\BetLeveInfoPoker). Returns true when answered.</summary>
        public static bool Handle(Client.GameClient client, ServerSockets.Packet stream)
        {
            try
            {
                if (!PokerModule.Ready(client)) return true;
                var r = PbR.FromPacket(stream);
                uint type = r.U32(1);
                if (type != 0) return true;                                   // only the info query is a client request
                if (!PokerModule.Throttle(client, 200)) return true;
                uint uid = r.U32(3);
                if (uid == 0) uid = client.Player.UID;
                State s;
                if (!states.TryGetValue(uid, out s)) s = new State();
                PokerModule.Send(client, new PbW().U(1, 0).U(2, r.U32(2)).U(3, uid).U(4, s.Level).U(5, uid == client.Player.UID ? s.Exp : 0), IdBetLevel);
                return true;
            }
            catch (Exception e) { PokerLog.Write(client, "2234 error: " + e.Message); return true; }
        }

        /// <summary>EXP for one finished hand (cash or tournament). Sends 2234 f1=1 to the player.</summary>
        public static void OnHandPlayed(Client.GameClient client, bool wonPot)
        {
            if (client == null || client.Player == null) return;
            var s = states.GetOrAdd(client.Player.UID, k => new State());
            ulong gain = (ulong)PokerConfig.D(Cfg, "expPerHand", 10) + (wonPot ? (ulong)PokerConfig.D(Cfg, "expPerWin", 10) : 0);
            uint before;
            lock (s)
            {
                before = s.Level;
                s.Exp += gain;
                while (s.Level < MaxLevel && s.Exp >= ExpToNext(s.Level)) { s.Exp -= ExpToNext(s.Level); s.Level++; }
                if (s.Level >= MaxLevel) s.Exp = 0;
            }
            PokerModule.Send(client, new PbW().U(1, 1).U(2, 0).U(3, client.Player.UID).U(4, s.Level).U(5, s.Exp), IdBetLevel);
            if (s.Level != before) PokerLog.Write(client, "BOYI level " + before + " -> " + s.Level);
        }
    }
}
