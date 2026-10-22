using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.MsgInterServer.Packets;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.DBFunctionality;
using Qahr.Mobile.Game.MsgTournaments;
using System.IO;
using System.Net.Mail;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;

using Qahr.Mobile.Game.MsgNpc;

using Qahr.Mobile.Role;

namespace Qahr.Mobile.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
        //[NpcAttribute(NpcID.RareRunes)]
        //public static void WorldVote(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{

        //    Dialog dialog = new Dialog(client, stream);

        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.Text("Hello there, I Can Give You Rare Runes (Bound) 1x = [ 50 Points ] \n");
        //                dialog.Option("x1-" + Pool.ItemsBase[4035101].Name + " [50 Points]", 1);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035201].Name + " [50 Points]", 2);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035301].Name + " [50 Points]", 3);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035401].Name + " [50 Points]", 4);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035501].Name + " [50 Points]", 5);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035601].Name + " [50 Points]", 6);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035701].Name + " [50 Points]", 7);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035801].Name + " [50 Points]", 8);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035901].Name + " [50 Points]", 9);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036001].Name + " [50 Points]", 10);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036101].Name + " [50 Points]", 11);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036201].Name + " [50 Points]", 12);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036301].Name + " [50 Points]", 13);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036401].Name + " [50 Points]", 14);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036501].Name + " [50 Points]", 15);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036701].Name + " [50 Points]", 16);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036801].Name + " [50 Points]", 17);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036901].Name + " [50 Points]", 18);
        //                dialog.Option("x1-" + Pool.ItemsBase[4037501].Name + " [50 Points]", 19);
        //                dialog.Option("x1-" + Pool.ItemsBase[4037701].Name + " [50 Points]", 20);
        //                dialog.Option("x1-" + Pool.ItemsBase[4038301].Name + " [50 Points]", 21);
        //                dialog.Option("x1-" + Pool.ItemsBase[4038501].Name + " [50 Points]", 22);
        //                dialog.Option("x1-" + Pool.ItemsBase[4039601].Name + " [50 Points]", 23);
        //                dialog.Option("x1-" + Pool.ItemsBase[4039801].Name + " [50 Points]", 24);
        //                dialog.Option("x1-" + Pool.ItemsBase[4070001].Name + " [50 Points]", 25);
        //                dialog.Option("x1-" + Pool.ItemsBase[4070501].Name + " [50 Points]", 31);
        //                dialog.Option("x1-" + Pool.ItemsBase[4070701].Name + " [50 Points]", 32);
        //                dialog.Option("x1-" + Pool.ItemsBase[4071001].Name + " [50 Points]", 33);
        //                dialog.Option("x1-" + Pool.ItemsBase[4071201].Name + " [50 Points]", 34);
        //                dialog.Option("x1-" + Pool.ItemsBase[4071401].Name + " [50 Points]", 35);
        //                dialog.Option("x1-Blue (" + Pool.ItemsBase[4025001].Name + ") [50 Points]", 26);
        //                dialog.Option("x1-Blue (" + Pool.ItemsBase[4027001].Name + ") [50 Points]", 27);
        //                dialog.Option("x1-Blue (" + Pool.ItemsBase[4028001].Name + ") [50 Points]", 28);
        //                dialog.Option("x1-Blue (" + Pool.ItemsBase[4028301].Name + ") [50 Points]", 29);
        //                dialog.Option("x1-Blue (" + Pool.ItemsBase[4028501].Name + ") [50 Points]", 30);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        default:
        //            {
        //                if (client.Player.DonatePoints >= 50)
        //                {
        //                    client.Player.DonatePoints -= 50;
        //                    uint itemID = 0;
        //                    switch (Option)
        //                    {
        //                        case 1: { itemID = 4035101; break; }
        //                        case 2: { itemID = 4035201; break; }
        //                        case 3: { itemID = 4035301; break; }
        //                        case 4: { itemID = 4035401; break; }
        //                        case 5: { itemID = 4035501; break; }
        //                        case 6: { itemID = 4035601; break; }
        //                        case 7: { itemID = 4035701; break; }
        //                        case 8: { itemID = 4035801; break; }
        //                        case 9: { itemID = 4035901; break; }
        //                        case 10: { itemID = 4036001; break; }
        //                        case 11: { itemID = 4036101; break; }
        //                        case 12: { itemID = 4036201; break; }
        //                        case 13: { itemID = 4036301; break; }
        //                        case 14: { itemID = 4036401; break; }
        //                        case 15: { itemID = 4036501; break; }
        //                        case 16: { itemID = 4036701; break; }
        //                        case 17: { itemID = 4036801; break; }
        //                        case 18: { itemID = 4036901; break; }
        //                        case 19: { itemID = 4037501; break; }
        //                        case 20: { itemID = 4037701; break; }
        //                        case 21: { itemID = 4038301; break; }
        //                        case 22: { itemID = 4038501; break; }
        //                        case 23: { itemID = 4039601; break; }
        //                        case 24: { itemID = 4039801; break; }
        //                        case 25: { itemID = 4070001; break; }
        //                        case 26: { itemID = 4025001; break; }
        //                        case 27: { itemID = 4027001; break; }
        //                        case 28: { itemID = 4028001; break; }
        //                        case 29: { itemID = 4028301; break; }
        //                        case 30: { itemID = 4028501; break; }
        //                        case 31: { itemID = 4070501; break; }
        //                        case 32: { itemID = 4070701; break; }
        //                        case 33: { itemID = 4071001; break; }
        //                        case 34: { itemID = 4071201; break; }
        //                        case 35: { itemID = 4071401; break; }


        //                    }
        //                    client.Inventory.AddItemWitchStack(itemID, 1, 1, stream, true);
        //                    dialog.Text("Congratulations! You've got 1x [ " + Pool.ItemsBase[itemID].Name + " ] (B) .");

        //                    dialog.Option("Thanks.", 255).FinalizeDialog();

        //                }
        //                else
        //                {
        //                    dialog.Text("Sorry, You Don`t Have 50 [ Points ] ");

        //                    dialog.Option("Okay Thanks.", 255).FinalizeDialog();
        //                }


        //                break;
        //            }

        //    }

        //}
        //[NpcAttribute(NpcID.DonatePointsNpc)]
        //public static void DonatePointsNPC(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);

        //    switch (Option)
        //    {
        //        case 0:
        //            { 
        //                dialog.Text("Hello [ <tip color=0xff00fa00> " + client.Player.Name + " </tip> ] ,Welcome to WorldConquer\n");
        //                dialog.Text("<tip color=0xffc501e7>Hello there! I can exchange your DonatePoints for valuable items!</tip>\n");



        //                dialog.Text("<tip color=0xfff05050>Your Current Donate Points:</tip><tip color=0xffffFFff> " + client.Player.DonatePoints + "</tip>\n");

        //                dialog.Text("<tip color=0xffffae00>Runes : </tip><tip color=0xffffFFff>250 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>Collection : </tip><tip color=0xffffFFff>100 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>Cosmos Hammer </tip>\n");
        //                dialog.Text("<tip color=0xffffae00>x1 - : </tip><tip color=0xffffFFff>10 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>x30 - : </tip><tip color=0xffffFFff>250 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>x60 - : </tip><tip color=0xffffFFff>450 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>x330 - : </tip><tip color=0xffffFFff>2400 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>Blaze's Grail : </tip><tip color=0xffffFFff>500 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>5-Star Relic : </tip><tip color=0xffffFFff0>1500 DonatePoints</tip>\n");
        //                dialog.Text("<tip color=0xffffae00>VIP 7 : </tip><tip color=0xffffFFff0>400 DonatePoints</tip>\n");


        //                 dialog.Option("VIP Section", 70);
        //                dialog.Option("Rare Runes Section", 100);
        //                dialog.Option("Collection Section", 200);
        //                dialog.Option("CosmosHammer Section", 60);
        //                dialog.Option("Blaze's Grail", 56);
        //                dialog.Option("5-Star Relic", 57);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        case 70:
        //            {
        //                dialog.Text("VIP Exchange - Choose your option:\n");
        //                dialog.Option("VIP 7 [400 DonatePoints]", 72);
        //                dialog.Option("Back", 0);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        case 72:
        //            {
        //                if (client.Player.DonatePoints >= 400)
        //                {
        //                    client.Player.DonatePoints -= 400;
        //                    client.Inventory.Add(stream, 3004124);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004124].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004124].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004124].Name + " ");

        //                }
        //                break;
        //            }
        //        case 60:
        //            {
        //                dialog.Text("CosmosHammer Exchange - Choose your option:\n");
        //                dialog.Option("x1 CosmosHammer [10 DonatePoints]", 52);
        //                dialog.Option("x30 CosmosHammer [250 DonatePoints]", 53);
        //                dialog.Option("x60 CosmosHammer [450 DonatePoints]", 54);
        //                dialog.Option("x330 CosmosHammer [2400 DonatePoints]", 55);
        //                dialog.Option("Back", 0);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        case 56:
        //            {
        //                if (client.Player.DonatePoints >= 500)
        //                {
        //                    client.Player.DonatePoints -= 500;
        //                    client.Inventory.AddItemWitchStack(2169645, 1, 1, stream, true);
        //                    dialog.Text("Congratulations! You've got a Blaze's Grail!\n");
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[2169645].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[2169645].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //                    dialog.Option("Thanks.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 57:
        //            {
        //                if (client.Player.DonatePoints >= 1500)
        //                {
        //                    client.Player.DonatePoints -= 1500;
        //                    client.Inventory.AddItemWitchStack(3314913, 1, 1, stream, false);
        //                    dialog.Text("Congratulations! You've got a 5-Star Relic!\n");
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3314913].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3314913].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");

        //                    dialog.Option("Thanks.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        default:
        //            {
        //                dialog.Text("Sorry, I don't recognize that option.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }


        //        case 100: // Rune Section
        //            {
        //                dialog.Text("Rare Runes Exchange - Choose your item:\n");

        //                // Add all 35 rune items, each costing 250 DonatePoints
        //                dialog.Option("x1-" + Pool.ItemsBase[4035101].Name + " [250 DonatePoints]", 1);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035201].Name + " [250 DonatePoints]", 2);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035301].Name + " [250 DonatePoints]", 3);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035401].Name + " [250 DonatePoints]", 4);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035501].Name + " [250 DonatePoints]", 5);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035601].Name + " [250 DonatePoints]", 6);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035701].Name + " [250 DonatePoints]", 7);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035801].Name + " [250 DonatePoints]", 8);
        //                dialog.Option("x1-" + Pool.ItemsBase[4035901].Name + " [250 DonatePoints]", 9);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036001].Name + " [250 DonatePoints]", 10);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036101].Name + " [250 DonatePoints]", 11);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036201].Name + " [250 DonatePoints]", 12);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036301].Name + " [250 DonatePoints]", 13);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036401].Name + " [250 DonatePoints]", 14);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036501].Name + " [250 DonatePoints]", 15);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036701].Name + " [250 DonatePoints]", 16);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036801].Name + " [250 DonatePoints]", 17);
        //                dialog.Option("x1-" + Pool.ItemsBase[4036901].Name + " [250 DonatePoints]", 18);
        //                dialog.Option("x1-" + Pool.ItemsBase[4037501].Name + " [250 DonatePoints]", 19);
        //                dialog.Option("x1-" + Pool.ItemsBase[4037701].Name + " [250 DonatePoints]", 20);
        //                dialog.Option("x1-" + Pool.ItemsBase[4038301].Name + " [250 DonatePoints]", 21);
        //                dialog.Option("x1-" + Pool.ItemsBase[4038501].Name + " [250 DonatePoints]", 22);
        //                dialog.Option("x1-" + Pool.ItemsBase[4039601].Name + " [250 DonatePoints]", 23);
        //                dialog.Option("x1-" + Pool.ItemsBase[4039801].Name + " [250 DonatePoints]", 24);
        //                dialog.Option("x1-" + Pool.ItemsBase[4070001].Name + " [250 DonatePoints]", 25);
        //                dialog.Option("x1-" + Pool.ItemsBase[4070501].Name + " [250 DonatePoints]", 26);
        //                dialog.Option("x1-" + Pool.ItemsBase[4070701].Name + " [250 DonatePoints]", 27);
        //                dialog.Option("x1-" + Pool.ItemsBase[4071001].Name + " [250 DonatePoints]", 28);
        //                dialog.Option("x1-" + Pool.ItemsBase[4071201].Name + " [250 DonatePoints]", 29);
        //                dialog.Option("x1-" + Pool.ItemsBase[4071401].Name + " [250 DonatePoints]", 30);
        //                dialog.Option("x1-" + Pool.ItemsBase[4025001].Name + " [250 DonatePoints]", 31);
        //                dialog.Option("x1-" + Pool.ItemsBase[4027001].Name + " [250 DonatePoints]", 32);
        //                dialog.Option("x1-" + Pool.ItemsBase[4028001].Name + " [250 DonatePoints]", 33);
        //                dialog.Option("x1-" + Pool.ItemsBase[4028301].Name + " [250 DonatePoints]", 34);
        //                dialog.Option("x1-" + Pool.ItemsBase[4028501].Name + " [250 DonatePoints]", 35);

        //                dialog.Option("Back", 0);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        case 200: // Collection Section
        //            {
        //                dialog.Text("Collection Section - Choose your item:\n");

        //                // Add collection items here, each costing 100 DonatePoints
        //                dialog.Option("x1-" + Pool.ItemsBase[4300000].Name + " [100 DonatePoints]", 36);
        //                dialog.Option("x1-" + Pool.ItemsBase[4300001].Name + " [100 DonatePoints]", 37);
        //                dialog.Option("x1-" + Pool.ItemsBase[4300002].Name + " [100 DonatePoints]", 38);
        //                dialog.Option("x1-" + Pool.ItemsBase[4300003].Name + " [100 DonatePoints]", 39);
        //                dialog.Option("x1-" + Pool.ItemsBase[4300004].Name + " [100 DonatePoints]", 40);
        //                dialog.Option("x1-" + Pool.ItemsBase[4310000].Name + " [100 DonatePoints]", 41);
        //                dialog.Option("x1-" + Pool.ItemsBase[4310001].Name + " [100 DonatePoints]", 42);
        //                dialog.Option("x1-" + Pool.ItemsBase[4310002].Name + " [100 DonatePoints]", 43);
        //                dialog.Option("x1-" + Pool.ItemsBase[4320000].Name + " [100 DonatePoints]", 44);
        //                dialog.Option("x1-" + Pool.ItemsBase[4320001].Name + " [100 DonatePoints]", 45);
        //                dialog.Option("x1-" + Pool.ItemsBase[4320002].Name + " [100 DonatePoints]", 46);
        //                dialog.Option("x1-" + Pool.ItemsBase[4330000].Name + " [100 DonatePoints]", 47);
        //                dialog.Option("x1-" + Pool.ItemsBase[4330001].Name + " [100 DonatePoints]", 48);
        //                dialog.Option("x1-" + Pool.ItemsBase[4330002].Name + " [100 DonatePoints]", 49);
        //                dialog.Option("x1-" + Pool.ItemsBase[4331000].Name + " [100 DonatePoints]", 50);
        //                dialog.Option("x1-" + Pool.ItemsBase[4332000].Name + " [100 DonatePoints]", 51);

        //                dialog.Option("Back", 0);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //       /* default:
        //            {
        //                // Default case for the whole NPC
        //                dialog.Text("Sorry, I don't recognize that option.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }*/
        //    }
        //    switch (Option)
        //    {
        //        case 52:
        //            if (client.Player.DonatePoints >= 10)
        //            {
        //                client.Player.DonatePoints -= 10;
        //                client.Inventory.AddItemWitchStack(3324226, 0, 1, stream, false);
        //                dialog.Text("Congratulations! You've got 1 CosmosHammer!\n");
        //                dialog.Option("Thanks.", 255).FinalizeDialog();
        //                string amessaj = "";
        //                if (Role.Core.IsBoy(client.Player.Body))
        //                    amessaj = "his";
        //                else if (Role.Core.IsGirl(client.Player.Body))
        //                    amessaj = "her";
        //                Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x1  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                Server.SendGlobalPacket(messaj.GetArray(stream));
        //                Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x1  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //            }
        //            else
        //            {
        //                dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;

        //        case 53:
        //            if (client.Player.DonatePoints >= 250)
        //            {
        //                client.Player.DonatePoints -= 250;
        //                client.Inventory.AddItemWitchStack(3324226, 0, 30, stream, false);
        //                dialog.Text("Congratulations! You've got 30 CosmosHammers!\n");
        //                string amessaj = "";
        //                if (Role.Core.IsBoy(client.Player.Body))
        //                    amessaj = "his";
        //                else if (Role.Core.IsGirl(client.Player.Body))
        //                    amessaj = "her";
        //                Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x30  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                Server.SendGlobalPacket(messaj.GetArray(stream));
        //                Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x30  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //                dialog.Option("Thanks.", 255).FinalizeDialog();
        //            }
        //            else
        //            {
        //                dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;

        //        case 54:
        //            if (client.Player.DonatePoints >= 450)
        //            {
        //                client.Player.DonatePoints -= 450;
        //                client.Inventory.AddItemWitchStack(3324226, 0, 60, stream, false);
        //                dialog.Text("Congratulations! You've got 60 CosmosHammers!\n");
        //                string amessaj = "";
        //                if (Role.Core.IsBoy(client.Player.Body))
        //                    amessaj = "his";
        //                else if (Role.Core.IsGirl(client.Player.Body))
        //                    amessaj = "her";
        //                Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x60  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                Server.SendGlobalPacket(messaj.GetArray(stream));
        //                Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x60  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //                dialog.Option("Thanks.", 255).FinalizeDialog();
        //            }
        //            else
        //            {
        //                dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;

        //        case 55:
        //            if (client.Player.DonatePoints >= 2400)
        //            {
        //                client.Player.DonatePoints -= 2400;

        //                int remaining = 330;
        //                while (remaining > 0)
        //                {
        //                    byte stackSize = (byte)Math.Min(remaining, 255);
        //                    client.Inventory.AddItemWitchStack(3324226, 0 , stackSize, stream, false);
        //                    remaining -= stackSize;
        //                }

        //                dialog.Text("Congratulations! You've got 330 CosmosHammers!\n");
        //                string amessaj = "";
        //                if (Role.Core.IsBoy(client.Player.Body))
        //                    amessaj = "his";
        //                else if (Role.Core.IsGirl(client.Player.Body))
        //                    amessaj = "her";
        //                Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x330  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                Server.SendGlobalPacket(messaj.GetArray(stream));
        //                Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " x330  " + Pool.ItemsBase[3324226].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //                dialog.Option("Thanks.", 255).FinalizeDialog();
        //            }
        //            else
        //            {
        //                dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;
        //    }
        //    // Default case for Rune Section
        //    switch (Option)
        //    {
        //        case 1:
        //        case 2:
        //        case 3:
        //        case 4:
        //        case 5:
        //        case 6:
        //        case 7:
        //        case 8:
        //        case 9:
        //        case 10:
        //        case 11:
        //        case 12:
        //        case 13:
        //        case 14:
        //        case 15:
        //        case 16:
        //        case 17:
        //        case 18:
        //        case 19:
        //        case 20:
        //        case 21:
        //        case 22:
        //        case 23:
        //        case 24:
        //        case 25:
        //        case 26:
        //        case 27:
        //        case 28:
        //        case 29:
        //        case 30:
        //        case 31:
        //        case 32:
        //        case 33:
        //        case 34:
        //        case 35:
        //            if (client.Player.DonatePoints >= 250)
        //            {
        //                uint itemID = 0;
        //                switch (Option)
        //                {
        //                    case 1: itemID = 4035101; break;
        //                    case 2: itemID = 4035201; break;
        //                    case 3: itemID = 4035301; break;
        //                    case 4: itemID = 4035401; break;
        //                    case 5: itemID = 4035501; break;
        //                    case 6: itemID = 4035601; break;
        //                    case 7: itemID = 4035701; break;
        //                    case 8: itemID = 4035801; break;
        //                    case 9: itemID = 4035901; break;
        //                    case 10: itemID = 4036001; break;
        //                    case 11: itemID = 4036101; break;
        //                    case 12: itemID = 4036201; break;
        //                    case 13: itemID = 4036301; break;
        //                    case 14: itemID = 4036401; break;
        //                    case 15: itemID = 4036501; break;
        //                    case 16: itemID = 4036701; break;
        //                    case 17: itemID = 4036801; break;
        //                    case 18: itemID = 4036901; break;
        //                    case 19: itemID = 4037501; break;
        //                    case 20: itemID = 4037701; break;
        //                    case 21: itemID = 4038301; break;
        //                    case 22: itemID = 4038501; break;
        //                    case 23: itemID = 4039601; break;
        //                    case 24: itemID = 4039801; break;
        //                    case 25: itemID = 4070001; break;
        //                    case 26: itemID = 4070501; break;
        //                    case 27: itemID = 4070701; break;
        //                    case 28: itemID = 4071001; break;
        //                    case 29: itemID = 4071201; break;
        //                    case 30: itemID = 4071401; break;
        //                    case 31: itemID = 4025001; break;
        //                    case 32: itemID = 4027001; break;
        //                    case 33: itemID = 4028001; break;
        //                    case 34: itemID = 4028301; break;
        //                    case 35: itemID = 4028501; break;
        //                }
        //                client.Player.DonatePoints -= 250;
        //                client.Inventory.AddItemWitchStack(itemID, 1, 1, stream, true);
        //                dialog.Text("Congratulations! You've got One " + Pool.ItemsBase[itemID].Name + "!\n");
        //                string amessaj = "";
        //                if (Role.Core.IsBoy(client.Player.Body))
        //                    amessaj = "his";
        //                else if (Role.Core.IsGirl(client.Player.Body))
        //                    amessaj = "her";
        //                Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[itemID].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                Server.SendGlobalPacket(messaj.GetArray(stream));
        //                Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[itemID].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //                dialog.Option("Thanks.", 255).FinalizeDialog();
        //            }
        //            else
        //            {
        //                dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;
        //    }

        //    // Default case for Collection Section
        //    switch (Option)
        //    {
        //        case 36:
        //        case 37:
        //        case 38:
        //        case 39:
        //        case 40:
        //        case 41:
        //        case 42:
        //        case 43:
        //        case 44:
        //        case 45:
        //        case 46:
        //        case 47:
        //        case 48:
        //        case 49:
        //        case 50:
        //        case 51:
        //            if (client.Player.DonatePoints >= 100)
        //            {
        //                uint itemID = 0;
        //                switch (Option)
        //                {
        //                    case 36: itemID = 4300000; break;
        //                    case 37: itemID = 4300001; break;
        //                    case 38: itemID = 4300002; break;
        //                    case 39: itemID = 4300003; break;
        //                    case 40: itemID = 4300004; break;
        //                    case 41: itemID = 4310000; break;
        //                    case 42: itemID = 4310001; break;
        //                    case 43: itemID = 4310002; break;
        //                    case 44: itemID = 4320000; break;
        //                    case 45: itemID = 4320001; break;
        //                    case 46: itemID = 4320002; break;
        //                    case 47: itemID = 4330000; break;
        //                    case 48: itemID = 4330001; break;
        //                    case 49: itemID = 4330002; break;
        //                    case 50: itemID = 4331000; break;
        //                    case 51: itemID = 4332000; break;
        //                }
        //                client.Player.DonatePoints -= 100;
        //                client.Inventory.AddItemWitchStack(itemID, 1, 1, stream, false);
        //                dialog.Text("Congratulations! You've got One " + Pool.ItemsBase[itemID].Name + "!\n");
        //                string amessaj = "";
        //                if (Role.Core.IsBoy(client.Player.Body))
        //                    amessaj = "his";
        //                else if (Role.Core.IsGirl(client.Player.Body))
        //                    amessaj = "her";
        //                Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[itemID].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                Server.SendGlobalPacket(messaj.GetArray(stream));
        //                Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[itemID].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");
        //                dialog.Option("Thanks.", 255).FinalizeDialog();
        //            }
        //            else
        //            {
        //                dialog.Text("Sorry, you don't have enough DonatePoints.\n");
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //            }
        //            break;
        //    }
        //}


        [NpcAttribute(NpcID.ChiSuper)]
        public static void ChiSuper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.Player.SelectedStage = 0;
                        client.Player.SelectedAttribute = 0;  
                        if (client.Player.MyChi != null)
                        {
                            bool have = false;
                            int count = 0;
                            foreach (var obj in client.Player.MyChi)
                            {
                                if (obj.UnLocked)
                                {
                                    have = true;
                                    dialog.Option(obj.Type.ToString(), (byte)(1 + count));
                                    count++;
                                }
                            }


                            if (have)
                            {
                                dialog.Text("Hello, Through Me You Can Open The Four Gates In Your Chi \n (Super Full) For 1 CPs").AddAvatar(3496).FinalizeDialog();
                            }
                            else
                            {
                                dialog.Text("Sorry, You Don`t Have Any Stage Chi.").Option("Okay.", 255).AddAvatar(3496).FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Don`t Have Any Stage Chi.").Option("Okay.", 255).AddAvatar(3496).FinalizeDialog();
                        }

                        break;
                    }
                default:
                    {
                        if (client.Player.ConquerPoints < 1)
                        {
                            dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            int mStage = Option;
                            if (mStage >= 1 && mStage <= 4)
                            {
                                if (client.Player.ConquerPoints < 1)
                                {
                                    dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();


                                    break;
                                }
                                client.Player.SelectedStage = mStage;
                                dialog.Text("You Choose " + ((MsgChiInfo.ChiPowerType)mStage).ToString() + " Stage Please Select Attribute You Want To Change.");
                                for (int i = 0; i < 4; i++)
                                {
                                    dialog.Option("Attribute [" + (byte)(i + 1) + "]", (byte)(i + 1 + 4));
                                }
                                dialog.Option("I'll think about it!", 255);
                                dialog.AddAvatar(3496).FinalizeDialog();
                                break;
                            }
                            else if (mStage >= 4 && mStage <= 8)
                            {
                                if (client.Player.ConquerPoints < 1)
                                {
                                    dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();

                                    break;
                                }
                                client.Player.SelectedAttribute = mStage - 4;
                                var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ((MsgChiInfo.ChiPowerType)client.Player.SelectedStage).ToString());
                                var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                                dialog.Text("You Choose Attribute [" + client.Player.SelectedAttribute + "] Please Select Attribute Type You Want To Change.");
                                for (int i = 0; i < Qahr.Mobile.Chi.AtributesType.Length; i++)
                                {
                                    var type = Qahr.Mobile.Chi.AtributesType[i];
                                    if (Power.Attributes[0].ToString() != type && Power.Attributes[1].ToString() != type && Power.Attributes[2].ToString() != type && Power.Attributes[3].ToString() != type)
                                    {
                                        dialog.Option(type, ((byte)(((i + 1) * 10))));
                                    }
                                }
                                dialog.Option("Back", (byte)client.Player.SelectedStage);
                                dialog.Option("I'll think about it!", 255);
                                dialog.AddAvatar(3496).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (client.Player.ConquerPoints < 1)
                                {
                                    dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();


                                    break;
                                }
                                var typex = mStage / 10;
                                var Mode = client.Player.SelectedStage;
                                var pos = client.Player.SelectedAttribute - 1;
                                var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ((MsgChiInfo.ChiPowerType)Mode).ToString());
                                var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                                var attributes = Power.Attributes;
                                foreach (var attr in attributes)
                                {

                                    if (attr.Type == (Game.MsgServer.MsgChiInfo.ChiAttribute)typex)
                                    {
                                        client.Player.MessageBox("Sorry~You~Have~The~Attributes.", null, null);
                                        return;
                                    }
                                }
                                var attribte = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)Enum.Parse(typeof(Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute), ((Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)typex).ToString());
                                var Value = Role.Instance.Chi.ChisuperValues(attribte);

                                attributes[pos].Type = attribte;
                                Power.Attributes[pos].Value = (ushort)Value;

                                client.Player.ConquerPoints -= 1;
                                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Send);
                            }
                        }
                        break;
                    }
            }
        }


        //[NpcAttribute(NpcID.ChiFull)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void ChiFull(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.Player.SelectedStage = 0;
                        client.Player.SelectedAttribute = 0;
                        if (client.Player.MyChi != null)
                        {
                            bool have = false;
                            int count = 0;
                            foreach (var obj in client.Player.MyChi)
                            {
                                if (obj.UnLocked)
                                {
                                    have = true;
                                    dialog.Option(obj.Type.ToString(), (byte)(1 + count));
                                    count++;
                                }
                            }


                            if (have)
                            {
                                dialog.Text("Hello, Through Me You Can Open The Four Gates In Your Chi \n (Super Full) For 1 CPs").AddAvatar(3496).FinalizeDialog();
                            }
                            else
                            {
                                dialog.Text("Sorry, You Don`t Have Any Stage Chi.").Option("Okay.", 255).AddAvatar(3496).FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Don`t Have Any Stage Chi.").Option("Okay.", 255).AddAvatar(3496).FinalizeDialog();
                        }

                        break;
                    }
                default:
                    {
                        if (client.Player.ConquerPoints < 1)
                        {
                            dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            int mStage = Option;
                            if (mStage >= 1 && mStage <= 4)
                            {
                                if (client.Player.ConquerPoints < 1)
                                {
                                    dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();


                                    break;
                                }
                                client.Player.SelectedStage = mStage;
                                dialog.Text("You Choose " + ((MsgChiInfo.ChiPowerType)mStage).ToString() + " Stage Please Select Attribute You Want To Change.");
                                for (int i = 0; i < 4; i++)
                                {
                                    dialog.Option("Attribute [" + (byte)(i + 1) + "]", (byte)(i + 1 + 4));
                                }
                                dialog.Option("I'll think about it!", 255);
                                dialog.AddAvatar(3496).FinalizeDialog();
                                break;
                            }
                            else if (mStage >= 4 && mStage <= 8)
                            {
                                if (client.Player.ConquerPoints < 1)
                                {
                                    dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();

                                    break;
                                }
                                client.Player.SelectedAttribute = mStage - 4;
                                var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ((MsgChiInfo.ChiPowerType)client.Player.SelectedStage).ToString());
                                var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                                dialog.Text("You Choose Attribute [" + client.Player.SelectedAttribute + "] Please Select Attribute Type You Want To Change.");
                                for (int i = 0; i < Qahr.Mobile.Chi.AtributesType.Length; i++)
                                {
                                    var type = Qahr.Mobile.Chi.AtributesType[i];
                                    if (Power.Attributes[0].ToString() != type && Power.Attributes[1].ToString() != type && Power.Attributes[2].ToString() != type && Power.Attributes[3].ToString() != type)
                                    {
                                        dialog.Option(type, ((byte)(((i + 1) * 10))));
                                    }
                                }
                                dialog.Option("Back", (byte)client.Player.SelectedStage);
                                dialog.Option("I'll think about it!", 255);
                                dialog.AddAvatar(3496).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                if (client.Player.ConquerPoints < 1)
                                {
                                    dialog.Text("You don`t have 1 CPs.").AddAvatar(3496).FinalizeDialog();


                                    break;
                                }
                                var typex = mStage / 10;
                                var Mode = client.Player.SelectedStage;
                                var pos = client.Player.SelectedAttribute - 1;
                                var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ((MsgChiInfo.ChiPowerType)Mode).ToString());
                                var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                                var attributes = Power.Attributes;
                                foreach (var attr in attributes)
                                {

                                    if (attr.Type == (Game.MsgServer.MsgChiInfo.ChiAttribute)typex)
                                    {
                                        client.Player.MessageBox("Sorry~You~Have~The~Attributes.", null, null);
                                        return;
                                    }
                                }
                                var attribte = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)Enum.Parse(typeof(Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute), ((Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)typex).ToString());
                                var Value = Role.Instance.Chi.ChiMaxValues(attribte);

                                attributes[pos].Type = attribte;
                                Power.Attributes[pos].Value = (ushort)Value;

                                client.Player.ConquerPoints -= 1;
                                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Send);
                            }
                        }
                        break;
                    }
            }
        }



        //[NpcAttribute(NpcID.ChiFull)]
        //public static void ChiFull(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog a = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                if (client.Player.Reborn < 2 || client.Player.Level == 15)
        //                {
        //                    a.AddText("Sorry, you are not in your 2nd-reborns and level 15.");
        //                    a.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
        //                    return;
        //                }
        //                a.AddText("Choose What You Want");
        //                a.AddOption("[HP]+[Attack]+[Immy]+[Strike].", 1);
        //                a.AddOption("[HP]+[Attack]+[Immy]+[Break].", 2);
        //                a.AddOption("[HP]+[Attack]+[AntiBreak]+[Strike].", 10);
        //                a.AddOption("[HP]+[Attack]+[AntiBreak]+[Break].", 11);
        //                a.AddOption("[HP]+[M-Attack]+[Immy]+[M-Strike].", 3);
        //                a.AddOption("[HP]+[P-Damge]+[AntiBreak]+[Immy].", 4);
        //                a.AddOption("[HP]+[M-Attack]+[Immy]+[Final-M].", 5);
        //                a.AddOption("[HP]+[M-Attack]+[Immy]+[Break].", 20);
        //                a.AddOption("[HP]+[M-Attack]+[Immy]+[AntiBreak].", 21);
        //                a.AddOption("I~will~come~later.", 255);
        //                a.AddAvatar(3497).FinalizeDialog();
        //                break;
        //            }                
        //        case 1:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 1", 0, 0));



        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Strike .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 10:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 1", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 1", 0, 0));



        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Strike .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 11:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 4", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Break .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 4", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Break .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 2", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 2", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 2", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 2", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Fire .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 20:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 4", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 4", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Fire .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }

        //        case 21:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 5", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Fire .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 4:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 12", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 12", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 12", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 5", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 12", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Defense .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }

        //        case 5:
        //            {
        //                if (client.Player.ConquerPoints >= 1)
        //                {

        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 1 3 11", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 2 3 11", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 3 3 11", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 1 8", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
        //                    MsgMessage.NpcCommands(client, new Game.MsgServer.MsgMessage("@chipower 4 3 11", 0, 0));
        //                    client.CreateBoxDialog("Congratulations, You Have Successfully Received [Full Chi Epic] .");
        //                }
        //                else
        //                {
        //                    a.AddText("You can not use this option for You've Chi~Fire .")
        //                        .AddOption("I see.", 255)
        //                          .AddAvatar(3497)
        //                        .FinalizeDialog();
        //                }


        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.ChargeChi)]
        public static void ChargeChi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        client.Player.SelectedStage = 0;
                        client.Player.SelectedAttribute = 0;
                        if (client.Player.MyChi != null)
                        {
                            bool have = false;
                            int count = 0;
                            foreach (var obj in client.Player.MyChi)
                            {
                                if (obj.UnLocked)
                                {
                                    have = true;
                                    dialog.Option(obj.Type.ToString(), (byte)(1 + count));
                                    count++;
                                }
                            }
                            if (have)
                            {
                                dialog.Text("Hello,Sir You Can Change Chi").AddAvatar(261).FinalizeDialog();
                            }
                            else
                            {
                                dialog.Text("Sir ,You Don't Have Any Stage Chi.").Option("#Thanks", 255).AddAvatar(261).FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.Text("Sir ,You Don't Have Any Stage Chi.").Option("#Thanks", 255).AddAvatar(261).FinalizeDialog();
                        }

                        break;
                    }
                default:
                    {

                        int mStage = Option;
                        if (mStage >= 1 && mStage <= 4)
                        {

                            client.Player.SelectedStage = mStage;
                            dialog.Text("You Choose " + ((MsgChiInfo.ChiPowerType)mStage).ToString() + " stage. please select Attribute you want to change ");
                            for (int i = 0; i < 4; i++)
                            {
                                dialog.Option("Attribute [" + (byte)(i + 1) + "]", (byte)(i + 1 + 4));
                            }
                            dialog.Option("#Thanks", 255);
                            dialog.AddAvatar(261).FinalizeDialog();
                            break;
                        }
                        else if (mStage >= 4 && mStage <= 8)
                        {
                            client.Player.SelectedAttribute = mStage - 4;
                            var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ((MsgChiInfo.ChiPowerType)client.Player.SelectedStage).ToString());
                            var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                            dialog.Text("You Choose Attribute [" + client.Player.SelectedAttribute + "] please select AttributeType you want to change ");
                            for (int i = 0; i < Qahr.Mobile.Chi.AtributesType.Length; i++)
                            {
                                var type = Qahr.Mobile.Chi.AtributesType[i];
                                if (Power.Attributes[0].ToString() != type && Power.Attributes[1].ToString() != type && Power.Attributes[2].ToString() != type && Power.Attributes[3].ToString() != type)
                                {
                                    dialog.Option(type, ((byte)(((i + 1) * 10))));
                                }
                            }
                            dialog.Option("Back Page", (byte)client.Player.SelectedStage);
                            dialog.Option("#Thanks", 255);
                            dialog.AddAvatar(261).FinalizeDialog();
                            break;
                        }
                        else
                        {

                            var typex = mStage / 10;
                            var Mode = client.Player.SelectedStage;
                            var pos = client.Player.SelectedAttribute - 1;
                            var PowerType = (Game.MsgServer.MsgChiInfo.ChiPowerType)Enum.Parse(typeof(Game.MsgServer.MsgChiInfo.ChiPowerType), ((MsgChiInfo.ChiPowerType)Mode).ToString());
                            var Power = client.Player.MyChi.Where(p => p.Type == PowerType).FirstOrDefault();
                            var attributes = Power.Attributes;
                            foreach (var attr in attributes)
                            {

                                if (attr.Type == (Game.MsgServer.MsgChiInfo.ChiAttribute)typex)
                                {
                                    client.Player.MessageBox("Sorry You Have This Attribute", null, null);
                                    return;
                                }
                            }
                            var attribte = (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)Enum.Parse(typeof(Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute), ((Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)typex).ToString());
                            var Value = Role.Instance.Chi.ChiMaxValues(attribte);

                            attributes[pos].Type = attribte;
                            Power.Attributes[pos].Value = (ushort)Value;

                            client.Inventory.Remove(728156, 1, stream);
                            Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                            client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Send);
                        }
                    }
                    break;
            }

        }
        //[NpcAttribute(NpcID.NewServerReward)]
        //public static void NewServerReward(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello," + client.Player.Name + " You can get one prize because of NewServer");
        //                data.Option("Claim Now", 1);
        //                //data.Option("Claim FreeStuff", 2);
        //                data.AddAvatar(85);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (!client.Player.CanClaim)
        //                {
        //                    Database.StarDragonBall.CheckUp(client);
        //                    break;
        //                }
        //                else
        //                {
        //                    data.Text("you already Have a Prize  maybe other account").AddOption("i~see", byte.MaxValue).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Inventory.AddItemWitchStack(3316568, 0, 1, stream, true);
        //                break;
        //            }

        //    }
        //}

        //[NpcAttribute(NpcID.GameSquid)]
        //public static void GameSquid(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello,"+client.Player.Name+" HappyNewYear join this  event to get alot prize ");
        //                data.Option("Join Now",1);
        //                data.AddAvatar(85);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {

        //                client.Player.DeathTime = DateTime.Now.AddMinutes(3);
        //                ActionQuery action = new ActionQuery();
        //                action.ObjId = client.Player.UID;
        //                action.Type = ActionType.CountDown;
        //                action.dwParam = (uint)180;
        //                client.Send(stream.ActionCreate(action));
        //                client.Teleport(149, 203, 10027);

        //                break;
        //            }

        //    }
        //}
        //[NpcAttribute(NpcID.PrizeOfficer)]
        //public static void PrizeOfficer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{

        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText(client.Player.Name + " have " + client.Player.DonatePoints + " USD\n");
        //              data.AddText("Please wait , until the system will check the donation !")
        //                .AddOption("Ok, i`m waiting.", 1);
        //                data.AddAvatar(85);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                var prizes = PayPalHandler.getItems(client.AccountName(client.Player.Name));
        //                foreach (var item in prizes)
        //                {
        //                    if (item.Key > 0)
        //                    {
        //                        client.Player.DonatePoints += item.Key;
        //                        client.CreateBoxDialog("You have received " + item.Key + " USD. Thank you for your donation.");
        //                        string amessaj = "";
        //                        if (Role.Core.IsBoy(client.Player.Body))
        //                            amessaj = "his";
        //                        else if (Role.Core.IsGirl(client.Player.Body))
        //                            amessaj = "her";
        //                        Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + item.Key + " USD bought from donate page.(Thanks for supporting our server).",Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                        Server.SendGlobalPacket(messaj.GetArray(stream));
        //                        PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + item.Key + " USD");
        //                    }
        //                }
        //                if (prizes.Count == 0)
        //                {
        //                    client.CreateBoxDialog("Please come back later there is nothing for you to claim at the moment. ");
        //                }
        //                else
        //                {
        //                    client.CreateBoxDialog("Thank you for your donation.");
        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.MarketCpAdmin)]
        //public static void MarketCpAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("You have TQ Point Card(s) worth of 1 VIP 7 (30Day) &&You have TQ Point Card(s) worth of 1 VIP 6 (30Day) && TQ Point Card(s) worth of 1 VIP 4 (30Day),&& TQ Point Card(s) worth of 1B Money&& TQ Point Card(s) worth  of 10B Money")
        //                    .AddText("9600151_0101@@")
        //              // .AddOption("5056105_0102@@", 1)
        //               //.AddOption("6049513_0102@@", 2)
        //               //.AddOption("3512220_0102@@", 3)
        //               //.AddOption("94486156_0102@@", 4)
        //               .AddOption("Claim  1 M Conquer Points(USD2)", 13)
        //               .AddOption("Claim 20B Money (USD2)", 8)
        //               .AddOption("Claim 1 VIP 7 (30Day) (USD30)", 11)
        //               .AddOption("Claim 1 VIP 6 (30Day) (USD15)", 5)
        //               .AddOption("Claim 1 VIP 7 (7Day) (USD7)", 12)
        //               .AddOption("Claim 1 VIP 6 (7Day) (USD 3)", 9)
        //               .AddOption("Claim 1 VIP 7 (1Day) (USD1)", 16)
        //               .AddOption("Claim 1 VIP 6 (1Day) (USD 0.5)", 17)
        //               .AddOption("I~have~nothing.", 255).AddAvatar(7).FinalizeDialog();

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Inventory.Contain(Database.ItemType.DragonBallScroll, 1))
        //                {
        //                    client.Inventory.Remove(Database.ItemType.DragonBallScroll, 1, stream);
        //                    client.Player.MyChi.ChiPoints += 8000;

        //                    client.CreateBoxDialog("Congratulations!You~have~got~8000~ChiPoints.");
        //                }
        //                else
        //                {
        //                    data.AddText("Where~is~your~DBScroll?")
        //                  .AddOption("Er,~sorry.~I'm not have DBScroll.", 255).AddAvatar(7).FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
        //                {
        //                    client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
        //                    client.Player.MyChi.ChiPoints += 800;

        //                    client.CreateBoxDialog("Congratulations!You~have~got~800~ChiPoints.");
        //                }
        //                else
        //                {
        //                    data.AddText("Where~is~your~DragonBall?")
        //                  .AddOption("Er,~sorry.~I'm not have DragonBall.", 255).AddAvatar(7).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Inventory.Contain(Database.ItemType.DragonBall, 1, 1))
        //                {
        //                    client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
        //                    client.Player.MyChi.ChiPoints += 800;

        //                    client.CreateBoxDialog("Congratulations!You~have~got~800~ChiPoints.");
        //                }
        //                else
        //                {
        //                    data.AddText("Where~is~your~DragonBall(B)?")
        //                  .AddOption("Er,~sorry.~I'm not have DragonBall(B).", 255).AddAvatar(7).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 4:
        //            {
        //                if (client.Inventory.Contain(Database.ItemType.DragonBallScroll, 1,1))
        //                {
        //                    client.Inventory.Remove(Database.ItemType.DragonBallScroll, 1, stream);
        //                    client.Player.MyChi.ChiPoints += 8000;

        //                    client.CreateBoxDialog("Congratulations!You~have~got~8000~ChiPoints.");
        //                }
        //                else
        //                {
        //                    data.AddText("Where~is~your~DBScroll(B)?")
        //                  .AddOption("Er,~sorry.~I'm not have DBScroll(B).", 255).AddAvatar(7).FinalizeDialog();
        //                }


        //                break;
        //            }
        //        case 5:
        //            {
        //                if (client.Player.DonatePoints >= 15)
        //                {
        //                    client.Player.DonatePoints -= 15;
        //                    client.Inventory.Add(stream, 3004123);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004123].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004123].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004123].Name + " ");

        //                }
        //                break;
        //            }
        //        case 6:
        //            {
        //                if (client.Player.DonatePoints >= 15.99)
        //                {
        //                    client.Player.DonatePoints -= 15.99;
        //                    client.Inventory.Add(stream, 3004121);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004121].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004121].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004121].Name + " ");
        //                }
        //                break;
        //            }
        //        case 7:
        //            {
        //                if (client.Player.DonatePoints >= 0.99)
        //                {
        //                    client.Player.DonatePoints -= 0.99;
        //                    client.Player.Money += 1000000000;
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " BB Gold bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + 1 + "BB Gold");
        //                }
        //                break;
        //            }
        //        case 8:
        //            {
        //                if (client.Player.DonatePoints >= 1)
        //                {
        //                    client.Player.DonatePoints -= 1;
        //                    client.Player.Money += 20000000000;
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 20 + " BB Gold bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 20 + " BB Gold bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + 2 + "BB Gold");
        //                }
        //                break;
        //            }

        //        case 10:
        //            {
        //                if (client.Player.DonatePoints >= 3)
        //                {
        //                    client.Player.DonatePoints -= 3;
        //                    client.Inventory.Add(stream, 780010);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[780010].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[780010].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[780010].Name + " ");

        //                }
        //                break;
        //            }
        //        case 11:
        //            {
        //                if (client.Player.DonatePoints >= 30)
        //                {
        //                    client.Player.DonatePoints -= 30;
        //                    client.Inventory.Add(stream, 3004124);
        //                    client.Inventory.Add(stream, 192775);
        //                    client.Inventory.Add(stream, 200221);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004124].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3004124].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3004124].Name + " ");

        //                }
        //                break;
        //            }
        //        case 12:
        //            {
        //                if (client.Player.DonatePoints >= 7)
        //                {
        //                    client.Player.DonatePoints -= 7;
        //                    client.Inventory.Add(stream, 3306221);
        //                    client.Inventory.Add(stream, 192775);
        //                    client.Inventory.Add(stream, 200221);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3306221].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3306221].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3306221].Name + " ");

        //                }
        //                break;
        //            }
        //        case 9:
        //            {
        //                if (client.Player.DonatePoints >= 3)
        //                {
        //                    client.Player.DonatePoints -= 3;
        //                    client.Inventory.Add(stream, 780057);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[780057].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[780057].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[780057].Name + " ");

        //                }
        //                break;
        //            }
        //        case 13:
        //            {
        //                if (client.Player.DonatePoints >= 5)
        //                {
        //                    client.Player.DonatePoints -= 5;
        //                    client.Player.ConquerPoints += 1000000;
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " M ConquerPoints Gold bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " M ConquerPoints Gold bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + 1 + "M ConquerPoints");
        //                }
        //                break;
        //            }
        //        case 14:
        //            {
        //                if (client.Player.DonatePoints >= 1)
        //                {
        //                    client.Player.DonatePoints -= 1;
        //                    client.Player.ConquerPoints += 200000;
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " K ConquerPoints bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " K ConquerPoints bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + 100 + "K ConquerPoints");
        //                }
        //                break;
        //            }
        //        case 15:
        //            {
        //                if (client.Player.DonatePoints >= 0.99)
        //                {
        //                    client.Player.DonatePoints -= 0.99;
        //                    client.UpdateLevel(stream, 140);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " K ConquerPoints bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + 1 + " K ConquerPoints bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + 100 + "K ConquerPoints");
        //                }
        //                break;
        //            }
        //        case 16:
        //            {
        //                if (client.Player.DonatePoints >= 1)
        //                {
        //                    client.Player.DonatePoints -= 1;
        //                    client.Inventory.Add(stream, 727381);
        //                    client.Inventory.Add(stream, 192775);
        //                    client.Inventory.Add(stream, 200221);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[727381].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[727381].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[727381].Name + " ");

        //                }
        //                break;
        //            }
        //        case 17:
        //            {
        //                if (client.Player.DonatePoints >= 0.5)
        //                {
        //                    client.Player.DonatePoints -= 0.5;
        //                    client.Inventory.Add(stream, 3306220);
        //                    string amessaj = "";
        //                    if (Role.Core.IsBoy(client.Player.Body))
        //                        amessaj = "his";
        //                    else if (Role.Core.IsGirl(client.Player.Body))
        //                        amessaj = "her";
        //                    Game.MsgServer.MsgMessage messaj = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3306220].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center);
        //                    Server.SendGlobalPacket(messaj.GetArray(stream));
        //                    Game.MsgServer.MsgMessage messaj2 = new Game.MsgServer.MsgMessage(client.Player.Name + " ! claimed " + amessaj + " " + Pool.ItemsBase[3306220].Name + " bought from donate page.(Thanks for supporting our server).", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
        //                    Server.SendGlobalPacket(messaj2.GetArray(stream));
        //                    PayPalHandler.logDonation(client.Player.Name, client.Player.Name, "You have received " + Pool.ItemsBase[3306220].Name + " ");

        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.MarketCpAdmin)] // The NPC Edit By Mohy ElDin ...
        public static void MarketCpAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can here exchange a Dragon Ball for 800 Chi Points. or exchange a DBScroll for 8000 Chi Points.                                                                                    and also You can here exchange a Dragon Ball (B) for 800 Chi Points.            or exchange a DBScroll (B) for 8000 Chi Points.")
                       .AddOption("Exchange~a~DragonBall.", 50)
                       .AddOption("Exchange~a~DBScroll.", 51)
                       .AddOption("Exchange a DragonBall (B).", 52)
                       .AddOption("Exchange a DBScroll (B).", 54)
                       .AddOption("I`ll talk to you later. ", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 50:
                    {
                        {
                            data.AddText("Are you sure you want to exchange a Dragon Ball for 800 Chi Points")
                          .AddOption("Yep!", 9)
                            .AddOption("Nope.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 52:
                    {
                        {
                            data.AddText("Are you sure you want to exchange a Dragon Ball (B) for 800 Chi Points")
                          .AddOption("Yep!", 53)
                            .AddOption("Nope.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 53:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1, 1))
                        {
                            client.Inventory.Removeb(Database.ItemType.DragonBall, 1, stream, 1);
                            client.Player.MyChi.ChiPoints += 800;

                            client.SendSysMesage("You have exchanged a Dragonball (B) for 800 Chi Points.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            data.AddText("You don`t have any Dragonballs(B). Come back when you get one.")
                          .AddOption("Sure, I will.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 51:
                    {
                        {
                            data.AddText("Are you sure you want to exchange a DBScroll for 8000 Chi Points")
                          .AddOption("Yep!", 10)
                            .AddOption("Nope.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 54:
                    {
                        {
                            data.AddText("Are you sure you want to exchange a DBScroll(B) for 8000 Chi Points?")
                          .AddOption("Yep!", 55)
                            .AddOption("Nope.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 55:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DBScroll, 1, 1))
                        {
                            client.Inventory.Removeb(Database.ItemType.DBScroll, 1, stream, 1);
                            client.Player.MyChi.ChiPoints += 8000;

                            client.SendSysMesage("You have exchanged a DBScroll (B) for 8000 Chi Points.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            data.AddText("Where is your DBScroll(B)?")
                          .AddOption("Sorry, I have none.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
                            client.Player.MyChi.ChiPoints += 800;
                            client.SendSysMesage("You have exchanged a Dragonball for 800 Chi Points.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            data.AddText("You don`t have any Dragonballs. Come back when you get one.")
                          .AddOption("Sure, I will.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DBScroll, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 1, stream);
                            client.Player.MyChi.ChiPoints += 8000;

                            client.SendSysMesage("You have exchanged a DBScroll for 8000 Chi Points.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                        }
                        else
                        {
                            data.AddText("Where~is~your~DBScroll")
                          .AddOption("Sorry, I have none.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DBScroll, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 1, stream);
                            client.Player.ConquerPoints += 2150;

                            client.CreateBoxDialog("Congratulations!You~have~got~2150~CPs~in~your~inventory");
                        }
                        else
                        {
                            data.AddText("Where~is~your~DBScroll?")
                          .AddOption("Er,~sorry.~I'm not have DBScroll.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
                            client.Player.ConquerPoints += 215;

                            client.CreateBoxDialog("Congratulations!You~have~got~215~CPs~in~your~inventory");
                        }
                        else
                        {
                            data.AddText("Where~is~your~DragonBall?")
                          .AddOption("Er,~sorry.~I'm not have Dragon Ball.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("You wana to change 10.000.000 Money for 100 ConquerPoints? or 100.000.000 Money for 1000 ConquerPoints?")
              .AddOption("Change 10kk for 100CPs", 4)
              .AddOption("Change 100kk for 1000CPs", 5)
              .AddOption("No Thanks", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.Money >= 10000000)
                        {
                            client.Player.Money -= 10000000;
                            client.Player.ConquerPoints += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);

                            client.CreateBoxDialog("Congratulations!You~have~got~100~CPs~in~your~inventory");
                        }
                        else
                        {
                            data.AddText("You not have 10.000.000 Money with you.")
                             .AddOption("I~see", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        if (client.Player.Money >= 100000000)
                        {
                            client.Player.Money -= 100000000;
                            client.Player.ConquerPoints += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);

                            client.CreateBoxDialog("Congratulations!You~have~got~1000~CPs~in~your~inventory");
                        }
                        else
                        {
                            data.AddText("You not have 100.000.000 Money with you.")
                             .AddOption("I~see", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Steed1)]
        public static void Steed1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(728525, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1.");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: black, brown or white.")
                            .AddOption("Black horse", 1)
                       .AddOption("Brown horse", 2)
                       .AddOption("White horse", 3)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(728525, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 255, 0, 150);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(728525, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 0, 150, 255);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(728525, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 150, 255, 0);

                        break;
                    }
            }
        }
        //DazzlingDiamondBox
        [NpcAttribute(NpcID.DazzlingDiamondBox)]
        public static void DazzlingDiamondBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3008732, 1))
            {
                //Sun~Diamond,~Moon~Diamond,~Star~Diamond~and~Cloud~Diamond
                client.CreateBoxDialog("Sorry, you don`t have a DazzlingDiamondBox.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a 30 pieces Diamond: Sun~Diamond, Moon~Diamond, Star~Diamond or Cloud~Diamond.")
                           .AddOption("Sun~Diamond", 1)
                      .AddOption("Moon~Diamond", 2)
                      .AddOption("Star~Diamond", 3)
                      .AddOption("Cloud~Diamond", 4)
                      .AddOption("Nevermind", 255)
                      .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(30))
                        {
                            client.Inventory.Remove(3008732, 1, stream);
                            client.Inventory.AddItemWitchStack(721533, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Sun~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }



                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(30))
                        {
                            client.Inventory.Remove(3008732, 1, stream);
                            client.Inventory.AddItemWitchStack(721534, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Moon~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }



                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(30))
                        {
                            client.Inventory.Remove(3008732, 1, stream);
                            client.Inventory.AddItemWitchStack(721535, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Star~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(30))
                        {
                            client.Inventory.Remove(3008732, 1, stream);
                            client.Inventory.AddItemWitchStack(721536, 0, 30, stream);

                            client.CreateBoxDialog("You receive 30 pieces of the Cloud~Diamond.");
                        }
                        else
                        {
                            data.AddText("Please make 30 spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .FinalizeDialog();
                        }



                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.Steed3)]
        public static void Steed3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(728526, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1.");
                return;
            }


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: black, brown or white.")
                           .AddOption("Black horse", 1)
                      .AddOption("Brown horse", 2)
                      .AddOption("White horse", 3)
                      .AddOption("Nevermind", 255)
                      .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(728526, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 255, 0, 150);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(728526, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 0, 150, 255);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(728526, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 150, 255, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Steed6)]
        public static void Steed6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(728527, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: black, brown or white.")
                            .AddOption("Black horse", 1)
                       .AddOption("Brown horse", 2)
                       .AddOption("White horse", 3)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(728527, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 255, 0, 150);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(728527, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 0, 150, 255);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(728527, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 150, 255, 0);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RareSteedPack6)]
        public static void RareSteedPack6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3001406, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a NobleSteedPack");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: SpitfireSteed, FrostbiteSteed, BlazehoofSteed, SpottedSteed or Zebra.")
                            .AddOption("+6~Spitfire~Steed", 1)
                       .AddOption("+6~Frostbite~Steed", 2)
                       .AddOption("+6~Blazehoof~Steed", 3)
                       .AddOption("+6~Spotted~Steed", 4)
                       .AddOption("+6~Zebra", 5)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3001406, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 201, 135, 187);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3001406, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 184, 63, 62);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3001406, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 137, 156, 148);

                        break;
                    }
                case 4:
                    {
                        client.Inventory.Remove(3001406, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 122, 134, 147);

                        break;
                    }
                case 5:
                    {
                        client.Inventory.Remove(3001406, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 46, 39, 142);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NobleSteedPack)]
        public static void NobleSteedPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(729990, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a NobleSteedPack");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello. You can chose a horse: SpitfireSteed, FrostbiteSteed, BlazehoofSteed, SpottedSteed or Zebra.")
                            .AddOption("+6~Spitfire~Steed", 1)
                       .AddOption("+6~Frostbite~Steed", 2)
                       .AddOption("+6~Blazehoof~Steed", 3)
                       .AddOption("+6~Spotted~Steed", 4)
                       .AddOption("+6~Zebra", 5)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(729990, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 201, 135, 187);

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(729990, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 184, 63, 62);

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(729990, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 137, 156, 148);

                        break;
                    }
                case 4:
                    {
                        client.Inventory.Remove(729990, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 122, 134, 147);

                        break;
                    }
                case 5:
                    {
                        client.Inventory.Remove(729990, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 46, 39, 142);

                        break;
                    }
            }
        }       
        [NpcAttribute(NpcID.ExitElitePk)]
        public static void ExitElitePk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.TransformationID != 0)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.OnMyOwnServer == false)
                        {
                            client.Player.InElitePk = false;
                            client.Player.InTeamPk = false;
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Player.InElitePk = true;
                            client.Player.InTeamPk = false;
                            client.Teleport(410, 354, 1002);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FindBoxLeave)]
        public static void FindBoxLeave(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                           .AddOption("Yes.", 3)
                           .AddOption("Let~me~think...", 255)
                           .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {

                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExitSuperGuildWar)]
        public static void ExitSuperGuildWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExitCaptureTheFlag)]
        public static void ExitCaptureTheFlag(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                    .AddOption("TwinCity.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        #region The NPC Edit By Mohy ElDin - Ã™â€¦Ã˜Â­Ã™Å  Ã˜Â§Ã™â€žÃ˜Â¯Ã™Å Ã™â€  ...
        [NpcAttribute(NpcID.JadesKylins)]
        public static void JadesKylins(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        data.AddText("Hello, Through Me You Can Exchange All Your Jade Kylin Until You Reach The Final Level, Which Is Level 15.");
                        data.AddOption("Exchange 2x [P1 Jade Kylin].", 1);
                        data.AddOption("Exchange 2x [P2 Jade Kylin].", 2);
                        data.AddOption("Exchange 2x [P3 Jade Kylin].", 3);
                        data.AddOption("Exchange 2x [P4 Jade Kylin].", 4);
                        data.AddOption("Exchange 2x [P5 Jade Kylin].", 5);
                        data.AddOption("Exchange 2x [P6 Jade Kylin].", 6);
                        data.AddOption("Exchange 2x [P7 Jade Kylin].", 7);
                        data.AddOption("Exchange 2x [P8 Jade Kylin].", 8);
                        data.AddOption("Exchange 2x [P9 Jade Kylin].", 9);
                        data.AddOption("Exchange 2x [P10 Jade Kylin].", 10);
                        data.AddOption("Exchange 2x [P11 Jade Kylin].", 11);
                        data.AddOption("Exchange 2x [P12 Jade Kylin].", 12);
                        data.AddOption("Exchange 2x [P13 Jade Kylin].", 13);
                        data.AddOption("Exchange 2x [P14 Jade Kylin].", 14);
                        data.AddOption("Just passing by.", 255);
                        data.AddAvatar(263).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169805, 2))
                        {

                            client.Inventory.Remove(2169805, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169815, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P2 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P1 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169815, 2))
                        {

                            client.Inventory.Remove(2169815, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169825, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P3 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P2 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169825, 2))
                        {

                            client.Inventory.Remove(2169825, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169835, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P4 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P3 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169835, 2))
                        {

                            client.Inventory.Remove(2169835, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169845, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P5 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P4 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169845, 2))
                        {

                            client.Inventory.Remove(2169845, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169855, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P6 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P5 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169855, 2))
                        {

                            client.Inventory.Remove(2169855, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169865, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P7 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P6 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169865, 2))
                        {

                            client.Inventory.Remove(2169865, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169875, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P8 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P7 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169875, 2))
                        {

                            client.Inventory.Remove(2169875, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169885, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P9 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P8 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169885, 2))
                        {

                            client.Inventory.Remove(2169885, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169895, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P10 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P9 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169895, 2))
                        {

                            client.Inventory.Remove(2169895, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169905, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P11 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P10 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169905, 2))
                        {

                            client.Inventory.Remove(2169905, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169915, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P12 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P11 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169915, 2))
                        {

                            client.Inventory.Remove(2169915, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169925, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P13 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P12 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169925, 2))
                        {

                            client.Inventory.Remove(2169925, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169935, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P14 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P13 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.InTrade) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™ÂÃ˜Â§Ã˜ÂªÃ˜Â­ Ã˜ÂªÃ˜Â±Ã™Å Ã˜Â¯ Ã™â€¦Ã˜Â¹ Ã˜Â´Ã˜Â®Ã˜Âµ Ã˜ÂªÃ˜Â§Ã™â€ Ã™Å 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor) // Ã™â€žÃ™Ë† Ã˜Â§Ã™â€žÃ™â€žÃ˜Â§Ã˜Â¹Ã˜Â¨ Ã™â€šÃ˜Â§Ã˜Â¹Ã˜Â¯ Ã™ÂÃ™Å  Ã˜Â´Ã™Ë†Ã˜Â¨ Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã™Æ’Ã˜Âª 
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(2169935, 2))
                        {

                            client.Inventory.Remove(2169935, 2, stream);
                            if (Role.Core.Rate(100))
                            {
                                client.Inventory.Add(stream, 2169945, 1);
                                client.CreateBoxDialog("Congratulations, You Have Successfully Upgraded and Got 1x [P15 Jade Kylin].");
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Jade Kylin Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P14 Jade Kylin To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        #endregion
        [NpcAttribute(NpcID.SkillTeamPkManager)]
        public static void SkillTeamPkManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (client.IsVendor)
                return;

            if (client.InTrade)
                return;

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("The new Skill Team Pk Tournament is open! It is held at 17:55,Every Saturday")
                            // .AddOption("Tell~me~more~about~the~rules.", 1)
                            .AddOption("Sign up!", 3)
                            .AddOption("Prize!", 5)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("There are limits on the Equipmen Level and Rank. Moreover, there are")
                           .AddText(" restrictions on Equipment Quality, Bonus Level, Sockets, Gems and ")
                           .AddText(" Blessing effects. Besides, the effects of the Sub-classes are ajusted, too.")
                           .AddText(" (Except for Apothecary.) By the way , the Battle Power shared from you Clan, ")
                           .AddText("  Mentor, and Guild won`t count; ")
                           .AddOption("Go ahead.", 10)
                      .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("here Skill Team Pk Tournament is held at 20:00 , every day. You need")
                          .AddText(" to team up with others to take part in it. Only the team leaders are capable of ")
                          .AddText(" claiming the rewards. You can up with friends in advance, or while")
                          .AddText("  (in the waiting area. People who don`t team up will be kicked out, when the")
                          .AddText(" contest starts. There are 4-groups: Level 130+, level 120-129, level 100-199  and level 1-99. ")
                          .AddOption("What else?.", 11)
                     .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("You can talk to me to enter the Waiting area at 21:00. No threams will be allowed to enter ")
                          .AddText(" after the contest start. The contest for level 130+ starts at 21:00, level 120-129 21:02,")
                          .AddText(" level 100-119 20:04 and level 1-99 21:06. Disbanding/creating a team during the contest will")
                          .AddText(" to disqualification. In knockout stage, each round lasts for 10 minutes")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Team == null)
                        {
                            data.AddText("You need to team up with others to take part in it. Please create team.")
                  .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            break;
                        }
                        if (!MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(client))
                        {
                            data.AddText("Please sign up between 19.55 Every Day night for the Skill PK Tournament.")
                            .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!MsgTournaments.MsgSchedules.SkillTeamPkTournament.GetReward(client, stream))
                            {
                                data.AddText("Sorry, you are not in top 8 in Skill Pk Tournament or you claimed the reward.")
                        .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make one space in your bag.")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TeamPkManager)]
        public static void TeamPkManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
           

            if (client.IsVendor)
                return;

            if (client.InTrade)
                return;

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("Attention! The Team Pk Tournament is held at 18:55.")
                            //  .AddOption("Tell~me~more~about~the~rules.", 1)
                            .AddOption("Sign up!", 3)
                            .AddOption("Prize!", 5)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("The Team Pk Tournament start at 19:00 every Saturday, and teams will be divided")
                           .AddText("  into four grups: Level 1-99, Level 100-119, level 120-129 and level 130+ ")
                           .AddText(" Only teams can participate in this Tournament . You can either create a team")
                           .AddText(" befor signin up, or enter the waiting area and create you team. At the start of the")
                           .AddText(" Tournament, any player who`s not in team will be removed from the tournament map.")
                           .AddText("  Where the Tournament ends, only the team leaders of the winning teams can claim the reward.")
                           .AddOption("Ok Thank you!", 255)
                      .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }

                case 3:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Team == null)
                        {
                            data.AddText("You need to team up with others to take part in it. Please create team.")
                  .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            break;
                        }
                        if (!MsgTournaments.MsgSchedules.TeamPkTournament.SignUp(client))
                        {
                            data.AddText("Please sign up at 18:55 every Saturday night for the Team PK Tournament.")
                            .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!MsgTournaments.MsgSchedules.TeamPkTournament.GetReward(client, stream))
                            {
                                data.AddText("Sorry, you are not in top 8 in Team Pk Tournament or you claimed the reward.")
                        .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make one space in your bag.")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ElitePk)]
        public static void ElitePk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            

            if (client.IsVendor)
                return;

            if (client.InTrade)
                return;

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("Glad to see you! I take it that you haven`t heard of the Elite PK Tournament, have you? No? Aha, please allow me to introduce this exiting event for you! It`s a new ")
                           .AddText("PK Tournament that takes place every Day. You will be divided into four groups, based")
                            // .AddOption("Tell~me~more~about~the~rules.", 1)
                           .AddOption("Sign up", 3)
                           .AddOption("Claim Prize", 5)
                      .AddOption("Thanks.", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("The Elite PK Tournament begins at 20:00 every Friday night, and players will be divided into four groups, which are the Under 100 group, the 100-119 group, the 120-129 group, and the 130+ group. You can sign up for it")
                                                 .AddText("~between 19:55 and 19:59. No admittance after the tournament begins. Players should first wait in the Waiting Area, and if you leave the area before your round, you will forfeit your chance. There will be a Knockout round before the Final.")
                                                 .AddText("~But if there are less than 8 players at the start of the Elite PK Tournament, the Final will be replaced by the Knockout. Each round in the Knockout lasts 3 minutes. At the end of 3 minutes, if no one is killed, the player with a higher score wins.")
                                                 .AddText("~After you finish your round, you`ll have to wait for all the other players to finish this round to enter the next round.")
                                                 .AddOption("There~is~a~Final?", 2).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("Sure. When there are less than 8 players left in the field, the Final starts. Each round of the Final lasts for 5 minutes. Though all of the Top 8 players will receive nice prizes, the top 3 will earn special prizes! Claim your reward before the next")
                                .AddText("~week`s Elite PK Tournament begins, or you won`t get them. We hope everyone will join this tournament, and so not only will the top 3 winners of each group receive nice prizes, you will also receive EXP worth 1 EXP Ball and 1000 Honor")
                                .AddText("~points for each match you take part in at the end of that match!")
                                .AddOption("Sign~me~up!", 3).AddOption("Not~interested", 255).AddAvatar(154).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (!MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(client))
                        {
                            // data.AddText("Please sign up between 20.55 every day night for the Elite PK Tournament.")
                            // .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!MsgTournaments.MsgSchedules.ElitePkTournament.GetReward(client, stream))
                            {
                                data.AddText("Sorry, you are not in top 8 in elite pk or you claimed the reward.")
                        .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your bag.")
                          .AddOption("Okay.", 255).AddAvatar(154).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ClassPkWar)]
        public static void ClassPkWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Class PK War Starts Monday At Hour 19.00")
                            .AddOption("Sign~me~up!", 1)
                            .AddOption("Not now.", 255).AddAvatar(107).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        MsgTournaments.MsgClassPKWar.War war;
                        if (Game.MsgTournaments.MsgSchedules.ClassPkWar.GetWar(client, out war) != MsgTournaments.ProcesType.Dead)
                        {
                            if (!war.CheckJoin())
                            {
                                if (war.IsFinish(client))
                                {
                                    if (!client.Inventory.HaveSpace(4))
                                    {
                                        data.AddText("Please make 4 more spaces in your inventory.")
                                            .AddOption("Let me check.", 255).AddAvatar(107).FinalizeDialog();

                                    }
                                    war.GetMyReward(client, stream);

                                }
                                else
                                {
                                    data.AddText("Sorry, the war is not finished, or other players are still alive.")
                   .AddOption("Okay.", 255).AddAvatar(107).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, The War is not finished.")
                    .AddOption("Okay.", 255).AddAvatar(107).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, the war is finished, or other players are still alive.")
           .AddOption("Okay.", 255).AddAvatar(107).FinalizeDialog();
                        }

                        break;

                    }
            }
        }
        [NpcAttribute(NpcID.ClassPkEnvoy)]
        public static void ClassPkEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Everyone settle down! I need to announce that there are new rules for the Class PK War!")
                            .AddOption("Tell~me~more.", 1)
                       .AddOption("Sign~up.", 2)
                       .AddOption("Not~interested.", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("The Class PK War for each class starts from 19:30 and ends at 20:30, every Monday. Each round can last up to 1 hour. Since I won`t accept any applications once the contest starts, you need to be sure and sign up 15 minutes before it begins. Players from each class will be divided")
                            .AddText(" into 4 groups, that is, Level 1-99, 100-119, 120-129 and 130+. The only survivor should talk to the Class PK Escort to claim the experience worth 5 EXP Balls. However, if there are 2 or more players left when time is up, the contest will be closed and no one will receive the grand prize. Would you like to sign up for the battle")
                           .AddOption("Sure. ", 2)
                              .AddOption("Later. ", 255).AddAvatar(154).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        MsgTournaments.MsgClassPKWar.War war;
                        if (Game.MsgTournaments.MsgSchedules.ClassPkWar.GetWar(client, out war) != MsgTournaments.ProcesType.Dead)
                        {
                            if (war.CheckJoin())
                            {
                                client.Teleport(76, 63, Game.MsgTournaments.MsgClassPKWar.MapID, war.DinamicID);
                            }
                            else
                            {
                                data.AddText("The current Class PK War is only open for Trojans, Warriors, Archers,        Water/Fire Taoists, Ninjas, Monks, Pirates and Dragon Warriors. If you`re not one of them, go talk to the class trainer!")
                  .AddOption("Alright.", 255).AddAvatar(154).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The current Class PK War is only open for Trojans, Warriors, Archers,        Water/Fire Taoists, Ninjas, Monks, Pirates and Dragon Warriors. If you`re not one of them, go talk to the class trainer!")
                     .AddOption("Alright.", 255).AddAvatar(154).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.NemesysConductort)]
        public static void NemesysConductort(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("With only 100 silver, I can teleport you to somewhere far away. Please choose your destination.")
                           .AddOption("Desert~City.", 1)
                            //  .AddOption("Market.", 2)
                      .AddOption("Just~passing~by.", 255).AddAvatar(1).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(502, 646, 1000);
                        }
                        else
                        {
                            data.AddText("You do not have 100 Silvers with you.")
   .AddOption("I~see", 255).AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(200, 204, 1036);
                        }
                        else
                        {
                            data.AddText("You do not have 100 Silvers with you.")
   .AddOption("I~see", 255).AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        public static void SpaceMarks(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {

                            data.AddText("I`m sell WindSpell for 1.000 Silver. Use that to teleport back to me")
                         .AddOption("Ok give me.", 1).AddOption("No thanks.", 255).AddAvatar(3).FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Only Water class can buy this teleport scroll.")
                           .AddOption("I see.", 255).AddAvatar(3).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            if (client.Player.Money >= 1000)
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Player.Money -= 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    switch (id)
                                    {
                                        case 10031: client.Inventory.Add(stream, 1060025); break;
                                        case 10032: client.Inventory.Add(stream, 1060026); break;
                                        case 10033: client.Inventory.Add(stream, 1060027); break;
                                        case 10034: client.Inventory.Add(stream, 1060028); break;
                                        case 10035: client.Inventory.Add(stream, 1060029); break;
                                        case 10036: client.Inventory.Add(stream, 1060038); break;
                                        case 10037: client.Inventory.Add(stream, 1060031); break;
                                        case 10038: client.Inventory.Add(stream, 1060032); break;
                                        case 10039: client.Inventory.Add(stream, 1060033); break;
                                        case 10040: client.Inventory.Add(stream, 1060034); break;
                                        case 10041: client.Inventory.Add(stream, 1060035); break;
                                        case 10043: client.Inventory.Add(stream, 1060037); break;

                                    }
                                }
                                else
                                {
                                    data.AddText("Please make space in you inventory")
          .AddOption("Ah~thank~you!.", 255)
          .AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You do not have 1000 Silvers with you.")
            .AddOption("I~see", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }

                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.JailCPAdmin)]
        //public static void JailCPAdmin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    return;
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Do~you~know~how~I~desire~the~DragonBall?~I~would~like~to~pay~all~I~have~for~DragonBalls.~Would~you~like~to~exchange")
        //                    .AddText("DragonBalls~for~CPs?~I~will~pay~you~215~CPs~for~a~DragonBall,~and~2150~CPs~for~a~DBScroll.~What~about~the~deal?")
        //                    .AddOption("Exchange~a~DBScroll.", 1)
        //               .AddOption("Exchange~a~DragonBall.", 2)
        //               .AddOption("Exchange~a~Money~to~ConquerPoints.", 3)
        //               .AddOption("I~have~nothing.", 255).AddAvatar(7).FinalizeDialog();


        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Inventory.Contain(Database.ItemType.DBScroll, 1))
        //                {
        //                    client.Inventory.Remove(Database.ItemType.DBScroll, 1, stream);
        //                    client.Player.ConquerPoints += 2150;

        //                    client.CreateBoxDialog("Congratulations!You~have~got~2150~CPs~in~your~inventory");
        //                }
        //                else
        //                {
        //                    data.AddText("Where~is~your~DBScroll?")
        //                  .AddOption("Er,~sorry.~I'm not have DBScroll.", 255).AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
        //                {
        //                    client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
        //                    client.Player.ConquerPoints += 215;

        //                    client.CreateBoxDialog("Congratulations!You~have~got~215~CPs~in~your~inventory");
        //                }
        //                else
        //                {
        //                    data.AddText("Where~is~your~DragonBall?")
        //                  .AddOption("Er,~sorry.~I'm not have Dragon Ball.", 255).AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 3:
        //            {
        //                data.AddText("You wana to change 10.000.000 Money for 100 ConquerPoints? or 100.000.000 Money for 1000 ConquerPoints?")
        //      .AddOption("Change 10kk for 100", 4)
        //      .AddOption("Change 100kk for 1000", 5)
        //      .AddOption("No Thanks", 255).AddAvatar(7).FinalizeDialog();

        //                break;
        //            }
        //        case 4:
        //            {
        //                if (client.Player.Money >= 10000000)
        //                {
        //                    client.Player.Money -= 10000000;
        //                    client.Player.ConquerPoints += 100;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);

        //                    client.CreateBoxDialog("Congratulations!You~have~got~100~CPs~in~your~inventory");
        //                }
        //                else
        //                {
        //                    data.AddText("You do not have 10.000.000 Money with you.")
        //                     .AddOption("I~see", 255).AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 5:
        //            {
        //                if (client.Player.Money >= 100000000)
        //                {
        //                    client.Player.Money -= 100000000;
        //                    client.Player.ConquerPoints += 1000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);

        //                    client.CreateBoxDialog("Congratulations!You~have~got~1000~CPs~in~your~inventory");
        //                }
        //                else
        //                {
        //                    data.AddText("You do not have 100.000.000 Money with you.")
        //                     .AddOption("I~see", 255).AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.WardenZhang)]
        public static void WardenZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Were always in the need of fresh bounty hunters.You see... murderers and thieves can evade out guards.")
                            .AddText("We need heroes to slay he killers and restore peace. If you can PK those whose names are black (known criminals),")
                            .AddText("you might get rewarded. If they pay")
                            .AddOption("I~want~to~claim~my~reward.", 1)
                       .AddOption("None~of~my~business.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        Database.TheCrimeTable MyCrimes;
                        if (Pool.TheCrimePoll.TryGetValue(client.Player.UID, out MyCrimes))
                        {
                            string KillerName = "";
                            int ConquerPoints = MyCrimes.Claim(out KillerName);
                            if (ConquerPoints > 0)
                            {
                                data.AddText("For arrest " + KillerName + ", you will be awarded " + ConquerPoints + " ConquerPoints.")
                   .AddOption("Ok, I see.", 255).AddAvatar(37).FinalizeDialog();
                                client.Player.ConquerPoints += (uint)ConquerPoints;// QahrMobile: the arrest reward was taken away instead of paid

                                break;
                            }
                        }

                        data.AddText("Good, good you have slain a murderer. Yes.. but only if he pays bail can you claim the bounty.")
                      .AddText("Its the way things are done! Heres a bit of advice, hehehe... Kill the rich, forget the poor. Ha ha Harrr!")
                      .AddText("Thats called makeing  a living!")
                 .AddOption("Despicable!~But~realistic!", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JailWarden)]
        public static void JailWarden(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Lucky~for~you,~the~player~who~killed~you~is~willing~to~drop~the~murder~charges~if~you~pay~him~according~to~what~you~did.")
                            .AddText("~So~if~you~pay~him~properly~he~is~willing~to~drop~the~murder~charges.")
                            .AddOption("Ok!~Let~me~get~out~from~here.", 1)
                            .AddOption("Any~chance~for~discount?", 2)
                       .AddOption("I~would~rather~rot~in~hell!", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("The~red~name,~whose~PK~points~are~30-99,~can~leave~here~without~any~price.~The~black~name,~whose~PK~points~are~100~")
                            .AddText("or~above,~should~pay~gold~as~recognizance~to~leave~here.~Or~pay~in~CPs~if~they~really~killed~a~lot.")
                            .AddOption("Ok,~I~will~pay.", 1)
                       .AddOption("I~would~rather~stay~here.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.PKPoints < 100)
                        {
                            data.AddText("You~don\'t~need~to~stay~here~any~more.~Let~me~help~you~out")
                       .AddOption("Ok,~thanks.", 3)
                  .AddOption("I~wanna~stay~here.", 255).AddAvatar(37).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            int cost = (int)(5000 + client.Player.PKPoints * 10);
                            data.AddText("You~really~murdered~a~lot.~Well~in~this~case~if~you~pay~for~yours~sins~" + cost.ToString() + "~CPs~I~will~let~you~go.")
         .AddOption("Here~is~my~payment.", 4)
    .AddOption("No,~thanks.", 255).AddAvatar(37).FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        uint cost = (uint)(5000 + client.Player.PKPoints * 10);
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= (uint)cost;


                            if (client.Player.MyKillerUID > 1000000)
                            {
                                Database.TheCrimeTable crime;
                                if (Pool.TheCrimePoll.TryGetValue(client.Player.MyKillerUID, out crime))
                                {
                                    crime.AddCrime(client.Player.Name, client.Player.UID, (int)cost);
                                }
                                else
                                {
                                    crime = new Database.TheCrimeTable();
                                    crime.AddCrime(client.Player.Name, client.Player.UID, (int)cost);
                                    Pool.TheCrimePoll.TryAdd(client.Player.MyKillerUID, crime);
                                }
                                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulation " + client.Player.Name + " rewarded " + client.Player.MyKillerName + " with " + cost.ToString() + " ConquerPoints compensation for his crimes!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            }
                            ushort[] Maps = new ushort[] { 1002, 1000, 1011, 1015 };

                            byte Rand = (byte)Pool.GetRandom.Next(0, Maps.Length);
                            switch (Rand)
                            {
                                case 0: client.Teleport(386, 662, 1011); break;
                                case 1: client.Teleport(294, 505, 1000); break;
                                case 2: client.Teleport(410, 354, 1002); break;
                                default: client.Teleport(303, 379, 1015); break;
                            }
                        }
                        else
                        {
                            data.AddText("You do not have " + cost.ToString() + " ConquerPoints with you.")
                             .AddOption("I~see", 255).AddAvatar(37).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JailJoin)]
        public static void JailJoin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What~can~I~do~for~you?")
                            .AddOption("Visit~the~jail.", 1)
                       .AddOption("Just~passing~by.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Give~me~1000~silver,~I~will~teleport~you~there.")
                                 .AddOption("Here~are~1000~silver.", 2)
                            .AddOption("If~so,~I~will~stay~here.", 255).AddAvatar(37).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.PKPoints < 30)
                        {
                            if (client.Player.Money > 1000)
                            {
                                client.Player.Money -= 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                client.Teleport(29, 72, 6000);

                            }
                            else
                            {
                                data.AddText("You do not have 1000 silvers with you.")
                               .AddOption("I~see", 255).AddAvatar(37).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I'm sorry you have more Pkpoints")
                             .AddOption("I~see", 255).AddAvatar(37).FinalizeDialog();
                        }

                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.MarialDealer)]
        public static void MarialDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.MyJiangHu == null)
            {
                data.AddText("Martial Dealer:\n");
                data.AddText("Listen, you can`t acquire any Skill Souls if you have`t increased you Jiang Hu Score to Level 9, Keep practicing!")
             .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                return;
            }
            if (client.Player.MyJiangHu.GetGrade() < 9)
            {
                data.AddText("Martial Dealer:\n");
                data.AddText("Listen, you can`t acquire any Skill Souls if you have`t increased you Jiang Hu Score to Level 9, Keep practicing!")
                                    .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                return;
            }

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Martial Dealer:\n");
                        data.AddText("Want your skills to be extraordinary? Skill Souls can make it happen! You can pay 2700 CPs to buy a Skill Soul Book, or use 20 Big Permanent Stones to buy a Super Skill Soul Book! If you lose a skill soul after rebirth, you can just retrieve it from me for free! Anyway, I would recommend you the Super Skill Soul Book. With this, you`ll have all Super Skill Souls once and for all!")
                            .AddOption("Upgrade My Skill.", 254)
                       .AddOption("I`ll talk to you later.", 255).AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 254:
                    {
                        data.AddText("Martial Dealer:\n");
                        data.AddText("You can buy your skills in exchange for CPs without needing the books on the skills. One skill is worth 2700 CPs So that you can update your skills Do you want to update your skills now?")
                            .AddOption("I want to Upgrade skills.", 70)
                       .AddOption("I`ll talk to you later.", 255).AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 70:
                    {
                        data.AddText("Martial Dealer:\n");
                        data.AddText("Choose the skill you want to update It will cost you 2700 CPs For one skill So that you can update it and own the skill's skill soul Her own.")
                        .AddOption("(ViperFang).", 1)
                        .AddOption("(FireofHell).", 2)
                        .AddOption("(ToxicFog).", 3)
                       .AddOption("(TwofoldBlades).", 4)
                       .AddOption("(SuperTwofoldBlades).", 200)
                       .AddOption("(BlisteringWave).", 5)
                        .AddOption("(Tornado).", 6)
                       .AddOption("(WhirlwindKick).", 7)
                       .AddOption("Next Page.", 8).AddAvatar(85).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        data.AddText("Martial Dealer:\n");
                        data.AddText("Choose the skill you want to update It will cost you 2700 CPs For one skill So that you can update it and own the skill's skill soul Her own.")
                        .AddOption("(BladeTempest).", 9)
                        .AddOption("(DaggerStorm).", 10)
                        .AddOption("(RadiantPalm).", 11)
                        .AddOption("(ScentSword).", 12)
                       .AddOption("(ChargingVortex).", 13)
                       .AddOption("(DragonTail).", 14)
                        .AddOption("Previous page.", 70)
                       .AddOption("Next Page.", 16).AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        data.AddText("Martial Dealer:\n");
                        data.AddText("Choose the skill you want to update It will cost you 2700 CPs For one skill So that you can update it and own the skill's skill soul Her own.")
                       .AddOption("(Snow).", 15)
                       .AddOption("(BloodyScythe).", 17)
                       .AddOption("(FastBlade).", 18)
                       .AddOption("(Phoenix).", 19)
                        .AddOption("(GaleBomb).", 20)
                       .AddOption("(Hercules).", 21)
                       .AddOption("Previous page.", 8).AddAvatar(85).FinalizeDialog();


                        break;
                    }
                case 200:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.SuperTwofoldBlade;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 21:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Hercules;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9)
                            GetMaxLevel = 1;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.GaleBomb;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 19:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Phoenix;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 18:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.FastBlader;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 17:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.BloodyScythe;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Snow;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 14:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.DragonTail;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 13:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ChargingVortex;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade == 14)
                            GetMaxLevel = 2;
                        if (mygrade == 15)
                            GetMaxLevel = 3;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ScrenSword;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 12)
                            GetMaxLevel = 1;
                        if (mygrade == 13)
                            GetMaxLevel = 2;
                        if (mygrade == 14) GetMaxLevel = 3;
                        if (mygrade == 15) GetMaxLevel = 4;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.RadiantPalm;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.DaggerStorm;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 9:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.BladeTempest;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 7:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.WhirlwindKick;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade == 14)
                            GetMaxLevel = 2;
                        if (mygrade == 15)
                            GetMaxLevel = 3;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;

                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.Tornado;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.BlisteringWave;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 4:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.TwofoldBlades;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        //if (mygrade > 13)
                        //    GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ToxicFog;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.ViperFang;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        byte GetMaxLevel = 0;
                        byte mygrade = client.Player.MyJiangHu.GetGrade();
                        if (mygrade >= 9 && mygrade <= 13)
                            GetMaxLevel = 1;
                        if (mygrade > 13)
                            GetMaxLevel = 2;
                        uint cost = 0;
                        if (GetMaxLevel == 1)
                            cost = 2700;
                        else if (GetMaxLevel > 1)
                            cost = 2700;
                        if (client.Player.ConquerPoints >= cost)
                        {
                            if (client.InTrade) return;
                            Role.Flags.SpellID SpellID = Role.Flags.SpellID.FireofHell;
                            if (client.UpdateSpellSoul(stream, SpellID, GetMaxLevel))
                            {
                                client.Player.ConquerPoints -= (uint)cost;
                                data.AddText("Martial Dealer:\n");
                                data.AddText("The spell " + SpellID.ToString() + " was updated successfully.")
                                      .AddOption("Thank you.", 255).AddAvatar(85).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Martial Dealer:\n");
                            data.AddText("Sorry you don`t have " + cost.ToString() + " CPs in your inventory to update your skill.")
                                   .AddOption("Alright.", 255).AddAvatar(85).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SarckSoldier)]
        public static void SarckSoldier(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Tell you what, there`s a monster called Nemesis Tyrant hiding at the bottom of the Bloodshed Sea! No one has come back alive after seeing it!")
                            .AddText("~It`s said that anyone who kills this goddamned monster will receive a Savage Bone, which can be used to explored the P7 Sacred Treasures!")
                            .AddOption("Send~me~to~the~Bloodshed~Sea!", 1)
                       .AddOption("Holy...So~scary!", 255).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.CreateBoxDialog("You~arrived~at~the~Bloodshed~Sea.");
                        client.Teleport(381, 271, 3846);


                        break;
                    }
            }
        }
        public static void InvokeSarckSoldier(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is where the Nemesis Tyrant is hiding. I can`t recall the number of the people who came here. I just remember how they got killed. If you wanna")
                            .AddText("~talk about it, tell me any time.")
                            .AddOption("Tell~me~about~the~sea.", 1)
                       .AddOption("I~wanna~buy~some~Sea~Potions.", 2)
                       .AddOption("God!~Send~me~back!", 3)
                    .AddOption("Alright.", 255).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("This is where the Nemesis Tyrant is hiding. I can`t recall the number of the people who came here. I just remember how they got killed. If you wanna")
                           .AddText("~talk about it, tell me any time.")
                           .AddOption("Tell~me~about~the~sea.", 1)
                      .AddOption("I~wanna~buy~some~Sea~Potions.", 2)
                      .AddOption("God!~Send~me~back!", 3)
                   .AddOption("Alright.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Well, this sea is kinda special. You won`t get PK points and can`t use City Gates. The Nemesis Tyrant will show up at 15 and 45 of each hour. Look at you...")
                        .AddText("~Before it shows up, you`ll find some Gold Boxes! Open the boxes, you`ll receive some Sea Potions and learn the position of the monster in advance!")
                        .AddText("~Oh, you can also get the potions if you defeat the GameServer Captain!")
                        .AddOption("Tell~me~something~else.", 4)
                   .AddOption("Got~you.", 255).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You~arrived~at~the~Gale~Shallow.");
                        client.Teleport(62, 89, 3055);

                        break;
                    }
                case 2:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            NpcID = client.ActiveNpc, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,
                            dwParam3 = client.Player.Map
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.SarckSoldierEnctrance)]
        public static void SarckSoldierEnctrance(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint1)]
        public static void SarckSoldierSuppluPoint1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint2)]
        public static void SarckSoldierSuppluPoint2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint3)]
        public static void SarckSoldierSuppluPoint3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint4)]
        public static void SarckSoldierSuppluPoint4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        [NpcAttribute(NpcID.SarckSoldierSuppluPoint5)]
        public static void SarckSoldierSuppluPoint5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            InvokeSarckSoldier(client, stream, Option, Input, id);
        }
        //        [NpcAttribute(NpcID.SelectP7EquipmentSoulPack)]
        //        public static void SelectP7EquipmentSoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //            if (!client.Inventory.Contain(3004248, 1))
        //            {
        //                client.CreateBoxDialog("Sorry, you don`t have a P7EquipmentSoulPack.");
        //                return;
        //            }
        //
        //
        //            Dialog data = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 40:
        //                    {
        //                        data.AddText("Select the p7 Equipment you need.")
        //                            .AddOption("MoonHeadgear", 1)
        //                       .AddOption("SunHeadgear", 2)
        //                       .AddOption("StarHeadgear", 3)
        //                       .AddOption("IceHeadgear", 4)
        //                       .AddOption("HeavenNecklace", 5)
        //                       .AddOption("FervorBag", 6)
        //                       .AddOption("NetherArmor", 7)
        //                    .AddOption("Next~Page", 8).FinalizeDialog();
        //
        //
        //                        break;
        //                    }
        //                case 8:
        //                    {
        //                        data.AddText("Select the p7 Equipment you need.")
        //                            .AddOption("EclipseArmor", 9)
        //                       .AddOption("CraneRing", 10)
        //                       .AddOption("DragonRing", 11)
        //                       .AddOption("RainbowBracelet", 12)
        //                       .AddOption("LionHeavyRing", 13)
        //                       .AddOption("TigerHeavyRing", 14)
        //                       .AddOption("FoxBoots", 15)
        //                       .AddOption("DragonBoots", 16)
        //                       .AddOption("CraneBoots", 17)
        //                    .AddOption("Back~Page", 40).FinalizeDialog();
        //
        //
        //
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 820073, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 820074, 1);
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 820075, 1);
        //                        break;
        //                    }
        //                case 4:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 820076, 1);
        //                        break;
        //                    }
        //                case 5:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 821033, 1);
        //                        break;
        //                    }
        //                case 6:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 821034, 1);
        //                        break;
        //                    }
        //                case 7:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 822071, 1);
        //                        break;
        //                    }
        //                case 9:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 822072, 1);
        //                        break;
        //                    }
        //                case 10:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 823058, 1);
        //                        break;
        //                    }
        //                case 11:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 823059, 1);
        //                        break;
        //                    }
        //                case 12:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 823060, 1);
        //                        break;
        //                    }
        //                case 13:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 823061, 1);
        //                        break;
        //                    }
        //                case 14:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 823062, 1);
        //                        break;
        //                    }
        //                case 15:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 824018, 1);
        //                        break;
        //                    }
        //                case 16:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 824019, 1);
        //                        break;
        //                    }
        //                case 17:
        //                    {
        //                        client.Inventory.Remove(3004248, 1, stream);
        //                        client.Inventory.Add(stream, 824020, 1);
        //                        break;
        //                    }
        //            }
        //        }
        //        [NpcAttribute(NpcID.SelectP7WeaponSoulPack)]
        //        public static void SelectP7WeaponSoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //            if (!client.Inventory.Contain(3004247, 1))
        //            {
        //                client.CreateBoxDialog("Sorry, you don`t have a P7WeaponSoulPack.");
        //                return;
        //            }
        //
        //
        //            Dialog data = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 20:
        //                    {
        //                        data.AddText("Select the p7 weapon you need.")
        //                            .AddOption("MonsterSaber", 1)
        //                       .AddOption("SkyHammer", 2)
        //                       .AddOption("ShadowKatana", 3)
        //                       .AddOption("SkyHalberd", 4)
        //                       .AddOption("DemonScythe", 5)
        //                       .AddOption("SpiritShield", 6)
        //                       .AddOption("TimeBacksword", 7)
        //                    .AddOption("Next~Page", 8).FinalizeDialog();
        //
        //
        //                        break;
        //                    }
        //                case 0:
        //                    {
        //                        data.AddText("Select the p7 weapon you need.")
        //                            .AddOption("MonsterSaber", 1)
        //                       .AddOption("SkyHammer", 2)
        //                       .AddOption("ShadowKatana", 3)
        //                       .AddOption("SkyHalberd", 4)
        //                       .AddOption("DemonScythe", 5)
        //                       .AddOption("SpiritShield", 6)
        //                       .AddOption("TimeBacksword", 7)
        //                    .AddOption("Next~Page", 8).FinalizeDialog();
        //
        //                        break;
        //                    }
        //                case 8:
        //                    {
        //                        data.AddText("Selecte the p7 weapon you need.")
        //                      .AddOption("SunBow", 9)
        //                 .AddOption("BuddaBeads", 10)
        //                 .AddOption("DeathPistol", 11)
        //                 .AddOption("RepentRapier", 12)
        //                 .AddOption("GhostKnife", 13)
        //                  .AddOption("WarCraze(Nunchaku)", 14)
        //                   .AddOption("WonderHossu", 15)
        //                  .AddOption("Back~Page", 20)
        //                  .AddOption("Next~Page", 21).FinalizeDialog();
        //
        //                        break;
        //                    }
        //                case 21:
        //                    {
        //                        data.AddText("Selecte the p7 weapon you need.")
        //                      .AddOption("FistofDeity(EpicWarrior)", 16)
        //                 .AddOption("FistofDemon(EpicWarrior)", 17)
        //                  .AddOption("DivineFan(WindWalker)", 18)
        //                     .AddOption("DivineShockwave(Thunderstriker-Stormhammer)", 25)
        //                     .AddOption("DivineShockwave(Thunderstriker-Flashaxe)", 26)
        //                        .AddOption("DivineVoidSplitter(Thunderstriker-Stormhammer)", 27)
        //                        .AddOption("DivineVoidSplitter(Thunderstriker-Flashaxe)", 28)
        //                                       .AddOption("Soul Hunter", 29)//EpicArcher 800633
        //                  .AddOption("Back~Page", 8).FinalizeDialog();
        //                        break;
        //                    }
        //                case 29:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800633, 1);
        //                        break;
        //                    }
        //                case 25:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 827010, 1);
        //                        break;
        //                    }
        //                case 26:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 827011, 1);
        //                        break;
        //                    }
        //                case 27:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 827012, 1);
        //                        break;
        //                    }
        //                case 28:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 827013, 1);
        //                        break;
        //                    }
        //                case 18:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 801308, 1);
        //                        break;
        //                    }
        //                case 16:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 801216, 1);
        //                        break;
        //                    }
        //                case 17:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 801218, 1);
        //                        break;
        //                    }
        //
        //                case 14:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 801004, 1);
        //                        break;
        //                    }
        //                case 15:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 801104, 1);
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800020, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800111, 1);
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800142, 1);
        //                        break;
        //                    }
        //                case 4:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800215, 1);
        //                        break;
        //                    }
        //                case 5:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800255, 1);
        //                        break;
        //                    }
        //                case 6:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800422, 1);
        //                        break;
        //                    }
        //                case 7:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800522, 1);
        //                        break;
        //                    }
        //                case 9:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800618, 1);
        //                        break;
        //                    }
        //                case 10:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800725, 1);
        //                        break;
        //                    }
        //                case 11:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800810, 1);
        //                        break;
        //                    }
        //                case 12:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800811, 1);
        //                        break;
        //                    }
        //                case 13:
        //                    {
        //                        client.Inventory.Remove(3004247, 1, stream);
        //                        client.Inventory.Add(stream, 800917, 1);
        //                        break;
        //                    }
        //            }
        //        }
        //        [NpcAttribute(NpcID.ThunderGloryGem)]
        //        public static void ThunderGloryGem(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //           
        //            Dialog dialog = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 0:
        //                    {
        //                        dialog.Text("Hello there, Choose items.\n");
        //                        dialog.Option("Super Thunder Gem.", 1);
        //                        dialog.Option("Super Glory Gem.", 2);
        //                        dialog.Option("I~see.", 255).FinalizeDialog();
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3311693, 1, stream);
        //                        client.Inventory.Add(stream, 700103, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3311693, 1, stream);
        //                        client.Inventory.Add(stream, 700123, 1);
        //                        break;
        //                    }
        //                   
        //            }
        //        }
        //        public static void DragonTacticsAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //           
        //            Dialog dialog = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 0:
        //                    {
        //                        dialog.Text("Hello there, i can sell you cool items for cool prices.\n");
        //                        dialog.Option("Dragon~Tactics~(A).", 1);
        //                        dialog.Option("Dragon~Tactics~(B).", 2);
        //                        dialog.Option("I~see.", 255).FinalizeDialog();
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3321817, 1, stream);
        //                        client.Inventory.Add(stream, 3005397, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3321817, 1, stream);
        //                        client.Inventory.Add(stream, 3005398, 1);
        //                        break;
        //                    }
        //
        //            }
        //        }
        //        public static void BoundlessHeartAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //
        //            Dialog dialog = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 0:
        //                    {
        //                        dialog.Text("Hello there, i can sell you cool items for cool prices.\n");
        //                        dialog.Option("Boundless~Heart~(A).", 1);
        //                        dialog.Option("Boundless~Heart~(B).", 2);
        //                        dialog.Option("Boundless~Heart~(C).", 3);
        //                        dialog.Option("I~see.", 255).FinalizeDialog();
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3321818, 1, stream);
        //                        client.Inventory.Add(stream, 3007115, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3321818, 1, stream);
        //                        client.Inventory.Add(stream, 3007116, 1);
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        client.Inventory.Remove(3321818, 1, stream);
        //                        client.Inventory.Add(stream, 3007117, 1);
        //                        break;
        //                    }
        //
        //            }
        //        }
        //        public static void DoctrineofDeityAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //
        //            Dialog dialog = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 0:
        //                    {
        //                        dialog.Text("Hello there, i can sell you cool items for cool prices.\n");
        //                        dialog.Option("Doctrine~of~Deity~(A).", 1);
        //                        dialog.Option("Doctrine~of~Deity~(B).", 2);
        //                        dialog.Option("Doctrine~of~Deity~(C).", 3);
        //                        dialog.Option("I~see.", 255).FinalizeDialog();
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3321819, 1, stream);
        //                        client.Inventory.Add(stream, 3005399, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3321819, 1, stream);
        //                        client.Inventory.Add(stream, 3005400, 1);
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        client.Inventory.Remove(3321819, 1, stream);
        //                        client.Inventory.Add(stream, 3005401, 1);
        //                        break;
        //                    }
        //
        //            }
        //        }
        //        public static void PuzzleofLifeAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //
        //            Dialog dialog = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 0:
        //                    {
        //                        dialog.Text("Hello there, i can sell you cool items for cool prices.\n");
        //                        dialog.Option("Puzzle~of~Life~(A).", 1);
        //                        dialog.Option("Puzzle~of~Life~(B).", 2);
        //                        dialog.Option("Puzzle~of~Life~(C).", 3);
        //                        dialog.Option("I~see.", 255).FinalizeDialog();
        //                        break;
        //                    }
        //                case 1:
        //                    {
        //                        client.Inventory.Remove(3321820, 1, stream);
        //                        client.Inventory.Add(stream, 3007230, 1);
        //                        break;
        //                    }
        //                case 2:
        //                    {
        //                        client.Inventory.Remove(3321820, 1, stream);
        //                        client.Inventory.Add(stream, 3007231, 1);
        //                        break;
        //                    }
        //                case 3:
        //                    {
        //                        client.Inventory.Remove(3321820, 1, stream);
        //                        client.Inventory.Add(stream, 3007232, 1);
        //                        break;
        //                    }
        //
        //            }
        //        }   
        [NpcAttribute(NpcID.SelectSacredRefineryPack)]
        public static void SelectSacredMaterial(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(3004249, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return;
            }
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                             .AddOption("Boots. ", 205) //1 (Done)
                             .AddOption("Bracelet. ", 209) //2 (Done)
                             .AddOption("Bag. ", 211) //3 (Done) 
                             .AddOption("Necklace. ", 212) //4 (Done)
                             .AddOption("Headgear. ", 213) //5 (Done)
                             .AddOption("Armor. ", 214) //6 (Done)
                             .AddOption("Next.", 201)
                             .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 200:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                             .AddOption("Boots. ", 205) //1 (Done)
                             .AddOption("Bracelet. ", 209) // 2 (Done)
                             .AddOption("Bag. ", 211) //3 (Done)
                             .AddOption("Necklace. ", 212) //4 (Done)
                             .AddOption("Headgear. ", 213) //5 (Done)
                             .AddOption("Armor. ", 214) //6 (Done)
                             .AddOption("Next.", 201)
                             .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 201:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                             .AddOption("2-handed weapon. ", 215)
                             .AddOption("1-handed weapon. ", 216)
                             .AddOption("Shield. ", 217)
                             .AddOption("Backsword. ", 218)
                             .AddOption("Hossu. ", 219)
                             .AddOption("Bow. ", 220)
                             .AddOption("Ring.", 221)
                             .AddOption("Previous.", 200)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 205:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                            .AddOption("Immunity (Sacred) Material. ", 27) // Done
                            .AddOption("Detoxication (Sacred) Material. ", 207) // Done
                            .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 209:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                           .AddOption("Breakthrough (Sacred) Material. ", 34) // Done
                           .AddOption("Penetration (Sacred) Material. ", 37) // Done
                           .AddOption("M-Defense (Sacred) Material. ", 18) // Done
                           .AddOption("SkillC.Strike (Sacred) Material. ", 29) // Done
                           .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 211:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                            .AddOption("Detoxication (Sacred) Material. ", 32) // Done
                            .AddOption("Penetration (Sacred) Material. ", 22) // Done
                            .AddOption("Counteraction (Sacred) Material.", 25) // Done
                            .AddOption("M-Defense (Sacred) Material. ", 41) // Done
                            .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 212:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                           .AddOption("Detoxication (Sacred) Material. ", 33) // Done
                           .AddOption("Counteraction (Sacred) Material. ", 15) // Donne
                           .AddOption("M-Defense (Sacred) Material.", 87) // Done
                           .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 213:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                          .AddOption("Penetration (Sacred) Material. ", 42) // Done
                          .AddOption("Detoxication (Sacred) Material. ", 44) // Done
                          .AddOption("Block (Sacred) Material. ", 43) // Done
                          .AddOption("Intensification (Sacred) Material. ", 49) // Done
                          .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 214:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                         .AddOption("Detoxication (Sacred) Material. ", 38) // Done
                         .AddOption("Counteraction (Sacred) Material.", 47) // Done
                         .AddOption("Immunity (Sacred) Material. ", 83) // Done
                         .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 215:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                        .AddOption("Breakthrough (Sacred) Material. ", 16) // Done
                        .AddOption("CriticalStrike (Sacred) Material. ", 105) // Done
                        .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 216:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                        .AddOption("Breakthrough (Sacred) Material. ", 48) // Done
                       .AddOption("CriticalStrike (Sacred) Material. ", 106) // Done
                       .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 217:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                         .AddOption("Block (Sacred) Material. ", 23) // Done
                         .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 218:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                          .AddOption("SkillC.Strike (Sacred) Material. ", 88) // Done
                          .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 219:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                        .AddOption("Sacred Hossu Hair. ", 222) // Done
                       .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 220:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                        .AddOption("Breakthrough (Sacred) Material. ", 26) // Done
                        .AddOption("CriticalStrike (Sacred) Material. ", 30) // Done
                        .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }
                case 221:
                    {

                        {
                            data.AddText("Please select a type of refinery material.")
                         .AddOption("Breakthrough (Sacred) Material. ", 36) //4 (Done)
                         .AddOption("M-Defense (Sacred) Material. ", 20) // Done
                         .AddOption("CriticalStrike (Sacred) Material. ", 17) // Done
                         .AddOption("Previous.", 201)
                            .AddOption("Leave.", 255)
                                //.AddAvatar(523)
                                  .FinalizeDialog();
                        }
                        break;
                    }


                #region Level 6 Refainaries.

                case 107: //CriticalStrike[Bow]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        client.Inventory.Add(stream, 3004147, 1);



                        break;
                    }
                case 222: //SacredHossuHair [Hossu] 800 HP
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        client.Inventory.Add(stream, 3006170, 1);



                        break;
                    }

                case 106: //CriticalStrike[1-Handed]
                    {



                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        client.Inventory.Add(stream, 3004137, 1);


                        break;
                    }
                case 105: //CriticalStrike[2-Handed]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004156, 1);


                        break;
                    }

                case 104: //Breakthrough[1-Handed]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004141, 1);


                        break;
                    }

                case 102: //Breakthrough[2-Handed]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004159, 1);


                        break;
                    }
                case 101: //Breakthrough[Bow].
                    {

                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");



                        client.Inventory.Add(stream, 3004150, 1);


                        break;
                    }

                case 97: //Block[Shield].
                    {

                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");



                        client.Inventory.Add(stream, 3004153, 1);


                        break;
                    }

                case 96: //Detoxication[Headgear]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004143, 1);


                        break;
                    }

                case 95: //Detoxication[Armor]
                    {



                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004161, 1);


                        break;
                    }

                case 94: //Detoxication[Boots]
                    {



                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004152, 1);


                        break;
                    }
                case 92: //Detoxication[Necklace]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004165, 1);


                        break;
                    }

                case 91: //Penetration[Headgear]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004145, 1);


                        break;
                    }


                case 90: //Detoxication[Bag]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004154, 1);

                        break;
                    }


                case 89: //Penetration[Bracelet]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004162, 1);


                        break;
                    }

                case 88: //SkillC.Strike[Backsword]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004138, 1);


                        break;
                    }

                case 87: //M-Defense[Necklace]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004136, 1);


                        break;
                    }


                case 85: //CriticalStrike[1-Handed]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004137, 1);


                        break;
                    }


                case 84: //SkillC.Strike[Backsword].
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004138, 1);


                        break;
                    }

                case 83: //Immunity[Armor].
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004139, 1);


                        break;
                    }


                case 49: //Intensification[Headgear]
                    {

                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004140, 1);


                        break;
                    }


                case 48: //Breakthrough[1-Handed]
                    {



                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004141, 1);


                        break;
                    }


                case 47: //Counteraction[Armor]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004142, 1);


                        break;
                    }


                case 44: //Detoxication[HeadGear]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004143, 1);


                        break;
                    }


                case 43: //Block[HeadGear]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004144, 1);


                        break;
                    }


                case 42: //Penetration[HeadGear]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004145, 1);


                        break;
                    }

                case 41: //M-Defense[Bag]
                    {



                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004146, 1);


                        break;
                    }

                case 30: //CriticalStrike[Bow]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004147, 1);


                        break;
                    }

                case 29: //SkillC.Strike[Bracelet]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004148, 1);


                        break;
                    }

                case 27: //Immunity[Boots]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004149, 1);


                        break;
                    }
                case 207: //Detoxication[Boots]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004152, 1);


                        break;
                    }

                case 26: //Breakthrough[Bow]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004150, 1);


                        break;
                    }

                case 25: //Counteraction[Bag]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004151, 1);

                        break;
                    }

                case 24: //Detoxication[Boots]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004152, 1);


                        break;
                    }

                case 23: //Block[Shield]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004153, 1);


                        break;
                    }


                case 22: //Penetration[Bag]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004154, 1);


                        break;
                    }
                case 38: //Detoxication[Armor]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004161, 1);


                        break;
                    }

                case 20: //M-Defense[Ring]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004155, 1);


                        break;
                    }

                case 19: //CriticalStrike[2-Handed]
                    {





                        client.Inventory.Add(stream, 3004156, 1);

                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
                        break;
                    }

                case 18: //M-Defense[Bracelet]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004157, 1);


                        break;
                    }

                case 17: //CriticalStrike[Ring]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004158, 1);

                        break;
                    }

                case 16: //Breakthrough[2-Handed]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004159, 1);


                        break;
                    }

                case 15: //Counteraction[Necklace]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004160, 1);


                        break;
                    }
                case 37: //Penetration[Bracelet]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004162, 1);


                        break;
                    }
                case 36: //Breakthrough[Ring]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004163, 1);



                        break;
                    }

                case 34: //Breakthrough[Bracelet]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004164, 1);


                        break;
                    }
                case 33: //Detoxication[Necklace]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");


                        client.Inventory.Add(stream, 3004165, 1);


                        break;
                    }
                case 32: //Detoxication[Bag]
                    {


                        client.Inventory.Remove(3004249, 1, stream);
                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");

                        client.Inventory.Add(stream, 3004166, 1);


                        break;
                    }
                #endregion
            }
        }
        [NpcAttribute(NpcID.SelectP7EquipmentSoulPack)]
        public static void SelectP7EquipmentSoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3004248, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return;
            }


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 40:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                           .AddOption("Boots. ", 50) //1 (Done)
                           .AddOption("Bracelet. ", 60) //2 (Done)
                           .AddOption("Ring. ", 244) //3 (Done)
                           .AddOption("Bag. ", 70) //4 (Done) 
                           .AddOption("Necklace. ", 80) //5 (Done)
                           .AddOption("Headgear. ", 90) //6 (Done)
                           .AddOption("Armor. ", 100) //7 (Done)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 200:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                            .AddOption("Boots. ", 50) //1 (Done)
                            .AddOption("Bracelet. ", 60) //2 (Done)
                            .AddOption("Ring. ", 244) //3 (Done)
                            .AddOption("Bag. ", 70) //4 (Done) 
                            .AddOption("Necklace. ", 80) //5 (Done)
                            .AddOption("Headgear. ", 90) //6 (Done)
                            .AddOption("Armor. ", 100) //7 (Done)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                         .AddOption("Fox~Boots [Strike].", 15)
                       .AddOption("Dragon~Boots [Break].", 16)
                       .AddOption("Crane~Boots [M-Attack].", 17)
                       .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 60:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                       .AddOption("Rainbow~Bracelet.", 12)
                            .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 244:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                      .AddOption("Crane~Ring [Strike].", 10)
                       .AddOption("Dragon~Ring [Break].", 11)
                       .AddOption("Lion~Heavy~Ring [Strike].", 13)
                       .AddOption("Tiger~Heavy~Ring [Break].", 14)
                            .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                       .AddOption("Fervor~Bag.", 6)
                            .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                       .AddOption("Heaven~Necklace.", 5)
                          .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 90:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                      .AddOption("Moon~Headgear [Immunity].", 1)
                       .AddOption("Sun~Headgear [Anti-break].", 2)
                       .AddOption("Star~Headgear [Immunity].", 3)
                       .AddOption("Ice~Headgear [Anti-break].", 4)
                           .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        data.AddText("Please select a P7 Dragon Soul of equipment.")
                          .AddOption("Nether~Armor [P-Defense].", 7)
                       .AddOption("Eclipse~Armor [M-Defense].", 9)
                            .AddOption("Previous.", 200)
                            .AddOption("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 820073, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 820074, 1);
                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 820075, 1);
                        break;
                    }
                case 4:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 820076, 1);
                        break;
                    }
                case 5:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 821033, 1);
                        break;
                    }
                case 6:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 821034, 1);
                        break;
                    }
                case 7:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 822071, 1);
                        break;
                    }
                case 9:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 822072, 1);
                        break;
                    }
                case 10:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 823058, 1);
                        break;
                    }
                case 11:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 823059, 1);
                        break;
                    }
                case 12:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 823060, 1);
                        break;
                    }
                case 13:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 823061, 1);
                        break;
                    }
                case 14:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 823062, 1);
                        break;
                    }
                case 15:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 824018, 1);
                        break;
                    }
                case 16:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 824019, 1);
                        break;
                    }
                case 17:
                    {
                        client.Inventory.Remove(3004248, 1, stream);
                        client.Inventory.Add(stream, 824020, 1);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SelectP7WeaponSoulPack)]
        public static void SelectP7WeaponSoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3004247, 1))
            {
                client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                return;
            }


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 20:
                    {
                        data.AddText("Please select a P7 Dragon Soul of weapon.")
                            .AddOption("Monster~Saber~[1-handed].", 1)
                       .AddOption("Sky~Hammer~[1-handed].", 2)
                       .AddOption("Shadow~Katana~[Katana].", 3)
                       .AddOption("Sky~Halberd~[2handed].", 4)
                       .AddOption("Demon~Scythe~[Scythe].", 5)
                       .AddOption("Spirit~Shield~[Shield].", 6)
                       .AddOption("Time~Backsword~[Backsword].", 7)
                    .AddOption("Next~Page.", 8).FinalizeDialog();


                        break;
                    }
                case 0:
                    {
                        data.AddText("Please select a P7 Dragon Soul of weapon.")
                            .AddOption("Monster~Saber~[1-handed].", 1)
                       .AddOption("Sky~Hammer~[1-handed].", 2)
                       .AddOption("Shadow~Katana~[Katana].", 3)
                       .AddOption("Sky~Halberd~[2-handed].", 4)
                       .AddOption("Demon~Scythe~[Scythe].", 5)
                       .AddOption("Spirit~Shield~[Shield].", 6)
                       .AddOption("Time~Backsword~[Backsword].", 7)
                    .AddOption("Next~Page.", 8).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        data.AddText("Please select a P7 Dragon Soul of weapon.")
                      .AddOption("Sun~Bow~[Bow].", 9)
                 .AddOption("Budda~Beads~[Beads].", 10)
                 .AddOption("Death~Pistol~[Pistol].", 11)
                 .AddOption("Repent~Rapier~[Rapier].", 12)
                 .AddOption("Ghost~Knife~[Knife].", 13)
                  .AddOption("War~Craze[Nunchaku].", 14)
                   .AddOption("Wonder~Hossu.", 15)
                  .AddOption("Previous.", 20)
                  .AddOption("Next~Page.", 21).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        data.AddText("Please choose the P7 Weapon Soul you need:")
                      .AddOption("Fist~of~Deity~(Strike).", 16)
                 .AddOption("Fist~of~Demon~(Break).", 17)
                  .AddOption("Divine~Fan~Soul~[Windwalker].", 18)
                     .AddOption("Divine~Shockwave~(Stormhammer).", 25)
                     .AddOption("Divine~Void~Splitter~(Stormhammer).", 26)
                        .AddOption("Divine~Shockwave~(Flashaxe).", 27)
                        .AddOption("Divine~Void~Splitter~(Flashaxe).", 28)
                        .AddOption("Soul~Hunter.", 29)
                  .AddOption("Previous.", 8).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 827010, 1);
                        break;
                    }
                case 26:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 827012, 1);
                        break;
                    }
                case 27:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 827011, 1);
                        break;
                    }
                case 28:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 827013, 1);
                        break;
                    }
                case 29:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800633, 1);
                        break;
                    }
                case 18:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 801308, 1);
                        break;
                    }
                case 16:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 801216, 1);
                        break;
                    }
                case 17:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 801218, 1);
                        break;
                    }

                case 14:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 801004, 1);
                        break;
                    }
                case 15:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 801104, 1);
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800020, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800111, 1);
                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800142, 1);
                        break;
                    }
                case 4:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800215, 1);
                        break;
                    }
                case 5:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800255, 1);
                        break;
                    }
                case 6:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800422, 1);
                        break;
                    }
                case 7:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800522, 1);
                        break;
                    }
                case 9:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800618, 1);
                        break;
                    }
                case 10:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800725, 1);
                        break;
                    }
                case 11:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800810, 1);
                        break;
                    }
                case 12:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800811, 1);
                        break;
                    }
                case 13:
                    {
                        client.Inventory.Remove(3004247, 1, stream);
                        client.Inventory.Add(stream, 800917, 1);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DancePK)]
        public static void DancePK(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        //if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            data.AddText("Hello , I'm here to dance PK .")
                                 .AddOption("Yes ", 1)
                              .AddOption("No Thanks ", 255)
                                 .AddAvatar(325).FinalizeDialog();
                            break;
                        }
                    }

                case 1:
                    {
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;
                            client.Teleport(50, 50, 7357);

                            //client.CreateBoxDialog("Congratulations!You~have~got~3.500.000.000~Money~in~your~inventory");
                        }
                        else
                        {
                            data.AddText("You not have 1 CPS with you.")
                             .AddOption("I~see", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DragonIslandConductress)]
        public static void DragonIslandConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {                        
                        dialog.Text("Conductress:\n");
                        dialog.Text("May I help you? I provide decent teleport services.");
                        dialog.Option("Twin~City.", 1);
                        dialog.Option("Twin~City~Market", 2);
                        dialog.Option("Deityland.~(new~hunting~area)", 3);
                        dialog.Option("I`m~just~passing~by.", 255).AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(412, 354, 1002);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(345, 441, 1002);
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level > 15 && client.Player.Reborn >= 2)
                        {
                            client.Teleport(453, 483, 10137);
                        }
                        else
                        {
                            dialog.AddText("Buddy, I dont`t think little of you, but the place you want to go is much more dangerous than you imagine. Why not get tougher and come see me, then");
                            dialog.AddOption("Alright.", 255).AddAvatar(1).FinalizeDialog();
                        }
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.CobbWind)]
        public static void CobbWind(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        var map = Pool.ServerMaps[10137];

                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text111]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text112]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text113]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text114]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text115]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text116]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text117]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text118]@@");


                        if (map.ContainMobID(4220))
                        {
                            dialog.AddOption("STR_ID_tUndergroundPalace[20500][Option3]@@", 10);
                        }
                        if (map.ContainMobID(4171))
                        {
                            dialog.AddOption("STR_ID_tUndergroundPalace[20500][Option5]@@", 11);
                        }
                        if (map.ContainMobID(4212))
                        {
                            dialog.AddOption("STR_ID_tUndergroundPalace[20500][Option4]@@", 12);
                        }

                        if (DateTime.Now.Minute <= 00)
                        {
                            dialog.AddText("	* [Thrilling Spook] will appear at " + DateTime.Now.Hour + ":00 today around (349,635) on the Dragon Island.* ");
                        }
                        else if (DateTime.Now.Minute <= 47)
                        {
                            dialog.AddText("	* [Nemesis Tyrant] will appear at " + DateTime.Now.Hour + ":15 today around (568,372) on the Dragon Island.* ");
                        }
                        else if (DateTime.Now.Minute <= 57)
                        {
                            dialog.AddText("	* [Snow Banshee] will appear at " + DateTime.Now.Hour + ":27 today around (658,718) on the Dragon Island.* ");
                        }
                        dialog.AddOption("STR_ID_tUndergroundPalace[20500][Option1]@@", 1);
                        dialog.AddOption("STR_ID_tUndergroundPalace[20500][Option2]@@", 255);

                        dialog.AddAvatar(85).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text211]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text212]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text213]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text214]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text215]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text216]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text217]@@");
                        dialog.AddText("STR_ID_tUndergroundPalace[20500][Text218]@@");
                        dialog.AddOption("STR_ID_tUndergroundPalace[20500][Option7]@@", 255);
                        dialog.AddAvatar(85).FinalizeDialog();
                        break;
                    }
                case 10:
                case 11:
                case 12:
                case 13:
                    {

                        dialog.AddText("The beast must be killed without any delay. You can find it with pathfinding or pay 1 CP to instantly get teleported.");
                        if (Option == 10)
                        {
                            dialog.AddOption("Go~there~with~pathfinding.", 20);
                            dialog.AddOption("Instant~teleport.~(1~CP)", 30);
                        }
                        if (Option == 11)
                        {
                            dialog.AddOption("Go~there~with~pathfinding.", 21);
                            dialog.AddOption("Instant~teleport.~(1~CP)", 31);
                        }
                        if (Option == 12)
                        {
                            dialog.AddOption("Go~there~with~pathfinding.", 22);
                            dialog.AddOption("Instant~teleport.~(1~CP)", 32);
                        }
                        if (Option == 13)
                        {
                            dialog.AddOption("Go~there~with~pathfinding.", 23);
                            dialog.AddOption("Instant~teleport.~(1~CP)", 33);
                        }
                        dialog.AddAvatar(85).FinalizeDialog();
                        break;
                    }
                case 20:
                case 21:
                case 22:
                case 23:
                    {
                        ushort x = 0;
                        ushort y = 0;
                        if (Option == 20)
                        {
                            client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                            x = 568;
                            y = 372;
                        }
                        if (Option == 21)
                        {
                            client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                            x = 658;
                            y = 718;
                        }
                        if (Option == 22)
                        {
                            client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                            x = 349;
                            y = 635;
                        }
                        if (Option == 23)
                        {
                            client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                            x = 294;
                            y = 540;
                        }

                        client.Player.QuestGUI.SendAutoPatcher(10137, x, y, 0);
                        break;
                    }
                case 30:
                case 31:
                case 32:
                case 33:
                    {
                        if (client.Player.ConquerPoints > 1)
                        {
                            ushort x = 0;
                            ushort y = 0;
                            if (Option == 30)
                            {
                                client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                                x = 568;
                                y = 372;
                            }
                            if (Option == 31)
                            {
                                client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                                x = 658;
                                y = 718;
                            }
                            if (Option == 32)
                            {
                                client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                                x = 349;
                                y = 635;
                            }
                            if (Option == 33)
                            {
                                client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                                x = 294;
                                y = 540;
                            }
                            client.Teleport(x, y, client.Player.Map);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                            client.Player.ConquerPoints -= 1;
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.IronTong)]
        public static void IronTong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {                       
                        data.AddText("Iron Tong: \n ");
                        data.AddText("The Queen of Evil and her troop suddenly attacked the Dragon Island. Obviously, she came for the Twisting Hell Array.");
                        data.AddText("~I`m afraid she has figured out how to reactivate that array. We`d better not to let it happen. For the sake of humans,");
                        data.AddText("~please join us to defend the Dragon Island!");
                        data.AddOption("Send~me~to~Dragon~Island.", 1);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(55).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds


                        if (client.Player.Reborn == 2)
                        {
                            client.Teleport(95, 409, 10137);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        }
                        else
                        {
                            data.AddText("STR_ID_tUndergroundPalace[NotUpLevelText]@@");
                            data.AddOption("I,See.", 255);
                            data.AddAvatar(55).FinalizeDialog();

                        }
                        break;
                    }

            }
            //break;
        }

        [NpcAttribute(NpcID.GlowtownPortal1)]
        public static void GlowtownPortal1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Are you sure you want to return to Glowtown?")
                            .AddOption("Yes.", 1)
                            .AddOption("No.", 255)
                            .AddAvatar(156)
                            .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(93, 408, 10137);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GlowtownPortal2)]
        public static void GlowtownPortal2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Are you sure you want to return to Glowtown?")
                            .AddOption("Yes.", 1)
                            .AddOption("No.", 255)
                            .AddAvatar(156)
                            .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(93, 408, 10137);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GlowtownPortal3)]
        public static void GlowtownPortal3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Are you sure you want to return to Glowtown?")
                            .AddOption("Yes.", 1)
                            .AddOption("No.", 255)
                            .AddAvatar(156)
                            .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(93, 408, 10137);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Glowtownportal)]
        public static void Glowtownportal(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {

                        data.AddText("Are you sure you want to return to Glowtown?.")
                      .AddOption("Yes.", 1)
                            .AddOption("No.", 255)
                        .AddAvatar(1)
                        .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(93, 408, 10137);

                        break;
                    }
            }

        }
       
        [NpcAttribute(NpcID.ElementalPool)]
        public static void ElementalPool(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My optione its: 1: I open the  NightmareCaptain for one YinYangSeal and you'll have 20% chance to obtain an Elemental Jade.")
                            .AddText("2: I open the  PurpleBanshee for one Elemental Jade and you'll have 40% chance to obtain an Life`sEye.")
                            .AddText("3: I change you Stardust for Shadow Scroll , TwilightScroll or FatalScroll!")
                            .AddOption("Change my Stardust", 1)
                       .AddOption("Open~NightmareCaptain", 3)
                    .AddOption("Open~PurpleBanshee", 2)
                    .AddOption("Not~interested.", 255)
                    .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3004462, 1))
                        {
                            if (client.Player.Map == 3851)
                            {
                                if (!client.Map.ContainMobID(20100))
                                {
                                    client.Inventory.Remove(3004462, 1, stream);
                                    Server.AddMapMonster(stream, client.Map, 20100, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                            }
                            else
                                client.CreateDialog(stream, "Use that item in Infinity Abyss,", "I~see.");
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the YinYangSeal in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3004463, 1))
                        {
                            if (client.Player.Map == 3851)
                            {
                                if (!client.Map.ContainMobID(20101))
                                {
                                    client.Inventory.Remove(3004463, 1, stream);
                                    Server.AddMapMonster(stream, client.Map, 20101, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");

                            }
                            else
                                client.CreateDialog(stream, "Use that item in Infinity Abyss,", "I~see.");
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the ElementalJade in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("I change you Stardust for Shadow Scroll , TwilightScroll or FatalScroll. Please select the item.")
                      .AddOption("ShadowScroll", 10)
                      .AddOption("TwilightScroll", 11)
                      .AddOption("FatalScroll", 12)

                      .AddOption("Not~interested.", 255)
                      .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.Add(stream, 3004459, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Stardust in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.Add(stream, 3004460, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Stardust in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(3004465, 1))
                        {
                            client.Inventory.Remove(3004465, 1, stream);
                            client.Inventory.Add(stream, 3004461, 1);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Stardust in your inventory.")
                .AddOption("Okay.", 255)
                .AddAvatar(203).FinalizeDialog();
                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.SwapperStarry)]
        public static void SwapperStarry(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You'll need 4 Red Flowers and 4 White Flowers to begin the quest. You can obtain Red Flowers and White Flowers in the Evil Abyss .")
                            .AddText("There are 3 stages for you to complete the Epic Quest:")
                            .AddText("Stage 1: Go to feel Yin Yang Stele, and you'll have 30% chance to obtain a Yin Yang Seal.")
                            .AddText("Stage 2: Use a Yin Yang Seal to try to awaken the Elemental Guard, and you'll have 20% chance to obtain an Elemental Jade.")
                            .AddText("Stage 3: Use an Elemental Jade to try Wheel of Life, and you'll have 40% chance to obtain a Life's Eye.")
                            .AddOption("Tell~me~more", 1)
                              .AddOption("I~have~4 Red Flowers and 4 White Flowers.", 3)
                              .AddOption("Buy YinYangSeal 1000 CPs.", 12)
                    .AddOption("Teleport~to~Evil~Abyss", 2)
                    .AddOption("Teleport~to~TwinCity", 10)
                    .AddOption("Not~interested.", 255)
                    .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("If you fail to obtain key quest items during Stage 2 and Stage 3, instead you'll receive Stardust. With Stardust, you can exchange with Swapper Starry at the Infinite Abyss (27,40) for Shadow Scroll, Twilight Scroll, Fatal Scroll.")
                            .AddText("Note: When using a Life's Eye with a Katana equipped, you'll be able to transform the Katana into an Epic Weapon, Nobunaga's Claw. If you already have a Nobunaga's Claw in your left hand, and a Katana in the right, when you use a Life's Eye, your Katana will automatically be transformed into a Nobunaga's Claw, vice versa.")
                      .AddOption("Sound~Good", 255)
                      .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.ConquerPoints >= 1000)
                        {
                            if (client.InTrade) return;
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.ConquerPoints -= 1000;
                                client.Inventory.Add(stream, 3004462, 1);
                            }
                            else
                            {
                                data.AddText("Please make 1 more space in your inventory")
                                                                    .AddOption("Let me check.", 255)
                                                                    .AddAvatar(203).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs to buy that.")
                        .AddOption("Alright.", 255).AddAvatar(7).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {

                        client.Teleport(612, 634, 1700, 0);
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(722732, 4) && client.Inventory.Contain(722736, 4))
                        {
                            client.Inventory.Remove(722732, 4, stream);
                            client.Inventory.Remove(722736, 4, stream);
                            if (Role.Core.Rate(30))
                            {
                                client.Inventory.Add(stream, 3004462);
                            }
                            else
                                data.AddText("You wasn`t very lucky today .. I hope you will have a better luck next time ^^.")
                     .AddOption("Ah~OK", 255)
                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Please come back when you will have at inventory 4 Red Flowers and 4 White Flowers.")
                     .AddOption("Ah~OK", 255)
                     .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GameManager)]
        public static void GameManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hi, I can teleport you to the poker room. Where you can enjoy yourself");
                        data.Text("in the Texas Hold`em Poker. Wish you a good luck and good health.");
                        data.Option("Enter~Poker~Room.", 1);
                        data.Option("Enter~VIP~Hall.", 2);
                        //data.Option("Enter~Roulette~Room.", 3);
                        data.Option("Not~interested.", 4);
                        data.Avatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.SendSysMesage("You are teleported to the Poker Room.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(204, 154, 3053);


                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You arrived on the VIP Hall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(55, 62, 1860);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GameManager2)]
        public static void GameManager2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hi, I can teleport you to the poker room. Where you can enjoy yourself");
                        data.Text("in the Texas Hold`em Poker. Wish you a good luck and good health.");

                        data.Option("Enter~Poker~Room.", 1);
                        data.Option("Enter~VIP~Hall.", 2);
                        //  data.Option("Enter~Roulette~Room.", 3);
                        data.Option("Not~interested.", 4);
                        data.Avatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.SendSysMesage("You are teleported to the Poker Room.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(204, 154, 3053);


                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You arrived on the VIP Hall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(55, 62, 1860);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CasinoHostess)]
        public static void CasinoHostess(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Greeting, I`m glad to be at your service. Win or lose is not that important. To win is not the ultimate aim of the game, happiness is what we have been pursuing. Here, I even more like to wish you a good health than a good luck!")
                    .AddOption("I~have~no~interest~in~it.", 1)
                    .AddOption("I~still~want~to~play.", 255)
                    .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Stanley)]
        public static void Stanley(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello there. Just let me know if you want to go to TwinCity.")
                   .AddOption("TwinCity.", 2)
                   .AddOption("Let~me~think...", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab4)]
        public static void Lab4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~NorthGeneral.~This~is~the~fourth~floor.~If~you~want~to~go~back~to~Twin~City.~I~can~help~you.");
                        data.AddOption("To~Twin~City.", 2);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 3)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab3)]
        public static void Lab3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~SouthGeneral.~This~is~the~third~floor~of~Labyrinth.~If~you~want~to~enter~the~next~floor,~you~should~give~me~a");
                        data.AddText("~SoulToken.~Or~would~you~like~to~go~back~to~Twin~City?");
                        data.AddOption("To~the~next~floor.", 1);
                        data.AddOption("To~Twin~City.", 3);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721539, 1))
                        {
                            data.AddText("The~next~floor~is~more~dangerous.~You~should~be~more~careful.");
                            data.AddOption("Thank~you~very~much.", 2);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~don`t~have~a~SoulToken,~I~cannot~teleport~you~there.");
                            data.AddOption("I~will~stay~here.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721539, 1))
                        {
                            client.Inventory.Remove(721539, 1, stream);
                            client.Teleport(5, 290, 1354);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~1~SoulToken.")
                        .AddOption("Sigh...", 255)
                        .AddAvatar(63).FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 4)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab2)]
        public static void Lab2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~WestGeneral.~This~is~the~second~floor~of~Labyrinth.~If~you~want~to~enter~the~next~floor,~you~should~give~me~an");
                        data.AddText("~EarthToken,~or~would~you~like~to~go~back~to~Twin~City?");
                        data.AddOption("To~the~next~floor.", 1);
                        data.AddOption("To~Twin~City.", 3);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721538, 1))
                        {
                            data.AddText("The~next~floor~is~more~dangerous.~You~should~be~more~careful.");
                            data.AddOption("Thank~you~very~much.", 2);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~don`t~have~an~EarthToken.~I~cannot~teleport~you~to~the~next~floor.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721538, 1))
                        {
                            client.Inventory.Remove(721538, 1, stream);
                            client.Teleport(25, 268, 1353);
                        }
                        else
                        {
                            data.AddText("You~don`t~have~an~EarthToken.~I~cannot~teleport~you~to~the~next~floor.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 4)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Lab1)]
        public static void Lab1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I~am~EastGeneral.~This~is~the~first~floor~of~Labyrinth.~If~you~want~to~enter~the~next~floor,~you~should~give~me~a");
                        data.AddText("~Sky~Token,~which~is~carried~by~the~Gold~Gosts.");
                        data.AddOption("To~the~next~floor.", 1);
                        data.AddOption("To~Twin~City.", 3);
                        data.AddOption("I~will~stay~here.");
                        data.AddAvatar(31);
                        data.FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721537, 1))
                        {
                            data.AddText("The~next~floor~is~more~dangerous.~You~should~be~more~careful.");
                            data.AddOption("Thank~you~very~much.", 2);
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~have~no~Sky~Token.~Hurry~to~loot~one~from~the~Gold~Gosts.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {

                        if (client.Inventory.Contain(721537, 1))
                        {
                            client.Inventory.Remove(721537, 1, stream);
                            client.Teleport(27, 227, 1352);
                        }
                        else
                        {
                            data.AddText("You~have~no~Sky~Token.~Hurry~to~loot~one~from~the~Gold~Gosts.");
                            data.AddOption("I~will~stay~here.");
                            data.AddAvatar(31);
                            data.FinalizeDialog();

                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("Sure you want to leave in TwinCity?")
                            .AddOption("Yes.", 4)
                            .AddOption("Let~me~think...", 255)
                            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Simon)]
        public static void Simon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Great~rewards~will~attract~many~brave~people.~I~am~looking~for~brave~people~to~help~me~take~my~patrimony~back.")
                   .AddOption("Please~tell~me~more.", 1)
                   .AddOption("What~rewards?", 10)
                   .AddOption("I~got~SunDiamonds.", 12)
                   .AddOption("I~got~MoonDiamonds.", 13)
                   .AddOption("I~got~StarDiamonds.", 14)
                   .AddOption("I~got~CloudDiamonds.", 15)
                   .AddOption("Let~me~think...", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(721536, 17))
                        {
                            client.Inventory.Remove(721536, 17, stream);
                            client.Inventory.Add(stream, 721540, 1);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~CloudDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(721535, 17))
                        {
                            client.Inventory.Remove(721535, 17, stream);
                            Role.Flags.Gem[] Gems = new Role.Flags.Gem[] { Role.Flags.Gem.NormalDragonGem, Role.Flags.Gem.NormalFuryGem, Role.Flags.Gem.NormalGloryGem
                           , Role.Flags.Gem.NormalKylinGem, Role.Flags.Gem.NormalMoonGem, Role.Flags.Gem.NormalPhoenixGem, Role.Flags.Gem.NormalRainbowGem, Role.Flags.Gem.NormalThunderGem
                           , Role.Flags.Gem.NormalTortoiseGem, Role.Flags.Gem.NormalVioletGem};

                            Role.Flags.Gem GemType = (Role.Flags.Gem)Gems[Pool.GetRandom.Next(0, Gems.Length)];
                            client.Inventory.Add(stream, (uint)(700000 + (int)GemType), 1);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~StarDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(721534, 17))
                        {
                            client.Inventory.Remove(721534, 17, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 4);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~MoonDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(721533, 17))
                        {
                            client.Inventory.Remove(721533, 17, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            data.AddText("Well~done.~Here~is~the~rewards~I~promised~to~you.");
                            data.AddOption("Thanks.");
                            data.AddAvatar(63);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You~are~kidding~on~me.~How~dare~you~come~here~to~claim~prize~with~so~few~SunDiamonds?")
             .AddOption("Wait.~I~will~get~more.", 255)
             .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I~need~Sun`Diamond,~Moon~Diamond,~Star~Diamond~and~Cloud~Diamond.")
                          .AddText("If~You~get~them~for~me,~I~will~give~you~some~rewards.")
                .AddOption("What~rewards?", 11)
                .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        data.AddText("2~meteors~for~17~SunDiamonds,~4~meteors~for~17~MoonDiamonds,~a~normal~gem~for~17~StarDiamonds~and~a~AncestorBox")
                         .AddText("for~17~CloudDiamonds.~If~you~are~lucky~enough,~you~will~get~a~big~surprise~from~the~box.")
               .AddOption("I~see.~Thank~you.", 255)
               .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("My~ancestors~built~a~Labyrinth~long~before.~Many~treasures~were~stored~there~like~SunDiamonds,~MoonDiamonds,~")
                                  .AddText("StarDiamond~and~so~on.~But~it~was~occupied~by~fiece~monsters~soon.~They~expelled~our~clansmen~and~kept~the~treasure.")
                        .AddOption("Wow,~miserable.", 2)
                        .AddOption("I~have~no~interest.", 255)
                        .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("The~Labyrinth~contains~4~floors~guarded~by~different~monsters.~If~you~can~find~the~treasures~and~give~them~to~me,~I~will~reward")
              .AddOption("Please~continue.", 3)
              .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("There~are~3~kinds~of~monsters~on~each~floor.~The~more~monsters,~the~weaker~they~are.~The~ones~in~lesser")
                              .AddText("quantities~are~strong.~The~fiercest~monsters~are~always~moving~around.~And~they~drop~different~tokens.")
                           .AddOption("What~do~they~drop?", 4)
                           .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("SkyToken,~EarthToken~and~SoulToken~you~need~to~enter~the~next~floor.~While~the~weak~monsters~usually~drop~treasure.")
                        .AddText("After~You~get~a~token,~find~a~general~who~will~send~you~to~the~next~floor.~Some~boss~monsters~drop~rare~items.")
              .AddOption("I~see.~Thanks.", 5)
              .AddOption("I~must~leave~now.", 255)
              .AddAvatar(63).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("There are 3 ways to enter the Labyrinth: 1. Pay 2,000 Virtue Points; 2. Submit 5 meteors;")
                      .AddText("~3. VIP service.")
            .AddOption("Give~you~2000~Virtue~Points.", 6)
            .AddOption("I~have~5~Meteors.", 7)
              .AddOption("VIP~service.", 8)
            .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.VipLevel >= 1)
                        {
                            client.Teleport(18, 127, 1351);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~VIP.")
                           .AddOption("Sigh...", 255)
                           .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Player.VirtutePoints > 2000)
                        {
                            client.Player.VirtutePoints -= 2000;
                            client.Player.SendUpdate(stream, client.Player.VirtutePoints, MsgServer.MsgUpdate.DataType.VirtutePoints);
                            client.Teleport(18, 127, 1351);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~enough~virtue~points.")
                           .AddOption("Sigh...", 255)
                           .AddAvatar(63).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 5))
                        {
                            client.Inventory.Remove(Database.ItemType.Meteor, 5, stream);
                            client.Teleport(18, 127, 1351);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~don`t~have~5~Meteors.")
                            .AddOption("Sigh...", 255)
                            .AddAvatar(63).FinalizeDialog();

                        }

                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.FrozenGrottoGuardian5)]
        public static void FrozenGrottoGuardian5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText(" Who`s there?.. Oh, you are from Twin City. aren`t you? Finally, i thoyght no one would come here to rescue us..")
                            .AddText("We`ve still got some business to carry out on the 6th floor. You`re here to go there and clear out the monsters,")
                            .AddText("right? I can also help you get the other floors, if you like.")
                    .AddOption("How can i go FG6? ", 1)
                    .AddOption("Gimme a Shadow Jave", 2)
                    .AddOption("Send me to FG4 then.", 3)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(31).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("There is a portal on this floor (150,178). You can get to F6 from here. Be careful.")
                            .AddOption("Thanks. I will.", 255)
                            .AddAvatar(31).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(720599, 1))
                            {
                                data.AddText("Sorry, but your have ShadowJade.")
                                 .AddOption("Ah~yes.", 255)
                               .AddAvatar(0).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 720599);
                        }
                        else
                        {
                            data.AddText("Please make 1 space in your inventory.")
                                  .AddOption("Ah~yes.", 255)
                                .AddAvatar(0).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        client.Teleport(379, 470, 2054);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FrozenFroto4Teleporter)]
        public static void FrozenFroto4Teleporter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText(" ~I guess you`ve heard about the 5th floor of Frozen Grotto before. The place is infested with so many ferocious monsters that no one dares go up there. Well, until recently,")
                            .AddText(" that is... Yeah, you heard me... I found out that the monsters on the 3rd and 4th")
                            .AddText("~are carrying a Grotto Key with them. With this key, it would be easier to kill the monsters on the 5th floor. If you are interested, go kill those monsters")
                            .AddText(" on the 3rd and 4th floor and get a key, then I`ll send you to the 5th floor to get rid of")
                            .AddText("~those nasty beasts. Hmmm,~I`m~on~it.")
                            .AddText("If your VIP level has reached 4, you can enter the 5th floor of Frozen Grotto everyday, for free!")
                    .AddOption("Hey,~I`ve~got~a~key~here.", 1)
                    .AddOption("Frozen~Grotto~F5.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(31).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721347, 1))
                        {
                            client.Inventory.Remove(721347, 1, stream);
                            client.Teleport(526, 562, 2055);
                        }
                        else
                        {
                            data.AddText("Sorry, you not have the Grotto Key in your inventory. Go to kill monsters to obtain that key..")
                    .AddOption("Okay.", 255)
                    .AddAvatar(31).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Player.VipLevel >= 4)
                        {
                            client.Teleport(526, 562, 2055);
                        }
                        else
                        {
                            data.AddText("Sorry, but your VIP Level has to reach 4 to enter the 5th floor for free. If you want to go there, you must collect the Grotto Key.")
                 .AddOption("I~see.", 255)
                 .AddAvatar(31).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DesertFrozenGroto)]
        public static void DesertFrozenGroto(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The Frozen Grotto is a dangerous and ancient place. Many heroes have died there while searching for its hidden treasures.")
                            .AddText("~Six layers of the Frozen Grotto have been excavated so far. If you have made up your mind to test your ability there, I can")
                            .AddText("~teleport you to the first 2 floors. If you want to go to the 5th floor, speak to the Guardian on Frozen Grotto F4 (387,460).")
                    .AddOption("Frozen~Grotto~1.", 1)
                    .AddOption("Frozen~Grotto~2.", 2)
                    .AddOption("Let~me~think...", 255)
                    .AddAvatar(183).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        client.Teleport(338, 354, 1762);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(392, 625, 1927);
                        break;
                    }
            }
        }
        public static void Furnitures(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Database.NpcServer.Furniture Furniture;
            if (Database.NpcServer.FurnitureInformations.TryGetValue(id, out Furniture))
            {
                Dialog data = new Dialog(client, stream);
                switch (Option)
                {
                    case 0:
                        {
                            data.AddText("Do you want this " + Furniture.Name + " ? It costs only " + Furniture.MoneyCost + " silvers.")
                                                     .AddOption("Yes.", 1)
                                                       .AddOption("No.", 255)
                                                     .AddAvatar(0).FinalizeDialog();


                            break;
                        }
                    case 1:
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (Furniture.MoneyCost < 0)
                                    return;
                                if (client.Player.Money >= Furniture.MoneyCost)
                                {
                                    client.Player.Money -= Furniture.MoneyCost;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, Furniture.ItemID, 1);

                                    Database.ItemType.DBItem item;
                                    if (Pool.ItemsBase.TryGetValue(Furniture.ItemID, out item))
                                    {
                                        client.SendSysMesage("You got a " + item.Name + "!", MsgMessage.ChatMode.System);
                                    }
                                }
                                else
                                {

                                    data.AddText("You do not have " + Furniture.MoneyCost + " silvers with you.")
                                     .AddOption("I~see.", 255)
                                   .AddAvatar(0).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("There is not enough room in your inventory.")
                                    .AddOption("Ah~yes.", 255)
                                  .AddAvatar(0).FinalizeDialog();
                            }


                            break;
                        }
                }
            }
        }

        public static void Warehause(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.ActiveNpc == (uint)NpcID.WHTwin)
            {
                var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BarberSha, client.Player.Class);
                var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);

                var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCViceCaptain, client.Player.Class, 1006);
                Dialog data = new Dialog(client, stream);
                switch (Option)
                {

                    case 0:
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You can deposit or withdraw your items and money, here. Wait, are you here to claim the Military Letter for the TC Vice Captain?")
                                 .AddOption("Yeah!", 10)
                                 .AddOption("Open~my~Warehouse.", 1)
                                 .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                            {
                                data.AddText("I am the most reliable man in Twin City! Your valuable things will be safe in my warehouse!")
                               .AddOption("You~look~reliable.", 2)
                               .AddOption("Open~my~Warehouse.", 1)
                               .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                            {
                                data.AddText("You really need a good weapon during your long journey! The Blacksmith has many good stuffs! He should have something for you.")
                .AddOption("I~will~speak~with~him~now!", 3)
                  .AddOption("Open~my~Warehouse.", 1)
                .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                goto case 1;
                            }
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("You really need a good weapon during your long journey! The Blacksmith has many good stuffs! He should have something for you.")
                  .AddOption("I~will~speak~with~him~now!", 3)
                    .AddOption("Open~my~Warehouse.", 1)
                  .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            goto case 1;


                        }
                    case 10:
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                               .AddOption("let me to check.", 255)
                               .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            if (client.Inventory.Contain(711235, 1))
                            {
                                data.AddText("You`ve got the Military Letter. Go give it to the TC Vice Captain!")
                                   .AddOption("I`ll~go~now.", 11)
                                   .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 711235);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1, 1);

                            data.AddText("This is the letter you want. Take it. By the way, don`t forget about our wonderful storage services!")
                                .AddOption("Okay,~thanks.", 11)
                                .AddAvatar(10).FinalizeDialog();



                            break;
                        }
                    case 11:
                        {
                            client.Player.QuestGUI.SendAutoPatcher(1002, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                            break;
                        }
                    case 1:
                        {
                            if (client.ActiveNpc == (uint)NpcID.WHPoker)
                            {
                                ActionQuery action = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = client.Player.UID,
                                    dwParam = 878,
                                    Fascing = 3,
                                    PositionX = client.Player.X,
                                    PositionY = client.Player.Y
                                };
                                client.Send(stream.ActionCreate(action));
                            }
                            else
                            {
                                ActionQuery action = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = client.Player.UID,
                                    dwParam = MsgServer.DialogCommands.Warehouse,
                                    PositionX = client.Player.X,
                                    PositionY = client.Player.Y
                                };
                                client.Send(stream.ActionCreate(action));
                            }
                            break;
                        }
                    case 2:
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 1400, Role.Flags.ExperienceEffect.angelwing);
                                data.AddText("You really need a good weapon during your long journey! The Blacksmith has many good stuffs! He should have something for you.")
                  .AddOption("I~will~speak~with~him~now!", 3)
                    .AddOption("Open~my~Warehouse.", 1)
                  .AddAvatar(10).FinalizeDialog();
                            }


                            break;
                        }
                    case 3:
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.QuestGUI.SendAutoPatcher("The~Warehouseman~asked~you~to~pay~a~visit~to~the~Blacksmith.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);



                            //client.Player.QuestGUI.Accept(818, 0);
                            //  client.Player.QuestGUI.SendAutoPatcher("The~Warehouseman~asked~you~to~pay~a~visit~to~the~Blacksmith.", 1002, 324, 230, (uint)NpcID.BlackSmith);

                            break;
                        }
                }
            }
            else
            {
                if (client.ActiveNpc == (uint)NpcID.WHPoker)
                {
                    ActionQuery action = new ActionQuery()
                    {
                        Type = ActionType.OpenDialog,
                        ObjId = client.Player.UID,
                        dwParam = 878,
                        Fascing = 3,
                        PositionX = client.Player.X,
                        PositionY = client.Player.Y
                    };
                    client.Send(stream.ActionCreate(action));
                }
                else
                {
                    ActionQuery action = new ActionQuery()
                    {
                        Type = ActionType.OpenDialog,
                        ObjId = client.Player.UID,
                        dwParam = MsgServer.DialogCommands.Warehouse,
                        PositionX = client.Player.X,
                        PositionY = client.Player.Y
                    };
                    client.Send(stream.ActionCreate(action));
                }



            }
        }
        public static void GuildCondTeleBack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("In exchange of a fee of 500gold, I will teleport you back to the guild war map. Don't bother me if you are poor.")
                        .AddOption("Teleport me.", 1)

                        .AddOption("I'm too poor.", 255).AddAvatar(3).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 500)
                        {
                            client.Player.Money -= 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.Teleport(348, 339, 1038);
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        }
                        else
                        {
                            data.AddText("You do not have 500 silvers with you.")
                                 .AddOption("I~see.", 255)
                               .AddAvatar(3).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        public static void GuildConductorsProces(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Are~you~heading~for~the~next~teleporter?~It~is~free~for~our~members,~and~1,000~silver~for~others.")
                        .AddOption("Please~teleport~me~there.", 1)
                        .AddOption("Buy~Guild~Teleporter.", 2)
                        .AddOption("Just~passing~by.", 255).AddAvatar(3).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 1000)
                        {
                            client.Player.Money -= 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);

                            if (id == (uint)NpcID.GuildConductor1)
                            {
                                client.Teleport(26, 133, 1351);
                            }
                            if (id == (uint)NpcID.GuildConductor2)
                            {
                                client.Teleport(32, 229, 1352);
                            }
                            if (id == (uint)NpcID.GuildConductor3)
                            {
                                client.Teleport(31, 265, 1353);
                            }
                            if (id == (uint)NpcID.GuildConductor4)
                            {
                                client.Teleport(20, 294, 1354);
                            }
                        }
                        else
                        {
                            data.AddText("Please~take~1000~silvers~to~use~guild~teleporter.")
                        .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("A~Teleporter~costs~100,000~silver.~After~you~buy~one~and~place~it~at~a~good~location,~you~can~be~teleported~there~directly.")
                     .AddOption("Buy~Teleporter~1.", 3)
                     .AddOption("Buy~Teleporter~2.", 4)
                              .AddOption("Buy~Teleporter~3.", 5)
                     .AddOption("Buy~Teleporter~4.", 6)
                     .AddOption("I~changed~my~mind.", 255).AddAvatar(3).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                                                   && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720021);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
                case 6:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                            && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720024);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                           && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720023);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
                case 4:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID
                            && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.Money >= 100000)
                                {
                                    client.Player.Money -= 100000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720022);
                                }
                                else
                                {

                                    data.AddText("You do not have 100.000 silvers with you.")
                                  .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Please make one more space in your inventory.")
                              .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~only~the~guild~leader~who~won~the~guild~war~can~buy~a~Guild~Teleporter.")
                          .AddOption("I~see.", 255).AddAvatar(3).FinalizeDialog();
                        }



                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.LeftGate)]
        public static void LeftGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID)
                        {
                            client.TerainMask = 0;
                            client.Teleport(162, 192, 1038);
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh == Role.SobNpc.StaticMesh.LeftGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints == 0)
                                MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints = MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].MaxHitPoints;

                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh == Role.SobNpc.StaticMesh.OpenLeftGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.OpenLeftGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RightGate)]
        public static void RightGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID)
                        {
                            client.TerainMask = 0;
                            client.Teleport(203, 176, 1038);

                            if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                client.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }


                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                           || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh == Role.SobNpc.StaticMesh.RightGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints == 0)
                                MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints = MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].MaxHitPoints;

                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.RightGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh);
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                            || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh == Role.SobNpc.StaticMesh.OpenRightGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.OpenRightGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == 1038)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].X
                                        , MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperGuildWar_CasteleLeftGate)]
        public static void SuperGuildWar_CasteleLeftGate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Select the option you want to pursue.")
                        .AddOption("Open~the~gate", 1)
                        .AddOption("Close gate.", 2)
                        .AddOption("Get inside.", 3)
                        .AddOption("Not~interested.", 255).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID)
                        {
                            client.Teleport(164, 194, 1138);
                        }
                        else
                        {
                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                              .AddOption("Okay.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 2:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                                                   || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {

                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleLeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.RightGate)
                            {
                                data.AddText("Gate~is~already~closed.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            if (Gate.HitPoints == 0)
                                Gate.HitPoints = Gate.MaxHitPoints;

                            Gate.Mesh = Role.SobNpc.StaticMesh.RightGate;

                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, (long)Gate.HitPoints);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X
                                        , Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if ((client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader
                           || client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader) && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID == client.Player.GuildID)
                        {
                            var Gate = MsgTournaments.MsgSchedules.SuperGuildWar.Furnitures[MsgTournaments.MsgSuperGuildWar.FurnituresType.CastleLeftGate];

                            if (Gate.Mesh == Role.SobNpc.StaticMesh.OpenRightGate)
                            {
                                data.AddText("Gate~is~already~opened.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                            Gate.Mesh = Role.SobNpc.StaticMesh.OpenRightGate;
                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Gate.UID, 1);

                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Gate.Mesh);
                            stream = upd.GetArray(stream);
                            foreach (var pclient in Pool.GamePoll.Values)
                            {
                                if (pclient.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID)
                                {
                                    if (Role.Core.GetDistance(pclient.Player.X, pclient.Player.Y, Gate.X, Gate.Y) <= Role.SobNpc.SeedDistrance)
                                    {
                                        pclient.Send(stream);
                                    }
                                }
                            }
                        }
                        else
                        {

                            data.AddText("Sorry,~you~cannot~open~it.Only DSl or GLs can, from guild who dominated the pole.")
                            .AddOption("Okay.", 255).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
    }
}

