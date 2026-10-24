// ============================================================================
//  [Festivals] data side: one folder per festival under Data\Festivals\<Key>\ (schema: _schema.json).
//
//      festival.json   key, names, enabled, schedule (manual | range | yearly | weekly + hours), announcements, maps
//      npcs.txt        Npcs.txt lines (ids 8800000-8899999, or [FestivalTrack] the official id a festival quest names in
//                      Questinfo.ini so the client quest tracker finds the NPC), placed at START, removed at END
//      monsters\*.ini  [cq_monstertype] files (ids 990000-999999), registered at START, removed at END
//      spawns.json     spawn boxes (monster, map, x, y, w, h, count, respawn)
//      quests.json     QuestEngine overlay (same schema as QuestEngine\quests.json), active only while running;
//                      its "instances" may list "npcs" that are spawned inside every private copy (own ids, same rules)
//      rewards.json    npcText + exchanges (cost -> give, once per character unless repeat)
//
//  FestivalLoader.LoadAll() parses every folder (skipping "_*"), validates each one on its own and then across
//  festivals (unique NPC / monster / quest ids). A festival with an error is never started; nothing here throws.
//  FestivalSchedule answers "is it due now" in SERVER LOCAL TIME and finds the next start / end.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Qahr.Mobile.Festivals
{
    public enum ScheduleType : byte { Manual, Range, Yearly, Weekly }

    /// <summary>[Festivals] when a festival runs (server local time, minute resolution).</summary>
    public sealed class FestivalSchedule
    {
        public ScheduleType Type = ScheduleType.Manual;
        public DateTime RangeStart, RangeEnd;              // range: [start, end)
        public int YStartMonth, YStartDay, YStartMin;      // yearly start (minutes of the day)
        public int YEndMonth, YEndDay, YEndMin = 1440;     // yearly end (exclusive; 1440 = end of that day)
        public bool[] Days = new bool[7];                  // weekly: DayOfWeek (0 = Sunday)
        public List<int[]> Hours = new List<int[]>();      // daily windows [startMin, endMin) (end may be <= start: crosses midnight)

        public bool IsDue(DateTime now)
        {
            switch (Type)
            {
                case ScheduleType.Manual: return false;
                case ScheduleType.Range: if (now < RangeStart || now >= RangeEnd) return false; break;
                case ScheduleType.Yearly: if (!InYearly(now)) return false; break;
            }
            if (Hours.Count == 0)
                return Type != ScheduleType.Weekly || Days[(int)now.DayOfWeek];
            foreach (var w in Hours)
            {
                for (int back = 0; back <= 1; back++)
                {
                    DateTime day = now.Date.AddDays(-back);
                    if (Type == ScheduleType.Weekly && !Days[(int)day.DayOfWeek]) continue;
                    DateTime ws = day.AddMinutes(w[0]);
                    int endMin = w[1] <= w[0] ? w[1] + 1440 : w[1];
                    DateTime we = day.AddMinutes(endMin);
                    if (now >= ws && now < we) return true;
                }
            }
            return false;
        }

        static DateTime YDate(int year, int month, int day, int minutes)
        {
            int dim = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, Math.Min(day, dim)).AddMinutes(minutes);
        }
        bool InYearly(DateTime now)
        {
            for (int y = now.Year - 1; y <= now.Year; y++)
            {
                DateTime s = YDate(y, YStartMonth, YStartDay, YStartMin);
                DateTime e = YDate(y, YEndMonth, YEndDay, 0).AddMinutes(YEndMin);
                if (e <= s) e = YDate(y + 1, YEndMonth, YEndDay, 0).AddMinutes(YEndMin);
                if (now >= s && now < e) return true;
            }
            return false;
        }

        /// <summary>The first minute after <paramref name="now"/> where IsDue changes (searched up to 400 days); null = never.</summary>
        public DateTime? NextChange(DateTime now, out bool becomesDue)
        {
            becomesDue = false;
            if (Type == ScheduleType.Manual) return null;
            DateTime t = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            bool cur = IsDue(now);
            if (Type == ScheduleType.Range && t >= RangeEnd) return null;
            DateTime limit = t.AddDays(400);
            for (DateTime m = t.AddMinutes(1); m < limit; m = m.AddMinutes(1))
            {
                if (IsDue(m) != cur) { becomesDue = !cur; return m; }
                if (Type == ScheduleType.Range && m >= RangeEnd && !cur) return null;
            }
            return null;
        }

        public override string ToString()
        {
            string h = Hours.Count == 0 ? "" : " hours " + string.Join(",", Hours.Select(w => Hm(w[0]) + "-" + Hm(w[1])));
            switch (Type)
            {
                case ScheduleType.Range: return "range " + RangeStart.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + " -> " + RangeEnd.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + h;
                case ScheduleType.Yearly: return "yearly " + YStartMonth.ToString("00") + "-" + YStartDay.ToString("00") + " " + Hm(YStartMin) + " -> " + YEndMonth.ToString("00") + "-" + YEndDay.ToString("00") + " " + Hm(YEndMin) + h;
                case ScheduleType.Weekly: return "weekly " + string.Join(",", Enumerable.Range(0, 7).Where(i => Days[i]).Select(i => ((DayOfWeek)i).ToString().Substring(0, 3))) + h;
                default: return "manual";
            }
        }
        static string Hm(int m) { return (m / 60).ToString("00") + ":" + (m % 60).ToString("00"); }
    }

    public sealed class FestivalNpc
    {
        public uint Uid; public byte Type; public uint Mesh; public uint Map; public ushort X, Y; public bool Dynamic; public string Name = ""; public int Line;
        public override string ToString() { return Uid + (Name.Length > 0 ? " " + Name : "") + " @" + Map + " (" + X + "," + Y + ")"; }
    }

    public sealed class FestivalSpawn
    {
        public string Monster = "";     // as written (name or id)
        public uint Map; public ushort X, Y, W = 10, H = 10;
        public int Count = 5, Respawn = 30;
        public override string ToString() { return Monster + " x" + Count + " @" + Map + " (" + X + "," + Y + " " + W + "x" + H + ") respawn " + Respawn + "s"; }
    }

    public sealed class FestivalItem { public uint Id; public ushort Count = 1; public bool Bound; }

    public sealed class FestivalExchange
    {
        public string Id = ""; public uint Npc; public string TextAr = "", TextEn = "";
        public List<FestivalItem> CostItems = new List<FestivalItem>(); public ulong CostSilver; public uint CostCps;
        public List<FestivalItem> GiveItems = new List<FestivalItem>(); public ulong GiveExp, GiveSilver; public uint GiveCps;
        public string Repeat = "once";  // once | daily | festival | unlimited
        public ushort MinLevel = 1;
        public bool HasCost { get { return CostItems.Count > 0 || CostSilver > 0 || CostCps > 0; } }
        public string DescribeCost() { var p = new List<string>(); foreach (var i in CostItems) p.Add(QuestEngine.QuestGrammar.ItemName(i.Id) + " x" + i.Count); if (CostSilver > 0) p.Add(CostSilver + " silver"); if (CostCps > 0) p.Add(CostCps + " CPs"); return p.Count == 0 ? "free" : string.Join(", ", p); }
        public string DescribeGive() { var p = new List<string>(); foreach (var i in GiveItems) p.Add(QuestEngine.QuestGrammar.ItemName(i.Id) + (i.Count > 1 ? " x" + i.Count : "")); if (GiveExp > 0) p.Add(GiveExp + " EXP"); if (GiveSilver > 0) p.Add(GiveSilver + " silver"); if (GiveCps > 0) p.Add(GiveCps + " CPs"); return p.Count == 0 ? "-" : string.Join(", ", p); }
    }

    /// <summary>[Festivals] one festival folder, parsed and validated.</summary>
    public sealed class FestivalDef
    {
        public string Key = "", Folder = "";
        public string NameAr = "", NameEn = "";
        public bool Enabled = true;
        public FestivalSchedule Schedule = new FestivalSchedule();
        public string AnnounceAr, AnnounceEn, EndAr, EndEn, Notes = "";
        /// <summary>[LuaUi] optional client Lua calls (festival.json icon_show / icon_hide / icon_click): the top icon shown on
        /// start + login, hidden on stop; icon_click = the callback the icon sends ("{msgId}</S>{func}", for routing).</summary>
        public string IconShow, IconHide, IconClick;
        /// <summary>[LuaUi] festival.json highlight_acts: the client's Highlight Events pages (luaui fairygui\window\config\actinfo
        /// ActInfo ids) listed in FuiActCenter_Open while this festival runs.</summary>
        public List<string> HighlightActs = new List<string>();
        public List<uint> Maps = new List<uint>();
        public List<FestivalNpc> Npcs = new List<FestivalNpc>();
        public Dictionary<uint, int> OfficialNpcLines = new Dictionary<uint, int>();   // [FestivalTrack] npcs.txt id outside 88xxxxx -> line
        public List<uint> InstanceNpcs = new List<uint>();                              // [InstanceNpc] ids spawned inside its instances (quests.json)
        public List<Game.MsgMonster.MonsterFamily> Monsters = new List<Game.MsgMonster.MonsterFamily>();
        public List<FestivalSpawn> Spawns = new List<FestivalSpawn>();
        public Dictionary<string, object> QuestsRoot;
        public List<uint> QuestIds = new List<uint>();
        public Dictionary<uint, string[]> NpcText = new Dictionary<uint, string[]>();   // npc -> {ar, en}
        public List<FestivalExchange> Exchanges = new List<FestivalExchange>();
        public List<string> Errors = new List<string>();
        public List<string> Warnings = new List<string>();
        public string Hash = "";
        public bool Valid { get { return Errors.Count == 0; } }

        public string StartAr { get { return string.IsNullOrWhiteSpace(AnnounceAr) ? "بدأ مهرجان " + NameAr + "!" : AnnounceAr; } }
        public string StartEn { get { return string.IsNullOrWhiteSpace(AnnounceEn) ? "The " + NameEn + " festival has started!" : AnnounceEn; } }
        public string StopAr { get { return string.IsNullOrWhiteSpace(EndAr) ? "انتهى مهرجان " + NameAr + "." : EndAr; } }
        public string StopEn { get { return string.IsNullOrWhiteSpace(EndEn) ? "The " + NameEn + " festival has ended." : EndEn; } }
    }

    /// <summary>[Festivals] reads and validates Data\Festivals\*\.</summary>
    public static class FestivalLoader
    {
        public const uint NpcIdMin = 8800000, NpcIdMax = 8899999;
        public const uint MonsterIdMin = 990000, MonsterIdMax = 999999;
        public const int MaxMonstersPerFestival = 500;
        static readonly Regex KeyRx = new Regex(@"^[A-Za-z0-9_\-]{2,40}$", RegexOptions.Compiled);
        static readonly Regex ExIdRx = new Regex(@"^[A-Za-z0-9_\-]{1,40}$", RegexOptions.Compiled);

        public static string Root { get { return Path.Combine(Program.ServerConfig.DbLocation, "Festivals"); } }

        /// <summary>All festival folders (valid and invalid). <paramref name="festivalTypes"/> = monster type ids that are
        /// in Pool.MonsterFamilies only because a running festival registered them (not counted as "already exists").</summary>
        public static List<FestivalDef> LoadAll(ICollection<uint> festivalTypes)
        {
            var list = new List<FestivalDef>();
            string root = Root;
            if (!Directory.Exists(root)) return list;
            foreach (string dir in Directory.GetDirectories(root).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                string name = Path.GetFileName(dir);
                if (name.StartsWith("_") || name.StartsWith(".")) continue;
                // an event's own data (config, player state) also lives in Festivals\<event>; only folders with
                // a festival.json are festival definitions
                if (!File.Exists(Path.Combine(dir, "festival.json"))) continue;
                FestivalDef def;
                try { def = LoadOne(dir, festivalTypes); }
                catch (Exception e) { def = new FestivalDef { Key = name, Folder = dir }; def.Errors.Add("load failed: " + e.Message); }
                list.Add(def);
            }
            CrossValidate(list);
            return list;
        }

        // ------------------------------------------------------------ one folder
        public static FestivalDef LoadOne(string dir, ICollection<uint> festivalTypes)
        {
            string folder = Path.GetFileName(dir);
            var def = new FestivalDef { Key = folder, Folder = dir };
            def.Hash = FolderHash(dir);
            if (!KeyRx.IsMatch(folder)) def.Errors.Add("folder name '" + folder + "' is not a valid key (letters, digits, _ and -, 2-40 chars)");

            // festival.json
            string fj = Path.Combine(dir, "festival.json");
            if (!File.Exists(fj)) { def.Errors.Add("festival.json is missing"); return def; }
            Dictionary<string, object> f;
            try { f = ReadJson(fj); }
            catch (Exception e) { def.Errors.Add("festival.json does not parse: " + e.Message); return def; }
            if (f == null) { def.Errors.Add("festival.json is not a JSON object"); return def; }
            string key = Str(f, "key");
            if (string.IsNullOrWhiteSpace(key)) def.Errors.Add("festival.json: 'key' is missing");
            else if (!key.Equals(folder, StringComparison.OrdinalIgnoreCase)) def.Errors.Add("festival.json: key '" + key + "' does not match the folder name '" + folder + "'");
            def.NameAr = Str(f, "name_ar") ?? ""; def.NameEn = Str(f, "name_en") ?? "";
            if (def.NameAr.Trim().Length == 0) def.Errors.Add("festival.json: 'name_ar' is missing");
            if (def.NameEn.Trim().Length == 0) def.Errors.Add("festival.json: 'name_en' is missing");
            if (def.NameEn.Length == 0) def.NameEn = folder;
            if (def.NameAr.Length == 0) def.NameAr = def.NameEn;
            var en = Bool(f, "enabled"); def.Enabled = en ?? true;
            def.AnnounceAr = Str(f, "announce_ar"); def.AnnounceEn = Str(f, "announce_en");
            def.EndAr = Str(f, "end_ar"); def.EndEn = Str(f, "end_en");
            def.Notes = Str(f, "notes") ?? "";
            def.IconShow = Str(f, "icon_show"); def.IconHide = Str(f, "icon_hide"); def.IconClick = Str(f, "icon_click");// [LuaUi]
            foreach (var a in Arr(Val(f, "highlight_acts")) ?? new List<object>()) { uint act = U(a); if (act != 0) def.HighlightActs.Add(act.ToString()); }
            foreach (var m in Arr(Val(f, "maps")) ?? new List<object>())
            {
                uint mid = U(m);
                if (mid == 0) { def.Errors.Add("festival.json: bad map id '" + m + "'"); continue; }
                if (!QuestEngine.QuestGrammar.MapKnown(mid)) def.Errors.Add("festival.json: map " + mid + " does not exist");
                def.Maps.Add(mid);
            }
            ParseSchedule(def, Obj(Val(f, "schedule")));

            // npcs.txt
            string nf = Path.Combine(dir, "npcs.txt");
            if (File.Exists(nf)) ParseNpcs(def, nf);

            // monsters\*.ini
            string md = Path.Combine(dir, "monsters");
            if (Directory.Exists(md)) ParseMonsters(def, md, festivalTypes);

            // spawns.json
            string sf = Path.Combine(dir, "spawns.json");
            if (File.Exists(sf)) ParseSpawns(def, sf, festivalTypes);

            // quests.json
            string qf = Path.Combine(dir, "quests.json");
            if (File.Exists(qf)) ParseQuests(def, qf);
            CheckOfficialNpcIds(def);// [FestivalTrack] needs QuestIds

            // rewards.json
            string rf = Path.Combine(dir, "rewards.json");
            if (File.Exists(rf)) ParseRewards(def, rf);
            return def;
        }

        static void ParseSchedule(FestivalDef def, Dictionary<string, object> s)
        {
            var sc = def.Schedule;
            if (s == null) { def.Errors.Add("festival.json: 'schedule' is missing"); return; }
            string type = (Str(s, "type") ?? "").Trim().ToLowerInvariant();
            switch (type)
            {
                case "manual": sc.Type = ScheduleType.Manual; break;
                case "range":
                    {
                        sc.Type = ScheduleType.Range;
                        DateTime a, b; bool at, bt;
                        if (!ParseDateTime(Str(s, "start"), out a, out at)) { def.Errors.Add("schedule: range 'start' must be 'yyyy-MM-dd' or 'yyyy-MM-dd HH:mm' (got '" + Str(s, "start") + "')"); break; }
                        if (!ParseDateTime(Str(s, "end"), out b, out bt)) { def.Errors.Add("schedule: range 'end' must be 'yyyy-MM-dd' or 'yyyy-MM-dd HH:mm' (got '" + Str(s, "end") + "')"); break; }
                        if (!bt) b = b.AddDays(1);// a date without a time = the end of that day
                        sc.RangeStart = a; sc.RangeEnd = b;
                        if (b <= a) def.Errors.Add("schedule: range end is not after its start");
                        else if (b < DateTime.Now) def.Warnings.Add("schedule: the range ended on " + b.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + " (it will not run again)");
                        break;
                    }
                case "yearly":
                    {
                        sc.Type = ScheduleType.Yearly;
                        int m, d, mi; bool tm;
                        if (!ParseMonthDay(Str(s, "start"), out m, out d, out mi, out tm)) { def.Errors.Add("schedule: yearly 'start' must be 'MM-dd' or 'MM-dd HH:mm' (got '" + Str(s, "start") + "')"); break; }
                        sc.YStartMonth = m; sc.YStartDay = d; sc.YStartMin = mi;
                        if (!ParseMonthDay(Str(s, "end"), out m, out d, out mi, out tm)) { def.Errors.Add("schedule: yearly 'end' must be 'MM-dd' or 'MM-dd HH:mm' (got '" + Str(s, "end") + "')"); break; }
                        sc.YEndMonth = m; sc.YEndDay = d; sc.YEndMin = tm ? mi : 1440;
                        break;
                    }
                case "weekly":
                    {
                        sc.Type = ScheduleType.Weekly;
                        var days = Arr(Val(s, "days"));
                        if (days == null || days.Count == 0) { def.Errors.Add("schedule: weekly needs 'days' (e.g. [\"Fri\",\"Sat\"])"); break; }
                        foreach (var dv in days)
                        {
                            int dw = ParseDay(dv == null ? "" : dv.ToString());
                            if (dw < 0) def.Errors.Add("schedule: unknown day '" + dv + "'");
                            else sc.Days[dw] = true;
                        }
                        break;
                    }
                default: def.Errors.Add("schedule: 'type' must be manual, range, yearly or weekly (got '" + type + "')"); return;
            }
            var hours = Val(s, "hours");
            var hl = hours is string ? new List<object> { hours } : Arr(hours);
            foreach (var hv in hl ?? new List<object>())
            {
                string hs = hv == null ? "" : hv.ToString().Trim();
                if (hs.Length == 0) continue;
                var parts = hs.Split('-');
                int a, b;
                if (parts.Length != 2 || !ParseHm(parts[0], out a) || !ParseHm(parts[1], out b) || a == b) { def.Errors.Add("schedule: bad hours window '" + hs + "' (use 'HH:mm-HH:mm')"); continue; }
                if (a >= 1440) { def.Errors.Add("schedule: hours window '" + hs + "' starts at 24:00"); continue; }
                sc.Hours.Add(new[] { a, b });
            }
        }

        /// <summary>[FestivalMaps 2026-10-14] a map id above 65535 (official 7-digit instance id) has a row in maps.txt whose map file exists.</summary>
        static bool BigMapKnown(uint map)
        {
            uint doc;
            try { return FestivalMaps.FestivalMapTable.TryResolve(map, out doc); } catch { return false; }
        }
        static string BigMapError(uint map)
        {
            return "map id " + map + " is not in Data\\FestivalMaps\\maps.txt (or its map file is missing): 7-digit official ids need a row there";
        }

        static void ParseNpcs(FestivalDef def, string file)
        {
            int n = 0;
            var seen = new HashSet<uint>();
            foreach (string raw in File.ReadAllLines(file, Encoding.UTF8))
            {
                n++;
                string line = raw.Trim().TrimStart('﻿');
                string comment = "";
                int hash = line.IndexOf('#');
                if (hash >= 0) { comment = line.Substring(hash + 1).Trim(); line = line.Substring(0, hash).Trim(); }
                if (line.Length == 0 || line.StartsWith("//")) continue;
                var p = line.Split(',').Select(x => x.Trim()).ToArray();
                uint uid, mesh, map; byte type; ushort x, y;
                if (p.Length < 6 || !uint.TryParse(p[0], out uid) || !byte.TryParse(p[1], out type) || !uint.TryParse(p[2], out mesh) || !uint.TryParse(p[3], out map) || !ushort.TryParse(p[4], out x) || !ushort.TryParse(p[5], out y))
                { def.Errors.Add("npcs.txt line " + n + ": expected id,type,mesh,map,x,y[,dynamic] (got '" + Short(raw) + "')"); continue; }
                // [FestivalMaps 2026-10-14] a 7-digit official instance id is accepted when Data\FestivalMaps\maps.txt maps it to a doc
                if (map > ushort.MaxValue && !BigMapKnown(map)) { def.Errors.Add("npcs.txt line " + n + ": " + BigMapError(map)); continue; }
                // [FestivalTrack] an id outside the festival range is accepted only when it is the OFFICIAL id of an NPC that
                // one of this festival's quests names in Questinfo.ini (checked in CheckOfficialNpcIds after quests.json)
                if (uid < NpcIdMin || uid > NpcIdMax) def.OfficialNpcLines[uid] = n;
                if (!seen.Add(uid)) { def.Errors.Add("npcs.txt line " + n + ": npc id " + uid + " is repeated"); continue; }
                if (mesh == 42580) def.Errors.Add("npcs.txt line " + n + ": mesh 42580 is skipped by the NPC loader");
                if (!QuestEngine.QuestGrammar.MapKnown(map)) def.Errors.Add("npcs.txt line " + n + ": map " + map + " does not exist");
                def.Npcs.Add(new FestivalNpc { Uid = uid, Type = type, Mesh = mesh, Map = map, X = x, Y = y, Dynamic = p.Length > 6 && p[6] == "1", Name = comment, Line = n });
            }
        }

        static void ParseMonsters(FestivalDef def, string dir, ICollection<uint> festivalTypes)
        {
            var ini = new WindowsAPI.IniFile("");
            var existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fams = Pool.MonsterFamilies;
            if (fams != null)
                foreach (var f in fams.Values.ToArray())
                    if (f != null && f.Name != null && (festivalTypes == null || !festivalTypes.Contains(f.ID))) existingNames.Add(f.Name);
            foreach (string file in Directory.GetFiles(dir, "*.ini").OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                string fn = Path.GetFileName(file);
                if (fn.StartsWith("_")) continue;
                string text;
                try { text = File.ReadAllText(file); } catch (Exception e) { def.Errors.Add("monsters\\" + fn + ": " + e.Message); continue; }
                if (text.IndexOf("[cq_monstertype]", StringComparison.OrdinalIgnoreCase) < 0) { def.Errors.Add("monsters\\" + fn + ": no [cq_monstertype] section"); continue; }
                Game.MsgMonster.MonsterFamily fam = null;
                try { fam = global::Qahr.Mobile.Server.ParseMonsterFamily(ini, Path.GetFullPath(file), false); }
                catch (Exception e) { def.Errors.Add("monsters\\" + fn + " does not parse: " + e.Message); continue; }
                if (fam == null) { def.Errors.Add("monsters\\" + fn + ": invalid (id, name and level are required and attack_min <= attack_max)"); continue; }
                if ((fam.ID < MonsterIdMin || fam.ID > MonsterIdMax) && fam.ID >= 100000) def.Errors.Add("monsters\\" + fn + ": id " + fam.ID + " is outside the festival range " + MonsterIdMin + "-" + MonsterIdMax);// [MonsterHp 2026-10-15] official client monster.dat ids (<100000) are allowed so the client draws the HP bar
                if (fams != null && fams.ContainsKey(fam.ID) && (festivalTypes == null || !festivalTypes.Contains(fam.ID))) def.Errors.Add("monsters\\" + fn + ": id " + fam.ID + " already exists in Data\\Monsters");
                if (existingNames.Contains(fam.Name)) def.Errors.Add("monsters\\" + fn + ": name '" + fam.Name + "' is already a normal monster name");
                if (def.Monsters.Any(m => m.ID == fam.ID)) def.Errors.Add("monsters\\" + fn + ": id " + fam.ID + " is repeated in this festival");
                if (def.Monsters.Any(m => string.Equals(m.Name, fam.Name, StringComparison.OrdinalIgnoreCase))) def.Errors.Add("monsters\\" + fn + ": name '" + fam.Name + "' is repeated in this festival");
                if (fam.Name.Length > 15) def.Warnings.Add("monsters\\" + fn + ": name '" + fam.Name + "' is longer than 15 characters (the client may cut it)");
                try { fam.CreateItemGenerator(); fam.CreateMonsterSettings(); }
                catch (Exception e) { def.Errors.Add("monsters\\" + fn + ": " + e.Message); continue; }
                def.Monsters.Add(fam);
            }
        }

        /// <summary>A monster type by name or id: this festival's types first, then the normal (non-festival) types.</summary>
        public static Game.MsgMonster.MonsterFamily FindType(FestivalDef def, string monster, ICollection<uint> festivalTypes)
        {
            if (string.IsNullOrWhiteSpace(monster)) return null;
            uint id;
            bool isId = uint.TryParse(monster.Trim(), out id);
            foreach (var m in def.Monsters)
                if ((isId && m.ID == id) || string.Equals(m.Name, monster.Trim(), StringComparison.OrdinalIgnoreCase)) return m;
            var fams = Pool.MonsterFamilies;
            if (fams == null) return null;
            if (isId) { Game.MsgMonster.MonsterFamily f; if (fams.TryGetValue(id, out f) && (festivalTypes == null || !festivalTypes.Contains(id))) return f; return null; }
            foreach (var f in fams.Values.ToArray().OrderBy(x => x.ID))
                if (f != null && string.Equals(f.Name, monster.Trim(), StringComparison.OrdinalIgnoreCase) && (festivalTypes == null || !festivalTypes.Contains(f.ID))) return f;
            return null;
        }

        static void ParseSpawns(FestivalDef def, string file, ICollection<uint> festivalTypes)
        {
            Dictionary<string, object> root;
            try { root = ReadJson(file); }
            catch (Exception e) { def.Errors.Add("spawns.json does not parse: " + e.Message); return; }
            var arr = Arr(Val(root, "spawns"));
            if (arr == null) { def.Errors.Add("spawns.json: 'spawns' list is missing"); return; }
            int total = 0, i = 0;
            foreach (var o in arr)
            {
                i++;
                var d = Obj(o);
                if (d == null) { def.Errors.Add("spawns.json #" + i + ": not an object"); continue; }
                var sp = new FestivalSpawn { Monster = Str(d, "monster") ?? "", Map = U(Val(d, "map")), X = (ushort)Math.Min(ushort.MaxValue, U(Val(d, "x"))), Y = (ushort)Math.Min(ushort.MaxValue, U(Val(d, "y"))) };
                if (Val(d, "w") != null) sp.W = (ushort)Math.Min(ushort.MaxValue, U(Val(d, "w")));
                if (Val(d, "h") != null) sp.H = (ushort)Math.Min(ushort.MaxValue, U(Val(d, "h")));
                if (Val(d, "count") != null) sp.Count = (int)Math.Min(int.MaxValue, U(Val(d, "count")));
                if (Val(d, "respawn") != null) sp.Respawn = (int)Math.Min(int.MaxValue, U(Val(d, "respawn")));
                string where = "spawns.json #" + i + " (" + sp.Monster + ")";
                if (sp.Monster.Length == 0) def.Errors.Add(where + ": 'monster' is missing");
                else if (FindType(def, sp.Monster, festivalTypes) == null) def.Errors.Add(where + ": monster '" + sp.Monster + "' is neither in monsters\\ nor a normal monster type");
                if (sp.Map == 0) def.Errors.Add(where + ": 'map' is missing");
                else if (sp.Map > ushort.MaxValue && !BigMapKnown(sp.Map)) def.Errors.Add(where + ": " + BigMapError(sp.Map));// [FestivalMaps 2026-10-14]
                else if (!QuestEngine.QuestGrammar.MapKnown(sp.Map)) def.Errors.Add(where + ": map " + sp.Map + " does not exist");
                if (Val(d, "x") == null || Val(d, "y") == null) def.Errors.Add(where + ": 'x' and 'y' are required");
                if (sp.W < 1 || sp.W > 300 || sp.H < 1 || sp.H > 300) def.Errors.Add(where + ": w/h must be 1-300");
                if (sp.Count < 1 || sp.Count > 100) def.Errors.Add(where + ": count must be 1-100");
                if (sp.Respawn < 5 || sp.Respawn > 86400) def.Errors.Add(where + ": respawn must be 5-86400 seconds");
                total += Math.Max(0, sp.Count);
                def.Spawns.Add(sp);
            }
            if (total > MaxMonstersPerFestival) def.Errors.Add("spawns.json: " + total + " monsters in total (max " + MaxMonstersPerFestival + ")");
        }

        static void ParseQuests(FestivalDef def, string file)
        {
            Dictionary<string, object> root;
            try { root = ReadJson(file); }
            catch (Exception e) { def.Errors.Add("quests.json does not parse: " + e.Message); return; }
            if (root == null) { def.Errors.Add("quests.json is not a JSON object"); return; }
            def.QuestsRoot = root;
            if (Val(root, "serverType") != null) def.Warnings.Add("quests.json: 'serverType' is ignored in a festival");
            var all = Database.QuestInfo.AllQuests;
            var quests = Obj(Val(root, "quests"));
            if (quests != null)
                foreach (var kv in quests)
                {
                    if (kv.Key.StartsWith("_")) continue;
                    uint id;
                    if (!uint.TryParse(kv.Key, out id) || id == 0) { def.Errors.Add("quests.json: quest key '" + kv.Key + "' is not a quest id"); continue; }
                    if (Obj(kv.Value) == null) { def.Errors.Add("quests.json: quest " + id + " is not an object"); continue; }
                    if (all != null && all.Count > 0 && !all.ContainsKey(id)) def.Errors.Add("quests.json: quest " + id + " is not in Questinfo.ini");
                    def.QuestIds.Add(id);
                }
            var main = MainQuestIds();
            foreach (uint id in def.QuestIds)
                if (main.Contains(id)) def.Warnings.Add("quests.json: quest " + id + " is also in QuestEngine\\quests.json - while this festival is stopped it is switched OFF");
            // [InstanceNpc] NPCs spawned inside the private copies of its instances (QuestEngine InstanceDef.Npcs)
            var insts = Obj(Val(root, "instances"));
            if (insts != null)
                foreach (var kv in insts)
                {
                    if (kv.Key.StartsWith("_")) continue;
                    var inst = Obj(kv.Value);
                    if (inst == null) continue;
                    uint imap = U(Val(inst, "map"));
                    if (imap == 0 || !QuestEngine.QuestGrammar.MapKnown(imap)) def.Errors.Add("quests.json instance '" + kv.Key + "': map " + imap + " does not exist");
                    int i = 0;
                    foreach (var o in Arr(Val(inst, "npcs")) ?? new List<object>())
                    {
                        i++;
                        var n = Obj(o);
                        string where = "quests.json instance '" + kv.Key + "' npc #" + i;
                        uint uid = U(Val(n, "id")), mesh = U(Val(n, "mesh"));
                        if (n == null || uid == 0) { def.Errors.Add(where + ": 'id' is missing"); continue; }
                        if (mesh == 0 || mesh == 42580) def.Errors.Add(where + " (" + uid + "): 'mesh' is missing or invalid");
                        if (Val(n, "x") == null || Val(n, "y") == null) def.Errors.Add(where + " (" + uid + "): 'x' and 'y' are required");
                        if (def.InstanceNpcs.Contains(uid)) continue;// the same NPC in two instances of this festival is fine
                        if (def.Npcs.Any(x => x.Uid == uid)) def.Errors.Add(where + ": id " + uid + " is also in this festival's npcs.txt (an instance NPC needs its own id)");
                        def.InstanceNpcs.Add(uid);
                    }
                }
        }

        // [FestivalTrack] The client's quest tracker / quest panel links ("[npc Name,id,map,x,y]", BeginNpcId, FinishNpcId in
        // the CLIENT's ini\Questinfo.ini) walk to map,x,y and then look for the NPC with that OFFICIAL id. A festival NPC
        // with an 88xxxxx id (+ quests.json npcAliases) works on the server but the client never finds it, so tracking
        // does nothing. A festival may therefore place the NPC under its official id when one of its own quests names
        // that id in Questinfo.ini; Npcs.txt / SobNpcs.txt / other festivals collisions are still refused (CrossValidate).
        static void CheckOfficialNpcIds(FestivalDef def)
        {
            var refs = (def.OfficialNpcLines.Count > 0 || def.InstanceNpcs.Count > 0) ? QuestNpcRefs() : null;
            foreach (var kv in def.OfficialNpcLines)
            {
                uint uid = kv.Key;
                string where = "npcs.txt line " + kv.Value + ": npc id " + uid;
                List<int[]> spots;
                if (!NamedByQuests(def, refs, uid, out spots))
                {
                    def.Errors.Add(where + " is outside the festival range " + NpcIdMin + "-" + NpcIdMax + " and is not an official NPC id named by this festival's quests in Questinfo.ini");
                    continue;
                }
                string clash = CodeClash(uid);
                if (clash != null) { def.Errors.Add(where + ": " + clash); continue; }
                var n = def.Npcs.FirstOrDefault(x => x.Uid == uid);
                if (n != null && spots != null && spots.Count > 0 && !spots.Any(s => s[0] == n.Map && Math.Abs(s[1] - n.X) <= 3 && Math.Abs(s[2] - n.Y) <= 3))
                    def.Warnings.Add(where + " is not at its Questinfo.ini spot " + spots[0][0] + " (" + spots[0][1] + "," + spots[0][2] + "): the client tracker walks there");
            }
            // [InstanceNpc] instance NPCs: the festival range, or an official id named by its quests (same rules as npcs.txt)
            foreach (uint uid in def.InstanceNpcs)
            {
                if (uid >= NpcIdMin && uid <= NpcIdMax) continue;
                List<int[]> spots;
                string where = "quests.json instance npc " + uid;
                if (!NamedByQuests(def, refs, uid, out spots)) { def.Errors.Add(where + " is outside the festival range " + NpcIdMin + "-" + NpcIdMax + " and is not an official NPC id named by this festival's quests in Questinfo.ini"); continue; }
                string clash = CodeClash(uid);
                if (clash != null) def.Errors.Add(where + ": " + clash);
            }
        }

        static bool NamedByQuests(FestivalDef def, Dictionary<uint, Dictionary<uint, List<int[]>>> refs, uint uid, out List<int[]> spots)
        {
            // every spot any of its quests gives this id (the same NPC may be written at different cells by different quests)
            spots = new List<int[]>();
            if (uid == 0 || uid >= 1000000 || refs == null) return false;
            foreach (uint q in def.QuestIds)
            {
                Dictionary<uint, List<int[]>> m; List<int[]> l;
                if (refs.TryGetValue(q, out m) && m.TryGetValue(uid, out l)) spots.AddRange(l);
            }
            return spots.Count > 0;
        }

        /// <summary>[FestivalTrack] an official id that hand-written code already answers (NpcID enum = an [NpcAttribute]
        /// script, or the Procesor's "under maintenance" list) can't be a festival NPC: the click would go to that code.</summary>
        static string CodeClash(uint uid)
        {
            try { if (Enum.IsDefined(typeof(Game.MsgNpc.NpcID), uid)) return "is an NpcID (hand-written NPC script) - use an id of the festival range"; } catch { }
            try { if (Game.MsgNpc.Procesor.MaintinaceNpc.Contains(uid)) return "is a maintenance NPC id - use an id of the festival range"; } catch { }
            return null;
        }

        static readonly Regex NpcLinkRx = new Regex(@"\[npc\s+[^,\]]*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Dictionary<uint, Dictionary<uint, List<int[]>>> _questNpcRefs;
        static string _questNpcRefsStamp;
        /// <summary>[FestivalTrack] Questinfo.ini: MissionId -> official npc id -> {map,x,y} spots (BeginNpcId, FinishNpcId, [npc ...] links).</summary>
        static Dictionary<uint, Dictionary<uint, List<int[]>>> QuestNpcRefs()
        {
            try
            {
                string f = Path.Combine(Program.ServerConfig.DbLocation, "Questinfo.ini");
                if (!File.Exists(f)) return null;
                var fi = new FileInfo(f);
                string stamp = fi.Length + ":" + fi.LastWriteTimeUtc.Ticks;
                var cached = _questNpcRefs;
                if (cached != null && _questNpcRefsStamp == stamp) return cached;
                var all = new Dictionary<uint, Dictionary<uint, List<int[]>>>();
                Dictionary<uint, List<int[]>> cur = null;
                Action<uint, int, int, int> add = (id, map, x, y) =>
                {
                    if (cur == null || id == 0) return;
                    List<int[]> l;
                    if (!cur.TryGetValue(id, out l)) cur[id] = l = new List<int[]>();
                    l.Add(new[] { map, x, y });
                };
                foreach (string raw in File.ReadLines(f, Encoding.Default))
                {
                    string line = raw.Trim();
                    if (line.StartsWith("[")) { cur = null; continue; }
                    if (line.StartsWith("MissionId=", StringComparison.OrdinalIgnoreCase))
                    {
                        uint mid = U(line.Substring(10));
                        if (mid != 0 && !all.TryGetValue(mid, out cur)) all[mid] = cur = new Dictionary<uint, List<int[]>>();
                        continue;
                    }
                    if (line.StartsWith("BeginNpcId=", StringComparison.OrdinalIgnoreCase) || line.StartsWith("FinishNpcId=", StringComparison.OrdinalIgnoreCase))
                    {
                        var p = line.Substring(line.IndexOf('=') + 1).Split(',');
                        int id, map = 0, x = 0, y = 0;
                        if (p.Length > 0 && int.TryParse(p[0].Trim(), out id) && id > 0)
                        {
                            if (p.Length > 3) { int.TryParse(p[1].Trim(), out map); int.TryParse(p[2].Trim(), out x); int.TryParse(p[3].Trim(), out y); }
                            add((uint)id, map, x, y);
                        }
                        continue;
                    }
                    if (line.IndexOf("[npc", StringComparison.OrdinalIgnoreCase) >= 0)
                        foreach (Match m in NpcLinkRx.Matches(line))
                            add(U(m.Groups[1].Value), (int)U(m.Groups[2].Value), (int)U(m.Groups[3].Value), (int)U(m.Groups[4].Value));
                }
                _questNpcRefs = all; _questNpcRefsStamp = stamp;
                return all;
            }
            catch { return null; }
        }

        static HashSet<uint> MainQuestIds()
        {
            var set = new HashSet<uint>();
            try
            {
                string f = Path.Combine(Path.Combine(Program.ServerConfig.DbLocation, "QuestEngine"), "quests.json");
                if (!File.Exists(f)) return set;
                var q = Obj(Val(ReadJson(f), "quests"));
                if (q != null) foreach (var k in q.Keys) { uint id; if (uint.TryParse(k, out id)) set.Add(id); }
            }
            catch { }
            return set;
        }

        static List<FestivalItem> Items(object o, string where, FestivalDef def)
        {
            var list = new List<FestivalItem>();
            foreach (var e in Arr(o) ?? new List<object>())
            {
                var d = Obj(e);
                if (d == null) { def.Errors.Add(where + ": item entries must be {\"id\":..,\"count\":..}"); continue; }
                var it = new FestivalItem { Id = U(Val(d, "id")), Count = (ushort)Math.Max(1, Math.Min(1000, Val(d, "count") == null ? 1 : U(Val(d, "count")))), Bound = Bool(d, "bound") ?? false };
                if (it.Id == 0) { def.Errors.Add(where + ": item without 'id'"); continue; }
                if (Pool.ItemsBase != null && Pool.ItemsBase.Count > 0 && !Pool.ItemsBase.ContainsKey(it.Id)) def.Errors.Add(where + ": item " + it.Id + " is not in itemtype.txt");
                list.Add(it);
            }
            return list;
        }

        static int GiveSlots(FestivalItem it)
        {
            Database.ItemType.DBItem db;
            if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(it.Id, out db) && db != null && db.StackSize > 1) return (it.Count + db.StackSize - 1) / db.StackSize;
            return it.Count;
        }

        static void ParseRewards(FestivalDef def, string file)
        {
            Dictionary<string, object> root;
            try { root = ReadJson(file); }
            catch (Exception e) { def.Errors.Add("rewards.json does not parse: " + e.Message); return; }
            var npcIds = new HashSet<uint>(def.Npcs.Select(n => n.Uid));
            var texts = Obj(Val(root, "npcText"));
            if (texts != null)
                foreach (var kv in texts)
                {
                    if (kv.Key.StartsWith("_")) continue;
                    uint id; var d = Obj(kv.Value);
                    if (!uint.TryParse(kv.Key, out id) || d == null) { def.Errors.Add("rewards.json npcText: '" + kv.Key + "' must be an npc id with {\"ar\":..,\"en\":..}"); continue; }
                    if (!npcIds.Contains(id) && !def.InstanceNpcs.Contains(id)) def.Warnings.Add("rewards.json npcText: npc " + id + " is not in this festival's npcs.txt");
                    def.NpcText[id] = new[] { Str(d, "ar") ?? "", Str(d, "en") ?? "" };
                }
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int i = 0;
            foreach (var o in Arr(Val(root, "exchanges")) ?? new List<object>())
            {
                i++;
                var d = Obj(o);
                if (d == null) { def.Errors.Add("rewards.json exchange #" + i + ": not an object"); continue; }
                var ex = new FestivalExchange { Id = Str(d, "id") ?? "", Npc = U(Val(d, "npc")), TextAr = Str(d, "text_ar") ?? "", TextEn = Str(d, "text_en") ?? "" };
                string where = "rewards.json exchange '" + (ex.Id.Length > 0 ? ex.Id : "#" + i) + "'";
                if (!ExIdRx.IsMatch(ex.Id)) def.Errors.Add(where + ": 'id' is missing or has bad characters");
                else if (!ids.Add(ex.Id)) def.Errors.Add(where + ": id is repeated");
                if (ex.Npc == 0 || !npcIds.Contains(ex.Npc)) def.Errors.Add(where + ": 'npc' must be an npc of this festival's npcs.txt");
                if (ex.TextEn.Trim().Length == 0) def.Errors.Add(where + ": 'text_en' is missing");
                if (ex.TextAr.Trim().Length == 0) def.Warnings.Add(where + ": 'text_ar' is missing");
                var cost = Obj(Val(d, "cost"));
                if (cost != null) { ex.CostItems = Items(Val(cost, "items"), where + " cost", def); ex.CostSilver = UL(Val(cost, "silver")); ex.CostCps = U(Val(cost, "cps")); }
                var give = Obj(Val(d, "give"));
                if (give == null) def.Errors.Add(where + ": 'give' is missing");
                else { ex.GiveItems = Items(Val(give, "items"), where + " give", def); ex.GiveExp = UL(Val(give, "exp")); ex.GiveSilver = UL(Val(give, "silver")); ex.GiveCps = U(Val(give, "cps")); }
                if (ex.GiveItems.Count == 0 && ex.GiveExp == 0 && ex.GiveSilver == 0 && ex.GiveCps == 0) def.Errors.Add(where + ": gives nothing");
                if (ex.GiveItems.Sum(x => GiveSlots(x)) > 40) def.Errors.Add(where + ": gives more than 40 items");// [DreamTheater 2026-10-15] counted in bag slots (stackable items: one slot per stack), like the exchange's own space check
                var rep = Val(d, "repeat");
                if (rep == null || (rep is bool && !(bool)rep)) ex.Repeat = "once";
                else if (rep is bool && (bool)rep) ex.Repeat = "unlimited";
                else
                {
                    string r = rep.ToString().Trim().ToLowerInvariant();
                    if (r == "daily" || r == "festival" || r == "once") ex.Repeat = r;
                    else if (r == "unlimited" || r == "true") ex.Repeat = "unlimited";
                    else def.Errors.Add(where + ": 'repeat' must be false, 'daily', 'festival' or true");
                }
                if (ex.Repeat != "once" && !ex.HasCost) def.Errors.Add(where + ": a repeatable exchange must have a cost (never free and repeatable)");
                if (ex.GiveCps > 0 && ex.Repeat != "once") def.Warnings.Add(where + ": gives CPs and is repeatable - check with the owner");
                if (Val(d, "minLevel") != null) ex.MinLevel = (ushort)Math.Max(1, Math.Min(255, U(Val(d, "minLevel"))));
                def.Exchanges.Add(ex);
            }
        }

        // ------------------------------------------------------------ across festivals
        static void CrossValidate(List<FestivalDef> list)
        {
            HashSet<uint> placed = null;
            try { placed = QuestEngine.QuestGrammar.PlacedNpcIds(); } catch { }
            foreach (var def in list)
                if (placed != null)
                {
                    foreach (var n in def.Npcs)
                        if (placed.Contains(n.Uid)) def.Errors.Add("npcs.txt: npc id " + n.Uid + " is already used in Npcs.txt / SobNpcs.txt");
                    foreach (uint id in def.InstanceNpcs)
                        if (placed.Contains(id)) def.Errors.Add("quests.json instance npc " + id + " is already used in Npcs.txt / SobNpcs.txt");
                }
            var keys = new Dictionary<string, FestivalDef>(StringComparer.OrdinalIgnoreCase);
            foreach (var def in list)
            {
                FestivalDef o;
                if (keys.TryGetValue(def.Key, out o)) def.Errors.Add("key '" + def.Key + "' is used by two folders");
                else keys[def.Key] = def;
            }
            for (int i = 0; i < list.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    var a = list[i]; var b = list[j];
                    foreach (var n in a.Npcs) if (b.Npcs.Any(x => x.Uid == n.Uid) || b.InstanceNpcs.Contains(n.Uid)) a.Errors.Add("npc id " + n.Uid + " is also used by festival " + b.Key);
                    foreach (uint id in a.InstanceNpcs) if (b.InstanceNpcs.Contains(id) || b.Npcs.Any(x => x.Uid == id)) a.Errors.Add("instance npc id " + id + " is also used by festival " + b.Key);
                    foreach (var m in a.Monsters)
                    {
                        if (b.Monsters.Any(x => x.ID == m.ID)) a.Errors.Add("monster id " + m.ID + " is also used by festival " + b.Key);
                        if (b.Monsters.Any(x => string.Equals(x.Name, m.Name, StringComparison.OrdinalIgnoreCase))) a.Errors.Add("monster name '" + m.Name + "' is also used by festival " + b.Key);
                    }
                    foreach (uint q in a.QuestIds) if (b.QuestIds.Contains(q)) a.Errors.Add("quest " + q + " is also in festival " + b.Key + " (a quest belongs to one festival)");
                }
        }

        // ------------------------------------------------------------ helpers
        public static string FolderHash(string dir)
        {
            try
            {
                using (var sha = SHA1.Create())
                {
                    var sb = new StringBuilder();
                    foreach (string f in Directory.GetFiles(dir, "*", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                    {
                        if (Path.GetFileName(f).EndsWith(".md", StringComparison.OrdinalIgnoreCase)) continue;
                        sb.Append(f.Substring(dir.Length)).Append(':').Append(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(f)))).Append(';');
                    }
                    return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()))).Replace("-", "");
                }
            }
            catch { return Guid.NewGuid().ToString("N"); }
        }
        public static Dictionary<string, object> ReadJson(string file)
        {
            string text = File.ReadAllText(file, Encoding.UTF8).TrimStart('﻿');
            return AdminApi.AdminJson.Parse(text) as Dictionary<string, object>;
        }
        static bool ParseDateTime(string s, out DateTime v, out bool hasTime)
        {
            v = DateTime.MinValue; hasTime = false;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim().Replace('T', ' ');
            if (DateTime.TryParseExact(s, new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out v)) { hasTime = true; return true; }
            return DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out v);
        }
        static bool ParseMonthDay(string s, out int month, out int day, out int minutes, out bool hasTime)
        {
            month = day = minutes = 0; hasTime = false;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Trim().Split(new[] { ' ', 'T' }, StringSplitOptions.RemoveEmptyEntries);
            var md = parts[0].Split('-');
            if (md.Length != 2 || !int.TryParse(md[0], out month) || !int.TryParse(md[1], out day)) return false;
            if (month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(2024, month)) return false;
            if (parts.Length > 1) { if (!ParseHm(parts[1], out minutes) || minutes > 1440) return false; hasTime = true; }
            return parts.Length <= 2;
        }
        static bool ParseHm(string s, out int minutes)
        {
            minutes = 0;
            var p = (s ?? "").Trim().Split(':');
            int h, m;
            if (p.Length != 2 || !int.TryParse(p[0], out h) || !int.TryParse(p[1], out m) || h < 0 || h > 24 || m < 0 || m > 59 || (h == 24 && m != 0)) return false;
            minutes = h * 60 + m;
            return true;
        }
        static int ParseDay(string s)
        {
            s = (s ?? "").Trim();
            int n;
            if (int.TryParse(s, out n)) return n >= 0 && n <= 6 ? n : (n == 7 ? 0 : -1);
            if (s.Length < 3) return -1;
            string p = s.Substring(0, 3).ToLowerInvariant();
            string[] names = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };
            return Array.IndexOf(names, p);
        }
        static string Short(string s) { s = s ?? ""; return s.Length > 60 ? s.Substring(0, 60) + "..." : s; }
        public static Dictionary<string, object> Obj(object o) { return o as Dictionary<string, object>; }
        public static List<object> Arr(object o) { return o as List<object>; }
        public static object Val(Dictionary<string, object> d, string k) { object v; return d != null && d.TryGetValue(k, out v) ? v : null; }
        public static string Str(Dictionary<string, object> d, string k) { var v = Val(d, k); return v == null ? null : v.ToString(); }
        public static bool? Bool(Dictionary<string, object> d, string k) { var v = Val(d, k); if (v is bool) return (bool)v; if (v is string) { bool b; if (bool.TryParse((string)v, out b)) return b; } return null; }
        public static uint U(object v)
        {
            if (v == null) return 0;
            string s = v.ToString().Trim().Split('.')[0];
            uint r; return uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? r : 0;
        }
        public static ulong UL(object v)
        {
            if (v == null) return 0;
            string s = v.ToString().Trim().Split('.')[0];
            ulong r; return ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? r : 0;
        }
    }
}
