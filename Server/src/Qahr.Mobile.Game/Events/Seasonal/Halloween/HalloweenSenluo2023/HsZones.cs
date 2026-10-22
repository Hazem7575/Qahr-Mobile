// ============================================================================
//  [HalloweenSenluo2023 2026-10-15] Macabre Realm gameplay (Global Halloween 2023, ERP559756, 10.24-11.06).
//
//  Official flow (Server_Key tHalloweenNpc2023 / tHallowMonster2023_DelText / tHalloweenDie_DelText, luaUiRes[1435]):
//    Count Dracula (Twin City) -> window DlgSenluoHalloweenMainUi -> choose ONE peripheral zone per day:
//      1 Death Sideroad (monsters)  : Blood-stained Signpost 32115, 3 passages, servants -> corroded relic -> Mysterious
//                                     Crack; the right crack leads out ("obtained %s" = Macabre Flames), a wrong one
//                                     back to the crossroads.
//      2 Blood Altar (Blood Zone)   : search the corpses on the altars (Secret Macabre Bag / Supply Pack / Black
//                                     Succubus / ghost / Secret Key), open the Mysterious Crack 32166 with the key.
//    passing the zone opens the Macabre Realm window DlgSenluoHalloweenGameUi: 4 halls (West/North/South/East, Counts
//    Nicolas/Friss/Apada/Jupp), Macabre Flame to challenge or mop up, seals, server-wide passing progress, Mighty Lord
//    with 4 seals + Destiny Card, Macabre Treasury (Count Dracula), Macabre Realm Ranking (Macabre Points).
//
//  Maps: Death Sideroad = 12270 (2021qixi_new, the maze file of NightmareMaze 11886/11414; 12270 sits in the 2023
//        Halloween block next to 12273) - DERIVED. Blood Altar = 12273 PeripheralZone (GameMapEx name, luacfg minimap
//        block "--外围死域": entrance 234,164, 5 altars, exit 187,186) - official. Halls / Mighty Lord = inner challenge
//        maps 12290/12286/12285/12292/12283 of the block 12275..12292 ("--内围 挑战") - which hall uses which map is DERIVED.
//  Monsters: servants 14639-14641 + hall monsters 21881-21885 = official monster.dat level/life; relics, Black Succubus,
//        Counts, Mighty Lord have no monster.dat row -> stats derived; every look is DERIVED (festival monsters\*.ini).
//  Rewards: item texts in itemtype.txt (official): Xuanyuan/Pangu/Shennong Box (relics), Secret Macabre Bag / Supply
//        Pack (corpses), Elite Macabre Box (hall combat), Nicolas`/Friss`/Apada`s/Jupp`s Box (Counts), Macabre
//        Iron/Silver/Gold Box (Mighty Lord, by danger level), Local No.x Macabre Box (ranking). Amounts in config.json.
// ============================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.HalloweenSenluo2023
{
    public static class HsZones
    {
        public const string Festival = HsState.Festival;
        public const uint Flame = 3345604, Card = 3345605, Coin = 3600645;
        public const uint SecretBag = 3345690, SupplyPack = 3345691, EliteBox = 3345711;
        public const uint IronBox = 3345615, SilverBox = 3345616, GoldBox = 3345617, CoinPack = 3345609;
        public static readonly uint[] RelicBox = { 0, 3345658, 3345659, 3345660 };          // Xuanyuan / Pangu / Shennong Box
        public static readonly uint[] CountBox = { 0, 3345712, 3345713, 3345714, 3345715 };  // Nicolas` / Friss` / Apada`s / Jupp`s Box
        public static readonly uint[] HallReward = { 0, 3345606, 3345607, 3345608, 3345618 };// server-wide reward (derived pick of event items)

        // ---- NPC ids / looks (npc.ini NpcTypeN x10, NpcX id x10)
        public const uint Dracula = 8823001;              // Count Dracula (official 32116, NpcX 110087 [2023Halloween])
        public const uint DraculaMesh = 1100870;
        public const uint Signpost = 32115, SignpostMesh = 6900;     // NpcType690 Blood-stainedSignpost [DeathSideroad]
        public const uint SideCrack = 8823010, SideCrackMesh = 6910; // NpcType691 MysteriousCrack [DeathSideroad] (+passage)
        public const uint BloodCrack = 32166, BloodCrackMesh = 37560;// NpcType3756 MysteriousCrack [Exit]
        public const uint Corpse = 8823100;                          // + altar*10 + k; NpcType3765/3766/3767 CorpseOf...
        public static readonly uint[] CorpseMesh = { 37650, 37660, 37670 };
        public static readonly string[] CorpseName = { "CorpseOfWarrior", "CorpseOfTaoist", "CorpseOfArcher" };
        public const uint Ghost = 32154;                             // 32154..32157 Jess / Courie / Elis / Dilys (NpcType3758..3761)
        public static readonly string[] GhostName = { "Jess", "Courie", "Elis", "Dilys" };
        public const uint KeyNpc = 8823200, KeyMesh = 37570;         // NpcType3757 SecretKey
        public const uint ExitCrack = 8823300;                       // halls / Mighty Lord exit (NpcType3756 [Exit])

        // ---- monsters (Festivals\HalloweenSenluo2023\monsters)
        public static readonly uint[] Servant = { 0, 14639, 14640, 14641 };   // YuanServant / PanguServant / ShennongServant
        public static readonly uint[] Relic = { 0, 994704, 994705, 994706 };     // CorrodedXuanyuan / CorrodedPangu / CorrodedShennong
        public static readonly int[] AddBossKey = { 0, 14639, 14640, 14641 };
        public const uint Succubus = 994707;                                      // BlackSuccubus
        public static readonly uint[] HallMob = { 0, 21881, 21882, 21883, 21885 };// EarthGolem / Incubus / FlameLizard / WhiteSuccubus
        public static readonly uint[] Count = { 0, 994712, 994713, 994714, 994715 };  // Nicolas / Friss / Apada / Jupp
        public const uint MightyLord = 994716;

        // ---- maps / points
        public const uint SideMap = 12270, BloodMap = 12273, LordMap = 12283;
        static readonly ushort[] SideEntry = { 292, 247 }, SideSign = { 290, 247 };
        static readonly ushort[][] Passage = { null, new ushort[] { 201, 110 }, new ushort[] { 244, 350 }, new ushort[] { 143, 220 } };
        static readonly ushort[] BloodEntry = { 234, 164 }, BloodCrackAt = { 187, 186 };
        static readonly ushort[][] Altar = { new ushort[] { 145, 89 }, new ushort[] { 273, 145 }, new ushort[] { 288, 264 }, new ushort[] { 186, 270 }, new ushort[] { 84, 173 } };
        static readonly sbyte[][] CorpseOff = { new sbyte[] { 3, 0 }, new sbyte[] { 0, 3 }, new sbyte[] { -3, 0 } };
        public static readonly uint[] HallMap = { 0, 12290, 12286, 12285, 12292 };
        static readonly ushort[][] HallEntry = { null, new ushort[] { 75, 67 }, new ushort[] { 56, 69 }, new ushort[] { 101, 121 }, new ushort[] { 67, 76 } };
        static readonly ushort[][] HallMobAt = { null, new ushort[] { 79, 58 }, new ushort[] { 53, 58 }, new ushort[] { 101, 107 }, new ushort[] { 72, 66 } };
        static readonly ushort[][] HallBossAt = { null, new ushort[] { 77, 51 }, new ushort[] { 56, 51 }, new ushort[] { 101, 103 }, new ushort[] { 67, 58 } };
        static readonly ushort[] LordEntry = { 50, 70 }, LordAt = { 50, 52 };
        public const uint ExitMap = 1002; public const ushort ExitX = 400, ExitY = 384;
        public const ushort DraculaX = 400, DraculaY = 380;// Twin City (DERIVED: no official position; walkable 7x7, free of Npcs.txt NPCs)

        public static readonly string[] HallName = { "", "West Hall", "North Hall", "South Hall", "East Hall" };
        public static readonly string[] SealName = { "", "Nicolas Seal", "Friss Seal", "Apada Seal", "Jupp Seal" };

        // ---------------------------------------------------------------- common checks
        static string Lv(Client.GameClient c) { return "Sorry, you need to reach level " + HsConfig.Get().MinLevel + " first."; }

        /// <summary>Official refusals before entering any zone/hall (level, team, booth, in another copy, map). Null = ok.</summary>
        static string CanEnter(Client.GameClient c)
        {
            if (!HsKit.Running(Festival)) return "The Halloween event is over.";
            if (c.Player.Level < HsConfig.Get().MinLevel) return Lv(c);
            if (!c.Player.Alive) return "You are dead.";
            try { if (c.Team != null) return "You can`t join in the event when you are in a team. Please dismiss the team first."; } catch { }
            try { if (c.IsVendor) return "You are peddling. Please try again when you finish."; } catch { }
            if (c.Player.DynamicID != 0) return "Cannot enter the Macabre Realm from the current location.";
            return null;
        }
        public static bool InCity(Client.GameClient c)
        {
            uint m = c.Player.Map;
            return m == 1002 || m == 1000 || m == 1011 || m == 1015 || m == 1020 || m == 1036;
        }

        // ---------------------------------------------------------------- coins / points / grants
        /// <summary>Gives Macabre Coins and the same amount of Macabre Points (item texts 3345609 / Text159).</summary>
        public static void GrantCoins(Client.GameClient c, HsPlayer p, int n, ServerSockets.Packet stream)
        {
            if (n <= 0) return;
            string got = Give(c, p, new HsGrant(Coin, n, false), stream);
            lock (HsState.Lock) p.Points += n;
            HsKit.Msg(c, "Congrats on earning " + n + " Macabre Points. You have earned " + p.Points + " Macabre Points in total.");
        }

        /// <summary>One grant; when the bag is full it waits in the player's pending list (delivered by the window).</summary>
        public static string Give(Client.GameClient c, HsPlayer p, HsGrant g, ServerSockets.Packet stream)
        {
            if (HsKit.HasSpace(c, HsKit.SlotsFor(new[] { g })))
            {
                string s = HsKit.Give(c, g, stream);
                if (s != null) { Log(c, "give " + s); return s; }
            }
            lock (HsState.Lock) p.Pending.Add(g);
            HsKit.Msg(c, "Your inventory is full. " + HsKit.Name(g.Id) + " x" + g.Count + " is kept for you: open the Macabre Realm window with free space to receive it.");
            Log(c, "pending " + g.Id + " x" + g.Count);
            return HsKit.Name(g.Id) + (g.Count > 1 ? " x" + g.Count : "");
        }

        /// <summary>Delivers pending grants (bag space permitting). Idempotent: each grant leaves the list before it is given.</summary>
        public static void DeliverPending(Client.GameClient c, HsPlayer p, ServerSockets.Packet stream)
        {
            while (true)
            {
                HsGrant g;
                lock (HsState.Lock)
                {
                    if (p.Pending.Count == 0) return;
                    g = p.Pending[0];
                    if (!HsKit.HasSpace(c, HsKit.SlotsFor(new[] { g }))) { HsKit.Msg(c, "Please make some room in your inventory to receive your kept Macabre rewards."); return; }
                    p.Pending.RemoveAt(0);
                }
                string s = HsKit.Give(c, g, stream);
                if (s == null) { lock (HsState.Lock) p.Pending.Insert(0, g); return; }
                Log(c, "pending delivered " + s);
                HsKit.Msg(c, "You received " + s + ".");
            }
        }

        static void Log(Client.GameClient c, string what)
        {
            try { Festivals.FestivalManager.Log("H2023", Festival, c.Player.UID + " " + c.Player.Name + " | " + what); } catch { }
        }

        // ---------------------------------------------------------------- zone entry (window buttons)
        /// <summary>HallowMonster2023_EnterInstanceMain (zone 1) / HalloweenDie_WaiWeiEnter (zone 2).</summary>
        public static void EnterZone(Client.GameClient c, int zone)
        {
            var p = HsState.Of(c);
            string why = CanEnter(c);
            if (why == null && !InCity(c)) why = "Cannot enter the Macabre Realm from the current location.";
            if (why != null) { HsKit.Msg(c, why); return; }
            lock (HsState.Lock)
            {
                if (p.ZonePassed) { HalloweenSenluo2023Ui.SendGame(c); return; }// today's zone done: the Macabre Realm is open
                if (p.Zone != 0 && p.Zone != zone)
                {
                    HsKit.Msg(c, zone == 2 ? "You have selected the Death Sideroad, so you cannot go to the Blood Altar now." : "This area has been blocked and will not be reset until the next day.");
                    return;
                }
            }
            if (!HsKit.HasSpace(c, 5)) { HsKit.Msg(c, "Your inventory is full. Please prepare at least 5 space(s) in your inventory first."); return; }
            if (zone == 1) EnterSideroad(c, p); else EnterBlood(c, p);
        }

        // ---------------------------------------------------------------- Death Sideroad
        static void EnterSideroad(Client.GameClient c, HsPlayer p)
        {
            var cfg = HsConfig.Get();
            var r = HsKit.Open(c, Festival, "sideroad", SideMap, SideEntry[0], SideEntry[1], cfg.ZoneMinutes, ExitMap, ExitX, ExitY);
            if (r == null) { HsKit.Msg(c, "This zone of the Macabre Realm isn`t open on this server yet."); return; }
            lock (HsState.Lock)
            {
                p.Zone = 1;
                if (p.Correct < 1 || p.Correct > 3) p.Correct = 1 + HsKit.Roll(3);
            }
            HsState.Save();
            r.OnKill = SideroadKill;
            HsKit.AddNpc(r, Signpost, SignpostMesh, "Blood-stainedSignpost", SideSign[0], SideSign[1]);
            HsKit.Msg(c, "There seems to be something written on the Blood-stained Signpost. Go take a look!");
            Log(c, "enter Death Sideroad (correct passage " + p.Correct + ")");
        }

        static void SignpostMenu(Client.GameClient c, ServerSockets.Packet stream)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("Blood-stained Signpost:\n--------------------------Death Sideroad------------------------\n1. There are three Death Sideroads, infested with monsters. Eliminate all monsters to bring out\nthe corroded relics.\n");
            d.AddText("2. Defeating the corroded relics will open Mysterious Cracks, which give you a chance to leave the Death Sideroads.\n--------------------------------------------------------\n");
            d.AddOption("Enter the upper-right passage.", 1).AddOption("Enter the lower-left passage.", 2).AddOption("Enter the upper-left passage.", 3);
            d.AddOption("Return to Twin City.", 9).AddAvatar(10).FinalizeDialog();
            Expect(c, Signpost, 1, 2, 3, 9);
        }

        static void StartPassage(Client.GameClient c, HsCopy r, HsPlayer p, int k)
        {
            if (r.Get("passage") != 0) { HsKit.Msg(c, "Let`s deal with the trouble in front of us first! Those monsters are ferocious!"); return; }
            lock (HsState.Lock)
                if ((p.Tried & (1 << k)) != 0) { HsKit.Msg(c, "Feeling dizzy, you found yourself back at the crossroads. Keep searching for the exit!"); return; }
            r.Set("passage", k);
            var at = Passage[k];
            HsKit.MoveInside(c, r, at[0], at[1]);
            int n = HsConfig.Get().SideroadServants;
            for (int i = 0; i < n; i++) HsKit.Spawn(r, Servant[k], (ushort)(at[0] + 3), (ushort)(at[1] - 3), 6, "servant");
            HsKit.Msg(c, GoTips[HsKit.Rand(GoTips.Length)]);
        }
        static readonly string[] GoTips =
        {
            "A sense of horror shrouds you, and you can hear your own heartbeat clearly, beating faster and faster.",
            "You feel like there are eyes behind you. Don`t turn back!",
            "Terrified screams from ahead intensify the splitting headache you`re experiencing.",
            "The sound of a beast chewing is coming from nearby. Where could it be hiding?",
            "The place is silent, and only the sound of your footsteps can be heard.",
            "Shh... Don`t make a sound. There`s a monster watching you!",
        };
        static readonly string[] RelicName = { "", "Corroded Xuanyuan", "Corroded Pangu", "Corroded Shennong" };

        static void SideroadKill(HsCopy r, HsMob m, Client.GameClient c)
        {
            int k = (int)r.Get("passage");
            if (k < 1 || k > 3 || c == null) return;
            if (m.Tag == "servant" && r.Alive("servant") == 0 && r.Get("relic") == 0)
            {
                r.Set("relic", 1);
                var at = Passage[k];
                HsKit.Spawn(r, Relic[k], at[0], (ushort)(at[1] - 4), 2, "relic");
                HsKit.Center(c, "In a surging black mist, " + RelicName[k] + " launched an attack! Be careful!");
            }
            else if (m.Tag == "relic" && r.Get("crack") == 0)
            {
                r.Set("crack", 1);
                var p = HsState.Of(c);
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    string got = Give(c, p, new HsGrant(RelicBox[k], 1, true), rec.GetStream());
                    HsKit.Msg(c, "You received " + got + ".");
                }
                HsState.Save();
                var at = Passage[k];
                HsKit.AddNpc(r, SideCrack + (uint)k, SideCrackMesh, "MysteriousCrack", at[0], (ushort)(at[1] - 4));
            }
        }

        static void SideCrackClick(Client.GameClient c, HsCopy r, HsPlayer p, int k, ServerSockets.Packet stream)
        {
            if (r.Get("passage") != k || r.Get("crack") == 0) return;
            bool right;
            lock (HsState.Lock)
            {
                right = p.Correct == k || (p.Tried | (1 << k)) == 0xE;// the last untried passage is always the way out
                p.Tried |= 1 << k;
            }
            HsKit.RemoveNpc(r, SideCrack + (uint)k);
            r.Set("passage", 0); r.Set("relic", 0); r.Set("crack", 0);
            if (!right)
            {
                HsState.Save();
                HsKit.MoveInside(c, r, SideEntry[0], SideEntry[1]);
                HsKit.Center(c, "Feeling dizzy, you found yourself back at the crossroads. Keep searching for the exit!");
                return;
            }
            PassZone(c, r, p, HsConfig.Get().SideroadPassFlames, stream, true);
        }

        /// <summary>The zone is passed: Macabre Flames, the Macabre Realm window, back to Twin City.</summary>
        static void PassZone(Client.GameClient c, HsCopy r, HsPlayer p, int flames, ServerSockets.Packet stream, bool sideroad)
        {
            lock (HsState.Lock)
            {
                if (p.ZonePassed) return;
                p.ZonePassed = true;
            }
            string got = flames > 0 ? Give(c, p, new HsGrant(Flame, flames, true), stream) : "";
            HsState.Save();
            Log(c, (sideroad ? "Death Sideroad" : "Blood Altar") + " passed, " + got);
            if (sideroad) HsKit.Center(c, "Feeling dizzy, you successfully walked out of the Death Sideroad and obtained " + got + ".");
            else HsKit.Center(c, "You have completed the peripheral quests. Go inside the realm!" + (got.Length > 0 ? " You obtained " + got + "." : ""));
            HsKit.Leave(c);
            HalloweenSenluo2023Ui.SendGame(c);
        }

        // ---------------------------------------------------------------- Blood Altar
        static void EnterBlood(Client.GameClient c, HsPlayer p)
        {
            var cfg = HsConfig.Get();
            var r = HsKit.Open(c, Festival, "blood", BloodMap, BloodEntry[0], BloodEntry[1], cfg.ZoneMinutes, ExitMap, ExitX, ExitY);
            if (r == null) { HsKit.Msg(c, "This zone of the Macabre Realm isn`t open on this server yet."); return; }
            lock (HsState.Lock) p.Zone = 2;
            HsState.Save();
            r.OnKill = BloodKill;
            HsKit.AddNpc(r, BloodCrack, BloodCrackMesh, "MysteriousCrack", BloodCrackAt[0], BloodCrackAt[1]);
            for (int a = 0; a < Altar.Length; a++)
                for (int k = 0; k < 3; k++)
                    HsKit.AddNpc(r, Corpse + (uint)(a * 10 + k), CorpseMesh[k], CorpseName[k], (ushort)(Altar[a][0] + CorpseOff[k][0]), (ushort)(Altar[a][1] + CorpseOff[k][1]));
            HsKit.Center(c, "After a flash of light, you were teleported to Blood Altar. There is a crack in front of you. Check it out.");
            Log(c, "enter Blood Altar");
        }

        static void BloodCrackMenu(Client.GameClient c, ServerSockets.Packet stream)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("Mysterious Crack:\nA mysterious sacrificial ceremony was once held here. After the sacrifice, this place became a realm of death, without any living beings. However, the altars are piled with mountains of corpses.\n");
            d.AddText("Perhaps the key to Mysterious Cracks are hidden in the corpses.\n");
            d.AddOption("Go to the altar.", 1).AddOption("Open the crack.", 2).AddOption("Return toTwin City.", 3).AddAvatar(10).FinalizeDialog();
            Expect(c, BloodCrack, 1, 2, 3);
        }

        static void SearchCorpse(Client.GameClient c, HsCopy r, HsPlayer p, uint npc, ServerSockets.Packet stream)
        {
            if (r.Alive("succubus") > 0) { HsKit.Msg(c, "Let`s deal with the trouble in front of us first! Those monsters are ferocious!"); return; }
            var np = HsKit.NpcOf(r, npc);
            if (np == null) return;
            HsKit.RemoveNpc(r, npc);// each corpse is searched once
            var cfg = HsConfig.Get();
            bool key;
            lock (HsState.Lock)
            {
                p.Searches++;
                key = !p.Key && r.Get("keyShown") == 0 && (p.Searches >= cfg.BloodKeyGuaranteeAt || HsKit.Roll(100) < cfg.BloodKeyChance);
            }
            HsKit.Msg(c, "Picking...");
            if (key)
            {
                r.Set("keyShown", 1);
                HsKit.AddNpc(r, KeyNpc, KeyMesh, "SecretKey", np.X, np.Y);
                HsKit.Center(c, "A burst of red light flashes and a key appears in front of you! Pick it up!");
                HsState.Save();
                return;
            }
            int[] w = cfg.BloodOutcome;
            int total = w.Sum(), roll = HsKit.Roll(Math.Max(1, total)), pick = 0;
            for (int acc = 0; pick < w.Length; pick++) { acc += w[pick]; if (roll < acc) break; }
            switch (pick)
            {
                case 0:
                    HsKit.Msg(c, "There`s a faint glow here. It`s a Secret Macabre Bag!");
                    Give(c, p, new HsGrant(SecretBag, 1, true), stream);
                    break;
                case 1:
                    HsKit.Msg(c, "You pawed around carefully and found an unopened Supply Pack!");
                    Give(c, p, new HsGrant(SupplyPack, 1, true), stream);
                    break;
                case 2:
                    HsKit.Spawn(r, Succubus, np.X, np.Y, 2, "succubus");
                    HsKit.Center(c, "Black Succubus suddenly opened eyes to attack you! Be careful!");
                    break;
                case 3:
                    {
                        int g = HsKit.Rand(4);
                        HsKit.AddNpc(r, Ghost + (uint)g, (uint)(37580 + g * 10), GhostName[g], np.X, np.Y);
                        HsKit.Center(c, "A  ghost appeared in the air. Ask him what happened!");
                        break;
                    }
                default:
                    HsKit.Msg(c, "Looks like there are no clues here. Look elsewhere!");
                    break;
            }
            HsState.Save();
        }

        static void BloodKill(HsCopy r, HsMob m, Client.GameClient c)
        {
            if (c == null || m.Tag != "succubus") return;
            var p = HsState.Of(c);
            using (var rec = new ServerSockets.RecycledPacket()) Give(c, p, new HsGrant(SupplyPack, 1, true), rec.GetStream());
            HsState.Save();
        }

        static readonly string[][] GhostTalk =
        {
            new[] { "Jess:\nMy friend told me that there are endless treasures hidden in the Macabre Palace, but I have never seen the shadow of the palace.\n", "Why didn`t your friend come?" },
            new[] { "Courie:\nIt`s my fault. I`m too incompetent to take treasures here. If there is an afterlife, I will try again!\n", "Again?" },
            new[] { "Elis:\nI received a bounty mission to find the secret to eternal youth in the Macabre Palace.\nBut I was ambushed by demons here. If I had known, I would never have taken the mission.\n", "Rest in peace." },
            new[] { "Dilys:\nI heard that the Macabre Realm is a famous ghost domain in the world, so I came here alone to explore.\nBut I didn`t even see what creature killed me.\n", "Curiosity killed the cat." },
        };

        // ---------------------------------------------------------------- halls (Macabre Palace)
        public static bool HallOpen(int i, out int daysLeft)
        {
            daysLeft = 0;
            var run = Festivals.FestivalManager.Running(Festival);
            DateTime start = run != null ? run.Started.Date : HsKit.Now.Date;
            int day = (int)(HsKit.Now.Date - start).TotalDays;
            int need = HsConfig.Get().HallOpenDay[i - 1];
            daysLeft = Math.Max(0, need - day);
            return daysLeft == 0;
        }

        /// <summary>CastleBeat2023_EnterCostKey i: Macabre Flame -> private hall copy (combat monsters, then the Count).</summary>
        public static void EnterHall(Client.GameClient c, int i)
        {
            if (i < 1 || i > 4) return;
            var p = HsState.Of(c);
            string why = CanEnter(c);
            if (why == null && !InCity(c)) why = "Cannot enter the Macabre Realm from the current location.";
            if (why != null) { HsKit.Msg(c, why); return; }
            if (!p.ZonePassed) { HsKit.Msg(c, "Pass the selected route to enter the Macabre Palace."); return; }
            int left; if (!HallOpen(i, out left)) { HsKit.Msg(c, "The " + HallName[i] + " will be available in " + left + " day(s)."); return; }
            if (HsState.Progress[i - 1] >= 100) { HsKit.Msg(c, "The passing progress reaches 100%, so you can mop up."); return; }
            var cfg = HsConfig.Get();
            if (!HsKit.HasSpace(c, 5)) { HsKit.Msg(c, "Your inventory is full. Please prepare at least 5 space(s) in your inventory first."); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!HsKit.Take(c, Flame, (uint)cfg.HallFlameCost, stream)) { HsKit.Msg(c, "You need " + cfg.HallFlameCost + " Macabre Flame(s) to light up the " + HallName[i] + "."); return; }
                var r = HsKit.Open(c, Festival, "hall" + i, HallMap[i], HallEntry[i][0], HallEntry[i][1], cfg.HallMinutes, ExitMap, ExitX, ExitY);
                if (r == null)
                {
                    HsKit.Give(c, new HsGrant(Flame, cfg.HallFlameCost, true), stream);// refund
                    HsKit.Msg(c, "The " + HallName[i] + " isn`t open on this server yet.");
                    return;
                }
                r.Set("hall", i);
                r.OnKill = HallKill;
                for (int k = 0; k < cfg.HallMobs; k++) HsKit.Spawn(r, HallMob[i], HallMobAt[i][0], HallMobAt[i][1], 5, "mob");
                Log(c, "enter " + HallName[i] + " (-" + cfg.HallFlameCost + " flame)");
                HsKit.Center(c, "You lit the Macabre Flame. Defeat the monsters of the " + HallName[i] + " to face its Count!");
            }
        }

        static void HallKill(HsCopy r, HsMob m, Client.GameClient c)
        {
            int i = (int)r.Get("hall");
            if (i < 1 || i > 4 || c == null) return;
            if (m.Tag == "mob" && r.Alive("mob") == 0 && r.Get("count") == 0)
            {
                r.Set("count", 1);
                var p = HsState.Of(c);
                using (var rec = new ServerSockets.RecycledPacket()) Give(c, p, new HsGrant(EliteBox, 1, true), rec.GetStream());
                HsKit.Spawn(r, Count[i], HallBossAt[i][0], HallBossAt[i][1], 2, "count");
                HsKit.Center(c, "Count " + CountName[i] + " appeared! Defeat him to obtain his treasures!");
            }
            else if (m.Tag == "count" && r.Get("done") == 0)
            {
                r.Set("done", 1);
                HallPassed(c, i, false);
                HsKit.AddNpc(r, ExitCrack, BloodCrackMesh, "MysteriousCrack", HallBossAt[i][0], HallBossAt[i][1]);
            }
        }
        public static readonly string[] CountName = { "", "Nicolas", "Friss", "Apada", "Jupp" };

        /// <summary>A hall passed (challenge or mop-up): Count`s Box, seal chance, progress (challenge, once a day).</summary>
        static void HallPassed(Client.GameClient c, int i, bool sweep)
        {
            var p = HsState.Of(c);
            var cfg = HsConfig.Get();
            bool seal = false, first = false, progress = false;
            lock (HsState.Lock)
            {
                p.HallPassed |= 1 << i;
                if ((p.Seals & (1 << i)) == 0 && HsKit.Roll(100) < cfg.SealOdds[i - 1])
                {
                    p.Seals |= 1 << i; seal = true;
                    if (HsState.FirstName[i - 1] == null)
                    {
                        HsState.FirstName[i - 1] = c.Player.Name;
                        string gang = null; try { gang = c.Player.MyGuild != null ? c.Player.MyGuild.GuildName : null; } catch { }
                        HsState.FirstGang[i - 1] = string.IsNullOrEmpty(gang) ? null : gang;
                        first = true;
                    }
                }
                if (!sweep && (p.HallDaily & (1 << i)) == 0)
                {
                    p.HallDaily |= 1 << i;
                    HsState.Progress[i - 1] = Math.Min(100, HsState.Progress[i - 1] + cfg.ProgressPerPass);
                    progress = true;
                }
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                string got = Give(c, p, new HsGrant(CountBox[i], 1, true), stream);
                HsKit.Msg(c, "You received " + got + ".");
                if (seal) HsKit.Center(c, "You obtained the " + SealName[i] + "!");
                if (first) { string fr = Give(c, p, new HsGrant(GoldBox, 1, true), stream); HsKit.Msg(c, "You are the first hero to obtain the " + SealName[i] + "! You received " + fr + "."); }
            }
            HsState.Save();
            Log(c, HallName[i] + (sweep ? " mop-up" : " passed") + (seal ? " +seal" : "") + (progress ? " progress " + HsState.Progress[i - 1] + "%" : ""));
            if (seal) HalloweenSenluo2023Ui.Lua(c, "DlgSenluoHalloweenGameUi_GetMarkEffect|" + i);
        }

        /// <summary>CastleBeat2023_SweepCastle i: the hall was passed before -> Macabre Flame -> the pass rewards, no fight,
        /// no progress (luaUiRes 1435-1 msgtip4: "mop-up won`t increase the progress").</summary>
        public static void SweepHall(Client.GameClient c, int i)
        {
            if (i < 1 || i > 4) return;
            var p = HsState.Of(c);
            if (!HsKit.Running(Festival)) { HsKit.Msg(c, "The Halloween event is over."); return; }
            if (c.Player.Level < HsConfig.Get().MinLevel) { HsKit.Msg(c, Lv(c)); return; }
            if (!p.ZonePassed) { HsKit.Msg(c, "Pass the selected route to enter the Macabre Palace."); return; }
            if ((p.HallPassed & (1 << i)) == 0) { HsKit.Msg(c, "Failed to mop up since you haven`t passed it."); return; }
            var cfg = HsConfig.Get();
            if (!HsKit.HasSpace(c, 3)) { HsKit.Msg(c, "Your inventory is full. Please prepare at least 3 space(s) in your inventory first."); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                if (!HsKit.Take(c, Flame, (uint)cfg.HallFlameCost, rec.GetStream())) { HsKit.Msg(c, "You need " + cfg.HallFlameCost + " Macabre Flame(s) to mop up the " + HallName[i] + "."); return; }
                Give(c, p, new HsGrant(EliteBox, 1, true), rec.GetStream());
            }
            HallPassed(c, i, true);
        }

        // ---------------------------------------------------------------- Mighty Lord
        public static bool LordFree(HsPlayer p) { return (p.Seals & 0x1E) == 0x1E; }
        public static int SealCount(HsPlayer p) { int n = 0; for (int i = 1; i <= 4; i++) if ((p.Seals & (1 << i)) != 0) n++; return n; }

        /// <summary>The reward box of the Mighty Lord by danger level (derived: L1-3 Iron, L4-6 Silver, L7-9 Gold).</summary>
        public static uint LordBox(int danger) { return danger >= 7 ? GoldBox : danger >= 4 ? SilverBox : IronBox; }

        public static void EnterLord(Client.GameClient c)
        {
            var p = HsState.Of(c);
            string why = CanEnter(c);
            if (why == null && !InCity(c)) why = "Cannot enter the Macabre Realm from the current location.";
            if (why != null) { HsKit.Msg(c, why); return; }
            if (!LordFree(p)) { HsKit.Msg(c, "You haven`t got all seals. Please go to the palace first."); return; }
            var cfg = HsConfig.Get();
            if (!HsKit.HasSpace(c, 3)) { HsKit.Msg(c, "Your inventory is full. Please prepare at least 3 space(s) in your inventory first."); return; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (!HsKit.Take(c, Card, (uint)cfg.LordCardCost, stream)) { HsKit.Msg(c, "You need " + cfg.LordCardCost + " Destiny Card(s) to challenge Mighty Lord."); return; }
                var r = HsKit.Open(c, Festival, "lord", LordMap, LordEntry[0], LordEntry[1], cfg.LordMinutes, ExitMap, ExitX, ExitY);
                if (r == null) { HsKit.Give(c, new HsGrant(Card, cfg.LordCardCost, true), stream); HsKit.Msg(c, "Mighty Lord isn`t open on this server yet."); return; }
                r.OnKill = LordKill;
                HsKit.Spawn(r, MightyLord, LordAt[0], LordAt[1], 2, "lord");
                Log(c, "enter Mighty Lord (-" + cfg.LordCardCost + " card)");
                HsKit.Center(c, "Mighty Lord pursues the ultimate power in the world and has unparalleled strength. Defeat him!");
            }
        }

        static void LordKill(HsCopy r, HsMob m, Client.GameClient c)
        {
            if (c == null || m.Tag != "lord" || r.Get("done") != 0) return;
            r.Set("done", 1);
            LordBeat(c);
            HsKit.AddNpc(r, ExitCrack, BloodCrackMesh, "MysteriousCrack", LordAt[0], LordAt[1]);
        }

        static void LordBeat(Client.GameClient c)
        {
            var p = HsState.Of(c);
            int before, after;
            lock (HsState.Lock)
            {
                before = HsState.Danger(p.LordCount);
                p.LordCount++;
                p.LordBeaten = true;
                after = HsState.Danger(p.LordCount);
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                string got = Give(c, p, new HsGrant(LordBox(before), 1, true), rec.GetStream());
                HsKit.Msg(c, "You received " + got + ".");
            }
            HsState.Save();
            Log(c, "Mighty Lord defeated (count " + p.LordCount + ", danger " + after + ")");
            if (after > before) HalloweenSenluo2023Ui.Lua(c, "DlgSenluoHalloweenGameUi_WipeBossUplevel|");
        }

        /// <summary>Halloween2023Lord_Sweep: Mighty Lord beaten before -> Destiny Card -> the same reward without the fight.</summary>
        public static void SweepLord(Client.GameClient c)
        {
            var p = HsState.Of(c);
            if (!HsKit.Running(Festival)) { HsKit.Msg(c, "The Halloween event is over."); return; }
            if (!LordFree(p)) { HsKit.Msg(c, "You haven`t got all seals. Please go to the palace first."); return; }
            if (!p.LordBeaten) { HsKit.Msg(c, "Failed to mop up since you haven`t passed it."); return; }
            var cfg = HsConfig.Get();
            if (!HsKit.HasSpace(c, 2)) { HsKit.Msg(c, "Your inventory is full. Please prepare at least 2 space(s) in your inventory first."); return; }
            using (var rec = new ServerSockets.RecycledPacket())
                if (!HsKit.Take(c, Card, (uint)cfg.LordCardCost, rec.GetStream())) { HsKit.Msg(c, "You need " + cfg.LordCardCost + " Destiny Card(s) to challenge Mighty Lord."); return; }
            LordBeat(c);
        }

        // ---------------------------------------------------------------- server-wide hall reward
        public static bool CanClaimHall(HsPlayer p, int i) { return HsState.Progress[i - 1] >= 100 && p.Points >= HsConfig.Get().RewardPoints && (p.Claimed & (1 << i)) == 0; }

        /// <summary>CastleBeat2023_StrategyReward i: progress 100% and 5000 Macabre Points (official msgtip4) -> once.</summary>
        public static void ClaimHall(Client.GameClient c, int i)
        {
            if (i < 1 || i > 4) return;
            var p = HsState.Of(c);
            if (!HsKit.Running(Festival)) { HsKit.Msg(c, "The Halloween event is over."); return; }
            if (!HsKit.HasSpace(c, 1)) { HsKit.Msg(c, "Your inventory is full. Please make some room, first."); return; }
            lock (HsState.Lock)
            {
                if (!CanClaimHall(p, i)) { HsKit.Msg(c, "When the passing progress reaches 100%, heroes with " + HsConfig.Get().RewardPoints + " Macabre Points can claim rewards."); return; }
                p.Claimed |= 1 << i;// recorded BEFORE the grant
            }
            HsState.Save();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                string got = Give(c, p, new HsGrant(HallReward[i], 1, true), rec.GetStream());
                HsKit.Msg(c, "You received " + got + ".");
                Log(c, HallName[i] + " server-wide reward " + got);
            }
        }

        // ---------------------------------------------------------------- ranking
        public static readonly uint[] RankBox = { 3345623, 3345624, 3345624, 3345625, 3345625, 3345625, 3345626, 3345626, 3345626, 3345626 };
        public static string RankText(Client.GameClient c)
        {
            var top = HsState.Top(10);
            var sb = new StringBuilder("Macabre Realm Ranking (Macabre Points): ");
            if (top.Count == 0) sb.Append("nobody yet. ");
            for (int i = 0; i < top.Count; i++) sb.Append("No." + (i + 1) + " " + top[i].Name + " " + top[i].Points + (i + 1 < top.Count ? ", " : ". "));
            int me = HsState.RankOf(c.Player.UID);
            var p = HsState.Of(c);
            sb.Append("Your points: " + p.Points + (me > 0 ? " (No." + me + ")." : "."));
            return sb.ToString();
        }

        public static bool RankClaimOpen()
        {
            var now = HsKit.Now;
            return now.Month == 11 && now.Day >= HsConfig.Get().RankClaimDay || now.Month == 12;
        }

        static void ClaimRank(Client.GameClient c, ServerSockets.Packet stream)
        {
            var p = HsState.Of(c);
            if (!RankClaimOpen()) { Box(c, stream, "The ranking rewards can be claimed from Nov. " + HsConfig.Get().RankClaimDay + ", the last day of the Macabre Realm."); return; }
            int rank = HsState.RankOf(c.Player.UID);
            if (rank < 1 || rank > 10) { Box(c, stream, "Only the top 10 heroes on the Macabre Realm Ranking can claim a reward."); return; }
            if (!HsKit.HasSpace(c, 1)) { Box(c, stream, "Your inventory is full. Please make some room, first."); return; }
            lock (HsState.Lock)
            {
                if (p.RankClaimed) { Box(c, stream, "You have claimed your ranking reward."); return; }
                p.RankClaimed = true;
            }
            HsState.Save();
            string got = Give(c, p, new HsGrant(RankBox[rank - 1], 1, true), stream);
            Log(c, "ranking No." + rank + " " + got);
            Box(c, stream, "You were ranked No." + rank + " on the Macabre Realm Ranking and obtained the " + got + ".");
        }

        // ---------------------------------------------------------------- NPC router
        sealed class Ask { public uint Npc; public byte[] Opts; public DateTime At; }
        static readonly ConcurrentDictionary<uint, Ask> _ask = new ConcurrentDictionary<uint, Ask>();
        static void Expect(Client.GameClient c, uint npc, params byte[] opts) { _ask[c.Player.UID] = new Ask { Npc = npc, Opts = opts, At = HsKit.Now }; }
        static bool Expected(Client.GameClient c, uint npc, byte opt)
        {
            Ask a;
            if (!_ask.TryGetValue(c.Player.UID, out a) || a.Npc != npc || (HsKit.Now - a.At).TotalMinutes > 5) return false;
            if (!a.Opts.Contains(opt)) return false;
            _ask.TryRemove(c.Player.UID, out a);
            return true;
        }
        static void Box(Client.GameClient c, ServerSockets.Packet stream, string text)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText(text).AddOption("OK.", 255).AddAvatar(10).FinalizeDialog();
        }

        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            if (npcid == Dracula) return DraculaNpc(c, stream, option);
            var r = HsKit.Here(c);
            if (r == null || r.Festival != Festival) return false;
            if (!HsKit.HasNpc(r, npcid)) return false;
            if (option == 255) return true;
            if (HsKit.Throttled(c.Player.UID, "npc", 300)) return true;
            var p = HsState.Of(c);
            if (r.Kind == "sideroad")
            {
                if (npcid == Signpost)
                {
                    if (option == 0) { SignpostMenu(c, stream); return true; }
                    if (!Expected(c, npcid, option)) return true;
                    if (option == 9) { HsKit.Leave(c); return true; }
                    StartPassage(c, r, p, option);
                    return true;
                }
                if (npcid > SideCrack && npcid <= SideCrack + 3) { SideCrackClick(c, r, p, (int)(npcid - SideCrack), stream); return true; }
                return true;
            }
            if (r.Kind == "blood")
            {
                if (npcid == BloodCrack)
                {
                    if (option == 0) { BloodCrackMenu(c, stream); return true; }
                    if (!Expected(c, npcid, option)) return true;
                    if (option == 1) { HsKit.MoveInside(c, r, Altar[0][0], (ushort)(Altar[0][1] + 5)); return true; }
                    if (option == 3) { HsKit.Leave(c); return true; }
                    bool key; lock (HsState.Lock) key = p.Key;
                    if (!key) { Box(c, stream, "You haven`t got the Secret Key, so you cannot open the crack. There are clues hidden in those corpses. Go search!"); return true; }
                    PassZone(c, r, p, HsConfig.Get().BloodPassFlames, stream, false);
                    return true;
                }
                if (npcid == KeyNpc)
                {
                    HsKit.RemoveNpc(r, KeyNpc);
                    lock (HsState.Lock) p.Key = true;
                    HsState.Save();
                    HsKit.Center(c, "You have found the Secret Key. Pick it up and head through the crack to Macabre Realm.");
                    return true;
                }
                if (npcid >= Ghost && npcid <= Ghost + 3)
                {
                    int g = (int)(npcid - Ghost);
                    if (option == 0)
                    {
                        var d = new Game.MsgNpc.Dialog(c, stream);
                        d.AddText(GhostTalk[g][0]).AddOption(GhostTalk[g][1], 1).AddAvatar(10).FinalizeDialog();
                        Expect(c, npcid, 1);
                        return true;
                    }
                    if (!Expected(c, npcid, option)) return true;
                    HsKit.RemoveNpc(r, npcid);
                    HsKit.Msg(c, "Before the ghost disappeared, he left his reward to you.");
                    Give(c, p, new HsGrant(SecretBag, 1, true), stream);
                    HsState.Save();
                    return true;
                }
                if (npcid >= Corpse && npcid < Corpse + 100) { SearchCorpse(c, r, p, npcid, stream); return true; }
                return true;
            }
            // halls / Mighty Lord: the exit crack
            if (npcid == ExitCrack)
            {
                if (option == 0)
                {
                    var d = new Game.MsgNpc.Dialog(c, stream);
                    d.AddText("Mysterious Crack:\nThe way back is open.\n").AddOption("Return to Twin City.", 1).AddOption("Stay a while.", 255).AddAvatar(10).FinalizeDialog();
                    Expect(c, npcid, 1);
                    return true;
                }
                if (Expected(c, npcid, option)) { HsKit.Leave(c); HalloweenSenluo2023Ui.SendGame(c); }
                return true;
            }
            return true;
        }

        // ---------------------------------------------------------------- Count Dracula (Twin City)
        static bool DraculaNpc(Client.GameClient c, ServerSockets.Packet stream, byte option)
        {
            if (!HsKit.Running(Festival)) return false;
            if (option == 0)
            {
                var d = new Game.MsgNpc.Dialog(c, stream);
                if (c.Player.Level < HsConfig.Get().MinLevel)
                {
                    d.AddText("Count Dracula:\nOn Halloween night, the Macabre Realm is shrouded in black mist.\nIt is said that escaping from the realm will lead to supreme treasures.\nWill you be the victor who successfully escapes from the Macabre Realm?\n");
                    d.AddText("Duration: 10.24-11.06\nRequirement: Level 80\n").AddOption("Level up first.", 255).AddAvatar(10).FinalizeDialog();
                    return true;
                }
                d.AddText("Count Dracula:\nMighty Lord is in the depths of the realm. Only by successfully passing the four halls and lifting the seals can you have a decisive battle with Mighty Lord!\n    ----------------Key Items--------------\n");
                d.AddText("    Macabre Flame         Obtained by passing Blood Zone\n    Destiny Card         Obtained by defeating Counts in the palace\n    Macabre Coin           Exploring Blood Zone and the palace\n     Macabre Points         Obtaining Macabre Coins\n");
                d.AddText("    ------------------------------------\n    The danger level of the Macabre Realm will increase as the your points increase. You can claim stage rewards when reaching the specified level.\n");
                d.AddOption("Enter Macabre Realm.", 201).AddOption("Macabre Treasury (Exchange Store).", 202).AddOption("View Macabre Realm Ranking.", 203);
                if (RankClaimOpen()) d.AddOption("Claim my ranking reward.", 204);
                d.AddOption("Come later.", 255).AddAvatar(10).FinalizeDialog();
                Expect(c, Dracula, 201, 202, 203, 204);
                return true;
            }
            if (option < 201 || option > 204) return false;// the exchange list of the Treasury (FestivalManager)
            if (!Expected(c, Dracula, option)) return true;
            switch (option)
            {
                case 201: HalloweenSenluo2023Ui.OpenMain(c); return true;
                case 202: Festivals.FestivalManager.ShowMenu(c, stream, Dracula); return true;
                case 203: Box(c, stream, RankText(c)); return true;
                case 204: ClaimRank(c, stream); return true;
            }
            return true;
        }

        /// <summary>Harness: forget the dialog expectations.</summary>
        public static void ResetForTests() { _ask.Clear(); }
    }
}
