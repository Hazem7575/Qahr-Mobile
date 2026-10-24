// [Poker 2026-10-14] Silver <-> table chips escrow. Guarantees that a buy-in is never lost or paid twice, also across
// a server crash or restart:
//
//   * Data\Poker\escrow.txt  (journal, rewritten atomically) - one line per table session:
//         sessionId|uid|tableId|chips|state(O=open,C=closed)|utcTicks
//     "chips" is what the player owns at the table outside the current hand (updated at buy-in, top-up, hand end,
//     leave). A crash in the middle of a hand therefore voids that hand.
//   * Users\<uid>.ini  [Poker] Escrow=<sessionId>  - written by SaveClient in the SAME atomic batch as Money.
//     A marker on disk means "this session's chips were never paid back in this saved state".
//
//   buy-in : journal(open, chips) -> Money -= chips + marker -> SaveClient
//   cash-out: journal(closed, chips) -> Money += chips + marker cleared -> SaveClient
//   login with a marker: Money += journal chips, marker cleared (next save persists both together; a crash before
//   that save leaves the old Money AND the marker, so the refund simply happens again - never twice in one state).
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.Poker
{
    public static class PokerLedger
    {
        public sealed class Entry
        {
            public ulong Session;
            public uint Uid;
            public uint Table;
            public ulong Chips;
            public bool Open;
            public long Ticks;
        }

        const string Section = "Poker", Key = "Escrow";
        static readonly object sync = new object();
        static Dictionary<ulong, Entry> journal;
        /// <summary>uid -> session id that is NOT yet paid back (what SaveToIni writes).</summary>
        static readonly ConcurrentDictionary<uint, ulong> markers = new ConcurrentDictionary<uint, ulong>();
        /// <summary>uid -> session id found in the character file at load (refund pending).</summary>
        static readonly ConcurrentDictionary<uint, ulong> pending = new ConcurrentDictionary<uint, ulong>();

        public static string JournalPath { get { return Path.Combine(PokerConfig.Folder, "escrow.txt"); } }

        static void EnsureLoaded()
        {
            if (journal != null) return;
            var j = new Dictionary<ulong, Entry>();
            try
            {
                if (File.Exists(JournalPath))
                    foreach (string line in File.ReadAllLines(JournalPath, Encoding.UTF8))
                    {
                        var p = line.Split('|');
                        if (p.Length < 6) continue;
                        var e = new Entry();
                        if (!ulong.TryParse(p[0], out e.Session) || !uint.TryParse(p[1], out e.Uid) || !uint.TryParse(p[2], out e.Table)
                            || !ulong.TryParse(p[3], out e.Chips) || !long.TryParse(p[5], out e.Ticks)) continue;
                        e.Open = p[4] == "O";
                        j[e.Session] = e;
                    }
            }
            catch (Exception ex) { PokerLog.Write("escrow journal read error: " + ex.Message); }
            journal = j;
        }

        static void Flush()
        {
            // prune closed sessions older than 30 days (their markers can no longer be on disk after a normal save)
            long cutoff = DateTime.UtcNow.AddDays(-30).Ticks;
            var sb = new StringBuilder();
            var dead = new List<ulong>();
            foreach (var e in journal.Values)
            {
                if (!e.Open && e.Ticks < cutoff) { dead.Add(e.Session); continue; }
                sb.Append(e.Session).Append('|').Append(e.Uid).Append('|').Append(e.Table).Append('|').Append(e.Chips).Append('|')
                  .Append(e.Open ? "O" : "C").Append('|').Append(e.Ticks).Append("\r\n");
            }
            foreach (var d in dead) journal.Remove(d);
            Directory.CreateDirectory(PokerConfig.Folder);
            string tmp = JournalPath + ".tmp";
            File.WriteAllText(tmp, sb.ToString(), Encoding.UTF8);
            if (File.Exists(JournalPath)) File.Replace(tmp, JournalPath, null);
            else File.Move(tmp, JournalPath);
        }

        public static Entry Get(ulong session)
        {
            lock (sync) { EnsureLoaded(); Entry e; return journal.TryGetValue(session, out e) ? e : null; }
        }

        /// <summary>Open (or extend) a session with chips the player owns at the table. Written to disk before silver moves.</summary>
        public static void SetChips(ulong session, uint uid, uint table, ulong chips, bool open)
        {
            lock (sync)
            {
                EnsureLoaded();
                Entry e;
                if (!journal.TryGetValue(session, out e)) journal[session] = e = new Entry { Session = session, Uid = uid, Table = table };
                e.Chips = chips; e.Open = open; e.Ticks = DateTime.UtcNow.Ticks;
                Flush();
            }
        }

        /// <summary>Hand end: many sessions in one atomic write.</summary>
        public static void SetMany(IEnumerable<KeyValuePair<ulong, ulong>> sessionChips)
        {
            lock (sync)
            {
                EnsureLoaded();
                bool any = false;
                foreach (var kv in sessionChips)
                {
                    Entry e;
                    if (journal.TryGetValue(kv.Key, out e) && e.Open) { e.Chips = kv.Value; e.Ticks = DateTime.UtcNow.Ticks; any = true; }
                }
                if (any) Flush();
            }
        }

        public static bool Online(Client.GameClient c)
        {
            if (c == null || c.Player == null || !c.FullLoading) return false;
            if (PokerModule.TestMode) return !PokerModule.TestOffline.Contains(c.Player.UID);
            return c.Socket != null && c.Socket.Alive;
        }

        /// <summary>
        /// Pays a tournament session out (prize / refund; 0 allowed). Online: silver now + marker cleared + save.
        /// Offline: the journal keeps the amount OPEN and the saved marker stays, so the next login pays it once.
        /// </summary>
        public static void Payout(Client.GameClient c, uint uid, ulong session, uint table, ulong amount, string why)
        {
            if (Online(c))
            {
                SetChips(session, uid, table, amount, false);
                if (amount > 0) PokerMoney.Credit(c, amount, why);
                SetMarker(uid, 0);
                PokerMoney.Save(c);
            }
            else
            {
                SetChips(session, uid, table, amount, true);
                pending[uid] = session;
                PokerLog.Write("escrow: " + uid + " offline, " + amount + " (" + why + ") paid at next login, session " + session);
            }
        }

        // ------------------------------------------------------------------ character file marker
        public static void SetMarker(uint uid, ulong session) { if (session == 0) { ulong x; markers.TryRemove(uid, out x); } else markers[uid] = session; }
        public static ulong Marker(uint uid) { ulong s; return markers.TryGetValue(uid, out s) ? s : 0; }

        public static void LoadFromIni(Client.GameClient client, WindowsAPI.IniFile reader)
        {
            try
            {
                if (client == null || client.Player == null || reader == null) return;
                PokerBoYi.Load(client, reader);
                string raw = reader.ReadString(Section, Key, "0");
                ulong sid;
                if (ulong.TryParse((raw ?? "0").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out sid) && sid != 0)
                {
                    pending[client.Player.UID] = sid;
                    markers[client.Player.UID] = sid;   // keep it on disk until the refund is applied
                }
                else { ulong x; markers.TryRemove(client.Player.UID, out x); pending.TryRemove(client.Player.UID, out x); }
            }
            catch (Exception e) { PokerLog.Write("escrow load error: " + e.Message); }
        }

        public static void SaveToIni(Client.GameClient client, WindowsAPI.IniFile writer)
        {
            try
            {
                if (client == null || client.Player == null || writer == null) return;
                writer.WriteString(Section, Key, Marker(client.Player.UID).ToString(CultureInfo.InvariantCulture));
                PokerBoYi.Save(client, writer);
            }
            catch (Exception e) { PokerLog.Write("escrow save error: " + e.Message); }
        }

        /// <summary>Pays back a session left open by a crash/restart. Safe to call any number of times.</summary>
        public static void ApplyPendingRefund(Client.GameClient client)
        {
            if (client == null || client.Player == null) return;
            uint uid = client.Player.UID;
            ulong sid;
            if (!pending.TryRemove(uid, out sid)) return;
            if (PokerModule.IsSeated(uid)) { pending[uid] = sid; return; }   // still playing (should not happen after a restart)
            var e = Get(sid);
            ulong chips = e != null && e.Uid == uid ? e.Chips : 0;
            if (e == null || e.Uid != uid) PokerLog.Write(client, "escrow marker " + sid + " has no journal entry - nothing to refund");
            lock (sync)
            {
                if (chips > 0) PokerMoney.Credit(client, chips, "escrow refund session " + sid);
                SetMarker(uid, 0);
                if (e != null && e.Open) { e.Open = false; e.Ticks = DateTime.UtcNow.Ticks; Flush(); }
            }
            PokerLog.Write(client, "escrow refund session " + sid + " table " + (e != null ? e.Table : 0) + " chips " + chips + " -> silver " + PokerMoney.Balance(client));
            if (chips > 0) PokerModule.Notify(client, "Your poker chips (" + chips.ToString("N0", CultureInfo.InvariantCulture) + ") were returned as silver.");
        }
    }

    /// <summary>All silver moves of the poker system go through here (Player.Money setter logs every change).</summary>
    public static class PokerMoney
    {
        /// <summary>Test hooks (harness only): replace the silver backend / skip character saves.</summary>
        public static Func<Client.GameClient, ulong> GetHook;
        public static Action<Client.GameClient, ulong> SetHook;
        public static Action<Client.GameClient> SaveHook;

        public static ulong Balance(Client.GameClient c) { return GetHook != null ? GetHook(c) : c.Player.Money; }
        static void Set(Client.GameClient c, ulong v) { if (SetHook != null) SetHook(c, v); else c.Player.Money = v; }

        public static bool Debit(Client.GameClient client, ulong amount, string why)
        {
            if (client == null || client.Player == null || amount == 0) return false;
            ulong before = Balance(client);
            if (before < amount) return false;
            Set(client, before - amount);
            PokerLog.Write(client, "silver -" + amount + " (" + why + ") " + before + " -> " + Balance(client));
            return true;
        }

        public static void Credit(Client.GameClient client, ulong amount, string why)
        {
            if (client == null || client.Player == null || amount == 0) return;
            ulong before = Balance(client);
            Set(client, before + amount);   // the Money setter moves anything above the bag limit to the warehouse
            PokerLog.Write(client, "silver +" + amount + " (" + why + ") " + before + " -> " + Balance(client));
        }

        public static void Save(Client.GameClient client)
        {
            if (SaveHook != null) { SaveHook(client); return; }
            try { if (client != null && client.Player != null && client.FullLoading) Database.ServerDatabase.SaveClient(client); }
            catch (Exception e) { PokerLog.Write("save error: " + e.Message); }
        }
    }

    public static class PokerLog
    {
        static readonly object sync = new object();
        public static void Write(Client.GameClient c, string text)
        {
            string who = c != null && c.Player != null ? c.Player.Name + "(" + c.Player.UID + ")" : "?";
            Write(who + " " + text);
        }
        public static void Write(string text)
        {
            try
            {
                string dir = Path.Combine(PokerConfig.Folder, "logs");
                lock (sync)
                {
                    Directory.CreateDirectory(dir);
                    File.AppendAllText(Path.Combine(dir, "poker_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"),
                        DateTime.Now.ToString("HH:mm:ss.fff") + " " + text + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }
    }
}
