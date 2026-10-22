//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Qahr.Mobile.Game.MsgServer;

//namespace Qahr.Mobile.Game.MsgNpc
//{
//    public class MsgSavageBoneNpc
//    {
//        // ── Item IDs ─────────────────────────────────────────────────────────
//        private const uint ANIMA_P14 = 4200014;
//        private const uint MYTH_SOUL_L6 = 3326003;
//        private const uint COSMOS_HAMMER = 3324226;
//        private const uint ITEM_3307146 = 3307146;

//        private static readonly uint[] COLLECTION_SCRAPS =
//        {
//            3326259, 3326260, 3326261, 3326262, 3326263, 3336146, 3355514, 
//        };

//        // ── NPC Handler ──────────────────────────────────────────────────────
//        [NpcAttribute(NpcID.P7SacredTreasuresEnvoy)]
//        public static void P7SacredTreasuresEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
//        {
//            Dialog data = new Dialog(client, stream);
//            switch (Option)
//            {
//                case 0: // Main Menu
//                    {
//                        data.AddText("Hello! I am the envoy of P7 Sacred Treasures.\n")
//                            .AddText("Would you like to explore the Savage Bones?")
//                            .AddOption("Explore (1 Savage Bone)", 1)
//                            .AddOption("Mass Explore (10 Savage Bones)", 10)
//                            .AddOption("No thanks.", 255).FinalizeDialog();
//                        break;
//                    }

//                case 1: // 1 Savage Bone
//                    {
//                        if (client.Inventory.Contain(3004181, 1))
//                        {
//                            if (client.Inventory.HaveSpace(1))
//                            {
//                                client.Inventory.Remove(3004181, 1, stream);
//                                GiveReward(client, stream, false);
//                            }
//                            else client.CreateBoxDialog("Please make 1 space in your inventory.");
//                        }
//                        else client.CreateBoxDialog("Sorry, you don't have any Savage Bones.");
//                        break;
//                    }

//                case 10: // 10 Savage Bones
//                    {
//                        if (client.Inventory.Contain(3004181, 10))
//                        {
//                            if (client.Inventory.HaveSpace(5))
//                            {
//                                client.Inventory.Remove(3004181, 10, stream);
//                                for (int i = 0; i < 10; i++)
//                                    GiveReward(client, stream, true);
//                            }
//                            else client.CreateBoxDialog("Please make 5 spaces in your inventory.");
//                        }
//                        else client.CreateBoxDialog("Sorry, you don't have 10 Savage Bones.");
//                        break;
//                    }
//            }
//        }

//        // ── Reward Logic ─────────────────────────────────────────────────────
//        private static void GiveReward(Client.GameClient client, ServerSockets.Packet stream, bool mass)
//        {
//            uint roll = (uint)Program.GetRandom.Next(0, 10000);

//            // 50,000,000 CPs ── 10%
//            if (roll < 500)
//            {
//                client.Player.ConquerPoints += 50000000;
//                Server.SendGlobalPacket(new MsgMessage(
//                    string.Format("WOW! [{0}] explored a Savage Bone and won 50,000,000 CPs!", client.Player.Name),
//                    MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
//                return;
//            }

//            //   2,000,000 CPs ── 15%
//            if (roll < 1500)
//            {
//                client.Player.ConquerPoints += 2000000;
//                if (!mass) client.SendSysMesage("You found 2,000,000 CPs! Lucky!");
//                return;
//            }

//            // Anima P14── 10%
//            if (roll < 1800)
//            {
//                client.Inventory.Add(stream, ANIMA_P14, 1);
//                Server.SendGlobalPacket(new MsgMessage(
//                    string.Format("[{0}] explored a Savage Bone and received an Anima P14!", client.Player.Name),
//                    MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
//                return;
//            }

//            // Myth Soul L6 ── 15%
//            if (roll < 2100)
//            {
//                client.Inventory.Add(stream, MYTH_SOUL_L6, 1);
//                Server.SendGlobalPacket(new MsgMessage(
//                    string.Format("[{0}] explored a Savage Bone and received a Myth Soul Level 6!", client.Player.Name),
//                    MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
//                return;
//            }

//            // Cosmos Hammer ── 10%
//            if (roll < 2400)
//            {
//                client.Inventory.AddItemWitchStack(COSMOS_HAMMER, 0, 1, stream, false);
//                if (!mass) client.SendSysMesage("You received a Cosmos Hammer!");
//                return;
//            }

//            //  Collection Scraps ── 30%
//            if (roll < 4400)
//            {
//                uint scrapId = COLLECTION_SCRAPS[Program.GetRandom.Next(COLLECTION_SCRAPS.Length)];
//                client.Inventory.Add(stream, scrapId, 1);
//                if (!mass) client.SendSysMesage("You received a Collection Scrap!");
//                return;
//            }

//            // Item 3307146 ── 30%
//            if (roll < 6400)
//            {
//                client.Inventory.Add(stream, ITEM_3307146, 1);
//                if (!mass) client.SendSysMesage("You received a special item!");
//                return;
//            }

//            // Fold ── 0%  →  Hard luck
//            client.SendSysMesage("Hard luck! Better luck next time.");
//        }
//    }
//}
