// Martial Token (BattlePass) - server logic. Protocol: BattlePassProto.cs. Season data: BattlePassConfig.cs.
// Per-player state is saved in the character file (Users\<uid>.ini, [BattlePass] State=) by SaveClient and loaded by
// LoadCharacterCore. Every reward / task / token / store action is appended to Infinity[DB]\BattlePass\_claims.log, and
// rewards already written there are never given twice (also after a rollback of the character file).
// Nothing the client sends is trusted: it only names a reward index or task index; level, token, price, count and
// item always come from the config.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Qahr.Mobile.BattlePass
{
    public class BpTaskState
    {
        public ulong Progress;
        public uint Status = 2;
        public int Period;
    }

    public class BpState
    {
        public uint Season;
        public int Level = 1;
        public ulong LevelPoints, HeroPoints, TotalPoints, WeekPoints;
        public int Privilege;
        public int WeekKey, LastLoginDay, LoginDays, LoginPeriod;
        public ulong[] Masks = new ulong[14];
        public Dictionary<int, BpTaskState> Tasks = new Dictionary<int, BpTaskState>();
        /// <summary>Unreadable saved value: kept and written back unchanged (never wipe a player's progress).</summary>
        public string Corrupt;
    }

    public static class BattlePassManager
    {
        const string Section = "BattlePass", Key = "State";
        const string ScoreLimitText = "You`ve reached the upper limit of weekly free points and cannot get more now."; // Cn_Res STR_BATTLEPASS_SCORE_LIMIT

        static readonly ConcurrentDictionary<uint, BpState> states = new ConcurrentDictionary<uint, BpState>();
        static readonly HashSet<string> ledger = new HashSet<string>();
        static readonly object ledgerSync = new object();
        static bool ledgerLoaded;

        static string LedgerFile { get { return Path.Combine(BattlePassConfig.Folder, "_claims.log"); } }

        static bool On { get { BattlePassConfig.EnsureLoaded(); return BattlePassConfig.Enabled && BattlePassConfig.SeasonId != 0; } }

        // ------------------------------------------------------------------ persistence (character ini)
        public static void LoadFromIni(Client.GameClient client, WindowsAPI.IniFile reader)
        {
            try
            {
                if (client == null || client.Player == null || reader == null) return;
                string raw = reader.ReadBigString(Section, Key, "");
                states[client.Player.UID] = Parse(raw);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        public static void SaveToIni(Client.GameClient client, WindowsAPI.IniFile writer)
        {
            try
            {
                if (client == null || client.Player == null || writer == null) return;
                BpState s;
                if (!states.TryGetValue(client.Player.UID, out s) || s == null) return; // not loaded: leave the file as it is
                string text;
                lock (s) text = Serialize(s);
                writer.WriteString(Section, Key, text);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static string Serialize(BpState s)
        {
            if (s.Corrupt != null) return s.Corrupt;
            var sb = new StringBuilder();
            sb.Append("1|").Append(s.Season).Append('|').Append(s.Level).Append('|').Append(s.LevelPoints).Append('|')
              .Append(s.HeroPoints).Append('|').Append(s.TotalPoints).Append('|').Append(s.Privilege).Append('|')
              .Append(s.WeekPoints).Append('|').Append(s.WeekKey).Append('|').Append(s.LastLoginDay).Append('|')
              .Append(s.LoginDays).Append('|').Append(s.LoginPeriod).Append('|');
            for (int g = 1; g <= 13; g++) { if (g > 1) sb.Append(';'); sb.Append(s.Masks[g].ToString("x")); }
            sb.Append('|');
            bool first = true;
            foreach (var kv in s.Tasks)
            {
                if (!first) sb.Append(';');
                first = false;
                sb.Append(kv.Key).Append(':').Append(kv.Value.Progress).Append(':').Append(kv.Value.Status).Append(':').Append(kv.Value.Period);
            }
            return sb.ToString();
        }

        static BpState Parse(string raw)
        {
            var s = new BpState();
            if (string.IsNullOrEmpty(raw) || raw == "0") return s;
            try
            {
                string[] p = raw.Split('|');
                if (p.Length < 14 || p[0] != "1") throw new FormatException("version");
                s.Season = uint.Parse(p[1]);
                s.Level = int.Parse(p[2]);
                s.LevelPoints = ulong.Parse(p[3]);
                s.HeroPoints = ulong.Parse(p[4]);
                s.TotalPoints = ulong.Parse(p[5]);
                s.Privilege = int.Parse(p[6]);
                s.WeekPoints = ulong.Parse(p[7]);
                s.WeekKey = int.Parse(p[8]);
                s.LastLoginDay = int.Parse(p[9]);
                s.LoginDays = int.Parse(p[10]);
                s.LoginPeriod = int.Parse(p[11]);
                string[] m = p[12].Split(';');
                for (int g = 1; g <= 13 && g - 1 < m.Length; g++)
                    s.Masks[g] = ulong.Parse(m[g - 1], NumberStyles.HexNumber);
                if (p[13].Length > 0)
                    foreach (string t in p[13].Split(';'))
                    {
                        string[] q = t.Split(':');
                        s.Tasks[int.Parse(q[0])] = new BpTaskState { Progress = ulong.Parse(q[1]), Status = uint.Parse(q[2]), Period = int.Parse(q[3]) };
                    }
                if (s.Level < 1) s.Level = 1;
                if (s.Privilege < 0 || s.Privilege > 2) throw new FormatException("privilege");
                return s;
            }
            catch (Exception e)
            {
                MyConsole.WriteLine("[BattlePass] unreadable state kept unchanged: " + e.Message);
                return new BpState { Corrupt = raw };
            }
        }

        // ------------------------------------------------------------------ time keys
        static int DayKey(DateTime d) { return d.Year * 10000 + d.Month * 100 + d.Day; }
        static int WeekKeyOf(DateTime d)
        {
            DateTime monday = d.Date.AddDays(-(((int)d.DayOfWeek + 6) % 7));
            return DayKey(monday);
        }
        static int PeriodOf(BpTask t, DateTime now)
        {
            if (t.ResetType == 1) return DayKey(now);
            if (t.ResetType == 2) return WeekKeyOf(now);
            return 0;
        }

        /// <summary>State of an online player, rolled to the current season / week / day. Returns null when off or unreadable.</summary>
        static BpState Get(Client.GameClient client)
        {
            if (client == null || client.Player == null || !On) return null;
            BpState s = states.GetOrAdd(client.Player.UID, delegate (uint k) { return new BpState(); });
            lock (s)
            {
                if (s.Corrupt != null) return null;
                Roll(s, DateTime.Now, true);
            }
            return s;
        }

        static void Roll(BpState s, DateTime now, bool online)
        {
            uint season = BattlePassConfig.SeasonId;
            if (s.Season != season)
            {
                // new season: everything starts over (claims are per season; the ledger key holds the season id)
                var fresh = new BpState();
                s.Season = season; s.Level = 1; s.LevelPoints = s.HeroPoints = s.TotalPoints = s.WeekPoints = 0;
                s.Privilege = 0; s.WeekKey = 0; s.LastLoginDay = 0; s.LoginDays = 0; s.LoginPeriod = 0;
                s.Masks = fresh.Masks; s.Tasks = fresh.Tasks;
            }
            int week = WeekKeyOf(now);
            if (s.WeekKey != week) { s.WeekKey = week; s.WeekPoints = 0; }
            foreach (var t in BattlePassConfig.Tasks.Values)
            {
                BpTaskState ts;
                int period = PeriodOf(t, now);
                if (!s.Tasks.TryGetValue(t.Index, out ts)) { ts = new BpTaskState { Period = period }; s.Tasks[t.Index] = ts; }
                else if (ts.Period != period) { ts.Progress = 0; ts.Status = 2; ts.Period = period; }
            }
            if (online)
            {
                int today = DayKey(now);
                if (s.LastLoginDay != today)
                {
                    int lp = BattlePassConfig.LoginDaysPerWeek ? week : 0;
                    if (s.LoginPeriod != lp) { s.LoginPeriod = lp; s.LoginDays = 0; }
                    s.LastLoginDay = today;
                    s.LoginDays++;
                }
                // official login tasks: 10 = daily login, 121 / 122 = login days (BattlePassTask.lua names)
                foreach (var t in BattlePassConfig.Tasks.Values)
                {
                    if (t.Type == 10) SetProgress(s, t, 1);
                    else if (t.Type == 121 || t.Type == 122) SetProgress(s, t, (ulong)s.LoginDays);
                }
            }
        }

        static bool SetProgress(BpState s, BpTask t, ulong value)
        {
            BpTaskState ts = s.Tasks[t.Index];
            if (ts.Status != 2) return false;
            if (value > t.Complete) value = t.Complete;
            if (value <= ts.Progress) return false;
            ts.Progress = value;
            return true;
        }

        static ulong EffectiveProgress(BpState s, BpTask t)
        {
            BpTaskState ts = s.Tasks[t.Index];
            if (ts.Status == 2 && s.Privilege >= t.AutoPrivilege) return t.Complete; // token auto-complete
            return ts.Progress;
        }

        // ------------------------------------------------------------------ outgoing
        public static void OnLogin(Client.GameClient client)
        {
            try
            {
                if (client == null || client.Player == null) return;
                BattlePassConfig.EnsureLoaded();
                // top-bar icon (CMsgBattlePass type 0 -> PostCmd 0x110e -> panel key 22), season id or 0 = hidden
                BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePass, new BpMsg { Type = 0, Value = On ? BattlePassConfig.SeasonId : 0 });
                Get(client);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static void SendUser(Client.GameClient client, BpState s, ulong gained)
        {
            var list = new List<BpUserAttr>();
            lock (s)
            {
                list.Add(new BpUserAttr { Type = 0, Value = s.LevelPoints });
                list.Add(new BpUserAttr { Type = 1, Value = s.HeroPoints });
                list.Add(new BpUserAttr { Type = 2, Value = (ulong)s.Privilege });
                list.Add(new BpUserAttr { Type = 3, Value = (ulong)s.Level });
                list.Add(new BpUserAttr { Type = 4, Value = s.WeekPoints });
                foreach (int g in BattlePassConfig.RewardGroups)
                    list.Add(new BpUserAttr { Type = (uint)(100 + g), Value = s.Masks[g] });
            }
            if (gained > 0) list.Add(new BpUserAttr { Type = 201, Value = gained });
            BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePassUser, new BpUserInfo { Attrs = list.ToArray() });
        }

        static void SendTasks(Client.GameClient client, BpState s)
        {
            var items = new List<BpTaskItem>();
            lock (s)
            {
                foreach (var t in BattlePassConfig.Tasks.Values)
                    items.Add(new BpTaskItem { Index = (uint)t.Index, Progress = EffectiveProgress(s, t), Status = s.Tasks[t.Index].Status });
            }
            BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePassTaskList, new BpTaskList { Items = items.ToArray() });
        }

        // ------------------------------------------------------------------ incoming 2064
        public static void OnClientMsg(Client.GameClient client, BpMsgIn req)
        {
            BpState s = Get(client);
            if (s == null) return;
            switch (req.Type)
            {
                case 2: SendTasks(client, s); break;            // window opened (first request)
                case 4: SendUser(client, s, 0); break;          // window opened: this answer opens it
                case 1: Claim(client, s, new ulong[] { req.Value }, false); break;
                case 6: Claim(client, s, req.List ?? new ulong[0], true); break;
                case 5: BuyToken(client, s, req.Value, req.List != null && req.List.Length > 0 ? req.List[0] : 2); break;
                default: break;
            }
        }

        static void Claim(Client.GameClient client, BpState s, ulong[] wanted, bool all)
        {
            var done = new List<ulong>();
            string fail = null;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                lock (s)
                {
                    var seen = new HashSet<ulong>();
                    foreach (ulong idx in wanted)
                    {
                        if (done.Count >= 200 || !seen.Add(idx) || idx > int.MaxValue) continue;
                        BpReward r;
                        if (!BattlePassConfig.Rewards.TryGetValue((int)idx, out r)) continue;
                        if (s.Level < r.Level || s.Privilege < r.Privilege) continue;
                        ulong bit = 1UL << (r.Bit - 1);
                        if ((s.Masks[r.Group] & bit) != 0) continue;
                        string lkey = client.Player.UID + ":" + s.Season + ":" + r.Index;
                        if (LedgerHas(lkey)) { s.Masks[r.Group] |= bit; continue; } // already given once (file rolled back)
                        Database.ItemType.DBItem db;
                        if (!Pool.ItemsBase.TryGetValue(r.ItemId, out db))
                        {
                            fail = "This reward item (" + r.ItemId + ") is not available on this server yet.";
                            Log(client, s, "REWARD-MISSING-ITEM", r.Index + " item " + r.ItemId);
                            continue;
                        }
                        int slots = db.StackSize > 1 ? (r.Count + db.StackSize - 1) / db.StackSize : r.Count;
                        if (client.OnInterServer) slots++;
                        if (slots > 255 || !client.Inventory.HaveSpace((byte)slots))
                        {
                            fail = "Your inventory is full. Make room and claim again.";
                            break;
                        }
                        // mark first, then give: a failure after this point can never be repeated for a second copy
                        s.Masks[r.Group] |= bit;
                        LedgerAdd(lkey, client, s, "REWARD", r.Index + " L" + r.Level + " P" + r.Privilege + " item " + r.ItemId + " x" + r.Count + (r.Bound ? " bound" : ""));
                        if (!client.Inventory.AddItemWitchStack(r.ItemId, 0, (ushort)r.Count, stream, r.Bound))
                            Log(client, s, "REWARD-ADD-FAILED", r.Index + " item " + r.ItemId + " x" + r.Count);
                        done.Add((ulong)r.Index);
                    }
                }
            }
            if (done.Count > 0)
            {
                if (all) BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePass, new BpMsg { Type = 6, Value = 0, List = done.ToArray() });
                else BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePass, new BpMsg { Type = 1, Value = done[0] });
            }
            if (fail != null) client.SendSysMesage(fail, Game.MsgServer.MsgMessage.ChatMode.System);
            SendUser(client, s, 0);
        }

        static void BuyToken(Client.GameClient client, BpState s, ulong target, ulong method)
        {
            string msg = null;
            bool ok = false;
            lock (s)
            {
                // OFFICIAL: VIPInfo Money = 0 and ItemType = 0, so only the CPs button (method 2) is offered by the client
                if (target < 1 || target > 2 || (int)target <= s.Privilege || method != 2)
                    msg = "Purchase failed. You haven`t met the requirement."; // STR_BATTLEPASS_VIP_CANT_BUY
                else
                {
                    uint cost = BattlePassConfig.Price[target] > BattlePassConfig.Price[s.Privilege]
                        ? BattlePassConfig.Price[target] - BattlePassConfig.Price[s.Privilege] : 0;
                    if (client.Player.ConquerPoints < cost)
                        msg = "Insufficient CPs."; // STR_BP_TASK_NOT_ENOUGHT_EMONEY
                    else
                    {
                        client.Player.ConquerPoints -= cost;
                        int old = s.Privilege;
                        s.Privilege = (int)target;
                        Log(client, s, "TOKEN", old + "->" + target + " paid " + cost + " CPs");
                        ok = true;
                    }
                }
            }
            if (ok)
            {
                BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePass, new BpMsg { Type = 5, Value = target });
                SendTasks(client, s);
            }
            else if (msg != null) client.SendSysMesage(msg, Game.MsgServer.MsgMessage.ChatMode.System);
            SendUser(client, s, 0);
        }

        // ------------------------------------------------------------------ incoming 2273 (task claim / multiply)
        public static void OnClientTaskMsg(Client.GameClient client, BpTaskMsgIn req)
        {
            BpState s = Get(client);
            if (s == null || req.Type != 0 || req.Values == null || req.Values.Length < 1 || req.Values[0] > int.MaxValue) return;
            BpTask t;
            if (!BattlePassConfig.Tasks.TryGetValue((int)req.Values[0], out t)) return;
            string msg = null;
            ulong gained = 0;
            uint status = 0;
            bool ok = false;
            lock (s)
            {
                BpTaskState ts = s.Tasks[t.Index];
                if (s.Privilege < t.ReqPrivilege || ts.Status == 0) { }
                else if (ts.Status == 2)
                {
                    if (EffectiveProgress(s, t) < t.Complete) { }
                    else
                    {
                        bool multiply = s.Privilege >= t.MultiPrivilege1; // "Free Multiply" (client dlgbptask FUN_009f4440)
                        ulong pts = (ulong)t.Score * (ulong)(multiply ? BattlePassConfig.MultiplyFactor : 1);
                        ulong room = BattlePassConfig.WeeklyFreePointCap > s.WeekPoints ? BattlePassConfig.WeeklyFreePointCap - s.WeekPoints : 0;
                        if (room == 0) msg = ScoreLimitText;
                        else
                        {
                            if (pts > room) pts = room;
                            s.WeekPoints += pts;
                            ts.Progress = t.Complete;
                            ts.Status = status = multiply ? 0u : 1u;
                            gained = pts;
                            AddPoints(s, pts);
                            Log(client, s, "TASK", t.Index + " type " + t.Type + " +" + pts + (multiply ? " (free multiply)" : ""));
                            ok = true;
                        }
                    }
                }
                else if (ts.Status == 1)
                {
                    // "Pay Multiply": the client asked the player to confirm multi_reward_cost CPs
                    uint cost = (uint)Math.Max(0, t.MultiCost);
                    if (client.Player.ConquerPoints < cost) msg = "Insufficient CPs"; // STR_BP_TASK_NOT_ENOUGHT_EMONEY
                    else
                    {
                        client.Player.ConquerPoints -= cost;
                        ulong pts = (ulong)t.Score * (ulong)Math.Max(0, BattlePassConfig.MultiplyFactor - 1);
                        ts.Status = status = 0;
                        gained = pts;
                        AddPoints(s, pts);
                        Log(client, s, "TASK-MULTIPLY", t.Index + " type " + t.Type + " +" + pts + " paid " + cost + " CPs");
                        ok = true;
                    }
                }
            }
            if (ok)
                BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePassTask, new BpTaskMsg { Type = 0, Values = new ulong[] { (ulong)t.Index, status, gained } });
            if (msg != null) client.SendSysMesage(msg, Game.MsgServer.MsgMessage.ChatMode.System);
            SendUser(client, s, gained);
        }

        static void AddPoints(BpState s, ulong pts)
        {
            s.LevelPoints += pts;
            s.HeroPoints += pts;
            s.TotalPoints += pts;
            while (s.Level < BattlePassConfig.MaxLevel)
            {
                ulong need = BattlePassConfig.Need(s.Level);
                if (need == 0 || s.LevelPoints < need) break;
                s.LevelPoints -= need;
                s.Level++;
            }
        }

        // ------------------------------------------------------------------ hooks from game events
        /// <summary>Game event for task type <paramref name="taskType"/> (BattlePass.lua BattlepassTask "type").</summary>
        public static void AddProgress(Client.GameClient client, int taskType, ulong amount = 1)
        {
            try
            {
                BpState s = Get(client);
                if (s == null) return;
                var changed = new List<ulong>();
                lock (s)
                {
                    foreach (var t in BattlePassConfig.Tasks.Values)
                    {
                        if (t.Type != taskType) continue;
                        BpTaskState ts = s.Tasks[t.Index];
                        if (SetProgress(s, t, ts.Progress + amount)) { changed.Add((ulong)t.Index); changed.Add(ts.Progress); }
                    }
                }
                for (int i = 0; i + 1 < changed.Count; i += 2)
                    BattlePassPackets.Send(client, BattlePassPackets.MsgBattlePassTask, new BpTaskMsg { Type = 2, Values = new ulong[] { changed[i], changed[i + 1] } });
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        // ------------------------------------------------------------------ Point Store (exchange shop paid with Hero Points)
        /// <summary>MsgExchangeShopBuy type 1 for a Hero-Points good (cost_type 2 in a BattlePass shop). True = bought.</summary>
        public static bool ShopBuy(Client.GameClient client, uint shopId, uint index, uint count, Database.ExchangeShopGoodsEx.item good, ServerSockets.Packet stream)
        {
            try
            {
                if (good == null || good.shopid != shopId || !BattlePassConfig.ShopIds.Contains(shopId)) return false;
                if (count < 1 || count > 1000 || good.cost_value == 0) return false;
                BpState s = Get(client);
                if (s == null) return false;
                Database.ItemType.DBItem db;
                if (!Pool.ItemsBase.TryGetValue(good.itemtypeid, out db)) return false;
                int slots = db.StackSize > 1 ? (int)((count + db.StackSize - 1) / db.StackSize) : (int)count;
                if (slots > 255 || !client.Inventory.HaveSpace((byte)slots))
                {
                    client.SendSysMesage("Your inventory is full.", Game.MsgServer.MsgMessage.ChatMode.System);
                    return false;
                }
                if (!client.MyExchangeShop.CanAddItemEx(shopId, index, count)) return false;
                ulong cost = (ulong)good.cost_value * count;
                lock (s)
                {
                    if (s.HeroPoints < cost)
                    {
                        client.SendSysMesage("You don`t have enough points.", Game.MsgServer.MsgMessage.ChatMode.System); // STR_BATTLEPASS_SHOP_LIMIT3
                        return false;
                    }
                    s.HeroPoints -= cost;
                    Log(client, s, "STORE", "shop " + shopId + " good " + index + " item " + good.itemtypeid + " x" + count + " -" + cost + " pts");
                }
                client.Inventory.AddItemWitchStack(good.itemtypeid, 0, (ushort)count, stream, good.monopoly != 0);
                client.MyExchangeShop.AddItemEx(shopId, index, count);
                SendUser(client, s, 0);
                return true;
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return false; }
        }

        // ------------------------------------------------------------------ ledger / log
        static void LoadLedger()
        {
            if (ledgerLoaded) return;
            ledgerLoaded = true;
            try
            {
                if (!File.Exists(LedgerFile)) return;
                foreach (string l in File.ReadAllLines(LedgerFile))
                {
                    string[] p = l.Split('|');
                    if (p.Length >= 6 && p[4] == "REWARD") ledger.Add(p[5]);
                }
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }

        static bool LedgerHas(string key)
        {
            lock (ledgerSync) { LoadLedger(); return ledger.Contains(key); }
        }

        static void LedgerAdd(string key, Client.GameClient client, BpState s, string what, string detail)
        {
            lock (ledgerSync)
            {
                LoadLedger();
                ledger.Add(key);
                Write(client, s, what, key + "|" + detail);
            }
        }

        static void Log(Client.GameClient client, BpState s, string what, string detail)
        {
            lock (ledgerSync) Write(client, s, what, "-|" + detail);
        }

        static void Write(Client.GameClient client, BpState s, string what, string rest)
        {
            try
            {
                Directory.CreateDirectory(BattlePassConfig.Folder);
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "|" + client.Player.UID + "|" + client.Player.Name + "|S" + s.Season
                    + "|" + what + "|" + rest + "|L" + s.Level + " pts " + s.LevelPoints + " hero " + s.HeroPoints + " token " + s.Privilege;
                File.AppendAllText(LedgerFile, line + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } }
        }
    }
}
