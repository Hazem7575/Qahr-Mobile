// ============================================================================
//  [BandsTrouble 2026-10-15] Antonio`s stories (bandsman 4 of the Hot Music Festival window).
//
//  The official chapters 7978-7984 exist only as story texts in the client (Server_Key tPlotEditorPlotEvent_DelText[79xx],
//  AntonioTexts.cs); this client`s Questinfo has no 7978-7984 entries, so the client can`t list them as quests. They are
//  played here as a server-side story: every chapter is a list of steps (talk pages with the official texts and options,
//  searches with the official messages, fights with the official Discord Group monsters), progress per character in
//  Infinity[DB]\BandsTrouble\players.json (keys ant_ch / ant_st / ant_pg), saved before anything is given.
//  Maps: 7978-7979 Antonio`s home = official instance 1003043 (doc 12547 family, contiguous BandsTrouble block 12544..12549,
//  maps.txt), 7980-7984 the Stage 1003045 (doc 12549). The NPCs are festival NPCs on those maps (npcs.txt 8851013-8851028).
//  DERIVED (README): the order of the steps where the texts don`t say it, the NPC cells, the PROXY meshes of Antonio /
//  Antonio`s Father / Conner / Old Guitar, and the reward: 1 Music Coin 3600726 at the end of 7979, 7981, 7983 and 7984
//  (like the other bandsmen: the second chapter of a story pays 1 Music Coin; the client has no prize for these).
//  Entry: the window`s Antonio '?' (MusicBox_ClickWenHao</N>4) from Twin City. Clicks: SummerWhalesUi.OnNpc delegates here.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.Game.MsgServer;
using G = Qahr.Mobile.SummerWhales.FestGive;
using SE = Qahr.Mobile.FestivalStory.StoryEntry;

namespace Qahr.Mobile.BandsTrouble
{
    public static class AntonioStory
    {
        public const uint Home = 1003043, Stage = 1003045;
        // festival NPC ids (npcs.txt)
        public const uint HAntonio = 8851013, HFather = 8851014, HChest1 = 8851015, HChest2 = 8851016, HChest3 = 8851017;
        public const uint SAntonio = 8851018, SConner = 8851019, SMiko = 8851020, SMaria = 8851021, SPlayer = 8851022,
                          SClutter = 8851023, SGuitar = 8851024, SNook = 8851025, SDrum = 8851026, SBass = 8851027, SSax = 8851028;
        public static readonly HashSet<uint> Npcs = new HashSet<uint> { HAntonio, HFather, HChest1, HChest2, HChest3, SAntonio, SConner, SMiko, SMaria, SPlayer, SClutter, SNook, SDrum, SBass, SSax };

        enum K { Talk, Search, Fight }
        sealed class Page { public string Text, Option; public Page(string t, string o) { Text = t; Option = o; } }
        sealed class Step
        {
            public K Kind; public uint Npc; public Page[] Pages = new Page[0];
            public string Before, After;           // Search: message; Fight: message when it appears / after the kill
            public string Monster; public ushort X, Y;
        }
        sealed class Chapter { public uint Id, Map; public ushort X, Y; public string Intro; public Step[] Steps; public bool Coin; }

        static string T(int q, string k) { string v; return AntonioTexts.T.TryGetValue(q + "." + k, out v) ? v : ""; }
        static string TT(int q, params string[] ks) { return string.Join(" ", ks.Select(k => T(q, k)).Where(s => s.Length > 0)); }
        static Step Talk(uint npc, params Page[] p) { return new Step { Kind = K.Talk, Npc = npc, Pages = p }; }
        static Step Search(uint npc, string msg) { return new Step { Kind = K.Search, Npc = npc, Before = msg }; }
        static Step Fight(uint npc, string monster, ushort x, ushort y, string appear, string after) { return new Step { Kind = K.Fight, Npc = npc, Monster = monster, X = x, Y = y, Before = appear, After = after }; }

        static Chapter[] _chapters;
        static Chapter[] Chapters
        {
            get
            {
                if (_chapters != null) return _chapters;
                _chapters = new[]
                {
                    new Chapter { Id = 7978, Map = Home, X = 100, Y = 116, Intro = T(7978, "MsgBox1"), Steps = new[]
                    {
                        Talk(HFather, new Page(TT(7978, "Text5", "Text6"), T(7978, "Option3")), new Page(TT(7978, "Text7", "Text8"), T(7978, "Option4")), new Page(TT(7978, "Text3", "Text4"), T(7978, "Option2"))),
                        Talk(HAntonio, new Page(TT(7978, "Text10", "Text11"), T(7978, "Option5")), new Page(T(7978, "Text12"), T(7978, "Option6")), new Page(T(7978, "Text13"), T(7978, "Option7")),
                            new Page(T(7978, "Text14"), T(7978, "Option8")), new Page(T(7978, "Text15"), T(7978, "Option9"))),
                    } },
                    new Chapter { Id = 7979, Map = Home, X = 100, Y = 116, Coin = true, Steps = new[]
                    {
                        Search(HChest1, T(7979, "MsgBox1")),
                        Fight(HChest2, "NotePixie", 96, 76, T(7979, "MsgBox2"), T(7979, "MsgBox4")),
                        Search(HChest3, T(7979, "MsgBox3")),
                        Talk(HAntonio, new Page(T(7979, "Text1"), T(7979, "Option1"))),
                        Talk(HFather, new Page(T(7979, "Text2"), T(7979, "Option2")), new Page(T(7979, "Text3"), T(7979, "Option3"))),
                    } },
                    new Chapter { Id = 7980, Map = Stage, X = 57, Y = 52, Intro = T(7980, "MsgBox1"), Steps = new[]
                    {
                        Talk(SAntonio, new Page(TT(7980, "Text3", "Text4"), T(7980, "Option1")), new Page(TT(7980, "Text5", "Text6"), T(7980, "Option2")), new Page(T(7980, "Text7"), T(7980, "Option3"))),
                        Talk(SConner, new Page(T(7980, "Text9"), T(7980, "Option4")), new Page(TT(7980, "Text10", "Text11"), T(7980, "Option5"))),
                    } },
                    new Chapter { Id = 7981, Map = Stage, X = 57, Y = 52, Coin = true, Steps = new[]
                    {
                        Fight(SPlayer, "DiscordGremlin", 54, 60, TT(7981, "MsgBox1", "MsgBox2"), TT(7981, "MsgBox4", "MsgBox3")),
                        Talk(SConner, new Page(T(7981, "Text1"), T(7981, "Option1")), new Page(T(7981, "Text2"), T(7981, "Option2")), new Page(TT(7981, "Text3", "Text4"), T(7981, "Option3")), new Page(T(7981, "Text5"), T(7981, "Option4"))),
                    } },
                    new Chapter { Id = 7982, Map = Stage, X = 57, Y = 52, Steps = new[]
                    {
                        Talk(SMaria, new Page(T(7982, "Text9"), "OK.")),
                        Talk(SMiko, new Page(TT(7982, "Text4", "Text5"), T(7982, "Option2")), new Page(TT(7982, "Text6", "Text7"), T(7982, "Option3"))),
                        Search(SClutter, TT(7982, "MsgBox2", "MsgBox4")),
                        Fight(SNook, "SnowDemon", 60, 60, T(7982, "MsgBox5"), T(7982, "MsgBox3")),
                        Talk(SMaria, new Page(T(7982, "Text8"), T(7982, "Option4"))),
                    } },
                    new Chapter { Id = 7983, Map = Stage, X = 57, Y = 52, Intro = T(7983, "MsgBox1"), Coin = true, Steps = new[]
                    {
                        Talk(SConner, new Page(T(7983, "Text3"), T(7983, "Option2"))),
                        Search(SDrum, T(7983, "MsgBox2")),
                        Search(SBass, T(7983, "MsgBox3")),
                        Search(SSax, TT(7983, "MsgBox4", "MsgBox5")),
                        Talk(SConner, new Page(T(7983, "Text1"), T(7983, "Option1")), new Page(T(7983, "Text2"), "OK.")),
                    } },
                    new Chapter { Id = 7984, Map = Stage, X = 57, Y = 52, Intro = T(7984, "MsgBox1"), Coin = true, Steps = new[]
                    {
                        Talk(SAntonio, new Page(T(7984, "Text4"), T(7984, "Option3")), new Page(T(7984, "Text5"), T(7984, "Option4")), new Page(TT(7984, "Text2", "Text3"), T(7984, "Option2"))),
                        Talk(SMiko, new Page(T(7984, "Text6"), T(7984, "Option5")), new Page(TT(7984, "Text7", "Text8"), T(7984, "Option6")), new Page(TT(7984, "Text9", "Text10"), T(7984, "Option7")),
                            new Page(TT(7984, "MsgBox2", "Text11"), T(7984, "Option8"))),
                        Talk(SAntonio, new Page(T(7984, "Text1"), T(7984, "Option1"))),
                    } },
                };
                return _chapters;
            }
        }
        public static int ChapterCount { get { return Chapters.Length; } }

        static SummerWhales.FestPlayerStore _store;
        public static SummerWhales.FestPlayerStore Store { get { return _store ?? (_store = new SummerWhales.FestPlayerStore("BandsTrouble")); } }
        public static void ResetForTests() { _store = null; _mobs.Clear(); _last.Clear(); }
        static readonly ConcurrentDictionary<uint, Game.MsgMonster.MonsterRole> _mobs = new ConcurrentDictionary<uint, Game.MsgMonster.MonsterRole>();
        static readonly ConcurrentDictionary<uint, long> _last = new ConcurrentDictionary<uint, long>();
        static bool Throttled(uint uid, int ms)
        {
            long now = Environment.TickCount & 0x7FFFFFFF, prev;
            if (_last.TryGetValue(uid, out prev) && now - prev >= 0 && now - prev < ms) return true;
            _last[uid] = now;
            return false;
        }

        public static int CurrentChapter(uint uid) { return (int)Store.Get(uid, "ant_ch"); }
        public static bool HasStory(uint uid) { return CurrentChapter(uid) < Chapters.Length; }

        /// <summary>The window`s Antonio '?': to the map of the current chapter.</summary>
        public static string Enter(Client.GameClient c)
        {
            var p = c.Player;
            if (p.Level < BandsTroubleUi.MinLevel) { SE.Msg(c, "Your level is less than " + BandsTroubleUi.MinLevel + ". Please level up first!"); return "level"; }
            int ch = CurrentChapter(p.UID);
            if (ch >= Chapters.Length) { SE.Msg(c, "You have already helped this bandsman. Thank you!"); return "done"; }
            if (!p.Alive || p.DynamicID != 0 || p.Map != SE.TwinCity) { SE.Msg(c, BandsTroubleUi.NotInTown); return "town"; }
            var chap = Chapters[ch];
            if (Pool.ServerMaps[chap.Map] == null) { SE.Msg(c, "This story isn`t open yet."); return "nomap"; }
            var o = QuestEngine.QuestEngineCore.TeleportOverride;
            if (o != null) o(c, chap.X, chap.Y, chap.Map, 0); else c.Teleport(chap.X, chap.Y, chap.Map, 0);
            if (Store.Get(p.UID, "ant_st") == 0 && Store.Get(p.UID, "ant_pg") == 0 && !string.IsNullOrEmpty(chap.Intro)) SE.Msg(c, chap.Intro);
            SE.Msg(c, "[Antonio] " + Hint(chap, (int)Store.Get(p.UID, "ant_st")));
            return "enter " + chap.Id;
        }

        static string NpcName(uint id)
        {
            switch (id)
            {
                case HAntonio: case SAntonio: return "Antonio";
                case HFather: return "Antonio`s Father";
                case HChest1: case HChest2: case HChest3: return "the Chest";
                case SConner: return "Conner";
                case SMiko: return "Miko";
                case SMaria: return "Maria";
                case SPlayer: return "the Player";
                case SGuitar: return "the Old Guitar";
                case SNook: return "the Nook";
                case SDrum: return "the Drumbeat";
                case SBass: return "the Bass";
                case SSax: return "the Saxophone";
                default: return "the Clutter Heap";
            }
        }
        static string Hint(Chapter chap, int st) { return st < chap.Steps.Length ? "Next: " + NpcName(chap.Steps[st].Npc) + "." : ""; }

        static void Dialog(Client.GameClient c, ServerSockets.Packet stream, string text, string option, byte opt)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText(text);
            if (!string.IsNullOrEmpty(option)) d.AddOption(option, opt);
            d.AddOption("Let me think.", 255).AddAvatar(0).FinalizeDialog();
        }

        /// <summary>A click on one of Antonio`s story NPCs (in view; option 0 = the first click, 1 = the page option).</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option)
        {
            if (c == null || c.Player == null || !Npcs.Contains(npcid)) return false;
            if (!BandsTroubleUi.Running) return false;
            if (option == 255) return true;
            if (Throttled(c.Player.UID, 300)) return true;
            uint uid = c.Player.UID;
            int ch = CurrentChapter(uid);
            if (ch >= Chapters.Length) { Dialog(c, stream, npcid == SAntonio || npcid == HAntonio ? T(7984, "Text1") : "Thank you for helping the Hot Summer Band!", null, 0); return true; }
            var chap = Chapters[ch];
            int st = (int)Store.Get(uid, "ant_st"), pg = (int)Store.Get(uid, "ant_pg");
            if (st >= chap.Steps.Length) st = chap.Steps.Length - 1;
            var step = chap.Steps[st];
            if (c.Player.Map != chap.Map || step.Npc != npcid)
            {
                Dialog(c, stream, (npcid == HFather && chap.Id == 7978 && st > 0) ? T(7978, "Text2") : (npcid == SAntonio && chap.Id == 7980 && st > 0) ? T(7980, "Text2") : "...", null, 0);
                SE.Msg(c, "[Antonio] " + Hint(chap, st));
                return true;
            }
            switch (step.Kind)
            {
                case K.Talk:
                    if (option == 1)
                    {
                        if (pg + 1 < step.Pages.Length) { Store.Set(uid, "ant_pg", pg + 1); var nx = step.Pages[pg + 1]; Dialog(c, stream, nx.Text, nx.Option, 1); return true; }
                        Advance(c, stream, chap, st);
                        return true;
                    }
                    var pp = step.Pages[Math.Min(pg, step.Pages.Length - 1)];
                    Dialog(c, stream, pp.Text, pp.Option, 1);
                    return true;
                case K.Search:
                    SE.Msg(c, step.Before);
                    Advance(c, stream, chap, st);
                    return true;
                case K.Fight:
                    {
                        Game.MsgMonster.MonsterRole mob;
                        bool had = _mobs.TryGetValue(uid, out mob) && mob != null;
                        if (had && !mob.Alive && Store.Get(uid, "ant_fight") == st + 1)
                        {
                            _mobs.TryRemove(uid, out mob);
                            SE.Msg(c, step.After);
                            Advance(c, stream, chap, st);
                            return true;
                        }
                        if (had && mob.Alive) { SE.Msg(c, step.Before); return true; }
                        var m = SpawnFor(c, chap.Map, step.Monster, step.X, step.Y);
                        if (m == null) { SE.Msg(c, "Nothing happens. Please try again later."); return true; }
                        _mobs[uid] = m;
                        Store.Set(uid, "ant_fight", st + 1);
                        SE.Msg(c, step.Before);
                        return true;
                    }
            }
            return true;
        }

        /// <summary>The fight step is won when the player`s monster is dead; checked on the next click of the step NPC.</summary>
        public static bool FightWon(uint uid) { Game.MsgMonster.MonsterRole m; return _mobs.TryGetValue(uid, out m) && m != null && !m.Alive; }

        static void Advance(Client.GameClient c, ServerSockets.Packet stream, Chapter chap, int st)
        {
            uint uid = c.Player.UID;
            int ch = Array.IndexOf(Chapters, chap);
            if (st + 1 < chap.Steps.Length)
            {
                if (!Store.Update(uid, get => get("ant_ch") != ch || get("ant_st") != st ? null : new Dictionary<string, long> { { "ant_st", st + 1 }, { "ant_pg", 0 }, { "ant_fight", 0 } })) return;
                SE.Msg(c, "[Antonio] " + Hint(chap, st + 1));
                return;
            }
            // chapter done: state first, then the coin
            if (chap.Coin && !c.Inventory.HaveSpace(1)) { SE.Msg(c, "Please make 1 space(s) in your inventory first."); return; }
            if (!Store.Update(uid, get => get("ant_ch") != ch || get("ant_st") != st ? null : new Dictionary<string, long> { { "ant_ch", ch + 1 }, { "ant_st", 0 }, { "ant_pg", 0 }, { "ant_fight", 0 } })) return;
            if (chap.Coin) G.Give(c, new[] { new G.GItem { Id = BandsTroubleUi.MusicCoin, Count = 1 } }, stream, "BandsTrouble/Antonio" + chap.Id);
            try { Festivals.FestivalManager.Log("story", "BandsTrouble", c.Player.Name + "(" + uid + ") finished Antonio " + chap.Id); } catch { }
            if (ch + 1 < Chapters.Length)
            {
                var next = Chapters[ch + 1];
                SE.Msg(c, "[Antonio] Chapter complete" + (chap.Coin ? ": you received 1 Music Coin" : "") + ". " + (next.Map != chap.Map ? "Click Antonio in the Hot Music Festival window to continue." : (next.Intro ?? "") + " " + Hint(next, 0)));
            }
            else SE.Msg(c, "[Antonio] You have helped Antonio through all his troubles" + (chap.Coin ? " and received 1 Music Coin" : "") + ". Thank you!");
        }

        static Game.MsgMonster.MonsterRole SpawnFor(Client.GameClient c, uint mapId, string monster, ushort x, ushort y)
        {
            try
            {
                if (Pool.MonsterFamilies == null) return null;
                var fam = Pool.MonsterFamilies.Values.FirstOrDefault(f => f != null && string.Equals(f.Name, monster, StringComparison.OrdinalIgnoreCase));
                if (fam == null) return null;
                var map = Pool.ServerMaps[mapId];
                if (map == null) return null;
                if (map.MonstersColletion == null) map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
                if (!map.MonstersColletion.ReadMap()) return null;
                var copy = fam.Copy();
                copy.SpawnX = x; copy.SpawnY = y; copy.MaxSpawnX = (ushort)(x + 3); copy.MaxSpawnY = (ushort)(y + 3);
                copy.MapID = map.ID; copy.SpawnCount = 1;
                Game.MsgMonster.MonsterRole mob = null;
                for (int i = 0; i < 4 && mob == null; i++) mob = map.MonstersColletion.Add(copy, true, 0, true);
                if (mob == null) return null;
                mob.RemoveOnDead = true;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var s = rec.GetStream();
                    try
                    {
                        mob.Send(s.ActionCreate(new ActionQuery { ObjId = mob.UID, Type = ActionType.ReviveMonster, PositionX = mob.X, PositionY = mob.Y }));
                        mob.Send(mob.GetArray(s, false));
                    }
                    catch { }
                }
                return mob;
            }
            catch (Exception e) { try { MyConsole.SaveException(e); } catch { } return null; }
        }
    }
}
