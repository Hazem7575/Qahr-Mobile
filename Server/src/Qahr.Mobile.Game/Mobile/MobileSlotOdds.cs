namespace Qahr.Mobile.Phone
{
    /// <summary>
    /// Slot machine reels and payout.
    /// Reels: SlotMachine.GetwheelPick ran one check per symbol and let every later check overwrite the earlier pick, and
    /// most checks failed, so nearly every reel came out 0 (drawn by the mobile as a random blank unit). Here each reel is
    /// drawn on its own from a weight table.
    /// Payout: the mobile works out the "Win" it shows by itself (CSlotMachineMgr::SSetResultInfo on the stop packet):
    /// bet x odds[type] x dragon pearl odds, with the type from CSlotMachineMgr::TestResultType (first match, 6 = dragon
    /// pearl, a wild card in types 1-6):
    ///   0 three 6 | 1 three 5 | 2 three 4 | 3 three 3 | 4 three 2 | 5 three 1 | 6 every reel 2..4 | 7 two of {1,6} | 8 one of {1,6}
    /// odds (slotmachine.ini Data via GetOddsLev 0,1,2,3,4,4,5,5,6): 1000 (CP machines 3000), 60, 40, 20, 10, 10, 5, 5, 2;
    /// dragon pearl odds (DragonPearlOdds1/2) x3 / x9 for one / two pearls on CP machines, x1 on silver.
    /// SlotMachine.GetRewardAmount had other combinations (e.g. sword+sword+pearl paid x2 instead of x10), so the
    /// player could be paid something else than the window showed; Reward() is the mobile's table.
    /// Return to player (all 343 outcomes summed): silver 0.910 (a win on 32% of spins), CPs 0.887 (24%).
    /// </summary>
    public static class MobileSlotOdds
    {
        //                                        blank meteor sword twoSwords sword&shield expBall dragonPearl
        static readonly int[] SilverWeights = { 57, 9, 11, 11, 7, 4, 2 };
        static readonly int[] CpsWeights = { 60, 7, 11, 11, 6, 4, 1 };
        static readonly uint[] Odds = { 1000, 60, 40, 20, 10, 10, 5, 5, 2, 0 };

        public static void Spin(Role.Instance.SlotMachine machine)
        {
            var w = machine.Cps ? CpsWeights : SilverWeights;
            int total = 0;
            foreach (int x in w) total += x;
            for (int i = 0; i < machine.Wheels.Length; i++)
            {
                int r = Pool.GetRandom.Next(total), s = 0;
                while (r >= w[s]) { r -= w[s]; s++; }
                machine.Wheels[i] = (Role.Instance.SlotMachine.SlotMachineItems)s;
            }
        }

        static int ResultType(int a, int b, int c)
        {
            if (a == 6 && b == 6 && c == 6) return 0;
            for (int t = 1; t <= 5; t++)
            {
                int s = 6 - t;// type 1 = 5 (exp ball) ... type 5 = 1 (meteor)
                if ((a == s || a == 6) && (b == s || b == 6) && (c == s || c == 6)) return t;
            }
            if (Weapon(a) && Weapon(b) && Weapon(c)) return 6;
            int n = (a == 1 || a == 6 ? 1 : 0) + (b == 1 || b == 6 ? 1 : 0) + (c == 1 || c == 6 ? 1 : 0);
            if (n >= 2) return 7;
            if (n == 1) return 8;
            return 9;
        }

        static bool Weapon(int x) { return x == 6 || (x >= 2 && x <= 4); }

        /// <summary>SlotMachine.GetRewardAmount hook: the payout the mobile window shows (jackpot message and the CP
        /// machines' exp ball on a loss as before).</summary>
        public static uint Reward(Role.Instance.SlotMachine m, Client.GameClient client, ServerSockets.Packet stream)
        {
            int a = (int)m.Wheels[0], b = (int)m.Wheels[1], c = (int)m.Wheels[2];
            int type = ResultType(a, b, c);
            uint odds = type == 0 && m.Cps ? 3000 : Odds[type];
            int pearls = (a == 6 ? 1 : 0) + (b == 6 ? 1 : 0) + (c == 6 ? 1 : 0);
            uint pearlOdds = !m.Cps ? 1u : pearls == 1 ? 3u : pearls == 2 ? 9u : 1u;
            uint win = m.BetAmount * odds * pearlOdds;
            if (type == 0)
            {
                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won the jackpot from the 1-Arm Bandit!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, "accession5");
            }
            if (win == 0 && m.Cps)
                client.GainExpBall(m.BetAmount * 3, true);
            return win;
        }
    }
}
