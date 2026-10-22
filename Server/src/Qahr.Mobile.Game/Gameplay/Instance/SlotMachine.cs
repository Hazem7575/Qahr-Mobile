using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Role.Instance
{
   public class SlotMachine
    {
       public enum SlotMachineItems : byte
       {
           Stancher = 0,
           Meteor = 1,
           Sword = 2,
           TwoSwords = 3,
           SwordAndShield = 4,
           ExpBall = 5,
           DragonBall = 6
       }
        public static readonly int[] Rates = new int[8] 
        {
            80, //Stancher
            540, //Meteor
            120, //Sword
            180, //TwoSwords
            210, //SwordAndShield
            180, //ExpBall
            600, //DragonBall
            200, //3s line
        };

        public SlotMachineItems[] Wheels = new SlotMachineItems[3];

        public uint NPCID;
        public uint BetAmount;
        public bool Cps;
        public SlotMachine(uint npcid, uint betamount, bool cps = false)
        {
            NPCID = npcid;
            BetAmount = betamount;
            Cps = cps;
        }

        int GetAmount(SlotMachineItems Item)
        {
            int count = 0;
            foreach (SlotMachineItems item in Wheels)
                if (item == Item)
                    count++;
            return count;
        }

        private int GetSLCount()
        {
            return GetAmount(SlotMachineItems.Sword) + GetAmount(SlotMachineItems.SwordAndShield) + GetAmount(SlotMachineItems.TwoSwords) + GetAmount(SlotMachineItems.DragonBall);
        }
        private bool IsSL(SlotMachineItems item)
        {
            return item == SlotMachineItems.DragonBall || item == SlotMachineItems.Sword || item == SlotMachineItems.SwordAndShield || item == SlotMachineItems.TwoSwords;
        }

        public unsafe uint GetRewardAmount(Client.GameClient client,ServerSockets.Packet stream)
        {
            return Qahr.Mobile.Phone.MobileSlotOdds.Reward(this, client, stream);// QahrMobile: the payout table the client shows, MobileSlotOdds.cs
            uint win = 0;
            if (GetAmount(SlotMachineItems.DragonBall) == 3)
            {

                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won the jackpot from the 1-Arm Bandit!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
              
                client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, "accession5");
                if (Cps) return BetAmount * 3000;
                else return BetAmount * 1000;
            }
            if (GetAmount(SlotMachineItems.ExpBall) == 3
                  || (GetAmount(SlotMachineItems.ExpBall) == 2 && GetAmount(SlotMachineItems.DragonBall) == 1)
                || (GetAmount(SlotMachineItems.ExpBall) == 1 && GetAmount(SlotMachineItems.DragonBall) == 2))
                win = BetAmount * 60;

            else if (GetAmount(SlotMachineItems.SwordAndShield) == 3
                  || (GetAmount(SlotMachineItems.SwordAndShield) == 2 && GetAmount(SlotMachineItems.DragonBall) == 1)
                || (GetAmount(SlotMachineItems.SwordAndShield) == 1 && GetAmount(SlotMachineItems.DragonBall) == 2))
                win = BetAmount * 40;

            else if (GetAmount(SlotMachineItems.TwoSwords) == 3
                     || (GetAmount(SlotMachineItems.TwoSwords) == 2 && GetAmount(SlotMachineItems.DragonBall) == 1)
                || (GetAmount(SlotMachineItems.TwoSwords) == 1 && GetAmount(SlotMachineItems.DragonBall) == 2))
                win = BetAmount * 20;
            else if (GetAmount(SlotMachineItems.Meteor) == 3 
                || (GetAmount(SlotMachineItems.Meteor) == 1 && GetAmount(SlotMachineItems.DragonBall) == 2) 
                || (GetAmount(SlotMachineItems.Meteor) == 2 && GetAmount(SlotMachineItems.DragonBall) == 1))
                win = BetAmount * 10;
            else if (GetAmount(SlotMachineItems.Sword) == 3 || (GetAmount(SlotMachineItems.Sword) == 1 && GetAmount(SlotMachineItems.DragonBall) == 2))
                win = BetAmount * 10;
            else if (GetAmount(SlotMachineItems.Meteor) == 1 && GetAmount(SlotMachineItems.DragonBall) == 1)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.Meteor) == 2)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.SwordAndShield) == 2 && GetAmount(SlotMachineItems.Sword) == 1
                || GetAmount(SlotMachineItems.SwordAndShield) == 2 && GetAmount(SlotMachineItems.TwoSwords) == 1)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.TwoSwords) == 2 && GetAmount(SlotMachineItems.Sword) == 1
                || GetAmount(SlotMachineItems.TwoSwords) == 2 && GetAmount(SlotMachineItems.SwordAndShield) == 1)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.Sword) == 2 && GetAmount(SlotMachineItems.SwordAndShield) == 1
                || GetAmount(SlotMachineItems.Sword) == 2 && GetAmount(SlotMachineItems.TwoSwords) == 1)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.Sword) == 1 && GetAmount(SlotMachineItems.SwordAndShield) == 1
                     && GetAmount(SlotMachineItems.TwoSwords) == 1)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.DragonBall) == 2)
                win = BetAmount * 5;
            else if (GetAmount(SlotMachineItems.Meteor) == 1)
                win = BetAmount * 2;
            else if (GetAmount(SlotMachineItems.DragonBall) == 1)
                win = BetAmount * 2;
           
            if (Cps)
            {
                if (GetAmount(SlotMachineItems.DragonBall) == 1)
                    win *= 3;
                else if (GetAmount(SlotMachineItems.DragonBall) == 2)
                    win *= 9;
            }
            if (win == 0)
            {

                if (Cps)
                    client.GainExpBall(BetAmount * 3, true);
            }
            return win;
        }
        public bool Rate(int value, int discriminant)
        {
            return value > Pool.GetRandom.Next() % discriminant;
        }
        public void GetwheelPick(out uint wheelPick)
        {
            wheelPick = 0;
            if (GetAmount(SlotMachineItems.DragonBall) == 1 && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1)
                || (GetAmount(SlotMachineItems.DragonBall) == 2 && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1)) && MyMath.Success(1) && MyMath.Success(1))
            {
                wheelPick = (int)SlotMachineItems.DragonBall;
            }
            else
            {
                if (MyMath.Success(1))
                    wheelPick = (int)SlotMachineItems.DragonBall;
            }

            if (GetAmount(SlotMachineItems.Meteor) == 1 && MyMath.Success(5) && MyMath.Success(5)
               || (GetAmount(SlotMachineItems.Meteor) == 2 && MyMath.Success(5) && MyMath.Success(5) && MyMath.Success(5)))
            {
                wheelPick = (int)SlotMachineItems.Meteor;
            }
            else
            {
                if (MyMath.Success(5) && MyMath.Success(5))
                    wheelPick = (int)SlotMachineItems.Meteor;
            }
            if (GetAmount(SlotMachineItems.ExpBall) == 1 && MyMath.Success(1) && MyMath.Success(1)
               || (GetAmount(SlotMachineItems.ExpBall) == 2 && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1)))
            {
                wheelPick = (int)SlotMachineItems.ExpBall;
            }
            else
            {
                if (MyMath.Success(2) && MyMath.Success(1))
                    wheelPick = (int)SlotMachineItems.ExpBall;
            }
            if (GetAmount(SlotMachineItems.Sword) == 1 && MyMath.Success(1) && MyMath.Success(1)
                     || (GetAmount(SlotMachineItems.Sword) == 2 && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1)))
            {
                wheelPick = (int)SlotMachineItems.Sword;
            }
            else
            {
                if (MyMath.Success(2))
                    wheelPick = (int)SlotMachineItems.Sword;
            }
            if (GetAmount(SlotMachineItems.SwordAndShield) == 1 && MyMath.Success(1) && MyMath.Success(1)
                   || (GetAmount(SlotMachineItems.SwordAndShield) == 2 && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1)))
            {
                wheelPick = (int)SlotMachineItems.SwordAndShield;
            }
            else
            {
                if (MyMath.Success(2))
                    wheelPick = (int)SlotMachineItems.SwordAndShield;
            }
            if (GetAmount(SlotMachineItems.TwoSwords) == 1 && MyMath.Success(1) && MyMath.Success(1)
               || (GetAmount(SlotMachineItems.TwoSwords) == 2 && MyMath.Success(1) && MyMath.Success(1) && MyMath.Success(1)))
            {
                wheelPick = (int)SlotMachineItems.TwoSwords;
            }
            else
            {
                if (MyMath.Success(2))
                    wheelPick = (int)SlotMachineItems.TwoSwords;
            }
            if (wheelPick == 0)
                wheelPick = (int)SlotMachineItems.Stancher;
        }
        public void SpinTheWheels()
        {
            Qahr.Mobile.Phone.MobileSlotOdds.Spin(this);// QahrMobile: GetwheelPick let each check overwrite the pick (reels mostly 0), MobileSlotOdds.cs
        }
        public unsafe void SendWheelsToClient(Client.GameClient client,ServerSockets.Packet packet)
        {
            client.Send(packet.MachineResponseCreate(MsgMachine.SlotMachineSubType.StartSpin, (byte)Wheels[0], (byte)Wheels[1], (byte)Wheels[2], NPCID));
        }
    }
}
