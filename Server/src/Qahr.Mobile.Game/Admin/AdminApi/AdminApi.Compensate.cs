// ============================================================================
//  AdminApi - [Compensation 2026-10-15] compensation campaigns (tools\CompensationStudio).
//
//  Official flow: a SYSTEM MAIL with attachments that the player claims from the mailbox (MY System\MailBox).
//  The mailbox holds silver + CPs + ONE item per mail (the item waits in the inbox warehouse 1000), so a bundle
//  becomes one mail per item stack (silver/CPs ride on the first one). A full bag never loses anything: the item
//  stays in the mail until the player has room. Bound CPs and EXP have no mail field -> given directly at delivery.
//
//  Once per campaign + uid, recorded in MySQL (Base\EventData\EventStore.cs, event "Compensation"):
//    claim  (uid, "Compensation", <campaignId>)        INSERT IGNORE before anything is given (TryClaimOnce)
//    flag   p:<campaignId>                             queued for an offline / still-loading player
//    flag   x:<campaignId>                             cancelled for this player (never delivered)
//    claim  (0, "Compensation", "c:<campaignId>")      the campaign id itself (a second "send" is refused)
//  MySQL down => send refused; a queued player is retried at the next login; nothing is granted without a claim row.
//  Login: EventStore.OnLogin -> CompEngine.OnLogin(uid) watches players with p: flags; a 2 s timer delivers once the
//  client finished loading (FullLoading + warehouse). Relogs / re-sends find the claim and give nothing.
//
//  Files: Data\Compensation\campaigns\<id>.json (+ <id>.recipients.txt), Data\Compensation\log.txt
//
//  op "compensate":
//    {"action":"status"}                                              -> time, dbOk, campaigns, watching
//    {"action":"preview", campaign..., "target":{...}, "reward":{...}} -> validated, count, online/offline, sample[20], mailsPerPlayer
//    {"action":"send",    same + "confirm":true}                      -> creates the campaign, delivers to ready players, queues the rest
//    {"action":"list"}                                                -> campaigns + recipients/delivered/pending/cancelled/expired
//    {"action":"get","id":"..."[,"limit":5000]}                       -> campaign + recipients with status
//    {"action":"cancel","id":"..."[,"uids":[..]]}                     -> cancels pending (not yet delivered) recipients; no uids = whole campaign
//  campaign: "id" [A-Za-z0-9_-]{3,40}, "title" (ASCII, <=60), "body" (ASCII, <=500),
//            expiry: "expiresAt":"yyyy-MM-dd HH:mm" (1 h - 1 year) | "noExpiry":true (1 year) | "expiresDays" 1-365
//  target  : "scope" all|online|list, "names":[..], "uids":[..], "minLevel","maxLevel","minReborn","minVip","since":"yyyy-MM-dd[ HH:mm]"
//  reward  : "items":[{"id","count","bound"}] (<=10 lines), "silver" (<=1e9), "cps" (<=100000), "boundCps" (<=1000000),
//            "expPercent" (0-1000, % of the current level), "levels" (0-5)
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

#if !COMP_HARNESS
namespace Qahr.Mobile.AdminApi
{
    public static partial class AdminApiServer
    {
        // ------------------------------------------------------------------ op
        private static Dictionary<string, object> CompensateOp(AdminArgs a)
        {
            string action = (a.Str("action") ?? "status").Trim().ToLowerInvariant();
            switch (action)
            {
                case "status": return CompStatus();
                case "preview": return CompPreviewOrSend(a, false);
                case "send": return CompPreviewOrSend(a, true);
                case "list": return CompList();
                case "get": return CompGet(a);
                case "cancel": return CompCancel(a);
                default: throw new AdminError("'action' must be status, preview, send, list, get or cancel");
            }
        }

        private static Dictionary<string, object> CompStatus()
        {
            var d = Ok();
            d["time"] = DateTime.Now;
            d["dbOk"] = global::Qahr.Mobile.Compensation.CompEngine.DbProbe();
            d["campaigns"] = global::Qahr.Mobile.Compensation.CompEngine.LoadAll().Count;
            d["watching"] = global::Qahr.Mobile.Compensation.CompEngine.WatchCount;
            d["online"] = OnlineClients().Count(c => !c.Fake);
            return d;
        }

        private sealed class CompTarget
        {
            public uint Uid; public string Name; public int Level, Reborn, Vip; public bool Online; public DateTime Seen;
        }

        private static DateTime? CompSince(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            DateTime t;
            string[] fmts = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" };
            if (!DateTime.TryParseExact(s.Trim(), fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out t))
                throw new AdminError("'since' must be yyyy-MM-dd or yyyy-MM-dd HH:mm");
            return t;
        }

        /// <summary>Resolves the target spec to players. Bots are never included. unknown = names/uids that were not found.</summary>
        private static List<CompTarget> CompResolve(AdminArgs t, List<string> unknown)
        {
            string scope = (t.Str("scope") ?? "all").Trim().ToLowerInvariant();
            if (scope != "all" && scope != "online" && scope != "list") throw new AdminError("target 'scope' must be all, online or list");
            int maxLevel = MaxLevel;
            long minLevel = t.Long("minLevel", 0), maxLv = t.Long("maxLevel", 0), minReborn = t.Long("minReborn", 0), minVip = t.Long("minVip", 0);
            if (minLevel < 0 || minLevel > maxLevel || maxLv < 0 || maxLv > maxLevel || (maxLv > 0 && maxLv < minLevel)) throw new AdminError("level range must be within 0-" + maxLevel);
            if (minReborn < 0 || minReborn > 2) throw new AdminError("'minReborn' must be 0-2");
            if (minVip < 0 || minVip > 7) throw new AdminError("'minVip' must be 0-7");
            DateTime? since = CompSince(t.Str("since"));

            var wantNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var wantUids = new HashSet<uint>();
            if (scope == "list")
            {
                var names = t.List("names");
                if (names != null) foreach (var n in names) { string s = (n ?? "").ToString().Trim(); if (s.Length > 0) wantNames.Add(s); }
                var uids = t.List("uids");
                if (uids != null) foreach (var u in uids) { uint v; if (uint.TryParse((u ?? "").ToString().Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) && v > 0) wantUids.Add(v); else unknown.Add((u ?? "").ToString()); }
                if (wantNames.Count + wantUids.Count == 0) throw new AdminError("target list is empty");
                if (wantNames.Count + wantUids.Count > 5000) throw new AdminError("target list is limited to 5000 names/uids");
            }

            var all = new Dictionary<uint, CompTarget>();
            foreach (var c in OnlineClients())
            {
                if (c.Fake) continue;
                var p = c.Player;
                all[p.UID] = new CompTarget { Uid = p.UID, Name = p.Name, Level = p.Level, Reborn = p.Reborn, Vip = p.VipLevel, Online = true, Seen = DateTime.Now };
            }
            if (scope != "online")
            {
                string dir = DbFile("Users");
                if (Directory.Exists(dir))
                    foreach (string f in Directory.EnumerateFiles(dir, "*.ini"))
                    {
                        uint uid;
                        if (!uint.TryParse(Path.GetFileNameWithoutExtension(f), out uid) || uid == 0 || all.ContainsKey(uid)) continue;
                        if (scope == "list" && wantUids.Count > 0 && wantNames.Count == 0 && !wantUids.Contains(uid)) continue;
                        var kv = ReadIniCharacter(f);
                        if (kv == null) continue;
                        string name = Get(kv, "Name", "");
                        if (name.Length == 0) continue;
                        all[uid] = new CompTarget { Uid = uid, Name = name, Level = (int)GetL(kv, "Level"), Reborn = (int)GetL(kv, "Reborn"), Vip = (int)GetL(kv, "VipLevel"), Online = false, Seen = File.GetLastWriteTime(f) };
                    }
            }

            IEnumerable<CompTarget> q = all.Values;
            if (scope == "list")
            {
                var found = new List<CompTarget>();
                var byName = new Dictionary<string, CompTarget>(StringComparer.OrdinalIgnoreCase);
                foreach (var x in all.Values) if (!byName.ContainsKey(x.Name)) byName[x.Name] = x;
                foreach (var n in wantNames) { CompTarget x; if (byName.TryGetValue(n, out x)) found.Add(x); else unknown.Add(n); }
                foreach (var u in wantUids) { CompTarget x; if (all.TryGetValue(u, out x)) found.Add(x); else unknown.Add(u.ToString(CultureInfo.InvariantCulture)); }
                q = found.GroupBy(x => x.Uid).Select(g => g.First());
            }
            if (minLevel > 0) q = q.Where(x => x.Level >= minLevel);
            if (maxLv > 0) q = q.Where(x => x.Level <= maxLv);
            if (minReborn > 0) q = q.Where(x => x.Reborn >= minReborn);
            if (minVip > 0) q = q.Where(x => x.Vip >= minVip);
            if (since.HasValue) q = q.Where(x => x.Online || x.Seen >= since.Value);
            var list = q.OrderByDescending(x => x.Online).ThenByDescending(x => x.Level).ThenBy(x => x.Uid).ToList();
            if (list.Count > 50000) throw new AdminError("more than 50000 targets - narrow the filters");
            return list;
        }

        private static global::Qahr.Mobile.Compensation.Campaign CompParse(AdminArgs a)
        {
            var c = new global::Qahr.Mobile.Compensation.Campaign();
            c.Id = (a.Str("id") ?? "").Trim();
            c.Title = (a.Str("title") ?? "").Trim();
            c.Body = (a.Str("body") ?? "").Trim();
            c.Created = DateTime.Now;
            if (a.Bool("noExpiry")) c.Expires = c.Created.AddDays(365);          // "no expiry" = the longest mail life (1 year)
            else if (!string.IsNullOrWhiteSpace(a.Str("expiresAt")))
            {
                DateTime at;
                if (!DateTime.TryParseExact(a.Str("expiresAt").Trim(), new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out at))
                    throw new AdminError("'expiresAt' must be yyyy-MM-dd HH:mm");
                if (at < c.Created.AddMinutes(55) || at > c.Created.AddDays(366)) throw new AdminError("'expiresAt' must be between 1 hour and 1 year from now");
                c.Expires = at;
            }
            else
            {
                long days = a.Long("expiresDays", 7);
                if (days < 1 || days > 365) throw new AdminError("'expiresDays' must be 1-365");
                c.Expires = c.Created.AddDays(days);
            }
            var rd = a.Raw.ContainsKey("reward") ? a.Raw["reward"] as Dictionary<string, object> : null;
            if (rd == null) throw new AdminError("missing 'reward'");
            c.Reward = global::Qahr.Mobile.Compensation.CompEngine.ParseReward(new AdminArgs(rd));
            string err = global::Qahr.Mobile.Compensation.CompEngine.Validate(c);
            if (err != null) throw new AdminError(err);
            return c;
        }

        private static Dictionary<string, object> CompPreviewOrSend(AdminArgs a, bool send)
        {
            var camp = CompParse(a);
            var td = a.Raw.ContainsKey("target") ? a.Raw["target"] as Dictionary<string, object> : null;
            if (td == null) throw new AdminError("missing 'target'");
            var targs = new AdminArgs(td);
            var unknown = new List<string>();
            var targets = CompResolve(targs, unknown);
            camp.TargetText = global::Qahr.Mobile.Compensation.CompEngine.DescribeTarget(targs);
            var r = Ok();
            r["count"] = targets.Count;
            r["online"] = targets.Count(x => x.Online);
            r["offline"] = targets.Count(x => !x.Online);
            r["unknown"] = unknown.Cast<object>().ToList();
            r["mailsPerPlayer"] = global::Qahr.Mobile.Compensation.CompEngine.MailCount(camp.Reward);
            r["reward"] = camp.Reward.Describe();
            r["target"] = camp.TargetText;
            r["expires"] = camp.Expires;
            r["dbOk"] = global::Qahr.Mobile.Compensation.CompEngine.DbProbe();
            r["exists"] = global::Qahr.Mobile.Compensation.CompEngine.Load(camp.Id) != null;
            var sample = new List<object>();
            foreach (var x in targets.Take(20))
                sample.Add(new Dictionary<string, object> { { "uid", x.Uid }, { "name", x.Name }, { "level", x.Level }, { "reborn", x.Reborn }, { "vip", x.Vip }, { "online", x.Online } });
            r["sample"] = sample;
            if (!send) { r["message"] = "dry run: " + targets.Count + " players"; return r; }

            if (!a.Bool("confirm")) throw new AdminError("send needs \"confirm\":true (run preview first)");
            if (unknown.Count > 0) throw new AdminError("unknown players in the list: " + string.Join(", ", unknown.Take(10).ToArray()));
            if (targets.Count == 0) throw new AdminError("no players match the target");
            camp.CreatedBy = "AdminApi";
            camp.Targets = targets.Count;
            string cerr = global::Qahr.Mobile.Compensation.CompEngine.Create(camp);
            if (cerr != null) { Log("compensate", camp.Id, "send REFUSED: " + cerr, false); throw new AdminError(cerr); }
            var res = global::Qahr.Mobile.Compensation.CompEngine.SendAll(camp, targets.Select(x => new KeyValuePair<uint, string>(x.Uid, x.Name)).ToList());
            foreach (var kv in res) r[kv.Key] = kv.Value;
            r["message"] = "campaign '" + camp.Id + "': " + res["delivered"] + " delivered now, " + res["queued"] + " queued, " + res["failed"] + " failed";
            Log("compensate", camp.Id, (string)r["message"] + " | " + camp.Reward.Describe() + " | " + camp.TargetText, true);
            return r;
        }

        private static Dictionary<string, object> CompList()
        {
            var r = Ok();
            r["campaigns"] = global::Qahr.Mobile.Compensation.CompEngine.ListJson();
            r["time"] = DateTime.Now;
            return r;
        }

        private static Dictionary<string, object> CompGet(AdminArgs a)
        {
            string id = (a.Str("id") ?? "").Trim();
            int limit = (int)Math.Max(1, Math.Min(20000, a.Long("limit", 5000)));
            var d = global::Qahr.Mobile.Compensation.CompEngine.DetailJson(id, limit);
            if (d == null) throw new AdminError("campaign '" + id + "' not found");
            var r = Ok();
            foreach (var kv in d) r[kv.Key] = kv.Value;
            return r;
        }

        private static Dictionary<string, object> CompCancel(AdminArgs a)
        {
            string id = (a.Str("id") ?? "").Trim();
            List<uint> uids = null;
            var l = a.List("uids");
            if (l != null)
            {
                uids = new List<uint>();
                foreach (var o in l) { uint u; if (uint.TryParse((o ?? "").ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out u) && u > 0) uids.Add(u); }
                if (uids.Count == 0) throw new AdminError("'uids' is empty");
            }
            string err;
            var res = global::Qahr.Mobile.Compensation.CompEngine.Cancel(id, uids, out err);
            if (err != null) throw new AdminError(err);
            var r = Ok("campaign '" + id + "': " + res["cancelled"] + " pending cancelled, " + res["alreadyDelivered"] + " already delivered");
            foreach (var kv in res) r[kv.Key] = kv.Value;
            Log("compensate", id, "cancel: " + r["message"], true);
            return r;
        }

        // ------------------------------------------------------------------ game side of the engine
        internal sealed class CompGameSink : global::Qahr.Mobile.Compensation.ICompSink
        {
            public bool ItemInfo(uint id, out string name, out int stack)
            {
                name = null; stack = 1;
                Database.ItemType.DBItem db;
                if (Pool.ItemsBase == null || !Pool.ItemsBase.TryGetValue(id, out db) || db == null) return false;
                name = (db.Name ?? ("Item " + id)).Replace('~', ' ');
                stack = Math.Max(1, (int)db.StackSize);
                return true;
            }

            private static Client.GameClient Ready(uint uid)
            {
                Client.GameClient c;
                if (Pool.GamePoll == null || !Pool.GamePoll.TryGetValue(uid, out c) || c == null) return null;
                if (c.Fake || c.Player == null || !c.FullLoading || c.OnInterServer || c.Socket == null || !c.Socket.Alive) return null;
                if (c.Warehouse == null || c.Inventory == null || c.PrizeInfo == null || c.Player.UID != uid) return null;
                return c;
            }

            public bool IsReady(uint uid) { return Ready(uid) != null; }

            public string NameOf(uint uid) { var c = Ready(uid); return c != null ? c.Player.Name : null; }

            public global::Qahr.Mobile.Compensation.GrantResult Grant(uint uid, global::Qahr.Mobile.Compensation.Campaign camp)
            {
                var res = new global::Qahr.Mobile.Compensation.GrantResult();
                var c = Ready(uid);
                if (c == null) { res.Error = "player not ready"; return res; }
                var rw = camp.Reward;
                var parts = new List<string>();
                try
                {
                    uint secs = (uint)Math.Max(3600, Math.Min(366 * 86400.0, (camp.Expires - DateTime.Now).TotalSeconds));
                    const string sender = "STR_SYSTEM_NAME@@";
                    bool moneySent = rw.Silver == 0 && rw.Cps == 0;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var it in rw.Items)
                        {
                            Database.ItemType.DBItem db;
                            if (!Pool.ItemsBase.TryGetValue(it.Id, out db) || db == null) { parts.Add("SKIPPED unknown item " + it.Id); continue; }
                            int stackMax = Math.Max(1, (int)db.StackSize);
                            int left = it.Count, mails = 0;
                            while (left > 0 && mails < 40)
                            {
                                int stack = stackMax > 1 ? Math.Min(left, stackMax) : 1;
                                uint itemUid = c.Inventory.AddInbox(stream, it.Id, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, it.Bound, Role.Flags.ItemEffect.None, (ushort)(stack > 1 ? stack : 0));
                                if (itemUid == 0) break;
                                new Game.MsgServer.PrizeInfo(c, camp.Title, sender, camp.Body, secs, moneySent ? 0 : rw.Silver, moneySent ? 0 : rw.Cps, itemUid, 0, 1);
                                moneySent = true;
                                res.Given = true;
                                left -= stack; mails++;
                            }
                            parts.Add(it.Count - left + " x " + (db.Name ?? "").Replace('~', ' ') + " [" + it.Id + "]" + (it.Bound ? " bound" : "") + (left > 0 ? " (" + left + " NOT GIVEN)" : ""));
                        }
                        if (!moneySent)
                        {
                            new Game.MsgServer.PrizeInfo(c, camp.Title, sender, camp.Body, secs, rw.Silver, rw.Cps, 0, 0, 1);
                            res.Given = true;
                        }
                        if (rw.Silver > 0) parts.Add(rw.Silver + " silver (mail)");
                        if (rw.Cps > 0) parts.Add(rw.Cps + " CPs (mail)");
                        if (rw.BoundCps > 0)
                        {
                            long old = Math.Max(0, c.Player.BoundConquerPoints);
                            c.Player.BoundConquerPoints = (int)Math.Min(int.MaxValue, old + rw.BoundCps);
                            res.Given = true;
                            parts.Add(rw.BoundCps + " bound CPs");
                        }
                        if (rw.ExpPercent > 0 || rw.Levels > 0)
                        {
                            string e = CompExp(c, stream, rw.ExpPercent, rw.Levels);
                            if (e != null) { parts.Add(e); res.Given = true; }
                        }
                    }
                    try { c.SendSysMesage("[" + camp.Title + "] You received a compensation. Open your mailbox to claim it.", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.yellow); } catch (Exception err) { MyConsole.SaveException(err); }
                    SafeSave(c);
                    res.Ok = true;
                }
                catch (Exception e)
                {
                    res.Error = e.Message;
                    MyConsole.SaveException(e);
                }
                res.Text = c.Player.Name + " (" + uid + "): " + string.Join("; ", parts.ToArray());
                return res;
            }
        }

        /// <summary>EXP: percent of the current level's requirement (same level-up loop as op addexp) and/or whole levels. Max level is never passed.</summary>
        private static string CompExp(Client.GameClient c, ServerSockets.Packet stream, int percent, int levels)
        {
            Dictionary<byte, Database.DBLevExp> table;
            if (!Pool.LevelInfo.TryGetValue(Database.DBLevExp.Sort.User, out table)) return "EXP skipped (level table not loaded)";
            int maxLevel = MaxLevel;
            var p = c.Player;
            int old = p.Level;
            if (p.Level >= maxLevel) return "EXP skipped (max level)";
            if (levels > 0)
            {
                int target = Math.Min(maxLevel, p.Level + levels);
                while (p.Level < target) c.UpdateLevel(stream, (ushort)(p.Level + 1));
            }
            Database.DBLevExp row;
            if (percent > 0 && p.Level < maxLevel && table.TryGetValue((byte)p.Level, out row) && row.Experience > 0)
            {
                ulong amount = (ulong)(row.Experience * (double)percent / 100.0);
                p.Experience += amount;
                while (p.Level < maxLevel && table.TryGetValue((byte)p.Level, out row) && row.Experience > 0 && p.Experience >= row.Experience)
                {
                    p.Experience -= row.Experience;
                    c.UpdateLevel(stream, (ushort)(p.Level + 1));
                }
            }
            if (p.Level >= maxLevel) p.Experience = 0;
            c.UpdateRebornLastLevel(stream);
            p.SendUpdate(stream, (long)p.Experience, Game.MsgServer.MsgUpdate.DataType.Experience, false);
            return (levels > 0 ? "+" + levels + " levels " : "") + (percent > 0 ? "+" + percent + "% exp " : "") + "(level " + old + " -> " + p.Level + ")";
        }
    }
}
#endif

namespace Qahr.Mobile.Compensation
{
    using Qahr.Mobile.AdminApi;

    public sealed class CompItem
    {
        public uint Id; public int Count; public bool Bound; public string Name = "";
    }

    public sealed class CompReward
    {
        public List<CompItem> Items = new List<CompItem>();
        public ulong Silver; public uint Cps; public uint BoundCps; public int ExpPercent; public int Levels;

        public bool IsEmpty { get { return Items.Count == 0 && Silver == 0 && Cps == 0 && BoundCps == 0 && ExpPercent == 0 && Levels == 0; } }

        public string Describe()
        {
            var p = new List<string>();
            foreach (var i in Items) p.Add(i.Count + " x " + i.Name + " [" + i.Id + "]" + (i.Bound ? " bound" : ""));
            if (Silver > 0) p.Add(Silver.ToString("N0", CultureInfo.InvariantCulture) + " silver");
            if (Cps > 0) p.Add(Cps.ToString("N0", CultureInfo.InvariantCulture) + " CPs");
            if (BoundCps > 0) p.Add(BoundCps.ToString("N0", CultureInfo.InvariantCulture) + " bound CPs");
            if (ExpPercent > 0) p.Add(ExpPercent + "% exp");
            if (Levels > 0) p.Add("+" + Levels + " levels");
            return string.Join(", ", p.ToArray());
        }

        public Dictionary<string, object> ToJson()
        {
            var items = new List<object>();
            foreach (var i in Items) items.Add(new Dictionary<string, object> { { "id", i.Id }, { "count", i.Count }, { "bound", i.Bound }, { "name", i.Name } });
            return new Dictionary<string, object> { { "items", items }, { "silver", Silver }, { "cps", Cps }, { "boundCps", BoundCps }, { "expPercent", ExpPercent }, { "levels", Levels } };
        }
    }

    public sealed class Campaign
    {
        public string Id = "", Title = "", Body = "", TargetText = "", CreatedBy = "";
        public DateTime Created, Expires;
        public CompReward Reward = new CompReward();
        public bool Cancelled;
        public int Targets, SentNow, Queued, Failed;

        public bool Expired { get { return DateTime.Now >= Expires; } }

        public Dictionary<string, object> ToJson()
        {
            return new Dictionary<string, object>
            {
                { "id", Id }, { "title", Title }, { "body", Body }, { "created", Created }, { "expires", Expires },
                { "reward", Reward.ToJson() }, { "rewardText", Reward.Describe() }, { "target", TargetText }, { "createdBy", CreatedBy },
                { "cancelled", Cancelled }, { "targets", Targets }, { "sentNow", SentNow }, { "queued", Queued }, { "failed", Failed },
            };
        }

        public static Campaign FromJson(Dictionary<string, object> d)
        {
            var a = new AdminArgs(d);
            var c = new Campaign
            {
                Id = a.Str("id", ""), Title = a.Str("title", ""), Body = a.Str("body", ""), TargetText = a.Str("target", ""), CreatedBy = a.Str("createdBy", ""),
                Cancelled = a.Bool("cancelled"), Targets = (int)a.Long("targets"), SentNow = (int)a.Long("sentNow"), Queued = (int)a.Long("queued"), Failed = (int)a.Long("failed"),
            };
            c.Created = ParseTime(a.Str("created"));
            c.Expires = ParseTime(a.Str("expires"));
            var rd = d.ContainsKey("reward") ? d["reward"] as Dictionary<string, object> : null;
            if (rd != null)
            {
                var r = new AdminArgs(rd);
                c.Reward.Silver = r.ULong("silver"); c.Reward.Cps = (uint)r.Long("cps"); c.Reward.BoundCps = (uint)r.Long("boundCps");
                c.Reward.ExpPercent = (int)r.Long("expPercent"); c.Reward.Levels = (int)r.Long("levels");
                var items = r.List("items");
                if (items != null)
                    foreach (var o in items)
                    {
                        var id = o as Dictionary<string, object>; if (id == null) continue;
                        var ia = new AdminArgs(id);
                        c.Reward.Items.Add(new CompItem { Id = (uint)ia.Long("id"), Count = (int)ia.Long("count", 1), Bound = ia.Bool("bound"), Name = ia.Str("name", "") });
                    }
            }
            return c;
        }

        static DateTime ParseTime(string s)
        {
            DateTime t;
            return DateTime.TryParseExact(s ?? "", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t) ? t : DateTime.MinValue;
        }
    }

    public sealed class GrantResult
    {
        /// <summary>true = finished (everything that could be given was given).</summary>
        public bool Ok;
        /// <summary>true = something reached the player (mail created / currency given) - a failed grant with Given=false is undone.</summary>
        public bool Given;
        public string Error, Text;
    }

    /// <summary>The game side (AdminApiServer.CompGameSink); the harness uses a fake one.</summary>
    public interface ICompSink
    {
        bool ItemInfo(uint id, out string name, out int stack);
        bool IsReady(uint uid);
        string NameOf(uint uid);
        GrantResult Grant(uint uid, Campaign c);
    }

    public enum Outcome { Delivered, Queued, AlreadyClaimed, Cancelled, Expired, DbDown, Failed, NotReady }

    public static class CompEngine
    {
        public const string Ev = "Compensation";
        public const int MaxItemLines = 10, MaxMails = 40;
        public const ulong MaxSilver = 1000000000UL;
        public const uint MaxCps = 100000, MaxBoundCps = 1000000;
        public const int MaxExpPercent = 1000, MaxLevels = 5;

#if COMP_HARNESS
        public static ICompSink Sink;
        public static bool AutoPump = false;
#else
        public static ICompSink Sink = new Qahr.Mobile.AdminApi.AdminApiServer.CompGameSink();
        public static bool AutoPump = true;
#endif
        static readonly object _fileLock = new object();
        static readonly object _watchLock = new object();
        static readonly Dictionary<uint, long> _watch = new Dictionary<uint, long>();
        static Timer _timer;
        static int _pumping;

        static string Db { get { string d = Program.ServerConfig.DbLocation ?? ""; return d; } }
        public static string Folder { get { return Path.Combine(Db, "Compensation"); } }
        static string CampDir { get { return Path.Combine(Folder, "campaigns"); } }
        static long Tick { get { return Environment.TickCount & 0x7FFFFFFF; } }

        // ------------------------------------------------------------------ log
        public static void Log(string s)
        {
            try
            {
                lock (_fileLock)
                {
                    Directory.CreateDirectory(Folder);
                    File.AppendAllText(Path.Combine(Folder, "log.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " | " + (s ?? "").Replace("\r", " ").Replace("\n", " ") + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }

        // ------------------------------------------------------------------ validation
        public static bool ValidId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length < 3 || id.Length > 40) return false;
            foreach (char ch in id) if (!(ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '_' || ch == '-')) return false;
            return true;
        }

        /// <summary>The mailbox stores title/body as single bytes: printable ASCII only.</summary>
        static bool Ascii(string s) { foreach (char ch in s) if (ch < 32 || ch > 126) return false; return true; }

        public static CompReward ParseReward(AdminArgs r)
        {
            var rw = new CompReward();
            ulong silver = r.ULong("silver"), cps = r.ULong("cps"), bcps = r.ULong("boundCps");
            long exp = r.Long("expPercent"), lv = r.Long("levels");
            if (silver > MaxSilver) throw new AdminError("'silver' must be 0-" + MaxSilver.ToString("N0", CultureInfo.InvariantCulture));
            if (cps > MaxCps) throw new AdminError("'cps' must be 0-" + MaxCps.ToString("N0", CultureInfo.InvariantCulture));
            if (bcps > MaxBoundCps) throw new AdminError("'boundCps' must be 0-" + MaxBoundCps.ToString("N0", CultureInfo.InvariantCulture));
            if (exp < 0 || exp > MaxExpPercent) throw new AdminError("'expPercent' must be 0-" + MaxExpPercent);
            if (lv < 0 || lv > MaxLevels) throw new AdminError("'levels' must be 0-" + MaxLevels);
            rw.Silver = silver; rw.Cps = (uint)cps; rw.BoundCps = (uint)bcps; rw.ExpPercent = (int)exp; rw.Levels = (int)lv;
            var items = r.List("items");
            if (items != null)
                foreach (var o in items)
                {
                    var d = o as Dictionary<string, object>;
                    if (d == null) throw new AdminError("each item must be an object {id,count,bound}");
                    var ia = new AdminArgs(d);
                    long id = ia.Long("id"), count = ia.Long("count", 1);
                    if (id <= 0 || id > uint.MaxValue) throw new AdminError("bad item id " + id);
                    rw.Items.Add(new CompItem { Id = (uint)id, Count = (int)Math.Max(-1, Math.Min(int.MaxValue, count)), Bound = ia.Bool("bound") });
                }
            return rw;
        }

        /// <summary>null = ok. Fills item names from the server's item table.</summary>
        public static string Validate(Campaign c)
        {
            if (!ValidId(c.Id)) return "'id' must be 3-40 characters: letters, digits, _ or -";
            if (c.Title.Length < 1 || c.Title.Length > 60 || !Ascii(c.Title)) return "'title' must be 1-60 characters, English letters/digits/punctuation only (the game mailbox has no Arabic)";
            if (c.Body.Length < 1 || c.Body.Length > 500 || !Ascii(c.Body)) return "'body' must be 1-500 characters, English letters/digits/punctuation only (the game mailbox has no Arabic)";
            var rw = c.Reward;
            if (rw.IsEmpty) return "the reward is empty";
            if (rw.Items.Count > MaxItemLines) return "at most " + MaxItemLines + " item lines";
            if (Sink == null) return "server not ready";
            foreach (var it in rw.Items)
            {
                string name; int stack;
                if (!Sink.ItemInfo(it.Id, out name, out stack)) return "item id " + it.Id + " does not exist in the server's item table";
                it.Name = name;
                int max = stack > 1 ? Math.Min(10 * stack, 10000) : 10;
                if (it.Count < 1 || it.Count > max) return "item " + it.Id + " (" + name + "): count must be 1-" + max;
            }
            if (MailCount(rw) > MaxMails) return "too many mails per player (" + MailCount(rw) + ", max " + MaxMails + ") - use fewer stacks";
            return null;
        }

        /// <summary>Mails one player receives: one per item stack, or one for silver/CPs alone.</summary>
        public static int MailCount(CompReward rw)
        {
            int n = 0;
            foreach (var it in rw.Items)
            {
                string name; int stack = 1;
                if (Sink != null) Sink.ItemInfo(it.Id, out name, out stack);
                stack = Math.Max(1, stack);
                n += (it.Count + stack - 1) / stack;
            }
            if (n == 0 && (rw.Silver > 0 || rw.Cps > 0)) n = 1;
            return n;
        }

        public static string DescribeTarget(AdminArgs t)
        {
            var p = new List<string>();
            string scope = (t.Str("scope") ?? "all").ToLowerInvariant();
            if (scope == "list")
            {
                int n = (t.List("names") != null ? t.List("names").Count : 0) + (t.List("uids") != null ? t.List("uids").Count : 0);
                var names = t.List("names");
                p.Add("list of " + n + (names != null && names.Count > 0 && names.Count <= 5 ? " (" + string.Join(", ", names.Select(x => (x ?? "").ToString()).ToArray()) + ")" : ""));
            }
            else p.Add(scope == "online" ? "online players" : "all accounts");
            if (t.Long("minLevel") > 0) p.Add("level >= " + t.Long("minLevel"));
            if (t.Long("maxLevel") > 0) p.Add("level <= " + t.Long("maxLevel"));
            if (t.Long("minReborn") > 0) p.Add("reborn >= " + t.Long("minReborn"));
            if (t.Long("minVip") > 0) p.Add("VIP >= " + t.Long("minVip"));
            if (!string.IsNullOrWhiteSpace(t.Str("since"))) p.Add("seen since " + t.Str("since").Trim());
            return string.Join(", ", p.ToArray());
        }

        // ------------------------------------------------------------------ campaign files
        static string CampFile(string id) { return Path.Combine(CampDir, id + ".json"); }
        static string RecipFile(string id) { return Path.Combine(CampDir, id + ".recipients.txt"); }

        public static Campaign Load(string id)
        {
            if (!ValidId(id)) return null;
            try
            {
                lock (_fileLock)
                {
                    string f = CampFile(id);
                    if (!File.Exists(f)) return null;
                    var d = AdminJson.Parse(File.ReadAllText(f, Encoding.UTF8)) as Dictionary<string, object>;
                    return d == null ? null : Campaign.FromJson(d);
                }
            }
            catch (Exception e) { Log("ERROR reading campaign " + id + ": " + e.Message); return null; }
        }

        public static List<Campaign> LoadAll()
        {
            var list = new List<Campaign>();
            try
            {
                if (!Directory.Exists(CampDir)) return list;
                foreach (var f in Directory.GetFiles(CampDir, "*.json"))
                {
                    var c = Load(Path.GetFileNameWithoutExtension(f));
                    if (c != null) list.Add(c);
                }
            }
            catch (Exception err) { MyConsole.SaveException(err); }
            return list.OrderByDescending(c => c.Created).ToList();
        }

        public static void Save(Campaign c)
        {
            lock (_fileLock)
            {
                Directory.CreateDirectory(CampDir);
                string f = CampFile(c.Id), tmp = f + ".tmp";
                File.WriteAllText(tmp, AdminJson.Write(c.ToJson()), Encoding.UTF8);
                if (File.Exists(f)) File.Replace(tmp, f, null); else File.Move(tmp, f);
            }
        }

        /// <summary>Real MySQL round trip (EnsureSchema is cached, so read uid 0 fresh is not enough): a claim probe that never changes anything visible.</summary>
        public static bool DbProbe()
        {
            try { return global::Qahr.Mobile.EventData.EventStore.IsAvailable(0) && ProbeWrite(); } catch (Exception err) { MyConsole.SaveException(err); return false; }
        }
        static bool ProbeWrite()
        {
            // the "_probe" claim exists after the first call; TryClaimOnce returns false then without touching MySQL,
            // so probe with a time-bucketed key (one row per minute at most, uid 0 / event "Compensation").
            try
            {
                string k = "probe:" + DateTime.Now.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
                if (global::Qahr.Mobile.EventData.EventStore.IsClaimed(0, Ev, k)) return _lastProbeOk;
                bool ok = global::Qahr.Mobile.EventData.EventStore.TryClaimOnce(0, Ev, k);
                if (ok) global::Qahr.Mobile.EventData.EventStore.Unclaim(0, Ev, k);
                _lastProbeOk = ok;
                return ok;
            }
            catch { _lastProbeOk = false; return false; }
        }
        static volatile bool _lastProbeOk;

        /// <summary>Registers a NEW campaign: the id is claimed in MySQL (uid 0) first -> a second send with the same id is refused,
        /// and MySQL down = refused. null = ok.</summary>
        public static string Create(Campaign c)
        {
            string err = Validate(c);
            if (err != null) return err;
            if (Load(c.Id) != null) return "campaign '" + c.Id + "' already exists (every campaign id is sent once)";
            if (!global::Qahr.Mobile.EventData.EventStore.TryClaimOnce(0, Ev, "c:" + c.Id))
            {
                bool exists = false;
                try { exists = global::Qahr.Mobile.EventData.EventStore.IsAvailable(0) && global::Qahr.Mobile.EventData.EventStore.IsClaimed(0, Ev, "c:" + c.Id); } catch (Exception ex) { MyConsole.SaveException(ex); }
                return exists ? "campaign '" + c.Id + "' was already sent (recorded in MySQL)" : "MySQL is unavailable - nothing was sent (compensation needs the database to stay once-only)";
            }
            try { Save(c); }
            catch (Exception e)
            {
                global::Qahr.Mobile.EventData.EventStore.Unclaim(0, Ev, "c:" + c.Id);
                return "cannot write the campaign file: " + e.Message;
            }
            Log("CAMPAIGN " + c.Id + " created: \"" + c.Title + "\" | " + c.Reward.Describe() + " | " + c.TargetText + " | expires " + c.Expires.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
            return null;
        }

        /// <summary>Delivers to every ready player, queues the rest. Returns counts.</summary>
        public static Dictionary<string, object> SendAll(Campaign c, List<KeyValuePair<uint, string>> targets)
        {
            int delivered = 0, queued = 0, failed = 0, already = 0, dbFails = 0;
            var failedList = new List<object>();
            var sb = new StringBuilder();
            foreach (var t in targets)
            {
                sb.Append(t.Key.ToString(CultureInfo.InvariantCulture)).Append('\t').Append((t.Value ?? "").Replace('\t', ' ')).Append("\r\n");
                Outcome o;
                if (dbFails >= 5) o = Outcome.DbDown; // MySQL went away during the run: stop hammering it
                else o = DeliverOrQueue(t.Key, c);
                switch (o)
                {
                    case Outcome.Delivered: delivered++; dbFails = 0; break;
                    case Outcome.Queued: queued++; dbFails = 0; break;
                    case Outcome.AlreadyClaimed: already++; break;
                    case Outcome.DbDown: dbFails++; failed++; if (failedList.Count < 200) failedList.Add(t.Value + " (" + t.Key + "): MySQL unavailable"); break;
                    default: failed++; if (failedList.Count < 200) failedList.Add(t.Value + " (" + t.Key + "): " + o); break;
                }
            }
            try { lock (_fileLock) File.WriteAllText(RecipFile(c.Id), sb.ToString(), Encoding.UTF8); } catch (Exception e) { Log("ERROR recipients file " + c.Id + ": " + e.Message); }
            c.SentNow = delivered; c.Queued = queued; c.Failed = failed;
            try { Save(c); } catch (Exception err) { MyConsole.SaveException(err); }
            Log("SEND " + c.Id + ": " + targets.Count + " targets, " + delivered + " delivered, " + queued + " queued, " + already + " already, " + failed + " failed");
            return new Dictionary<string, object> { { "id", c.Id }, { "targets", targets.Count }, { "delivered", delivered }, { "queued", queued }, { "already", already }, { "failed", failed }, { "failedList", failedList } };
        }

        public static Outcome DeliverOrQueue(uint uid, Campaign c)
        {
            if (Sink != null && Sink.IsReady(uid))
            {
                var o = Deliver(uid, c);
                if (o != Outcome.NotReady && o != Outcome.Failed) return o; // Failed = nothing given, pending again -> report as queued
            }
            return Queue(uid, c);
        }

        /// <summary>Marks the player as pending (written to MySQL now). Watched at once if they are online but still loading.</summary>
        public static Outcome Queue(uint uid, Campaign c)
        {
            try
            {
                if (global::Qahr.Mobile.EventData.EventStore.IsAvailable(uid) && global::Qahr.Mobile.EventData.EventStore.IsClaimed(uid, Ev, c.Id)) return Outcome.AlreadyClaimed;
                global::Qahr.Mobile.EventData.EventStore.Edit(uid, Ev, s => { s.Flags.Add("p:" + c.Id); s.Flags.Remove("x:" + c.Id); }, true);
            }
            catch (Exception e) { Log("QUEUE FAILED " + c.Id + " uid " + uid + ": " + e.Message); return Outcome.DbDown; }
            Watch(uid);
            return Outcome.Queued;
        }

        /// <summary>The one place a compensation is given: ready check -> claim in MySQL (once) -> grant -> log.</summary>
        public static Outcome Deliver(uint uid, Campaign c)
        {
            if (c == null) return Outcome.Failed;
            if (c.Cancelled) return Outcome.Cancelled;
            if (c.Expired) return Outcome.Expired;
            if (Sink == null || !Sink.IsReady(uid)) return Outcome.NotReady;
            if (!global::Qahr.Mobile.EventData.EventStore.IsAvailable(uid)) { Log("REFUSED " + c.Id + " uid " + uid + ": MySQL unavailable (stays pending)"); return Outcome.DbDown; }
            var sec = global::Qahr.Mobile.EventData.EventStore.Section(uid, Ev);
            if (sec.Flags.Contains("x:" + c.Id)) return Outcome.Cancelled;
            if (!global::Qahr.Mobile.EventData.EventStore.TryClaimOnce(uid, Ev, c.Id))
            {
                if (global::Qahr.Mobile.EventData.EventStore.IsClaimed(uid, Ev, c.Id))
                {
                    ClearPending(uid, c.Id);
                    return Outcome.AlreadyClaimed;
                }
                Log("REFUSED " + c.Id + " uid " + uid + ": claim could not be recorded (MySQL) - nothing given, stays pending");
                return Outcome.DbDown;
            }
            ClearPending(uid, c.Id);
            GrantResult g;
            try { g = Sink.Grant(uid, c); }
            catch (Exception e) { g = new GrantResult { Ok = false, Given = false, Error = e.Message }; }
            if (!g.Ok && !g.Given)
            {
                // nothing reached the player: undo the claim and keep it pending (retried at the next login)
                bool undone = global::Qahr.Mobile.EventData.EventStore.Unclaim(uid, Ev, c.Id);
                try { global::Qahr.Mobile.EventData.EventStore.Edit(uid, Ev, s => s.Flags.Add("p:" + c.Id), true); } catch (Exception err) { MyConsole.SaveException(err); }
                Log("GRANT FAILED " + c.Id + " uid " + uid + " (nothing given, " + (undone ? "claim undone, pending again" : "CLAIM NOT UNDONE") + "): " + g.Error);
                return Outcome.Failed;
            }
            Log("GRANT " + c.Id + " -> " + (g.Text ?? ("uid " + uid)) + (g.Ok ? "" : " | PARTIAL: " + g.Error));
            return Outcome.Delivered;
        }

        static void ClearPending(uint uid, string id)
        {
            try { global::Qahr.Mobile.EventData.EventStore.Edit(uid, Ev, s => { s.Flags.Remove("p:" + id); }, true); }
            catch (Exception e) { Log("note: could not clear pending flag " + id + " uid " + uid + " (" + e.Message + ") - harmless, the claim blocks a second grant"); }
        }

        public static List<string> PendingOf(uint uid)
        {
            var s = global::Qahr.Mobile.EventData.EventStore.Section(uid, Ev);
            return s.Flags.Where(f => f.StartsWith("p:", StringComparison.Ordinal)).Select(f => f.Substring(2)).ToList();
        }

        // ------------------------------------------------------------------ login hook + pump
        /// <summary>[hook] EventStore.OnLogin (during LoadCharacterCore): cheap, never throws, never grants here.</summary>
        public static void OnLogin(uint uid)
        {
            try
            {
                var s = global::Qahr.Mobile.EventData.EventStore.Section(uid, Ev);
                if (s.Flags.Any(f => f.StartsWith("p:", StringComparison.Ordinal))) Watch(uid);
            }
            catch (Exception err) { MyConsole.SaveException(err); }
        }

        public static int WatchCount { get { lock (_watchLock) return _watch.Count; } }

        static void Watch(uint uid)
        {
            lock (_watchLock) _watch[uid] = Tick;
            if (AutoPump && _timer == null)
                lock (_watchLock) if (_timer == null) _timer = new Timer(_ => { try { Pump(); } catch (Exception err) { MyConsole.SaveException(err); } }, null, 2000, 2000);
        }

        /// <summary>Delivers the pending campaigns of watched players whose login finished. Drops a watch after 3 minutes (next login re-adds it).</summary>
        public static int Pump()
        {
            if (Interlocked.Exchange(ref _pumping, 1) == 1) return 0;
            int n = 0;
            try
            {
                KeyValuePair<uint, long>[] snap;
                lock (_watchLock) snap = _watch.ToArray();
                long now = Tick;
                foreach (var w in snap)
                {
                    bool ready = Sink != null && Sink.IsReady(w.Key);
                    if (!ready)
                    {
                        if (now - w.Value > 180000 || now < w.Value) lock (_watchLock) _watch.Remove(w.Key);
                        continue;
                    }
                    bool retry = false;
                    foreach (var id in PendingOf(w.Key))
                    {
                        var c = Load(id);
                        if (c == null) { Log("pending " + id + " uid " + w.Key + ": campaign file missing - skipped"); continue; }
                        var o = Deliver(w.Key, c);
                        if (o == Outcome.Delivered) n++;
                        else if (o == Outcome.DbDown || o == Outcome.NotReady) retry = true;
                    }
                    if (!retry) lock (_watchLock) _watch.Remove(w.Key);
                }
            }
            finally { Interlocked.Exchange(ref _pumping, 0); }
            return n;
        }

        // ------------------------------------------------------------------ history
        sealed class Counts { public int Delivered, Pending, Cancelled; }

        static Dictionary<string, Counts> CountAll(out Dictionary<uint, global::Qahr.Mobile.EventData.EventSection> all)
        {
            all = global::Qahr.Mobile.EventData.EventStore.All(Ev);
            var res = new Dictionary<string, Counts>(StringComparer.Ordinal);
            Func<string, Counts> get = id => { Counts x; if (!res.TryGetValue(id, out x)) res[id] = x = new Counts(); return x; };
            foreach (var kv in all)
            {
                var s = kv.Value;
                foreach (var cl in s.Claims.Keys) get(cl).Delivered++;
                foreach (var f in s.Flags)
                {
                    if (f.StartsWith("p:", StringComparison.Ordinal) && !s.Claims.ContainsKey(f.Substring(2))) get(f.Substring(2)).Pending++;
                    else if (f.StartsWith("x:", StringComparison.Ordinal) && !s.Claims.ContainsKey(f.Substring(2))) get(f.Substring(2)).Cancelled++;
                }
            }
            return res;
        }

        public static List<object> ListJson()
        {
            Dictionary<uint, global::Qahr.Mobile.EventData.EventSection> all;
            var counts = CountAll(out all);
            var list = new List<object>();
            foreach (var c in LoadAll())
            {
                Counts k; counts.TryGetValue(c.Id, out k); k = k ?? new Counts();
                var d = c.ToJson();
                d["delivered"] = k.Delivered;
                d["pending"] = c.Expired || c.Cancelled ? 0 : k.Pending;
                d["expiredPending"] = c.Expired && !c.Cancelled ? k.Pending : 0;
                d["cancelledCount"] = k.Cancelled + (c.Cancelled ? k.Pending : 0);
                d["expired"] = c.Expired;
                list.Add(d);
            }
            return list;
        }

        public static Dictionary<string, object> DetailJson(string id, int limit)
        {
            var c = Load(id);
            if (c == null) return null;
            var all = global::Qahr.Mobile.EventData.EventStore.All(Ev);
            var rows = new List<object>();
            int delivered = 0, pending = 0, cancelled = 0, expired = 0, other = 0;
            var lines = new List<string>();
            try { lock (_fileLock) if (File.Exists(RecipFile(id))) lines = File.ReadAllLines(RecipFile(id), Encoding.UTF8).ToList(); } catch (Exception err) { MyConsole.SaveException(err); }
            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                uint uid;
                if (parts.Length < 1 || !uint.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out uid)) continue;
                string name = parts.Length > 1 ? parts[1] : "";
                global::Qahr.Mobile.EventData.EventSection s;
                all.TryGetValue(uid, out s);
                string status; object at = null;
                long t;
                if (s != null && s.Claims.TryGetValue(id, out t)) { status = "delivered"; at = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(t).ToLocalTime(); delivered++; }
                else if (s != null && s.Flags.Contains("x:" + id)) { status = "cancelled"; cancelled++; }
                else if (s != null && s.Flags.Contains("p:" + id)) { if (c.Cancelled) { status = "cancelled"; cancelled++; } else if (c.Expired) { status = "expired"; expired++; } else { status = "pending"; pending++; } }
                else { status = "failed"; other++; }
                if (rows.Count < limit) rows.Add(new Dictionary<string, object> { { "uid", uid }, { "name", name }, { "status", status }, { "at", at } });
            }
            var d = c.ToJson();
            d["recipients"] = rows;
            d["total"] = lines.Count;
            d["delivered"] = delivered; d["pending"] = pending; d["cancelledCount"] = cancelled; d["expiredPending"] = expired; d["failedCount"] = other;
            d["expired"] = c.Expired;
            return d;
        }

        /// <summary>Cancels pending (not yet delivered) recipients. uids null = the whole campaign (also blocks later deliveries).</summary>
        public static Dictionary<string, object> Cancel(string id, List<uint> uids, out string error)
        {
            error = null;
            var c = Load(id);
            if (c == null) { error = "campaign '" + id + "' not found"; return null; }
            if (!global::Qahr.Mobile.EventData.EventStore.IsAvailable(0)) { error = "MySQL is unavailable - nothing cancelled"; return null; }
            var all = global::Qahr.Mobile.EventData.EventStore.All(Ev);
            if (uids == null)
            {
                uids = all.Where(kv => kv.Value.Flags.Contains("p:" + id)).Select(kv => kv.Key).ToList();
                c.Cancelled = true;
                try { Save(c); } catch (Exception e) { error = "cannot write the campaign file: " + e.Message; return null; }
            }
            int cancelled = 0, already = 0, notPending = 0, failed = 0;
            foreach (var uid in uids)
            {
                try
                {
                    string r = global::Qahr.Mobile.EventData.EventStore.Edit(uid, Ev, s =>
                    {
                        if (s.Claims.ContainsKey(id)) return Tuple.Create(false, "delivered");
                        if (!s.Flags.Contains("p:" + id)) return Tuple.Create(false, "none");
                        s.Flags.Remove("p:" + id); s.Flags.Add("x:" + id);
                        return Tuple.Create(true, "ok");
                    }, true);
                    if (r == "ok") cancelled++; else if (r == "delivered") already++; else notPending++;
                }
                catch (Exception e) { failed++; Log("CANCEL FAILED " + id + " uid " + uid + ": " + e.Message); }
            }
            Log("CANCEL " + id + (c.Cancelled ? " (whole campaign)" : "") + ": " + cancelled + " cancelled, " + already + " already delivered, " + notPending + " not pending, " + failed + " failed");
            return new Dictionary<string, object> { { "cancelled", cancelled }, { "alreadyDelivered", already }, { "notPending", notPending }, { "failed", failed }, { "campaignCancelled", c.Cancelled } };
        }
    }
}
