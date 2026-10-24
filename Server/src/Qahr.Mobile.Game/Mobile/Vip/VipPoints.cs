using System;
using System.Collections.Generic;
using System.Linq;
using Qahr.Mobile.EventData;

namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// QahrMobile: VIP points. Points raise the player's permanent VIP level; every level has a reward claimed once.
    /// Data: Data\mobile_vip_levels.txt ("level vip points items..." and "card item points priceCps").
    /// State: EventStore event "mvippts" (counter "pts", claims "lv&lt;n&gt;").
    /// The one place that decides the VIP level a player keeps without a timed VIP: <see cref="FloorLevel"/>
    /// (used by PoolProcesses when a timed VIP ends, and at login).
    /// Points come from VIP cards (bag items) for now; <see cref="AddPoints"/> is the single entry for any other source.
    /// UI: the VIP menu below (main-screen VIP icon, "@vip": levels and privileges, cards, services) and the level rewards
    /// in the client's Welfare window (<see cref="VipWelfareUi"/>).
    /// </summary>
    public static class VipPoints
    {
        const string Ev = "mvippts", Pts = "pts";
        public const uint ShopNpc = 9900212;// no NPC has this id; Procesor routes the shop dialog answers here
        static readonly DateTime Permanent = new DateTime(2099, 12, 31);

        public sealed class Level
        {
            public int Vip;
            public long Points;
            public List<RewardItem> Items = new List<RewardItem>();
        }

        public sealed class Card
        {
            public uint Item;
            public int Points;
            public uint Price;
        }

        sealed class Config
        {
            public readonly List<Level> Levels = new List<Level>();
            public readonly List<Card> Cards = new List<Card>();
            public readonly SortedDictionary<int, string> Perks = new SortedDictionary<int, string>();
            public uint Window;
            public readonly List<uint> Quiet = new List<uint>();

            public static Config Parse(string[] lines)
            {
                var cfg = new Config();
                foreach (var line in DataFile.Clean(lines))
                {
                    var p = DataFile.Words(line);
                    if (p.Length >= 2 && p[0] == "window")
                    {
                        cfg.Window = uint.Parse(p[1]);
                        cfg.Quiet.AddRange(p.Skip(2).Select(uint.Parse));
                    }
                    else if (p.Length >= 3 && p[0] == "perk")
                        cfg.Perks[int.Parse(p[1])] = string.Join(" ", p, 2, p.Length - 2);
                    else if (p.Length >= 3 && p[0] == "level")
                        cfg.Levels.Add(new Level { Vip = int.Parse(p[1]), Points = long.Parse(p[2]), Items = Rewards.Parse(p, 3) });
                    else if (p.Length >= 4 && p[0] == "card")
                        cfg.Cards.Add(new Card { Item = uint.Parse(p[1]), Points = int.Parse(p[2]), Price = uint.Parse(p[3]) });
                }
                cfg.Levels.Sort((a, b) => a.Points.CompareTo(b.Points));
                return cfg;
            }
        }

        static readonly DataFile<Config> Settings = new DataFile<Config>("mobile_vip_levels.txt", "VipPoints", Config.Parse);

        public static List<Level> Levels { get { return Settings.Value.Levels; } }
        public static List<Card> Cards { get { return Settings.Value.Cards; } }
        /// <summary>What each VIP level gives ("perk" lines; text only, the effects live in the game code).</summary>
        public static SortedDictionary<int, string> Perks { get { return Settings.Value.Perks; } }
        /// <summary>The client activity whose Welfare "Credit Rebate" rows show the VIP levels (VipWelfareUi), 0 = none.</summary>
        public static uint WindowActivity { get { return Settings.Value.Window; } }
        /// <summary>Client activities marked "not running" so the client's activity query fits in one packet.</summary>
        public static List<uint> QuietActivities { get { return Settings.Value.Quiet; } }

        public static long Points(uint uid)
        {
            return EventStore.GetLong(uid, Ev, Pts);
        }

        /// <summary>The VIP level the points give (1 = the free level).</summary>
        public static int LevelFor(long points)
        {
            int lv = 1;
            foreach (var l in Levels)
                if (points >= l.Points) lv = Math.Max(lv, l.Vip);
            return lv;
        }

        /// <summary>The VIP level a player keeps when no timed VIP is active.</summary>
        public static int FloorLevel(Client.GameClient c)
        {
            return c == null || c.Player == null ? 1 : LevelFor(Points(c.Player.UID));
        }

        public static bool IsClaimed(uint uid, int vip)
        {
            return EventStore.IsClaimed(uid, Ev, "lv" + vip);
        }

        /// <summary>Adds points (any source), then raises the VIP level and refreshes an open window.</summary>
        public static long AddPoints(Client.GameClient c, int points, string source)
        {
            if (c == null || c.Player == null || points <= 0) return 0;
            int before = FloorLevel(c);
            long now = EventStore.AddLong(c.Player.UID, Ev, Pts, points, 0, long.MaxValue, true);
            MyConsole.WriteLine("[VipPoints] " + c.Player.Name + " +" + points + " (" + source + ") = " + now, ConsoleColor.Cyan);
            int after = LevelFor(now);
            ApplyLevel(c);
            PlayerMsg.Tell(c, "You received " + points + " VIP points (" + now + " in total).");
            if (after > before)
                PlayerMsg.Tell(c, "Congratulations! You reached VIP " + after + ". Claim the reward in Welfare > VIP rewards.");
            VipWelfareUi.Refresh(c);
            return now;
        }

        /// <summary>Makes sure the player has at least the points level (permanent). A higher timed VIP is kept.</summary>
        public static void ApplyLevel(Client.GameClient c)
        {
            if (c == null || c.Player == null) return;
            var p = c.Player;
            int floor = FloorLevel(c);
            bool timedActive = DateTime.Now < p.ExpireVip && p.VipLevel > floor;
            if (floor <= 1 || timedActive || (p.VipLevel == floor && p.ExpireVip >= Permanent)) return;
            p.VipLevel = (byte)floor;
            if (p.ExpireVip < Permanent) p.ExpireVip = Permanent;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                p.SendUpdate(stream, p.VipLevel, Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                p.UpdateVip(stream);
            }
        }

        /// <summary>Login: give back the points level (a timed VIP that ended while offline fell to the free level).</summary>
        public static void OnLogin(Client.GameClient c)
        {
            try
            {
                ApplyLevel(c);
                VipWelfareUi.OnLogin(c);
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        // ------------------------------------------------------------------ rewards
        /// <summary>Claims the reward of one VIP level. false = told the player why not.</summary>
        public static bool Claim(Client.GameClient c, Level lv)
        {
            uint uid = c.Player.UID;
            if (Points(uid) < lv.Points) { PlayerMsg.Tell(c, "You need " + lv.Points + " VIP points for the VIP " + lv.Vip + " reward."); return false; }
            if (IsClaimed(uid, lv.Vip)) return false;
            if (!Rewards.CanGive(c, lv.Items, "VipPoints", "Nothing was claimed.")) return false;
            if (!EventStore.TryClaimOnce(uid, Ev, "lv" + lv.Vip)) { PlayerMsg.Tell(c, "The reward could not be saved. Please try again later."); return false; }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var given = Rewards.Give(c, rec.GetStream(), lv.Items, "VipPoints");
                PlayerMsg.Tell(c, "VIP " + lv.Vip + " reward: " + string.Join(", ", given) + ".");
            }
            return true;
        }

        // ------------------------------------------------------------------ VIP cards (bag items)
        /// <summary>MobileItemUse: using a VIP card adds its points. True = handled.</summary>
        public static bool OnUseItem(Client.GameClient c, Game.MsgServer.MsgGameItem item)
        {
            var card = Cards.FirstOrDefault(x => x.Item == item.ITEM_ID);
            if (card == null) return false;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                if (!c.Inventory.Remove(item.ITEM_ID, 1, rec.GetStream()))
                    return true;
            }
            AddPoints(c, card.Points, "card " + item.ITEM_ID);
            return true;
        }

        // ------------------------------------------------------------------ the VIP menu (an NPC-style dialog)
        // Options: 1..9 = buy card n (cards page), 100 services, 101 rewards window, 102 cards page, 110 levels,
        // 110 + n = VIP n, 0 = back to the menu, 255 = close.
        const byte ServicesOption = 100, RewardsOption = 101, CardsOption = 102, LevelsOption = 110;

        /// <summary>The main-screen VIP icon (newmenu.dat QahrHud icon "vip": Public_CallServerFunc(19992,"QahrVip_Cmd",...)).</summary>
        public const uint IconMsg = 19992;

        /// <summary>LuaPush.HandleCallback (msg 19992). True = answered.</summary>
        public static bool OnIcon(Client.GameClient c, string func)
        {
            if (func != "QahrVip_Cmd" || c == null || c.Player == null) return false;
            return OpenMenu(c);
        }

        /// <summary>The main-screen VIP icon and "@vip": the VIP menu.</summary>
        public static bool OpenMenu(Client.GameClient c)
        {
            c.ActiveNpc = ShopNpc;
            using (var rec = new ServerSockets.RecycledPacket())
                ShowMenu(c, rec.GetStream());
            return true;
        }

        /// <summary>Procesor (NPC answers): true = this was the VIP menu.</summary>
        public static bool OnNpc(Client.GameClient c, ServerSockets.Packet stream, uint npcid, byte option)
        {
            if (npcid != ShopNpc) return false;
            if (option == 255) return true;
            if (option == 0) ShowMenu(c, stream);
            else if (option == ServicesOption) MobileVipMenu.ChatCommand(c, "vipmenu");
            else if (option == RewardsOption) VipWelfareUi.Open(c);
            else if (option == CardsOption) ShowCards(c, stream);
            else if (option == LevelsOption) ShowLevels(c, stream);
            else if (option > LevelsOption) ShowLevel(c, stream, option - LevelsOption);
            else if (option <= Cards.Count) Buy(c, Cards[option - 1], stream);
            return true;
        }

        static void ShowMenu(Client.GameClient c, ServerSockets.Packet stream)
        {
            long pts = Points(c.Player.UID);
            var next = Levels.FirstOrDefault(l => l.Points > pts);
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("VIP " + c.Player.VipLevel + " - " + pts + " VIP points." + (next != null ? "~" + (next.Points - pts) + " more points to reach VIP " + next.Vip + "." : "~You reached the highest VIP level."));
            d.AddOption("VIP~level~rewards", RewardsOption);
            d.AddOption("VIP~levels~and~privileges", LevelsOption);
            d.AddOption("Buy~VIP~cards~(VIP~points)", CardsOption);
            d.AddOption("VIP~services~(teleport,~repair,~warehouse)", ServicesOption);
            d.AddOption("Close", 255).AddAvatar(7).FinalizeDialog();
        }

        static void ShowCards(Client.GameClient c, ServerSockets.Packet stream)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("VIP cards give VIP points. Use a card from your bag to add its points.~You have " + Points(c.Player.UID) + " VIP points and " + c.Player.ConquerPoints + " CPs.");
            var cards = Cards;
            for (int i = 0; i < cards.Count && i < 9; i++)
                d.AddOption(ItemName(cards[i].Item) + "~(" + cards[i].Points + "~pts)~-~" + cards[i].Price + "~CPs", (byte)(i + 1));
            d.AddOption("Back", 0).AddAvatar(7).FinalizeDialog();
        }

        static long PointsFor(int vip)
        {
            var lv = Levels.FirstOrDefault(x => x.Vip == vip);
            return lv == null ? 0 : lv.Points;
        }

        static void ShowLevels(Client.GameClient c, ServerSockets.Packet stream)
        {
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("Your VIP level: " + c.Player.VipLevel + " - VIP points: " + Points(c.Player.UID) + ".~Every level keeps the privileges of the levels below it. Pick a level to see what it gives.");
            foreach (var vip in Perks.Keys.Where(v => v < 255 - LevelsOption))
            {
                long need = PointsFor(vip);
                string state = c.Player.VipLevel >= vip ? "(yours)" : need > 0 ? "(" + need + "~points)" : "";
                d.AddOption("VIP~" + vip + "~" + state, (byte)(LevelsOption + vip));
            }
            d.AddOption("Back", 0).AddAvatar(7).FinalizeDialog();
        }

        static void ShowLevel(Client.GameClient c, ServerSockets.Packet stream, int vip)
        {
            string perk;
            if (!Perks.TryGetValue(vip, out perk)) { ShowLevels(c, stream); return; }
            long need = PointsFor(vip);
            var lv = Levels.FirstOrDefault(x => x.Vip == vip);
            var d = new Game.MsgNpc.Dialog(c, stream);
            d.AddText("VIP " + vip + (need > 0 ? " - needs " + need + " VIP points (you have " + Points(c.Player.UID) + ")." : " - free for every player.") + "~~" + perk);
            if (lv != null && lv.Items.Count > 0)
                d.AddText("~~Level reward (once): " + string.Join(", ", lv.Items.Select(i => ItemName(i.Id) + " x" + i.Count)) + ".");
            d.AddOption("All~levels", LevelsOption);
            d.AddOption("Back", 0).AddAvatar(7).FinalizeDialog();
        }

        static void Buy(Client.GameClient c, Card card, ServerSockets.Packet stream)
        {
            var items = new List<RewardItem> { new RewardItem { Id = card.Item, Count = 1, Bound = true } };
            if (c.Player.ConquerPoints < card.Price) { PlayerMsg.Tell(c, "You need " + card.Price + " CPs."); return; }
            if (!Rewards.CanGive(c, items, "VipPoints", "Nothing was charged.")) return;
            c.Player.ConquerPoints -= card.Price;
            Rewards.Give(c, stream, items, "VipPoints");
            PlayerMsg.Tell(c, "You bought " + ItemName(card.Item) + " for " + card.Price + " CPs. Use it from your bag to add " + card.Points + " VIP points.");
            MyConsole.WriteLine("[VipPoints] " + c.Player.Name + " bought card " + card.Item + " for " + card.Price + " CPs", ConsoleColor.Cyan);
            ShowCards(c, stream);
        }

        static string ItemName(uint id)
        {
            Database.ItemType.DBItem db;
            return Pool.ItemsBase != null && Pool.ItemsBase.TryGetValue(id, out db) && db.Name != null ? db.Name : id.ToString();
        }
    }
}
