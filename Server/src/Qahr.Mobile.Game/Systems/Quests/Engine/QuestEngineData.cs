// ============================================================================
//  [QuestEngine] data side: turns Questinfo.ini (+ Data\QuestEngine\quests.json) into typed objectives,
//  prizes and drop rules, so quests work from DATA instead of one hand-written NPC script per quest.
//
//  Intention grammar (verified against the 2,190 quests of Questinfo.ini, see tools\QuestAudit\engine):
//      [npc  Name,npcId,map,x,y]        an NPC (npcId -1 = instance NPC, not supported)
//      [mob  Name,map,x,y]              a monster (or a place / object when the name is not a monster)
//      [item_t Name,map,x,y]            where a quest item can be found
//      [map  Name,map,x,y]              a place (descriptions only)
//      [chk_quest qid,flag,slot,count]  progress counter: QuestGUI Intentions[slot-1] must reach count
//      [chk_item itemId,count]          the client checks the bag: count of itemId
//      IntentionN_mask=                 per-class variant of line N (the engine uses the plain line, or the first variant)
//      "Teammate kills can be shared in this quest." -> kills of team mates on screen count too.
//  Objective kinds: Talk (npc), Kill (monster name, count), Collect (item in bag, count, dropped by the named
//  monster or by monsters near the [item_t] point), Gather (counter raised by kills near a point), Visit
//  (stand within Range of map,x,y), State (live player state), Event (raised by a server system through
//  QuestEngineCore.OnEvent), Counter (raised only by hand-written code), KillPoints (monster levels on a map),
//  Unknown (scripted step the engine cannot judge -> quest not engine-workable).
//
//  quests.json (optional, re-read by @questreload / AdminApi reloadquests):
//  {
//    "serverType": 0,                    // quests whose Questinfo ShowServerType list lacks it are not offered
//                                        // (unless the quest has "engine": true)
//    "drops":  [ { "item": 711236, "monster": "Turtledove", "chance": 30 } ],          // global drop rules
//    "npcAliases": { "24309": 65787 },   // quest NPC id -> the server NPC that stands in for it (begin/finish/talk)
//    "npcTeleports": { "24171": [ {"text":"Nightmare Hall I","map":10479,"x":216,"y":555,"minLevel":100,
//                                   "requireQuest":11411, "instance": "<instance name>"} ] },
//    "instances": { "<name>": { "map": 12617, "x": 172, "y": 183, "minutes": 30,
//                               "exit": {"map":1002,"x":439,"y":478},
//                               "monsters": [ {"monster":"Wraith","x":63,"y":134,"count":1} ],
//                               "npcs": [ {"id":33397,"mesh":30680,"x":80,"y":163,"name":"GhostScout"} ] } },
//                    // npcs: one NPC object per private copy (the copy's DynamicID), removed with the copy; the id must
//                    // be unique (an instance-only id, or the id of the same NPC on the base map = a copy of it)
//    "quests": {
//      "1735": {
//        "engine": true,                 // true = the engine owns it even if hand-written code references it,
//                                        // false = never touch it (no counting, no dialogs)
//        "engineV2": true|false|"auto",  // read INSTEAD of "engine" by this engine version (lets one file keep a
//                                        // quest off for an older running engine and on after the update)
//        "objectives": [                 // replaces the parsed objectives
//          { "type": "kill", "monster": "Robin", "count": 100, "slot": 1, "map": 1002,
//            "monsters": ["RobinL2"], "timeLimit": 10 },              // also "monster": "*" + "maps"/"minLevel"
//          { "type": "talk", "npc": 4, "slot": 2, "after": 1 },
//          { "type": "collect", "item": 711236, "count": 5, "take": true, "source": "external",
//            "drops": [ { "monster": "Turtledove", "chance": 30 }, { "map": 1015, "x": 500, "y": 400, "radius": 60 } ] },
//          { "type": "gather", "count": 10, "slot": 1, "map": 1002, "x": 300, "y": 300, "radius": 40, "chance": 50 },
//          { "type": "visit", "map": 1002, "x": 300, "y": 300, "range": 6, "slot": 3 },
//          { "type": "state", "stat": "classStage|reborn|level|hundredWeapon|hundredWeaponsUnlocked", "min": 1, "weapon": "Dagger", "slot": 1 },
//          { "type": "classStage", "min": 5003, "slot": 1 },          // = state classStage
//          { "type": "event", "event": "bountyDone", "count": 1, "slot": 1 },  // also "signal" (key) / "useItem" (item)
//          { "type": "counter", "slot": 1, "count": 3 },
//          { "type": "killPoints", "map": 1700, "count": 40000, "points": "monsterLevel", "slot": 1 }
//        ],
//        "drops": [ { "item": 711236, "monster": "Turtledove", "chance": 30 } ],     // extra rules for its collect items
//        "prize": { "exp": 1000, "silver": 5000, "cps": 0, "expBalls": 1,
//                   "items": [ { "id": 1088000, "count": 1, "bound": true } ], "random": false },
//        "prizeExtra": { "ninjaUnlock": true, "ninjaGates": ["Gate_of_Dawn"], "archive": "StoneCracker" },
//        "allowParsedCps": false,        // CPs / silver > 1,000,000 parsed from the Prize TEXT are paid only with true
//        "final": true,                  // force (true) / clear (false) the chain-end flag ("Final prize" items)
//        "beginNpc": 2001, "finishNpc": 2001, "finishMap": 1002, "finishX": 439, "finishY": 464,
//        "prequest": "11477|11502",      // replaces Questinfo Prequest (same syntax; "" or "0" = none)
//        "onAccept":   { "teleport": {"map":10764,"x":253,"y":336}, "giveItems": [{"id":720861,"count":1}],
//                        "spawn": {"monster":"Itachi","map":1015,"x":698,"y":681,"count":1,"minutes":10},
//                                 (a list of specs spawns each monster; "monsterId": 5917 picks one of several types sharing the name)
//                        "instance": "<instance name>" },
//        "onComplete": { ... },  "onFinish": { ..., "leaveInstance": true },
//        "repeat": "daily" | "weekly",   // ("daily": true too) finished/active copies are removed at the reset
//        "offerOnly": true,              // engine offers/accepts it but never finishes it (hand script does)
//        "engineCounts": true,           // (or "forceCount") count its objectives although its hand code counts too
//        "pool": "LadyWind28524",        // one quest of the pool per player per day is offered
//        "requireClan": true,
//        "beginText": "...", "progressText": "...", "finishText": "...",
//        "next": 1736, "autoAccept": true, "team": true, "takeItems": true
//      }
//    }
//  }
//  "chance" is a percent (0.01 - 100). Unknown keys (and every key starting with "_") are ignored.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Qahr.Mobile.QuestEngine
{
    public enum ObjectiveKind : byte { Talk, Kill, Collect, Gather, Visit, Unknown, State, Event, Counter, KillPoints }

    /// <summary>[QuestEngine] where a quest item (or a Gather count) comes from. Empty Monster = any monster.</summary>
    public sealed class DropRule
    {
        public string Monster = "";   // normalized monster name ("" = any)
        public uint Map;              // 0 = any map
        public ushort X, Y;
        public ushort Radius;         // 0 = whole map
        public int Chance = 2500;     // per 10000

        public bool Matches(string monsterNorm, uint map, ushort x, ushort y)
        {
            if (Monster.Length > 0 && !QuestGrammar.SameMonster(Monster, monsterNorm))
                return false;
            if (Map != 0 && Map != map)
                return false;
            if (Map != 0 && Radius > 0 && Math.Max(Math.Abs(x - X), Math.Abs(y - Y)) > Radius)
                return false;
            return Monster.Length > 0 || Map != 0;// a rule with neither would drop from everything: refused
        }
        public override string ToString()
        {
            return (Monster.Length > 0 ? Monster : "any monster") + (Map != 0 ? " on map " + Map + (Radius > 0 ? " within " + Radius + " of (" + X + "," + Y + ")" : "") : "") + " " + (Chance / 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }
    }

    /// <summary>[QuestEngine] one typed objective of a quest.</summary>
    public sealed class Objective
    {
        public ObjectiveKind Kind = ObjectiveKind.Unknown;
        public byte Slot;               // 1-based QuestGUI Intentions slot (0 = none: Collect is judged on the bag)
        public uint Count = 1;
        public uint NpcId;
        public string Name = "";        // name as written in the data
        public string Monster = "";     // normalized monster name (Kill)
        public List<string> Monsters = new List<string>(); // Kill: more accepted (normalized) names
        public bool AnyMonster;         // Kill "*": any monster (MinLevelGap / Maps filters)
        public int MinLevelGap = 20;    // AnyMonster / KillPoints: monster level >= player level - gap
        public HashSet<uint> Maps;      // AnyMonster: only these maps (null = Map, or any map when Map is 0)
        public uint TimeLimit;          // Kill: seconds from the first kill of the run (the counter restarts after it)
        public string SpawnedBy = "";   // Kill: the monster is summoned by another system (item use, NPC script): no static spawn needed
        public uint ItemId;             // Collect
        public bool TakeItem = true;    // Collect: removed from the bag on finish
        public bool External;           // Collect: the item comes from another system (no drop rule needed)
        public uint Map;
        public ushort X, Y;
        public ushort Range = 8;        // Visit distance / Gather radius
        public int Chance = 10000;      // Gather: per 10000
        public List<DropRule> Drops = new List<DropRule>();
        public string Text = "";
        public string Stat = "";        // State
        public uint Min;                // State
        public string Weapon = "";      // State hundredWeapon
        public string Event = "";       // Event key
        public string Points = "";      // KillPoints mode ("monsterLevel")
        public byte After;              // Talk: counts only once this slot is done
        public int Line;                // Intention line it came from (0 = quests.json)

        public bool MatchesMonster(string killedNorm)
        {
            if (AnyMonster) return true;
            if (Monster.Length > 0 && QuestGrammar.SameMonster(Monster, killedNorm)) return true;
            foreach (var m in Monsters) if (QuestGrammar.SameMonster(m, killedNorm)) return true;
            return false;
        }

        public string Describe()
        {
            switch (Kind)
            {
                case ObjectiveKind.Talk: return "Talk to " + Name + " (npc " + NpcId + ")" + (Slot > 0 ? " [slot " + Slot + "]" : "") + (After > 0 ? " after slot " + After : "");
                case ObjectiveKind.Kill: return "Kill " + Count + " x " + (AnyMonster ? "any monster" : (Name.Length > 0 ? Name : Monster)) + (Monsters.Count > 0 ? " (or " + string.Join("/", Monsters) + ")" : "") + " [slot " + Slot + "]" + (Map != 0 ? " near map " + Map : "") + (TimeLimit > 0 ? " within " + TimeLimit + "s" : "");
                case ObjectiveKind.Collect: return "Collect " + Count + " x item " + ItemId + (Name.Length > 0 ? " (" + Name + ")" : "") + (TakeItem ? " (taken on finish)" : "") + (External ? " from another system" : (Drops.Count > 0 ? " drops: " + string.Join("; ", Drops.Select(d => d.ToString())) : " NO DROP SOURCE"));
                case ObjectiveKind.Gather: return "Gather " + Count + " near map " + Map + " (" + X + "," + Y + ") r" + Range + " [slot " + Slot + "]";
                case ObjectiveKind.Visit: return "Visit " + (Name.Length > 0 ? Name + " " : "") + "map " + Map + " (" + X + "," + Y + ") r" + Range + " [slot " + Slot + "]";
                case ObjectiveKind.State: return "State " + Stat + (Weapon.Length > 0 ? " " + Weapon : "") + " >= " + Min + " [slot " + Slot + "]";
                case ObjectiveKind.Event: return "Event '" + Event + "' x" + Count + " [slot " + Slot + "]";
                case ObjectiveKind.Counter: return "Counter (hand-written code) " + Count + " [slot " + Slot + "]";
                case ObjectiveKind.KillPoints: return "Kill points (" + (Points.Length > 0 ? Points : "monsterLevel") + ") " + Count + (Map != 0 ? " on map " + Map : "") + " [slot " + Slot + "]";
                default: return "Scripted step (engine can't judge): " + Text;
            }
        }
    }

    public sealed class PrizeItem
    {
        public uint Id;
        public ushort Count = 1;
        public bool Bound;
    }

    /// <summary>[QuestEngine] what a finish grants. [QuestEngineFix 2026-10-14] QuestGUI.FinishQuest no longer adds its
    /// 600 exp-ball to engine-owned / Bounty quests (QuestEngineCore.PaysFinishExpBall).</summary>
    public sealed class Prize
    {
        public ulong Exp;
        public ulong Silver;
        public uint Cps;
        public uint ExpBallUnits;          // GainExpBall units (600 = one EXP ball)
        public List<PrizeItem> Items = new List<PrizeItem>();
        public List<PrizeItem> FinalItems = new List<PrizeItem>(); // "Final prize": only when the quest ends its chain
        public bool RandomItem;            // "etc." / "chance" lists: one random item of Items
        public string Text = "";
        public bool FromJson;              // explicit quests.json prize (never trimmed / capped)

        public bool IsEmpty { get { return Exp == 0 && Silver == 0 && Cps == 0 && ExpBallUnits == 0 && Items.Count == 0 && FinalItems.Count == 0; } }
        public string Describe(bool chainEnd)
        {
            var parts = new List<string>();
            if (Exp > 0) parts.Add(Exp + " EXP");
            if (ExpBallUnits > 0) parts.Add((ExpBallUnits / 600.0).ToString("0.##", CultureInfo.InvariantCulture) + " EXP ball(s)");
            if (Silver > 0) parts.Add(Silver + " silver");
            if (Cps > 0) parts.Add(Cps + " CPs");
            foreach (var i in Items) parts.Add(QuestGrammar.ItemName(i.Id) + (i.Count > 1 ? " x" + i.Count : "") + (RandomItem ? "?" : ""));
            if (chainEnd) foreach (var i in FinalItems) parts.Add(QuestGrammar.ItemName(i.Id) + (i.Count > 1 ? " x" + i.Count : "") + " (final)");
            return parts.Count == 0 ? "-" : string.Join(", ", parts) + (RandomItem && Items.Count > 1 ? " (one of the items)" : "");
        }
    }

    /// <summary>[QuestEngine] a monster the engine spawns for one player (story boss / instance monster).</summary>
    public sealed class SpawnSpec
    {
        public string Name = "";        // as written
        public string Norm = "";        // normalized
        public uint Map;                // 0 = the instance / player's map
        public ushort X, Y;
        public byte Count = 1;
        public ushort Range = 2;
        public int Minutes = 10;
        public uint MonsterId;          // [NinjaChain 2026-10-14] "monsterId": this exact cq_monstertype id (two types share the name); 0 = by name
        public override string ToString() { return Name + (MonsterId != 0 ? "#" + MonsterId : "") + " x" + Count + " @" + Map + " (" + X + "," + Y + ") " + Minutes + "min"; }
    }

    /// <summary>[QuestEngine] onAccept / onComplete / onFinish actions.</summary>
    public sealed class QuestActions
    {
        public uint TeleportMap; public ushort TeleportX, TeleportY;
        public List<PrizeItem> Give = new List<PrizeItem>();
        public List<SpawnSpec> Spawn = new List<SpawnSpec>();
        public string Instance;          // enter a private copy of this instance
        public bool LeaveInstance;       // leave the quest's instance (onFinish leaves it by default)
        public bool IsEmpty { get { return TeleportMap == 0 && Give.Count == 0 && Spawn.Count == 0 && Instance == null && !LeaveInstance; } }
        public override string ToString()
        {
            var p = new List<string>();
            if (TeleportMap != 0) p.Add("teleport " + TeleportMap + " (" + TeleportX + "," + TeleportY + ")");
            if (Give.Count > 0) p.Add("give " + string.Join(",", Give.Select(g => g.Id + "x" + g.Count)));
            if (Spawn.Count > 0) p.Add("spawn " + string.Join(",", Spawn.Select(s => s.ToString())));
            if (Instance != null) p.Add("instance " + Instance);
            if (LeaveInstance) p.Add("leave instance");
            return string.Join("; ", p);
        }
    }

    /// <summary>[QuestEngine] rewards that are not items: Ninja gates, Archives war suits.</summary>
    public sealed class PrizeExtra
    {
        public bool NinjaUnlock;
        public List<string> NinjaGates = new List<string>();
        public List<string> Archives = new List<string>();
        public bool IsEmpty { get { return !NinjaUnlock && NinjaGates.Count == 0 && Archives.Count == 0; } }
        public override string ToString() { return (NinjaUnlock ? "ninja unlock " : "") + (NinjaGates.Count > 0 ? "gates " + string.Join(",", NinjaGates) + " " : "") + (Archives.Count > 0 ? "archives " + string.Join(",", Archives) : ""); }
    }

    /// <summary>[QuestEngine] a static teleport option at an NPC (quests.json "npcTeleports").</summary>
    public sealed class NpcTeleport
    {
        public string Text = "Teleport";
        public uint Map; public ushort X, Y;
        public ushort MinLevel;
        public uint RequireQuest;       // accepted or finished
        public string Instance;         // enter a private copy instead of the map itself
    }

    /// <summary>[QuestEngine] a per-player instance: a private DynamicID copy of an existing map.</summary>
    public sealed class InstanceDef
    {
        public string Name = "";
        public uint Map; public ushort X, Y;
        public uint ExitMap = 1002; public ushort ExitX = 439, ExitY = 478;
        public int Minutes = 30;
        public List<SpawnSpec> Monsters = new List<SpawnSpec>();
        public List<InstanceNpc> Npcs = new List<InstanceNpc>();   // [InstanceNpc] spawned inside every private copy
    }

    /// <summary>[InstanceNpc] an NPC that exists only inside the private copies of an instance (one object per copy,
    /// with the copy's DynamicID; removed with the copy). Clicks go through the normal NPC path (engine / festival).</summary>
    public sealed class InstanceNpc
    {
        public uint Uid; public uint Mesh; public byte Type = 2; public ushort X, Y; public string Name = "";
        public override string ToString() { return Uid + (Name.Length > 0 ? " " + Name : "") + " (" + X + "," + Y + ") mesh " + Mesh; }
    }

    /// <summary>[QuestEngine] a quest as the engine sees it.</summary>
    public sealed class EngineQuest
    {
        public uint MissionId;
        public string Name = "";
        public byte TypeId;
        public uint BeginNpc, FinishNpc;
        public uint FinishMap; public ushort FinishX, FinishY;
        public List<Objective> Objectives = new List<Objective>();
        public Prize Prize = new Prize();
        public bool Team;
        public bool HandCoded;      // referenced by hand-written code (kept, never double-handled)
        public bool SelfCounted;    // hand-written code already counts its objectives
        public bool Owned;          // the engine offers / tracks / finishes / rewards it
        public bool Disabled;       // quests.json "engine": false
        public bool Workable;       // every objective can be completed with the loaded data
        public bool FromJson;
        public bool FromJsonOwned;  // quests.json "engine": true
        public bool TakeItems = true;
        public bool ChainEnd = true;// no other quest needs it as a prerequisite (or quests.json "final")
        public bool FinalSet;       // quests.json "final" given
        public List<string> Problems = new List<string>();
        public string Content = "", Desc = "";
        public string BeginText, ProgressText, FinishText;
        public uint NextQuest;
        public bool AutoAccept;
        // ---- features (quests.json)
        public byte Repeat;         // 0 once, 1 daily, 2 weekly
        public bool RepeatExplicit;
        public bool OfferOnly;      // never finished by the engine
        public bool ForceCount;     // engine counting although the hand code counts too
        public QuestActions OnAccept, OnComplete, OnFinish;
        public PrizeExtra Extra;
        public string Pool;
        public bool RequireClan;
        public string PrequestOverride; // Questinfo syntax, applied to DBQuest after each load
        public bool BeginOverride;      // quests.json beginNpc (also written to DBQuest.BeginNpcId)
        public List<uint> ServerTypes;  // Questinfo ShowServerType (null = every server type)
        public bool AllowParsedCps;
        public uint[] Prequests = new uint[0];
        // ---- [Festivals] overlay (Events\Framework): the festival that owns this quest; FestivalOff = that festival
        // is not running -> the quest is Disabled (no offer / counting / finish / prize) but keeps its daily/weekly reset
        public string Festival;
        public bool FestivalOff;
        // ---- [QuestEngineFix 2026-10-14]
        public int IntentionLines;          // non-empty Intention lines in Questinfo.ini
        public string IntentionText = "";   // those lines, joined
        public bool ObjectivesExplicit;     // quests.json gave an "objectives" array (even an empty one)
        public string OfficialPrizeText = "";// Questinfo.ini Prize text (kept even when quests.json replaces the prize)

        /// <summary>The generic kill/drop/visit counter may move this quest (owned, or hand-coded but never counted by its code).</summary>
        public bool EngineCounts { get { return !Disabled && (Owned || (HandCoded && (!SelfCounted || ForceCount))); } }
        public bool IsBounty { get { return Role.Instance.Quests.IsBountyHallMission(MissionId); } }
        public byte MaxSlot { get { byte m = 0; foreach (var o in Objectives) if (o.Slot > m) m = o.Slot; return m; } }
        public IEnumerable<QuestActions> AllActions { get { if (OnAccept != null) yield return OnAccept; if (OnComplete != null) yield return OnComplete; if (OnFinish != null) yield return OnFinish; } }
    }

    /// <summary>[QuestEngine] one loaded snapshot (published by reference swap on reload).</summary>
    public sealed class QuestTable
    {
        public Dictionary<uint, EngineQuest> Quests = new Dictionary<uint, EngineQuest>();
        public Dictionary<uint, List<EngineQuest>> ByBeginNpc = new Dictionary<uint, List<EngineQuest>>();
        public Dictionary<uint, List<EngineQuest>> ByFinishNpc = new Dictionary<uint, List<EngineQuest>>();
        public Dictionary<uint, List<DropRule>> GlobalDrops = new Dictionary<uint, List<DropRule>>();
        public Dictionary<uint, uint> NpcAliases = new Dictionary<uint, uint>();          // quest npc -> server npc
        public Dictionary<uint, List<NpcTeleport>> NpcTeleports = new Dictionary<uint, List<NpcTeleport>>();
        public Dictionary<string, InstanceDef> Instances = new Dictionary<string, InstanceDef>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, List<EngineQuest>> Pools = new Dictionary<string, List<EngineQuest>>(StringComparer.OrdinalIgnoreCase);
        public uint ServerType;
        public int Lines, LinesParsed, Objectives, ObjectivesTyped;
        public Dictionary<ObjectiveKind, int> KindCounts = new Dictionary<ObjectiveKind, int>();
        public int Owned, Workable, HandCoded, KillCounted, JsonQuests;
        public int TalkSlotsFixed, ChainPrizesTrimmed, ParsedCpsBlocked, HiddenByServerType;
        public string JsonError;
        public DateTime Loaded = DateTime.Now;

        public string Summary()
        {
            double cov = Objectives == 0 ? 0 : 100.0 * ObjectivesTyped / Objectives;
            return Quests.Count + " quests, " + Objectives + " objectives (" + cov.ToString("0.0", CultureInfo.InvariantCulture) + "% typed: " +
                   string.Join(" ", KindCounts.OrderBy(k => k.Key).Select(k => k.Key + "=" + k.Value)) + "), engine-owned " + Owned + " (workable " + Workable + "), hand-coded " + HandCoded +
                   " (engine kill counting for " + KillCounted + "), quests.json entries " + JsonQuests +
                   ", talk slots fixed " + TalkSlotsFixed + ", chain prizes trimmed " + ChainPrizesTrimmed + ", parsed CPs blocked " + ParsedCpsBlocked +
                   ", hidden by server type " + HiddenByServerType + ", aliases " + NpcAliases.Count + ", instances " + Instances.Count +
                   (JsonError != null ? " [quests.json ERROR: " + JsonError + "]" : "");
        }
    }

    /// <summary>[QuestEngine] Intention / Prize grammar and the loader.</summary>
    public static class QuestGrammar
    {
        static readonly Regex Token = new Regex(@"\[\s*([A-Za-z_]+)[ ,]([^\[\]]*)\]", RegexOptions.Compiled);
        static readonly Regex KillVerb = new Regex(@"\b(kill|kills|killing|defeat|defeating|eliminate|slay|hunt|destroy|beat|exterminate|vanquish|wipe out|subdue|slaughter|take down|get rid of|expel|clear|drive away|raze|purge|smash)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex CollectVerb = new Regex(@"\b(collect|gather|obtain|retrieve|get|catch|loot|bring|acquire|fetch|pick up|grab)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex TalkVerb = new Regex(@"\b(talk|speak|visit|meet|ask|report|tell|inform|find|look for|question|reply|consult|greet|return to|go to|give|send|deliver|submit|hand)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex KillCountText = new Regex(@"\b(?:kill|defeat|slay|eliminate)\s+(\d+)\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex KillNameText = new Regex(@"\b(?i:kill|defeat|eliminate|slay|beat)\s+(?:\d+\s+)?(?:the\s+)?((?:[A-Z][\w`']*\s*){1,3})", RegexOptions.Compiled);
        static readonly Regex LevelSuffix = new Regex(@"^(.*?)(l\d+)$", RegexOptions.Compiled);
        static readonly Regex Words = new Regex(@"[A-Za-z]+", RegexOptions.Compiled);
        static readonly Regex IntentionKey = new Regex(@"^Intention(\d+)(_\d+)?$", RegexOptions.Compiled);

        /// <summary>Event keys that a server system really raises (QuestEngineCore.OnEvent call sites). An event
        /// objective with another key is "not workable" (it could never complete). "useItem:&lt;id&gt;" is always raised.</summary>
        public static readonly HashSet<string> KnownEvents = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "bountyDone" };

        /// <summary>Maps whose monsters come only from MonsterSpawns\MonstersXXXX.txt (+ myMonsters.txt): mirrors the
        /// point-file branch of Role\GameMap.cs (their MonsterSpawns\map\name\*.ini folders are NOT loaded).</summary>
        public static readonly Dictionary<uint, string> PointFileMaps = new Dictionary<uint, string>
        {
            {1002,"Monsters1002.txt"},{1020,"Monsters1020.txt"},{1011,"Monsters1011.txt"},{1768,"Monsters1768.txt"},{4460,"GameServer[EU]Monster.txt"},
            {10005,"Monsters10005.txt"},{10006,"Monsters10006.txt"},{10007,"Monsters10007.txt"},{3819,"Monsters3819.txt"},{10008,"Monsters10008.txt"},
            {10040,"Monsters10040.txt"},{10041,"Monsters10041.txt"},{10042,"Monsters10042.txt"},{10043,"Monsters10043.txt"},{10044,"Monsters10044.txt"},
            {10045,"Monsters10045.txt"},{10020,"Monsters10020.txt"},{1000,"Monsters1000.txt"},{1779,"Monsters1779.txt"},{1780,"Monsters1780.txt"},
            {3935,"Monsters3935.txt"},{10137,"Monsters10137.txt"},{3998,"Monsters3998.txt"},{10089,"Monsters10089.txt"},{1015,"Monsters1015.txt"},
            {1016,"Monsters1016.txt"},{3845,"Monsters3845.txt"},{10250,"Monsters10250.txt"},
        };

        // ------------------------------------------------------------ names
        public static string Norm(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            var sb = new StringBuilder(name.Length);
            foreach (char c in name)
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) sb.Append(c);
                else if (c >= 'A' && c <= 'Z') sb.Append((char)(c + 32));
            return sb.ToString();
        }
        public static string Singular(string n)
        {
            if (n.EndsWith("men")) return n.Substring(0, n.Length - 3) + "man";
            if (n.EndsWith("ies")) return n.Substring(0, n.Length - 3) + "y";
            if (n.EndsWith("es") && (n.EndsWith("sses") || n.EndsWith("shes") || n.EndsWith("ches") || n.EndsWith("xes"))) return n.Substring(0, n.Length - 2);
            if (n.EndsWith("s") && !n.EndsWith("ss")) return n.Substring(0, n.Length - 1);
            return n;
        }
        /// <summary>Same monster: exact normalized name, or one is the other plus a level suffix (BladeGhost / BladeGhostL83).</summary>
        public static bool SameMonster(string objective, string killed)
        {
            if (objective == killed) return true;
            var m = LevelSuffix.Match(killed);
            if (m.Success && m.Groups[1].Value == objective) return true;
            return false;
        }
        static string TokenMonster(string name, HashSet<string> monsters)
        {
            string n = Norm(name);
            if (n.Length == 0) return "";
            var m = LevelSuffix.Match(n);
            string core = m.Success ? m.Groups[1].Value : n, lv = m.Success ? m.Groups[2].Value : "";
            foreach (string c in new[] { core + lv, Singular(core) + lv, Singular(core) })
                if (monsters.Contains(c)) return c;
            if (monsters.Contains(n)) return n;
            return Singular(core) + lv;
        }
        public static string ItemName(uint id)
        {
            Database.ItemType.DBItem it;
            if (Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out it) && it != null && !string.IsNullOrEmpty(it.Name))
                return it.Name;
            return "item " + id;
        }

        static List<string> Fields(string body)
        {
            return body.Split(',').Select(p => p.Trim()).ToList();
        }
        static uint U(string s) { uint v; return uint.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }
        static int I(string s) { int v; return int.TryParse((s ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        /// <summary>QahrMobile: placeholder NPC id of a talk step named only by a mobile "[lua Name,...]" link.</summary>
        public const int LuaNpcByName = int.MaxValue;

        // ------------------------------------------------------------ one Intention line
        /// <summary>Parses one Intention line into objectives (empty list = description only).</summary>
        public static List<Objective> ParseLine(uint missionId, int lineNo, string text, HashSet<string> monsters)
        {
            var result = new List<Objective>();
            if (string.IsNullOrWhiteSpace(text)) return result;
            var chkQuest = new List<List<string>>(); var chkItem = new List<List<string>>();
            var npcs = new List<List<string>>(); var mobs = new List<List<string>>(); var itemsT = new List<List<string>>();
            var luas = new List<List<string>>();
            foreach (Match m in Token.Matches(text))
            {
                var f = Fields(m.Groups[2].Value);
                switch (m.Groups[1].Value.ToLowerInvariant())
                {
                    case "chk_quest": chkQuest.Add(f); break;
                    case "chk_item": chkItem.Add(f); break;
                    case "npc": npcs.Add(f); break;
                    case "mob": mobs.Add(f); break;
                    case "item_t": itemsT.Add(f); break;
                    case "lua": luas.Add(f); break;// QahrMobile: mobile questinfo link "[lua Name,</F>LuaFunc</N>arg]"
                }
            }
            // QahrMobile: a mobile line names its target only in a [lua Name,...] link (the client runs a Lua function on
            // click): a monster name becomes a [mob], anything else an NPC by name (LuaNpcByName, resolved in Build
            // against the quest's finish / begin NPC, else the step stays scripted)
            if (luas.Count > 0 && npcs.Count == 0 && mobs.Count == 0 && itemsT.Count == 0 && luas[0].Count > 0 && luas[0][0].Length > 0)
            {
                string ln = luas[0][0];
                if (monsters.Contains(TokenMonster(ln, monsters))) mobs.Add(new List<string> { ln });
                else npcs.Add(new List<string> { ln, LuaNpcByName.ToString(CultureInfo.InvariantCulture) });
            }
            string plain = Token.Replace(text, " ");
            int slot = lineNo; uint count = 1;
            if (chkQuest.Count > 0)
            {
                var a = chkQuest[0];
                if (a.Count >= 4)
                {
                    int s = I(a[2]); if (s > 0) slot = s;
                    count = U(a[3]);
                }
            }
            if (slot < 1) slot = 1; if (slot > 32) slot = 32;
            // [QuestEngineFix 2026-10-14] "Reach Character [npc Level30,2001,...][chk_quest 3555,1,1,30]" is a level
            // requirement, not "talk to NPC 2001" (that paid the prize at any level)
            if (chkItem.Count == 0)
            {
                int reach = ReachLevel(plain, npcs);
                if (reach > 0)
                {
                    result.Add(new Objective { Kind = ObjectiveKind.State, Stat = "level", Min = (uint)reach, Count = count > 1 ? count : 1, Slot = (byte)slot, Name = "Reach level " + reach, Text = text, Line = lineNo });
                    return result;
                }
            }
            // a location: first item_t / mob / npc token with numeric map,x,y (npc: skip the id)
            uint lmap = 0; ushort lx = 0, ly = 0; bool hasLoc = false;
            foreach (var t in itemsT.Concat(mobs).Concat(npcs))
            {
                var nums = new List<int>();
                for (int i = 1; i < t.Count; i++) { int v; if (int.TryParse(t[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) nums.Add(v); }
                if (npcs.Contains(t) && nums.Count > 0) nums.RemoveAt(0);
                if (nums.Count >= 3) { if (nums[0] > 0) { lmap = (uint)nums[0]; lx = (ushort)Math.Max(0, nums[1]); ly = (ushort)Math.Max(0, nums[2]); hasLoc = true; } break; }
            }
            // monster targets: a [mob] that is a monster, else a monster named in the text, else the [mob] name, else "Kill Some Name"
            var targets = new List<string>(); string targetName = "";
            foreach (var t in mobs)
            {
                if (t.Count == 0) continue;
                string n = TokenMonster(t[0], monsters);
                if (monsters.Contains(n)) { targets.Add(n); targetName = t[0]; }
            }
            if (targets.Count == 0)
            {
                var w = Words.Matches(plain).Cast<Match>().Select(m => m.Value).ToList();
                string best = null;
                for (int size = 3; size >= 1 && best == null; size--)
                    for (int i = 0; i + size <= w.Count && best == null; i++)
                    {
                        string n = Norm(string.Concat(w.Skip(i).Take(size)));
                        foreach (string c in new[] { n, Singular(n) })
                            if (c.Length >= 4 && monsters.Contains(c)) { best = c; targetName = string.Join(" ", w.Skip(i).Take(size)); break; }
                    }
                if (best != null) targets.Add(best);
            }
            if (targets.Count == 0 && mobs.Count > 0 && mobs[0].Count > 0 && mobs[0][0].Length > 0)
            { targets.Add(TokenMonster(mobs[0][0], monsters)); targetName = mobs[0][0]; }
            if (targets.Count == 0)
            {
                var mk = KillNameText.Match(plain);
                if (mk.Success) { targets.Add(Singular(Norm(mk.Groups[1].Value))); targetName = mk.Groups[1].Value.Trim(); }
            }
            bool killVerb = KillVerb.IsMatch(plain), collectVerb = CollectVerb.IsMatch(plain), talkVerb = TalkVerb.IsMatch(plain);

            if (chkItem.Count > 0)
            {
                foreach (var a in chkItem)
                {
                    uint item = a.Count > 0 ? U(a[0]) : 0;
                    if (item == 0) continue;
                    uint c = a.Count > 1 ? U(a[1]) : 1;
                    var o = new Objective { Kind = ObjectiveKind.Collect, ItemId = item, Count = Math.Max(1, c), Text = text, Name = itemsT.Count > 0 && itemsT[0].Count > 0 ? itemsT[0][0].Replace('~', ' ') : "" };
                    // which monster drops it: a monster named on the line; else monsters near the [item_t] point
                    if (targets.Count > 0 && monsters.Contains(targets[0]))
                        o.Drops.Add(new DropRule { Monster = targets[0] });
                    else if (hasLoc)
                        o.Drops.Add(new DropRule { Map = lmap, X = lx, Y = ly, Radius = 60 });
                    o.Map = lmap; o.X = lx; o.Y = ly;
                    result.Add(o);
                }
                return result;
            }
            if (count == 0) count = 1;
            if (chkQuest.Count > 0 || killVerb || npcs.Count > 0 || itemsT.Count > 0 || mobs.Count > 0)
            {
                bool mobIsMonster = mobs.Count > 0 && mobs[0].Count > 0 && monsters.Contains(TokenMonster(mobs[0][0], monsters));
                if (targets.Count > 0 && (killVerb || (collectVerb && npcs.Count == 0) || (mobIsMonster && !talkVerb)) && (chkQuest.Count > 0 || KillCountText.IsMatch(plain)))
                {
                    if (chkQuest.Count == 0) { var mc = KillCountText.Match(plain); count = Math.Max(1, U(mc.Groups[1].Value)); slot = lineNo; }
                    result.Add(new Objective { Kind = ObjectiveKind.Kill, Monster = targets[0], Name = targetName, Count = count, Slot = (byte)slot, Map = lmap, X = lx, Y = ly, Text = text });
                }
                else if (npcs.Count > 0)
                {
                    var t = npcs[0];
                    int id = t.Count > 1 ? I(t[1]) : -1;
                    if (t.Count > 0 && t[0].Length == 0 && t.Count > 2) id = I(t[2]);// "[npc,Seal,-1,...]"
                    // a talk line without [chk_quest] gets slot 0 here; Build gives it a free slot unless it is the finish NPC (ER-16)
                    result.Add(new Objective { Kind = ObjectiveKind.Talk, NpcId = id > 0 ? (uint)id : 0, Name = t.Count > 0 ? t[0] : "", Count = count, Slot = (byte)(chkQuest.Count > 0 ? slot : 0), Map = lmap, X = lx, Y = ly, Text = text });
                }
                else if (itemsT.Count > 0 && collectVerb && chkQuest.Count > 0)
                {
                    result.Add(new Objective { Kind = hasLoc ? ObjectiveKind.Gather : ObjectiveKind.Unknown, Count = count, Slot = (byte)slot, Map = lmap, X = lx, Y = ly, Range = 40, Chance = 5000, Name = itemsT[0].Count > 0 ? itemsT[0][0] : "", Text = text });
                }
                else if ((mobs.Count > 0 || itemsT.Count > 0) && chkQuest.Count > 0)
                {
                    var t = mobs.Count > 0 ? mobs[0] : itemsT[0];
                    result.Add(new Objective { Kind = hasLoc ? ObjectiveKind.Visit : ObjectiveKind.Unknown, Count = count, Slot = (byte)slot, Map = lmap, X = lx, Y = ly, Range = 8, Name = t.Count > 0 ? t[0].Replace('~', ' ') : "", Text = text });
                }
                else if (chkQuest.Count > 0)
                    result.Add(new Objective { Kind = ObjectiveKind.Unknown, Count = count, Slot = (byte)slot, Text = text });
            }
            foreach (var o in result) o.Line = lineNo;
            return result;
        }

        /// <summary>[QuestEngineFix] item-handling verbs (CollectVerb without "get": "check if the toads get hurt" is no collect).</summary>
        static readonly Regex ItemVerb = new Regex(@"\b(collect|gather|obtain|retrieve|catch|loot|bring|acquire|fetch|pick up|grab)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex ReachVerb =new Regex(@"^\s*reach\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex LevelNpcName = new Regex(@"^\s*Level\s*(\d{1,3})\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex ReachLevelText = new Regex(@"^\s*reach\s+(?:character\s+)?level\s*(\d{1,3})\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>[QuestEngineFix] a line that starts with "Reach Character [npc Level30,...]" / "Reach level 30": the
        /// level, else 0 ("Any Taoist Tool reaches level 20" is not the character level).</summary>
        static int ReachLevel(string plain, List<List<string>> npcs)
        {
            if (!ReachVerb.IsMatch(plain)) return 0;
            int lv = 0;
            foreach (var t in npcs)
            {
                if (t.Count == 0) continue;
                var m = LevelNpcName.Match(t[0]);
                if (m.Success) { lv = I(m.Groups[1].Value); break; }
            }
            if (lv == 0)
            {
                var m = ReachLevelText.Match(plain);
                if (m.Success) lv = I(m.Groups[1].Value);
            }
            return lv >= 1 && lv <= 140 ? lv : 0;
        }

        // ------------------------------------------------------------ Prize
        static readonly Regex PrizeItemToken = new Regex(@"\[item\s*(\d+)\s*,([^\]]*)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex ExpNumber = new Regex(@"EXP:\s*([\d,]+)|([\d,]{2,})\s*EXP\b(?!\s*Ball)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex ExpMinutes = new Regex(@"(\d+)\s*(?:-\s*min|minutes?\s+of)\s*EXP", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex ExpBalls = new Regex(@"(\d+)\s*EXP\s*Balls?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex SilverNumber = new Regex(@"([\d,]{2,})\s*Silver|(?:Gold|Silver):\s*([\d,]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);// [QuestEngineFix] + "Silver: 1500"
        static readonly Regex CpsNumber = new Regex(@"([\d,]{2,})\s*CPs?\b(?!\s*\()", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>Parsed (free-text) silver above this is never paid without quests.json "allowParsedCps": true (ER-9).</summary>
        public const ulong ParsedSilverCap = 1000000;

        static ulong Num(string s) { ulong v; return ulong.TryParse((s ?? "").Replace(",", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0; }

        // [QuestEngineFix 2026-10-14] "EXP: 1/30 of an EXP Ball", "EXP worth 1/3 EXP Ball", "2/15 or 11/60 of an EXP Ball"
        static readonly Regex ExpFraction = new Regex(@"(\d+)\s*/\s*(\d+)(?:\s*or\s*\d+\s*/\s*\d+)?\s*(?:of\s+an?\s+)?EXP\s*Balls?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // [QuestEngineFix 2026-10-14] item counts written in the text: "5 [item ...]", "3 Stanchers", "StanchersX3", "[item ...] x3"
        static readonly Regex CountRightBefore = new Regex(@"(?<![\d,./:+])(\d{1,4})\s*(?:[xX]\s*)?$", RegexOptions.Compiled);
        static readonly Regex CountRightAfter = new Regex(@"^\s*[xX]\s*(\d{1,4})\b", RegexOptions.Compiled);
        static readonly Regex NotACountWord = new Regex(@"^(level|lv|lvl|gold|silver|exp|cps?|minutes?|points?|plus|stage|phase|step|day|days)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex CountThenName = new Regex(@"(?<![\d,./:+])\b(\d{1,4})\s*(?:[xX]\s+)?((?:[A-Za-z'`]+[ \t]*){1,4})", RegexOptions.Compiled);
        static readonly Regex NameThenCount = new Regex(@"([A-Za-z]{3,})\s*[xX]\s*(\d{1,4})\b", RegexOptions.Compiled);

        /// <summary>A count written right before an [item] token ("5 [item ...]"), not a "Gold: 200" / "Level 40" number.</summary>
        static uint CountBefore(string seg)
        {
            var m = CountRightBefore.Match(seg);
            if (!m.Success) return 0;
            string before = seg.Substring(0, m.Index).TrimEnd();
            if (before.EndsWith(":")) return 0;
            var w = Regex.Match(before, @"([A-Za-z]+)$");
            if (w.Success && NotACountWord.IsMatch(w.Groups[1].Value)) return 0;
            return U(m.Groups[1].Value);
        }
        /// <summary>A count written next to the item's name in the text ("3 Stanchers", "2 Twin City Gates", "StanchersX3").</summary>
        static uint CountByName(string plain, uint itemId)
        {
            string n = Norm(ItemName(itemId));
            if (n.Length < 3 || n.StartsWith("item")) return 0;
            foreach (Match m in CountThenName.Matches(plain))
            {
                var words = Words.Matches(m.Groups[2].Value).Cast<Match>().Select(x => x.Value).ToList();
                for (int len = words.Count; len >= 1; len--)
                    for (int s = 0; s < len; s++)// "2 Elite Pride Fans" = PrideFan
                    {
                        string cand = Norm(string.Concat(words.Skip(s).Take(len - s)));
                        if (cand == n || Singular(cand) == n) return U(m.Groups[1].Value);
                    }
            }
            foreach (Match m in NameThenCount.Matches(plain))
            {
                string cand = Norm(m.Groups[1].Value);
                if (cand == n || Singular(cand) == n) return U(m.Groups[2].Value);
            }
            return 0;
        }

        public static Prize ParsePrize(string text)
        {
            var p = new Prize { Text = text ?? "" };
            if (string.IsNullOrWhiteSpace(text)) return p;
            string plain = PrizeItemToken.Replace(text, " ");
            int finalAt = text.IndexOf("final", StringComparison.OrdinalIgnoreCase);
            int prevEnd = 0;
            foreach (Match m in PrizeItemToken.Matches(text))
            {
                string seg = text.Substring(prevEnd, m.Index - prevEnd);
                int nl = seg.LastIndexOf("\\n", StringComparison.Ordinal);
                if (nl >= 0) seg = seg.Substring(nl + 2);
                string after = text.Substring(m.Index + m.Length);
                prevEnd = m.Index + m.Length;
                uint id = U(m.Groups[1].Value);
                if (id == 0) continue;
                var f = Fields(m.Groups[2].Value);
                uint cnt = f.Count > 0 ? U(f[0]) : 0;// [item id,count,0,0,flag,0,0] (count is 0 in all the official data)
                if (cnt == 0)
                {
                    // [QuestEngineFix] the count written in the text
                    var ma = CountRightAfter.Match(after);
                    cnt = ma.Success ? U(ma.Groups[1].Value) : 0;
                    if (cnt == 0) cnt = CountBefore(seg);
                    if (cnt == 0) cnt = CountByName(plain, id);
                }
                var it = new PrizeItem { Id = id, Count = (ushort)Math.Max(1, Math.Min(cnt == 0 ? 1 : cnt, 1000)), Bound = f.Count > 3 && U(f[3]) != 0 };
                if (finalAt >= 0 && m.Index > finalAt) p.FinalItems.Add(it); else p.Items.Add(it);
            }
            p.RandomItem = p.Items.Count > 1 && Regex.IsMatch(plain, @"\betc\b|\bchance\b|\brandom\b|\bone of\b", RegexOptions.IgnoreCase);
            string beforeFinal = finalAt >= 0 ? plain.Substring(0, Math.Min(plain.Length, finalAt)) : plain;
            // [QuestEngineFix] a fraction of an EXP ball (600 units = one ball); then neither "6 EXP Ball" (of "5/6 EXP
            // Ball") nor "1 EXP" (of "EXP: 1/30") nor the silver number before "EXP:" is read as the EXP
            var mf = ExpFraction.Match(beforeFinal);
            if (mf.Success)
            {
                ulong num = Num(mf.Groups[1].Value), den = Num(mf.Groups[2].Value);
                if (den > 0 && num > 0) p.ExpBallUnits += (uint)Math.Min(100000UL, Math.Max(1UL, 600UL * num / den));
                beforeFinal = ExpFraction.Replace(beforeFinal, " ");
            }
            var mm = ExpMinutes.Match(beforeFinal);
            if (mm.Success) p.ExpBallUnits += (uint)Math.Min(100000, Num(mm.Groups[1].Value) * 10);// 60 min = 600 = one EXP ball
            var mb = ExpBalls.Match(beforeFinal);
            if (mb.Success) p.ExpBallUnits += (uint)Math.Min(100000, Num(mb.Groups[1].Value) * 600);
            if (!mm.Success && !mf.Success)
            {
                var me = ExpNumber.Match(beforeFinal);
                if (me.Success) p.Exp = Num(me.Groups[1].Success && me.Groups[1].Length > 0 ? me.Groups[1].Value : me.Groups[2].Value);
            }
            var ms = SilverNumber.Match(beforeFinal);
            if (ms.Success) p.Silver = Num(ms.Groups[1].Success && ms.Groups[1].Length > 0 ? ms.Groups[1].Value : ms.Groups[2].Value);
            var mc = CpsNumber.Match(beforeFinal);
            if (mc.Success) p.Cps = (uint)Math.Min(1000000UL, Num(mc.Groups[1].Value));
            return p;
        }

        // ------------------------------------------------------------ Questinfo.ini reader
        /// <summary>Numeric sections of Questinfo.ini as key->value (first occurrence of a key wins, like the profile API).</summary>
        public static List<Dictionary<string, string>> ReadSections(string path)
        {
            var list = new List<Dictionary<string, string>>();
            Dictionary<string, string> cur = null;
            foreach (var raw in File.ReadLines(path, WindowsAPI.IniFile.FileEncoding))
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;
                if (line[0] == '[')
                {
                    int close = line.IndexOf(']');
                    uint n;
                    cur = null;
                    if (close > 1 && uint.TryParse(line.Substring(1, close - 1).Trim(), out n))
                    {
                        cur = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        list.Add(cur);
                    }
                    continue;
                }
                if (cur == null) continue;
                int eq = raw.IndexOf('=');
                if (eq <= 0) continue;
                string key = raw.Substring(0, eq).Trim();
                if (!cur.ContainsKey(key)) cur[key] = raw.Substring(eq + 1).Trim();
            }
            return list;
        }

        static string Get(Dictionary<string, string> s, string key) { string v; return s.TryGetValue(key, out v) ? v : ""; }

        // ------------------------------------------------------------ monsters / spawns
        /// <summary>Normalized names of all monster types (Pool.MonsterFamilies, or Monsters\*.ini when the pool is empty).</summary>
        public static HashSet<string> MonsterNames()
        {
            var set = new HashSet<string>();
            try
            {
                if (Pool.MonsterFamilies != null && Pool.MonsterFamilies.Count > 0)
                {
                    foreach (var f in Pool.MonsterFamilies.Values.ToArray())
                        if (f != null && !string.IsNullOrEmpty(f.Name)) set.Add(Norm(f.Name));
                    return set;
                }
                string dir = Path.Combine(Program.ServerConfig.DbLocation, "Monsters");
                if (Directory.Exists(dir))
                    foreach (string f in Directory.GetFiles(dir, "*.ini"))
                        foreach (string l in File.ReadLines(f, WindowsAPI.IniFile.FileEncoding))
                            if (l.StartsWith("name=", StringComparison.OrdinalIgnoreCase)) { set.Add(Norm(l.Substring(5))); break; }
            }
            catch (Exception e) { MyConsole.WriteLine("[QuestEngine] monster names: " + e.Message); }
            return set;
        }
        /// <summary>
        /// Normalized names of the monster types that really spawn: MonsterSpawns\map\name\*.ini (only for maps that
        /// read those folders), the MonstersXXXX.txt point files of the point-file maps (ER-6) and myMonsters.txt.
        /// NewMonster.txt is kept for compatibility.
        /// </summary>
        // [FastBoot 2026-10-15] boot-only memo of the MonsterSpawns file scan (see SpawnedMonsterNames). Program.Main
        // switches it off (and drops the memo) right after the festivals' boot rebuild.
        sealed class SpawnScan { public string Root; public List<uint> Ids; public List<string> Names; }
        static volatile SpawnScan _bootSpawnScan;
        static volatile bool _bootSpawnMemo;
        public static bool BootSpawnMemo { get { return _bootSpawnMemo; } set { _bootSpawnMemo = value; if (!value) _bootSpawnScan = null; } }

        public static HashSet<string> SpawnedMonsterNames()
        {
            var ids = new HashSet<uint>();
            var names = new HashSet<string>();
            try
            {
                string root = Path.Combine(Program.ServerConfig.DbLocation, "MonsterSpawns");
                var memo = BootSpawnMemo ? _bootSpawnScan : null;// [FastBoot 2026-10-15]
                if (memo != null && memo.Root == root && Directory.Exists(root))
                {
                    // [FastBoot 2026-10-15] the boot builds the table twice (QuestEngine.Load, then the festival overlays rebuild
                    // ~1 s later); the spawn files can't change in between, so the second build reuses the first file scan
                    // (same ids/names, same insertion order). Off after boot (Program.Main), so reloads always re-read the files.
                    foreach (uint id in memo.Ids) ids.Add(id);
                    foreach (string nm in memo.Names) names.Add(nm);
                }
                else if (Directory.Exists(root))
                {
                    foreach (string mapDir in Directory.GetDirectories(root))
                    {
                        uint mapId = U(Path.GetFileName(mapDir));
                        if (PointFileMaps.ContainsKey(mapId)) continue;// that map loads its point file instead
                        foreach (string mobDir in Directory.GetDirectories(mapDir))
                            foreach (string f in Directory.GetFiles(mobDir))
                                foreach (string l in File.ReadLines(f, WindowsAPI.IniFile.FileEncoding))
                                    if (l.StartsWith("npctype=", StringComparison.OrdinalIgnoreCase)) { ids.Add(U(l.Substring(8))); break; }
                    }
                    foreach (var pair in new[] { new KeyValuePair<string, int>("myMonsters.txt", 9), new KeyValuePair<string, int>("NewMonster.txt", 8) })
                    {
                        string f = Path.Combine(root, pair.Key);
                        if (!File.Exists(f)) continue;
                        foreach (string l in File.ReadLines(f))
                        {
                            var a = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (a.Length > pair.Value) ids.Add(U(a[pair.Value]));
                        }
                    }
                    // point files (uid,type,name,x,y,map): the server spawns by NAME? no - by type id; keep both
                    foreach (var pf in PointFileMaps.Values.Distinct())
                    {
                        string f = Path.Combine(root, pf);
                        if (!File.Exists(f)) continue;
                        foreach (string l in File.ReadLines(f))
                        {
                            var a = l.Split(',');
                            if (a.Length < 6) continue;
                            ids.Add(U(a[1]));
                            if (a[2].IndexOf("Titan", StringComparison.Ordinal) < 0) names.Add(Norm(a[2]));
                        }
                    }
                    if (BootSpawnMemo)// [FastBoot 2026-10-15] keep the completed scan for the boot's second build
                        _bootSpawnScan = new SpawnScan { Root = root, Ids = new List<uint>(ids), Names = new List<string>(names) };
                }
                if (Pool.MonsterFamilies != null && Pool.MonsterFamilies.Count > 0)
                {
                    foreach (uint id in ids)
                    {
                        Game.MsgMonster.MonsterFamily fam;
                        if (Pool.MonsterFamilies.TryGetValue(id, out fam) && fam != null) names.Add(Norm(fam.Name));
                    }
                }
                else
                {
                    string dir = Path.Combine(Program.ServerConfig.DbLocation, "Monsters");
                    if (Directory.Exists(dir))
                        foreach (string f in Directory.GetFiles(dir, "*.ini"))
                        {
                            uint id = 0; string nm = null;
                            foreach (string l in File.ReadLines(f, WindowsAPI.IniFile.FileEncoding))
                            {
                                if (l.StartsWith("id=", StringComparison.OrdinalIgnoreCase)) id = U(l.Substring(3));
                                else if (l.StartsWith("name=", StringComparison.OrdinalIgnoreCase)) nm = l.Substring(5);
                                if (id != 0 && nm != null) break;
                            }
                            if (id != 0 && nm != null && ids.Contains(id)) names.Add(Norm(nm));
                        }
                }
            }
            catch (Exception e) { MyConsole.WriteLine("[QuestEngine] monster spawns: " + e.Message); }
            return names;
        }

        /// <summary>Ids of the NPCs placed by data (Npcs.txt + SobNpcs.txt); null when the files can't be read (no check).</summary>
        public static HashSet<uint> PlacedNpcIds()
        {
            try
            {
                var set = new HashSet<uint>();
                bool any = false;
                foreach (string name in new[] { "Npcs.txt", "SobNpcs.txt" })
                {
                    string f = Path.Combine(Program.ServerConfig.DbLocation, name);
                    if (!File.Exists(f)) continue;
                    any = true;
                    foreach (string l in File.ReadLines(f))
                    {
                        int comma = l.IndexOf(',');
                        if (comma > 0) { uint id = U(l.Substring(0, comma)); if (id != 0) set.Add(id); }
                    }
                }
                return any ? set : null;
            }
            catch { return null; }
        }

        static bool Known(HashSet<string> set, string norm)
        {
            return set.Contains(norm) || set.Any(n => SameMonster(norm, n));
        }
        /// <summary>The map can be entered (GameMap.dat, directly or through a doc remap in GameMap.ResolveBase).</summary>
        public static bool MapKnown(uint map)
        {
            if (map == 0) return false;
            if (Role.GameMap.MapContents == null || Role.GameMap.MapContents.Count == 0) return true;// not loaded (tools): don't judge
            return Role.GameMap.MapContents.ContainsKey((int)Role.GameMap.ResolveBase(map));
        }

        /// <summary>[QuestEngineFix] a drop rule can fire: its monster exists and spawns (or the engine spawns it), and
        /// its map (when given) exists.</summary>
        static bool DropPossible(DropRule r, HashSet<string> monsters, HashSet<string> spawned)
        {
            if (r.Map != 0 && !MapKnown(r.Map)) return false;
            if (r.Monster.Length > 0) return Known(monsters, r.Monster) && Known(spawned, r.Monster);
            return r.Map != 0;
        }

        /// <summary>[QuestEngineFix] walkability results by map file stamp + point + range (maps are read once, not loaded).</summary>
        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> PlaceCache = new System.Collections.Concurrent.ConcurrentDictionary<string, bool>();
        /// <summary>
        /// [QuestEngineFix 2026-10-14] Visit / Gather objectives: the map must exist and a walkable DMap cell must lie within
        /// the objective's range of its point. The DMap file is read directly (the map is NOT loaded, so no NPCs / monsters
        /// are spawned by a check). Objective -> problem text.
        /// </summary>
        static Dictionary<Objective, string> CheckPlaces(QuestTable table)
        {
            var res = new Dictionary<Objective, string>();
            var byMap = new Dictionary<uint, List<Objective>>();
            foreach (var q in table.Quests.Values)
                foreach (var o in q.Objectives)
                    if ((o.Kind == ObjectiveKind.Visit || o.Kind == ObjectiveKind.Gather) && o.Map != 0)
                    {
                        List<Objective> l;
                        if (!byMap.TryGetValue(o.Map, out l)) byMap[o.Map] = l = new List<Objective>();
                        l.Add(o);
                    }
            bool judge = Role.GameMap.MapContents != null && Role.GameMap.MapContents.Count > 0;// tools without GameMap.dat: don't judge
            foreach (var kv in byMap)
            {
                if (!judge) break;
                if (!MapKnown(kv.Key)) { foreach (var o in kv.Value) res[o] = "map " + kv.Key + " does not exist"; continue; }
                string file = null; byte[] dmap = null; int w = 0, h = 0; string stamp = null;
                try
                {
                    file = Role.GameMap.MapContents[(int)Role.GameMap.ResolveBase(kv.Key)];
                    var fi = Role.MapFiles.Find(file);
                    if (fi != null) stamp = fi.FullName + "|" + fi.Length + "|" + fi.LastWriteTimeUtc.Ticks;
                }
                catch { }
                if (stamp == null) { foreach (var o in kv.Value) res[o] = "map " + kv.Key + ": the map file is missing"; continue; }
                foreach (var o in kv.Value)
                {
                    string key = stamp + "|" + o.X + "|" + o.Y + "|" + o.Range;
                    bool ok;
                    if (!PlaceCache.TryGetValue(key, out ok))
                    {
                        if (dmap == null)
                        {
                            try
                            {
                                dmap = Role.MapFiles.Read(file);
                                w = BitConverter.ToInt32(dmap, 268); h = BitConverter.ToInt32(dmap, 272);
                                if (w <= 0 || h <= 0 || dmap.Length < 276 + (long)h * (w * 6 + 4)) dmap = null;
                            }
                            catch { dmap = null; }
                            if (dmap == null) { foreach (var o2 in kv.Value) res[o2] = "map " + kv.Key + ": the map file can't be read"; break; }
                        }
                        ok = false;
                        int r = o.Range;
                        for (int dx = -r; dx <= r && !ok; dx++)
                            for (int dy = -r; dy <= r; dy++)
                            {
                                int x = o.X + dx, y = o.Y + dy;
                                if (x < 0 || y < 0 || x >= w || y >= h) continue;
                                if (BitConverter.ToInt16(dmap, 276 + y * (w * 6 + 4) + x * 6) == 0) { ok = true; break; }
                            }
                        PlaceCache[key] = ok;
                    }
                    if (!ok) res[o] = "no walkable ground within " + o.Range + " of (" + o.X + "," + o.Y + ") on map " + kv.Key;
                }
            }
            return res;
        }

        // ------------------------------------------------------------ build
        /// <summary>Builds a complete table from Questinfo.ini (+ quests.json). Never publishes anything itself.</summary>
        public static QuestTable Build()
        {
            var table = new QuestTable();
            var monsters = MonsterNames();
            var spawned = SpawnedMonsterNames();
            var hand = new HashSet<uint>(HandCodedQuests.HandCoded);
            var placed = PlacedNpcIds();
            var self = new HashSet<uint>(HandCodedQuests.SelfCounted);
            // [Festivals] running festivals add their monster types (they really spawn) and NPCs (placed at runtime)
            var overlays = FestivalOverlays();
            foreach (var ov in overlays)
            {
                if (!ov.Active) continue;
                foreach (var n in ov.MonsterNames) { string nn = Norm(n); if (nn.Length > 0) { monsters.Add(nn); spawned.Add(nn); } }
                if (placed != null) foreach (uint id in ov.NpcIds) placed.Add(id);
            }
            string path = Path.Combine(Program.ServerConfig.DbLocation, "Questinfo.ini");
            var sections = ReadSections(path);
            foreach (var s in sections)
            {
                uint id = U(Get(s, "MissionId"));
                if (id == 0 || table.Quests.ContainsKey(id)) continue;
                var q = new EngineQuest { MissionId = id, Name = Get(s, "Name").Replace('`', '\''), TypeId = (byte)U(Get(s, "TypeId")) };
                var b = Fields(Get(s, "BeginNpcId")); var f = Fields(Get(s, "FinishNpcId"));
                q.BeginNpc = b.Count > 0 ? U(b[0]) : 0;
                q.FinishNpc = f.Count > 0 ? U(f[0]) : 0;
                if (f.Count > 3) { q.FinishMap = U(f[1]); q.FinishX = (ushort)U(f[2]); q.FinishY = (ushort)U(f[3]); }
                q.Content = Clean(Get(s, "Content"));
                q.Desc = Clean(Get(s, "IntentionDesp"));
                q.OfficialPrizeText = Get(s, "Prize");
                q.Prize = ParsePrize(q.OfficialPrizeText);
                q.Prequests = Get(s, "Prequest").Split(new[] { '|', ',', '&' }, StringSplitOptions.RemoveEmptyEntries).Select(U).Where(x => x != 0).ToArray();
                string sst = Get(s, "ShowServerType");
                if (sst.Length > 0)
                    q.ServerTypes = sst.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => U(x)).ToList();
                // Intention lines: the plain IntentionN, else its first per-class variant
                var lines = new SortedDictionary<int, string>();
                foreach (var kv in s)
                {
                    var m = IntentionKey.Match(kv.Key);
                    if (!m.Success) continue;
                    int n = int.Parse(m.Groups[1].Value);
                    bool variant = m.Groups[2].Success;
                    if (!variant) lines[n] = kv.Value;
                    else if (!lines.ContainsKey(n) && !IsPlain(s, n)) lines[n] = kv.Value;
                }
                foreach (var kv in lines)
                {
                    if (kv.Value.IndexOf("teammate kills can be shared", StringComparison.OrdinalIgnoreCase) >= 0) q.Team = true;
                    if (string.IsNullOrWhiteSpace(kv.Value)) continue;
                    table.Lines++;
                    q.IntentionLines++;
                    q.IntentionText += " " + kv.Value;
                    var objs = ParseLine(id, kv.Key, kv.Value, monsters);
                    if (objs.Count > 0) table.LinesParsed++;
                    q.Objectives.AddRange(objs);
                }
                // QahrMobile: "[lua Name,...]" talk steps -> the finish / begin NPC carrying that name, else a scripted step
                foreach (var o in q.Objectives)
                {
                    if (o.Kind != ObjectiveKind.Talk || o.NpcId != (uint)LuaNpcByName) continue;
                    string nn = Norm(o.Name);
                    if (q.FinishNpc != 0 && f.Count > 4 && Norm(f[4]) == nn) o.NpcId = q.FinishNpc;
                    else if (q.BeginNpc != 0 && b.Count > 4 && Norm(b[4]) == nn) o.NpcId = q.BeginNpc;
                    else { o.Kind = ObjectiveKind.Unknown; o.NpcId = 0; }
                }
                q.HandCoded = hand.Contains(id);
                q.SelfCounted = self.Contains(id);
                // repeatable by name ([Daily] / [Weekly]), quests.json "repeat" overrides it
                if (q.Name.IndexOf("[Daily]", StringComparison.OrdinalIgnoreCase) >= 0 || q.Name.IndexOf("(Daily)", StringComparison.OrdinalIgnoreCase) >= 0) q.Repeat = 1;
                else if (q.Name.IndexOf("[Weekly]", StringComparison.OrdinalIgnoreCase) >= 0 || q.Name.IndexOf("(Weekly)", StringComparison.OrdinalIgnoreCase) >= 0) q.Repeat = 2;
                table.Quests.Add(id, q);
            }

            ApplyJson(table, monsters);
            ApplyFestivalOverlays(table, monsters, overlays);// [Festivals]
            QuestUnblock.Apply(table, monsters, spawned, placed);// QahrMobile: unblock pass (QuestUnblock.cs)

            // prerequisite graph after the quests.json overrides
            var prereqOf = new HashSet<uint>();
            var successors = new Dictionary<uint, List<EngineQuest>>();
            foreach (var q in table.Quests.Values)
            {
                if (q.PrequestOverride != null)
                    q.Prequests = q.PrequestOverride.Split(new[] { '|', ',', '&' }, StringSplitOptions.RemoveEmptyEntries).Select(U).Where(x => x != 0).ToArray();
                foreach (uint pre in q.Prequests)
                {
                    prereqOf.Add(pre);
                    List<EngineQuest> l;
                    if (!successors.TryGetValue(pre, out l)) successors[pre] = l = new List<EngineQuest>();
                    l.Add(q);
                }
            }
            foreach (var q in table.Quests.Values)
                if (!q.FinalSet) q.ChainEnd = !prereqOf.Contains(q.MissionId);

            // ER-15: a chain that repeats the same parsed item list on every step pays it only at the last step
            var originalItems = table.Quests.Values.ToDictionary(q => q.MissionId, q => new HashSet<uint>(q.Prize.Items.Select(i => i.Id)));
            foreach (var q in table.Quests.Values)
            {
                if (q.Prize.FromJson || q.Prize.Items.Count == 0) continue;
                List<EngineQuest> next;
                if (!successors.TryGetValue(q.MissionId, out next)) continue;
                var mine = originalItems[q.MissionId];
                if (next.Any(n => !n.Prize.FromJson && originalItems[n.MissionId].SetEquals(mine)))
                {
                    q.Prize.Items.Clear();
                    table.ChainPrizesTrimmed++;
                }
            }

            // ER-9: CPs (and big silver) parsed out of free Prize text are never paid unless quests.json allows it
            foreach (var q in table.Quests.Values)
            {
                if (q.Prize.FromJson || q.AllowParsedCps) continue;
                if (q.Prize.Cps > 0 || q.Prize.Silver > ParsedSilverCap) table.ParsedCpsBlocked++;
                q.Prize.Cps = 0;
                if (q.Prize.Silver > ParsedSilverCap) q.Prize.Silver = 0;
            }

            // npc aliases: the server NPC stands in for the quest NPC (begin, finish, talk steps)
            foreach (var q in table.Quests.Values)
            {
                uint a;
                if (table.NpcAliases.TryGetValue(q.BeginNpc, out a)) q.BeginNpc = a;
                if (table.NpcAliases.TryGetValue(q.FinishNpc, out a)) q.FinishNpc = a;
                foreach (var o in q.Objectives)
                    if (o.Kind == ObjectiveKind.Talk && table.NpcAliases.TryGetValue(o.NpcId, out a)) o.NpcId = a;
            }

            // ER-16: a talk step on another NPC without a [chk_quest] counter gets a free slot (it was never completable)
            foreach (var q in table.Quests.Values)
            {
                var used = new HashSet<int>(q.Objectives.Where(o => o.Slot > 0).Select(o => (int)o.Slot));
                foreach (var o in q.Objectives)
                {
                    if (o.Kind != ObjectiveKind.Talk || o.Slot != 0 || o.NpcId == 0 || o.NpcId == q.FinishNpc) continue;
                    int s = o.Line > 0 && !used.Contains(o.Line) ? o.Line : 0;
                    if (s == 0) { s = 1; while (used.Contains(s) && s < 32) s++; }
                    if (used.Contains(s)) continue;
                    o.Slot = (byte)s; used.Add(s);
                    table.TalkSlotsFixed++;
                }
            }

            // monsters the engine spawns itself count as spawned (when the type exists)
            foreach (var q in table.Quests.Values)
                foreach (var act in q.AllActions)
                    foreach (var sp in act.Spawn)
                        if (monsters.Contains(sp.Norm)) spawned.Add(sp.Norm);
            foreach (var inst in table.Instances.Values)
                foreach (var sp in inst.Monsters)
                    if (monsters.Contains(sp.Norm)) spawned.Add(sp.Norm);

            // [InstanceNpc] NPCs that stand inside instance copies count as placed (talk / begin / finish NPCs)
            if (placed != null)
                foreach (var inst in table.Instances.Values)
                    foreach (var n in inst.Npcs) placed.Add(n.Uid);

            // [QuestEngineFix 2026-10-14] Visit / Gather places: the map exists and has walkable ground within range
            var placeProblems = CheckPlaces(table);

            // ownership / workability
            foreach (var q in table.Quests.Values)
            {
                foreach (var o in q.Objectives)
                {
                    table.Objectives++;
                    int c; table.KindCounts.TryGetValue(o.Kind, out c); table.KindCounts[o.Kind] = c + 1;
                    if (o.Kind != ObjectiveKind.Unknown) table.ObjectivesTyped++;
                    if (o.Kind == ObjectiveKind.Collect)
                    {
                        List<DropRule> global;
                        if (table.GlobalDrops.TryGetValue(o.ItemId, out global)) o.Drops.AddRange(global);
                    }
                }
                q.Problems.Clear();
                if (q.Objectives.Count == 0)
                {
                    // [QuestEngineFix 2026-10-14] intention lines but none understood: it would finish at once (3636, 6490)
                    // A pure travel step ("Go ahead to enter Toad Mountain" -> finish at another NPC) is understood: going
                    // to the finish NPC IS the step. Same begin/finish NPC, or a kill/collect line, is not.
                    bool travel = q.BeginNpc != 0 && q.FinishNpc != 0 && q.BeginNpc != q.FinishNpc && !KillVerb.IsMatch(q.IntentionText) && !ItemVerb.IsMatch(q.IntentionText);
                    if (q.IntentionLines > 0 && !q.ObjectivesExplicit && !travel) q.Problems.Add("no objective the engine understands (" + q.IntentionLines + " intention line(s))");
                    else if (q.FinishNpc == 0) q.Problems.Add("no objectives and no finish NPC");
                }
                foreach (var o in q.Objectives)
                {
                    switch (o.Kind)
                    {
                        case ObjectiveKind.Unknown: q.Problems.Add("scripted step: " + Short(o.Text)); break;
                        case ObjectiveKind.Kill:
                            if (o.AnyMonster) break;
                            {
                                var all = new List<string> { o.Monster }; all.AddRange(o.Monsters);
                                if (!all.Any(n => n.Length > 0 && Known(monsters, n))) q.Problems.Add("monster '" + o.Monster + "' is not in Monsters\\");
                                else if (o.SpawnedBy.Length == 0 && !all.Any(n => n.Length > 0 && Known(spawned, n))) q.Problems.Add("monster '" + o.Monster + "' has no spawn");
                            }
                            if (o.Slot == 0) q.Problems.Add("kill without a counter slot");
                            break;
                        case ObjectiveKind.Collect:
                            if (Pool.ItemsBase != null && Pool.ItemsBase.Count > 0 && !Pool.ItemsBase.ContainsKey(o.ItemId)) q.Problems.Add("item " + o.ItemId + " is not in itemtype.txt");
                            else if (o.Drops.Count == 0 && !o.External) q.Problems.Add("item " + o.ItemId + " has no drop source");
                            // [QuestEngineFix 2026-10-14] the dropping monster must really spawn (200 Tempest Wings <- HeavenRoc: none)
                            else if (!o.External && !o.Drops.Any(r => DropPossible(r, monsters, spawned)))
                                q.Problems.Add("item " + o.ItemId + ": no drop source spawns (" + string.Join("; ", o.Drops.Select(r => r.ToString())) + ")");
                            break;
                        case ObjectiveKind.Gather:
                        case ObjectiveKind.Visit:
                            if (o.Map == 0) q.Problems.Add("no location");
                            else { string pp; if (placeProblems.TryGetValue(o, out pp)) q.Problems.Add(pp); }
                            break;
                        case ObjectiveKind.Talk:
                            if (o.NpcId == 0) q.Problems.Add("instance NPC");
                            else if (placed != null && o.NpcId != q.FinishNpc && !placed.Contains(o.NpcId)) q.Problems.Add("talk NPC " + o.NpcId + " is not placed (Npcs.txt)");
                            break;
                        case ObjectiveKind.State:
                            if (!QuestEngineCore.StateKnown(o)) q.Problems.Add("unknown state '" + o.Stat + "'");
                            if (o.Slot == 0) q.Problems.Add("state without a slot");
                            break;
                        case ObjectiveKind.Event:
                            if (!(KnownEvents.Contains(o.Event) || o.Event.StartsWith("useItem:", StringComparison.OrdinalIgnoreCase))) q.Problems.Add("event '" + o.Event + "' has no server hook yet");
                            if (o.Slot == 0) q.Problems.Add("event without a slot");
                            break;
                        case ObjectiveKind.Counter:
                            if (!q.HandCoded) q.Problems.Add("counter slot that no hand-written code raises");
                            break;
                        case ObjectiveKind.KillPoints:
                            if (o.Points.Length > 0 && !o.Points.Equals("monsterLevel", StringComparison.OrdinalIgnoreCase)) q.Problems.Add("kill points '" + o.Points + "' not supported");
                            if (o.Slot == 0) q.Problems.Add("kill points without a slot");
                            break;
                    }
                }
                foreach (var act in q.AllActions)
                {
                    if (act.TeleportMap != 0 && !MapKnown(act.TeleportMap)) q.Problems.Add("teleport target map " + act.TeleportMap + " does not exist");
                    foreach (var g in act.Give)
                        if (Pool.ItemsBase != null && Pool.ItemsBase.Count > 0 && !Pool.ItemsBase.ContainsKey(g.Id)) q.Problems.Add("give item " + g.Id + " is not in itemtype.txt");
                    foreach (var sp in act.Spawn)
                    {
                        if (!monsters.Contains(sp.Norm)) q.Problems.Add("spawn monster '" + sp.Name + "' is not in Monsters\\");
                        else if (sp.MonsterId != 0 && Pool.MonsterFamilies != null && Pool.MonsterFamilies.Count > 0)
                        {
                            Game.MsgMonster.MonsterFamily fam;// [NinjaChain 2026-10-14] the exact type must exist and carry that name
                            if (!Pool.MonsterFamilies.TryGetValue(sp.MonsterId, out fam) || fam == null || Norm(fam.Name) != sp.Norm)
                                q.Problems.Add("spawn monster '" + sp.Name + "' #" + sp.MonsterId + " is not in Monsters\\");
                        }
                        if (sp.Map != 0 && !MapKnown(sp.Map)) q.Problems.Add("spawn map " + sp.Map + " does not exist");
                    }
                    if (act.Instance != null)
                    {
                        InstanceDef inst;
                        if (!table.Instances.TryGetValue(act.Instance, out inst)) q.Problems.Add("instance '" + act.Instance + "' is not defined");
                        else if (!MapKnown(inst.Map)) q.Problems.Add("instance map " + inst.Map + " does not exist");
                        else foreach (var sp in inst.Monsters) if (!monsters.Contains(sp.Norm)) q.Problems.Add("instance monster '" + sp.Name + "' is not in Monsters\\");
                    }
                }
                if (q.Extra != null)
                {
                    foreach (var g in q.Extra.NinjaGates) { Role.Instance.Ninja.Flags fl; if (!Enum.TryParse(g, out fl)) q.Problems.Add("unknown ninja gate '" + g + "'"); }
                    foreach (var a in q.Extra.Archives) { Role.Instance.Archives.TypeID ty; if (!Enum.TryParse(a, out ty)) q.Problems.Add("unknown archive '" + a + "'"); }
                }
                if (q.IsBounty && q.Objectives.Count == 0) q.Problems.Add("bounty without objectives");
                if (!q.IsBounty && placed != null && q.FinishNpc != 0 && !placed.Contains(q.FinishNpc)) q.Problems.Add("finish NPC " + q.FinishNpc + " is not placed (Npcs.txt)");
                // festival / cross-server / kingdom / realm / global-event quests run on schedules and systems the
                // engine doesn't know: off unless quests.json enables them ("engine": true)
                if (q.TypeId >= 8 && !q.FromJsonOwned) q.Problems.Add("event type " + q.TypeId + " (festival/cross-server): enable in quests.json");
                // ER-11: the client hides a quest whose ShowServerType lacks our server type
                if (q.ServerTypes != null && q.ServerTypes.Count > 0 && !q.ServerTypes.Contains(table.ServerType) && !q.FromJsonOwned)
                {
                    q.Problems.Add("hidden on server type " + table.ServerType + " (ShowServerType " + string.Join(",", q.ServerTypes) + ")");
                    table.HiddenByServerType++;
                }
                q.Workable = q.Problems.Count == 0;
                if (!q.Disabled && !(q.HandCoded && !q.FromJsonOwned))
                    q.Owned = true;
                if (q.Disabled) q.Owned = false;
                if (q.Owned) table.Owned++;
                if (q.Owned && q.Workable) table.Workable++;
                if (q.HandCoded) table.HandCoded++;
                if (q.HandCoded && q.EngineCounts && !q.Owned && q.Objectives.Any(o => o.Kind == ObjectiveKind.Kill || o.Kind == ObjectiveKind.Gather || o.Kind == ObjectiveKind.Collect || o.Kind == ObjectiveKind.Visit)) table.KillCounted++;
                if (q.Owned)
                {
                    if (q.BeginNpc != 0) Index(table.ByBeginNpc, q.BeginNpc, q);
                    if (q.FinishNpc != 0) Index(table.ByFinishNpc, q.FinishNpc, q);
                    if (q.Pool != null)
                    {
                        List<EngineQuest> pl;
                        if (!table.Pools.TryGetValue(q.Pool, out pl)) table.Pools[q.Pool] = pl = new List<EngineQuest>();
                        pl.Add(q);
                    }
                }
            }
            foreach (var pl in table.Pools.Values) pl.Sort((a, b) => a.MissionId.CompareTo(b.MissionId));
            QuestUnblock.Report(table);// QahrMobile: unblock_report.csv
            return table;
        }

        static bool IsPlain(Dictionary<string, string> s, int n) { return s.ContainsKey("Intention" + n); }
        static void Index(Dictionary<uint, List<EngineQuest>> d, uint key, EngineQuest q)
        {
            List<EngineQuest> l;
            if (!d.TryGetValue(key, out l)) d[key] = l = new List<EngineQuest>();
            l.Add(q);
        }
        public static string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Replace("\\n", " ").Replace('`', '\'').Replace('~', ' ');
            s = Token.Replace(s, m => { var f = Fields(m.Groups[2].Value); string k = m.Groups[1].Value.ToLowerInvariant(); return (k == "npc" || k == "mob" || k == "item_t" || k == "map") && f.Count > 0 ? f[0] : ""; });
            return Regex.Replace(s, @"\s+", " ").Trim();
        }
        static string Short(string s) { s = Clean(s); return s.Length > 60 ? s.Substring(0, 57) + "..." : s; }

        // ------------------------------------------------------------ quests.json
        static Dictionary<string, object> Obj(object o) { return o as Dictionary<string, object>; }
        static List<object> Arr(object o) { return o as List<object>; }
        static object Val(Dictionary<string, object> d, string k) { object v; return d != null && d.TryGetValue(k, out v) ? v : null; }
        static string Str(Dictionary<string, object> d, string k) { var v = Val(d, k); return v == null ? null : v.ToString(); }
        static uint JU(Dictionary<string, object> d, string k, uint def = 0) { var v = Val(d, k); if (v == null) return def; ulong r = Num(v.ToString().Split('.')[0]); return r == 0 && v.ToString().Trim() != "0" ? def : (uint)Math.Min(r, uint.MaxValue); }
        static ulong JUL(Dictionary<string, object> d, string k) { var v = Val(d, k); return v == null ? 0 : Num(v.ToString().Split('.')[0]); }
        static bool? JB(Dictionary<string, object> d, string k) { var v = Val(d, k); if (v is bool) return (bool)v; return null; }
        static int Chance(Dictionary<string, object> d, int def)
        {
            var v = Val(d, "chance");
            double p;
            if (v == null || !double.TryParse(v.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out p)) return def;
            return (int)Math.Max(1, Math.Min(10000, Math.Round(p * 100)));
        }
        static DropRule ParseDrop(Dictionary<string, object> d)
        {
            if (d == null) return null;
            var r = new DropRule { Monster = Norm(Str(d, "monster") ?? ""), Map = JU(d, "map"), X = (ushort)JU(d, "x"), Y = (ushort)JU(d, "y"), Radius = (ushort)JU(d, "radius"), Chance = Chance(d, 2500) };
            return r.Monster.Length > 0 || r.Map != 0 ? r : null;
        }
        static List<PrizeItem> Items(object o)
        {
            var list = new List<PrizeItem>();
            foreach (var io in Arr(o) ?? new List<object>())
            {
                var idd = Obj(io);
                if (idd != null)
                {
                    uint iid = JU(idd, "id");
                    if (iid != 0) list.Add(new PrizeItem { Id = iid, Count = (ushort)Math.Max(1, Math.Min(1000, JU(idd, "count", 1))), Bound = JB(idd, "bound") ?? false });
                }
            }
            return list;
        }
        /// <summary>{"map":m,"x":x,"y":y} or [m,x,y]</summary>
        static bool Point(object o, out uint map, out ushort x, out ushort y)
        {
            map = 0; x = y = 0;
            var d = Obj(o);
            if (d != null) { map = JU(d, "map"); x = (ushort)JU(d, "x"); y = (ushort)JU(d, "y"); return map != 0; }
            var a = Arr(o);
            if (a != null && a.Count >= 3) { map = U(a[0].ToString().Split('.')[0]); x = (ushort)U(a[1].ToString().Split('.')[0]); y = (ushort)U(a[2].ToString().Split('.')[0]); return map != 0; }
            return false;
        }
        static SpawnSpec Spawn(Dictionary<string, object> d)
        {
            if (d == null) return null;
            string name = Str(d, "monster");
            if (string.IsNullOrEmpty(name)) return null;
            return new SpawnSpec { Name = name, Norm = Norm(name), Map = JU(d, "map"), X = (ushort)JU(d, "x"), Y = (ushort)JU(d, "y"), Count = (byte)Math.Max(1, Math.Min(10, JU(d, "count", 1))), Range = (ushort)Math.Min(20, JU(d, "range", 2)), Minutes = (int)Math.Max(1, Math.Min(60, JU(d, "minutes", 10))), MonsterId = JU(d, "monsterId") };
        }
        static QuestActions Actions(object o)
        {
            var d = Obj(o);
            if (d == null)
            {
                // progression shape: [ {"teleport":[m,x,y]}, {"summon":{...}}, {"give":[item,count]} ]
                var arr = Arr(o);
                if (arr == null) return null;
                var merged = new QuestActions();
                foreach (var e in arr) { var a = Actions(e); if (a != null) Merge(merged, a); }
                return merged.IsEmpty ? null : merged;
            }
            var r = new QuestActions();
            uint m; ushort x, y;
            if (Point(Val(d, "teleport"), out m, out x, out y)) { r.TeleportMap = m; r.TeleportX = x; r.TeleportY = y; }
            r.Give.AddRange(Items(Val(d, "giveItems")));
            var give = Arr(Val(d, "give"));
            if (give != null && give.Count >= 1 && !(give[0] is Dictionary<string, object>))
                r.Give.Add(new PrizeItem { Id = U(give[0].ToString().Split('.')[0]), Count = (ushort)Math.Max(1, Math.Min(1000, give.Count > 1 ? U(give[1].ToString().Split('.')[0]) : 1)) });
            else r.Give.AddRange(Items(give));
            r.Give.RemoveAll(g => g.Id == 0);
            foreach (string key in new[] { "spawn", "summon" })
            {
                var sv = Val(d, key);
                if (Obj(sv) != null) { var sp = Spawn(Obj(sv)); if (sp != null) r.Spawn.Add(sp); }
                else foreach (var e in Arr(sv) ?? new List<object>()) { var sp = Spawn(Obj(e)); if (sp != null) r.Spawn.Add(sp); }
            }
            r.Instance = Str(d, "instance");
            r.LeaveInstance = JB(d, "leaveInstance") ?? false;
            return r.IsEmpty ? null : r;
        }
        static void Merge(QuestActions into, QuestActions a)
        {
            if (a.TeleportMap != 0) { into.TeleportMap = a.TeleportMap; into.TeleportX = a.TeleportX; into.TeleportY = a.TeleportY; }
            into.Give.AddRange(a.Give); into.Spawn.AddRange(a.Spawn);
            if (a.Instance != null) into.Instance = a.Instance;
            into.LeaveInstance |= a.LeaveInstance;
        }
        static List<string> Strings(object o)
        {
            var l = new List<string>();
            if (o is string) { l.Add((string)o); return l; }
            foreach (var e in Arr(o) ?? new List<object>()) if (e != null && e.ToString().Length > 0) l.Add(e.ToString());
            return l;
        }

        static void ApplyJson(QuestTable table, HashSet<string> monsters)
        {
            string file = Path.Combine(Path.Combine(Program.ServerConfig.DbLocation, "QuestEngine"), "quests.json");
            if (!File.Exists(file)) return;
            Dictionary<string, object> root;
            try { root = Obj(AdminApi.AdminJson.Parse(File.ReadAllText(file, Encoding.UTF8))); }
            catch (Exception e) { table.JsonError = e.Message; MyConsole.WriteLine("[QuestEngine] quests.json does not parse: " + e.Message + " (ignored)", ConsoleColor.Yellow); return; }
            if (root == null) return;
            table.ServerType = JU(root, "serverType", 0);
            ApplyRoot(table, monsters, root, null, false);
        }

        // ------------------------------------------------------------ [Festivals] overlays
        /// <summary>[Festivals] one festival's quests.json as the QuestEngine sees it (Events\Framework publishes these).</summary>
        public sealed class QuestOverlay
        {
            public string Key;                                   // festival key
            public bool Active;                                  // the festival is running
            public Dictionary<string, object> Root;              // its parsed quests.json (null = none)
            public List<string> MonsterNames = new List<string>();// its monster types (registered while active)
            public List<uint> NpcIds = new List<uint>();          // its NPCs (placed while active)
        }
        /// <summary>[Festivals] set by Events\Framework\FestivalManager; null = no festivals.</summary>
        public static Func<List<QuestOverlay>> OverlayProvider;
        static List<QuestOverlay> FestivalOverlays()
        {
            try { var p = OverlayProvider; var l = p != null ? p() : null; return l ?? new List<QuestOverlay>(); }
            catch (Exception e) { MyConsole.WriteLine("[QuestEngine] festival overlays: " + e.Message, ConsoleColor.Yellow); return new List<QuestOverlay>(); }
        }
        /// <summary>Running festivals: their quests.json on top of the main file (drops, aliases, teleports, instances,
        /// quests; quests engine-owned by default). Stopped festivals: only their quests are read and then switched off
        /// (Disabled + FestivalOff) so nobody can accept, count, finish or be paid for them, while repeat resets still work.</summary>
        static void ApplyFestivalOverlays(QuestTable table, HashSet<string> monsters, List<QuestOverlay> overlays)
        {
            foreach (var ov in overlays.Where(o => !o.Active).Concat(overlays.Where(o => o.Active)))
            {
                if (ov == null || ov.Root == null) continue;
                try { ApplyRoot(table, monsters, ov.Root, ov.Key ?? "?", !ov.Active); }
                catch (Exception e) { MyConsole.WriteLine("[QuestEngine] festival " + ov.Key + " quests.json: " + e.Message + " (skipped)", ConsoleColor.Yellow); }
            }
        }

        static void ApplyRoot(QuestTable table, HashSet<string> monsters, Dictionary<string, object> root, string festival, bool festivalOff)
        {
            if (!festivalOff)
            {
                ApplyRootData(table, root);
            }
            ApplyRootQuests(table, monsters, root, festival, festivalOff);
        }

        static void ApplyRootData(QuestTable table, Dictionary<string, object> root)
        {
            foreach (var o in Arr(Val(root, "drops")) ?? new List<object>())
            {
                var d = Obj(o); var r = ParseDrop(d); uint item = JU(d, "item");
                if (r == null || item == 0) continue;
                List<DropRule> l;
                if (!table.GlobalDrops.TryGetValue(item, out l)) table.GlobalDrops[item] = l = new List<DropRule>();
                l.Add(r);
            }
            var aliases = Obj(Val(root, "npcAliases"));
            if (aliases != null)
                foreach (var kv in aliases)
                {
                    uint from = U(kv.Key), to = U((kv.Value ?? "").ToString().Split('.')[0]);
                    if (from != 0 && to != 0 && from != to) table.NpcAliases[from] = to;
                }
            var tps = Obj(Val(root, "npcTeleports"));
            if (tps != null)
                foreach (var kv in tps)
                {
                    uint npc = U(kv.Key);
                    if (npc == 0) continue;
                    var list = new List<NpcTeleport>();
                    foreach (var e in Arr(kv.Value) ?? new List<object>())
                    {
                        var d = Obj(e); if (d == null) continue;
                        var tp = new NpcTeleport { Text = Str(d, "text") ?? "Teleport", Map = JU(d, "map"), X = (ushort)JU(d, "x"), Y = (ushort)JU(d, "y"), MinLevel = (ushort)JU(d, "minLevel"), RequireQuest = JU(d, "requireQuest"), Instance = Str(d, "instance") };
                        if (tp.Map != 0 || tp.Instance != null) list.Add(tp);
                    }
                    if (list.Count > 0) table.NpcTeleports[npc] = list;
                }
            var insts = Obj(Val(root, "instances"));
            if (insts != null)
                foreach (var kv in insts)
                {
                    var d = Obj(kv.Value);
                    if (d == null || kv.Key.StartsWith("_")) continue;
                    var inst = new InstanceDef { Name = kv.Key, Map = JU(d, "map"), X = (ushort)JU(d, "x"), Y = (ushort)JU(d, "y"), Minutes = (int)Math.Max(5, Math.Min(120, JU(d, "minutes", 30))) };
                    uint em; ushort ex, ey;
                    if (Point(Val(d, "exit"), out em, out ex, out ey)) { inst.ExitMap = em; inst.ExitX = ex; inst.ExitY = ey; }
                    foreach (var e in Arr(Val(d, "monsters")) ?? new List<object>())
                    {
                        var sp = Spawn(Obj(e));
                        if (sp != null) { sp.Map = inst.Map; sp.Minutes = inst.Minutes; inst.Monsters.Add(sp); }
                    }
                    // [InstanceNpc] "npcs": [ {"id":8900001,"mesh":20280,"x":91,"y":122,"type":2,"name":"Villager"} ]
                    foreach (var e in Arr(Val(d, "npcs")) ?? new List<object>())
                    {
                        var nd = Obj(e);
                        if (nd == null) continue;
                        var n = new InstanceNpc { Uid = JU(nd, "id"), Mesh = JU(nd, "mesh"), Type = (byte)Math.Max(1, Math.Min(255, JU(nd, "type", 2))), X = (ushort)JU(nd, "x"), Y = (ushort)JU(nd, "y"), Name = Str(nd, "name") ?? "" };
                        if (n.Uid == 0 || n.Mesh == 0 || n.X == 0 || n.Y == 0 || inst.Npcs.Any(x => x.Uid == n.Uid)) { MyConsole.WriteLine("[QuestEngine] instance " + kv.Key + ": npc entry skipped (needs a unique id, mesh, x, y)", ConsoleColor.Yellow); continue; }
                        inst.Npcs.Add(n);
                    }
                    if (inst.Map != 0) table.Instances[inst.Name] = inst;
                }
        }

        static void ApplyRootQuests(QuestTable table, HashSet<string> monsters, Dictionary<string, object> root, string festival, bool festivalOff)
        {
            var quests = Obj(Val(root, "quests"));
            if (quests == null) return;
            foreach (var kv in quests)
            {
                uint id = U(kv.Key);
                var d = Obj(kv.Value);
                EngineQuest q;
                if (id == 0 || d == null || !table.Quests.TryGetValue(id, out q)) continue;
                table.JsonQuests++;
                q.FromJson = true;
                bool? engine;
                if (d.ContainsKey("engineV2"))
                {
                    var v2 = Val(d, "engineV2");
                    engine = v2 is bool ? (bool?)(bool)v2 : null;// "auto" / null = the normal rules
                }
                else engine = JB(d, "engine");
                if (festival != null)
                {
                    // [Festivals] festival quests are engine-owned by default; a stopped festival switches them off
                    q.Festival = festival;
                    if (engine == null) engine = true;
                    if (festivalOff) { q.FestivalOff = true; q.Disabled = true; }
                    else { q.FestivalOff = false; q.Disabled = false; }
                }
                if (engine == false) q.Disabled = true;
                if (engine == true) q.FromJsonOwned = true;
                if (q.FestivalOff) q.Disabled = true;
                var objs = Arr(Val(d, "objectives"));
                if (objs != null)
                {
                    q.ObjectivesExplicit = true;// [QuestEngineFix] "objectives": [] = finish at the NPC on purpose
                    q.Objectives.Clear();
                    foreach (var oo in objs)
                    {
                        var od = Obj(oo); if (od == null) continue;
                        var ob = new Objective { Text = "quests.json", Count = Math.Max(1, JU(od, "count", 1)), Slot = (byte)Math.Min(32, JU(od, "slot")), Map = JU(od, "map"), X = (ushort)JU(od, "x"), Y = (ushort)JU(od, "y") };
                        switch ((Str(od, "type") ?? "").ToLowerInvariant())
                        {
                            case "kill":
                                ob.Kind = ObjectiveKind.Kill; ob.Name = Str(od, "monster") ?? "";
                                if (ob.Name == "*" || ob.Name.Length == 0) { ob.AnyMonster = true; ob.Name = "monster"; }
                                else ob.Monster = TokenMonster(ob.Name, monsters);
                                foreach (var mn in Strings(Val(od, "monsters"))) { string nn = TokenMonster(mn, monsters); if (nn.Length > 0 && nn != ob.Monster && !ob.Monsters.Contains(nn)) ob.Monsters.Add(nn); }
                                var maps = Arr(Val(od, "maps"));
                                if (maps != null) ob.Maps = new HashSet<uint>(maps.Select(x => U((x ?? "").ToString().Split('.')[0])).Where(x => x != 0));
                                if (Val(od, "minLevel") != null) ob.MinLevelGap = (int)Math.Min(200, JU(od, "minLevel"));
                                ob.TimeLimit = Math.Min(3600, JU(od, "timeLimit"));
                                ob.SpawnedBy = Str(od, "spawnedBy") ?? "";
                                if (ob.Slot == 0) ob.Slot = 1;
                                break;
                            case "talk": ob.Kind = ObjectiveKind.Talk; ob.NpcId = JU(od, "npc"); ob.Name = Str(od, "name") ?? ("npc " + ob.NpcId); ob.After = (byte)Math.Min(32, JU(od, "after")); break;
                            case "collect":
                                ob.Kind = ObjectiveKind.Collect; ob.ItemId = JU(od, "item"); ob.Slot = 0;
                                ob.TakeItem = JB(od, "take") ?? true;
                                string src = (Str(od, "source") ?? "").ToLowerInvariant();
                                ob.External = src == "external" || src == "exchange";
                                foreach (var dd in Arr(Val(od, "drops")) ?? new List<object>()) { var r = ParseDrop(Obj(dd)); if (r != null) ob.Drops.Add(r); }
                                break;
                            case "gather": ob.Kind = ObjectiveKind.Gather; ob.Range = (ushort)JU(od, "radius", 40); ob.Chance = Chance(od, 5000); if (ob.Slot == 0) ob.Slot = 1; ob.Name = Str(od, "name") ?? ""; break;
                            case "visit": ob.Kind = ObjectiveKind.Visit; ob.Range = (ushort)JU(od, "range", 8); ob.Name = Str(od, "name") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "classstage":
                                ob.Kind = ObjectiveKind.State; ob.Stat = "classStage"; ob.Min = JU(od, "min", 1); ob.Name = Str(od, "name") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "state":
                                ob.Kind = ObjectiveKind.State; ob.Stat = Str(od, "stat") ?? ""; ob.Min = JU(od, "min", 1); ob.Weapon = Str(od, "weapon") ?? ""; ob.Name = Str(od, "text") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "event":
                            case "signal":
                                ob.Kind = ObjectiveKind.Event; ob.Event = Str(od, "event") ?? Str(od, "key") ?? ""; ob.Name = Str(od, "text") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "useitem":
                                ob.Kind = ObjectiveKind.Event; ob.Event = "useItem:" + JU(od, "item"); ob.Name = Str(od, "text") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "bountydone":
                                ob.Kind = ObjectiveKind.Event; ob.Event = "bountyDone"; ob.Name = "Bounty Hall"; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "counter":
                                ob.Kind = ObjectiveKind.Counter; ob.Name = Str(od, "text") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            case "killpoints":
                                ob.Kind = ObjectiveKind.KillPoints; ob.Points = Str(od, "points") ?? ""; ob.Name = Str(od, "text") ?? ""; if (ob.Slot == 0) ob.Slot = 1; break;
                            default: ob.Kind = ObjectiveKind.Unknown; ob.Text = Str(od, "text") ?? ((Str(od, "type") ?? "?") + " (quests.json step)"); break;
                        }
                        q.Objectives.Add(ob);
                    }
                }
                foreach (var dd in Arr(Val(d, "drops")) ?? new List<object>())
                {
                    var dobj = Obj(dd); var r = ParseDrop(dobj); uint item = JU(dobj, "item");
                    if (r == null) continue;
                    foreach (var ob in q.Objectives)
                        if (ob.Kind == ObjectiveKind.Collect && (item == 0 || item == ob.ItemId)) ob.Drops.Add(r);
                }
                var pz = Obj(Val(d, "prize"));
                if (pz != null)
                {
                    var p = new Prize { Text = "quests.json", FromJson = true, Exp = JUL(pz, "exp"), Silver = JUL(pz, "silver"), Cps = JU(pz, "cps"), ExpBallUnits = (uint)Math.Min(100000UL, JUL(pz, "expBalls") * 600), RandomItem = JB(pz, "random") ?? false };
                    p.Items.AddRange(Items(Val(pz, "items")));
                    p.FinalItems.AddRange(Items(Val(pz, "finalItems")));
                    q.Prize = p;
                }
                q.AllowParsedCps = JB(d, "allowParsedCps") ?? false;
                var fin = JB(d, "final");
                if (fin != null) { q.ChainEnd = fin.Value; q.FinalSet = true; }
                q.BeginText = Str(d, "beginText") ?? q.BeginText;
                q.ProgressText = Str(d, "progressText") ?? q.ProgressText;
                q.FinishText = Str(d, "finishText") ?? q.FinishText;
                q.NextQuest = JU(d, "next", q.NextQuest);
                q.AutoAccept = JB(d, "autoAccept") ?? q.AutoAccept;
                q.Team = JB(d, "team") ?? q.Team;
                q.TakeItems = JB(d, "takeItems") ?? q.TakeItems;
                if (!q.TakeItems) foreach (var ob in q.Objectives) ob.TakeItem = false;
                // ---- features
                if (Val(d, "beginNpc") != null) { q.BeginNpc = JU(d, "beginNpc"); q.BeginOverride = true; }
                if (Val(d, "finishNpc") != null) q.FinishNpc = JU(d, "finishNpc");
                if (Val(d, "finishMap") != null) { q.FinishMap = JU(d, "finishMap"); q.FinishX = (ushort)JU(d, "finishX"); q.FinishY = (ushort)JU(d, "finishY"); }
                if (Val(d, "prequest") != null) q.PrequestOverride = Str(d, "prequest") ?? "";
                q.OnAccept = Actions(Val(d, "onAccept"));
                q.OnComplete = Actions(Val(d, "onComplete"));
                q.OnFinish = Actions(Val(d, "onFinish"));
                var acts = Obj(Val(d, "actions"));// progression shape {"onAccept":[...], "onFinish":[...]}
                if (acts != null)
                {
                    var a1 = Actions(Val(acts, "onAccept")); if (a1 != null) { if (q.OnAccept == null) q.OnAccept = a1; else Merge(q.OnAccept, a1); }
                    var a2 = Actions(Val(acts, "onFinish")); if (a2 != null) { if (q.OnFinish == null) q.OnFinish = a2; else Merge(q.OnFinish, a2); }
                    var a3 = Actions(Val(acts, "onComplete")); if (a3 != null) { if (q.OnComplete == null) q.OnComplete = a3; else Merge(q.OnComplete, a3); }
                }
                var px = Obj(Val(d, "prizeExtra"));
                if (px != null)
                {
                    var ex = new PrizeExtra { NinjaUnlock = JB(px, "ninjaUnlock") ?? false };
                    ex.NinjaGates.AddRange(Strings(Val(px, "ninjaGates")));
                    ex.Archives.AddRange(Strings(Val(px, "archive")));
                    ex.Archives.AddRange(Strings(Val(px, "archives")));
                    if (!ex.IsEmpty) q.Extra = ex;
                }
                string rep = (Str(d, "repeat") ?? "").ToLowerInvariant();
                if (rep == "daily" || JB(d, "daily") == true) { q.Repeat = 1; q.RepeatExplicit = true; }
                else if (rep == "weekly" || JB(d, "weekly") == true) { q.Repeat = 2; q.RepeatExplicit = true; }
                else if (rep == "none" || rep == "once") { q.Repeat = 0; q.RepeatExplicit = true; }
                q.OfferOnly = JB(d, "offerOnly") ?? false;
                q.ForceCount = (JB(d, "engineCounts") ?? false) || (JB(d, "forceCount") ?? false);
                q.Pool = Str(d, "pool");
                q.RequireClan = JB(d, "requireClan") ?? false;
            }
        }
    }
}
