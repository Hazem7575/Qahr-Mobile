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
        [NpcAttribute(NpcID.Steed3Pack)]
        public static void Steed3Pack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3007283, 1, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1Pack.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim one from the three +3 Steeds (B) in this pack. Which one do you like?")
                            .AddOption("+3 White Steed(B)", 1)
                       .AddOption("+3 Brown Steed(B)", 5)
                       .AddOption("+3 Black Steed(B)", 10)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to claim a +3 Black Steed (B) from this pack?")
                           .AddOption("Yes.", 11)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.SendSysMesage("You received +3 Black Steed (B)!", MsgMessage.ChatMode.System);
                        client.Inventory.Remove(3007283, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 255, 0, 150);
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to claim a +3 Brown Steed (B) from this pack?")
                           .AddOption("Yes.", 6)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.SendSysMesage("You received +3 Brown Steed (B)!", MsgMessage.ChatMode.System);
                        client.Inventory.Remove(3007283, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 0, 150, 255);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to claim a +3 White Steed (B) from this pack?")
                           .AddOption("Yes.", 2)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You received +3 White Steed (B)!", MsgMessage.ChatMode.System);
                        client.Inventory.Remove(3007283, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 3, true, 150, 255, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Steed1Pack)]
        public static void Steed1Pack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3007287, 1, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a Steed+1Pack.");
                return;
            }

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can claim one from the three +1 Steeds (B) in this pack. Which one do you like?")
                            .AddOption("+1 White Steed(B)", 1)
                       .AddOption("+1 Brown Steed(B)", 5)
                       .AddOption("+1 Black Steed(B)", 10)
                       .AddOption("Nevermind", 255)
                       .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to claim a +1 Black Steed (B) from this pack?")
                           .AddOption("Yes.", 11)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.SendSysMesage("You received +1 Black Steed (B)!", MsgMessage.ChatMode.System);
                        client.Inventory.Remove(3007287, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 255, 0, 150);
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to claim a +1 Brown Steed (B) from this pack?")
                           .AddOption("Yes.", 6)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.SendSysMesage("You received +1 Brown Steed (B)!", MsgMessage.ChatMode.System);
                        client.Inventory.Remove(3007287, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 0, 150, 255);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Are you sure you want to claim a +1 White Steed (B) from this pack?")
                           .AddOption("Yes.", 2)
                      .AddOption("No.", 255)
                      .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You received +1 White Steed (B)!", MsgMessage.ChatMode.System);
                        client.Inventory.Remove(3007287, 1, stream);
                        client.Inventory.AddSteed(stream, 300000, 1, 1, true, 150, 255, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ServerTransfer)]
        public static void ServerTransfer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            DateTime Now64 = DateTime.Now;
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Guys, I`m responsible for transferring servers. If you don`t wanna stay here anymore, ");
                        data.AddText("I can help anytime except 07:00 - 09:30 and 15:00 - 17:30. Anything?");
                        // if (DateTime.Now.Day == 7 && Now64.Hour == 21 && Now64.Minute == 0 && Now64.Second < 4)
                        data.AddOption("Yeah,~tell~me~more~about~it.", 1);
                        //else
                        // if (DateTime.Now.Day >= 7 && Now64.Hour >= 21 && Now64.Minute >= 0 && Now64.Second > 4)
                        data.AddOption("I~wanna~transfer~now.", 10);
                        // else
                        //    data.AddOption("Transportation will open on day 7.", 255);
                        data.AddOption("Nothing.", 255).AddAvatar(186)
                        .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("I like curious people, seriously. There`re something you need to do before transferring:")
                       .AddText("1. Cancel all your relationships with this server, including guild, clan, mentor, trade partner and marriage./n")
                       .AddText("2. Make sure you aren`t detaining other players` equipment, or your equipment is being detained by others./n")
                       .AddText("3. Retrieve all items from your itemboxes in your house./n")
                        .AddText("4. Claim all your TQ Point Cards./n")
                         .AddText("5. View all your unread messages./n")
                          .AddText("6. Claim your mentor exp./n")
                          .AddText("After that, give me 200,000 CPs, done!")
             .AddOption("I~got~it.", 255).AddAvatar(186)
             .FinalizeDialog();
                        break;
                    }
                case 10:
                    {


                        data.AddText("Select a server to transfer. ");
                        foreach (var server in Database.GroupServerList.CanTransferServers())
                            data.AddOption("" + server.Name + "", (byte)(100 + server.ID));
                        data.AddOption("Nothing.", 255)
                            .AddAvatar(186)
                        .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (Option > 100)
                        {

                            if (client.Player.MyGuild != null || client.Player.InUnion || client.Player.MyClan != null)
                            {
                                data.AddText("Please remove your character from Guild , Clan and Union before transfer!")
                                .AddOption("I~see.", 255).AddAvatar(186)
                                .FinalizeDialog();
                                return;
                            }

                            if (client.Player.IsCheckedPass == false)
                            {
                                client.CreateBoxDialog("Make~sure~you~have~entered~a~correct~secondary~password~in~the~warehouse!");
                                return;
                            }
                            var server = Database.GroupServerList.GetServer((uint)(Option - 100));
                            if (server != null)
                            {

                                client.Player.MessageBox("Are~you~sure~you~want~to~transfer~to~" + server.Name + "~server?",
                                    p => p.Player.InitializeTransfer(server.ID), null, 0, MsgStaticMessage.Messages.None);

                            }
                        }
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.IdealDesgner)]
        public static void IdealDesgner(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I can turn a Spirit of Legend into a random temporary Phoenix Cup or <tip color=0xfff05050 desc=A#certain#number#of#cup#fragments#can#be#combined#into#the#corresponding#Phoenix#Cup.>cup fragment</tip>.")
                            .AddText("~Equipping the Phoenix Cup will bring you attribute bonus. If you have any cup that you don`t need anymore, you can exchange it for <tip color=0xfff05050 desc=30#fragments#can#be#combined#into#a#90-day#Holy#Phoenix#Cup.>Holy Phoenix Cup Fragment</tip> or")
                            .AddText("~Chi Points.")
                   .AddOption("Use~the~Spirit~of~Legend.", 1)
                   .AddOption("Swap~for~cup~fragment~or~Chi.", 255)
                   .AddOption("Learn~more~about~Phoenix~Cups.", 30)
                   .AddOption("I`ll~talk~to~you~later.", 255)
                   .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3300056, 5))
                        {
                            data.AddText("f you really want the Phoenix Cup, you can also pay 5 Spirit of Legend to make a try.")
                  .AddOption("Deal!~Here~are~5xSpiritofLegend.", 5)
                  .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("It seems you don`t have 5x Spirit of Legend. If you really want the Phoenix Cup, you can also pay 15,000 CPs to make a try.")
                    .AddOption("Deal!~Here~are~15,000~CPs.", 2)
                    .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to try with 5x Spirit of Legend?")
            .AddOption("Yes.~I`ll~pay~the~5xSpiritofLegend.", 6)
            .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(3300056, 5))
                        {
                            client.Inventory.Remove(3300056, 5, stream);
                            byte rand = (byte)Pool.GetRandom.Next(1, 3);
                            byte count = (byte)Pool.GetRandom.Next(1, 3);
                            switch (rand)
                            {
                                case 1:
                                    {

                                        client.SendSysMesage("You received SilverPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300053, 0, count, stream);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.SendSysMesage("You received GoldenPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300054, 0, count, stream);
                                        break;
                                    }
                                default:
                                    {
                                        client.SendSysMesage("You received HolyPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300055, 0, count, stream);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You should give me a Spirit of Legend or 15,000 CPs before trying to make a phoenix cup.")
             .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to try with 15,000 CPs instead of a Spirit of Legend?")
              .AddOption("Yes.~I`ll~pay~the~money.", 3)
              .AddOption("I`ll~think~about~it.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 15000)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 15000;
                            byte rand = (byte)Pool.GetRandom.Next(1, 3);
                            byte count = (byte)Pool.GetRandom.Next(1, 3);
                            switch (rand)
                            {
                                case 1:
                                    {

                                        client.SendSysMesage("You received SilverPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300053, 0, count, stream);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.SendSysMesage("You received GoldenPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300054, 0, count, stream);
                                        break;
                                    }
                                default:
                                    {
                                        client.SendSysMesage("You received HolyPhoenixCupFragment*" + count + "!", MsgMessage.ChatMode.System);
                                        client.Inventory.AddItemWitchStack(3300055, 0, count, stream);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("You should give me a Spirit of Legend or 15,000 CPs before trying to make a phoenix cup.")
             .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("The Cross-server Elite PK Tournament will award 4 kinds of phoenix cups which grant different attributes.")
                          .AddText("~Which cup would you like to know about?")
                 .AddOption("Holy~Phoenix~Cup.", 31)
                 .AddOption("Golden~Phoenix~Cup.", 32)
                 .AddOption("Silver~Phoenix~Cup.", 33)
                 .AddOption("Bronze~Phoenix~Cup.", 34).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Holy Phoenix Cup's basic attributes: 1% Blessed, HP +1200, MP +1200, Defense +1000, \n")
                        .AddText("                  P-Strike +1%, M-Strike +1%, Break +1%,      Anti-break +1% \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.5 Draining Touch, Lv.2 Blood Spawn, \n")
                        .AddText("                   Lv.5 Free Soul, Lv.5 Killing Flash, Lv.3 Straight Life, HP +500")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("Golden Phoenix Cup's basic attributes: 1% Blessed, HP +1000, MP +1000, Defense +500 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.4 Draining Touch, Lv.2 Blood Spawn, \n")
                        .AddText("                  Lv.4 Free Soul, Lv.4 Killing Flash, Lv.3 Straight Life, HP +300 \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("Silver Phoenix Cup's basic attributes: 1% Blessed, HP +900, MP +900, Defense +300 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.3 Draining Touch, Lv.1 Blood Spawn, \n")
                        .AddText("                  Lv.3 Free Soul, Lv.3 Killing Flash, Lv.2 Straight Life \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("Bronze Phoenix Cup's basic attributes: 1% Blessed, HP +800, MP +800, Defense +150 \n")
                        .AddText("Bonus attributes if tempered: Lv.3 Toxin Eraser, Lv.2 Draining Touch, Lv.2 Killing Flash,  Lv.2 Free Soul \n")
               .AddOption("I~see.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.AstralPheonix)]
        public static void AstralPheonix(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("1.Join the Cross-Server Elite PK Tournament and you can get Elite Points. \n")
                            .AddText("2.Elite Points can be used to exchange for reward. \n")
                            .AddText("3.Top 8 players can get valuable reward such as exclusive halo and garments.")
                   .AddOption("I~see.", 255).AddAvatar(4).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CSElitePkManager)]
        public static void CSElitePkManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The 1st Cross-server Elite PK Tournament will kick off soon. On the stage, the elite fighters will compete for the glory championship together with exclusive mount armor and title! \n")
                            .AddText("Admittance: 19:55 - 19:59 on the event date \n")
                            .AddText("Kick-off Time: 20:00 on the event date \n")
                            .AddText("Requirement: Level 100 \n")
                            .AddOption("Sign~me~up! ", 1)
                            .AddOption("Learn~about~the~rules.", 2)
                            .AddOption("Swap~points~for~rewards.", 10)
                            .AddOption("Claim~participation~reward.", 30)
                            .AddOption("View~the~ranking.", 20)
                   .AddOption("I`ll~talk~to~you~later. ", 255)
                   .AddAvatar(4).FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255).AddAvatar(4).FinalizeDialog();
                            break;
                        }
                        if (MsgInterServer.Instance.CrossElitePKTournament.GetReward(client, stream))
                        {
                            client.CreateBoxDialog("I have added the coupons to inventory. Thanks for participating.");
                        }
                        else
                        {
                            data.AddText("You can`t claim the reward since you didn`t participate in the Cross-server Elite PK this time.")
                           .AddOption("Alright.", 255).AddAvatar(4).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        List<Game.MsgTournaments.MsgEliteGroup.FighterStats> array = new List<MsgTournaments.MsgEliteGroup.FighterStats>();
                        if (MsgInterServer.Instance.CrossElitePKTournament.RanksElite.TryGetValue(3, out array))
                        {
                            stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, (uint)(array.Count), client.Player.UID);

                            for (int x = 0; x < array.Count; x++)
                                stream.AddItemElitePkRanking(array[x], array[x].CrossEliteRank);
                            client.Send(stream.ElitePkRankingFinalize());
                        }
                        else
                        {
                            stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, 3, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, client.Player.UID);
                            client.Send(stream.ElitePkRankingFinalize());
                        }
                        break;
                    }
                case 10:
                    {

                        break;
                    }
                case 2:
                    {
                        data.AddText("Contestants will be fighting in 3 level divisions. The higher division, the better the prize. Prizes are granted via mail.")
                              .AddText("~The server`s Elite PK Top 8 winners will be qualified for the cross-server tournament, while others need to register")
                              .AddText("~with me to get an accreditation card. Please note, leaving the match early will result in no reward for you.")
                              .AddOption("Prizes~for~L130+~division.", 3)
                              .AddOption("Prizes~for~L120-129~division.", 4)
                                .AddOption("Prizes~for~L100-119~division.", 5)
                            .AddOption("I`ll~talk~to~you~later. ", 255)
                            .AddAvatar(4).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Champion: 7-day 1% Blessed Astral Phoenix, 30-day champion title and halo,")
                              .AddText("      3 <tip color=0xfff05050 desc=It#requires#50#fragments#to#make#a#complete#Astral#Phoenix.>Astral Phoenix Fragments </tip>, and 5,000")
                              .AddText("<tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 7-day 1% Blessed Astral Phoenix, 30-day 2nd Place title and halo,")
                              .AddText("      2 Astral Phoenix Fragments, and 4,000 Elite PK Point Coupons.")
                              .AddText("3rd Place: 7-day 1% Blessed Astral Phoenix, 30-day 3rd Place title and halo,")
                              .AddText("      1 Astral Phoenix Fragment, and 3,500  Elite PK Point Coupons.")
                              .AddText("Top 8: 30-day Top 8 title and halo and 3,000 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")
                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Champion: 30-day champion halo and 3,000 <tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 30-day 2nd Place halo and 2,500 Elite PK Point Coupons.")
                              .AddText("3rd Place: 30-day 3rd Place halo and 2,000 Elite PK Point Coupons.")
                              .AddText("Top 8: 30-day Top 8 halo and 1,500 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")

                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Champion: 2,000 <tip color=0xfff05050 desc=Swap#it#for#nice#rewards#with#the#CS#Elite#PK#Manager.>Elite PK Point Coupons</tip>.")
                              .AddText("2nd Place: 1,500 Elite PK Point Coupons.")
                              .AddText("3rd Place: 1,200 Elite PK Point Coupons.")
                              .AddText("Top 8: 1,000 Elite PK Point Coupons.")
                              .AddText("The participants who`ve completed the match but failed to reach the top 8 can claim 500 Elite PK Point Coupons from me.")
                            .AddOption("I~see", 255)
                            .AddAvatar(4).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgInterServer.Core.IsCrossPkOpen)
                        {
                            client.Player.SetLocationType = 1;
                            MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);
                        }
                        else
                        {
                            data.AddText("Please sign up between 19:55 and 19:59 every Thursday night for the Cross Elite PK Tournament.")
                              .AddOption("Okay.", 255).AddAvatar(4).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HolyPhoenixCup)]
        public static void HolyPhoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                  *Holy Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +1200					 Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +1200					 Lv.5 Draining Touch \n")
                            .AddText(" 					Defense +1000			 Lv.2 Blood Spawn \n")
                            .AddText(" 					P-Strike +1%				 Lv.5 Free Soul \n")
                            .AddText(" 					M-Strike +1%				 Lv.5 Killing Flash \n")
                            .AddText(" 					Break +1%					 Lv.3 Straight Life \n")
                             .AddText(" 				Anti-break +1%				 HP +500 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Holy Phoenix Cup`s Duration: 90 days")
                            //.AddOption("Change 50 legend spirit..", 255)
                   .AddOption("I~see ", 255)
                   .AddAvatar(318).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.GoldenPhoenixCup)]
        public static void GoldenPhoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                  *Golden Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +1000					 Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +1000					 Lv.4 Draining Touch \n")
                            .AddText(" 					Defense +500			 Lv.2 Blood Spawn \n")
                            .AddText(" 					1% Blessed			    Lv.4 Free Soul \n")
                             .AddText("											Lv.4 Killing Flash \n")
                            .AddText(" 											Lv.3 Straight Life \n")
                             .AddText(" 										   HP +300 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Golden Phoenix Cup`s Duration: 90 days")
                   .AddOption("I~see ", 255)
                   .AddAvatar(317).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SilverPheoenixCup)]
        public static void SilverPheoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                   *Silver Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +900					Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +900					Lv.3 Draining Touch \n")
                            .AddText(" 					Defense +300		  Lv.1 Blood Spawn \n")
                            .AddText(" 					1% Blessed			  Lv.3 Free Soul \n")
                             .AddText("										         Lv.3 Killing Flash \n")
                            .AddText(" 										         Lv.2 Straight Life \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText("  				Silver Phoenix Cup`s Duration: 60 days")
                   .AddOption("I~see ", 255)
                   .AddAvatar(316).FinalizeDialog();


                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BronzePhoenixCup)]
        public static void BronzePhoenixCup(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" 					                   *Bronze Phoenix Cup* \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 					[Basic Attributes]				[Bonus Attributes If Tempered] \n")
                            .AddText(" 					HP +800					Lv.3 Toxin Eraser \n")
                            .AddText(" 					MP +800					Lv.2 Draining Touch \n")
                            .AddText(" 					Defense +150		  Lv.2 Killing Flash \n")
                            .AddText(" 					1% Blessed			  Lv. Free Soul2 \n")
                            .AddText(" ???????????????????????????????????????????????????????????? \n")
                            .AddText(" 				        Bronze Phoenix Cup`s Duration: 30 days")

                   .AddOption("I~see ", 255)
                   .AddAvatar(315).FinalizeDialog();


                        break;
                    }

            }
        }




        [NpcAttribute(NpcID.VoteManager)]
        public static void VoteManager(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.Level >= 100 && client.Player.Reborn >= 1)
                            data.AddText("Welcome to VoteManger seems like you are eligible for vote please do remember that you can vote only once at 12 hours in exchange of your Vote you will get 2 Hour Vip.")
                             .AddOption("Vote Server.", 1)
                             .AddOption("I`ll think about it.", 255);
                        else
                        {
                            data.AddText("I'm sorry,but it appears you don't meet the requirements to be eligible to vote. Please return when you reach level 100 after your first rebirth.");
                            data.AddOption("I~see.", 255);
                        }

                        data.AddAvatar(3)
             .FinalizeDialog();
                        break;
                    }


                case 1:
                    {
                        var SameHWID = Pool.GamePoll.Values.Where(p => p.OnLogin.HWID == client.OnLogin.HWID).ToArray();
                        var CantClaim = SameHWID.Where(p => p.Player.StartVote == true).ToArray();
                        if (CantClaim.Length > 0)
                        {
                            data.AddText("You have already claimed your reward for voting. You may only vote once every 12 hour.")
                          .AddOption("I~see.", 255)
                         .AddAvatar(323)
                         .FinalizeDialog();
                            break;

                        }
                        if (Database.VoteSystem.CanVote(client))
                        {

                            if (client.Player.VipLevel < 4)
                            {
                                client.SendSysMesage(Program.ServerConfig.XtremeTopLink, MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.white);
                                client.Player.StartVote = true;
                                client.Player.StartVoteStamp = DateTime.Now.AddSeconds(30);

                                client.CreateBoxDialog("Please wait for system to check your vote.");



                            }

                            else
                            {
                                data.AddText("You have already VIP When Your VIP Finish Come Agian")
                                 .AddOption("I~see.", 255)
                                .AddAvatar(323)
                                .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You have already claimed your reward for voting. You may only vote once every 12 hour.")
                          .AddOption("I~see.", 255)
                         .AddAvatar(323)
                         .FinalizeDialog();
                        }
                        break;
                    }





                case 99:
                    {

                        //nothing

                        break;
                    }
            }
        }







        //        [NpcAttribute(NpcID.SelectSacredRefineryPack)]
        //        public static void SelectSacredMaterial(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //        {
        //
        //            {
        //                if (!client.Inventory.Contain(3004249, 1))
        //                {
        //                    client.CreateBoxDialog("Sorry, you don`t have a SacredRefineryPack.");
        //                    return;
        //                }
        //            }
        //
        //
        //            Dialog data = new Dialog(client, stream);
        //            switch (Option)
        //            {
        //                case 0:
        //                    {
        //
        //                        {
        //                            data.AddText("Choose what Refineries with level 6 you need .")
        //                             .AddOption("Detoxication[Bag]. ", 32) //32
        //                             .AddOption("Detoxication[Necklace]. ", 33)
        //                             .AddOption("Breakthrough[Bracelet]. ", 34)
        //                             .AddOption("Breakthrough[Ring]. ", 36)
        //                             .AddOption("Penetration[Bracelet]. ", 37)
        //                             .AddOption("Detoxication[Armor]. ", 38)
        //                             .AddOption("Next Page.", 39)
        //                                   .AddAvatar(200)
        //                                  .FinalizeDialog();
        //                        }
        //
        //
        //                        break;
        //                    }
        //
        //
        //
        //
        //                #region null2
        //                case 31:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("Detoxication[Bag]. ", 32)
        //                        .AddOption("Detoxication[Necklace]. ", 33)
        //                        .AddOption("Breakthrough[Bracelet]. ", 34)
        //                        .AddOption("Breakthrough[Ring]. ", 36)
        //                        .AddOption("Penetration[Bracelet]. ", 37)
        //                        .AddOption("Detoxication[Armor]. ", 38)
        //                        .AddOption("Next Page.", 39)
        //
        //                          .AddAvatar(200)
        //                        .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 1
        //                case 39:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("Counteraction[Necklace]. ", 15)
        //                        .AddOption("Breakthrough[2-Handed]. ", 16)
        //                        .AddOption("CriticalStrike[Ring]. ", 17)
        //                        .AddOption("M-Defense[Bracelet]. ", 18)
        //                        .AddOption("CriticalStrike[2-Handed]. ", 19)
        //                        .AddOption("M-Defense[Ring]. ", 20)
        //                        .AddOption("Next Page.", 21)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 2
        //                case 21:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("Penetration[Bag]. ", 22)
        //                        .AddOption("Block[Shield]. ", 23)
        //                        .AddOption("Detoxication[Boots]. ", 24)
        //                        .AddOption("Counteraction[Bag]. ", 25)
        //                        .AddOption("Breakthrough[Bow]. ", 26)
        //                        .AddOption("Immunity[Boots]. ", 27)
        //                        .AddOption("Next Page.", 28)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 3
        //                case 28:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("SkillC.Strike[Bracelet]. ", 29)
        //                        .AddOption("CriticalStrike[Bow]. ", 30)
        //                        .AddOption("M-Defense[Bag]. ", 41)
        //                        .AddOption("Penetration[HeadGear]. ", 42)
        //                        .AddOption("Block[HeadGear]. ", 43)
        //                        .AddOption("Detoxication[Headgear]. ", 44)
        //                        .AddOption("Next Page.", 45)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 4
        //                case 45:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("SkillC.Strike[Bracelet]. ", 29)
        //                        .AddOption("CriticalStrike[Bow]. ", 30)
        //                        .AddOption("M-Defense[Bag]. ", 41)
        //                        .AddOption("Penetration[HeadGear]. ", 42)
        //                        .AddOption("Block[HeadGear]. ", 43)
        //                        .AddOption("Detoxication[Headgear]. ", 44)
        //                        .AddOption("Next Page.", 46)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 5
        //                case 46:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("Counteraction[Armor]. ", 47)
        //                        .AddOption("Breakthrough[1-Handed]. ", 48)
        //                        .AddOption("Intensification[Headgear]. ", 49)
        //                        .AddOption("Immunity[Armor]. ", 83)
        //                        .AddOption("SkillC.Strike[Backsword]. ", 84)
        //                        .AddOption("CriticalStrike[1-Handed]. ", 85)
        //                        .AddOption("Next Page.", 86)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 6
        //                case 86:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("M-Defense[Necklace]. ", 87)
        //                        .AddOption("SkillC.Strike[Backsword]. ", 88)
        //                        .AddOption("Penetration[Bracelet]. ", 89)
        //                        .AddOption("Detoxication[Bag]. ", 90)
        //                        .AddOption("Penetration[Headgear]. ", 91)
        //                        .AddOption("Detoxication[Necklace]. ", 92)
        //                        .AddOption("Next Page.", 93)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 7
        //                case 93:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("Detoxication[Boots]. ", 94)
        //                        .AddOption("Detoxication[Armor]. ", 95)
        //                        .AddOption("Detoxication[Headgear]. ", 96)
        //                        .AddOption("Block[Shield]. ", 97)
        //                        .AddOption("Breakthrough[Bow]. ", 101)
        //                        .AddOption("Breakthrough[2-Handed]. ", 102)
        //                        .AddOption("Next Page.", 103)
        //                          .AddAvatar(200)
        //                          .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //                #region Next Page 7
        //                case 103:
        //                    {
        //                        data.AddText("Choose what Refineries with level 6 you need .")
        //                        .AddOption("Breakthrough[1-Handed]. ", 104)
        //                        .AddOption("CriticalStrike[2-Handed]. ", 105)
        //                        .AddOption("CriticalStrike[1-Handed]. ", 106)
        //                        .AddOption("CriticalStrike[Bow]. ", 107)
        //                         .AddOption("MaxHP[Hossu]. ", 108)
        //                        .AddOption("First page.", 31) //aici
        //                          .AddAvatar(200)
        //                         .FinalizeDialog();
        //                        break;
        //                    }
        //                #endregion
        //
        //                #region Level 6 Refainaries.
        //                case 108: //MaxHP[Hossu]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //                        client.Inventory.Add(stream, 3006170, 1);
        //
        //
        //
        //                        break;
        //                    }
        //                case 107: //CriticalStrike[Bow]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //                        client.Inventory.Add(stream, 3004147, 1);
        //
        //
        //
        //                        break;
        //                    }
        //
        //                case 106: //CriticalStrike[1-Handed]
        //                    {
        //
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //                        client.Inventory.Add(stream, 3004137, 1);
        //
        //
        //                        break;
        //                    }
        //                case 105: //CriticalStrike[2-Handed]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //                        client.Inventory.Add(stream, 3004156, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 104: //Breakthrough[1-Handed]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //                        client.Inventory.Add(stream, 3004141, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 102: //Breakthrough[2-Handed]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004159, 1);
        //
        //
        //                        break;
        //                    }
        //                case 101: //Breakthrough[Bow].
        //                    {
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //
        //                        client.Inventory.Add(stream, 3004150, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 97: //Block[Shield].
        //                    {
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //
        //                        client.Inventory.Add(stream, 3004153, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 96: //Detoxication[Headgear]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //                        client.Inventory.Add(stream, 3004143, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 95: //Detoxication[Armor]
        //                    {
        //
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //                        client.Inventory.Add(stream, 3004161, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 94: //Detoxication[Boots]
        //                    {
        //
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004152, 1);
        //
        //
        //                        break;
        //                    }
        //                case 92: //Detoxication[Necklace]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004165, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 91: //Penetration[Headgear]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004145, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 90: //Detoxication[Bag]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004154, 1);
        //
        //                        break;
        //                    }
        //
        //
        //                case 89: //Penetration[Bracelet]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004162, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 88: //SkillC.Strike[Backsword]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004138, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 87: //M-Defense[Necklace]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004136, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 85: //CriticalStrike[1-Handed]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004137, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 84: //SkillC.Strike[Backsword].
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004138, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 83: //Immunity[Armor].
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004139, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 49: //Intensification[Headgear]
        //                    {
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //                        client.Inventory.Add(stream, 3004140, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 48: //Breakthrough[1-Handed]
        //                    {
        //
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004141, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 47: //Counteraction[Armor]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //                        client.Inventory.Add(stream, 3004142, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 44: //Detoxication[HeadGear]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004143, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 43: //Block[HeadGear]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004144, 1);
        //
        //
        //                        break;
        //                    }
        //
        //
        //                case 42: //Penetration[HeadGear]
        //                    {
        //
        //
        //                        client.Inventory.Remove(3004249, 1, stream);
        //                        client.CreateBoxDialog("You received 1 Sacred Refinery Material.");
        //
        //
        //                        client.Inventory.Add(stream, 3004145, 1);
        //
        //
        //                        break;
        //                    }
        //
        //                case 41: //M-Defense[Bag]
        //                    {
        //
        //
        //
        //                        c    
        public static void KingDoomCrystals(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            //To claim a bounty, you should kill 50 enemies in the Realm.
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Crystal, client.Player.Class, 35028);

            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You`ve already claimed a bounty today. No more to claim.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255)
                          .FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 50))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.CreateBoxDialog("You`ve restored peace to the Realm by killing to enemy soldiers, and received a Realm Crystal Fragment.");

                            }
                            else
                                client.CreateBoxDialog("To claim a bounty, you should kill 50 enemies in the Realm.");
                        }
                        else
                            client.CreateBoxDialog("To claim a bounty, you should kill 50 enemies in the Realm.");

                        break;
                    }
            }

        }
        //[NpcAttribute(NpcID.GuildChief)]
        //public static void GuildChief(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    /* if (!client.Inventory.Contain(723467, 1) && !client.Inventory.Contain(2100085, 1))
        //     {
        //         data.AddText("7234102_0101@@")
        //                       .AddOption("iSee.", 255)
        //                       .FinalizeDialog();
        //         return;
        //     }*/

        //    switch (Option)
        //    {
        //        case 0:
        //            {

        //                data.AddText("7234120_0101@@")
        //                    .AddText("7234121_0101@@")
        //                     .AddText("7234122_0101@@")
        //                      .AddText("7234123_0101@@")
        //                  .AddOption("Tell me more.", 255)
        //                                   .AddOption("i want to claim my reward.", 1)
        //                  .AddOption("Leave.", 255)
        //                  .FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                data.AddText("7234200_0101@@")
        //                    .AddText("7234201_0101@@")
        //                     .AddText("7234202_0101@@")
        //                  .AddOption("7234204_0102@@", 2)
        //                            .AddOption("7234205_0102@@", 3)
        //                                      .AddOption("7234206_0102@@", 4)
        //                                         .AddOption("7234207_0102@@", 5)
        //                  .AddOption("Let me think over.", 255)
        //                  .FinalizeDialog();
        //                break;
        //            }
        //    }
        //}

        [NpcAttribute(NpcID.SashFragment_Realm)]
        public static void SashFragment_Realm(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(3600024, 1))
            {
                client.CreateBoxDialog("Sorry, you don`t have a SashFragment(Realm).");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("You`ll have a 20% chance to get a Sash (S) if using this fragment alone, or lose it if failed.")
                            .AddText("~While if you have 5 fragments, you can surely combine them into a Sash (S). How many fragments would you like to use?")
                          .AddOption("1~fragment.~(20%~chance)", 1)
                          .AddOption("Combine~5~fragments.", 10)
                          .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(3600024, 1, 0))
                        {
                            client.Inventory.Remove(3600024, 5, stream);
                            client.Inventory.Add(stream, 1100003);
                            data.AddText("You successfully made a Sash (S)!")
                  .AddOption("Great!", 255)
                  .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You should have 5 Sash Fragments to combine them into a Sash (S). Please retry when you have enough fragments.")
                   .AddOption("Alright.", 255)
                   .FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .FinalizeDialog();
                            break;
                        }
                        client.Inventory.Remove(3600024, 1, stream);
                        if (Role.Core.Rate(20))
                        {
                            client.Inventory.Add(stream, 1100003);
                            data.AddText("Wow, it must be your lucky day! You successfully made a Sash (S)!")
                   .AddOption("Great!", 255)
                   .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The fragment vanished since you failed to make a Sash (S) with it.")
                    .AddOption("Great!", 255)
                    .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.HpSeller)]
        public static void HpSeller(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            ushort num;
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    dialog.AddText("Hello Man Select 10,000 MP Or HP For [ Free ] You Need NightMareFo222.");
                    dialog.AddOption("Buy  Hp ", 1);
                    dialog.AddOption("Buy  Mp ", 3);
                    dialog.AddOption("Lets~rock! ", 0xff);
                    dialog.AddAvatar(0x3f).FinalizeDialog(false);
                    break;

                case 1:
                    dialog.AddText("Enter the amount You Want.");
                    dialog.AddInput("Hp Amount .", 2);
                    dialog.AddOption("Just~passing~by.", 0xff);
                    dialog.AddAvatar(6);
                    dialog.AddAvatar(0x3f).FinalizeDialog(false);
                    break;

                case 2:
                    if (ushort.TryParse(Input, out num))
                    {
                        if (num >= 1)
                        {
                            if (num > 10000)
                            {
                                dialog.AddText("You can not input more than 10000 Hp..");
                                dialog.AddInput("Hp  Amount ", 2);
                                dialog.AddOption("No thank you.", 0xff);
                                dialog.AddAvatar(0x30).FinalizeDialog(false);
                            }
                            else if (!client.Inventory.HaveSpace(1))
                            {
                                client.SendSysMesage("Please Free up some space 1 Slots.", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, false);
                                client.Player.MessageBox("Character must have 1 free slots into inventory", null, null, 0, MsgStaticMessage.Messages.None);
                            }
                            else
                            {
                                client.Inventory.AddItemWitchStack(0xf4e02, 0, num, stream, false);
                            }
                            break;
                        }
                        dialog.AddText("You need to input more or atleast 1 Hp..");
                        dialog.AddInput("Hp  Amount ", 2);
                        dialog.AddOption("No thank you.", 0xff);
                        dialog.AddAvatar(0x30).FinalizeDialog(false);
                    }
                    break;

                case 3:
                    dialog.AddText("Enter the amount You Want.");
                    dialog.AddInput("Mp # Amount #.", 4);
                    dialog.AddOption("Just~passing~by.", 0xff);
                    dialog.AddAvatar(6);
                    dialog.AddAvatar(0x3f).FinalizeDialog(false);
                    break;

                case 4:
                    if (ushort.TryParse(Input, out num))
                    {
                        if (num >= 1)
                        {
                            if (num > 10000)
                            {
                                dialog.AddText("You can not input more than 10000 Mp..");
                                dialog.AddInput("Mp # Amount #", 2);
                                dialog.AddOption("No thank you.", 0xff);
                                dialog.AddAvatar(0x30).FinalizeDialog(false);
                            }
                            else if (!client.Inventory.HaveSpace(1))
                            {
                                client.SendSysMesage("Please Free up some space 1 Slots.", MsgMessage.ChatMode.Talk, MsgMessage.MsgColor.white, false);
                                client.Player.MessageBox("Character must have 1 free slots into inventory", null, null, 0, MsgStaticMessage.Messages.None);
                            }
                            else
                            {
                                client.Inventory.AddItemWitchStack(0xf51ea, 0, num, stream, false);
                            }
                            break;
                        }
                        dialog.AddText("You need to input more or atleast 1 Mp..");
                        dialog.AddInput("Mp # Amount #", 2);
                        dialog.AddOption("No thank you.", 0xff);
                        dialog.AddAvatar(0x30).FinalizeDialog(false);
                    }
                    break;
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide1)]
        public static void ExplosiveDevide1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");

                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide2)]
        public static void ExplosiveDevide2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");
                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide3)]
        public static void ExplosiveDevide3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");
                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ExplosiveDevide4)]
        public static void ExplosiveDevide4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                client.CreateBoxDialog("The Deserted Village has been burnt down into ashes. Report back to the Kingdom Mission Envoy and claim your reward.");
                            }
                            else
                                client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);
                        }
                        else
                            client.SendSysMesage("You've already enabled the Explosive Device today. No need to do it again.", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.RealmEnvoy)]
        public static void RealmEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.ServerID == Database.GroupServerList.MyServerInfo.ID)
                        {
                            data.AddText("I`m serving the Realm. After you travel to other servers, you can find me to return to the Realm.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(43).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            if (client.Player.OnTransform)
                            {
                                client.Player.TransformInfo.FinishTransform();
                            }
                            data.AddText("I have the code to enter the Realm. If you want to return to the Realm, i can help you.");
                            data.AddOption("Return to the Realm.", 1)
                                .AddOption("I see.", 255)
                               .AddAvatar(43).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        client.Send(stream.ConnectionInfoCreate(1, Database.GroupServerList.InterServer.ID));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KingdomMissionEnvoy)]
        public static void KingdomMissionEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35024);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35007);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Crystal, client.Player.Class, 35028);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35025);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KingdomMissionEnvoy, client.Player.Class, 35034);


            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.OnMyOwnServer == false)
                        {
                            data.AddText("I have the code to enter the Realm. If you want to return to the Realm, i can help you.");
                            data.AddOption("Return to the Realm.", 2)
                                .AddOption("I see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level >= 110 || client.Player.Reborn >= 1)
                        {
                            data.AddText("A real hero must harbor great ambition for his kingdom.")
                                .AddText("~In the name of conquer, I`m calling heroes who`ve reached Level 110 of the 1st rebirth.")
                                .AddText("~for a series of cross-server missions. Go to the Realm, and you`ll find a way to explore")
                                .AddText("~a new world out of this world.")
                                .AddText("\nYou`ve earned " + client.Player.KingDomDeeds.ToString() + " points of Kingdom Deeds.");
                            data.AddOption("Enter~the~Realm.", 1)
                                //data.AddOption("Enter~the~Realm.", 255)
                               .AddOption("Kingdom~Mission.", 10)
                             .AddOption("Submit~Gold~Bricks.~(special)", 20)
                             .AddOption("Swap~deeds~for~rewards.", 30)
                             .AddOption("How~to~travel~across~server?", 40)
                             .AddOption("What~should~I~pay~attention~to?", 50)
                             .AddOption("I~see", 255)
                            .AddAvatar(6).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You should reach at Level 110 of the 1st rebirth before you can enter the Realm.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {

                        if (client.Player.InUnion)
                            client.Send(stream.HandBrickInfoCreate(MsgHandBrickInfo.BrickInfo.ShowSubmitDialog, client.Player.MyUnion.GoldBrick, 2));
                        else
                            client.Send(stream.HandBrickInfoCreate(MsgHandBrickInfo.BrickInfo.ShowSubmitDialog, 0, 2));
                        break;
                    }

                case 30:
                    {
                        data.AddText("You can collect up to 300 Kingdom Deeds in a day from the Kingdom Mission. If you`ve earned 50, 150 or 300 points,")
                            .AddText("~you can swap them for corresponding reward with me. So, which pack would you like to claim? \n")
                            .AddText(" Your Kingdom Deeds: " + client.Player.KingDomDeeds + ".");
                        data.AddOption("Outstanding~Exploit~Pack.~(50)", 31)
                            .AddOption("Brilliant~Exploit~Pack.~(150)", 32)
                            .AddOption("Supreme~Exploit~Pack.~(300)", 33)
                            .AddOption("Buy~Kingdom~Deeds.", 34)
                            .AddOption("What`re~in~the~packs?", 37)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 37:
                    {
                        data.AddText("You can swap 50 Kingdom Deeds for an Outstanding Exploit Pack: 50 War Exploits, 400 Potency Points and 50 points for Union Fund;")
                            .AddText("~150 Kingdom Deeds for a Brilliant Exploit Pack: 50 Champion Points (excluded if maximum is reached), 100 War Exploits, 600 Potency Points and 160 points to Union Fund;")
                            .AddText("~300 Kingdom Deeds for a Supreme Exploit Pack: 100 Champion Points (excluded if maximum is reached), 150 War Exploits, 1000 Potency Points and 360 points to Union Fund.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("So far, you`ve earned " + client.Player.KingDomDeeds + " Kingdom Deeds. If you want it full of 300 points immediately, you need to pay CPs. The costs: \n")
                        .AddText("     0 - 49 Kingdom Deeds: 990 CPs; 50 - 149 Kingdom Deeds: 540 CPs;  150 - 299 Kingdom Deeds: 270 CPs.");
                        data.AddOption("Pay~CPs~to~fill~up~with~deeds.", 35)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        uint Cost = 990;

                        if (client.Player.KingDomDeeds >= 159)
                            Cost = 270;
                        else if (client.Player.KingDomDeeds >= 50)
                            Cost = 540;

                        if (client.Player.BuyKingdomDeeds != 0)
                        {
                            data.AddText("You already bought one today , please come back tomorrow .");
                            data.AddOption("Okay.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            return;
                        }

                        data.AddText("Are you sure you want to pay " + Cost + " CPs to fill up your Kingdom Deed?");
                        data.AddOption("Yes.", 36)
                            .AddOption("Wait,~I~haven`t~decided.", 255)
                           .AddAvatar(6).FinalizeDialog();



                        break;
                    }
                case 36:
                    {
                        uint Cost = 990;

                        if (client.Player.KingDomDeeds >= 159)
                            Cost = 270;
                        else if (client.Player.KingDomDeeds >= 50)
                            Cost = 540;
                        if (client.Player.ConquerPoints >= Cost)
                        {
                            if (client.InTrade) return;
                            client.Player.BuyKingdomDeeds = 1;
                            int Amount = (int)Cost;
                            if (Amount < 0)
                                return;
                            client.Player.ConquerPoints -= (uint)Cost;
                            client.Player.KingDomDeeds = 300;
                            client.CreateBoxDialog("You've received 300 KingDom Deeds.");
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have enough CPs to fill up your Kingdom Deed.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 33:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.KingDomDeeds >= 300)
                        {
                            client.Player.KingDomDeeds -= 300;
                            client.Inventory.Add(stream, 3007110);
                            client.CreateBoxDialog("You received a Supreme Exploit Pack.");
                        }
                        else
                        {
                            data.AddText("Sorry, you can`t claim this pack since you don`t have 150 Kingdom Deeds. Go complete the Kingdom Mission to earn more Kingdom Deeds.");
                            data.AddOption("Alright.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.KingDomDeeds >= 150)
                        {
                            client.Player.KingDomDeeds -= 150;
                            client.Inventory.Add(stream, 3007109);
                            client.CreateBoxDialog("You received a Brilliant Exploit Pack.");
                        }
                        else
                        {
                            data.AddText("Sorry, you can`t claim this pack since you don`t have 150 Kingdom Deeds. Go complete the Kingdom Mission to earn more Kingdom Deeds.");
                            data.AddOption("Alright.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.KingDomDeeds >= 50)
                        {
                            client.Player.KingDomDeeds -= 50;
                            client.Inventory.Add(stream, 3007108);
                            client.CreateBoxDialog("You received a Outstanding Exploit Pack.");
                        }
                        else
                        {
                            data.AddText("Sorry, you can`t claim this pack since you don`t have 50 Kingdom Deeds. Go complete the Kingdom Mission to earn more Kingdom Deeds.");
                            data.AddOption("Alright.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("A real hero must harbor great ambition for his kingdom. In the name of Conquer, I`m calling heroes who`ve reached Level 110 of the 1st rebirth.")
                            .AddText("~for a series of cross-server missions. Go abroad, and explore a new world out of this world.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Beasts~of~Legend(completed)", 90);
                        else
                            data.AddOption("[Realm]~Beasts~of~Legend.", 90);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Thunder~Strike(completed)", 60);
                        else
                            data.AddOption("[Realm]~Thunder~Strike.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Ashes~of~Anger(completed)", 11);
                        else
                            data.AddOption("[Realm]~Ashes~of~Anger.", 11);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Scramble~for~Justice(completed)", 80);
                        else
                            data.AddOption("[Realm]~Scramble~for~Justice.", 80);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Crystal~Bounty(completed)", 70);
                        else
                            data.AddOption("[Realm]~Crystal~Bounty.", 70);
                        data.AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("A horde of bloodthirsty devils have occupied the Deserted Village in the Realm. They built a weird array with 4 Dark Crystals,")
                        .AddText("~guarding their leader Alluring Witch at the center. We`re calling heroes to team up and behead the horrific Alluring Witch")
                    .AddText("~after breaking through her magic array. Tons of rewards will be offered to the contributors.");
                        data.AddOption("Count~on~me!", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        data.AddText("The Realm has received countless threats, but this time seems different. The enemies are so manic that we`re")
                         .AddText("~in the most urgent need of helpers. After killing a certain number of enemies, you can claim a starry")
                     .AddText("<tip color=0xfff05050 desc=A#chance#to#get#Dragon#Ball#Piece,#Savage#Bone,#Chi#Points,#Frozen#Chi#Pill,#etc.,#once#a#day.>bounty</tip> from the Realm Crystal.");
                        data.AddOption("Count~on~me!", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 90:
                    {
                        data.AddText("Unbelievable! The four mythological creatures make a rare appearance together in the Realm.")
                           .AddText("~Right now, go defeat 3 of their defenders, and you can challenge these divine beasts who carry")
                       .AddText("<tip color=0xfff05050 desc=Eminent#Exploit#Pack,#Protection#Pill,#Fierce#Spirit,#200-min#EXP,#30#Kingdom#Deeds,#50#Champion#Points#and#500#Horse#Racing#Points.> invaluable treasures</tip>.");
                        data.AddOption("Claim~my~rewards.", 91)
                            .AddOption("I`m~not~ready~yet!", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 91:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already claimed the reward for the [Beasts of Legend] mission in the Realm.");
                            data.AddOption("Sorry,~I~forgot.", 255)
                               .AddAvatar(6).FinalizeDialog();

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            == false && client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest4.MissionId, 1))
                        {
                            client.Inventory.Add(stream, 3200005);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId);
                            data.AddText("Your loyalty to your kingdom is quite strong. Please accept this reward.");
                            data.AddOption("Thanks!", 255)
                               .AddAvatar(6).FinalizeDialog();
                            client.SendSysMesage("You've received a bounty of (Divine Conquest Pack) for conquering the divine beast! ", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("To claim the reward, you should conquer the divine beast first.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 60:
                    {
                        data.AddText("While in a battle against millions of enemies, striking first is the key to gain initiative. Hurry up and disable the enemy soldiers, officers, and their Kylin Chariots in the Realm!")
                             .AddText("~When you collect 300 Strike Points, a wonderful<tip color=0xfff05050 desc=Eminent#Exploit#Pack,#Free#Training#Pill,#or#other#rewards#like#Chi#Points#&#300-min#EXP,#90#Kingdom#Deeds,#and#50#Champion#Points.>bounty</tip>")
                         .AddText("~will be yours. You can also earn War Exploits by destroying the enemy`s Defensive Barrier, Dark Crystal, or beheading their Military Officer.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddOption("Learn~about~other~things.", 10).AddOption("I`m~not~ready~yet!", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        data.AddOption("Claim~my~bounty.", 61)
                            .AddOption("Directly~complete~it.~(500~CPs)", 62)
                            .AddOption("Learn~about~other~things.", 10)
                            .AddOption("I`m~not~ready~yet!", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 62:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            break;
                        }
                        if (client.Player.ConquerPoints >= 500)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 500;
                            MsgQuestList.QuestListItem _quest;
                            client.Player.QuestGUI.AcceptKingDomMission(ActiveQuest2, 0, out _quest);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.Inventory.Add(stream, 3200000);
                            data.AddText("Your loyalty to your kingdom is quite strong. Please accept this reward.");
                            data.AddOption("Thanks!", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.");
                            data.AddOption("Okay.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;

                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            == false && client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest2.MissionId, 300))
                        {
                            client.Inventory.Add(stream, 3200000);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            data.AddText("Your loyalty to your kingdom is quite strong. Please accept this reward.");
                            data.AddOption("Thanks!", 255)
                               .AddAvatar(6).FinalizeDialog();
                            client.SendSysMesage("You've completed the mission [Thunder~Strike]. Hurry and check your reward in the inventory.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("To claim a reward, you should collect 300 Strike Points, first. Fight for your home, and kill enemies in the Realm!");
                            data.AddOption("Go~fight!", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        data.AddText("In the Realm, the bloodthirsty devils reside in the Deserted Village and start killing straggling soldiers.")
                               .AddText("~Their cruelty cannot be forgiven. You`re asked to ignite the Explosive Device and burn the Deserted")
                           .AddText("~Village into ashes. If succeed, you can claim a valuable reward.");
                        data.AddOption("Claim~my~rewards.", 12)
                            .AddOption("Count~on~me!", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already claimed the reward for the [Ashes of Anger] mission in the Realm.");
                            data.AddOption("Sorry,~I~forgot.", 255)
                               .AddAvatar(6).FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            == false && client.Player.QuestGUI.CheckKingDomQuest(ActiveQuest.MissionId, 1))
                        {
                            client.Inventory.Add(stream, 3200004);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.SendSysMesage("You've completed the mission [Realm~Ashes of Anger]. Hurry and check your reward in the inventory.", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            data.AddText("You haven`t destroyed Deserted Village. Please finish your job, first.");
                            data.AddOption("Okay.", 255)
                               .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Send(stream.ConnectionInfoCreate(1, Database.GroupServerList.InterServer.ID));
                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        client.SendSysMesage("Pay attention you won`t be able to attack the players in other servers if you travel there bevond Plumber War(Tuesday, Wednesdat, Thursday, Saturday and Sunday)!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        MsgInterServer.PipeClient.Connect(client, Database.GroupServerList.InterServer.IPAddress, Database.GroupServerList.InterServer.Port);



                        break;
                    }
                case 40:
                    {
                        data.AddText("To travel to another server, you need to find me to enter the Realm first.")
                               .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm.")
                               .AddText("~It`s the same when you return. First, go transit in the Realm, and then find the Keeper of your Server.")
                               .AddText("~Please learn all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 41)
                            .AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                                 .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the Server. During the Plunder War, you can travel to other servers through the Realm.")
                                 .AddText("~only chat and PK are available now. The chat channels include Talk, Whisper, Team, Guild, Clan, Friend, Service, World and Ally,")
                                 .AddText("~Please note that, if you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~will~be~unavailable?", 42)
                            .AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions concerning money, item trade, forging,")
                                   .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP matches.")
                                   .AddText("~Things will get right after I send you back to your original Server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~~mentor and apprentice, and bonus BP of guild and clan will also be registered in the Server. During the Plunder War, you can travel to other servers through the Realm.")
                             .AddText("~only chat and PK are available now. The chat channels include Talk, Whisper, Team, Guild, Clan, Friend, Service, World and Ally,")
                             .AddText("~Please note that, if you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~will~be~unavailable?", 51)
                            .AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions concerning money, item trade, forging,")
                                   .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP matches.")
                                   .AddText("~Things will get right after I send you back to your original Server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.PokerMillionaireLee)]
        //public static void PokerMillionaireLee(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {

        //                data.AddText("STR_ID_tCrossPorkerPack[25818][Text111]@@")
        //                    .AddText("STR_ID_tCrossPorkerPack[25818][Text112]@@");
        //                data.AddOption("STR_ID_tCrossPorkerPack[25818][Option111]@@", 1)
        //                    .AddOption("STR_ID_tCrossPorkerPack[25818][Option112]@@.", 255)
        //                   .AddAvatar(7).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                data.AddText("Good choice! You can just pay 100,000 Silver to pack 1,228,800,000 Silver into a PokerCard A or pack");
        //                data.AddText("~614,400,000 Silver into a PokerCard B.");
        //                data.AddOption("PokerCard A.", 2);
        //                data.AddOption("PokerCard B.", 3);
        //                data.AddAvatar(7).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                data.AddText("Good choice! You can just pay 100,000 Silver to pack 1,228,800,000 Silver into a PokerCard A or pack");
        //                data.AddText("~614,400,000 Silver into a PokerCard B.");
        //                data.AddOption("1.", 4);
        //                data.AddOption("10.", 5);
        //                data.AddOption("81.", 6);
        //                data.AddAvatar(7).FinalizeDialog();
        //                break;
        //            }
        //        case 3:
        //            {
        //                data.AddText("Good choice! You can just pay 100,000 Silver to pack 1,228,800,000 Silver into a PokerCard A or pack");
        //                data.AddText("~614,400,000 Silver into a PokerCard B.");
        //                data.AddOption("1.", 7);
        //                data.AddOption("10.", 8);
        //                data.AddOption("81.", 9);
        //                data.AddAvatar(7).FinalizeDialog();

        //                break;
        //            }
        //        case 4:
        //            {

        //                if (!client.Player.IsCheckedPass)
        //                    return;
        //                if (client.Inventory.HaveSpace(2) == false)
        //                {
        //                    data.AddText("Please make 2 more space in your inventory.");
        //                    data.AddOption("Let me check.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.Money >= 1228800000 + 100000)
        //                {
        //                    client.Player.Money -= 1228800000 + 100000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.Inventory.AddItemWitchStack(3390084, 0, 1, stream);
        //                    client.CreateBoxDialog("You Get 1 " + Pool.ItemsBase[3390084].Name + "");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, you don`t have 1,228,800,000 Silver in total.");
        //                    data.AddOption("My~bad.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 5:
        //            {

        //                if (!client.Player.IsCheckedPass)
        //                    return;
        //                if (client.Inventory.HaveSpace(2) == false)
        //                {
        //                    data.AddText("Please make 2 more space in your inventory.");
        //                    data.AddOption("Let me check.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.Money >= (long)1228800000 * 10 + 100000)
        //                {
        //                    client.Player.Money -= (long)1228800000 * 10 + 100000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.Inventory.AddItemWitchStack(3390084, 0, 10, stream);
        //                    client.CreateBoxDialog("You Get 10 " + Pool.ItemsBase[3390084].Name + "");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, you don`t have " + (long)1228800000 * 10 + " Silver in total.");
        //                    data.AddOption("My~bad.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 6:
        //            {

        //                if (!client.Player.IsCheckedPass)
        //                    return;
        //                if (client.Inventory.HaveSpace(2) == false)
        //                {
        //                    data.AddText("Please make 2 more space in your inventory.");
        //                    data.AddOption("Let me check.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.Money >= (long)1228800000 * 81 + 100000)
        //                {
        //                    client.Player.Money -= (long)1228800000 * 81 + 100000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.Inventory.AddItemWitchStack(3390084, 0, 81, stream);
        //                    client.CreateBoxDialog("You Get 81 " + Pool.ItemsBase[3390084].Name + "");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, you don`t have " + (long)1228800000 * 81 + " Silver in total.");
        //                    data.AddOption("My~bad.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 7:
        //            {

        //                if (!client.Player.IsCheckedPass)
        //                    return;
        //                if (client.Inventory.HaveSpace(2) == false)
        //                {
        //                    data.AddText("Please make 2 more space in your inventory.");
        //                    data.AddOption("Let me check.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.Money >= 614400000  + 100000)
        //                {
        //                    client.Player.Money -= 614400000 + 100000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.Inventory.AddItemWitchStack(3390083, 0, 1, stream);
        //                    client.CreateBoxDialog("You Get 1 " + Pool.ItemsBase[3390083].Name + "");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, you don`t have 614,400,000 Silver in total.");
        //                    data.AddOption("My~bad.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 8:
        //            {

        //                if (!client.Player.IsCheckedPass)
        //                    return;
        //                if (client.Inventory.HaveSpace(2) == false)
        //                {
        //                    data.AddText("Please make 2 more space in your inventory.");
        //                    data.AddOption("Let me check.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.Money >= (long)614400000 * 10 + 100000)
        //                {
        //                    client.Player.Money -= (long)614400000 * 10 + 100000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.Inventory.AddItemWitchStack(3390083, 0, 10, stream);
        //                    client.CreateBoxDialog("You Get 10 " + Pool.ItemsBase[3390083].Name + "");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, you don`t have " + (long)614400000 * 10 + " Silver in total.");
        //                    data.AddOption("My~bad.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 9:
        //            {

        //                if (!client.Player.IsCheckedPass)
        //                    return;
        //                if (client.Inventory.HaveSpace(2) == false)
        //                {
        //                    data.AddText("Please make 2 more space in your inventory.");
        //                    data.AddOption("Let me check.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                    break;
        //                }
        //                if (client.Player.Money >= (long)614400000 * 81 + 100000)
        //                {
        //                    client.Player.Money -= (long)614400000 * 81 + 100000;
        //                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
        //                    client.Inventory.AddItemWitchStack(3390083, 0, 81, stream);
        //                    client.CreateBoxDialog("You Get 81 " + Pool.ItemsBase[3390083].Name + "");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, you don`t have " + (long)614400000 * 81 + " Silver in total.");
        //                    data.AddOption("My~bad.", 255)
        //                       .AddAvatar(7).FinalizeDialog();
        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.AuntPeach)]
        public static void AuntPeach(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1806);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntPeach, client.Player.Class, 1807);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("When we two parted, in silence and tears, my dear. The dew of the morning, stuck chilled. It felt like the")
                            .AddText("~warning of what I feel now. How should I greet thee? With silence and tears.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Do~you~know~the~bag?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~the~Blunt~Sword.", 4);


                        data.AddOption("Beautiful.", 255).AddAvatar(186).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        data.AddText("Oh, this is the Peach Bag I made for my husband. Where did you get it? He left twenty years ago... No, he will come")
                            .AddText("~back...He promised...no...");
                        data.AddOption("Take~this~Bag.", 2)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("As you may have guessed by now, Obscure Warrior is my husband. Twenty years ago, he left for a duel, but he never")
                       .AddText("~came back ever since then. I made this Peach bag. Look, there is a note inside it.");
                        data.AddOption("Let~me~see.", 3)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Aunt~Peach~found~a~Note~in~the~Peach~Bag~and~gave~it~to~you.");
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(721842, 1, stream);
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received EXP worth 1 EXP Ball and 6,000 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("I will do what my husband told me to. As for the Divine Sword, oh, there is never such thing.")
                     .AddText("~My husband excels in using sword, and that Divine Sword they were talking about was only a normal sword.");
                        data.AddOption("Brilliant.", 4)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Thank you for bringing the message. Please take this as your reward.");

                        data.AddOption("My~pleasure.", 5)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("You know my feelings, right? Leave me alone, please... Oh, by the way, you can claim your reward from Taskmaster Chang.");

                        data.AddOption("OK,~I~will.", 6)
                            .AddOption("I`m~leaving.", 255)
                           .AddAvatar(186).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Hurry~to~claim~your~reward~from~Taskmaster~Chang!");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ObscureWarrior)]
        public static void ObscureWarrior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1801);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1802);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1803);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1804);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1805);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1806);

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m like a candle in the wind. But you are so young, so vigorous...I have engraved the essence of my blade skills on the four")
                            .AddText("~columns around you. You may click them to get some experience.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("How~long~have~you~been~here?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~the~Blunt~Sword.", 3);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                              client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~can~I~do~for~you?", 5);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~can~I~do~for~you?", 7);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                           client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~something~for~you.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true &&
                         client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~got~something~for~you.", 11);


                        data.AddOption("Okay.", 255).AddAvatar(57).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Inventory.Remove(721841, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("I thought Blade Han would keep his word...");
                        data.AddOption("Harvey`s~Letter,~for~you.", 11)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("So I have been waiting for nothing for 20 years? Sigh... This is my last wish. Will you take this Peach Bag to Aunt Peach on Bird Island (765,602)? She will tell you more about")
                            .AddText("~the Divine Sword.");
                        data.AddOption("Gotcha.", 12)
                            .AddOption("Later.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721842, 1) == false)
                            client.Inventory.Add(stream, 721842);

                        client.Teleport(503, 652, 1000);
                        client.CreateBoxDialog("Obscure~Warrior~took~out~a~Peach~Bag,~gave~it~to~you,~and~teleported~you~to~Desert~City.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(721843, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Haha, I have been here since 20 years ago. Someone finally came...");
                        data.AddOption("I`m~looking~for~you.", 2)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("One`s success relies on thousands of deaths. I have been looking forward for that duel for years... But it just never occurred...");
                        data.AddOption("Tell~me~about~Divine~Sword.", 3)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 200)
                                && client.Inventory.Contain(721845, 1))
                            {

                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(721845, 1, stream);
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                }
                                data.AddText("My hero, you are as might as your fame suggests!");
                                data.AddOption("Thank~you.", 5)
                                   .AddAvatar(57).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hey you! Why don`t you kill 200 Blade Ghosts L83 and take back the Blunt Sword first?!");
                                data.AddOption("Okay.", 4)
                                    .AddOption("I~am~kinda~busy~right~now.", 255)
                                   .AddAvatar(57).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Divine Sword? Of course, what else can you want from me... Well, it`s in the hands of the Blade Ghosts in Desert (165,276),")
                            .AddText("~but turned into Blunt Sword. Kill 200 Blade Ghosts and take Blunt Sword back to me, maybe I will do something about it.");

                        data.AddOption("I`m~in!", 4)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(165, 271, 1000);
                        client.CreateBoxDialog("After~a~dazzling~flash,~you~arrived~at~Desert~(165,276).~It`s~about~time~to~conquer~those~stupid~Blade~Ghosts!");

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721846, 5))
                            {
                                data.AddText("Good job! I knew I could count on you!");
                                data.AddOption("No~problem.", 7)
                                   .AddAvatar(57).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Well, I need 5 Blade Ghost Fangs. It seems that you haven`t brought enough of them.");
                                data.AddOption("Send~me~back~to~Desert.", 6)
                                    .AddOption("I~am~kinda~busy~right~now.", 255)
                                   .AddAvatar(57).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("Now, I need 5 Blade Ghost Fangs to sharpen the Sword and restore")
                            .AddText("~its power. You will do me the favor?");
                        data.AddOption("Of~course.", 6)
                            .AddOption("Sorry,~I~can`t~help.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Teleport(165, 271, 1000);
                        client.CreateBoxDialog("You~have~been~teleported~to~the~Level~83~Blade~Ghosts.");
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721846, 5))
                            {

                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.Inventory.Remove(721846, 5, stream);
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                }
                            }
                        }
                        data.AddText("I see a hero in you, my child, just like me when I was young... so ambitious, so bold... Oh, if only I could defeat Blade Han!");
                        data.AddOption("Who~is~it?", 8)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Blade Han is famous for his `Fervent Blade`. We had a rendezvous here, to set a duel. I have been waiting for years...")
                            .AddText("~Will you find him for me?");
                        data.AddOption("I~should~visit~Harvey.", 9)
                            .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(57).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        client.CreateBoxDialog("Weird,~Blade~Han~looked~just~like~Harvey~in~Desert~City~(495,579).~Find~him~to~learn~more.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Teleport(503, 652, 1000);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn1)]
        public static void StoneColumn1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn2)]
        public static void StoneColumn2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 0, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 0, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn3)]
        public static void StoneColumn3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 0, 0, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 0, 0, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn4)]
        public static void StoneColumn4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { Intentions = 4, MissionId = 11 }, 0);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(11, 0, 0, 0, 1) || client.Player.QuestGUI.CheckQuest(11, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You have already learnt these inscriptions. You can`t learn them again.");
                            data.AddOption("All~right~then.", 255).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, 11, 0, 0, 0, 1);
                            data.AddText("You obtained experience worth 1/12 of an EXP Ball after reading those inscriptions on the Stone Columns.");
                            data.FinalizeDialog();
                            client.GainExpBall(100, false);
                            if (client.Player.QuestGUI.CheckObjectives(11, 1, 1, 1, 1))
                                client.Player.QuestGUI.FinishQuest(11);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneColumn)]
        public static void StoneColumn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1801);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntPeach, client.Player.Class, 1807);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You need to post the Stone Spell onto the Stone Column to see what will happen.");

                        if ((client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Place~the~spell.", 1);



                        data.AddOption("Not~now.", 255).FinalizeDialog();


                        break;
                    }
                //case 1:
                //    {
                //        client.CreateBoxDialog("You~opened~your~eyes~and~find~yourself~in~the~Crypt.");
                //        client.Teleport(52, 57, 1801);
                //        break;
                //    }
            }
        }
        [NpcAttribute(NpcID.Harvey)]
        public static void Harvey(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1800);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1801);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneColumn, client.Player.Class, 1804);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harvey, client.Player.Class, 1805);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Have you ever seen an isis oasis? That is a sea of green, interspersed with towering, rock mountains and surrounded")
                           .AddText("~by the formidable Desert. Maybe only the legendary Obscure Warrior can survive the daunting Desert.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Who`s~Obscure~Warrior?", 2);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("You~know~Blade~Han?", 6);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Who`s~Obscure~Warrior?", 7);


                        data.AddOption("You~bet!", 255)
                        .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Well, yes, that is my father. He was famous for wielding blades. Twenty years ago")
                            .AddText("~he went to the Desert for a duel. But he didn`t find the Warrior as there was a horrific sand storm. Pity.");
                        data.AddOption("Sorry~to~hear~that.", 7)
                           .AddAvatar(64).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Have you ever seen an isis oasis? That is a sea of green, interspersed with towering, rock mountains and surrounded")
                     .AddText("~by the formidable Desert. Maybe only the legendary Obscure Warrior can survive the daunting Desert.");
                            data.AddOption("You~bet!", 255)
                               .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        data.AddText("Because of this unfinished dream, my father shelved his blade and took to drinking... So I began to search for the Divine Sword")
                          .AddText("~to realize his dream. Please take this letter to the Ghost of Obscure Warrior and let him rest in peace.");
                        data.AddOption("Okay.~Gimme.", 8)
                            .AddOption("Later.", 255)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721841, 1) == false)
                            client.Inventory.Add(stream, 721841);
                        client.CreateBoxDialog("Harvey~gave~the~letter~to~you.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I have kept this secret for years...");
                        data.AddOption("Can~you~tell~me?", 2)
                           .AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            data.AddText("Twenty years ago, a brave warrior appeared, helping the poor while punishing the bad. No one knows where he was from. So they")
                      .AddText("~called him Obscure Warrior. Go find Blade Ghost L83 (165,276) to find out more about him, and get his Stone Spell from them.");
                            data.AddOption("I~see.", 255)
                               .AddAvatar(64).FinalizeDialog();

                            break;
                        }
                        data.AddText("People won`t forget those people who have helped them. A pure heart shouldn`t be forgotten. You heard of the Obscure Warrior before,")
                            .AddText("~right? Do you want to know more?");
                        data.AddOption("All~ears.", 3)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Rumor is spread that Obscure Warrior went for a duel in the Desert but never came back. He had an enormously powerful weapon, the")
                          .AddText("~Divine Sword. I learnt that anyone who could find him would get the Sword. But the Desert... Hey, are you interested?");
                        data.AddOption("Risk~nothing,~gain~nothing.", 4)
                            .AddOption("Not~really.", 255)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Indeed! I heard the Level 83 Blade Ghosts have a Stone Spell that was once possessed by Obscure")
                         .AddText("~Warrior. Find them at (165,276) and get the spell first. Goodbye for now.");
                        data.AddOption("Okay.", 5)
                           .AddAvatar(64).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.CreateBoxDialog("Hunt~Level~83~Blade~Ghosts~(165,276)~to~get~the~Stone~Spell!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Carolyn)]
        public static void Carolyn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1798);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Carolyn, client.Player.Class, 1799);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, there! My father asked you to persuade me to marry, right?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Yes.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Yup!", 4);

                        data.AddOption("I`ll~leave~you~alone.", 255)
                        .AddAvatar(109).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("Well, answer my question, first!");
                        data.AddOption("Okay!", 2)
                            .AddOption("Not~interested.", 255)
                           .AddAvatar(109).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Listen carefully. Which kind of Jade can a Leaf Stone and a Maple Stone be fused into?");
                        data.AddOption("Rune~Jade.", 3)
                           .AddOption("Moon~Jade.", 10)
                           .AddOption("Sun~Jade.", 10)
                           .AddOption("Ocean~Jade.", 10)
                           .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Sorry, but you`ll need to go study up a little more.")
                        .AddOption("Ok.", 255)
                        .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Bingo! Wow, you are so brilliant! Um, I`m starting to like you... Could you tell me...would you...marry me?")
                            .AddOption("I~like~you,~too.~But~sorry...", 4)
                            .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~managed~to~persuade~Carolyn!~You`ve~received~3,000~Silver~and~EXP~worth~1/2~EXP~Ball!");
                        }



                        data.AddText("I`m so happy to become your friend. Now go claim your reward from Taskmaster Chang!")
                          .AddOption("See~ya.", 5)
                          .AddAvatar(109).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.CreateBoxDialog("You~managed~to~persuade~Carolyn!~Go~claim~your~reward~from~Taskmaster~Chang.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.FarmerLynn)]
        public static void FarmerLynn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1795);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1796);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1797);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FarmerLynn, client.Player.Class, 1798);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Ah, a hero! Please stop and have a talk with me! You see, my days are slowly ending... I`ve been working on my")
                            .AddText("~farm for my whole life, and to be frank, I`m still quite poor. Wait, let me ask you something first. Have you married, yet?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~about~the~house?", 2);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~about~the~house?", 5);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~about~the~house?", 7);

                        data.AddOption("Um.~What`s~up?", 255)
                        .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("This problem is killing me. I have to look for someone to help me make sure she is taken care of.");
                        data.AddOption("What`s~the~problem?", 2)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(710581, 10))
                            {
                                data.AddText("Wow, you made it! This house will be done in no time! Thank you!");
                                data.AddOption("You`re~welcome.", 5)
                                   .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need 10 hunks of Marble to build a house for my daughter. You can get them from the Rock Monsters. It won`t be hard for you, right?");
                                data.AddOption("Yup.", 255)
                                   .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("You`ll help me?! That`s the best news I`ve heard today! First, she needs a house. It won`t be difficult for you, right?");
                        data.AddOption("Er?~Buy~one~for~you?", 3)
                            .AddOption("No~way.", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("No, no. I`m not a greedy man. You just help me get some marble. Let me see...1, 2, 3...um, I need 10 slabs of marble. You can get them")
                         .AddText("~from the Rock Monsters. Just take your time and make sure they`re good quality!");
                        data.AddOption("Wait~a~second.", 4)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.CreateBoxDialog("Hurry~to~hunt~the~Rock~Monsters~to~get~10~hunks~of~Marble.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710581, 10))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(710581, 10, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You`ve~received~100~Silver!", MsgMessage.ChatMode.System);
                            }

                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(710582, 1) && client.Inventory.Contain(710583, 1)
                                && client.Inventory.Contain(710584, 1))
                            {
                                data.AddText("Wow, unbelievable! You got them? Thank you so much, hero!");
                                data.AddOption("You`re~welcome.", 7)
                                   .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Yeah, jade! My daughter likes the Rune Jade, Moon Jade and Sun Jade very much. Each of the jades needs to be fused by 2 special stones.")
                                    .AddText("~A Leaf Stone and a Maple Stone can be fused into a Rune Jade. An Ocean Stone and a Leaf Stone can be fused into a Sun Jade.")
                                    .AddText("~The Moon Jade needs to be fused by a Maple Stone and an Ocean Stone. You can get these special stones from Level 78 Rock Monsters. Understand?");
                                data.AddOption("Yes.", 255)
                                   .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I still need some decorations for the house... Just imagine, if you lived in a big, empty")
                            .AddText("~stone house... Oh, it`s far too dull for a girl of her tastes.");
                        data.AddOption("I`d~like~to~help.", 6)
                            .AddOption("Decorations?", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.CreateBoxDialog("Hurry~and~hunt~the~Level~78~Rock~Monsters~for~the~stones~to~make~the~3~jades!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(710582, 1) && client.Inventory.Contain(710583, 1)
                                  && client.Inventory.Contain(710584, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Inventory.Remove(710582, 1, stream);
                                client.Inventory.Remove(710583, 1, stream);
                                client.Inventory.Remove(710584, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You`ve~received~100~Silver!", MsgMessage.ChatMode.System);
                            }
                        }
                        data.AddText("My daughter is 30 years old and wants to be single forever! Please try to persuade her to change her mind!")
                            .AddText("~But since she`s really childish, unless you can correctly answer her questions she won`t listen to you.");
                        data.AddOption("I`ll~try~my~best.", 9)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        client.CreateBoxDialog("You~promised~to~try~and~persuade~Farmer~Lynn`s~daughter~to~marry.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.TaoistShine)]
        public static void TaoistShine(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.JerkWang, client.Player.Class, 1790);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistShine, client.Player.Class, 1791);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistShine, client.Player.Class, 1792);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Reverence for nature and ancestor spirits is important. Never fool around with celestials.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("You~knew~Mo~Mo?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Trust~me.~I`ll~find~Mo~Mo.", 2);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("How~to~get~there?", 10);

                        data.AddOption("Any~ideas?", 255)
                        .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("You came for Mo Mo? I`ve heard the story. It`s weird...");
                        data.AddOption("Any~ideas?", 2)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721712, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {

                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                }
                                data.AddText("This Powder is used to bewilder little kids and kidnap them to Ghost City.");
                                data.AddOption("Okay.", 10)
                                   .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hey you! You already have one!");
                                data.AddOption("Oh,~yeah.", 255)
                                   .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I`m afraid he has entered Ghost City, by mistake. Here I have a Charm, with it, you can enter there, too.");
                        data.AddOption("Gimme.", 3)
                            .AddOption("I`m~not~going~there.", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Take this Powder and use it where Mo Mo was seen missing, and you will be able to enter the city.");
                        data.AddOption("Okay.", 11)
                            .AddOption("I`ll~go~there~later.", 255)
                           .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721713, 1))
                            client.Inventory.Add(stream, 721713);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Try~your~best~to~rescue~Mo~Mo.");
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721713, 1))
                            client.Inventory.Add(stream, 721713);

                        client.CreateBoxDialog("You~obtained~a~Charm~from~Taoist~Shine.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyPuddy)]
        public static void Ghost_SweetyPuddy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This place is great!");
                        data.AddOption("You~bet.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyPuddy)]
        public static void Dark_SweetyPuddy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This place is great!");
                        data.AddOption("You~bet.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyLily)]
        public static void Dark_SweetyLily(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Come and play with me!");
                        data.AddOption("Nope.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Ghost_SweetyLily)]
        public static void Ghost_SweetyLily(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Come and play with me!");
                        data.AddOption("Nope.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyMindy)]
        public static void Dark_SweetyMindy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I like here.");
                        data.AddOption("Whatever.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyMindy)]
        public static void Ghost_SweetyMindy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I like here.");
                        data.AddOption("Whatever.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyDay)]
        public static void Dark_SweetyDay(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("It rocks here!");
                        data.AddOption("It~sucks.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyDay)]
        public static void Ghost_SweetyDay(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("It rocks here!");
                        data.AddOption("It~sucks.", 255)
                           .AddAvatar(143).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_SweetyCuty)]
        public static void Ghost_SweetyCuty(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Best playground ever!");
                        data.AddOption("It`s~dangerous.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Dark_SweetyCuty)]
        public static void Dark_SweetyCuty(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Best playground ever!");
                        data.AddOption("It`s~dangerous.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Ghost_FiendAltar1)]
        public static void Ghost_FiendAltar1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(410, 354, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_FiendAltar2)]
        public static void Ghost_FiendAltar2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(410, 354, 1002);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_FiendAltar3)]
        public static void Ghost_FiendAltar3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(410, 354, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_FiendAltar4)]
        public static void Ghost_FiendAltar4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(721712, 1))
                        {
                            client.CreateBoxDialog("You`ve~already~got~the~Powder~in~inventory.");
                        }
                        else
                        {
                            client.CreateBoxDialog("You`ve~found~some~powder.~Hurry~to~deliver~it~to~Taoist~Shine.");
                            client.Inventory.Add(stream, 721712);
                        }
                        client.Teleport(410, 354, 1002);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ghost_MoMo)]
        public static void Ghost_MoMo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Here is way better than home. I could play all day here. Mom and Dad would only scold me for anything I do.");
                        data.AddOption("You~should~go~home.", 1)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("You will find some clues near Fete Pillar (162,217).");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Dark_MoMo)]
        public static void Dark_MoMo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistShine, client.Player.Class, 1792);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Dark_MoMo, client.Player.Class, 1793);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I love here! It`s fun! I`m not going home!");
                        data.AddOption("Your~mom~is~worried~about~you!", 1)
                            .AddOption("If~you~like.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721714, 1))
                        {
                            data.AddText("It`s dark. I`m scared.");
                            data.AddOption("Take~the~Bezoar.", 2)
                               .AddAvatar(36).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You still don`t get the Bezoar. Go get it.");
                            data.AddOption("I`m~on~my~way.", 255)
                               .AddAvatar(36).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("I love here! It`s fun! I`m not going home!");
                        data.AddOption("Go~with~me.", 3)
                            .AddOption("If~you~like.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("Thanks for bringing me home! My mother must prepare a reward for you! Go talk to her!");
                        data.AddOption("I~will.", 4)
                            .AddOption("If~you~like.", 255)
                           .AddAvatar(36).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Teleport(410, 354, 1002);
                        client.CreateBoxDialog("Mo~Mo~took~the~Bezoar~and~returned~home.~Hurry~to~find~Aunt~Zhang~to~claim~your~reward!");
                        client.Inventory.Remove(721714, 1, stream);
                        client.Inventory.Remove(721712, 1, stream);
                        client.Inventory.Remove(721713, 1, stream);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JerkWang)]
        public static void JerkWang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1788);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JerkWang, client.Player.Class, 1789);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JerkWang, client.Player.Class, 1790);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Impossible is nothing. Nothing is impossible.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Where`s~Mo~Mo?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                             && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`m~trying~find~Mo~Mo.", 3);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Do~you~have~any~solution?", 10);
                        data.AddOption("Just~do~it.", 255)
                        .AddAvatar(74).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        data.AddText("It was all my fault. I should have kept an eye on him. I was reading. Mo Mo was out playing alone.");
                        data.AddOption("Then~what?", 2)
                           .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Some one saw him disappear in the east of the city all of a sudden. Will you find him for me?");
                        data.AddOption("Sure.", 3)
                            .AddOption("I`m~afraid.", 255)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(721717, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                                data.AddText("It`s all my fault... I should have been more careful...");
                                data.AddOption("What~shall~we~do?", 10)
                                   .AddAvatar(74).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Mo Mo was seen in these three spots: (269,215), (298,205) and (357,304). Please search there.");
                        data.AddOption("I`m~on~my~way.", 4)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721717, 1))
                            client.Inventory.Add(stream, 721717);
                        client.CreateBoxDialog("You`ve~obtained~Magic~Fireworks~from~the~Jerk~Wang.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 10:
                    {
                        data.AddText("Ah... now I remember, maybe you should visit Taoist Shine (245,258). He lives in Twin City")
                            .AddText("~and knows all the secrets behind peculiarity.");
                        data.AddOption("I`ll~visit~him.", 11).AddOption("I`m~not~going~anywhere.", 255)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        data.AddText("Taoist Shine (245,258) must know why. Please pay him a visit.");
                        data.AddOption("Good~idea!", 255)
                           .AddAvatar(74).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AuntZhang)]
        public static void AuntZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1787);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1788);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Dark_MoMo, client.Player.Class, 1793);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1794);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Childern are happiest when celebrating festivals...They are running around everywhere. But sometimes they might get lost...");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("You~should~be~careful.", 2);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                         && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Your~son~is~back!", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
               && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I`ve~found~your~son!", 11);
                        data.AddOption("I~agree.", 255)
                        .AddAvatar(155).FinalizeDialog();


                        break;
                    }
                case 10:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("If you forgot some skills or you qualifed for rebirth, you can learn skills in Job Center.", MsgMessage.ChatMode.System);
                            client.SendSysMesage("You've received EXP worth 1 EXP Ball and 6,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for saving my son. Please take this as my token of my gratitude.");
                        data.AddOption("My~pleasure.", 11)
                           .AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I really appreciate your help! Please go claim your reward from Taskmaster Chang!");
                        data.AddOption("I~will.", 12)
                           .AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Go~claim~your~reward~from~Taskmaster~Chang!");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("Mo Mo is lost! Please, you have to help me!");
                        data.AddOption("How~did~it~happen?", 2)
                           .AddAvatar(155).FinalizeDialog();


                        break;
                    }
                case 2:
                    {

                        data.AddText("Have you seen my son on your way here? His name is Mo Mo. He is a cute little boy, about this height... I couldn`t find him.")
                            .AddText("~He was with Jerk Wang (347,232), setting off Magic Fireworks, but he didn`t come back last night.");
                        data.AddOption("I`ll~give~him~a~visit.", 3)
                            .AddOption("Sorry~to~hear~that.", 255)
                           .AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Hurry~and~talk~to~Jerk~Wang~(TwinCity~347,232)!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }
        //SuperHeadgearPack
        [NpcAttribute(NpcID.SuperHeadgearPack)]
        public static void SuperHeadgearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 1049)
                        {
                            data.AddOption("Super~Monk~Coronet.", 1);
                            data.AddOption("Super~Jade~Earring.", 9);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 2049)
                            {
                                data.AddOption("Super~Violent~Helmet.", 2);
                                data.AddOption("Super~Jade~Earring.", 9);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 4049)
                                {
                                    data.AddOption("Super~Antler~Hat.", 3);
                                    data.AddOption("Super~Jade~Earring.", 9);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 5049)
                                    {
                                        data.AddOption("Super~Evil~Hood.", 4);
                                        data.AddOption("Super~Jade~Earring.", 9);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 6049)
                                        {
                                            data.AddOption("Super~Headband~Of~Holy~Light.", 5);
                                            data.AddOption("Super~Jade~Earring.", 9);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 7049)
                                            {
                                                data.AddOption("Super~Cruelty~Hat.", 6);
                                                data.AddOption("Super~Jade~Earring.", 9);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 8049)
                                                {
                                                    data.AddOption("Super~Shadow~Hood.", 7);
                                                    data.AddOption("Super~Jade~Earring.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 14049)
                                                {
                                                    data.AddOption("Super~Crane~Cap.", 8);
                                                    data.AddOption("Super~Jade~Earring.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Super~WindWalker~Cap.", 10);
                                                    data.AddOption("Super~Jade~Earring.", 9);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 101089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Crane~Cap!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 114089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Crane~Cap!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 148089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~received~an~Super~Shadow~Hood~and~an~EXP~Ball!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 144089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Cruelty~Hat.");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 143089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Headband~Of~Holy~Light!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 123089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Evil~Hood!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 113069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Antler~Hat!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 111089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Violent~Helmet!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 117089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Jade~Earring!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728247, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 118089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Monk~Coronet!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DoctorKnowitAll)]
        public static void DoctorKnowitAll(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1809);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1810);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1811);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1812);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1813);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1814);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1815);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1816);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1817);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("The~pill~isn`t~working!", 40);
                            data.AddOption("Are~you~nuts?");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("Help~Poor~Xiao~then!", 1);
                            data.AddOption("Whatever.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("What~do~you~need?", 10);
                            data.AddOption("Leave.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("How~will~you~help~Xiao?", 5);
                                data.AddOption("See~ya.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must pay me a meteor and then I will make the Peel Pill for Xiao. The pill is made by Bird Blood, Ginseng Grease and Hawk Claw. But they are toxic and there are");
                                data.AddText("~side-effects when people take this...I can`t guarantee its effects either...you know...Anything is possible...");
                                data.AddOption("Let~me~ask~Xiao,~first.", 3);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~Bird~Blood,~right?", 14);
                                data.AddOption("See~ya~later.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("Tell~me~what~shall~I~do.", 12);
                                data.AddOption("No...");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
               && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~Bitter~Ginseng?", 21);
                                data.AddOption("Yes...");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("What~else~do~you~need?", 20);
                                data.AddOption("I`ll~catch~ya~later.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
         && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~Hawk~Claws?", 27);
                                data.AddOption("Yup.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("You~need~anything~else?", 26);
                                data.AddOption("Nope.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
&& client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("May~I~have~a~Peel~Pill?", 33);
                                data.AddOption("I~don`t~need~to~understand.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                                data.AddOption("How`s~going~with~the~pill?", 30);
                                data.AddOption("...");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
&& client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("What`s~the~problem~with~Xiao?", 44);
                            data.AddOption("Crazy.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                            data.AddOption("Whatever.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 44:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("According to my records, only Golden Rocs may suffer from this kind of syndrome...");
                            data.AddText("~General Yue is said to be the incarnation of a Golden Roc. So I have the theory, Xiao is his descendent.");
                            data.AddOption("I`ll~tell~him.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Calm down, let me check first ... According to my records, only Golden Rocs may suffer from this kind of syndrome...");
                            data.AddText("~General Yue is said to be the incarnation of a Golden Roc. So I have the theory, Xiao is his descendent.");
                            data.AddOption("I`ll~tell~him.", 43);
                            data.AddOption("You~can`t~be~serious.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 40:
                    {
                        data.AddText("I heard that General Yue, the great hero, was the incarnation of a Golden Roc. Is it possible that Poor Xiao is his descendent?");
                        data.AddOption("What?!", 41);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!");
                            data.AddText("Calm down, let me check first ... According to my records, only Golden Rocs may suffer from this kind of syndrome...");
                            data.AddText("~General Yue is said to be the incarnation of a Golden Roc. So I have the theory, Xiao is his descendent.");
                            data.AddOption("I`ll~tell~him.", 43);
                            data.AddOption("You~can`t~be~serious.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 43:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        break;
                    }
                case 33:
                    {
                        if (client.Inventory.Contain(721911, 1))
                        {
                            data.AddText("You`ve already got a Peel Pill!");
                            data.AddOption("Oh,~yes.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You lost it? You should have been more careful! Well, I will give you a new one.");
                            data.AddOption("Thank~you.", 34);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 34:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721911);
                            client.CreateBoxDialog("You`ve~received~the~Peel~Pill.~Hurry~to~take~it~to~Poor~Xiao!");
                        }
                        else
                        {
                            data.AddText("Hey, hey! Your inventory is full! Prepare some room, please.");
                            data.AddOption("Okay.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("Come come! I`ve made it, the Peel Pill! Now hurry up and take it to Poor Xiao.");
                        data.AddOption("Yes,~sir!", 31);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721911);
                            client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                            client.CreateBoxDialog("You`ve~received~the~Peel~Pill.~Hurry~to~take~it~to~Poor~Xiao!");
                            data.AddText("Come come! I`ve made it, the Peel Pill! Now hurry up and take it to Poor Xiao.");
                            data.AddOption("Yes,~sir!", 255);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey, hey! Your inventory is full! Prepare some room, please.");
                            data.AddOption("Okay.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        data.AddText("Only when you kill 2 of them within 1 minute, will you get a Hawk Claw. So have you got 5 Hawk Claws?");
                        data.AddOption("Yes.~Here~you~are.", 28);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();

                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Contain(721910, 5))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId);
                            client.Inventory.Remove(721910, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!");
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721911);
                                client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                                client.CreateBoxDialog("You`ve~received~the~Peel~Pill.~Hurry~to~take~it~to~Poor~Xiao!");
                                data.AddText("Come come! I`ve made it, the Peel Pill! Now hurry up and take it to Poor Xiao.");
                                data.AddOption("Yes,~sir!", 255);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Hey, hey! Your inventory is full! Prepare some room, please.");
                                data.AddOption("Okay.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("I need 5 Hawk Claws. Otherwise I`m not able to make the Peel Pill.");
                            data.AddOption("No~problem.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 26:
                    {
                        data.AddText("Good, now I need 5 Hawk Claws. You`re going to hunt the Level 93 Hawks. Only when you kill 2 of them within 1 minute, will you get a Hawk Claw.");
                        data.AddOption("I~see.", 25);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.CreateBoxDialog("Hurry~to~collect~5~Hawk~Claws!");
                        break;
                    }
                case 21:
                    {
                        data.AddText("Bitter Ginseng is made by Haw King`s Gallbladderr. You`ve got it?");
                        data.AddOption("Yes,~here~it~is.", 22);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(721908, 1))
                        {
                            client.Inventory.Remove(721912, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!");
                            client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId);
                            client.Inventory.Remove(721908, 1, stream);
                            data.AddText("Good, now I need 5 Hawk Claws. You`re going to hunt the Level 93 Hawks. Only when you kill 2 of them within 1 minute, will you get a Hawk Claw.");
                            data.AddOption("I~see.", 25);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721912, 1))
                            {
                                data.AddText("I need 1 Bitter Ginseng to make Ginseng Grease. Please take the Gallbladder drops from Haw King on Bird Island (451,676)");
                                data.AddOption("I~see.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("You lost the map? It`s okay. I`ll give you another.");
                                data.AddOption("Thank~you.", 23);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721912);
                            client.CreateBoxDialog("You`ve~got~a~Ginseng~Map.~Right~click~to~read~it.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Hurry to make some room!");
                            data.AddOption("I`ll~do~right~away.");
                            data.AddAvatar(67);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("It`s made by Haw King`s Gallbladderr. Here`s a map, telling you where the Bitter Ginseng could be found. Use the Gallbladderr at the spots");
                        data.AddText("~and you may find a Bitter Ginseng.");
                        data.AddOption("Gimme~the~map.", 17);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        data.AddText("I need 10 samples of Bird Blood. Have you got them?");
                        data.AddOption("Yes.~Here~you~are.", 15);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(721906, 10))
                        {
                            data.AddText("Good job. Now, you are going to find a Bitter Ginseng.");
                            data.AddOption("How~to~get~one?", 16);//1272310
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey, there! You don`t have enough Bird Blood. I need 10. You got it?");
                            data.AddOption("Yes.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 16:
                    {

                        if (client.Inventory.Contain(721906, 10))
                        {
                            client.Inventory.Remove(721906, 10, stream);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You`ve received 100 Silver!");
                                data.AddText("It`s made by Haw King`s Gallbladderr. Here`s a map, telling you where the Bitter Ginseng could be found. Use the Gallbladderr at the spots");
                                data.AddText("~and you may find a Bitter Ginseng.");
                                data.AddOption("Gimme~the~map.", 17);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 17:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        if (client.Inventory.Contain(721912, 1))
                        {
                            data.AddText("You`ve got a map, already!");
                            data.AddOption("Alright.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721912);
                                client.CreateBoxDialog("You`ve~got~a~Ginseng~Map.~Right~click~to~read~it.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full! Hurry to make some room!");
                                data.AddOption("I`ll~do~right~away.");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("I need a Meteor. Do you have?");
                        data.AddOption("Yes.~Here~it~is.", 11);
                        data.AddOption("Not~yet.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(1088001, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Inventory.Remove(1088001, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                client.SendSysMesage("You`ve received 100 Silver!");
                                client.CreateBoxDialog("You~gave~the~doctor~a~Meteor.");
                                data.AddText("You`re kindhearted, huh?");
                                data.AddOption("Tell~me~the~recipe!", 12);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I need a Meteor. Do not bother me again unless you have got one.");
                            data.AddOption("Sorry.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        data.AddText("Well, first of all, you`re going to gather 10 samples of Bird Blood. You can get them from the Birdmen and Level 88 Birdmen.");
                        data.AddOption("I`ll~be~back~soon.", 13);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I know the antidote for Poor Xiao`s weird disease!");
                        data.AddOption("What~is~it?", 2);
                        data.AddOption("Impress~me.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You`ve received 100 Silver!");
                            data.AddText("You must pay me a meteor and then I will make the Peel Pill for Xiao. The pill is made by Bird Blood, Ginseng Grease and Hawk Claw. But they are toxic and there are");
                            data.AddText("~side-effects when people take this...I can`t guarantee its effects either...you know...Anything is possible...");
                            data.AddOption("Let~me~ask~Xiao,~first.", 3);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("You must pay me a meteor and then I will make the Peel Pill for Xiao. The pill is made by Bird Blood, Ginseng Grease and Hawk Claws. But they are toxic and there are");
                        data.AddText("~side-effects when people take this...I can`t guarantee its effects either...");
                        data.AddOption("Let~me~ask~Xiao,~first.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You know what? They all call me Doctor Know-it-All because I know the secrets beneath all disease...all of them, understand?");
                        data.AddOption("How~will~you~help~Xiao?", 4);
                        data.AddOption("See~ya.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PoorXiao)]
        public static void PoorXiao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1808);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1809);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1810);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1811);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1815);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1816);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorKnowitAll, client.Player.Class, 1817);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1818);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("You~knew~General~Yue?", 30);
                            data.AddOption("Bye.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`ve~got~the~pill.", 20);
                            data.AddOption("Just~passing~by.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1);
                            data.AddOption("Enjoy~yourself.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("Let~me~tell~you~something.", 10);
                            data.AddOption("Okay.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`ll~help~you.", 4);
                            data.AddOption("I`ll~leave~you~alone.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("The pill is toxic? And the Meteor, I don`t have any...");
                                data.AddOption("What`s~your~opinion?", 11);
                                data.AddAvatar(56);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                                data.AddOption("Have~you~made~your~choice?", 13);
                                data.AddOption("I`ll~be~back~soon.");
                                data.AddAvatar(56);
                                data.FinalizeDialog();
                            }
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                   && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("Here`s~the~antidote.", 22);
                            data.AddOption("I`m~busy.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`m~leaving.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
               && client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Thank you, for bringing me the pill and helping me know my identity! Please go claim your reward from Taskmaster Chang.");
                            data.AddOption("I~will.", 33);
                            data.AddOption("Later.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come and enjoy the beautiful scenery of Bird Island with me...");
                            data.AddOption("I`m~leaving.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        client.CreateBoxDialog("Go~claim~your~reward~from~Taskmaster~Chang!");
                        break;
                    }
                case 30:
                    {
                        data.AddText("My parents passed away when I was young, leaving only a Jade Pendant. The words inscribed on it say, `Serve thy country`.");
                        data.AddText("~Now I know I am one of the descendents of General Yue, the great hero.");
                        data.AddOption("That`s~true.", 31);
                        data.AddAvatar(7);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Since I`ve known my identity, I`ll work hard and never disappoint my ancestors. Thank you! Please go claim your reward from Taskmaster Chang.");
                        data.AddOption("My~pleasure.", 32);
                        data.AddAvatar(7);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Player.Money += 6000;
                            client.GainExpBall(600);
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.CreateBoxDialog("You`ve~gained~EXP~worth~1~EXP~Ball~and~6,000~Silver.~Hurry~to~claim~your~reward~from~Taskmaster~Chang!");
                            client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("You`ve got the pill? Great!");
                        data.AddOption("Hurry~to~take~it!", 21);
                        data.AddAvatar(56);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {

                        if (client.Inventory.Contain(721911, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                            {
                                client.Inventory.Remove(721911, 1, stream);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You`ve received 100 Silver!", MsgMessage.ChatMode.System);
                                data.AddText("Oh my god! Are you sure it is the antidote? The wings do not disappear! No, look at my face! It turns golden!");
                                data.AddOption("I`ll~ask~the~doctor!", 23);
                                data.AddAvatar(56);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding me? Please, go find Doctor Know-it-All right away.");
                            data.AddOption("Okay.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh my god! Are you sure it is the antidote? The wings do not disappear! No, look at my face! It turns golden!");
                            data.AddOption("I`ll~ask~the~doctor!");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Oh my god! Are you sure it is the antidote? The wings do not disappear! No, look at my face! It turns golden!");
                            data.AddOption("I`ll~ask~the~doctor!", 23);
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 23:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 13:
                    {
                        data.AddText("I will try it, no matter what might happen. But I still need a Meteor to complete the prescription of the Peel Pill.");
                        data.AddOption("Don`t~worry.");
                        data.AddAvatar(56);

                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("The pill is toxic? And the Meteor, I don`t have any...");
                        data.AddOption("What`s~your~opinion?", 11);
                        data.AddAvatar(56);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve received 100 Silver!", MsgMessage.ChatMode.System);
                            data.AddText("Alright, I`m willing to have a try. But you see, I`m poor and it`s impossible for me to afford a Meteor...");
                            data.AddOption("You~can~count~on~me.", 12);
                            data.AddOption("Sorry~about~it.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1809, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Thank you. Please find Doctor Know-it-All on Bird Island (701,595) as soon as possible.");
                            data.AddOption("I`m~on~my~way.");
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Thank you. Please find Doctor Know-it-All on Bird Island (701,595) as soon as possible.");
                            data.AddOption("I`m~on~my~way.", 3);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Hello! Please don`t get scared by me...These ugly feathers grew all over my body out of a blue. I learnt that");
                        data.AddText("~Doctor Know-it-All on Bird Island (701,595) has a special prescription for this. Will you give me a hand?");
                        data.AddOption("Of~course~I~will.", 2);
                        data.AddOption("Sorry,~not~now.");
                        data.AddAvatar(56);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.SendSysMesage("You`ve received 100 Silver!", MsgMessage.ChatMode.System);
                            data.AddText("Thank you. Please find Doctor Know-it-All on Bird Island (701,595) as soon as possible.");
                            data.AddOption("I`m~on~my~way.", 3);
                            data.AddAvatar(56);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }


        //ClothingPack
        [NpcAttribute(NpcID.ClothingPack)]
        public static void ClothingPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 1049)
                        {
                            data.AddOption("Super~Conquest~Armor.", 1);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 2049)
                            {
                                data.AddOption("Super~Phoenix~Armor.", 2);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 4049)
                                {
                                    data.AddOption("Super~Phoenix~Jerkin.", 3);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 5049)
                                    {
                                        data.AddOption("Super~Owl~Vest.", 4);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 6049)
                                        {
                                            data.AddOption("Super~Robe~Of~The~Buddha.", 5);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 7049)
                                            {
                                                data.AddOption("Super~Whale~Coat.", 6);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 8049)
                                                {
                                                    data.AddOption("L120~Super~Yellow~Combat~Suit.", 7);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 14049)
                                                {
                                                    data.AddOption("Super~Pine~Robe.", 8);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Super~WindWalker~Armor.", 9);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 170109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Conquest~Armor(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 130109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Conquest~Armor(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 131109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Phoenix~Armor(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 133109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Phoenix~Jerkin(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 135109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Owl~Vest(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 136109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Robe~Of~The~Buddha(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 139109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~WhaleCoat.");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 138109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~received~an~Level~120~Super~Yellow~Combat~Suit~and~an~EXP~Ball!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728251, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 134109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Pine~Robe(B)!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TaskmasterChang)]
        public static void TaskmasterChang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1718);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1722);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1723);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1726);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1727);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1732);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1733);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1737);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1738);
            var ActiveQuest10 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1744);
            var ActiveQuest11 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1745);
            var ActiveQuest12 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1750);
            var ActiveQuest13 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1751);
            var ActiveQuest14 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KungfuMaster, client.Player.Class, 1757);
            var ActiveQuest15 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1781);
            var ActiveQuest16 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1786);

            var ActiveQuest17 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1787);
            var ActiveQuest18 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntZhang, client.Player.Class, 1794);

            var ActiveQuest19 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1795);
            var ActiveQuest20 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Carolyn, client.Player.Class, 1799);



            var ActiveQuest21 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1800);
            var ActiveQuest22 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AuntPeach, client.Player.Class, 1807);

            var ActiveQuest23 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1808);
            var ActiveQuest24 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoorXiao, client.Player.Class, 1818);

            var ActiveQuest25 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1819);
            var ActiveQuest26 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Felix, client.Player.Class, 1827);


            var ActiveQuest27 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1828);
            var ActiveQuest28 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1834);

            var ActiveQuest29 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1835);
            var ActiveQuest30 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1839);

            var ActiveQuest31 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1840);
            var ActiveQuest32 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1852);

            var ActiveQuest33 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 200);
            switch (Option)
            {
                case 33:
                case 0:
                    {

                        data.AddText("Everyone wants to be a hero, and so do I, but Rome wasn`t built in a day.")
                          .AddText("~If you are dreaming of becoming a celebrity around here, I can help you get prepared. Please be noted that Reincarnated players can`t accept quests from me.");
                        data.AddOption("The~Connector~(" + GetStrTask(client, ActiveQuest2) + ").", 1);
                        data.AddOption("Spun~Gold~Armor~(" + GetStrTask(client, ActiveQuest4) + ").", 4)
                            .AddOption("Azure~Pill~(" + GetStrTask(client, ActiveQuest6) + ").", 7)
                            .AddOption("Artisan`s~Feather~(" + GetStrTask(client, ActiveQuest8) + ").", 10)
                            .AddOption("Mystic~Ring~(" + GetStrTask(client, ActiveQuest10) + ").", 13)
                            .AddOption("Next~page.", 16)
                            .AddOption("Wait,~I`m~not~ready~yet!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }

                case 16:
                    {

                        data.AddText("Everyone wants to be a hero, and so do I, but Rome wasn`t built in a day.")
                         .AddText("~If you are dreaming of becoming a celebrity around here, I can help you get prepared. Please be noted that Reincarnated players can`t accept quests from me.");
                        data.AddOption("Frantic~Monkeys~(" + GetStrTask(client, ActiveQuest12) + ").", 17)
                             .AddOption("Soul~of~General~(" + GetStrTask(client, ActiveQuest14) + ").", 20)
                                .AddOption("Mad~Man~(" + GetStrTask(client, ActiveQuest16) + ").", 24)
                                .AddOption("Ghost~City~(" + GetStrTask(client, ActiveQuest18) + ").", 30)
                                .AddOption("Tomboy`s~Marriage~(" + GetStrTask(client, ActiveQuest20) + ").", 40)
                                .AddOption("Desert~Blood~(" + GetStrTask(client, ActiveQuest22) + ").", 50)
                                .AddOption("Previous.", 33)
                            .AddOption("Next~page.", 55)
                           .AddAvatar(72).FinalizeDialog();
                        break;
                    }
                case 55:
                    {
                        data.AddText("Everyone wants to be a hero, and so do I, but Rome wasn`t built in a day.")
                    .AddText("~If you are dreaming of becoming a celebrity around here, I can help you get prepared. Please be noted that Reincarnated players can`t accept quests from me.");
                        data.AddOption("Hero`s~Wings~(" + GetStrTask(client, ActiveQuest24) + ").", 60)
                            .AddOption("Family~Feud~(" + GetStrTask(client, ActiveQuest26) + ").", 70)
                            .AddOption("Desert~Treasure~(" + GetStrTask(client, ActiveQuest28) + ").", 80)
                             .AddOption("Desert~Expedition~(" + GetStrTask(client, ActiveQuest30) + ").", 90)
                             .AddOption("Power~Book~(" + GetStrTask(client, ActiveQuest32) + ").", 100)
                                .AddOption("TempestWings.~(" + GetStrTask(client, ActiveQuest33) + ").", 110)
                              .AddOption("Previous.", 16)
                            //Desert~Expedition~
                           .AddAvatar(72).FinalizeDialog();
                        break;
                    }
                case 110:
                    {
                        if (client.Player.Level < 120)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 120 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest33.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The~Tempest~Wings~have~appeared!~It`s~an~intelligent~talisman~to~boost~your~strength,~as~well~as~a~dazzling~symbol~of~nobility,");
                            data.AddText("~making~you~stand~out~in~a~second.~Wanna~be~the~Champion~of~champions?~Go~and~find~Mr.~Wonder~(212,196)~in~Phoenix~Castle");
                            data.AddText("~to~forge~a~pair~of~Tempest~Wings.");
                            data.AddOption("Take~me~to~see~Mr.~Wonder.", 111);
                            data.AddOption("Close.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest33.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("You`ve~already~completed~the~quest,~haven`t~you?");
                                data.AddOption("Yeah,~you`re~right.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The~Tempest~Wings~have~appeared!~It`s~an~intelligent~talisman~to~boost~your~strength,~as~well~as~a~dazzling~symbol~of~nobility,");
                                data.AddText("~making~you~stand~out~in~a~second.~Wanna~be~the~Champion~of~champions?~Go~and~find~Mr.~Wonder~(212,196)~in~Phoenix~Castle");
                                data.AddText("~to~forge~a~pair~of~Tempest~Wings.");
                                data.AddOption("Take~me~to~see~Mr.~Wonder.", 111);
                                data.AddOption("Close.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }

                case 111:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest33, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1011, 212, 196, 10850);
                        break;
                    }
                case 100:
                    {
                        if (client.Player.Level < 115)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 115 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest32.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;//1262670
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest32.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("There`s an Old Beggar at the entrance of the Labyrinth (263,135). He has a very special book about the essence of all the secrets behind Chinese Martial Arts.");
                            data.AddText("If you are interested in it, pay a visit to him.");
                            data.AddOption("Claim~rewards.", 102);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest31.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("There`s an Old Beggar at the entrance of the Labyrinth (391,235). He has a very special book about the essence of all the secrets behind Chinese Martial Arts.");
                                data.AddText("If you are interested in it, pay a visit to him.");
                                data.AddOption("Accept~the~quest.", 101);
                                data.AddOption("I~see.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 102:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest32.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728251, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L120~Super~Clothing~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        }
                        break;
                    }
                case 101:
                    {
                        client.CreateBoxDialog("Hurry~to~pay~a~visit~to~the~Old~Beggar.");
                        client.Player.QuestGUI.Accept(ActiveQuest31, 0);
                        break;
                    }
                case 90:
                    {
                        if (client.Player.Level < 105)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 105 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest30.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;//1262670
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest30.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Master Mo Mo in Desert City (520,589) is a nobleman who is fond of making gunpowder and researching new weapons. But");
                            data.AddText("~he seems to be in trouble. Help him. When it is done, you can claim rewards from me.");
                            data.AddOption("Claim~rewards.", 92);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest29.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Master Mo Mo in Desert City (520,589) is a nobleman who is fond of making gunpowder and researching new weapons. But");
                                data.AddText("~he seems to be in trouble. Help him. When it is done, you can claim rewards from me.");
                                data.AddOption("Accept~the~quest.", 91);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 92:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest30.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728250, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L110~Super~Ring~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        }
                        break;
                    }
                case 91:
                    {
                        client.CreateBoxDialog("Hurry~up~to~help~Master~Mo~Mo~in~Desert~City!");
                        client.Player.QuestGUI.Accept(ActiveQuest29, 0);
                        break;
                    }
                case 80://1264450
                    {
                        if (client.Player.Level < 98)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 98 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest28.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest28.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Louis in Desert City (466,649) is looking for the Zen Scripture, the Victory Portrait and the Buddha Relic. He needs your help.");
                            data.AddOption("Claim~rewards.", 82);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();

                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest27.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Louis in Desert City (466,649) is looking for the Zen Scripture, the Victory Portrait and the Buddha Relic. He needs your help.");
                                data.AddOption("Accept~the~quest.", 81);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 82:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest28.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728249, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L100~Super~Boots~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        }

                        break;
                    }

                case 81:
                    {
                        client.CreateBoxDialog("Hurry~up~to~help~Louis~in~Desert~City.");
                        client.Player.QuestGUI.Accept(ActiveQuest27, 0);
                        break;
                    }
                case 70:
                    {
                        if (client.Player.Level < 95)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level 95 to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest26.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest26.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Felix on Bird Island (686,547) is plagued by the fact that his parents were killed by his foster father, who is a Bandit");
                            data.AddText("~Leader. Help him seek revenge in honor of his parents. When it is done, you can claim rewards from me.");
                            data.AddOption("Claim~rewards.", 72);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();

                            //1264340
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest25.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Felix on Bird Island (686,547) is plagued by the fact that his parents were killed by his foster father, who is a Bandit");
                                data.AddText("~Leader. Help him seek revenge in honor of his parents. When it is done, you can claim rewards from me.");
                                data.AddOption("Accept~the~quest.", 71);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("Sorry, but you didn`t complete the quest.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest26.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728248);
                                client.CreateBoxDialog("You~completed~the~quest~and~received~a~L97~Super~Necklace~Pack!");
                            }
                        }
                        else
                            client.CreateBoxDialog("Your~inventory~is~full.~You`ll~need~to~make~some~room,~first.");
                        break;
                    }
                case 71:
                    {
                        client.CreateBoxDialog("Hurry~up~to~help~Felix~on~Bird~Island!");
                        client.Player.QuestGUI.Accept(ActiveQuest25, 0);
                        // 1264320
                        break;
                    }
                case 60:
                    {
                        if (client.Player.Level < ActiveQuest24.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest24.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Hero`s Wings`, haven`t you?");
                            data.AddOption("Oh,~yeah.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest24.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Poor Xiao on Bird Island (725,541) caught a strange disease which made his body covered by plumes. You need to visit");
                            data.AddText("~Doctor Know-it-All for him.");
                            data.AddOption("Done!~Claim~my~reward!", 61);
                            data.AddOption("Leave.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest23.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                data.AddText("Poor Xiao on Bird Island (725,541) caught a strange disease which made his body covered by plumes. You need to visit");
                                data.AddText("~Doctor Know-it-All for him.");
                                data.AddOption("You~can~count~on~me.", 62);
                                data.AddOption("None~of~my~business.");
                                data.AddAvatar(72);
                                data.FinalizeDialog();
                                break;
                            }
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Hero`s Wings`!");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 62:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest23.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest23, 0);
                            client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        }
                        break;
                    }
                case 61:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest24.MissionId))
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.Inventory.Add(stream, 728247);
                                client.CreateBoxDialog("You`ve~completed~the~quest~`Hero`s~Wings`~and~received~a~Super~Headgear~Pack!");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Your~inventory~is~full.~Make~some~room~first,~and~then~claim~the~reward,~later!");
                        }

                        break;
                    }
                case 50:
                    {
                        if (client.Player.Level < ActiveQuest22.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest22.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Desert Blood`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest22.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Harvey, in Desert City (495,579) is searching for a Divine Sword once possessed by an Obscure Warrior.")
                    .AddText("~Go try to help him.");
                            data.AddOption("Done!~Claim~my~reward!", 52)
                                .AddOption("Leave.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest21.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest21.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Desert Blood`!");
                            data.AddOption("Alright.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest21.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("Harvey, in Desert City (495,579) is searching for a Divine Sword once possessed by an Obscure Warrior.")
                  .AddText("~Go try to help him.");
                            data.AddOption("You~can~count~on~me.", 51)
                                .AddOption("None~of~my~business.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        client.Player.QuestGUI.Accept(ActiveQuest21, 0);
                        break;
                    }
                case 52:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest22.MissionId))
                        {
                            client.Inventory.Add(stream, 728242);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Desert~Blood`~and~received~a~Level~90~Elite~Ring~Pack!");
                        }

                        break;
                    }
                case 40:
                    {
                        if (client.Player.Level < ActiveQuest20.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest20.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the `Tomboy`s Marriage` quest, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest20.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Farmer Lynn in Desert City (461,661) feels great anxiety about his daughter, Carolyn, who still isn`t married at the age of 30.")
                    .AddText("~Will you go talk to him? I`ve prepared a whopping reward for a hero kind enough to help him out.");
                            data.AddOption("Claim~rewards.", 42)
                                .AddOption("Leave.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest19.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                      || client.Player.QuestGUI.CheckQuest(ActiveQuest19.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest: `Tomboy`s Marriage`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest19.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("Farmer Lynn in Desert City (461,661) feels great anxiety about his daughter, Carolyn, who still isn`t married at the age of 30.")
                  .AddText("~Will you go talk to him? I`ve prepared a whopping reward for a hero kind enough to help him out.");
                            data.AddOption("You~can~count~on~me.", 41)
                                .AddOption("None~of~my~business.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 42:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest20.MissionId))
                        {
                            client.Inventory.Add(stream, 728241);
                            client.CreateBoxDialog("You`ve~completed~the~`Tomboy`s~Marriage`~quest~and~received~a~Level~82~Elite~Necklace~Pack!");
                        }
                        break;
                    }
                case 41:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest19, 0);
                        client.CreateBoxDialog("Hurry~up~and~set~off,~right~away!");
                        break;
                    }

                case 30:
                    {
                        if (client.Player.Level < ActiveQuest18.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest18.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Ghost City`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest18.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Aunt Zhang (361,459) is looking everywhere for her son Mo Mo. He went missing in Twin City,")
                    .AddText("~the Taoist Shine should know the reason beneath this mystery.");
                            data.AddOption("Done!~Claim~my~reward!", 32)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest17.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                         || client.Player.QuestGUI.CheckQuest(ActiveQuest17.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Ghost City`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest17.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                        {
                            data.AddText("Aunt Zhang (240,326) is looking everywhere for her son Mo Mo. He went missing in Twin City, and his uncle believed that")
                  .AddText("~the Taoist Shine should know the reason beneath this mystery.");
                            data.AddOption("You~can~count~on~me.", 31)
                                .AddOption("None~of~my~business.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Hurry~up~and~set~off~right~away!");
                        client.Player.QuestGUI.Accept(ActiveQuest17, 0);
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest18.MissionId))
                        {
                            client.Inventory.Add(stream, 728246);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Ghost~City`~and~received~a~Level~87~Elite~Armor~Pack!");
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Player.Level < ActiveQuest16.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest16.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Mad~Man`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest16.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Du San in Ape City (552,601) was the nephew of General Du (396,233). Du San turned lunatic out of the blue, with snakebites all")
                    .AddText("~over his body. General needs somebody to kill those Snake Men for revenge.");
                            data.AddOption("Done!~Claim~my~reward!", 26)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest15.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                          || client.Player.QuestGUI.CheckQuest(ActiveQuest15.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Mad~Man`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Du San in Ape City (552,601) was the nephew of General Du (396,233). Du San turned lunatic out of the blue, with snakebites all")
                   .AddText("~over his body. General needs somebody to kill those Snake Men for revenge.");
                        data.AddOption("You~can~count~on~me.", 25)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 26:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest16.MissionId))
                        {
                            client.Inventory.Add(stream, 728245);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Mad~Man`~and~received~a~Level~67~Elite~Headgear~Pack!");
                        }

                        break;
                    }
                case 25:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest15, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off~right~away!", ActiveQuest15.FinishNpcId.Map, ActiveQuest15.FinishNpcId.X, ActiveQuest15.FinishNpcId.Y, ActiveQuest15.FinishNpcId.ID);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.Level < ActiveQuest14.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest14.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Soul~of~General`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest14.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("General Amber (879,562) in Ape City lived a heroic life. But his spirit couldn`t rest in peace because of unfinished business.")
                                .AddText("~He needs your help.");
                            data.AddOption("Done!~Claim~my~reward!", 22)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest13.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                              || client.Player.QuestGUI.CheckQuest(ActiveQuest13.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Soul of General`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("General Amber (879,562) in Ape City lived a heroic life. But his spirit couldn`t rest in peace because of unfinished business.")
                   .AddText("~He needs your help.");
                        data.AddOption("You~can~count~on~me.", 21)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 22:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest14.MissionId))
                        {
                            client.Inventory.Add(stream, 728244);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Soul~of~General`~and~received~a~Level~60~Elite~Boot~Pack!");
                        }
                        break;
                    }
                case 21:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest13, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off~right~away!", ActiveQuest13.FinishNpcId.Map, ActiveQuest13.FinishNpcId.X, ActiveQuest13.FinishNpcId.Y, ActiveQuest13.FinishNpcId.ID);
                        break;

                    }
                case 17:
                    {
                        if (client.Player.Level < ActiveQuest12.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest12.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Frantic Monkeys`, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest12.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ape City used to be at peace. But the monkeys outside the city strangely went frantic and began to attack villagers. Officer")
                                           .AddText("~Bao (553,584) needs your help to find out why.");
                            data.AddOption("Done!~Claim~my~reward!", 19)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest11.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                             || client.Player.QuestGUI.CheckQuest(ActiveQuest11.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Franti Monkeys`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Ape City used to be at peace. But the monkeys outside the city strangely went frantic and began to attack villagers. Officer")
                   .AddText("~Bao (553,584) needs your help to find out why.");
                        data.AddOption("You~can~count~on~me.", 18)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 19:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest12.MissionId))
                        {
                            client.Inventory.Add(stream, 728243);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Frantic~Monkeys`~and~received~a~Level~55~Elite~Weapon~Pack!");
                        }

                        break;
                    }
                case 18:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest11, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off~right~away!", ActiveQuest11.FinishNpcId.Map, ActiveQuest11.FinishNpcId.X, ActiveQuest11.FinishNpcId.Y, ActiveQuest11.FinishNpcId.ID);

                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < ActiveQuest10.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest10.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the `Antique Ring` quest, haven`t you?");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest10.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("A Minstrel (184,282) came to Phoenix Castle looking for the Mystic Ring. He heard that Antique Dealer Shen (779,477) has an Antique Ring")
                .AddText("~that can be converted into the Mystic Ring, when combined with a Meteor. He needs you to visit Dealer Shen and learn more.");
                            data.AddOption("Done!~Claim~my~reward!", 15)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                              || client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest: `Mystic Ring`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("A Minstrel (184,282) came to Phoenix Castle looking for the Mystic Ring. He heard that Antique Dealer Shen (779,477) has an Antique Ring")
                   .AddText("~that can be converted into the Mystic Ring, when combined with a Meteor. He needs you to visit Dealer Shen and learn more.");
                        data.AddOption("You~can~count~on~me.", 14)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 15:
                    {

                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest10.MissionId))
                        {
                            client.Inventory.Add(stream, 728240);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Mystic~Ring`~and~received~a~Level~52~Unique~Headgear~Pack!");
                        }
                        break;
                    }
                case 14:
                    {

                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~up~and~set~off,~right~away!", ActiveQuest9.FinishNpcId.Map, ActiveQuest9.FinishNpcId.X, ActiveQuest9.FinishNpcId.Y, ActiveQuest9.FinishNpcId.ID);


                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < ActiveQuest8.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve already completed the quest `Azure Pill`!");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Apprentice Luo (TwinCity 447,331) is waiting for his father, Artisan Luo. He has been out collecting feathers near the river (442,431) for")
                                .AddText("~days, and his son is growing more and more worried that he hasn`t returned. He needs your help to find his father.");
                            data.AddOption("Done!~Claim~my~reward!", 12)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                           || client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, unless you complete the quest: `Artisan`s Feather`!");
                            data.AddOption("I~see.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Apprentice Luo (TwinCity 447,331) is waiting for his father, Artisan Luo. He has been out collecting feathers near the river (442,431) for")
                      .AddText("~days, and his son is growing more and more worried that he hasn`t returned. He needs your help to find his father.");
                        data.AddOption("You~can~count~on~me!", 11)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Inventory.Add(stream, 728239);
                            client.CreateBoxDialog("You`ve~completed~the~`Artisan`s~Feather`~quest~and~received~a~Level~50~Unique~Weapon~Pack!");
                        }
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~find~Artisan~Luo!", ActiveQuest7.FinishNpcId.Map, ActiveQuest7.FinishNpcId.X, ActiveQuest7.FinishNpcId.Y, ActiveQuest7.FinishNpcId.ID);

                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < ActiveQuest6.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            data.AddText("You`ve already completed the quest `Azure Pill`!");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Scholar Wu (Phoenix Castle,214 231) has been suffering from a weird asthma for years. He heard Pharmacist Mu Mu (182,241) could")
                     .AddText("~cure his disease if some rare ingredients are at hand. He now needs you to give him a hand and collect the ingredients.");
                            data.AddOption("Done!~Claim~my~reward!", 9)
                                .AddOption("None~of~my~business!", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m not giving you anything, you know, unless you complete the quest `Azure Pill`!");
                            data.AddOption("Got~it.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Scholar Wu (Phoenix Castle,214 231) has been suffering from a weird asthma for years. He heard Pharmacist Mu Mu (182,241) could")
                      .AddText("~cure his disease if some rare ingredients are at hand. He now needs you to give him a hand and collect the ingredients.");
                        data.AddOption("I~won`t~let~you~down.", 8)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.Add(stream, 728238);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Azure~Pill`~and~received~a~Level~43~Unique~Ring~Pack!");
                        }
                        break;
                    }
                case 8:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`re~going~to~help~Scholar~Wu~collect~the~ingredients.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.Level < ActiveQuest4.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Hey! You`ve already completed the quest: `Spun Gold Armor`!");
                            data.AddOption("Oh,~yeah.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        data.AddText("Thief Wong (347,228) has stolen some jewelry from the notorious landlords. He wants to give these treasures to the poor in Maple")
                          .AddText("~the Lustful and his gangsters have something to do with it. You will need to disguise yourself and set a trap to capture him.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddOption("Done!~Claim~my~reward.", 6)
                           .AddOption("Later.", 255)
                          .AddAvatar(72).FinalizeDialog();
                        }
                        else
                        {
                            data.AddOption("You~can~count~on~me!", 5)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(729974, 1, stream);
                            client.Inventory.Add(stream, 728237);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`Spun~Gold~Armor`~and~received~a~Level~32~Unique~Armor~Pack!");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < ActiveQuest3.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Player.QuestGUI.SendAutoPatcher("You~have~accepted~the~quest:~`Spun~Gold~Armor`.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I won`t give you anything, you know, until you complete the quest: `Spun Gold Armor`!");
                            data.AddOption("Got~it.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level < ActiveQuest2.Lv_min)
                        {
                            data.AddText("Sorry, but you`ll need to reach at least Level " + ActiveQuest24 + " to accept the quest.");
                            data.AddOption("I~see.");
                            data.AddAvatar(72);
                            data.FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("You`ve~already~completed~the~quest:~`The~Connector`!");
                            data.AddOption("Yep.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Thief Wong (347,228) has stolen some jewelry from the notorious landlords. He wants to give these treasures to the poor in Maple")
                       .AddText("~Forest, but Thief Chen, his connector, is being cut off by Bandits. I need you to go and help them. It`s very urgent!");
                            data.AddOption("Done!~Claim~my~reward.", 3)
                                .AddOption("I`ll~be~back~soon.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I won`t give you anything, you know, because you haven`t finished the quest `The Connector`, yet!");
                            data.AddOption("Got~it.", 255)
                               .AddAvatar(72).FinalizeDialog();

                            break;
                        }
                        data.AddText("Thief Wong (347,228) has stolen some jewelry from the notorious landlords. He wants to give these treasures to the poor in Maple")
                         .AddText("~Forest, but Thief Chen, his connector, is being cut off by Bandits. I need you to go and help them. It`s very urgent!");
                        data.AddOption("You~can~count~on~me!", 2)
                            .AddOption("None~of~my~business!", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~accepted~the~quest:~`The~Connector`!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 120046);
                            client.CreateBoxDialog("You`ve~completed~the~quest~`The~Connector`~and~received~a~Refined~Necklace~Pack!");
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.GreenSnake)]
        //public static void GreenSnake(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3337906, 292);
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.Mulan)]
        public static void Mulan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1644);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1645);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1646);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1648);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1649);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Do you believe I can also do what any man can do? I`ll join the army and to be a general, someday. Then I`ll realize my father`s wish of serving the country. Yeah, that`s")
                            .AddText("~also my dream!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Are~you~Mulan?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("How~are~things~going,~now?", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Tell~me~all~you~know.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("General~Armand~sent~me~here.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Is~anything~wrong?", 40);

                        data.AddOption("You~did~have~a~great~mind!", 255);
                        data.AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did you see my father? No? Come on, I should keep my mind on combat. Please go to the Fishing Village and tell Harriet `I`m fine`, okay? Just that.")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(1).FinalizeDialog();

                            break;
                        }
                        data.AddText("No, I`m fine. I`m glad to hear that, and I really want to join the army, but I know I`m not strong enough. So, I decided to stay here and keep practicing.")
                            .AddText("~The only thing I worry about is my father. It`s just too early to return home. Could you do me a favor and go tell my father about my situation?")
                            .AddOption("No~problem.", 41)
                            .AddOption("I~have~more~important~things.", 255)
                            .AddAvatar(1).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("You~need~to~go~tell~Harriet~about~Mulan`s~situation~and~comfort~him.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Add(stream, 150197, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~20~minutes~of~EXP,~2,000~Silver~and~a~Diamond~Ring(B)!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("Really? General Armand asked me to return to the army? That`s wonderful! Wait... Um... I think I need more time to consider it.")
.AddOption("Alright.", 40)
.AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }

                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why are you still here? You haven`t informed General Armand of the Banditti`s plan, have you? It`s an emergency! Please be quick!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(1).FinalizeDialog();

                            break;
                        }
                        data.AddText("Hush! When I was looking for the Royal Sword, I found the Banditti plotted to ally with the Bandits L98 and attack the army from the back. If their plan succeeds, you")
                                        .AddText("~know what it means to the whole island. It will be a disaster! Could you go inform General Armand? Hurry up!")
        .AddOption("I`ll~do~it,~right~now.", 21)
        .AddOption("Why~not~tell~him~by~yourself?", 255)
        .AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("You~need~to~tell~General~Armand~about~the~Banditti`s~plan~within~2~minutes.~Hurry!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711475, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711475, 1, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    data.AddText("Yeah, that`s my Royal Sword! Without your help, I don`t even know how to face my father. I`ll be more cautious with those cunning Banditti. However, I feel I`m not")
                                        .AddText("~their real target.")
        .AddOption("You`d~better~return~home,~now.", 20)
        .AddAvatar(1).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Did you find the Royal Sword? No? My father will be angry with me, and he won`t forgive me. He treats that sword as his life. What can I do now?")
.AddOption("I`ll~go~look~for~it,~again.", 255)
.AddAvatar(1).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Um... Not very well. General Armand found out I was a girl soon after I arrived the camp. Although he didn`t reveal my true identity, he forced me to leave...")
.AddOption("Then~you~should~return~home.", 11)
.AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I`ve thought of that, but I can`t return now, since the Royal Sword was taken by the Banditti. If they weren`t hiding in the dark, I wouldn`t be caught so easily.")
                            .AddText("~Anyway, I should take back the sword. You know, that was given by the Emperor to commemorate my father`s military exploits. It`s very important for my family.")

.AddOption("Don`t~worry.~I`ll~go~find~it.", 12)
.AddOption("That`s~terrible.", 255)
.AddAvatar(1).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("You~need~to~kill~the~Banditti~to~retrieve~the~Royal~Sword.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP,~3,000~Silver~and~a~Diligence~Book!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("How do you know my name? Did my father send you here? Alright. You don`t need to try and persuade me, since I won`t return home before realizing my father`s dream.")
.AddOption("You`re~the~dream~of~Harriet.", 10)
.AddAvatar(1).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Harriet)]
        public static void Harriet(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1643);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1644);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1649);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Harriet, client.Player.Class, 1652);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Alas, you can`t understand how sad I was when I was forced to leave the army, due to injuries. You know, the battlefield was where my dream started.")
                            .AddText("~But now, I can only play with my hoe on the farm. Son? I don`t have a son, but a daughter. Haha, you must be kidding! How could a girl join the army? It`s illegal.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Where~is~Mulan?", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Mulan?", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~found~Mulan.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Flower~Wine.", 30);
                        data.AddOption("Take~it~easy.", 255);
                        data.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711477, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {

                                    client.Inventory.Remove(711477, 1, stream);
                                    uint[] Items = new uint[]
                                {
                                    (uint)(700000 + (uint)Role.Flags.Gem.RefinedDragonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedFuryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedGloryGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedKylinGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedMoonGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedPhoenixGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedRainbowGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedThunderGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedTortoiseGem)
                                    ,(uint)(700000 + (uint)Role.Flags.Gem.RefinedVioletGem)
                                };
                                    uint ItemID = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                                    client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~a~Refined~Gem!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Is that Flower Wine in your hands? It really smells nice! Owing to your help, I can have a taste of the Flower Wine, again. Why not drink with me?")
        .AddOption("No,~thanks.", 255)
        .AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You haven`t found the Flower Wine? What a pity! You know I haven`t had a good night since I smelt that bouquet. Could you understand?")
                          .AddOption("I`ll~go~check,~again.", 255)
                          .AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Hey! Do you smell something nice? It must be the Flower Wine! I bet this good bouquet is from the Banditti in the west. Eh... When did I drink Flower Wine last?")
                                  .AddText("~Perhaps, 2 or 3 years ago... That`s really an unforgettable taste. If you are able to bring me some Flower Wine, I`d be very grateful.")
                            .AddOption("No~problem!~Haha...", 31)
                            .AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("You~need~to~kill~the~Banditti~to~collect~the~Flower~Wine.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.Inventory.Add(stream, 160197, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~20~minutes~of~EXP,~2,000~Silver~and~a~pair~of~Unique~Kylin~Boots(B)!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("Really? Where is she now? Why didn`t you bring her home? Wait, wait... You mentioned that General Armand appreciated her effort and wanted to recruit her in the army?")
                                .AddText("~Is that true? Oh, my dear daughter! In that case, I don`t need to force her to return home. I just hope Mulan will be safe and happy.")
.AddOption("I~hope~so,~too.", 255)
.AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You can`t find Mulan? Alas! What should I do? She is my only child... If she was caught by the bandits on the Island, I wouldn`t dare to continue living alone.")
                                .AddText("~Please, go look for her, again.")
     .AddOption("Okay.~I~will.", 255)
     .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("Yeah, that`s my daughter. Wait, that`s Mulan who bought me the wine... and she mentioned my dream, last night. Could this silly girl be thinking of joining the army for")
                                       .AddText("~me? Oh, no! If the general finds out, she will be put to death! I can`t move that fast, so I wonder whether you can go find her? Please, bring")
                                       .AddText("~her home! I won`t mention my military dreams, anymore.")
   .AddOption("I`ll~go~look~for~her.", 11)
   .AddOption("I~have~more~important~things.", 255)
   .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~look~for~Mulan~and~advise~her~to~return~home.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(1002030, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.Inventory.Remove(1002030, 1, stream);
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP,~3,000~Silver~and~5~Ginsengs!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                    data.AddText("Yeah, this is what I need. Don`t worry, I feel better now. Without you, I should have died at home. By the way, did you see Mulan?")
                                        .AddText("~How can she leave me at home, alone!")
    .AddOption("Anyway,~you~are~fine~now.", 10)
    .AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Where is the Refreshing Pill? You know where the Storekeeper is, right? Go buy a Refreshing Pill from her. I`m in urgent need of one to cure my headache.")
      .AddOption("I`m~on~my~way.", 255)
      .AddAvatar(10).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Yeah, where is my daughter? Mulan? Mulan? Gods, this headache is killing me. I must have drank too much, last night. Could you do me a favor and buy me a Refreshing Pill?")
                         .AddText("~I`m sure you can get one from the Storekeeper. Be quick!")
    .AddOption("Okay.~I`ll~be~back~soon!", 2)
    .AddOption("Sorry,but~I~don`t~carry~money.", 255)
    .AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("You~need~to~buy~a~Refreshing~Pill~from~the~Storekeeper~for~Harriet.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TempestSecretLetter)]
        public static void TempestSecretLetter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            if (!client.Inventory.Contain(3006393, 1))
            {
                //Sun~Diamond,~Moon~Diamond,~Star~Diamond~and~Cloud~Diamond
                client.CreateBoxDialog("Sorry, you don`t have a TempestSecretLetter.");
                return;
            }



            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Taskmaster Chang (JobCenter 51,49) has news about the legendary talisman, Tempest Wings.")
                             .AddText("~If you`re interested, pay a visit to him.")
                          .AddOption("Visit~Taskmaster~Chang.", 1)
                          .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.SendSysMesage("You received 30 minutes of EXP!", MsgMessage.ChatMode.System);
                        client.GainExpBall(300, false);
                        client.Player.QuestGUI.SendAutoPatcher(1004, 51, 49, (uint)NpcID.TaskmasterChang);
                        client.Inventory.Remove(3006393, 1, stream);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralArmand)]
        public static void GeneralArmand(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Alvis, client.Player.Class, 1636);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Mulan, client.Player.Class, 1646);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1647);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1648);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1639);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1642);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The most regretful thing of all, that man devoted all he had, but died without accomplishing what he wanted. We should hurry to seize the day!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Be~careful~of~the~Banditti!", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Is~there~anything~I~can~help?", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Is~there~anything~I~can~help?", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Excuse~me,~do~you~know~Alvis?", 1);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Task:~Return~Favors.", 40);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Task:~Bandit`s~Saber.", 50);

                        data.AddOption("Take~it~easy.", 255);
                        data.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711474, 10))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                                {
                                    client.Inventory.Remove(711474, 10, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~a~bottle~of~Exp~Potion!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Inventory.Add(stream, Database.ItemType.DoubleExp);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Wow, these Short Sabers are all of high quality. I knew you wouldn`t let me down. Thanks for your help!")
              .AddOption("You`re~welcome.", 255).AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Have you collected 10 Short Sabers? It seems not. You`re not going to sneak away, are you? The Bandits L98 are no match for you. Take it easy, okay?")
    .AddOption("Alright.~I`m~on~my~way.", 255).AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Sigh... It`s all my fault. If I didn`t underestimate the enemy, my troops wouldn`t suffer such heavy casualties in the battle. Anyway, the most")
                                       .AddText("~important thing is to collect some weapons. I noticed the Short Sabers those Bandits L98 use weren`t so bad. If you are able to bring me 10 Short Sabers, I`d")
                                       .AddText("~be very grateful.")
             .AddOption("You~can~count~on~me.", 51)
             .AddOption("Bandits?~I`ll~be~killed!", 255).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("You~need~to~kill~the~Bandits~L98~and~collect~10~Short~Sabers~from~them.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did you see the Village Head? No? Come on, don`t tell me you are lost. Hurry to the Fishing Village and send my gift to the Village Head, okay?")
     .AddOption("I`m~on~my~way.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hey! We have smashed the den and eliminated hundreds of bandits! See, people are all cheering our crushing victory. Without help from the Village Head, we couldn`t")
                                 .AddText("~achieve all this so easy. Are you going to return to the Fishing Village? Great! I wonder whether you can send this Jewelry Bag to the Village Head for me and")
                                 .AddText("~deliver my sincere thanks to him?")
       .AddOption("No~problem.~I`ll~tell~him.", 41)
       .AddOption("I~have~more~important~things.", 255).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711471);
                        client.CreateBoxDialog("You~need~to~send~the~Jewelry~Bag~to~the~Village~Head.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText(" From Mulan`s issue, I think I should never underestimate anyone. Will Mulan return?")
           .AddOption("I~hope~so.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("Mulan? Yeah, I totally forgot that it was Mulan who discovered the Banditti`s plan. Without her report, we may have been defeated. I`m so sorry to think so little of that girl.")
                                        .AddText("~Tell you what... Could you ask her to return to the army? I`ll keep the secret for her, as long as she fights like a man!")
              .AddOption("I`d~like~to~have~a~try.", 31).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("General~Armand~asked~you~to~find~Mulan~and~invite~her~back~to~the~army.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 3, 2))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Aha, these Banditti are hastily retreating from our land. It`s good enough. We don`t need to press an enemy in retreat. Thanks for your help!")
                                        .AddText("~Why not celebrate for the victory with us, tonight?")
              .AddOption("We~will~get~a~chance.~Bye!", 30).AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("I didn`t expect there would be so many enemies! Are you okay? Try your best to kill as many as possible!")
    .AddOption("I~can`t~be~better.~Let`s~go!", 255).AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I`m really looking forward to it. Actually, some Banditti have snuck into the camp! I hope you can join the fight with us. Will you?")
    .AddOption("Sure,~you~have~me~now.", 21).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        Server.AddMapMonster(stream, client.Map, 55, client.Player.X, client.Player.Y, 8, 8, 3, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        Server.AddMapMonster(stream, client.Map, 79, client.Player.X, client.Player.Y, 8, 8, 3, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        break;
                    }
                case 10:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("What? The Banditti want to ally with the Bandits L98, then attack my troops? Were their brains eaten by zombies? I`ll go intensify the defenses, right away.")
                                .AddText("~Thanks for the news!")
      .AddOption("My~pleasure.", 20).AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711468, 1, stream);
                            client.Inventory.Add(stream, 117087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~a~Unique~Jade~Earring(B)!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 4000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Alvis)]
        public static void Alvis(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1634);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Alvis, client.Player.Class, 1635);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Alvis, client.Player.Class, 1636);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Heeelp! The food supply was...was...taken. I can`t die like this! Help...me.....");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Hey!~Are~you~Alvis?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Food~Delivery~II.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Food~Delivery~III.", 20);

                        data.AddOption("It`s~not~me.~Let~me~go!", 255);
                        data.AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What? You lost the food supply? Fortunately, I just gave you part of it. Anyway, here are the remains. Keep them well, and send them to General Armand!")
    .AddOption("Don`t~worry.~I~will.", 255).AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        data.AddText("I`m afraid! Could you...could you go deliver this box of food to General Armand, on the Central Island? I hope you can finish this task for me.")
                            .AddText("~Remember, to General Armand. I won`t, won`t forget your help...")
       .AddOption("I`ll~take~care~of~this.", 21)
       .AddOption("I~have~more~important~things.", 255)
       .AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711468);
                        client.CreateBoxDialog("You~need~to~send~this~box~of~food~to~General~Armand.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711468, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711468, 1, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Yeah, this is exactly what I lost. For...Fortunately, this box of food isn`t in the wrong hands, now.")
        .AddOption("Hey,~are~you~okay?", 20)
        .AddAvatar(9).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Did you find the food supply? No? Please, I can`t die with this great shame. I can`t...")
                              .AddOption("Come~on,~I`ll~go~find,~again.", 255)
                              .AddAvatar(9).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("The Bandits L97 looted the food supply. Could you... could you help me to retrieve them? You wouldn`t deny a dying request, would you?")
.AddOption("I`ll~go~find~the~food~supply.", 11)
.AddOption("Bandits?~My~goodness!", 255)
.AddAvatar(9).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~retrieve~the~food~supply~from~the~Bandits~L97.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Did th... the Village Head sent you here? I`m soooo sorry to make him disappointed. I don`t want to die like this...")
.AddOption("Hold~on,~you~will~be~fine.", 10)
.AddAvatar(9).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BiVillageHead)]
        public static void BiVillageHead(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1633);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1634);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1641);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BiVillageHead, client.Player.Class, 1650);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralArmand, client.Player.Class, 1639);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Here, our way is simple and people are hardworking. But since the Bandits L97 came, peace is no longer with us. Monsters from other islands continuously")
                            .AddText("threaten to destroy our village. As the head of this village, I should take full responsibility to protect the people, here. However, all of the strong villagers")
                            .AddText("~have been drafted into the army. How could I lead the weak that were left to fight against the cruel bandits?");



                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hey!~You~have~me,~now!", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("~Food~Delivery~!", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~Bandits~L98.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Banditti.", 30);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Return~Favors.", 40);

                        data.AddOption("It`s~terrible...", 255);
                        data.AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Inventory.Remove(711471, 1, stream);
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 5719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Really? Those bandits are still enjoying beer and women? Good. I`ll make this disorderly mob collapse at the first blow!")
                                .AddText("~Thanks for telling me!")
.AddOption("It`s~my~pleasure!", 255)
.AddAvatar(56).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 30))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
               .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~5~Ginsengs!");
                                    client.IncreaseExperience(stream, 5719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Finally, we are rid of those Banditti! My hero, please wait! We have prepared a feast to thank you. Would you mind staying here and celebrating with us?")
        .AddOption("Thanks,~but~maybe~next~time.", 255)
        .AddAvatar(56).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("It seems you haven`t killed 30 Banditti, have you? Make haste, please! I hope you can restore peace to our village.")
                           .AddOption("I`m~on~my~way.", 255)
                           .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Oh, no! Someone told me 30 Banditti from the west and the northwest gathered outside the Fishing Village, again. It`s really annoying. Could you please go kill them?")
                            .AddOption("Don`t~worry.~It`s~on~me.", 31)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("You~need~to~kill~30~Banditti.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 30))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {

                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 5719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You`ve killed 30 Bandits L98? Great! It`s the SECOND time you saved our village! I can never thank you enough.")
        .AddOption("It`s~my~pleasure.", 255)
        .AddAvatar(56).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("It couldn`t be true. I`m sure those Bandits L98 are still watching our village, nearby. Please go check again, okay?")
                  .AddOption("Alright.", 255)
                  .AddAvatar(56).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("It`s so good to see you! Those Bandits L97 haven`t give up their evil designs, and summoned lots of Bandits L98 from the desert. I can`t wait for them to rush")
                            .AddText("~our village. The best way to defend is to attack! One told me a group of Bandits L98 are lingering around. Could you go kill 30 Bandits L98 for us?")
                 .AddOption("Don`t~worry.~I~won`t~sit~back.", 21)
                 .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("You~need~to~kill~30~Bandits~L98.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What? You didn`t find Alvis? It`s really worrying. According to his route, Alvis should be on the Central Island. Could you go there and check again?")
                    .AddOption("Alright.", 255)
                    .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        data.AddText("Um... It has been more than 15 days since Alvis went to deliver the food supply to General Armand on the Central Island, and I received no reply back. It shouldn`t be more")
                            .AddText("~than 10 days, unless Alvis had some kind of accident. Could you do me a favor and go have a look?")
                        .AddOption("Yeah,~I`m~on~it.", 11)
                        .AddOption("I~have~more~important~things.", 255)
                        .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~go~to~the~Central~Island~and~find~Alvis~around~(324,192).");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 50))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
               .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {

                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP,~4,000~Silver~and~an~EXP~Ball~Scrap!");
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great job! I really don`t know how to thank you. You saved the whole village!")
        .AddOption("It`s~my~pleasure.", 255)
        .AddAvatar(56).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Have you killed 50 Bandits L97? It seems the answer is no... Look, if you just kill a few of them, you will find that you`re only provoking them.")
                                    .AddText("~Then, these bullies will take more revenge on us. I hope you can keep your promise to protect us from start to finish. You know, the bandits are on the")
                                    .AddText("~Central Island.")
                                    .AddOption("Okay.~I`m~on~my~way.", 255)
                                    .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("You? I believe you are an able body, but there are many bandits, and all our young men have been drafted into the army. Are you sure you can deal with those brutes alone?")
                          .AddOption("Right.~You~can~count~on~me!", 2)
                          .AddOption("I~guess~I~was~too~impulsive...", 255)
                          .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        data.AddText("It`s very kind of you! So, can you go kill 50 Bandits L97 on the Central Island, now? I suppose that is the best way to scare the other bandits away.")
                         .AddOption("Great!~I`m~going...", 3)
                         .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You~need~to~go~kill~50~Bandits~L97~on~the~Central~Island.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.AdjutantLi)]
        public static void AdjutantLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AdjutantLi, client.Player.Class, 6131);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hey! You seem like an able body. Why not join the army? I`ll recommend you to the general!");



                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I~came~to~deliver~gifts.", 1);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Deadly~Trap.", 10);

                        data.AddOption("You~must~be~kidding.~Bye!", 255);
                        data.AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720995, 1) && client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(720995, 1, stream);
                                    client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("How are things going, now? Really? Those stupid Bandits L98 ate up all the Salted Fish? Aha, now we just need to wait and clear their bodies away from our land!")
                                        .AddText("~Did I say thank you? You`re incredible useful!")
        .AddOption("It`s~my~pleasure.", 255)
        .AddAvatar(10).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Are you ready to carry out the `Deadly Plan`? Go poison the Bandits L98!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(10).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Buddy, I got a great idea to eliminate the Bandits L98! As you know, people here love the Salted Fish very much, including the Bandits L98. If they eat poisonous Salted Fish, things will be")
                                .AddText("~easy for us. You agree with me, right? Then, I`ll count you in the plan. Look, this is a bottle of Poison Liquid. I want you to take a pack of Salted Fish from Old Zhang,")
                                .AddText("~then use the Poison Liquid on the Salted Fish. When the liquid diffuses into the Salted Fish completely, you can entice the Bandits L98 to eat the Poison Fish. They`ll never see it coming!")
                                .AddText("~So?")
.AddOption("Got~it,~sir!", 11)
.AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
.AddOption("Let me check.", 255)
.AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720994);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711472, 1, stream);
                            client.Inventory.Remove(711473, 1, stream);
                            client.CreateBoxDialog("You~received~30~minutes~of~EXP,~3,000~Silver~and~a~Defense~Pot~(20m)!");
                            client.Inventory.AddItemWitchStack(723017, 0, 1, stream);
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Gifts? Wow, the Salted Fish smells so good! It`s very kind of the people from the Fishing Village! We are just doing our jobs.")
                                .AddText("~And what`s that? A Fishing Net? Yeah, we can use this net to make Salted Fish by ourselves! Old Zhang is so considerate.")
.AddOption("I`m~glad~you~like~them.", 255)
.AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        public static void BandittiFlowers(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1653);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "CollectingRainbowFlower", 2);

            }
        }
        [NpcAttribute(NpcID.OldZhang)]
        public static void OldZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);//1631);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1653);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Greetings! Have you ever tasted this Salted Fish? You should never miss this special food, when you come to our Fishing Village. Suppose you are eating a dried fish")
                            .AddText("~with the right salty taste, what would you think? You can even feel the warm GameServer and refreshing sea wind while eating! That`s amazing, right?");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Can~I~have~a~taste?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Rainbow~Flowers.", 10);

                        data.AddOption("Yes,~indeed.", 255);
                        data.AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711478, 10))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711478, 10, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Yeah, these flowers are exactly what I am looking for. Actually, the Rainbow Flower is my secret! By chance, I found the bait made from the Rainbow Flowers and rice would")
                                        .AddText("~attract more fish. Now, I`ve told you all I know. Promise you won`t tell anyone, okay?")
        .AddOption("I~swear~I~won`t.", 255)
        .AddAvatar(111).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Just as its name indicates, the Rainbow Flower shows bright colors in the sun. So, you can easily recognize them. Remember to bring me 10 Rainbow Flowers!")
                       .AddOption("No~problem.", 255)
                       .AddAvatar(56).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Speaking of fishing skill, no one in the village is better than me. Haha, alright. I`ll tell you my secret, if you are able to bring me 10 Rainbow Flowers from the")
                         .AddText("~West Island and the Northwest Island. So, what do you say?")
                            .AddOption("Okay.~I`ll~be~back~soon!", 11)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You~need~to~collect~10~Rainbow~Flowers~from~the~West~Island~and~the~Northwest~Island.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The gift itself may be small, but the goodwill is deep. Remember to express my sincere gratitude to Adjutant Li on the Central Island, too.")
                           .AddOption("They~will~understand.", 255)
                           .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        data.AddText("Of course! Here you are. How is the taste? I knew you would like it! By the way, did you notice the soldiers on the Central Island? They came to eliminate the")
                              .AddText("~bandits. I do appreciate their effort. Could you send this bag of Salted Fish to Adjutant Li of the army?")
                            .AddOption("No~problem.~Anything~else?", 2)
                            .AddOption("I~have~more~important~things..", 255)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Yeah, and this Fishing Net. They can go fishing in their free time. Hope they like the Salted Fish!")
                             .AddOption("You`re~very~kind.", 3)
                             .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You~need~to~send~the~Fishing~Net~and~Salted~Fish~to~Adjutant~Li.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FishingNet)]
        public static void FishingNet(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "FishingNet", 3);
            }

        }
        [NpcAttribute(NpcID.SaltedFish)]
        public static void SaltedFish(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldZhang, client.Player.Class, 1640);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AdjutantLi, client.Player.Class, 6131);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "SaltedFish", 3);
            }
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                client.Player.AddPick(stream, "SaltedFish", 3);
            }
        }
        [NpcAttribute(NpcID.SoldiersMother)]
        public static void SoldiersMother(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1631);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldiersMother, client.Player.Class, 1632);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldiersMother, client.Player.Class, 1651);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("My son has been away from home for almost 2 years. I`m really worried about him. I hope")
                            .AddText("~he can come back to me, soon.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Soldier`s~Letter.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~Bacon.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~Yarn~Ball.", 20);

                        data.AddOption("He`ll~be~back.", 255);
                        data.AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711476, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.Inventory.Remove(711476, 1, stream);
                                    client.CreateBoxDialog("You~received~40~minutes~of~EXP~and~4,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("That`s the Yarn Ball! I believe my son will be more handsome if he wears the new clothes.")
                                        .AddText("~I hope he`ll come back, soon.")
        .AddOption("He`ll~be~back,~soon.", 255)
        .AddAvatar(111).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! My son is still wearing the old clothes he wore on the day he left home! That just won`t do!")
 .AddOption("Ok.", 255)
 .AddAvatar(111).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Brave hero, please help me! I want to make new clothes for my son, but the Yarn Ball was stole by a Banditti.")
                            .AddText("~Could you help me retrieve it?")
  .AddOption("Ok.", 21)
  .AddOption("Sorry~but~I`m~busy.", 255)
  .AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Kill~the~Banditti~and~retrieve~the~Yarn~Ball.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711467, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711467, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~40~minutes~of~Exp,~4,000~silver~and~1~Diligence~Book!");
                                    client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thank you for retrieving the bacon for me. If my son was home, the monsters wouldn`t have stolen my things so easily.")
        .AddOption("Take~care.", 255)
        .AddAvatar(111).FinalizeDialog();
                                }
                            }

                            else
                            {

                            }

                            break;
                        }
                        data.AddText("A few days ago, several Bandits stole my bacon, and I`m running out of food! Could you")
                            .AddText("~please help me retrieve some of my lost bacon?")
.AddOption("Ok.", 11)
.AddOption("Sorry,~but~I`m~busy.", 255)
.AddAvatar(111).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Help~Soldier`s~Mother~retrieve~5~pieces~of~Bacon~from~the~BanditL97.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711466, 1, stream);
                            client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~1~Small~Joy~Stone!");
                            client.Inventory.AddItemWitchStack(720173, 0, 1, stream);
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("What? A letter from my son? I feel much better now, since he says everything is fine. Thank you!")
.AddOption("You`re~welcome.", 255)
.AddAvatar(111).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierZhang)]
        public static void SoldierZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1623);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1630);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1631);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("The HawKings are so terrible...");

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Can~I~help~you?", 1);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Revenge", 10);
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Letter.", 30);
                        if (client.Player.QuestGUI.CheckQuest(10, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("An~accessory", 40);

                        data.AddOption("Be~careful.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(10, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            client.Player.QuestGUI.Accept(new Database.QuestInfo.DBQuest() { MissionId = 10, Intentions = 1 }, 0);
                        if (client.Player.QuestGUI.FinishQuest(10))
                        {
                            client.CreateBoxDialog("Congratulations!~You~received~20~minutes~of~Exp,~2,000~silver~and~1~Diligence~Book!");
                            client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("What? You got it from a soldier`s corpse? No! It must be my brother. I have a same one like this. Thank you, anyway.")
.AddOption("Take~care.", 255)
.AddAvatar(35).FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please bring this letter to my mother. My home is on north Fishpond Island.")
      .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        data.AddText("I`ve been here for 2 years, and never went back home. My old mother lives alone, on north Fishpond Island.")
       .AddOption("Do~you~miss~her?", 31).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Of course. But the only thing I can do is to pray that she is healthy. Could you please give this letter")
                            .AddText("~to her? I want my mother to know everything is good, here.")
   .AddOption("Ok.", 32)
   .AddOption("Sorry,~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
       .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711466);
                        client.CreateBoxDialog("Send~the~soldier`s~letter~to~his~mother.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30, 20))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Ginseng!");
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You killed the monsters, but my friends still won`t come back... Thanks for your help, anyway.")
        .AddOption("Good~luck.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Kill the Hawks! That`s the only way to avenge my dead friends.")
                                             .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I`m dreaming of my dead friends, everyday. Just painful... How I wish I could avenge them! But I`m just too")
                            .AddText("~weak to kill the Hawks, myself. Could you please kill 30 HawKings and 20 HawkL93 for me?")
             .AddOption("Ok.", 11)
             .AddOption("Sorry,~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~30~HawKings~and~20~HawkL93.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You did it! Thank you very much! Rest in peace, my friends...")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! I hope that they will rest in peace!")
                .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("This war was so awful. Though we won, many soldiers died during the war. Could you please kill some HawkL93")
                  .AddText("~and collect their heads? I want you to cremate the heads and dead soldiers` bodies as a memorial.")
                  .AddOption("Okay.", 2)
                  .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Collect~5~heads~of~the~HawkL93~and~cremate~them~with~dead~soldiers`~bodies.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        public static void SoldierBird(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZhang, client.Player.Class, 1623);

            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                switch (Option)
                {
                    case 0:
                        {

                            data.AddText("Are you sure you want to burn the Soldier`s Corpse and the Hawk Head?");

                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Yes.", 1);

                            data.AddOption("No.", 255);
                            data.AddAvatar(35).FinalizeDialog();
                            break;
                        }
                    case 1:
                        {
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
                            packet.ID = MsgStringPacket.StringID.Effect;
                            packet.UID = client.OnRemoveNpc.UID;
                            packet.Strings = new string[1] { "M_Fire1" };
                            client.Player.View.SendView(stream.StringPacketCreate(packet), true);


                            client.OnRemoveNpc.Respawn = DateTime.Now.AddSeconds(10);
                            client.Map.RemoveNpc(client.OnRemoveNpc, stream);
                            client.Map.soldierRemains.TryAdd(client.OnRemoveNpc.UID, client.OnRemoveNpc);

                            break;
                        }
                }
            }
        }
        [NpcAttribute(NpcID.MysteryTaoist)]
        public static void MysteryTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1619);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1622);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1624);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1625);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1627);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1628);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1629);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("So my brother sent you here? The HawKings are really a great trouble to us. I will kill as many as possible.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("I`ll~stop~them", 1);

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Claws~and~Fangs.", 10);

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Hawk`s~Treasure.", 20);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Hawk`s~Treasure~2.", 30);

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle.", 40);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle~2.", 50);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle~3.", 60);
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Elemental~Circle~4.", 70);

                        data.AddOption("Good~luck.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest8.MissionId, 1))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {

                                    data.AddText("Please make 1 more space in your inventory.")
                                          .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                                {
                                    client.Inventory.Add(stream, 120187, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~a~Level~97~Unique~necklace~(B)!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Without their leader, the Hawks will be easily destroyed. Thanks for your help!")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                if (client.Player.View.ContainMobInScreen("HawkLeader") == false)
                                    Server.AddMapMonster(stream, client.Map, 4431179, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                                data.AddText("Sorry, but I`m busy activating the elemental circle.")
      .AddOption("Good~luck!", 255)
      .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Once the elemental circle is activated, the leader of the Hawks will appear. He will disappear after 1 minute,")
                     .AddText("~so you`ll need to hurry! Are you ready?")
                     .AddOption("Yes.", 71)
                     .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 71:
                    {
                        if (client.Player.View.ContainMobInScreen("HawkLeader") == false)
                            Server.AddMapMonster(stream, client.Map, 4431179, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.CreateBoxDialog("Kill~the~Leader~of~the~Hawks~within~1~minute,~after~the~elemental~circle~is~activated.~Are~you~ready?");
                        client.Player.QuestGUI.Accept(ActiveQuest8, 100);
                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 10))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                                {
                                    client.Inventory.Remove(711465, 1, stream);
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Panacea!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);

                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thanks for your help! The smell will lure the HawkL93 into the elemental circle, and then we can kill them all.")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You haven`t collected 10 scents of the HawkL93. Hurry up!")
                                   .AddOption("I`m~on~my~way.", 255).AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The elemental circle is finally complete! Now, I need you to collect 10 scents of the HawkL93.")
                        .AddText("~We can place the scent inside of the circle, to lure them into it. Here is a scent bag. You can use it to")
                        .AddText("~store the scent.")
                        .AddOption("Easy.", 61)
                        .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711465);
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        client.CreateBoxDialog("Go~and~collect~10~scents~of~the~HawkL93.");
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                                {

                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp~and~3,000~silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);

                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great! Now I can focus on the elemental circle. Once complete, the HawKings will all be killed!")
        .AddOption("I`m~looking~forward~to~it.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! We can`t give the HawKings the chance to disturb the summoning of the elemental circle.")
                          .AddOption("I~see.", 255).AddAvatar(35).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I`m ready to summon the elemental circle. Would you please go to the southern bridge and ask")
                           .AddText("~the Truth Taoist to prevent the HawKings from disturbing me? He may also need your help.")
                           .AddOption("Alright.", 51)
                           .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Go~and~talk~with~the~Truth~Taoist.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711461, 1) && client.Inventory.Contain(711462, 1)
                                && client.Inventory.Contain(711463, 1) && client.Inventory.Contain(711464, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                                {
                                    client.Inventory.Remove(711461, 1, stream);
                                    client.Inventory.Remove(711462, 1, stream);
                                    client.Inventory.Remove(711463, 1, stream);
                                    client.Inventory.Remove(711464, 1, stream);
                                    client.Inventory.AddItemWitchStack(1060024, 0, 1, stream);

                                    client.CreateBoxDialog("Congratulations!~You~received~40~minutes~of~Exp,~4,000~silver~and~1~Bird~Island~Gate!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);

                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Now I`ve got all 5 items! I hope the elemental circle will have a great effect. Thanks for your help!")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You haven`t collect all the items I need. Hurry up!")
                          .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("After a long time of studying, I`ve come up with an elemental circle that can lure out the leader of the")
                            .AddText("~HawKings. If we kill the leader, things will become easier! Could you lend me a hand?")
                            .AddOption("Of~course.", 41)
                            .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        data.AddText("To complete the elemental circle, we need 5 items, each standing for an element. I`ve got the earth item. Could you please")
                           .AddText("~collect the Birdman`s golden beak, Ximen Qing`s wood sword, Cook Yuan`s water bag and Wealthy Wan`s gunpowder?")
                           .AddOption("Ok.", 42)
                           .AddOption("Sorry~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        client.CreateBoxDialog("Collect~the~items~which~stand~for~the~gold,~water,~wood~and~fire~elements.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711460, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.Inventory.Remove(711460, 1, stream);
                                    client.Inventory.Add(stream, 152187);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~a~Unique~Blue~Bracelet~(B)!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);

                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("These treasures were actually taken away from the people here. Now, we should return the treasures")
                                        .AddText("~to their owners. Thanks for your help!")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Hurry up! We must take away all the treasures before the Hawks notice it.")
                                .AddOption("Ok.", 255).AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Finally, we`ve got the location of the HawkL93`s treasure! It`s buried under the tree (445, 681).")
                                    .AddText("~Could you please go and dig it up? I`ll distribute what we make to the poor of the village.")
                                 .AddOption("Ok.", 31)
                                 .AddOption("Sorry,~but~I`m~busy.", 255).AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Find~the~tree~(445,~681)~and~dig~up~the~treasure.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711459, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.Inventory.Remove(711459, 1, stream);
                                    client.Inventory.AddItemWitchStack(1002010, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Ginseng!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thanks for helping me reveal the truth, brave hero! Now, I just need some time to find out where the treasures are hidden.")
        .AddOption("You`re~welcome.", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("This map may help us find some treasures, or even secrets of the HawkL93! Please go and")
                                    .AddText("~collect some blood of the hawks to reveal the secret of the map!")
                                 .AddOption("Okay.", 255).AddAvatar(35).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("A few days ago, I accidently stumbled upon the HawkL93`s treasure map, but it is cursed that only the blood of the HawkL93")
   .AddText("~can reveal the true path. Could you help me collect some of their blood?")
                            .AddOption("I`ll~try~my~best.", 21)
                            .AddOption("Leave.", 255)
     .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711458);
                        client.CreateBoxDialog("Collect~some~Hawk~Blood~to~reveal~the~secret~of~the~treasure~map.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711456, 1) && client.Inventory.Contain(711457, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711456, 1, stream);
                                    client.Inventory.Remove(711457, 1, stream);
                                    client.Inventory.AddItemWitchStack(720173, 0, 1, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~1~Small~Joy~Stone!");
                                    client.IncreaseExperience(stream, 3719000);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("I will find out the weakness of the HawkL93 with all these things you collected for me. Thanks a lot!")
        .AddOption("You`re~welcome!", 255)
        .AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("If we have the claws and fangs of the HawkL93, we`ll know them better, and it will be much")
                                    .AddText("~easier to destroy them. Hurry up!")
    .AddOption("I~will.", 255)
    .AddAvatar(35).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("The HawkL93 is strong and powerful. It`s very difficult to defeat them in a group. Could you please collect some of their claws and fangs?")
                            .AddText("~I want to know what makes them so undefeatable.")
                            .AddOption("Ok.", 11)
                            .AddOption("Sorry,~but~I`m~busy.", 255)
                            .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 11:
                    {

                        client.CreateBoxDialog("Collect~the~claws~and~fangs~of~the~HawkL93.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(720992, 5, stream);
                            client.CreateBoxDialog("Both~bridges~have~been~blocked~and~the~Haw~Kings~won`t~escape!~You`ve~received~EXP~worth~1/2~EXP~Ball~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("So you`re here to warn me? Thanks! Since the bridges have been blocked, the Haw Kings will not be able to escape!")
.AddOption("You`re~welcome.", 255)
.AddAvatar(35).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierGuan)]
        public static void SoldierGuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierGuan, client.Player.Class, 1614);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It`s boring to stay here all the time... How I miss the wine from the city!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Story", 10);


                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Please talk to me! I`ve been here alone for such a long time. Do you want to hear my story?")
                          .AddOption("Tell~me.", 11)
                          .AddOption("Sorry,~but~I`m~busy.", 255)
                          .AddAvatar(237).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("Haha. Actually, I was a general, once. The reason I`m here now is because I loved drinking so much that")
                            .AddText("~I always got drunk, even on duty. My superior sent me here as a punishment.")
                         .AddOption("Don`t~drink~too~much.", 12)
                         .AddAvatar(237).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("Congratulations! You received 10 minutes of Exp and 1,000 Silver!", MsgMessage.ChatMode.System);
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(237).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierXu)]
        public static void SoldierXu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierXu, client.Player.Class, 1615);//1614);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`ve been away from home for such a long time... I miss my family so much...");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Worried~Soldier", 10);


                        data.AddOption("Homesick~soldier.", 255);
                        data.AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("These questions are driving me crazy! Could you please help me?")
                           .AddOption("Ok.", 11)
                           .AddOption("Sorry,~but~I`m~busy.", 255)
                           .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("What country is most popular on Thanksgiving?")
                         .AddOption("China.", 19)
                         .AddOption("Turkey.", 12)
                         .AddOption("Japan.", 19)
                         .AddOption("France.", 19)
                         .AddOption("No~idea.", 255)
                         .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("What tree is always very sad?")
                      .AddOption("Weeping~Willow.", 13)
                      .AddOption("Pine.", 19)
                      .AddOption("Christmas~tree.", 19)
                      .AddOption("Cypress.", 19)
                      .AddOption("No~idea.", 255)
                      .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("What letter is also a body of water?")
                    .AddOption("W.", 19)
                    .AddOption("A.", 19)
                    .AddOption("E.", 19)
                    .AddOption("C.", 14)
                    .AddOption("No~idea.", 255)
                    .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Why is a river rich?")
                  .AddOption("It~has~water.", 19)
                  .AddOption("It~has~banks.", 15)
                  .AddOption("It~has~fish.", 19)
                  .AddOption("It~has~money.", 19)
                  .AddOption("No~idea.", 255)
                  .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("What`s too much for two and just right for one?")
                .AddOption("Friend.", 19)
                .AddOption("Time.", 19)
                .AddOption("Secret.", 16)
                .AddOption("Room.", 19)
                .AddOption("No~idea.", 255)
                .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 19:
                    {
                        data.AddText("Sorry, but you`ll need to go study up a little more.")
                              .AddOption("Ok.", 255)
                              .AddAvatar(47).FinalizeDialog();
                        break;
                    }

                case 16:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("Congratulations!~You~received~10~minutes~of~Exp~and~1,000~silver!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Correct! You`re really smart!")
                          .AddOption("Haha.", 255)
                          .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierZheng)]
        public static void SoldierZheng(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierZheng, client.Player.Class, 1616);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`ve been training hard to become a soldier who can protect the island.");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Challenge", 10);


                        data.AddOption("Good~luck.", 255);
                        data.AddAvatar(55).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~10~minutes~of~Exp,~1,000~silver~and~5~Panacea!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                }
                                data.AddText("You`re really a brave hero. I lose.")
                                  .AddOption("Haha.", 255)
                                  .AddAvatar(55).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kill 10 HawKings in 3 minutes. Hurry up!")
 .AddOption("Ok.", 255)
 .AddAvatar(55).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I heard that you`re a brave hero... Let`s play a game! If you can kill 10 HawKings within 3 minutes, you`ll win!")
    .AddOption("It`s~easy.", 11)
    .AddOption("Sorry,~but~I`m~busy.", 255)
    .AddAvatar(55).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 180);
                        client.CreateBoxDialog("Please~kill~10~HawKings~within~3~minutes.~Are~you~ready?");

                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(55).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierFei)]
        public static void SoldierFei(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierFei, client.Player.Class, 1617);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Before I joined the army, I married my childhood sweetheart. Sigh, I really miss my beautiful wife...");


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1) == false)
                                data.AddOption("Deliver~food.", 1);
                        }

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("Soldier`s~Request", 10);

                        data.AddOption("Sigh...", 255);
                        data.AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {

                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(720991, 1))
                                client.Inventory.Add(stream, 720991);
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~10~minutes~of~Exp~and~1,000~silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 1000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                }
                                data.AddText("Thanks for your help! I will never let anyone damage my wife`s portrait.")
                                  .AddOption("You`re~welcome.", 255)
                                  .AddAvatar(55).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please help me bury the portrait under the tree, next to me.")
                        .AddOption("Ok.", 255)
                        .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I was sent here not long after I got married, so I miss my beautiful wife very much. I have a portrait of her,")
                            .AddText("~but I`m afraid it will be lost in battle. Could you please bury it under the tree, next to us?")
                           .AddOption("Ok.", 11)
                           .AddOption("Sorry,~but~I`m~busy.", 255)
                           .AddAvatar(56).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720991, 1))
                            client.Inventory.Add(stream, 720991);
                        client.CreateBoxDialog("Bury~the~portrait~under~the~tree~next~to~Soldier~Fei.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                        data.AddText("Great! I was really starving. Thanks a lot!")
                            .AddOption("You`re~welcome.", 255)
                            .AddAvatar(56).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.CookYuan)]
        public static void CookYuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CookYuan, client.Player.Class, 1613);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hello, hero! I`m the cook for the army on Bird Island, and I`m really busy. I`d be")
                            .AddText("~really happy if you could help me solve a problem I`m having.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711462, 1))
                            data.AddOption("Elemental~Circle.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("At~your~service.", 1);




                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("So you want my water bag to complete the elemental circle? I`m happy I can help! Here you are.")
                            .AddOption("Thanks.", 11)
                            .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~received~the~Water~Bag~from~the~Cook~(water~element)!");
                        client.Inventory.Add(stream, 711462);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Thanks for your help! I can have a little break, now.")
.AddOption("You`re~welcome.", 255)
.AddAvatar(35).FinalizeDialog();
                                }

                            }
                            else
                            {
                                data.AddText("Hurry up! Our soldiers are hungry!")
           .AddOption("Ok.", 255)
           .AddAvatar(35).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("Great! Actually, my assistant has become ill, so I need someone to help me deliver food to the 4 soldiers on the island.")
              .AddOption("I`ll~help~you.", 2)
              .AddOption("Sorry,~but~I`m~busy.", 255)
              .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Deliver~food~to~the~soldiers~in~the~4~corners~of~the~island.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.SouthwestIsland)]
        public static void SouthwestIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
            }
        }
        [NpcAttribute(NpcID.WestIsland)]
        public static void WestIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
            }
        }
        [NpcAttribute(NpcID.NorthwestIsland)]
        public static void NorthwestIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.NorthIsland)]
        public static void NorthIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.NortheastIsland)]
        public static void NortheastIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.CentralIsland)]
        public static void CentralIsland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 0, 1) == false)
            {
                client.SendSysMesage("This stone is safe. Go and check the other stoned.", MsgMessage.ChatMode.System);
                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 0, 1);
            }
        }
        [NpcAttribute(NpcID.TruthTaoist)]
        public static void TruthTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1612);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1618);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 1619);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TruthTaoist, client.Player.Class, 6130);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1627);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I was appointed to stay here and prevent the HawKings from escaping. I`d be really happy if you could lend me a hand.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1) == false)
                            data.AddOption("Elemental~Circle~(2).", 40);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("What~can~I~do?", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Blockade", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Blockade~2", 20);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Patrol~the~Islands.", 30);




                        data.AddOption("Sorry,~but~I`m~busy.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        data.AddText("Don`t worry, I know how to stop the HawKings, here. I hope the elemental circle will soon be summoned!")
                            .AddOption("We`ll~win!", 41)
                            .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("Now~you~can~report~back~to~the~Mystery~Taoist.");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest5.MissionId, 1);

                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1, 1, 1, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("You`ve checked all the stones, and I think you`re becoming more familiar with the islands.")
.AddOption("Definitely.", 255)
.AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please go to each of the islands and check on those stones!")
                .AddOption("Ok.", 255)
                .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("To prevent the monsters from landing, I placed 6 Guarding Stones on the islands.")
                            .AddText("~Could you please go to each island and see if these stones are safe?")
                 .AddOption("At~your~service.", 31)
                 .AddOption("Sorry,~but~I`m~busy.", 255)
                 .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(720992, 1))
                                client.Inventory.Add(stream, 720992);

                            data.AddText("Go to the two portals and use the pill to stop the HawKings. Hurry up!")
                   .AddOption("Ok.", 255)
                   .AddAvatar(35).FinalizeDialog();



                            break;
                        }
                        data.AddText("I`ve made the special pill! Now, I need you to burn it on the southern and northern bridges.")
                            .AddText("~And then, the bridges will be blocked and the Haw Kings won`t be able to escape!")
                            .AddText("~After that, you`re going to inform the Mystery Taoist near the northern bridge and ask him to be on alert.")
                          .AddOption("Alright.", 21)
                          .AddOption("Sorry,~but~I`m~busy.", 255)
                          .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.Contain(720992, 1))
                            client.Inventory.Add(stream, 720992);
                        client.CreateBoxDialog("Go~burn~the~pill~on~the~southern~and~northern~bridges!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711454, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711454, 5, stream);
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.CreateBoxDialog("~You~received~30~minutes~of~Exp,~3,000~silver~and~1~EXP~Ball~Scrap!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Such wonderful feathers! Now I can start to make the special pill. Thanks for your help!")
.AddOption("You`re~welcome.", 255)
.AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Go collect 5 feathers from the Haw Kings before they run out of the island! Hurry up!")
                        .AddOption("Got~it.", 255)
                        .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We`ve built some wooden bridges for heroes like you, as easy accesses to the islands. However, Those Haw Kings")
                           .AddText("~also use the bridges and get out to make troubles! I need you to stop them!")
                         .AddOption("At~your~service.", 11)
                         .AddOption("Sorry,~but~I`m~busy.", 255)
                         .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I need you to kill Haw Kings and collect 5 of their feathers. I`ll use them to make a special pill.")
                        .AddText("~And then, you`re going to burn the pill on both bridges, to prevent them from escaping.")
                      .AddOption("I~see.", 12)
                      .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Collect~5~feathers~from~the~HawKings.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {

                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(720990, 1))
                                client.Inventory.Add(stream, 720990);

                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.Inventory.Remove(720990, 5, stream);
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~5~Panacea!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Well done! The HawKings can never leave the magic circle, though will be destroyed sooner or later!")
.AddOption("Good~luck.", 255)
.AddAvatar(35).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please use the Sealing Tokens at the 4 corners of Bird Island to reinforce the magic circle.")
                               .AddOption("Ok.", 255)
                               .AddAvatar(35).FinalizeDialog();

                            }
                            break;

                        }
                        data.AddText("I`ve been using a magic circle to seal the HawKings all this time, but the monsters have destroyed many of the")
                            .AddText("~Sealing Tokens. I hope you can use these new tokens to reinforce the magic circle. They must be placed in the 4 corners of Bird Island.")
                          .AddOption("Got~it..", 2)
                          .AddOption("Sorry,~but~I`m~busy.", 255)
                          .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255).AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720990);
                        client.CreateBoxDialog("Place~the~Sealing~Tokens~at~the~4~corners~of~Bird~Island~to~reinforce~the~magic~circle.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
    }
}

