// ============================================================================
//  [CastleMystery 2026-10-15] Valentine 2025 "Castle Startle" (Lover2025Kua) - the Twin City part, the Castle Key
//  treasures, the event boxes, and the item dialogs of the Valentine group (also Valentine2023's Dream Note).
//  Official sources (client luacfg / Server_Key, Dec 2024 - Feb 2025):
//    Suyi [CastleStartle] (NpcX 110010, tLover2025Kua_DelText[33978]): story, "Head to Auter Castle", "Obtain castle
//        treasures", "Check ranking rewards", "Visit Love Garden"; CP shop EmoneySaleShop[33978]: Castle Key 3351993 20 CPs,
//        Fine Castle Key Box 3351995 199 CPs (max 20), Grand Castle Key Box 3351996 999 CPs (max 5), Feb. 11 - Feb. 24.
//    Jet [CastleStore] (NpcX 110011, EmoneySaleShop[33979], Chocolate Candy 3352015) = rewards.json exchanges (FestivalManager).
//    Lord Anthony (npc.ini 10029, tLover2025Kua_DelText[34030]): the stories; after a chat he gives a key ([Sys][ADNTip]).
//    Item texts: Castle Key "Use to enter the castle and obtain treasures"; Fine box = 10 + 1 keys; Grand box = 50 + 5 keys;
//        Romantic Parasol / Flute / String Box = choose one of the named items; Romantic Pet Box = the 3 pets of Suyi's text.
//  DERIVED (Data\CastleMystery\castle.json): the treasure table of a Castle Key (Chocolate Candy amounts + odds,
//  Castle Points per key), Anthony's key once a day, NPC spots in Twin City. Not here: Love Garden / rankings (cross-server
//  "Kua" parts), the Castle Key draw window (no 2025 DlgID data in this client).
//  Every grant: bag space first, cost taken first, state recorded before the give, per-player throttle.
//
//  Hooks (one line each): Game\MsgNpc\Procesor.cs OnNpc + OnItemDialog, MY System\MsgItem\MsgItemUsuagePacket.cs OnUse.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using SE = Qahr.Mobile.FestivalStory.StoryEntry;

namespace Qahr.Mobile.CastleMystery
{
    public static class CastleStartle
    {
        public const string Festival = "CastleMystery";
        public const uint Suyi = 8871011, Jet = 8871012, Anthony = 8871013;
        public const uint CastleKey = 3351993, FineBox = 3351995, GrandBox = 3351996, Candy = 3352015;
        public const uint ParasolBox = 3351997, FluteBox = 3351998, StringBox = 3351999, PetBox = 3352000;
        public const int MinLevel = 80;

        public static Func<DateTime> ClockOverride;
        public static Func<bool> RunningOverride;
        public static Func<int, int> RandomOverride;
        static DateTime Now { get { var f = ClockOverride; return f != null ? f() : DateTime.Now; } }
        public static bool Running { get { var o = RunningOverride; if (o != null) return o(); try { return Festivals.FestivalManager.IsRunning(Festival); } catch { return false; } } }

        // ------------------------------------------------------------------ config
        public sealed class Draw { public int Candy, Weight; }
        public sealed class Config
        {
            public uint KeyCps = 20, FineCps = 199, GrandCps = 999; public int FineMax = 20, GrandMax = 5;
            public int FineKeys = 11, GrandKeys = 55, PointsPerKey = 10, DailyKeys = 1;
            public List<Draw> Table = new List<Draw> { new Draw { Candy = 20, Weight = 600 }, new Draw { Candy = 50, Weight = 250 }, new Draw { Candy = 100, Weight = 100 }, new Draw { Candy = 300, Weight = 40 }, new Draw { Candy = 1000, Weight = 10 } };
        }
        static Config _cfg; static string _stamp;
        static string DataDir { get { return Path.Combine(global::Qahr.Mobile.Festivals.FestivalLoader.Root, "CastleMystery"); } }
        public static Config Cfg
        {
            get
            {
                string f = Path.Combine(DataDir, "castle.json");
                try
                {
                    var fi = new FileInfo(f);
                    string st = fi.Exists ? fi.Length + ":" + fi.LastWriteTimeUtc.Ticks : "none";
                    if (_cfg != null && st == _stamp) return _cfg;
                    var c = new Config();
                    if (fi.Exists)
                    {
                        var d = Festivals.FestivalLoader.ReadJson(f);
                        Func<string, uint, uint> U = (k, def) => { var v = Festivals.FestivalLoader.Val(d, k); return v == null ? def : Festivals.FestivalLoader.U(v); };
                        c.KeyCps = Math.Max(1, U("keyCps", c.KeyCps)); c.FineCps = Math.Max(1, U("fineBoxCps", c.FineCps)); c.GrandCps = Math.Max(1, U("grandBoxCps", c.GrandCps));
                        c.FineMax = (int)Math.Min(1000, U("fineBoxMax", (uint)c.FineMax)); c.GrandMax = (int)Math.Min(1000, U("grandBoxMax", (uint)c.GrandMax));
                        c.FineKeys = (int)Math.Min(1000, U("fineBoxKeys", (uint)c.FineKeys)); c.GrandKeys = (int)Math.Min(1000, U("grandBoxKeys", (uint)c.GrandKeys));
                        c.PointsPerKey = (int)Math.Min(100000, U("castlePointsPerKey", (uint)c.PointsPerKey)); c.DailyKeys = (int)Math.Min(100, U("anthonyDailyKeys", (uint)c.DailyKeys));
                        var t = Festivals.FestivalLoader.Val(d, "keyTreasure") as List<object>;
                        if (t != null)
                        {
                            var list = new List<Draw>();
                            foreach (var o in t)
                            {
                                var e = o as Dictionary<string, object>; if (e == null) continue;
                                int candy = (int)Math.Min(100000, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(e, "candy")));
                                int w = (int)Math.Min(1000000, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(e, "weight")));
                                if (candy > 0 && w > 0) list.Add(new Draw { Candy = candy, Weight = w });
                            }
                            if (list.Count > 0) c.Table = list;
                        }
                    }
                    _cfg = c; _stamp = st;
                }
                catch (Exception e) { try { MyConsole.WriteLine("[CastleMystery] castle.json: " + e.Message, ConsoleColor.Yellow); } catch { } if (_cfg == null) _cfg = new Config(); }
                return _cfg;
            }
        }

        // ------------------------------------------------------------------ state
        public sealed class PState { public uint Uid; public int Season, Fine, Grand, KeyDay, Points; }
        static readonly object _lock = new object();
        static Dictionary<uint, PState> _players;
        static string StateFile { get { return Path.Combine(DataDir, "state.json"); } }
        static int Season() { var r = Festivals.FestivalManager.Running(Festival); return (r != null ? r.Started : Now).Year; }
        public static PState State(Client.GameClient c)
        {
            lock (_lock)
            {
                if (_players == null) Load();
                int s = Season();
                PState p;
                if (!_players.TryGetValue(c.Player.UID, out p) || p.Season != s) { p = new PState { Uid = c.Player.UID, Season = s }; _players[p.Uid] = p; }
                return p;
            }
        }
        static void Load()
        {
            _players = new Dictionary<uint, PState>();
            try
            {
                if (!File.Exists(StateFile)) return;
                var list = Festivals.FestivalLoader.Val(Festivals.FestivalLoader.ReadJson(StateFile), "players") as List<object>;
                if (list == null) return;
                foreach (var o in list)
                {
                    var e = o as Dictionary<string, object>; if (e == null) continue;
                    Func<string, int> I = k => (int)Math.Min(int.MaxValue, Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(e, k)));
                    var p = new PState { Uid = Festivals.FestivalLoader.U(Festivals.FestivalLoader.Val(e, "uid")), Season = I("season"), Fine = I("fine"), Grand = I("grand"), KeyDay = I("keyDay"), Points = I("points") };
                    if (p.Uid != 0) _players[p.Uid] = p;
                }
            }
            catch (Exception ex) { try { MyConsole.WriteLine("[CastleMystery] state.json: " + ex.Message, ConsoleColor.Yellow); } catch { } }
        }
        public static void Save()
        {
            lock (_lock)
            {
                if (_players == null) return;
                try
                {
                    var sb = new StringBuilder("{\"_readme\":\"Written by the GameServer [CastleMystery] Castle Startle. Do not edit while the server runs.\",\"players\":[");
                    bool first = true;
                    foreach (var p in _players.Values)
                    {
                        if (!first) sb.Append(','); first = false;
                        sb.Append("{\"uid\":").Append(p.Uid).Append(",\"season\":").Append(p.Season).Append(",\"fine\":").Append(p.Fine).Append(",\"grand\":").Append(p.Grand)
                          .Append(",\"keyDay\":").Append(p.KeyDay).Append(",\"points\":").Append(p.Points).Append('}');
                    }
                    sb.Append("]}");
                    Directory.CreateDirectory(DataDir);
                    string f = StateFile, tmp = f + ".tmp";
                    File.WriteAllText(tmp, sb.ToString(), new UTF8Encoding(false));
                    if (File.Exists(f)) File.Replace(tmp, f, null); else File.Move(tmp, f);
                }
                catch (Exception e) { try { MyConsole.WriteLine("[CastleMystery] save failed: " + e.Message, ConsoleColor.Yellow); } catch { } }
            }
        }

        // ------------------------------------------------------------------ helpers
        static readonly Random _rnd = new Random();
        static int Rnd(int n) { var o = RandomOverride; if (o != null) return Math.Max(0, Math.Min(n - 1, o(n))); lock (_rnd) return _rnd.Next(n); }
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now; return false;
        }
        static void Msg(Client.GameClient c, string t) { SE.Msg(c, t); }
        static void Log(Client.GameClient c, string what) { try { Festivals.FestivalManager.Log("CASTLE", Festival, c.Player.UID + " " + c.Player.Name + " | " + what); } catch { } }
        public static uint Count(Client.GameClient c, uint id)
        {
            uint n = 0;
            try { foreach (var it in c.Inventory.ClientItems.Values.ToArray()) if (it.ITEM_ID == id) n += it.StackSize > 1 ? it.StackSize : 1u; } catch { }
            return n;
        }
        public static bool Take(Client.GameClient c, uint id, uint count, ServerSockets.Packet stream)
        {
            if (count == 0) return true;
            if (Count(c, id) < count) return false;
            uint left = count;
            foreach (var item in c.Inventory.ClientItems.Values.Where(i => i.ITEM_ID == id).ToArray())
            {
                if (left == 0) break;
                uint stack = item.StackSize > 1 ? item.StackSize : 1u;
                if (stack > left) { item.StackSize = (ushort)(stack - left); item.Mode = Role.Flags.ItemMode.Update; item.Send(c, stream); left = 0; }
                else { left -= stack; item.StackSize = 1; c.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream); }
            }
            return true;
        }
        public static int Slots(uint id, uint count)
        {
            Database.ItemType.DBItem db;
            if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(id, out db)) return (int)count;
            return db.StackSize > 1 ? (int)((count + db.StackSize - 1) / db.StackSize) : (int)count;
        }
        public static bool Space(Client.GameClient c, int slots) { try { return slots <= 0 || c.Inventory.HaveSpace((byte)Math.Min(40, slots)); } catch { return false; } }
        public static bool Give(Client.GameClient c, uint id, uint count, bool bound, ServerSockets.Packet stream)
        {
            if (Pool.ItemsBase == null || !Pool.ItemsBase.ContainsKey(id)) return false;
            uint left = count;
            while (left > 0)
            {
                ushort n = (ushort)Math.Min(left, 1000u);
                if (!c.Inventory.AddItemWitchStack(id, 0, n, stream, bound)) return false;
                left -= n;
            }
            return true;
        }
        static string Name(uint id) { try { return QuestEngine.QuestGrammar.ItemName(id); } catch { return id.ToString(); } }
        static void Text(Game.MsgNpc.Dialog d, string text)
        {
            text = (text ?? "").Trim();
            while (text.Length > 0)
            {
                int n = Math.Min(text.Length, 200);
                if (n < text.Length) { int sp = text.LastIndexOf(' ', n - 1, n); if (sp > 100) n = sp + 1; }
                d.AddText(text.Substring(0, n)); text = text.Substring(n);
            }
        }
        static void Box(Client.GameClient c, ServerSockets.Packet stream, string text) { var d = new Game.MsgNpc.Dialog(c, stream); Text(d, text); d.AddOption("OK.", 255).AddAvatar(0).FinalizeDialog(); }

        // dialog state: only options this module offered are accepted
        sealed class Dlg { public uint Npc; public readonly HashSet<byte> Opts = new HashSet<byte>(); public DateTime Expires; }
        static readonly ConcurrentDictionary<uint, Dlg> _dlg = new ConcurrentDictionary<uint, Dlg>();
        sealed class DB { readonly Game.MsgNpc.Dialog d; readonly Dlg st; public DB(Client.GameClient c, ServerSockets.Packet s, uint npc) { d = new Game.MsgNpc.Dialog(c, s); st = new Dlg { Npc = npc, Expires = DateTime.UtcNow.AddMinutes(5) }; _dlg[c.Player.UID] = st; }
            public DB T(string t) { Text(d, t); return this; }
            public DB O(string t, byte id) { st.Opts.Add(id); d.AddOption(t.Length > 60 ? t.Substring(0, 57) + "..." : t, id); return this; }
            public void End(ushort avatar = 10) { d.AddOption("Goodbye.", 255).AddAvatar(avatar).FinalizeDialog(); } }
        static bool Offered(Client.GameClient c, uint npc, byte option)
        {
            Dlg st;
            if (!_dlg.TryGetValue(c.Player.UID, out st) || st.Npc != npc || DateTime.UtcNow > st.Expires || !st.Opts.Contains(option)) return false;
            _dlg.TryRemove(c.Player.UID, out st);
            return true;
        }

        // ------------------------------------------------------------------ NPC hook (Procesor)
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null || (npcid != Suyi && npcid != Anthony)) return false;
            try
            {
                if (option == 255) return true;
                if (option != 0 && !Offered(c, npcid, option)) return true;
                if (Throttled(c.Player.UID, 300)) return true;
                if (npcid == Suyi) SuyiDialog(c, stream, option); else AnthonyDialog(c, stream, option);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }

        const byte OCastle = 1, OTreasure = 2, OShop = 3, ORank = 4, OGarden = 5, OBack = 9;
        const byte OUse1 = 21, OUse10 = 22, OKey1 = 31, OKey10 = 32, OFine = 33, OGrand = 34;
        static void SuyiDialog(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (!Running)
            {
                new DB(c, stream, Suyi).T("Suyi:\nAre you here for the Valentine`s Day event at Auter Castle? Sorry, Auter Castle has been closed and doesn`t accept visitors.").End();
                return;
            }
            var cfg = Cfg;
            switch (option)
            {
                case 0:
                case OBack:
                    {
                        var db = new DB(c, stream, Suyi).T("Suyi:\nOn the eve of Valentine`s Day, the century-old castle reappeared, yet couples disappeared there. What lies behind all this? Uncover the truth yourself!\n"
                            + "[Castle Mystery] Entering the castle to explore the mysterious disappearance of couples.\n"
                            + "By participating in the above activities, you can obtain Castle Keys, which can be used to gain treasures and accumulate Castle Points for rankings and rewards.");
                        if (c.Player.Level < MinLevel) { db.T("Requirement: Level 80").O("Come back after leveling up.", 255).End(); return; }
                        db.O("Head to Auter Castle.", OCastle).O("Obtain castle treasures.", OTreasure).O("Buy Castle Keys.", OShop).O("Check ranking rewards.", ORank).O("Visit Love Garden.", OGarden).End();
                        return;
                    }
                case OCastle:
                    SE.Enter(c, CastleMysteryUi.Chain, MinLevel, "Please return to Twin City before entering Auter Castle.", "You have uncovered the secrets of Auter Castle. Thank you!");
                    return;
                case OTreasure: TreasureMenu(c, stream, Suyi); return;
                case OUse1: UseKeys(c, stream, 1); return;
                case OUse10: UseKeys(c, stream, 10); return;
                case OShop:
                    {
                        var p = State(c);
                        new DB(c, stream, Suyi).T("Suyi:\nCastle Keys are on sale while the castle is open. You have " + c.Player.ConquerPoints + " CPs.")
                            .O("1 Castle Key (" + cfg.KeyCps + " CPs).", OKey1).O("10 Castle Keys (" + cfg.KeyCps * 10 + " CPs).", OKey10)
                            .O("Fine Castle Key Box (" + cfg.FineCps + " CPs, " + p.Fine + "/" + cfg.FineMax + ").", OFine)
                            .O("Grand Castle Key Box (" + cfg.GrandCps + " CPs, " + p.Grand + "/" + cfg.GrandMax + ").", OGrand).O("Back.", OBack).End();
                        return;
                    }
                case OKey1: Box(c, stream, Buy(c, CastleKey, 1, cfg.KeyCps, 0)); return;
                case OKey10: Box(c, stream, Buy(c, CastleKey, 10, cfg.KeyCps * 10, 0)); return;
                case OFine: Box(c, stream, Buy(c, FineBox, 1, cfg.FineCps, 1)); return;
                case OGrand: Box(c, stream, Buy(c, GrandBox, 1, cfg.GrandCps, 2)); return;
                case ORank:
                    Box(c, stream, "Your Castle Points: " + State(c).Points + ". The Valentine`s Day ranking is a cross-server ranking, so it isn`t held on this server.");
                    return;
                case OGarden:
                    Box(c, stream, "Love Garden is a cross-server land, so it can`t be visited from this server.");
                    return;
            }
        }
        /// <summary>CP shop (official EmoneySaleShop[33978]). kind 1 / 2 = Fine / Grand box (per-character limits).</summary>
        public static string Buy(Client.GameClient c, uint item, uint count, uint cps, int kind)
        {
            if (!Running) return "Auter Castle has been closed.";
            if (c.Player.Level < MinLevel) return "Your level is below 80. Please return once you`ve leveled up!";
            var cfg = Cfg;
            var p = State(c);
            lock (_lock)
            {
                if (kind == 1 && p.Fine >= cfg.FineMax) return "You have bought the most Fine Castle Key Boxes for this event.";
                if (kind == 2 && p.Grand >= cfg.GrandMax) return "You have bought the most Grand Castle Key Boxes for this event.";
            }
            if (!Space(c, Slots(item, count))) return "Please make some spaces in your inventory first.";
            if (c.Player.ConquerPoints < cps) return "Sorry, you don`t have enough CPs.";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                lock (_lock)
                {
                    if ((kind == 1 && p.Fine >= cfg.FineMax) || (kind == 2 && p.Grand >= cfg.GrandMax) || c.Player.ConquerPoints < cps) return "Sorry, you can`t buy it now.";
                    c.Player.ConquerPoints -= cps;
                    if (kind == 1) p.Fine++; else if (kind == 2) p.Grand++;
                }
                Save();
                bool ok = Give(c, item, count, false, rec.GetStream());
                Log(c, "BUY " + Name(item) + " x" + count + " for " + cps + " CPs" + (ok ? "" : " (GIVE FAILED)"));
                return ok ? "You bought " + Name(item) + (count > 1 ? " x" + count : "") + " for " + cps + " CPs." : "Something went wrong. Please contact a GM.";
            }
        }

        static void TreasureMenu(Client.GameClient c, ServerSockets.Packet stream, uint npc)
        {
            uint keys = Count(c, CastleKey);
            var db = new DB(c, stream, npc).T("The castle`s treasure chamber opens with a Castle Key. You have " + keys + " Castle Key(s) and " + State(c).Points + " Castle Points.");
            if (keys >= 1) db.O("Use 1 Castle Key.", OUse1);
            if (keys >= 10) db.O("Use 10 Castle Keys.", OUse10);
            db.End();
        }
        /// <summary>Castle Key treasures (DERIVED table, castle.json): Chocolate Candy + Castle Points per key.</summary>
        public static string UseKeys(Client.GameClient c, ServerSockets.Packet stream, int n)
        {
            if (!Running) { Msg(c, "Auter Castle has been closed."); return "closed"; }
            if (c.Player.Level < MinLevel) { Msg(c, "Your level is below 80. Please return once you`ve leveled up!"); return "level"; }
            if (n < 1 || n > 10) return "bad";
            if (Count(c, CastleKey) < (uint)n) { Msg(c, "You lack a Castle Key and therefore cannot enter the treasure chamber to claim the treasure."); return "nokey"; }// official NoDrawItem
            var cfg = Cfg;
            int total = 0, sum = cfg.Table.Sum(t => t.Weight);
            for (int i = 0; i < n; i++)
            {
                int r = Rnd(sum);
                foreach (var t in cfg.Table) { if (r < t.Weight) { total += t.Candy; break; } r -= t.Weight; }
            }
            if (!Space(c, Slots(Candy, (uint)total) + 1)) { Msg(c, "Please make some spaces in your inventory first."); return "space"; }
            var p = State(c);
            lock (_lock)
            {
                if (!Take(c, CastleKey, (uint)n, stream)) { Msg(c, "You lack a Castle Key and therefore cannot enter the treasure chamber to claim the treasure."); return "nokey"; }
                p.Points += n * cfg.PointsPerKey;
            }
            Save();
            bool ok = Give(c, Candy, (uint)total, false, stream);
            Msg(c, "You opened the treasure chamber with " + n + " Castle Key(s) and got " + total + " Chocolate Candy and " + n * cfg.PointsPerKey + " Castle Points!");
            Log(c, "KEYS " + n + " -> candy " + total + (ok ? "" : " (GIVE FAILED)") + ", points " + p.Points);
            return "candy " + total;
        }

        // ---- Lord Anthony: the stories (official texts) + a key once a day ([Sys][ADNTip])
        static readonly string[][] Stories = {
            new[] { "Tell me about Jeanne.", "You want to know about Jeanne? Hahaha, let me think...", "Shouldn`t you know her well?", "No, I often feel that I didn`t understand her enough. She always surprised me. She was strong yet timid, optimistic but prone to tears. So contradictory, yet so lovable, right?", "I find it very true-to-life." },
            new[] { "Tell me about yourself.", "Putting aside my title, I`m actually quite ordinary, but truthfully, there`s not much to boast about with my status either.", "Being the heir of the Auter family is impressive.", "But still, I messed everything up. From birth, I was destined to shoulder the responsibility of the family, seemingly my only mission. I`d rather just be an ordinary person.", "That must have been tough." },
            new[] { "Tell me about how you met.", "It was a sunny afternoon when she first came to the castle, and we met in the garden.", "You look quite nostalgic.", "Don`t tease me. I can get shy too. That day she looked flustered yet fierce, like a little cat with claws out.", "So adorable!" },
            new[] { "Tell me about the family curse.", "It was the Dark King`s curse, its punishment for our family, for my betrayal.", "What kind of curse?", "All members of the Auter family turned into phantoms, and Auter Castle was sealed forever. We were doomed to live in darkness and terror, as you see it.", "That sounds terrifying." },
            new[] { "Tell me about this castle.", "I`ve lived in this castle since birth, honestly, it gets pretty boring.", "But it`s huge, though.", "Indeed, it`s vast. Besides this floor, there are treasure rooms upstairs and enormous cellars and storages underground. But have you seen the castle at midnight? Empty and eerily quiet...", "Stop! Scary!" },
            new[] { "Tell me about the Auter family.", "The Auter family has lasted for over two hundred years. The history I know comes from elders because I never enjoyed reading family chronicles.", "Your family seems powerful!", "It`s not as great as you think. There were many dark deals, parts of history I wished not to know.", "Big families have their skeletons." },
            new[] { "Tell me about the Dark King.", "Why would you want to learn about this terrifying entity? It wields dark power and is thoroughly evil.", "Like what?", "It usually tempts people into signing pacts, and those who betray the pact receive its wicked curse.", "That`s despicable!" },
            new[] { "Tell me about your love.", "How should I describe it? Let me think... Our love was like morning dew or afternoon sunshine...", "Sounds abstract.", "In short, it was pure, simple, warm, bright, and filled me with endless happiness..", "I see." },
            new[] { "Tell me about the castle`s cellar.", "You haven`t been to the castle`s cellar, have you? It`s the largest area in the castle.", "What`s the cellar for?", "The cellar stores daily necessities and rare treasures for the castle. If you need anything while in the castle, just go to the cellar.", "Got it!" },
            new[] { "Tell me about the secret chamber.", "You`ve asked the right person. The secret chamber was built by me.", "Wow, tell me more!", "I didn`t want to keep my personal collections in the treasure room, so I built this chamber for my private items. I`ll grant you access anytime. Feel free to choose what you like. I`m delighted to give them to you.", "Wow, thank you so much!" },
            new[] { "Tell me about the treasure room.", "All people have a thing for treasures, and you`re no exception.", "It`s human nature!", "The treasure room is on the highest level of the castle, housing precious relics collected over generations. I hold the key. If you`re interested, I`ll take you there next time!", "That sounds great." },
            new[] { "Tell me about the family pact.", "It`s less of a pact and more of deception, a fraud!", "Why do you say that?", "A century ago, our family leader was lured by the Dark King into a dark pact. The pact required each leader to sacrifice their most cherished possession for the family`s glory. This pact became the root of our family`s tragedy!", "Now I see!" },
            new[] { "Tell me about your thoughts on Jeanne", "I hope you`re not looking for an objective review. My feelings for her are biased.", "Love makes everyone blind.", "Jeanne was a girl like the sun, passionate, sincere, cheerful, optimistic. In short, she`s the shining presence in my heart.", "Your love is intense!" },
            new[] { "Tell me about your wish.", "For now, my wish is to lift the dark curse and restore Auter Castle.", "And in the future?", "In the future, I wish to accompany Jeanne back to her hometown, helping her reclaim what belongs to her...", "Feels like there`s more story to come!" } };
        static readonly string[] KeyTips = { "The young lord smiled and handed you a key from his pocket.", "After talking with you, the young lord was so happy that he immediately gave you a key.", "After our pleasant conversation, Lord Anthony happily gave you a key." };
        const byte AComfort = 1, AListen = 2, AStory = 40, AAsk = 80, AEnd = 120;
        static int Today { get { var n = Now; return n.Year * 1000 + n.DayOfYear; } }
        static void AnthonyDialog(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (!Running) { new DB(c, stream, Anthony).T("Lord Anthony:\nAuter Castle has been closed.").End(); return; }
            if (c.Player.Level < MinLevel) { new DB(c, stream, Anthony).T("Lord Anthony:\nYour level is below 80. Please return once you`ve leveled up!").O("Come back after leveling up.", 255).End(); return; }
            if (option == 0)
            {
                new DB(c, stream, Anthony).T("Lord Anthony:\nI was an unfit family leader, leading to my family`s downfall and cursing everyone. And I was an unfit lover, failing to protect Jeanne. I am so terrible!")
                    .O("This isn`t your fault.", AComfort).End();
                return;
            }
            if (option == AComfort)
            {
                new DB(c, stream, Anthony).T("No one has ever said such things to me before. I feel much better. Thank you for comforting me. If you don`t mind my rambling, I can tell you stories from the past...")
                    .O("I would be happy to listen.", AListen).End();
                return;
            }
            if (option == AListen)
            {
                var db = new DB(c, stream, Anthony).T("The story of Jeanne and me is long. What part would you like to hear today?");
                int start = Today % Stories.Length;// 5 topics a day, rotating through the 14 official ones
                for (int i = 0; i < 5; i++) { int k = (start + i) % Stories.Length; db.O(Stories[k][0], (byte)(AStory + k)); }
                db.End();
                return;
            }
            if (option >= AStory && option < AStory + Stories.Length)
            {
                var s = Stories[option - AStory];
                new DB(c, stream, Anthony).T(s[1]).O(s[2], (byte)(AAsk + option - AStory)).End();
                return;
            }
            if (option >= AAsk && option < AAsk + Stories.Length)
            {
                var s = Stories[option - AAsk];
                new DB(c, stream, Anthony).T(s[3]).O(s[4], AEnd).End();
                return;
            }
            if (option == AEnd) { Box(c, stream, DailyKey(c)); return; }
        }
        /// <summary>The key after a chat with Lord Anthony (official ADNTip texts), once a day (derived).</summary>
        public static string DailyKey(Client.GameClient c)
        {
            var cfg = Cfg;
            var p = State(c);
            if (cfg.DailyKeys <= 0) return "Thank you for listening.";
            lock (_lock) if (p.KeyDay == Today) return "Phantoms don`t feel pain. Want to punch me and see? I`m terribly bored. Play with me! (Come back tomorrow for another chat.)";// official Text131/132
            if (!Space(c, 1)) return "Please make some spaces in your inventory first.";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                lock (_lock) { if (p.KeyDay == Today) return "Come back tomorrow."; p.KeyDay = Today; }
                Save();
                bool ok = Give(c, CastleKey, (uint)cfg.DailyKeys, false, rec.GetStream());
                Log(c, "ANTHONY daily key x" + cfg.DailyKeys + (ok ? "" : " (GIVE FAILED)"));
                return KeyTips[Rnd(KeyTips.Length)];
            }
        }

        // ------------------------------------------------------------------ items (UseItem + item dialogs, npcid = item id)
        static readonly Dictionary<uint, uint[]> Choices = new Dictionary<uint, uint[]> {
            { ParasolBox, new uint[] { 4300000, 4300001, 4300002, 4300003, 4300004 } },// Pistil Aroma/Lotus Dew/Begonia Frost/Snowsweet Melody/Peach Bloom
            { FluteBox, new uint[] { 4310000, 4310001, 4310002 } },                   // Ethereal Luster/Fancy Carrier/Droplet Glow
            { StringBox, new uint[] { 4320000, 4320001, 4320002 } },                  // Moonlight Whispers/Celestial Melody/Fox Dream
            { PetBox, new uint[] { 4330000, 4330001, 4330002 } } };                   // Panda Ponpon/Dragon Ruyi/Doggy Pudding (Suyi Text117)
        // [DreamTheater 2026-10-15] White Day 2024 vouchers (Fluffy / Love Velvet goods), official item texts "Use to get 1 permanent ...":
        // the garment / mount armor itself; the "1% Blessed / 3-star / 5-star" attributes and the Swan Sonata hairstyle are not on this server.
        static readonly Dictionary<uint, uint[]> Vouchers = new Dictionary<uint, uint[]> {
            { 3347510, new uint[] { 194005, 0 } }, { 3347511, new uint[] { 194015, 0 } }, { 3347512, new uint[] { 205011, 0 } },
            { 3347513, new uint[] { 194005, 1 } }, { 3347514, new uint[] { 194015, 1 } }, { 3347515, new uint[] { 205011, 1 } } };
        static bool Ours(uint id) { return id == CastleKey || id == FineBox || id == GrandBox || Choices.ContainsKey(id) || Vouchers.ContainsKey(id) || id == Valentine2023.Valentine2023Ui.DreamNote; }

        public static bool OnUse(Client.GameClient c, MsgGameItem item, ServerSockets.Packet stream)
        {
            if (c == null || c.Player == null || item == null || !Ours(item.ITEM_ID)) return false;
            try
            {
                c.ActiveNpc = item.ITEM_ID;
                ItemDialog(c, stream, item.ITEM_ID, 0);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }
        public static bool OnItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (c == null || c.Player == null || !Ours(npcid)) return false;
            try
            {
                if (option == 255) return true;
                if (Count(c, npcid) == 0) { Msg(c, "You don`t have this item in your inventory."); return true; }
                if (!Offered(c, npcid, option)) return true;
                if (Throttled(c.Player.UID, 300)) return true;
                ItemDialog(c, stream, npcid, option);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
            return true;
        }
        static void ItemDialog(Client.GameClient c, ServerSockets.Packet stream, uint id, byte option)
        {
            if (id == Valentine2023.Valentine2023Ui.DreamNote) { DreamNote(c, stream, option); return; }
            if (id == CastleKey)
            {
                if (option == 0) { TreasureMenu(c, stream, CastleKey); return; }
                if (option == OUse1) UseKeys(c, stream, 1); else if (option == OUse10) UseKeys(c, stream, 10);
                return;
            }
            if (id == FineBox || id == GrandBox)
            {
                int keys = id == FineBox ? Cfg.FineKeys : Cfg.GrandKeys;
                if (option == 0) { new DB(c, stream, id).T("Open the " + Name(id) + " to get " + keys + " Castle Keys?").O("Open it.", 1).End(0); return; }
                Box(c, stream, OpenBox(c, id, CastleKey, (uint)keys, stream));
                return;
            }
            uint[] v;
            if (Vouchers.TryGetValue(id, out v))
            {
                if (option == 0) { new DB(c, stream, id).T("Use the " + Name(id) + " to get 1 permanent " + Name(v[0]) + (v[1] != 0 ? " (bound)" : "") + "?").O("Use it.", 1).End(0); return; }
                Box(c, stream, OpenBox(c, id, v[0], 1, stream, v[1] != 0));
                return;
            }
            uint[] list;
            if (Choices.TryGetValue(id, out list))
            {
                if (option == 0)
                {
                    var db = new DB(c, stream, id).T("Open the " + Name(id) + " and choose one:");
                    for (int i = 0; i < list.Length; i++) db.O(Name(list[i]) + ".", (byte)(1 + i));
                    db.End(0);
                    return;
                }
                if (option >= 1 && option <= list.Length) Box(c, stream, OpenBox(c, id, list[option - 1], 1, stream));
            }
        }
        /// <summary>Takes exactly one box, then gives (bag space first). Works also after the event (the official boxes do).</summary>
        public static string OpenBox(Client.GameClient c, uint box, uint give, uint count, ServerSockets.Packet stream, bool bound = false)
        {
            if (Pool.ItemsBase == null || !Pool.ItemsBase.ContainsKey(give)) return "This box can`t be opened on this server.";
            if (!Space(c, Slots(give, count))) return "Please make some spaces in your inventory first.";
            if (!Take(c, box, 1, stream)) return "You don`t have this item in your inventory.";
            bool ok = Give(c, give, count, bound, stream);
            Log(c, "OPEN " + Name(box) + " -> " + Name(give) + " x" + count + (ok ? "" : " (GIVE FAILED)"));
            return ok ? "You got " + Name(give) + (count > 1 ? " x" + count : "") + "." : "Something went wrong. Please contact a GM.";
        }

        // ---- Valentine2023 Dream Note (official tValentineDay2022Npc[3340595] texts; Chi Points / CP options are not on this server)
        const uint Waltz = 199965, WaltzBrilliance = 199975;
        static void DreamNote(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            uint have = Count(c, Valentine2023.Valentine2023Ui.DreamNote);
            if (option == 0)
            {
                new DB(c, stream, Valentine2023.Valentine2023Ui.DreamNote).T("You can turn Dream Notes into Valentine garments.\n25 Dream Notes can be turned into 1 permanent 3-star Twinkle Waltz (B).\n50 Dream Notes can be turned into 1 permanent 5-star Twinkle Waltz (Brilliance).\nWhich garment do you want? (You have " + have + " Dream Notes.)")
                    .O("Twinkle Waltz (B)", 1).O("Twinkle Waltz (Brilliance)", 2).End(0);
                return;
            }
            uint cost = option == 1 ? 25u : 50u, give = option == 1 ? Waltz : WaltzBrilliance;
            if (have < cost) { Box(c, stream, "You don`t have enough Dream Notes to turn into garments."); return; }
            if (!Space(c, 1)) { Box(c, stream, "Please make some spaces in your inventory first."); return; }
            if (!Take(c, Valentine2023.Valentine2023Ui.DreamNote, cost, stream)) { Box(c, stream, "You don`t have enough Dream Notes to turn into garments."); return; }
            bool ok = Give(c, give, 1, true, stream);
            Log(c, "DREAMNOTE " + cost + " -> " + Name(give) + (ok ? "" : " (GIVE FAILED)"));
            Box(c, stream, ok ? "Congrats! You successfully turned Dream Notes into 1 " + (option == 1 ? "Twinkle Waltz (B)" : "Twinkle Waltz (Brilliance)") + "!" : "Something went wrong. Please contact a GM.");
        }
    }
}
