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
        [NpcAttribute(NpcID.ClanNpc)]
        public static void ClanNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.MyClan == null)
                        {

                            data.AddText("The Clan is not only a group, but also a bond between its members! You can have a maximum of 12 members in a clan at one time,")
                           .AddText("~this means 6 members and their Spouses. After you create and upgrade the Clan, you can gain the BP Bonus Level, which can be")
                           .AddText("~shared by Clan members.")
                               .AddOption("How~to~create~a~Clan.", 1)
                               .AddOption("I~have~no~interest.", 255)
                               .AddAvatar(39).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The Clan is not only a group, but also a bond between its members! You can have a maximum of 12 members in a clan at one time,")
                           .AddText("~this means 6 members and their Spouses. After you create and upgrade the Clan, you can gain the BP Bonus Level, which can be")
                           .AddText("~shared by Clan members.")
                            .AddOption("Disband~My~Clan.", 10)
                            .AddOption("Leave.", 255)
                            .AddAvatar(39).FinalizeDialog();
                        }


                        break;
                    }
                case 30:
                    {
                        data.AddText("Only~the~Clan~Leader~can~change~the~Guild`s~name,~which~will~take~effect~after~the~server~maintenance~on~the~second~day.~It`ll~cost~you~1075~CPs.~Deal?");
                        data.AddOption("Deal!", 31);
                        data.AddOption("Wait~a~minute.");
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Please~Enter~a~New~Name~For~Your~Clan.");
                        data.AddInput("Enter Name:", 32);
                        data.AddOption("Cancel.");
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Player.MyClan != null)
                        {
                            if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                            {
                                if (client.Player.ConquerPoints < 1075)
                                {
                                    data.AddText("Sorry,~you~don`t~have~enough~CPs.");
                                    data.AddOption("My~bad.");
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.InTrade) return;
                                    string clanname = Input;
                                    if (Role.Instance.Clan.AllowCreateClan(clanname))
                                    {
                                        client.Player.ConquerPoints -= 1075;
                                        Role.Instance.Clan.RegisterChangeName(client.Player.MyClan.ID, clanname);
                                        data.AddText("Done!~You~can~check~the~name~after~the~server~maintenance~tomorrow.");
                                        data.AddOption("Great!");
                                        data.AddAvatar(39);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.CreateBoxDialog("Failed~to~change~name~Clan.~The~name~has~been~used~or~you~use~invalid~characters.");
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Sorry,~you`re~not~the~Clan~Leader.");
                                data.AddOption("My~bad.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.MyClan != null)
                        {
                            if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                            {
                                data.AddText("Only the Clan Leader has the right to disband the Clan. Do you really want to disband your Clan")
                                    .AddOption("Yes.", 11)
                       .AddOption("Of~course~not.", 255)
                       .AddAvatar(39).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You aren`t the Clan Leader. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the BP Bonus Level.");
                                data.AddOption("I~see.").AddAvatar(39).FinalizeDialog();
                            }


                        }


                        break;
                    }
                case 11:
                    {
                        if (client.Player.ClanRank != (ushort)Role.Instance.Clan.Ranks.Leader)
                        {
                            data.AddText("You aren`t the Clan Leader. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the BP Bonus Level.");
                            data.AddOption("I~see.").AddAvatar(39).FinalizeDialog();
                            break;
                        }
                        if (client.Player.MyClan != null)
                        {
                            client.Player.MyClan.DisbandClan(stream);
                            client.CreateBoxDialog("Your~Clan~has~been~disbanded.");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            if (Role.Instance.Clan.UpdateBPAmount.Length > client.Player.MyClan.BP)
                            {
                                data.AddText("You have reached BP Bonus Level " + client.Player.MyClan.BP + ", and the Clan members can share percent of the Battle Power margin in the same map. Do you");
                                data.AddText("~want to upgrade the BP Bonus Level using " + Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP] + " in Clan Fund?");
                                data.AddOption("Yes.", 8);
                                data.AddOption("Not~right~now.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                data.AddOption("Sounds~good!");
                                data.AddAvatar(39);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t create or upgrade the BP Bonus Level. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the");
                            data.AddText("~BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 8:
                    {
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            if (client.Player.MyClan == null)
                                break;
                            uint Money = Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP];
                            if (client.Player.MyClan.Donation > Money)
                            {
                                client.Player.MyClan.Donation -= Money;
                                client.Player.MyClan.BP++;
                                if (Role.Instance.Clan.UpdateBPAmount.Length > client.Player.MyClan.BP)
                                {
                                    data.AddText("You have reached BP Bonus Level " + client.Player.MyClan.BP + ", and the Clan members can share percent of the Battle Power margin in the same map. Do you");
                                    data.AddText("~want to upgrade the BP Bonus Level using " + Role.Instance.Clan.UpdateBPAmount[client.Player.MyClan.BP] + " in Clan Fund?");
                                    data.AddOption("Yes.", 8);
                                    data.AddOption("Not~right~now.");
                                    data.FinalizeDialog();

                                }
                                else
                                {
                                    data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                    data.AddOption("Sounds~good!");
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Oh, you don`t have " + Money + " in Clan Fund to gain the BP Bonus Level... Maybe I will help you next time when you have the");
                                data.AddText("~money.");
                                data.AddOption("Okay.~See~you.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t create or upgrade the BP Bonus Level. Only the Clan Leader can upgrade or disband the Clan and create or upgrade the");
                            data.AddText("~BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            if (Role.Instance.Clan.UpdateLevelAmount.Length > client.Player.MyClan.Level)
                            {
                                data.AddText("Your current Clan Level is Level " + (byte)(client.Player.MyClan.Level + 1) + " . Do you want to spend " + Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level] + " in Clan Fund to upgrade the Clan?")
                                   .AddOption("Yes.", 6)
                                   .AddOption("Not~right~now.", 255)
                                   .AddAvatar(39).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                data.AddOption("Sounds~good!");
                                data.AddAvatar(39);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t upgrade the Clan. Only the Clan Leader can upgrade or disband the Clan, and create or upgrade the BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader)
                        {
                            uint Money = Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level];
                            if (client.Player.MyClan.Donation > Money)
                            {
                                client.Player.MyClan.Donation -= Money;
                                client.Player.MyClan.Level++;
                                if (Role.Instance.Clan.UpdateLevelAmount.Length > client.Player.MyClan.Level)
                                {
                                    data.AddText("Your current Clan Level is Level " + (byte)(client.Player.MyClan.Level + 1) + " . Do you want to spend " + Role.Instance.Clan.UpdateLevelAmount[client.Player.MyClan.Level] + " in Clan Fund to upgrade the Clan?")
                                       .AddOption("Yes.", 6)
                                       .AddOption("Not~right~now.", 255)
                                       .AddAvatar(6).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You have reahced the highest BP Bonus Level and the members can share 70 percent of the Battle Power margin in the same map.");
                                    data.AddOption("Sounds~good!");
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Your do not have " + Money + " silvers in you Clan.")
                                     .AddOption("Okay.", 255)
                                     .AddAvatar(39).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You can`t upgrade the Clan. Only the Clan Leader can upgrade or disband the Clan, and create or upgrade the BP Bonus Level.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("After you reach Level 50, you can create a Clan with 500,000 silver, half of which will be the Clan Fund. You can recruit your")
                      .AddText("~members and build your elite group! Of course, you can only be in one Clan at a time. What do you think     Are you ready to")
                      .AddText("~start your new group")
                          .AddOption("Create~a~Clan~now.", 2)
                          .AddOption("I`m~busy.", 255)
                          .AddAvatar(39).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.MyClan == null)
                        {
                            if (client.Player.Money >= 500000)
                            {
                                if (client.Player.Level >= 50)
                                {
                                    data.AddText("To create a Clan, you need to think of a fabulous name for it! Type in the name you like, but remember the name should be");
                                    data.AddText("~less than 35 characters. A name is very important to your reputation. You should make it good enough so that people will");
                                    data.AddText("~tremble upon the very mention of it! So what`ll it be?");
                                    data.AddInput("My~Clan~Name...", 3).AddOption("Let~me~think...").AddAvatar(39).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You haven`t reached Level 50 and are unable to create a Clan. You should be strong enough to lead your Clan!");
                                    data.AddOption("Got~it.");
                                    data.AddAvatar(39).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, You don`t have enough money to create a Clan. Creating a Clan needs 500,000 Silver.")
                             .AddOption("Okay.", 255)
                             .AddAvatar(39).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You already created a Clan and are unable to create another one!");
                            data.AddOption("Okay.");
                            data.AddAvatar(39);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.Money >= 500000)
                        {
                            if (client.Player.MyClan == null)
                            {
                                string clanname = Input;
                                if (Role.Instance.Clan.AllowCreateClan(clanname))
                                {
                                    data.AddText("Congratulations! You have created a Clan successfully! You should protect your Clan and fight with your Clan members for glory");
                                    data.AddText("~and honor!");
                                    data.AddOption("thanks~I~will~that.", 20);
                                    data.AddAvatar(39);
                                    data.FinalizeDialog();

                                    client.Player.Money -= 500000;
                                    new Role.Instance.Clan().Create(client, clanname, stream);
                                }
                                else
                                {
                                    client.CreateBoxDialog("Failed~to~create~a~Clan.~The~name~has~been~used~or~you~haven`t~quit~a~Clan~for~at~least~a~day.");
                                }
                            }
                            else
                            {
                                data.AddText("You already created a Clan and are unable to create another one!");
                                data.AddOption("Okay.");
                                data.AddAvatar(39);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, You don`t have enough money to create a Clan. Creating a Clan needs 500,000 Silver.")
                         .AddOption("Okay.", 255)
                         .AddAvatar(39).FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        data.AddText("Now you can sign up at the Clan Guide in the name of your Clan to join in the Clan War once a day. You will obtain a Normal");
                        data.AddText("~Clan Pack as a reward if you win!");
                        data.AddOption("Nice!", 21);
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        data.AddText("If you can occupy the map continuously, you will get better prizes! Besides the reward you get after you win the Clan War,");
                        data.AddText("~you will get a Refined Clan Pack on the 7th day, Unique Clan Pack on the 14th day, Elite Clan Pack on the 21st day, and a");
                        data.AddText("~Super Clan Pack on the 28th day! The stronger you are, the more you can win!");
                        data.AddOption("I`ll~try~my~best!");
                        data.AddAvatar(39);
                        data.FinalizeDialog();
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.GuildCreator)]
        public static void GuildCreator(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        data.AddText("Through Me You Can Delete Your Guild If You Want For Free. AnD Creat New Union");
                        //.AddText("Guild leaders can post up a guild recruitment ad to recruit new blood for their guilds! Soldiers without a guild can get the latest guild recruitment information from me, and then find a good guild to join!")
                        data.AddOption("Create New Union.", 10);
                        // .AddOption("Create~a~guild", 1)
                        data.AddOption("Disband~My~Guild.", 199);
                       // data.AddOption("Change~My~Guild~Name.", 30);
                        data.AddOption("Leave.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 30:
                    {                        
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        data.AddText("Only~the~Guild~Leader~can~change~the~Guild`s~name,~which~will~take~effect~after~the~server~maintenance~on~the~second~day.~It`ll~cost~you~1075~CPs.~Deal?");
                        data.AddOption("Deal!", 31);
                        data.AddOption("Wait~a~minute.");
                        data.AddAvatar(7);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {                        
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        data.AddText("Please~enter~a~new~name~for~your~Guild.");
                        data.AddInput("Name:", 32);
                        data.AddOption("Cancel.");
                        data.AddAvatar(7);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.Player.MyGuild != null)
                        {
                            if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                            {
                                if (client.Player.ConquerPoints < 1075)
                                {
                                    data.AddText("Sorry,~you~don`t~have~enough~CPs.");
                                    data.AddOption("My~bad.");
                                    data.AddAvatar(7);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    string GuildName = Input;
                                    if (Role.Instance.Guild.AllowCreateGuild(GuildName))
                                    {
                                        client.Player.ConquerPoints -= 1075;
                                        client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgServer.MsgUpdate.DataType.ConquerPoints);
                                        Role.Instance.Guild.RegisterChangeName(client.Player.GuildID, GuildName);
                                        client.Socket.Disconnect();
                                        data.AddText("Done.");
                                        data.AddOption("Great!");
                                        data.AddAvatar(7);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.CreateBoxDialog("Failed~to~change~name~Guild.~The~name~has~been~used~or~you~use~invalid~characters.");
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Sorry,~you`re~not~the~Guild~Leader.");
                                data.AddOption("My~bad.").AddAvatar(7).FinalizeDialog();                                
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        data.AddText("A great war wich involves millions of heroes is about the fire. If you have a Level 9+")
                            .AddText(" Guild ranking top in Capture the Flag of your server, you can pay me 3,000 Guild CPs to")
                            .AddText(" create a Union. It`s to gather the elites and picture a new world in your hands.");

                        if (client.Player.MyUnion == null)
                            data.AddOption("Create Union.", 11);
                        else
                        {

                            if (client.Player.InUnion)
                                data.AddOption("Set Kingdom Title.", 13);
                            data.AddOption("Change Union`s Name.", 12);
                        }
                        data.AddOption("It`s time to strike!", 255)
                        .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        ActionQuery query = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.SetKingdomTitle,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(query));
                        break;
                    }
                case 12:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.Player.UnionMemeber != null && client.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                        {

                            ActionQuery query = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.ChangeNameUnion,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y
                            };
                            client.Send(stream.ActionCreate(query));
                        }
                        else
                            client.SendSysMesage("Failed to change the Union`s name. You're not the Union Leader.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                        break;
                    }
                case 11:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.Player.MyGuild != null && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                        {

                            var Mohy666 = new ActionQuery();
                            Mohy666.ObjId = client.Player.UID;
                            Mohy666.dwParam = 0;
                            Mohy666.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                            Mohy666.Type = 258;
                            Mohy666.Fascing = 0;
                            Mohy666.PositionX = 0;
                            Mohy666.PositionY = 0;
                            Mohy666.ScheduleTime = 0;
                            Mohy666.RemainTime = 0;
                            Mohy666.Strings = new string[] { "LuaUIMgr_OpenWindow|693" };
                            client.Send(stream.ActionCreate(Mohy666));
                        }
                        else
                        {
                            data.AddText("I'm sorry but you don't meet the requirements to create a Union only guild leaders are able to do that.")
                            .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Write the name, you wana use for guild")
                     .AddInput("Name", 3).AddAvatar(7).FinalizeDialog();


                        break;
                    }
                case 199:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.Player.GuildRank != Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            data.AddText("Sorry, You Do Not Have a Guild in Your Name To Be Able To Delete it!")
                           .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.InUnion)
                        {
                            data.AddText("Please Quit Union First Time.")
                          .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                            return;
                        }
                        if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
                        {
                        }
                        else
                        {
                            data.AddText("Sorry, You Do Not Have a Guild in Your Name To Be Able To Delete it!")
                           .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }
                        data.AddText("Are You Sure You Want To Delete Your Guild Permanently!")
                     .AddOption("Yes.", 2)
                     .AddOption("No.", 255).AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.Player.GuildRank != Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            data.AddText("Sorry, You Do Not Have a Guild in Your Name To Be Able To Delete it!")
                           .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.InUnion)
                        {
                            data.AddText("Please Quit Union First Time.")
                          .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                            return;
                        }
                        if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
                        {
                            client.Player.MyGuild.Dismis(client, stream);
                        }
                        else
                        {
                            data.AddText("Sorry, You Do Not Have a Guild in Your Name To Be Able To Delete it!")
                           .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        string GuildName = Input;
                        //  if (Input < 8)
                        if (Role.Instance.Guild.AllowToCreate(GuildName))
                        {
                            if (client.Player.Level >= 90)
                            {
                                if (client.Player.Money >= 10000000)
                                {
                                    if (client.Player.InUnion)
                                    {
                                        data.AddText("Please quit union first time.")
                                        .AddOption("Okay.", 255).AddAvatar(7).FinalizeDialog();
                                        return;
                                    }
                                    if (client.Player.MyGuild == null)
                                    {

                                        client.Player.Money -= 10000000;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                        new Role.Instance.Guild(client, GuildName, stream);


                                    }
                                    else
                                    {
                                        data.AddText("Sorry, you can't create another guild while you have one!")
                     .AddOption("Okay thanks.", 255).AddAvatar(7).FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You do not have 10,000,000 silvers with you.")
                      .AddOption("Ah, Ok", 255).AddAvatar(7).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Sorry! you need level 90 to use my options")
                        .AddOption("Ah, Ok", 255).AddAvatar(7).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Soory,the name is used!")
                          .AddOption("Ah, Ok", 255).AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LoveStone)]
        public static void LoveStone(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Fate can bring lovers together. I hope you all can get married and live happily with your lovers. What can I do for you?")
                            .AddOption("Claim~double~EXP.", 1)
                            .AddOption("I~want~to~get~married.", 2)
                            .AddOption("Can~you~help~me~divorce?", 3)
                            .AddOption("Nothing~special.", 4)
                            .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.HeavenBlessing > 0)
                        {
                            if (client.Player.BDExp == 0)
                            {
                                client.Player.BDExp = 1;

                                client.Player.RateExp = 2;
                                client.Player.DExpTime = 3600;
                                client.Player.CreateExtraExpPacket(stream);
                                client.SendSysMesage("You got double exp. time of one hour.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            }
                            else
                            {
                                data.AddText("Sorry , but you're claimed today double Xp!").AddOption("Ah yes.", 255)
                        .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry , you not have Blessing!").AddOption("Ah yes.", 255)
                       .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 3:
                    {
                        data.AddText("if you marriage left you nothing but regret and pain, then bring 3 Meteors,i can help you out.")
                         .AddOption("I have made up my ming", 8).AddOption("Let me reconsider it", 255)
                         .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(Database.ItemType.Meteor, 3))
                        {
                            if (client.Player.Spouse != "None")
                            {
                                client.Inventory.Remove(Database.ItemType.Meteor, 3, stream);
                                Client.GameClient Spouse;
                                if (Pool.GamePoll.TryGetValue(client.Player.SpouseUID, out Spouse))
                                {
                                    client.Player.Spouse = "None";
                                    client.Player.SpouseUID = 0;

                                    Spouse.Player.SpouseUID = 0;
                                    Spouse.Player.Spouse = "None";

                                    client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
                                    Spouse.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
                                    return;
                                }
                                else
                                {
                                    client.ClientFlag |= Client.ServerFlag.RemoveSpouse;
                                    Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                                }
                            }
                            else
                            {
                                data.AddText("You don`t have a wife").AddOption("Ah, you right", 255)
                             .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have 3 Meteors").AddOption("Ah, you right", 255)
                         .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }

                case 2:
                    {
                        data.AddText("Fate can bring lovers together. And everybody can live happily ever after... Ahhhh... What can I do for you?")
                           .AddOption("I~want~to~get~married.", 5)
                           .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Marriage~is~the~promise~forever.~Are~you~sure~that~you~want~to~spend~the~rest~of~your~life~with~your~lover?")
                          .AddOption("I~want~to~get~married.", 6)
                           .AddOption("I~prefer~being~single.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        data.AddText("Are~you~ready~to~propose?")
                         .AddOption("Yeah.~I~am~ready..", 7)
                          .AddOption("Let~me~think~it~over.", 255)
                         .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        data.AddText("Please click on your lover to propose.")
                          .AddOption("Okay.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        ActionQuery action = new ActionQuery()
                        {
                            dwParam = MsgServer.CustomCommands.FlowerPointer,
                            Type = ActionType.OpenCustom,
                            ObjId = client.Player.UID
                        };
                        client.Send(stream.ActionCreate(action));


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TwinCityConductress)]
        public static void TwinCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Where are you heading for~~~~~I can teleport you")
                        .AddOption("Texas. ", 6)
                        .AddOption("Phoenix Castle", 1)
                        .AddOption("Desert City ", 3)
                        .AddOption("Ape Mountain ", 2)
                            //  .AddOption("Desert City. ", 3)
                        .AddOption("Bird Island. ", 4)
                        .AddOption("Mine Cave ", 5)
                            //     .AddOption("Market.", 6)
                        .AddOption("DragonIsland ", 7)
                       .AddOption("Just~passing~by. ", 255).AddAvatar(1).FinalizeDialog();


                        break;
                    }
                default:
                    {
                        if (client.Player.PKPoints < 29)
                        {
                            if (client.Player.Money >= 0)
                            {
                                client.Player.Money -= 0;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                switch (Option)
                                {
                                    case 1: client.Teleport(680, 320, 1002); break; //phoenix castle
                                    case 2: client.Teleport(348, 841, 1002); break; //ape mountain
                                    case 3: client.Teleport(33, 528, 1002); break; //desert city
                                    case 4: client.Teleport(559, 576, 1002); break; //birdisland
                                    case 5: client.Teleport(107, 449, 1002); break; // Mine Cave
                                    case 6: client.Teleport(343, 434, 1002); break; //market
                                    case 7: client.Teleport(580, 586, 1002); break; //Dragon Island

                                }
                            }
                            else
                            {
                                data.AddText("Hey! You don't have 100 gold do you?\nDon't you dare try to cheat me or I'll call the guards!")
                             .AddOption("Crazy!", 255)
                             .AddAvatar(1)
                             .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I will not help MURDERS!")
                           .AddOption("Okay...", 255)
                            .AddAvatar(1)
                           .FinalizeDialog();

                        }



                        break;
                    }
            }
        }      

        [NpcAttribute(NpcID.BlackSmith)]
        public static void BlackSmith(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WHTwin, client.Player.Class);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class, 823);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("Ahh, Warehouseman asked you here, huh? Yeah, you are in the right place! You know, weapons are the most important things in battle. Even the toughest warriors can`t survive without a weapon in their hands!")
                            .AddOption("Teach~me~more~about~weapons.", 2)
                            .AddOption("Buy~weapons.", 1)
                            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("What a pity. I cannot own a good weapon! But that doesn`t mean you have to be the same! Please take this weapon as a gift. Well")
                   .AddOption("Okay!", 4).AddOption("Buy~weapons.", 1)
                   .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("What a pity. I cannot own a good weapon! But that doesn`t mean you have to be the same! Please take this weapon as a gift. Well")
                .AddOption("Okay!", 4).AddOption("Buy~weapons.", 1)
                .AddAvatar(10).FinalizeDialog();
                            break;
                        }


                        goto case 1;
                    }
                case 2:
                    {
                        data.AddText("Most weapons and equipments are upgrade, you can buy them here.")
            .AddOption("Okay", 3)
            .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.GainExpBall(500);

                                if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                    client.Inventory.Add(stream, 410016, 1);//leafBlade
                                else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                    client.Inventory.Add(stream, 410016, 1);//leafBlade
                                else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                    client.Inventory.Add(stream, 500006, 1);//BambooBow
                                else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                    client.Inventory.Add(stream, 601016, 1);//iron katana
                                else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                    client.Inventory.Add(stream, 610016, 1);//BambooPrayerBeads
                                else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                    client.Inventory.Add(stream, 611016, 1);//recruitRapier
                                else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                    client.Inventory.Add(stream, 421016, 1);//SpellBacksword 
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                    client.Inventory.Add(stream, 626016, 1);

                                data.AddText("I`m selling the Headgear and armors. You should not miss the equipment in my shop! Hey, you`re still here! What else can I do for you? ")
                               .AddOption("I~want~to~learn~more~from~you.", 5)
                               .AddOption("Buy~weapons.", 1)
                               .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make space in you inventory")
                      .AddOption("Ah~thank~you!.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("What a pity. I cannot own a good weapon! But that doesn`t mean you have to be the same! Please take this weapon as a gift. Well")
                .AddOption("Okay!", 4)
                .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("TBlacksmith~asked~you~to~speak~with~the~TC~Captain~to~see~if~there~is~anything~you~can~do.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 1:
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

        [NpcAttribute(NpcID.Pharmacist)]
        public static void Pharmacist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TheStorekeeper, client.Player.Class);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class, 815);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (ActiveQuest3 != null && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;

                        }
                        if (ActiveQuest != null && client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("We have all kinds of potions to restore your HP and MP, what do you need? ")
                         .AddOption("Tell~me~about~your~potions.", 2)
                         .AddOption("Buy~potions.", 1)
                         .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (ActiveQuest2 != null && client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("We have all kinds of potions to restore your HP and MP, what do you need? ")
                          .AddOption("Tell~me~about~your~potions.", 2)
                          .AddOption("Buy~potions.", 1)
                          .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (ActiveQuest2 != null && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (ActiveQuest != null && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("What? Hey, don`t look at me in that way. What do you want? Tell me! Don`t just stand there.")
                .AddOption("I~want~to~learn~more~from~you.", 5)
                  .AddOption("Buy~potions.", 1)
                .AddAvatar(10).FinalizeDialog();
                            break;
                        }


                        goto case 1;
                    }
                case 7:
                    {
                        data.AddText("The Barber! She is a talented hairdresser. She is worth paying a visit to! Go! ~Now, you should pay a visit to the Barber. I`m sure you will get something from her. ")
             .AddOption("Okay!", 255)
             .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("Oops, I have told you all I know. You should pay a visit to the Barber. I`m sure you will get something from her.")
               .AddOption("Okay!", 6)
                 .AddOption("See~you~later.", 255)
               .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(Database.QuestInfo.GetQuest(815), 0);// [QuestFix 2026-10-13] safe lookup
                        client.Player.QuestGUI.SendAutoPatcher("The~Pharmacist~asked~you~to~speak~with~the~Barber~in~Twin~City.", 1002, 284, 276, (uint)NpcID.BarberSha);

                        // client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        // client.Player.QuestGUI.SendAutoPatcher("The~Pharmacist~asked~you~to~speak~with~the~Barber~in~Twin~City.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 2:
                    {
                        data.AddText("Besides MP and HP potions, I also have a special item called Scroll, which will take you to whatever city it is made for! ~You are lucky! I have just got something new for you!")
               .AddOption("Thanks~a~lot!", 3)
               .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 300, Role.Flags.ExperienceEffect.angelwing);
                                client.Inventory.AddItemWitchStack(1000000, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(1001000, 0, 3, stream);
                                client.Inventory.AddItemWitchStack(1060020, 0, 2, stream);
                                client.Inventory.Add(stream, 721626, 1);
                                client.Inventory.Add(stream, 721625, 1);
                                client.SendSysMesage("You received 2 Twin City Gates, 3 Stanchers, 3 Agrypnotics, 1 Attack Pot(30 min) 1 DefencePot (30 min) and some experience", MsgMessage.ChatMode.System);
                                data.AddText("What? Hey, don`t look at me in that way. What do you want? Tell me! Don`t just stand there.")
              .AddOption("I~want~to~learn~more~from~you.", 5)
                .AddOption("Buy~potions.", 1)
              .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 5 more spaces in your inventory")
                      .AddOption("let me to check.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
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
        [NpcAttribute(NpcID.TheStorekeeper)]
        public static void TheStorekeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Armorer, client.Player.Class);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("I am here selling all kinds of Accessories and Boots. You should not miss the equipment in my shop. ")
                             .AddOption("Tell~me~about~your~equipment.", 2)
                             .AddOption("Buy~equipment.", 1)
                             .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (ActiveQuest2 != null && client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("Ah ha? Why don`t you just leave here? Let me guess...You want to buy something? ")
                          .AddOption("Nah.~What~should~I~do~next?", 8)
                          .AddOption("Buy~equipment.", 1)
                          .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (ActiveQuest2 != null && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Oh, by the way, I am also selling Pickax for mining! Come to me if you need it!")
                .AddOption(" I~will!", 5)
                  .AddOption("Buy~equipment.", 1)
                .AddAvatar(10).FinalizeDialog();
                            break;
                        }


                        goto case 1;
                    }
                case 8:
                    {
                        data.AddText("Have you spoken with the Pharmacist? She has all kinds of potions that will be helpful to you in your journey.")
           .AddOption("Okay!", 255)
           .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 1:
                    {

                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = (uint)Role.Flags.NpcType.DialogAndGui,
                            Type = ActionType.OpenGuiNpc,
                            NpcID = client.ActiveNpc, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,
                            MapID = (ushort)client.Player.Map
                        };
                        client.Send(stream.ActionCreate(action));

                        break;
                    }
                case 2:
                    {
                        data.AddText("I have boots, necklaces and rings, which are quite necessary to improve your skills and prowess ")
                        .AddOption("I~learnt~something~new!", 3)
                        .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You are lucky, kid! I have just got something new for you!")
                      .AddOption("Thanks~a~lot!", 4)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(3))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 1000, Role.Flags.ExperienceEffect.angelwing);

                                foreach (var item in ActiveQuest.Prize)
                                    client.Inventory.Add(stream, item);

                                data.AddText("Oh, by the way, I am also selling Pickax for mining! Come to me if you need it!")
                    .AddOption(" I~will!", 5)
                    .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make 3 more spaces in your inventory")
                      .AddOption("Ah~thank~you!.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }



                        break;
                    }
                case 5:
                    {
                        data.AddText("I have told you all I know. I suggest you to find the Pharmacist. She should have something for you there.")
                 .AddOption("Thanks!", 6)
                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Storekeeper~asked~you~to~speak~with~the~Pharmacist~in~Twin~City.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Armorer)]
        public static void Armorer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            // var next_quest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KungfuMaster, client.Player.Class);
            if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)2001, client.Player.Class, 3750);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
            var AtiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
            if (client.Player.QuestGUI.IsActiveQuest(AtiveQuest3.MissionId))
                ActiveQuest = AtiveQuest3;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        {
                            data.AddText("I`m selling Headgears and Armors. They are so wonderful. You should not miss them. ")
                            .AddOption("Tell~me~about~your~equipment.", 2)
                            .AddOption("Buy~equipment.", 1)
                            .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest2.MissionId))
                        {
                            data.AddText("Why are you still here? Pay a visit to that lady!")
                           .AddOption("Who?~Sorry,~I~forgot~the~name..", 10)
                           .AddOption("Buy~equipment.", 1)
                           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            goto case 1;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m selling the Headgear and armors. You should not miss the equipment in my shop! Hey, you`re still here! What else can I do for you? ")
                             .AddOption("I~want~to~learn~more~from~you.", 5)
                             .AddOption("Buy~equipment.", 1)
                             .AddAvatar(10).FinalizeDialog();
                            break;
                        }

                        goto case 1;
                    }
                case 10:
                    {
                        data.AddText("The Storekeeper. She has many useful items. She is worth paying a visit to!")
                       .AddOption("I~won`t~forget~this~time.", 255)
                       .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        data.AddText("I have Armors and Headgears of all classes! Different classes use different kinds of equipment. Right click the equipment in my shop to buy it! ")
                           .AddOption("Thank~you~for~the~knowledge.", 3)
                           .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("You are lucky, kid! I have just got something new for you!")
                        .AddOption("Thank~you!.", 4)
                        .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.IncreaseExperience(stream, 1000, Role.Flags.ExperienceEffect.angelwing, true);
                                foreach (var item in ActiveQuest.Prize)
                                    client.Inventory.Add(stream, item);

                                data.AddText("I`m selling the Headgear and armors. You should not miss the equipment in my shop! Hey, you`re still here! What else can I do for you? ")
                               .AddOption("I~want~to~learn~more~from~you.", 5)
                               .AddOption("Buy~equipment.", 1)
                               .AddAvatar(10).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Please make space in you inventory")
                      .AddOption("Ah~thank~you!.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        data.AddText("You should know only one piece of armor is not enough to survive. Now pay a visit to the Storekeeper, will you? ")
                         .AddOption("Thanks~for~your~advice.", 6)
                         .AddOption("See~you~later!", 255)
                         .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        data.AddText("You should know only one piece of armor is not enough to survive. Now pay a visit to the Storekeeper, will you? ")
                            .AddOption("Thanks~for~your~advice.", 7)
                            .AddOption("See~you~later!", 255)
                            .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 7://812
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
                        client.Player.QuestGUI.Accept(next_quest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~Armorer~asked~you~to~speak~with~the~Storekeeper~in~Twin~City.", next_quest.FinishNpcId.Map, next_quest.FinishNpcId.X, next_quest.FinishNpcId.Y, next_quest.FinishNpcId.ID);



                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = 32,
                            Type = ActionType.OpenGuiNpc,
                            NpcID = client.ActiveNpc, // [ProtoFix 2026-10-14] ActionQuery.NpcID is uint now
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,
                            MapID = (ushort)client.Player.Map
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KungFuKing)]
        public static void KungFuKing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
                        if (client.Player.QuestGUI.IsActiveQuest(6435))
                        {
                            data.AddText("I dare not say I am the best martial artist in the world, but I will never admit I rank the second. ")
                                .AddText("~Do you wonder where my power comes? You`ll know it after you try it.")
                                .AddOption("I~want~to~try!", 1)
                                .AddOption("I~doubt~it.", 255)
                                .AddAvatar(259).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(next_quest.MissionId, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(6435, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("As the descendant of Dragon, I recommend nunchaku as your best weapon. When weapon is ready, it`s time to fight. ")
                           .AddOption("I~can`t~wait~to~fight! ", 4)
                           .AddOption("I`m~not~ready~yet.", 255)
                           .AddAvatar(259).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I dare not say I am the best martial artist in the world, but I will never admit I rank the second.")
                                .AddOption("I~doubt~it.", 255)
                                .AddAvatar(259).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("My hero, you`re chosen by God of Dragon as the new generation for the mythical power.")
                            .AddText(" ~In the name of Dragon, I`ll grant you the spirit of fight,")
                            .AddText(" ~as well as the duty to sweep evil and darkness off the land. Are you ready?")
                                .AddOption(" I`m~always~ready!", 2)
                                .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(6435))
                            client.IncreaseExperience(stream, 100, Role.Flags.ExperienceEffect.angelwing, true);
                        data.AddText("Good! Now, I`m transferring my Power of Dragon to you. Feel it, and remember that Dragon`s ")
                            .AddText(" ~descendants equip nunchaku, and fight in the way of Jeet Kune Do. The saying `Using no")
                            .AddText(" ~way as way & Having no limitation as limitation` reveals the true meaning of Jeet Kune Do.")
                            .AddText(" ~Empty your mind, and fight freely. Your talent for martial arts will shine through severe trials.")
                             .AddOption("Thanks!", 3)
                             .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("As the descendant of Dragon, I recommend nunchaku as your best weapon. When weapon is ready, it`s time to fight. ")
                         .AddOption("I~can`t~wait~to~fight! ", 4)
                         .AddOption("I`m~not~ready~yet.", 255)
                         .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("There are many things for you to learn in this world! Um, I guess you want to learn more, huh? ")
                            .AddText("Let me see. What about equipment and armors? That will be useful! Now speak with the Armor in Twin City, ")
                            .AddText("he will tell you something about the armors.")

                             .AddOption("Wonderful!", 5)
                             .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 6425);
                        client.Player.QuestGUI.Accept(next_quest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Kungfu~King~asked~you~to~speak~with~the~Armorer~in~Twin~City.", next_quest.FinishNpcId.Map, next_quest.FinishNpcId.X, next_quest.FinishNpcId.Y, next_quest.FinishNpcId.ID);


                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.Level140)]
        public static void Level140(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            data.AddText("Level 140 .")
                                 .AddOption("Yes", 1)
                                 .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        // if (client.Player.QuestGUI.IsActiveQuest(804))
                        {
                            data.AddText("Level 140 .")
                                .AddOption("Yes.", 1)
                                .AddOption("Nah", 255)
                                .AddAvatar(10).FinalizeDialog();
                        }



                        break;
                    }
                case 1:
                    {
                        data.AddText("Here Level 140 Free.")
                            // .AddText(" Now, listen closely. Press the Ctrl button , move the mouse arrow to the place you wana to jump to,")
                            // .AddText(" and left click. You should know , you can jump somewhere that you can`t just normally walk to.")
                            .AddOption("Thanks!", 3)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }

                case 3:
                    {
                        if (client.Player.Level == 140)
                        {
                            client.CreateBoxDialog("You already achieved the highest level possible.");
                            return;
                        }
                        // if (client.Player.QuestGUI.FinishQuest(804))
                        client.UpdateLevel(stream, 140);
                        // client.IncreaseExperience(stream, 300, Role.Flags.ExperienceEffect.angelwing, true);
                        data.AddText("Okay, you have a Level 140! ")
                            //.AddText("You know, that will bring you many benefits if you have a tearn. That is the next thing you must learn.")
                            //AddOption("Who should i speak with? ", 4)
 .AddOption("Thx ", 255)
 .AddAvatar(15).FinalizeDialog();

                        break;
                    }

            }
        }


        [NpcAttribute(NpcID.KungfuBoy)]
        public static void KungfuBoyNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            data.AddText("I started learning Kungfu when I was very young. Now, I have learned to master my Chi very well! Wanna have a try? .")
                                 .AddOption("Nah", 255)
                                 .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(804))
                        {
                            data.AddText("I started learning KungFu where i was very young.Now, i have learn to master my chi very well! Wanna have a try? .")
                                .AddOption("I got a message for you.", 1)
                                .AddOption("Nah", 255)
                                .AddAvatar(10).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(802, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Okay, you have a mastered an important skill, But you still have a lot to learn before venturing out! ")
                           .AddText("You know, that will bring you many benefits if you have a tearn. That is the next thing you must learn.")
                           .AddOption("Who should i speak with? ", 4)
                           .AddOption("See you then ", 255)
                           .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I started learning Kungfu when I was very young. Now, I have learned to master my Chi very well! Wanna have a try? .")
                               .AddOption("Nah", 255)
                               .AddAvatar(10).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh, a message from the Recruit Master ? I see. He wana me to teach you how to master you Chi.")
                            .AddText(" Now, listen closely. Press the Ctrl button , move the mouse arrow to the place you wana to jump to,")
                            .AddText(" and left click. You should know , you can jump somewhere that you can`t just normally walk to.")
                            .AddOption("Thanks!", 2)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Good job! That is what i want you to do! Ok! Hope this kill will help you in the future.")
                          .AddOption("Thanks!", 3)
                         .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(804))
                            client.IncreaseExperience(stream, 300, Role.Flags.ExperienceEffect.angelwing, true);
                        data.AddText("Okay, you have a mastered an important skill, But you still have a lot to learn before venturing out! ")
 .AddText("You know, that will bring you many benefits if you have a tearn. That is the next thing you must learn.")
 .AddOption("Who should i speak with? ", 4)
 .AddOption("See you then ", 255)
 .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("General Zhao in Twin City will tel you more about the team system! Go speak with him !")
                         .AddOption("Great!", 5).AddOption("Not now.", 255)
                        .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(Database.QuestInfo.GetQuest(802), 0);// [QuestFix 2026-10-13] safe lookup
                        client.Player.QuestGUI.SendAutoPatcher("Kungfu~Master~asked~you~to~speak~with~General~Zhao~in~Twin~City!", 1002, 262, 247, 4488);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PheonixCityConductress)]
        public static void PheonixCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("With only 100 Silver, I can teleport you to somewhere far away.")
                            .AddOption("Twin~City.", 1)
                               .AddOption("Market in Twin City.", 2)
                            // .AddOption("DragonIsland.", 3)
                            // .AddOption("Market in Twin City.", 4)

                            .AddOption("Just~passing~by.", 255).AddAvatar(1).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {
                                case 1: client.Teleport(410, 354, 1002); break;
                                case 2: client.Teleport(216, 215, 1036); break;
                                case 3: client.Teleport(410, 354, 1002); break;
                                // case 4: client.Teleport(410, 354, 1002); break;

                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(1).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DesertCityConductress)]
        public static void DesertCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("With only 100 Silver, I can teleport you to somewhere far away. Twin City is free.")
                            .AddOption("Twin~City.", 1)
                               .AddOption("Market in Twin City.", 2)
                               .AddOption("Dungeon.", 3)
                            // .AddOption("DragonIsland.", 4)

                            .AddOption("Just~passing~by.", 255).AddAvatar(1).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {
                                case 1: client.Teleport(410, 354, 1002); break;
                                case 2: client.Teleport(216, 215, 1036); break;
                                case 3: client.Teleport(85, 323, 1000); break;
                                case 4: client.Teleport(410, 354, 1002); break;

                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(1).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ApeCityConductress)]
        public static void ApeCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("With only 100 silver, I can teleport you to somewhere far away. Please choose your destination.")
                            .AddOption("Twin~City.", 1)
                               .AddOption("Market in Twin City.", 2)
                            //.AddOption("DragonIsland.", 3)
                            // .AddOption("Market in Twin City.", 4)

                            .AddOption("Just~passing~by.", 255).AddAvatar(1).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {
                                case 1: client.Teleport(410, 354, 1002); break;
                                case 2: client.Teleport(216, 215, 1036); break;
                                case 3: client.Teleport(410, 354, 1002); break;
                                // case 4: client.Teleport(410, 354, 1002); break;

                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(1).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.BirdCityConductress)]
        public static void BirdCityConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("With only 100 silver, I can teleport you to somewhere far away.")
                            .AddOption("Twin~City.", 1)
                               .AddOption("Market in Twin City.", 2)
                            // .AddOption("DragonIsland.", 3)
                            // .AddOption("Market in Twin City.", 4)

                            .AddOption("Just~passing~by.", 255).AddAvatar(1).FinalizeDialog();

                        break;
                    }
                default:
                    {
                        if (client.Player.Money >= 100)
                        {
                            client.Player.Money -= 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            switch (Option)
                            {
                                case 1: client.Teleport(410, 354, 1002); break;
                                case 2: client.Teleport(216, 215, 1036); break;
                                case 3: client.Teleport(410, 354, 1002); break;
                                // case 4: client.Teleport(410, 354, 1002); break;

                            }
                        }
                        else
                        {
                            data.AddText("I~can~teleport~you~for~a~price~of~100~silver.")
                         .AddOption("Ah yes.", 255).AddAvatar(1).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.BoxerConductor)]
        public static void BoxerConductor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("If you want to leave just tell me when you are ready.It's free.You will be teleported in the city where in before coming here.")
                                                 .AddOption("Yeah.Thanks.", 1)
                                                 .AddOption("No,I shall stay here.", 255)
                                                  .AddOption("Just~passing~by.", 255)
                                                  .AddAvatar(15)
                                                  .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        {
                            uint PrevMap = client.Player.PMap;
                            switch (PrevMap)
                            {
                                default:
                                    {

                                        client.Teleport(410, 354, 1002);
                                        break;
                                    }
                                case 1000:
                                    {

                                        client.Teleport(500, 650, 1000);
                                        break;
                                    }
                                case 1020:
                                    {

                                        client.Teleport(565, 562, 1020);
                                        break;
                                    }
                                case 1011:
                                    {

                                        client.Teleport(188, 264, 1011);
                                        break;
                                    }
                                case 1015:
                                    {
                                        client.Teleport(717, 571, 1015);
                                        break;
                                    }
                            }

                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Ose)]
        public static void Ose(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("A good weapon will sure come in handy when fighting for wealth and fame.")
                         .AddOption("You bet!", 255)
                         .AddAvatar(59).FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.MarketConductress)]
        public static void MarketConductress(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Do you want to leave the market     I can teleport you for free.")
                         .AddOption("Yeah. Thanks.", 1)
                         .AddOption("No, I shall stay here.", 255).AddAvatar(156).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        {
                            uint PrevMap = client.Player.PMap;
                            switch (PrevMap)
                            {
                                default:
                                    {

                                        client.Teleport(410, 354, 1002);
                                        break;
                                    }
                                case 1000:
                                    {

                                        client.Teleport(500, 650, 1000);
                                        break;
                                    }
                                case 1020:
                                    {

                                        client.Teleport(565, 562, 1020);
                                        break;
                                    }
                                case 1011:
                                    {

                                        client.Teleport(188, 264, 1011);
                                        break;
                                    }
                                case 1015:
                                    {
                                        client.Teleport(717, 571, 1015);
                                        break;
                                    }
                            }

                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.OflineTGNpc)]
        public static void OflineTGNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.Level == 140)
                        {
                            client.Send(stream.OfflineTGStatsCreate(0, 0, client.Player.Level, 0));
                            break;
                        }
                        var T1 = new TimeSpan(DateTime.Now.Ticks);
                        var T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                        ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                        minutes = (ushort)Math.Min((ushort)900, minutes);

                        ushort nextlevel = 0;
                        ulong nextExperience = client.CalcExpBall(minutes, out nextlevel);

                        var LevelDBExp = Pool.LevelInfo[Database.DBLevExp.Sort.User][(byte)client.Player.Level];

                        decimal give_exp = (decimal)((decimal)nextExperience / (decimal)LevelDBExp.Experience);

                        string exp = give_exp.ToString().Replace(".", "");

                        string aaexpp = "";
                        for (int x = 0; x < exp.Length; x++)
                        {
                            if (x == 8)
                                break;
                            aaexpp += exp[x];

                        }
                        client.Send(stream.OfflineTGStatsCreate(minutes, (ushort)(900 - minutes), nextlevel, (long)(int.Parse(aaexpp))));

                        break;
                    }

            }
        }
        public static void RareYellowRuneBoxS(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        goto case 60;


                    }
                case 60:
                    {
                        data.Text("STR_ID_tStandardize_RewardPoolProps[3314245][Text111]@@\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option111]@@\n", 1);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option112]@@\n", 2);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option113]@@\n", 3);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option114]@@\n", 4);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option115]@@\n", 5);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option116]@@\n", 6);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option117]@@\n", 7);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option118]@@\n", 8);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option119]@@\n", 9);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option120]@@\n", 10);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option121]@@\n", 11);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option122]@@\n", 12);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option123]@@\n", 13);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option124]@@\n", 14);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option1100]@@\n", 29);
                        data.FinalizeDialog();
                        break;
                    }
                case 29:
                    {
                        data.Text("STR_ID_tStandardize_RewardPoolProps[3314245][Text111]@@\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option125]@@\n", 15);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option126]@@\n", 16);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option127]@@\n", 17);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option128]@@\n", 18);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option129]@@\n", 19);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option130]@@\n", 20);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option131]@@\n", 21);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option132]@@\n", 22);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option133]@@\n", 23);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option134]@@\n", 24);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option135]@@\n", 25);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option136]@@\n", 26);

                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option1101]@@\n", 60);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("Are you sure you want to select Sanctity?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 31);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 2:
                    {
                        data.Text("Are you sure you want to select Fire~Blast?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 32);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 3:
                    {
                        data.Text("Are you sure you want to select Serenity?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 33);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 4:
                    {
                        data.Text("Are you sure you want to select Wonder?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 34);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 5:
                    {
                        data.Text("Are you sure you want to select Soul~Chant?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 35);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 6:
                    {
                        data.Text("Are you sure you want to select Wide~Swipe?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 36);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 7:
                    {
                        data.Text("Are you sure you want to select Nirvana?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 37);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 8:
                    {
                        data.Text("Are you sure you want to select Sharpness?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 38);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 9:
                    {
                        data.Text("Are you sure you want to select Evocation?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 39);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 10:
                    {
                        data.Text("Are you sure you want to select Barrier?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 40);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 11:
                    {
                        data.Text("Are you sure you want to select Breakdown?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 41);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;


                    }
                case 12:
                    {
                        data.Text("Are you sure you want to select Quick~Mount?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 42);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.Text("Are you sure you want to select Divine~Shield?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 43);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.Text("Are you sure you want to select Reverse~Magic?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 44);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.Text("Are you sure you want to select Tacit~Strike?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 45);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        data.Text("Are you sure you want to select Deterrence?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 46);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 17:
                    {
                        data.Text("Are you sure you want to select Deadly~Sight?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 47);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        data.Text("Are you sure you want to select Overwhelm?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 48);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 19:
                    {
                        data.Text("Are you sure you want to select Judgment?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 49);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.Text("Are you sure you want to select Nature`s~Chant?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 50);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        data.Text("Are you sure you want to select Justice~Guard?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 51);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        data.Text("Are you sure you want to select Surging~Force?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 52);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        data.Text("Are you sure you want to select Miracle~Touch?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 53);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        data.Text("Are you sure you want to select Curse~Block?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 54);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        data.Text("Are you sure you want to select Universal~Shield?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 55);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        data.Text("Are you sure you want to select Tortoise~Smasher?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option211]@@\n", 56);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314245][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }

                default:
                    {
                        MsgGameItem item;
                        if (client.Inventory.SearchItemByID(client.ActiveNpc, out item))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 31: { itemID = 4032001; break; }
                                    case 32: { itemID = 4032101; break; }
                                    case 33: { itemID = 4032201; break; }
                                    case 34: { itemID = 4032301; break; }
                                    case 35: { itemID = 4032401; break; }
                                    case 36: { itemID = 4032501; break; }
                                    case 37: { itemID = 4032601; break; }
                                    case 38: { itemID = 4032701; break; }
                                    case 39: { itemID = 4032801; break; }
                                    case 40: { itemID = 4031601; break; }
                                    case 41: { itemID = 4031501; break; }
                                    case 42: { itemID = 4031401; break; }
                                    case 43: { itemID = 4034001; break; }
                                    case 44: { itemID = 4033901; break; }
                                    case 45: { itemID = 4033401; break; }
                                    case 46: { itemID = 4034101; break; }
                                    case 47: { itemID = 4033301; break; }
                                    case 48: { itemID = 4032901; break; }
                                    case 49: { itemID = 4033001; break; }
                                    case 50: { itemID = 4034201; break; }
                                    case 51: { itemID = 4034301; break; }
                                    case 52: { itemID = 4034401; break; }
                                    case 53: { itemID = 4034501; break; }
                                    case 54: { itemID = 4034601; break; }
                                    case 55: { itemID = 4034701; break; }
                                    case 56: { itemID = 4034801; break; }
                                    case 57: { itemID = 4035101; break; }
                                    case 58: { itemID = 4035201; break; }
                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false, Flags.ItemEffect.None);
                            }
                        }

                        break;
                    }


            }
        }
        public static void RareBlueRuneBoxS(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tStandardize_RewardPoolProps[3314246][Text111]@@\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option111]@@\n", 1);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option112]@@\n", 2);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option113]@@\n", 3);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option114]@@\n", 4);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option115]@@\n", 5);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option116]@@\n", 6);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option117]@@\n", 7);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option118]@@\n", 8);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option119]@@\n", 9);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option120]@@\n", 10);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option121]@@\n", 11);
                        data.FinalizeDialog();
                        break;


                    }
                case 1:
                    {
                        data.Text("Are you sure you want to select Double~Thunder?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 31);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 2:
                    {
                        data.Text("Are you sure you want to select Crack~Star?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 32);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 3:
                    {
                        data.Text("Are you sure you want to select Shadow~Fist?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 33);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 4:
                    {
                        data.Text("Are you sure you want to select Retaliation?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 34);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 5:
                    {
                        data.Text("Are you sure you want to select Star~Raid?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 35);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 6:
                    {
                        data.Text("Are you sure you want to select Duel?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 36);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 7:
                    {
                        data.Text("Are you sure you want to select Neptune`s~Curse?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 37);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 8:
                    {
                        data.Text("Are you sure you want to select Space~Leap?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 38);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 9:
                    {
                        data.Text("Are you sure you want to select Rise~of~Taoism?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 39);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 10:
                    {
                        data.Text("Are you sure you want to select Fury~Strike?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 40);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 11:
                    {
                        data.Text("Are you sure you want to select Blood~Tide?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option211]@@\n", 41);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314246][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 31: { itemID = 4021101; break; }
                                case 32: { itemID = 4021201; break; }
                                case 33: { itemID = 4021301; break; }
                                case 34: { itemID = 4021401; break; }
                                case 35: { itemID = 4021501; break; }
                                case 36: { itemID = 4021601; break; }
                                case 37: { itemID = 4021701; break; }
                                case 38: { itemID = 4021801; break; }
                                case 39: { itemID = 4021901; break; }
                                case 40: { itemID = 4022001; break; }
                                case 41: { itemID = 4023001; break; }

                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Flags.ItemEffect.None);
                        }
                        break;
                    }


            }
        }

        public static void BlueRuneSelectionPack2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            string add = Input == "3306561" ? "" : "";
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Please Choose One From The Following (Blue Rune)~(+1) " + add + ".")
                       .AddOption("Trojan Rampage (+1)." + add + "", 1)
                       .AddOption("Warrior All round (+1)." + add + "", 2)
                       .AddOption("Archer Infinity (+1)." + add + "", 3)
                       .AddOption("Ninja Absolution (+1)." + add + "", 4)
                       .AddOption("Monk Iron Shield (+1)." + add + "", 5)
                       .AddOption("Pirate Slayer (+1)." + add + "", 6)
                       .AddOption("Dragon~Warrior Counter Punch (+1)." + add + "", 7)
                       .AddOption("Next~Page.", 8)
                       .AddAvatar(1098).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        data.AddText("Please Choose One From The Following (Blue Rune)~(+1) " + add + ".")
                       .AddOption("Trojan Rampage (+1)." + add + "", 1)
                       .AddOption("Warrior All round (+1)." + add + "", 2)
                       .AddOption("Archer Infinity (+1)." + add + "", 3)
                       .AddOption("Ninja Absolution (+1)." + add + "", 4)
                       .AddOption("Monk Iron Shield (+1)." + add + "", 5)
                       .AddOption("Pirate Slayer (+1)." + add + "", 6)
                       .AddOption("Dragon~Warrior Counter Punch (+1)." + add + "", 7)
                       .AddOption("Next~Page.", 8)
                       .AddAvatar(1098).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Please Choose One From The Following (Blue Rune)~(+1) " + add + ".")
                       .AddOption("Water~Taoist Fine Rain (+1)." + add + "", 9)
                       .AddOption("Fire~Taoist Fire Curse (+1)." + add + "", 10)
                       .AddOption("Wind~Walker Wild Wind (+1)." + add + "", 11)
                       .AddOption("Thunder~Striker`s Flash Shield (+1)." + add + "", 12)
                        .AddOption("Dune~Wanderere Tide Reversal (+1)." + add + "", 25)
                            // .AddOption("Wind~Walker Double Thunder (+1)." + add + "", 13)
                       .AddOption("Previous.", 50)
                       .AddOption("Leave.", 255)
                      .AddAvatar(1098).FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        //  data.AddText("Please Choose One From The Following (Blue Rune) " + add + ".")
                        // .AddOption("Wind~Walker Crack Star (+1)." + add + "", 14)
                        //.AddOption("Dragon~Warrior Shadow Fist (+1)." + add + "", 15)
                        // .AddOption("Warrior Retaliation (+1)." + add + "", 16)
                        // .AddOption("Archer Star Raid (+1)." + add + "", 17)
                        // .AddOption("Ninja Duel (+1)." + add + "", 18)
                        //.AddOption("Previous.", 8)
                        // .AddOption("Next~Page.", 70)
                        // .FinalizeDialog();
                        break;
                    }
                case 70:
                    {
                        //data.AddText("Please Choose One From The Following (Blue Rune) " + add + ".")
                        //.AddOption("Pirate Neptune`s Curs (+1)." + add + "", 19)
                        // .AddOption("Water~Taoist Space Leap (+1)." + add + "", 20)
                        // .AddOption("Fire~Taoist Riseof Taoism (+1)." + add + "", 21)
                        //  .AddOption("Monk Fury Strike (+1)." + add + "", 22)
                        //  .AddOption("Trojan Blood Tide (+1)." + add + "", 24)
                        // .AddOption("Previous.", 8)
                        // .AddOption("Leave.", 255)
                        // .FinalizeDialog();
                        break;
                    }
                default:
                    {
                        if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                        {
                            uint itemID = 0;
                            switch (Option)
                            {
                                case 1: { itemID = 4020101; break; }
                                case 2: { itemID = 4020201; break; }
                                case 3: { itemID = 4020301; break; }
                                case 4: { itemID = 4020401; break; }
                                case 5: { itemID = 4020501; break; }
                                case 6: { itemID = 4020601; break; }
                                case 7: { itemID = 4020701; break; }
                                case 9: { itemID = 4020801; break; }
                                case 10: { itemID = 4020901; break; }
                                case 11: { itemID = 4021001; break; }
                                case 12: { itemID = 4022201; break; }
                                case 13: { itemID = 4021101; break; }
                                case 14: { itemID = 4021201; break; }
                                case 15: { itemID = 4021301; break; }
                                case 16: { itemID = 4021401; break; }
                                case 17: { itemID = 4021501; break; }
                                case 18: { itemID = 4021601; break; }
                                case 19: { itemID = 4021701; break; }
                                case 20: { itemID = 4021801; break; }
                                case 21: { itemID = 4021901; break; }
                                case 22: { itemID = 4022001; break; }
                                case 23: { itemID = 4022201; break; }
                                case 24: { itemID = 4023001; break; }
                                case 25: { itemID = 4028801; break; }
                            }
                            client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, Input == "3306561");
                        }
                        break;
                    }
            }
        }

        public static void RedRuneSelectionPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        goto case 50;
                    }
                case 50:
                    {
                        data.AddText("STR_ID_tStandardize_RewardPoolProps[3314242][Text111]@@\n")
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option111]@@\n", 1)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option112]@@\n", 2)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option113]@@\n", 3)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option114]@@\n", 4)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option115]@@\n", 5)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option116]@@\n", 6)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option117]@@\n", 7)
                       .AddOption("Next.", 80)
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("STR_ID_tStandardize_RewardPoolProps[3314242][Text111]@@\n")
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option118]@@\n", 8)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option119]@@\n", 9)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option120]@@\n", 10)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option121]@@\n", 11)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314242][Option122]@@\n", 12)
                       .AddOption("Previous.", 50)
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("Are you sure you want to select Revenge~Gale?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 30);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 2:
                    {
                        data.Text("Are you sure you want to select Burning~Sky?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 31);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 3:
                    {
                        data.Text("Are you sure you want to select Sacrifice?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 32);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 4:
                    {
                        data.Text("Are you sure you want to select Swinging~Tail?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 33);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 5:
                    {
                        data.Text("Are you sure you want to select Acalanatha?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 34);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 6:
                    {
                        data.Text("Are you sure you want to select Frost~Arrows?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 35);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 7:
                    {
                        data.Text("Are you sure you want to select Assassin?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 36);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 8:
                    {
                        data.Text("Are you sure you want to select Tide~Trap?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 37);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 9:
                    {
                        data.Text("Are you sure you want to select Comprehension?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 38);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 10:
                    {
                        data.Text("Are you sure you want to select Pitching?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 39);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 11:
                    {
                        data.Text("Are you sure you want to select Chained~Storm?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 40);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 12:
                    {
                        data.Text("Are you sure you want to select Fighting~Will?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option211]@@\n", 41);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314242][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }

                default:
                    {
                        MsgGameItem item;
                        if (client.Inventory.SearchItemByID(client.ActiveNpc, out  item))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 30: { itemID = 4010901; break; }
                                    case 31: { itemID = 4010801; break; }
                                    case 32: { itemID = 4010701; break; }
                                    case 33: { itemID = 4010601; break; }
                                    case 34: { itemID = 4010401; break; }
                                    case 35: { itemID = 4010201; break; }
                                    case 36: { itemID = 4010301; break; }
                                    case 37: { itemID = 4010501; break; }
                                    case 38: { itemID = 4010001; break; }
                                    case 39: { itemID = 4010101; break; }
                                    case 40: { itemID = 4011501; break; }
                                    case 41: { itemID = 4011401; break; }
                                }
                                if (item.Bound > 0)
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Flags.ItemEffect.None, Input == "3306562");
                                else
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Flags.ItemEffect.None, Input == "3306562");
                            }
                        }
                        break;
                    }
            }
        }
        public static void BlueRuneSelectionPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        goto case 50;
                    }
                case 50:
                    {
                        data.AddText("STR_ID_tStandardize_RewardPoolProps[3314244][Text111]@@\n")
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option111]@@\n", 1)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option112]@@\n", 2)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option113]@@\n", 3)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option114]@@\n", 4)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option115]@@\n", 5)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option116]@@\n", 6)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option117]@@\n", 7)
                       .AddOption("Next.", 80)
                       .FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        data.AddText("STR_ID_tStandardize_RewardPoolProps[3314244][Text111]@@\n")
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option118]@@\n", 8)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option119]@@\n", 9)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option120]@@\n", 10)
                       .AddOption("STR_ID_tStandardize_RewardPoolProps[3314244][Option121]@@\n", 11)
                       .AddOption("Previous.", 50)
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("Are you sure you want to select Rampage?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 30);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 2:
                    {
                        data.Text("Are you sure you want to select All-round?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 31);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 3:
                    {
                        data.Text("Are you sure you want to select Infinity?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 32);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 4:
                    {
                        data.Text("Are you sure you want to select Absolution?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 33);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 5:
                    {
                        data.Text("Are you sure you want to select Iron~Shield?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 34);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 6:
                    {
                        data.Text("Are you sure you want to select Slayer?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 35);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 7:
                    {
                        data.Text("Are you sure you want to select Counter~Punch?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 36);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 8:
                    {
                        data.Text("Are you sure you want to select Fine~Rain?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 37);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 9:
                    {
                        data.Text("Are you sure you want to select Fire~Curse?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 38);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 10:
                    {
                        data.Text("Are you sure you want to select Wild~Wind?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 39);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 11:
                    {
                        data.Text("Are you sure you want to select Flash~Shield?\n");
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option211]@@\n", 40);
                        data.Option("STR_ID_tStandardize_RewardPoolProps[3314244][Option212]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }

                default:
                    {
                        MsgGameItem item;
                        if (client.Inventory.SearchItemByID(client.ActiveNpc, out  item))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 30: { itemID = 4020101; break; }
                                    case 31: { itemID = 4020201; break; }
                                    case 32: { itemID = 4020301; break; }
                                    case 33: { itemID = 4020401; break; }
                                    case 34: { itemID = 4020501; break; }
                                    case 35: { itemID = 4020601; break; }
                                    case 36: { itemID = 4020701; break; }
                                    case 37: { itemID = 4020801; break; }
                                    case 38: { itemID = 4020901; break; }
                                    case 39: { itemID = 4021001; break; }
                                    case 40: { itemID = 4022201; break; }

                                }
                                if (item.Bound > 0)
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Flags.ItemEffect.None, Input == "3306562");
                                else
                                    client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Flags.ItemEffect.None, Input == "3306562");
                            }
                        }
                        break;
                    }
            }
        }
        public static void YellowRuneSelectionPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);

            switch (Option)
            {

                case 0:
                    {
                        data.Text("STR_ID_tMoneyTreeGift[3307212][Text111]@@\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option112]@@\n", 1);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option113]@@\n", 2);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option114]@@\n", 3);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option115]@@\n", 4);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option116]@@\n", 5);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option117]@@\n", 6);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option118]@@\n", 7);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option119]@@\n", 8);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option1110]@@\n", 9);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option1111]@@\n", 10);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option1112]@@\n", 11);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option1113]@@\n", 12);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option1114]@@\n", 13);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.Text("Are you sure you want to select Tortoise~Breaker?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 15);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.Text("Are you sure you want to select Draining~Touch~Booster?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@", 16);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.Text("Are you sure you want to select Blood~Spawn~Booster?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 17);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.Text("Are you sure you want to select Healer?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 18);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 5:
                    {
                        data.Text("Are you sure you want to select Sturdiness?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 19);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 6:
                    {
                        data.Text("Are you sure you want to select XP~Killer?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 20);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 7:
                    {
                        data.Text("Are you sure you want to select Grabber?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 21);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 8:
                    {
                        data.Text("Are you sure you want to select MP~Master?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 22);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 9:
                    {
                        data.Text("Are you sure you want to select Boss~Killer?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 23);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 10:
                    {
                        data.Text("Are you sure you want to select No~Mercy?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 24);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;


                    }
                case 11:
                    {
                        data.Text("Are you sure you want to select XP~Booster?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 25);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 12:
                    {
                        data.Text("Are you sure you want to select Free~Soul~Booster?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 26);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }
                case 13:
                    {
                        data.Text("Are you sure you want to select Witchery?\n");
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option121]@@\n", 27);
                        data.Option("STR_ID_tMoneyTreeGift[3307212][Option122]@@\n", 255);
                        data.FinalizeDialog();
                        break;

                    }

                default:
                    {
                        MsgGameItem item;
                        if (client.Inventory.SearchItemByID(client.ActiveNpc, out  item))
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, 1, stream))
                            {
                                uint itemID = 0;
                                switch (Option)
                                {
                                    case 15: { itemID = 4030101; break; }
                                    case 16: { itemID = 4030201; break; }
                                    case 17: { itemID = 4030301; break; }
                                    case 18: { itemID = 4030401; break; }
                                    case 19: { itemID = 4030501; break; }
                                    case 20: { itemID = 4030601; break; }
                                    case 21: { itemID = 4030701; break; }
                                    case 22: { itemID = 4030801; break; }
                                    case 23: { itemID = 4030901; break; }
                                    case 24: { itemID = 4031001; break; }
                                    case 25: { itemID = 4031101; break; }
                                    case 26: { itemID = 4031201; break; }
                                    case 27: { itemID = 4031301; break; }
                                }
                                client.Inventory.Add(stream, itemID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound >= 1 ? true : false, Flags.ItemEffect.None);
                            }
                        }
                        break;
                    }
            }
        }
        //public static void RuneBag(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3304540, 14);


        //                break;
        //            }
        //    }
        //}

        //public static void P7WeaponSoul(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3004247, 16);

        //                break;
        //            }
        //    }
        //}
        //public static void SacredRefinery(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3004249, 17);


        //                break;
        //            }
        //    }
        //}
        //public static void P7EquipmentSoul(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3004248, 15);


        //                break;
        //            }
        //    }
        //}
        public static void P6SoulPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Remove(3005893, 1, stream))
                        {
                            ushort rand = (ushort)(Pool.GetRandom.Next() % 1000);
                            byte count = (byte)(rand % 3);
                            if (Database.ItemType.PurificationItems.ContainsKey(6))
                            {
                                var array = Database.ItemType.PurificationItems[6].Values.ToArray();
                                for (int x = 0; x < 1; x++)
                                {
                                    int position = Pool.GetRandom.Next(0, array.Length);
                                    client.Inventory.Add(stream, array[position].ID);
                                }
                            }

                        }

                        break;
                    }
            }
        }
        //public static void RedRune(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3306560, 21);


        //                break;
        //            }
        //    }
        //}

        //public static void BlueRune(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3306561, 20);


        //                break;
        //            }
        //    }
        //}
        //public static void Mount5Star(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3008263, 13);


        //                break;
        //            }
        //    }
        //}
        //public static void Mount4Star(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3008267, 12);


        //                break;
        //            }
        //    }
        //}
        //public static void Germant5Star(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3008265, 10);

        //                break;
        //            }
        //    }
        //}
        //public static void Germant4Star(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                client.MyExchangeShop.Open(stream, 3008253, 11);


        //                break;
        //            }
        //    }
        //}
        //public static void RelicSpirit(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {

        //        case 0:
        //            {
        //                data.AddText("                      *Shaping of Relic Spirit*\n")
        //               .AddText("==========================================================\n")
        //               .AddText("		Required Material to Shape: any one of the treasures listed below\n")
        //               .AddText("		Expect to Get: A random Relic\n")
        //               .AddText("==========================================================\n")
        //               .AddText("Please choose one from the following materials to shape your Relic Spirit into a Relic.\n")

        //               .AddOption("10~Meteor~Scrolls.", 1)
        //               .AddOption("1~Dragon~Ball.", 2)
        //               .AddOption("10~Super~Protection~Pills.", 3)
        //               .AddOption("15~Special~Training~Pills.", 4)
        //               .AddOption("15~Favored~Training~Pills.", 5)
        //               .AddOption("3~Senior~Training~Pills.", 6)
        //               .AddOption("3~Free~Training~Pills.", 7)
        //               .AddOption("Next.", 8)
        //               .FinalizeDialog();
        //                break;
        //            }
        //        case 8:
        //            {
        //                data.AddText("                      *Shaping of Relic Spirit*\n")
        //               .AddText("==========================================================\n")
        //               .AddText("		Required Material to Shape: any one of the treasures listed below\n")
        //               .AddText("		Expect to Get: A random Relic\n")
        //               .AddText("==========================================================\n")
        //               .AddText("Please choose one from the following materials to shape your Relic Spirit into a Relic.\n")

        //               .AddOption("1~Mystery~Fruit.", 9)
        //               .AddOption("1~Sash~(S).", 10)
        //               .AddOption("Previous.", 0)
        //               .FinalizeDialog();
        //                break;
        //            }
        //        default:
        //            {
        //                uint ItemID = 720027;
        //                ushort ItemsCount = 10;
        //                if (Option == 2) { ItemID = 1088000; ItemsCount = 1; }
        //                else if (Option == 3) { ItemID = 3002030; }
        //                else if (Option == 4) { ItemID = 3003125; ItemsCount = 15; }
        //                else if (Option == 5) { ItemID = 3003124; ItemsCount = 15; }
        //                else if (Option == 6) { ItemID = 3003126; ItemsCount = 3; }
        //                else if (Option == 7) { ItemID = 3002926; ItemsCount = 3; }
        //                else if (Option == 9) { ItemID = 3001044; ItemsCount = 1; }
        //                else if (Option == 10) { ItemID = 1100003; ItemsCount = 1; }
        //                if (client.Inventory.Remove(client.ActiveNpc, 1, stream) && client.Inventory.Remove(ItemID, ItemsCount, stream))
        //                {
        //                    client.Inventory.AddRandomRelicL1(stream);
        //                    client.Player.RelicSpirit = true;
        //                }
        //                break;
        //            }
        //    }

        //}
        public static void RelicCrystal(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Relic Crystal can be used to repair your equipped Relic. You can also collect 100 Relic Crystals and ask Saint Taiyi (Deityland 1013,1278) help you craft a complete Relic.")

                       .AddOption("Repair~my~Relic.", 1)
                       .AddOption("Take~me~to~see~Saint~Taiyi.", 2)
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var Item = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Relics);
                        if (Item != null)
                        {
                            if (Item.Durability == Item.MaximDurability)
                            {
                                data.AddText("Your relics is already at Max Durability!")
                                .AddOption("Okay.")
                                .FinalizeDialog();
                                break; ;
                            }
                            var count = client.Inventory.GetCountItem(client.ActiveNpc);

                            data.AddText("\t\t\t\t\t\t\t\t\t\t\t\t\t*Repairing Relic*\n")
                                .AddText("\t\t\t==============================================\n")
                                .AddText("\t\t\t\t\t\t\t\t\t\tEquipped Relic: " + Pool.ItemsBase[Item.ITEM_ID].Name + "\n")
                                .AddText("\t\t\t\t\t\t\t\t\t\tRelic Crystals Required: " + ((Item.MaximDurability - Item.Durability) / 100).ToString() + "\n")
                                .AddText("\t\t\t\t\t\t\t\t\t\tYour Relic Crystals: " + count.ToString() + "\n")
                                .AddText("\t\t\t==============================================\n")
                                .AddText("Tip: Each Relic Crystal increases the Durability by 1 point.")

                               .AddOption("Repair now.", 3)
                               .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Please equip a Relic, first.")
                           .AddOption("Okay.")
                           .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.Map == 10250)
                            client.Player.QuestGUI.SendAutoPatcher(10250, 1014, 1282, 22569);
                        break;
                    }
                case 3:
                    {
                        var Item = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Relics);
                        if (Item != null)
                        {
                            if (client.Inventory.Remove(client.ActiveNpc, (ushort)((Item.MaximDurability - Item.Durability) / 100), stream))
                            {
                                Item.Durability = Item.MaximDurability;
                                Item.Mode = Role.Flags.ItemMode.Update;
                                Item.Send(client, stream);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }
                        break;
                    }
            }

        }


        [NpcAttribute(NpcID.DragonFurnaceTwin)]
        public static void DragonFurnaceTwin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=Animae#are#formed#by#spirits#of#all#creatures#and#empowered#by#the#universe.\nThis#ancient#furnace#contains#wonder#energy#to#forge#Anima.>Dragon Furnace</tip>:\n");
                        dialog.Text("      This ancient furnace contains wonder energy that may enhance your Anima.");
                        dialog.Text("~However, it`s not always successful. Are you sure you want to use this furnace?");

                        dialog.Option("Yes.", 254);
                        dialog.Option("Get~details~on~official~site.", 253).FinalizeDialog();
                        break;
                    }
                case 253:
                    {
                        client.SendSysMesage("https://co.99.com/guide/event/anima_introduction.shtml", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red);
                        break;
                    }
                case 254:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=Animae#are#formed#by#spirits#of#all#creatures#and#empowered#by#the#universe.\nThis#ancient#furnace#contains#wonder#energy#to#forge#Anima.>Dragon Furnace</tip>:\n");
                        dialog.Text("      It`s said there is an alien world located deep in the Dragon Furnace.");
                        dialog.Text("~When you put in one or two Animae, you have a chance to make a stronger Anima.");

                        //   dialog.Option("Forge~Anima.", 1);
                        dialog.Option("Fuse~Animae.", 100);
                        dialog.Option("Smelt~Anima.", 200);
                        dialog.Option("I`m~not~interested~in~it.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.Text("    In the alien world, there are several floors. Your Anima will be forged on corresponding floor.");
                        dialog.Text("~If the forging is successful, your Anima`s phase may be upgraded.");
                        dialog.Text("~You have 8 chances to forge the Anima every time you visit the alien world, and you can stop forging at any time.\n");
                        dialog.Text("    Which floor would you like to visit and forge your Anima?");

                        // dialog.Option("12th~floor.~(1~P12~Anima)", 12);
                        dialog.Option("11th~floor.~(1~P11~Anima)", 11);
                        dialog.Option("10th~floor.~(1~P10~Anima)", 10);
                        dialog.Option("9th~floor.~(1~P9~Anima)", 9);
                        dialog.Option("8th~floor.~(1~P8~Anima)", 8);
                        dialog.Option("7th~floor.~(1~P7~Anima)", 7);
                        dialog.Option("6th~floor.~(1~P6~Anima)", 6);
                        dialog.Option("Next.", 50).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        dialog.Text("    In the alien world, there are several floors. Your Anima will be forged on corresponding floor.");
                        dialog.Text("~If the forging is successful, your Anima`s phase may be upgraded.");
                        dialog.Text("~You have 8 chances to forge the Anima every time you visit the alien world, and you can stop forging at any time.\n");
                        dialog.Text("    Which floor would you like to visit and forge your Anima?");

                        dialog.Option("6th~floor.~(1~P6~Anima)", 6);
                        dialog.Option("5th~floor.~(1~P5~Anima)", 5);
                        dialog.Option("4th~floor.~(1~P4~Anima)", 4);
                        dialog.Option("3rd~floor.~(1~P3~Anima)", 3);
                        dialog.Option("Tell~me~more~details.", 51);
                        dialog.Option("Previous.", 1).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        dialog.Text("     Every time you put an Anima into the Furnace, you have 8 chances to forge it.");
                        dialog.Text("~After each successful forging, you can choose to Continue or Stop.");

                        dialog.Option("What`s~the~forging~success~rate?", 52).FinalizeDialog();
                        break;
                    }
                case 52:
                    {
                        dialog.Text("      There is a 55% chance of successfully forging an Anima that may upgrade the Anima`s phase.");
                        dialog.Text("~If the forging fails (45% chance), you`ll lose the Anima.");

                        dialog.Option("How~to~forge~an~Anima?", 51).FinalizeDialog();
                        break;
                    }

                case 100:
                    {
                        dialog.Text("    To fuse your Animae, you must have two same Animae and the Ancient Dragon`s blood.");
                        dialog.Text("~You`ll find the Ancient Dragon in the alien world. Fusing Animae may bring you a new");
                        dialog.Text("~Anima of higher phase and better value. You know, it`s not always successful.\n");
                        dialog.Text("    Which floor would you like to visit and fuse your Animae?");

                        // dialog.Option("12th~floor.~(2~P12~Animae)", 112);
                        dialog.Option("11th~floor.~(2~P11~Animae)", 111);
                        dialog.Option("10th~floor.~(2~P10~Animae)", 110);
                        dialog.Option("9th~floor.~(2~P9~Animae)", 109);
                        dialog.Option("8th~floor.~(2~P8~Animae)", 108);
                        dialog.Option("7th~floor.~(2~P7~Animae)", 107);
                        dialog.Option("6th~floor.~(2~P6~Animae)", 106);
                        dialog.Option("Next.", 150).FinalizeDialog();
                        break;
                    }
                case 150:
                    {
                        dialog.Text("    To fuse your Animae, you must have two same Animae and the Ancient Dragon`s blood.");
                        dialog.Text("~You`ll find the Ancient Dragon in the alien world. Fusing Animae may bring you a new");
                        dialog.Text("~Anima of higher phase and better value. You know, it`s not always successful.\n");
                        dialog.Text("    Which floor would you like to visit and fuse your Animae?");

                        dialog.Option("5th~floor.~(2~P5~Animae)", 105);
                        dialog.Option("4th~floor.~(2~P4~Animae)", 104);
                        dialog.Option("3rd~floor.~(2~P3~Animae)", 103);
                        dialog.Option("2nd~floor.~(2~P2~Animae)", 102);
                        dialog.Option("1st~floor.~(2~P1~Animae)", 101);
                        dialog.Option("Tell~me~more~details.", 151);
                        dialog.Option("Previous.", 100).FinalizeDialog();
                        break;
                    }
                case 151:
                    {
                        dialog.Text("      There is a 50% chance of successfully fusing two same Animae into a new Anima of higher phase.");
                        dialog.Text("~That is to say, there is 50% chance it may fail.");

                        dialog.Option("Learn~about~other~things.", 150);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 200:
                    {
                        dialog.Text("    In the alien world, the twin furnaces for smelting are called Solar Furnace and Lunar Furnace.");
                        dialog.Text("~You can choose one of them to try your luck. If the smelting is successful, you may upgrade");
                        dialog.Text("~the Anima`s phase, making it more valuable.\n");

                        dialog.Option("Smelt~P3~to~P12~Anima.", 201);
                        dialog.Option("Tell~me~more~details.", 202);
                        dialog.Option("Learn~about~other~things.", 254);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 202:
                    {
                        dialog.Text("    The Solar Furnace and Lunar Furnace hidden in the alien world provide more possibilities of Anima improvement.");
                        dialog.Text("The fire in the twin furnaces never die out.");

                        dialog.Option("What`s~the~smelting~success~rate?", 203);
                        dialog.Option("Get~details~on~official~site.", 253);
                        dialog.Option("Learn~about~other~things.", 200).FinalizeDialog();
                        break;
                    }
                case 203:
                    {
                        dialog.Text("      There is a 50% chance of successfully smelting an Anima that may upgrade its phase.");
                        dialog.Text("~Things are not always perfect. There is another 50% chance that it may fail.");

                        dialog.Option("Learn~about~other~things.", 200);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 201:
                    {
                        client.Teleport(54, 59, 10428);
                        client.CreateBoxDialog("With a strong wind, you were taken to the alien world. Go and check which furnace would you like to use to smelt Anima.");
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                        client.Player.DragonFurnace = 0;

                        break;
                    }
                default:
                    {
                        if (Option >= 101 && Option < 113)//Fuse
                        {
                            byte floor = (byte)(Option % 100);
                            if (client.Inventory.Contain((uint)(4200000 + floor), 2))
                            {
                                ushort X = 0, Y = 0;
                                Pool.ServerMaps[10428].GetRandCoord(ref X, ref Y);
                                client.Teleport(X, Y, 10428, Pool.ServerMaps[10428].GenerateDynamicID());
                                client.Player.DragonFurnace = (uint)(4200000 + floor);


                                Pool.ServerMaps[10428].GetRandCoord(ref X, ref Y);
                                client.Map.AddMapMonster(stream, 2984, X, Y, 1, 1, 1, client.Player.DynamicID);

                                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                                np.UID = (uint)NpcID.DragonFurnaceFusing;
                                np.NpcType = Role.Flags.NpcType.Talker;
                                np.Mesh = 22850;
                                np.Map = client.Player.Map;
                                np.DynamicID = client.Player.DynamicID;
                                np.X = 54;
                                np.Y = 59;
                                client.Map.AddNpc(np);
                                np.Send(stream);
                                np.Send(stream);
                                client.Player.View.Role(false);

                                client.CreateBoxDialog("With a strong wind, you were taken to the alien world floor " + floor + ". Go defeat the Ancient Dragon and start fusing your Animae in the Dragon Furnace.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                            }
                            else
                            {
                                dialog.Text("    Please make sure you have 2 " + Pool.ItemsBase[(uint)(4200000 + floor)].Name + ", first.");

                                dialog.Option("Okay.", 255).FinalizeDialog();
                            }
                        }
                        else if (Option >= 3 && Option < 13)//Forging
                        {
                            byte floor = (byte)(Option % 100);
                            if (client.Inventory.Contain((uint)(4200000 + floor), 1))
                            {
                                ushort X = 0, Y = 0;
                                Pool.ServerMaps[10428].GetRandCoord(ref X, ref Y);
                                client.Teleport(X, Y, 10428, Pool.ServerMaps[10428].GenerateDynamicID());
                                client.Player.DragonFurnace = (uint)(4200000 + floor);
                                client.Player.DragonFurnaceForge = 0;
                                client.Player.DragonFurnaceForgeCount = 0;

                                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                                np.UID = (uint)NpcID.DragonFurnaceForging;
                                np.NpcType = Role.Flags.NpcType.Talker;
                                np.Mesh = 15370;
                                np.Map = client.Player.Map;
                                np.DynamicID = client.Player.DynamicID;
                                np.X = 54;
                                np.Y = 59;
                                client.Map.AddNpc(np);
                                np.Send(stream);
                                client.Player.View.Role(false);

                                client.CreateBoxDialog("With a strong wind, you were taken to the alien world floor " + floor + ". Go forge your Anima!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "moveback" });
                            }
                            else
                            {
                                dialog.Text("    Please make sure you have 1 " + Pool.ItemsBase[(uint)(4200000 + floor)].Name + ", first.");

                                dialog.Option("Okay.", 255).FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LuckySmeltingList)]
        public static void LuckySmeltingList(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        var array = Pool.SmeltingSuccesses.Take(10).ToList();
                        Pool.SmeltingSuccesses = array;
                        dialog.Text("                   *Lucky Smelting List*\n");
                        dialog.Text("------------------------------------------------------\n");
                        if (array.Count > 0)
                        {
                            dialog.Text("          [Player]          [Anima Gained Item]\n");
                            for (int i = array.Count - 1; i >= 0; i--)
                            {
                                dialog.Text("        <tip color=" + (Pool.SmeltingSuccesses[i].Furnace == 0 ? "0xff4bb52d desc=Solar#Furnace" : "0xfff05050 desc=Lunar#Furnace") + ">" + Pool.SmeltingSuccesses[i].Name + "</tip>       " + Pool.SmeltingSuccesses[i].Prize + "\n");
                            }
                        }
                        else
                        {
                            dialog.Text("                       [No data]\n");
                        }
                        dialog.Text("------------------------------------------------------\n");
                        dialog.Text("      The heroes successfully smelting a P14+ Anima will be on the list.\n");

                        dialog.Option("Recent~successful~smelting~records.", 1);
                        dialog.Option("Close.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var array = Pool.SmeltingSessions.Take(10).ToList();
                        Pool.SmeltingSessions = array;
                        dialog.Text("                   *Recent Successful Smelting Records*\n");
                        dialog.Text("------------------------------------------------------\n");
                        if (array.Count > 0)
                        {
                            dialog.Text("          [Furnace]         [Smelting Session]\n");
                            for (int i = array.Count - 1; i >= 0; i--)
                            {
                                dialog.Text("        <tip color=" + (Pool.SmeltingSessions[i] == 0 ? "0xff4bb52d>Solar Furnace" : "0xfff05050>Lunar Furnace") + "</tip>       Session 000" + i + "\n");
                            }
                        }
                        else
                        {
                            dialog.Text("                       [No data]\n");
                        }
                        dialog.Text("------------------------------------------------------\n");

                        dialog.Option("Close.", 255).FinalizeDialog();
                        break;

                    }
            }
        }


        [NpcAttribute(NpcID.SolarFurnace)]
        public static void SolarFurnace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Map.View.GetAllMapRolesCount(Role.MapObjectType.Item, i => (i as MsgItem).MsgFloor.m_ID == 2285) > 0 || client.Player.DragonFurnace > 0)
            {
                client.CreateBoxDialog("There is already an Anima smelting in the furnace. Please try again during the preparation stage for a new round of smelting.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=With#the#solar#power,#this#furnace#was#built#to#smelt#Anima.>Solar Furnace</tip>   \n");
                        dialog.Text("    The solar energy to fuel this furnace can smelt Anima. Only one of the twin furnaces can perform successful smelting at a time. If you choose the right one, you may get a higher-phase Anima.");

                        dialog.Option("P12~Anima.", 12);
                        dialog.Option("P11~Anima.", 11);
                        dialog.Option("P10~Anima.", 10);
                        dialog.Option("P9~Anima.", 9);
                        dialog.Option("P8~Anima.", 8);
                        dialog.Option("P7~Anima.", 7);
                        dialog.Option("P6~Anima.", 6);
                        dialog.Option("Next.", 13).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=With#the#solar#power,#this#furnace#was#built#to#smelt#Anima.>Solar Furnace</tip>   \n");
                        dialog.Text("    The solar energy to fuel this furnace can smelt Anima. Only one of the twin furnaces can perform successful smelting at a time. If you choose the right one, you may get a higher-phase Anima.");

                        dialog.Option("P5~Anima.", 5);
                        dialog.Option("P4~Anima.", 4);
                        dialog.Option("P3~Anima.", 3);
                        dialog.Option("Tell~me~more~details.", 14);
                        dialog.Option("Previous.", 0).FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        dialog.Text("    It`s said the twin furnaces were built by Gan Jiang and Ou Yezi. They were legendary blacksmiths in ancient China.");
                        dialog.Text("Though thousands of years have passed, the fire in the twin furnaces are still fierce.");

                        dialog.Option("What`s~the~smelting~success~rate?", 15);
                        dialog.Option("Get~details~on~official~site.", 16);
                        dialog.Option("Learn~about~other~things.", 13);
                        break;
                    }
                case 15:
                    {
                        dialog.Text("    There is a 50% chance of successfully smelting an Anima that may upgrade its phase. Things are not always perfect. There is another 50% chance that it may fail.");

                        dialog.Option("Learn~about~other~things.", 14);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        client.SendSysMesage("https://co.99.com/guide/event/anima_introduction.shtml", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red);
                        break;
                    }
                default:
                    {
                        if (Option >= 3 && Option < 13)//Fuse
                        {
                            if (client.Inventory.Remove((uint)(4200000 + Option), 1, stream))
                            {
                                client.Player.DragonFurnace = (uint)(4200000 + Option);
                                client.CreateBoxDialog("You activated SolarFurnace and put in a " + Pool.ItemsBase[client.Player.DragonFurnace].Name + ". Please wait for the smelting to complete.");
                                client.Player.DragonFurnace += 1000000;
                            }
                            else
                            {
                                dialog.Text("    Please make sure you have 1 " + Pool.ItemsBase[(uint)(4200000 + Option)].Name + ", first.");

                                dialog.Option("Okay.", 255).FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LunarFurnace)]
        public static void LunarFurnace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (client.Map.View.GetAllMapRolesCount(Role.MapObjectType.Item, i => (i as MsgItem).MsgFloor.m_ID == 2285) > 0 || client.Player.DragonFurnace > 0)
            {
                client.CreateBoxDialog("There is already an Anima smelting in the furnace. Please try again during the preparation stage for a new round of smelting.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=With#the#lunar#power,#this#furnace#was#built#to#smelt#Anima.>Lunar Furnace</tip>   \n");
                        dialog.Text("    The lunar energy to fuel this furnace can smelt Anima. Only one of the twin furnaces can perform successful smelting at a time. If you choose the right one, you may get a higher-phase Anima.");

                        dialog.Option("P12~Anima.", 12);
                        dialog.Option("P11~Anima.", 11);
                        dialog.Option("P10~Anima.", 10);
                        dialog.Option("P9~Anima.", 9);
                        dialog.Option("P8~Anima.", 8);
                        dialog.Option("P7~Anima.", 7);
                        dialog.Option("P6~Anima.", 6);
                        dialog.Option("Next.", 13).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=With#the#lunar#power,#this#furnace#was#built#to#smelt#Anima.>Lunar Furnace</tip>   \n");
                        dialog.Text("    The lunar energy to fuel this furnace can smelt Anima. Only one of the twin furnaces can perform successful smelting at a time. If you choose the right one, you may get a higher-phase Anima.");

                        dialog.Option("P5~Anima.", 5);
                        dialog.Option("P4~Anima.", 4);
                        dialog.Option("P3~Anima.", 3);
                        dialog.Option("Tell~me~more~details.", 14);
                        dialog.Option("Previous.", 0).FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        dialog.Text("    It`s said the twin furnaces were built by Gan Jiang and Ou Yezi. They were legendary blacksmiths in ancient China.");
                        dialog.Text("Though thousands of years have passed, the fire in the twin furnaces are still fierce.");

                        dialog.Option("What`s~the~smelting~success~rate?", 15);
                        dialog.Option("Get~details~on~official~site.", 16);
                        dialog.Option("Learn~about~other~things.", 13).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        dialog.Text("    There is a 50% chance of successfully smelting an Anima that may upgrade its phase. Things are not always perfect. There is another 50% chance that it may fail.");

                        dialog.Option("Learn~about~other~things.", 14);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        client.SendSysMesage("https://co.99.com/guide/event/anima_introduction.shtml", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red);
                        break;
                    }
                default:
                    {
                        if (Option >= 3 && Option < 13)//Fuse
                        {
                            if (client.Inventory.Remove((uint)(4200000 + Option), 1, stream))
                            {
                                client.Player.DragonFurnace = (uint)(4200000 + Option);
                                client.CreateBoxDialog("You activated LunarFurnace and put in a " + Pool.ItemsBase[client.Player.DragonFurnace].Name + ". Please wait for the smelting to complete.");
                            }
                            else
                            {
                                dialog.Text("    Please make sure you have 1 " + Pool.ItemsBase[(uint)(4200000 + Option)].Name + ", first.");

                                dialog.Option("Okay.", 255).FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        public static int AnimaPrice(uint AnimaID)
        {
            switch (AnimaID % 100)
            {
                case 1:
                    return 6;
                case 2:
                    return 11;
                case 3:
                    return 21;
                case 4:
                    return 41;
                case 5:
                    return 81;
                case 6:
                    return 161;
                case 7:
                    return 321;
                case 8:
                    return 641;
                case 9:
                    return 1281;
                case 10:
                    return 2561;
                case 11:
                    return 5121;
                case 12:
                    return 10241;
            }

            return 0;
        }

        public static void RuneFragments(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            MsgGameItem item;
            if (client.Inventory.TryGetItem(client.ActiveNpc, out item))
            {
                bool bound = item.ITEM_ID >= 3306369 && item.ITEM_ID <= 3306371;
                if (bound && item.Bound < 1) item.Bound = 1;
                Dialog dialog = new Dialog(client, stream);
                switch (Option)
                {

                    case 0:
                        {
                            dialog.Text("You can combine 20 " + Pool.ItemsBase[item.ITEM_ID].Name + "s into a complete " + Pool.ItemsBase[item.ITEM_ID].Name.Replace("Fragment", "") + ".\n");
                            dialog.Text("If you use this fragment alone, you have a chance of getting 2 fragments.");

                            dialog.Option("Combine~20~fragments.", 1);
                            dialog.Option("Use~1~fragment~for~double.", 2).FinalizeDialog();
                            break;
                        }
                    case 1:
                        {
                            if (client.Inventory.Contain(item.ITEM_ID, 20, item.Bound))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    if (client.Inventory.Remove(item.ITEM_ID, 20, stream))
                                    {
                                        uint ID = 0;
                                        if (Pool.ItemsBase[item.ITEM_ID].Name.Contains("Blue"))
                                        {
                                            var runes = Pool.ItemsBase.Values.Where(i => Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.Garment && i.ID % 100 == 1);
                                            ID = runes.ToArray()[Pool.GetRandom.Next(0, runes.Count())].ID;
                                        }
                                        else if (Pool.ItemsBase[item.ITEM_ID].Name.Contains("Red"))
                                        {
                                            var runes = Pool.ItemsBase.Values.Where(i => Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.RedRune && i.ID % 100 == 1);
                                            ID = runes.ToArray()[Pool.GetRandom.Next(0, runes.Count())].ID;
                                        }
                                        else if (Pool.ItemsBase[item.ITEM_ID].Name.Contains("Yellow"))
                                        {
                                            var runes = Pool.ItemsBase.Values.Where(i => Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.YellowRune && i.ID % 100 == 1);
                                            ID = runes.ToArray()[Pool.GetRandom.Next(0, runes.Count())].ID;
                                        }
                                        client.Inventory.Add(stream, ID, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, item.Bound > 0);
                                        client.SendSysMesage("You have got 1 " + Pool.ItemsBase[ID].Name + (bound ? "(B)." : "."));
                                    }
                                }
                                else client.CreateBoxDialog("Please make sure you have 1 free slot in your inventory.");
                            }
                            else client.CreateBoxDialog("Please make sure you have 20 fragments to make a Rune.");
                            break;
                        }
                    case 2:
                        {
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.RemoveStackItem(item.UID, 1, stream);
                                client.CreateBoxDialog("Failed to combine. The magic of the fragment has vanished, and it disappeared.");
                            }
                            else
                            {
                                client.Inventory.AddItemWitchStack(item.ITEM_ID, 0, 1, stream, item.Bound > 0);
                                client.CreateBoxDialog("Congratulations! You successfully made 2 " + Pool.ItemsBase[item.ITEM_ID].Name + "s!");
                            }
                            break;
                        }

                }
            }
        }

        public static void RareDeitylandChests(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("									*<tip desc=This#delicate#chest#with#vivid#dragon#pattern#contains#valuable#items~crafted~by~the~ancient~deities.#However,#it`s#locked.>Rare Deityland Chest</tip>*\n");
                        dialog.Text("\nÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");
                        dialog.Text("\nRequirement: A <tip color=0xfff05050 desc=The#devils#in#the#Deityland#may#carry#the#Rare#Chest#Key.>Rare Chest Key</tip>\n");
                        dialog.Text("Rewards: A chance to get Chasm Star Stone Pack, Favored Training Pill,\n");
                        dialog.Text("	      +1 Stone, Soul Scroll, etc.\n");
                        dialog.Text("ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");

                        if (client.Inventory.Contain(3303973, 1))
                            dialog.Option("I~own~the~key!", 1);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(3303973, 1, stream))
                        {
                            uint R = (uint)Pool.GetRandom.Next(1, 4);
                            if (R == 1) R = 730001;
                            else if (R == 2) R = 3303480;
                            else if (R == 3) R = 3003124;
                            client.Inventory.Add(stream, R, 1, (byte)(R == 730001 ? 1 : 0));


                            var npc = client.Map.View.GetMapObject<Game.MsgNpc.Npc>(Role.MapObjectType.Npc, client.ActiveNpc);

                            client.Map.RemoveNpc(npc, stream);
                            ushort X = 0, Y = 0;
                            client.Map.GetRandCoord(ref X, ref Y);
                            npc.X = X;
                            npc.Y = Y;
                            client.Map.AddNpc(npc);
                        }
                        break;
                    }
            }
        }
        public static void EpicDeitylandChests(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("									*<tip desc=This#divine#chest#contains#fabulous#treasures#of#the#ancient#deities.#However,#it`s#locked.>Epic Deityland Chest</tip>*\n");
                        dialog.Text("\nÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");
                        dialog.Text("\nRequirement: An <tip color=0xfff05050 desc=The#devils#in#the#Deityland#may#carry#the#Epic#Chest#Key.>Epic Chest Key</tip>\n");
                        dialog.Text("Rewards: A chance to get 3,000 CPs, Frozen Chi Pill Scrap,\n");
                        dialog.Text("	      +3 Stone, P7 Dragon Soul, etc.\n");
                        dialog.Text("ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");


                        if (client.Inventory.Contain(3303974, 1))
                            dialog.Option("I~own~the~key!", 1);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(3303973, 1, stream))
                        {
                            uint R = (uint)Pool.GetRandom.Next(1, 5);
                            if (R == 1) R = 730003;
                            else if (R == 2) R = 3007269;
                            else if (R == 3) R = 3004247;
                            else if (R == 4) { client.Player.ConquerPoints += 3000; break; }
                            client.Inventory.Add(stream, R, 1, (byte)(R == 730003 ? 3 : 0));


                            var npc = client.Map.View.GetMapObject<Game.MsgNpc.Npc>(Role.MapObjectType.Npc, client.ActiveNpc);

                            client.Map.RemoveNpc(npc, stream);
                            ushort X = 0, Y = 0;
                            client.Map.GetRandCoord(ref X, ref Y);
                            npc.X = X;
                            npc.Y = Y;
                            client.Map.AddNpc(npc);
                        }
                        break;
                    }
            }
        }
        public static void VictoryChest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var quest = Database.QuestInfo.GetFinishQuest(21972, client.Player.Class, 3944);
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("									*<tip desc=Heroes#who#have#killed#100#devils#in#the#Deityland#can#unlock#this#chest#and#get#wonderful#rewards.>Victory Chest</tip>*\n");
                        dialog.Text("ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");
                        dialog.Text("\nRequirement: Kill 100 devils in the Deityland, once a day.\n");
                        dialog.Text("Rewards: 1 Bright Star Stone for sure, and a shot to win +2 Stone,\n");
                        dialog.Text("	      Blazing CP Fragment, 150 Chi, etc.\n");
                        dialog.Text("ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");

                        if (client.Player.QuestGUI.CheckObjectives(quest.MissionId, 100) || client.Player.QuestGUI.CheckQuest(quest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            dialog.Option("Unlock~this~chest.", 1);
                        }
                        dialog.Option("I~see.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(quest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            dialog.Text("You`ve already unlocked this chest today. Please try again tomorrow.");

                            dialog.Option("Okay. ", 255).FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.FinishQuest(quest.MissionId))
                            {
                                int R = Pool.GetRandom.Next(1, 4);
                                if (R == 1) client.Inventory.Add(stream, 3008727, 1);
                                else if (R == 2) client.Inventory.Add(stream, 3009001, 1);
                                else client.Inventory.Add(stream, 730002, 1, 2);
                            }
                        }
                        break;
                    }
            }
        }
        public static void DeityAltar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("									*Deityland Sacrifice Ranking*\n");
                        dialog.Text("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        dialog.Text("\n[Rank]    [Jades]  [Character]                [Prize]\n");
                        var array = Pool.DeitylandSacrificeRankings.Where(i => i.Value.Jades >= 30).OrderBy(i => i.Value.Jades).Take(10).ToArray();
                        for (int i = 0; i < array.Length; i++)
                            dialog.Text(" No." + (i + 1) + "       " + array[i].Value.Jades + "         " + array[i].Value.Name + "            " + array[i].Value.Prize + "\n");
                        dialog.Text("\nÃ˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        dialog.Text("\nThe final ranking will be determined at 23:59 every day. The prizes will be sent via mail.\n");
                        dialog.Text("You`ve sacrificed " + (Pool.DeitylandSacrificeRankings.ContainsKey(client.Player.UID) ? Pool.DeitylandSacrificeRankings[client.Player.UID].Jades : 0) + " Jades of Faith. Sacrifice 10 jades to get a reward, and 30 jades to qualify for the ranking.\n");

                        dialog.Option("Submit~a~Jade~of~Faith.", 1);
                        dialog.Option("Rune~transformation.", 2).AddAvatar(1258).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        dialog.Text("									*Rules for Deityland Sacrifice*\n");
                        dialog.Text("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");
                        dialog.Text("1. Killing devils in the Deityland brings you a chance of getting Jades of Faith.\n");
                        dialog.Text("2. Sacrifice 10 Jades of Faith to get a <tip color=0xfff05050 desc=Open#to#randomly#receive#one#from#Yellow#Rune#(B)#x1,#Yellow#Rune#Essence#(B)#x10,#Yellow#Rune#Fragment#(B)#x1,#etc.>Yellow Rune (B) Pack</tip>, and 30 jade to qualify for the ranking.\n");
                        dialog.Text("3. An extra Yellow Rune (B) Pack will be rewarded for entering the top 10 for the first time.\n");
                        dialog.Text("4. An extra prize will be rewarded to the final top 10 heroes announced at 23:59 every day.\n");
                        dialog.Text("5. The Deityland Altar allows Rune transformation between a L1 No Mercy and a L1 Witchery.\n");
                        dialog.Text("Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾Ã˜Å’ÃšÂ¾\n");

                        dialog.Option("Submit~1~Jade~of~Faith.", 5);
                        dialog.Option("Submit~10~Jades~of~Faith.", 6);
                        dialog.Option("Submit~all~Jades~of~Faith.", 255).AddAvatar(1258).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        dialog.Text("We allows Rune transformation between a L1 No Mercy and a L1 Witchery here.\n");
                        dialog.Text("What`s the Rune you want to transform?");

                        dialog.Option("Change~L1~Witchery~to~No~Mercy.", 3);
                        dialog.Option("Change~L1~No~Mercy~to~Witchery.", 4).AddAvatar(1258).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(4031301, 1, stream))
                            client.Inventory.Add(stream, 4031001);
                        else client.CreateBoxDialog("Failed to transform. Please make sure you have a L1 Witchery.");
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(4031001, 1, stream))
                            client.Inventory.Add(stream, 4031301);
                        else client.CreateBoxDialog("Failed to transform. Please make sure you have a L1 No Mercy.");
                        break;
                    }
                case 5:
                case 6:
                case 7:
                    {
                        ushort count = 1;
                        if (Option == 6) count = 10;
                        else if (Option == 7)
                        {
                            var array = client.Inventory.ClientItems.Values.Where(i => i != null && i.ITEM_ID == 3307083);
                            foreach (var item in array)
                                count = (ushort)item.StackSize;
                        }
                        if (client.Inventory.Remove(3307083, count, stream))
                        {
                            if (Pool.DeitylandSacrificeRankings.ContainsKey(client.Player.UID))
                                Pool.DeitylandSacrificeRankings[client.Player.UID].Jades += count;
                            else Pool.DeitylandSacrificeRankings.Add(client.Player.UID, new Pool.DeitylandSacrificeRanking() { Name = client.Player.Name, Jades = count, Prize = "" });
                            DeityAltar(client, stream, 0, "", 0);
                        }
                        else
                        {
                            dialog.Text("It seems you don`t have enough Jades of Faith to sacrifice.");
                            dialog.Text("Go hunting devils in the Deityland to collect more Jades of Faith.");

                            dialog.Option("Alright.", 255).AddAvatar(1258).FinalizeDialog();
                        }
                        break;
                    }
            }

        }
        public static void DeitylandPortals(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("This portal will send you to the supply station. Do you want to use the portal now?");

                        dialog.Option("Yes.", 1);
                        dialog.Option("No.", 255).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        client.Teleport(1009, 1283, 10250);
                        break;
                    }
            }
        }
        public static void HauKan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("<tip desc=Hau#Kan#has#served#in#the#Legion#of#Heroes#for#dozens#of#years.#Even#though#he#is#very#old#now,#");
                        dialog.Text("he#still#wants#to#make#contribution#to#the#hunting#evil,#and#acts#as#the#Logistics#Officer.>");
                        dialog.Text("Hau Kan</tip>:\n");
                        dialog.Text("I`m old, but I`m vigorous. If you need any supplies, find me.\n");
                        dialog.Text("Flowers and herbs in the Deityland have miraculous effects that can help us fight against the evil. If you`ve collected some <tip color=0xfff05050 desc=Collect#Wonder#Herbs#by#killing#devils#in#the#Deityland.>Wonder Herb</tip>,");
                        dialog.Text("~just exchange them for supplies with me.\n");

                        dialog.Option("Exchange~for~supplies.", 1);
                        dialog.Option("Who`s~won~the~super~elimination~prize?", 2).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        break;
                    }
                case 2:
                    {
                        dialog.Text("								*Winner of Super Elimination Prize*\n");
                        dialog.Text("ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");

                        dialog.Text("ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬ÂÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â\n");

                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
            }
        }
        public static void Nezha(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=Nezha#is#a#human-shaped#talisman#created#by#Saint#Taiyi.#He#may#look#lovely,#");
                        dialog.Text("but#he#possesses#stunning#abilities#and#is#out#of#control#sometimes.#His#tricks#make#Saint#Taiyi#loves#");
                        dialog.Text("and#hates#him#the#most.>Nezha</tip>:\n");
                        dialog.Text("My master looks so cute when he is angry. I often decompose his crafts into Relics Crystals, so you know...");
                        dialog.Text("~I`m not making trouble. Relics Crystal is really useful. I can show you if you have a Relic to decompose.");

                        dialog.Option("Decompose~a~Relic.", 1).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Send(stream.ActionCreate(new ActionQuery() { Type = ActionType.OpenDialog, ObjId = client.Player.UID, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), dwParam = DialogCommands.RuneDecompose, PositionX = client.Player.X, PositionY = client.Player.Y }));
                        break;
                    }
            }
        }
        public static void SaintTaiyi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        dialog.Text("<tip color=0xffffff00 desc=Saint#Taiyi#may#not#the#most#powerful#taoist#in#Mt.#Kunlun,#");
                        dialog.Text("but#his#crafting#skill#is#incomparable.#He#has#crafted#countless#talismans#and#beneficial#pills,#");
                        dialog.Text("including#his#favorite#Nezha,#a#human-shape#talisman.#Though#Nezha#is#very#naughty#sometimes,#");
                        dialog.Text("Saint#Taiyi#could#never#hate#him.>Saint Taiyi</tip>:\n");
                        dialog.Text("Do you know the `Relics`? They are rare attribute items crafted by the ancient deities. Fortunately, I`ve acquired this crafting skill.");
                        dialog.Text("~If you have some <tip color=0xfff05050 desc=The#magic#crystal#can#be#used#to#craft#or#repair#Relics.#How#to#obtain:#decompose#Relics#(100%#chance#to#get),#hunting#monsters#in#each#zone,#10x#drop#rate#on#Dragon#Island#devils.>Relic Crystals</tip>,");
                        dialog.Text("~I can help you make a Relic providing more than 3 kinds of attributes.\nBy the way,");
                        dialog.Text("I can give 1 <tip color=0xfff05050 desc=As#the#core#of#Relics,#it#can#be#shaped#into#a#Relic#after#absorbing#energy#from#treasures#like#DBs#and#Meteors.#Once#a#day.>Relic Spirit</tip> for every 50,000 Silver.");

                        dialog.Option("Craft~a~Relic~Spirit.~(500,000~Money)", 1);
                        dialog.Option("Craft~a~Relic.~(100~Relic~Crystals)", 2);
                        dialog.Option("How~to~obtain~the~Relics?", 3).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.Money >= 500000)
                        {
                            if (client.Inventory.Add(stream, 3307142))
                            {
                                client.Player.Money -= 500000;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                            }
                        }
                        else
                        {
                            client.SendSysMesage("You don't have enough Silver.");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3306885, 100))
                        {
                            if (client.Inventory.Add(stream, 3307142, 1))
                            {
                                client.Inventory.RemoveStackItem(3306885, 100, stream);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                            }
                        }
                        else
                        {
                            dialog.Text("It seems you don`t have enough Relic Crystals to craft a Relic.\nYou can collect Relic Crystals by decomposing spare Relics. Hunting monsters in the wild also gives you a chance.");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        dialog.Text("                      *How to Obtain Relics?*\n");
                        dialog.Text("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                        dialog.Text("1. The common devils in the Deityland may carry such a Relic.\n");
                        dialog.Text("2. Killing the Beast/BOSS will definitely bring you such a Relic.\n");
                        dialog.Text("3. Each Relic can be decomposed into several Relic Crystals according to their attached attributes. The Relic Crystals can be used to craft or repair Relics.\n4. When defeated, a hero`s equipped Relic may be dropped.\n");
                        dialog.Text("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");

                        dialog.Option("Attributes~of~Relics.", 4);
                        dialog.Option("Durability~of~Relics.", 5).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.SendSysMesage("http://co.99.com/guide/event/2018/relic/", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red);
                        break;
                    }
                case 5:
                    {
                        dialog.Text("                      *Durability of Relics*\n");
                        dialog.Text("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                        dialog.Text("1. The durability of your equipped Relic will decrease if you were killed.\n");
                        dialog.Text("2. When your Relic is broken, you can use Relic Crystal to repair it within 48 hours.\n");
                        dialog.Text("3. Death will not lead to drop or durability loss of Relics during Guild War, Clan War, Plunder War, or Arena.\n");
                        dialog.Text("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");

                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
            }
        }
        public static void YuKoon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("<tip desc=To#rescue#more#people#from#misery,#Yu#Koon#decided#to#join#the#Legion#of#");
                        dialog.Text("Heroes#as#an#adviser.#His#careful#and#tactical#planning#has#helped#the#legion#won#many#");
                        dialog.Text("battles#against#the#devil#army.>Yo Koon</tip>:\n");
                        dialog.Text("The Deityland hides ancient treasures, but devils may ambush you somewhere. Be careful!");

                        dialog.Option("Return~to~the~Dragon~Island.", 1);
                        dialog.Option("Any~other~tips~for~me?", 2).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(453, 483, 10137);
                        break;
                    }
                case 2:
                    {
                        dialog.Text("							 		*Deityland in Chaos*\n");
                        dialog.Text("--------------------------------------------------------\n");
                        dialog.Text("1. Killing devils in the Deityland will bring you rewards.\nIt may be rare rewards if you have <tip color=0xfff05050 desc=100#Luck#Points#are#added#at#00:00#every#day.#10#Luck#Points#is#consumed#per#rare#treasure#you#get.>Luck Points</tip> while hunting.\n");
                        dialog.Text("   Without Luck Point, you can only get common rewards.\nAt the end of a day, 80% of your remaining Luck will be added to your Luck next day, up to 2.5x points in a day.\n");
                        dialog.Text("2. You`ll be granted 1 stack of <tip color=0xfff05050 desc=Each#Bliss#lasts#for#5#minutes.#When#your#stack#of#Bliss#increases,#your#Bliss#will#be#reset#to#5#minutes.>Bliss</tip> per opponent you defeat, up to 9 stacks.\n");
                        dialog.Text("    The more stacks of your Bliss, the higher drop rate of treasures.\n");
                        dialog.Text("3. Defeating robber Sprite will definitely bring you treasures and also a random 90-sec <tip color=0xfff05050 desc=1.#P-Attack#10000,#M-Attack#30000\n2.#HP#100000\n3.#Break#200%\n4.#Immunity#200%\n5.#Storm#effect>buff</tip>.\n");
                        dialog.Text("4. Defeating the Queen of Evil within 8 minutes or each of the other beasts within 3 minutes\n");
                        dialog.Text("    will bring you extra rewards.\n");
                        dialog.Text("--------------------------------------------------------\n");

                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DragonYuKoon)]
        public static void DragonYuKoon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("<tip desc=To#rescue#more#people#from#misery,#Yu#Koon#decided#to#join#the#Legion#of#");
                        dialog.Text("Heroes#as#an#adviser.#His#careful#and#tactical#planning#has#helped#the#legion#won#many#");
                        dialog.Text("battles#against#the#devil#army.>Yo Koon</tip>:\n");
                        dialog.Text("It`s said the ancient deities sealed the sources of their power in the Deityland after the battle of justice thousands of years ago.");
                        dialog.Text("~However, this land is under attack from the Queen of Evil and her devil army. Hurry and go smash the greedy enemies!\n");
                        dialog.Text("Rare Rewards: 3,000 CPs, Yellow Rune, Relics, etc.\n");
                        dialog.Text("Common Rewards: Rune Fragment, Meteors, Favored Training Pills, etc.\n");

                        dialog.Option("Send~me~to~the~Deityland.", 1);
                        dialog.Option("Return~to~the~Glowtown.", 2);
                        dialog.Option("Any~other~tips~for~me?", 3).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Player.MessageBox("Your kills in the Deity will increase your PK. Penitence Amulet can only be used in the Deityland once every 30 minutes. Are you sure you want to enter the Deityland?", (p) =>
                        {
                            if (client.Player.QuestGUI.CheckQuest(3944, MsgQuestList.QuestListItem.QuestStatus.Available))
                                client.Player.QuestGUI.Accept(Database.QuestInfo.GetFinishQuest(21972, client.Player.Class, 3944), 0);
                            p.Teleport(1014, 1288, 10250);
                            p.CreateBoxDialog("Your kills in the Deity will increase your PK. Penitence Amulet can be used once every 30 minutes.");
                        });

                        break;
                    }
                case 2:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(102, 408, 10137);
                        break;
                    }
                case 3:
                    {
                        dialog.Text("							 		*Deityland in Chaos*\n");
                        dialog.Text("--------------------------------------------------------\n");
                        dialog.Text("1. Killing devils in the Deityland will bring you rewards.\nIt may be rare rewards if you have <tip color=0xfff05050 desc=100#Luck#Points#are#added#at#00:00#every#day.#10#Luck#Points#is#consumed#per#rare#treasure#you#get.>Luck Points</tip> while hunting.\n");
                        dialog.Text("   Without Luck Point, you can only get common rewards.\nAt the end of a day, 80% of your remaining Luck will be added to your Luck next day, up to 2.5x points in a day.\n");
                        dialog.Text("2. You`ll be granted 1 stack of <tip color=0xfff05050 desc=Each#Bliss#lasts#for#5#minutes.#When#your#stack#of#Bliss#increases,#your#Bliss#will#be#reset#to#5#minutes.>Bliss</tip> per opponent you defeat, up to 9 stacks.\n");
                        dialog.Text("    The more stacks of your Bliss, the higher drop rate of treasures.\n");
                        dialog.Text("3. Defeating robber Sprite will definitely bring you treasures and also a random 90-sec <tip color=0xfff05050 desc=1.#P-Attack#10000,#M-Attack#30000\n2.#HP#100000\n3.#Break#200%\n4.#Immunity#200%\n5.#Storm#effect>buff</tip>.\n");
                        dialog.Text("4. Defeating the Queen of Evil within 8 minutes or each of the other beasts within 3 minutes\n");
                        dialog.Text("    will bring you extra rewards.\n");
                        dialog.Text("--------------------------------------------------------\n");

                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
            }
        }
        public static void DeityCobbWind(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var map = Pool.ServerMaps[10250];
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("STR_ID_tTreasureArea[20825][Text111]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text112]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text113]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text114]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text115]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text116]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text117]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text118]@@");

                        if (DateTime.Now.Minute == 5)
                        {
                            dialog.Text("	[Dragon Wraith] has appeared around (163,415) on the Deity Land!\n");
                            dialog.Text("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                        }
                        else if ((DateTime.Now.Hour == 19 || DateTime.Now.Hour == 21) && DateTime.Now.Minute == 10)
                        {
                            dialog.Text("	[Queen of Evil] has appeared at random, central part of Deityland!\n");
                            dialog.Text("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                        }
                        else if (DateTime.Now.Minute == 30)
                        {
                            dialog.Text("	[Chilling Spook] has appeared around (1020,698) on the Deity Land!\n");
                            dialog.Text("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                        }
                        else if (DateTime.Now.Minute == 45)
                        {
                            dialog.Text("	[Nether Tyrant] has appeared at random, central part of Deityland!\n");
                            dialog.Text("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                        }
                        else if (DateTime.Now.Minute == 57)
                        {
                            dialog.Text("	[Bloody Banshee] has appeared around (484,176) in the upper-right part of the Deityland!\n");
                            dialog.Text("Many heroes are heading to fight against the beasts. Hurry and go support them!");
                        }
                        else
                        {
                            if (DateTime.Now.Minute < 5 || DateTime.Now.Minute > 57)
                            {
                                dialog.Text("*Dragon Wraith will appear at " + (DateTime.Now.Hour + (DateTime.Now.Minute > 57 ? 1 : 0)).ToString() + ":05 today around (163,415) in the upper-left part of the Deityland.*\n");
                            }
                            else if ((DateTime.Now.Hour == 19 || DateTime.Now.Hour == 21) && DateTime.Now.Minute < 10)
                            {
                                dialog.Text("*Queen of Evil will appear at " + DateTime.Now.Hour + ":10 today at random, central part of Deityland.*\n");
                            }
                            else if (DateTime.Now.Minute < 30)
                            {
                                dialog.Text("*Chilling Spook will appear at " + DateTime.Now.Hour + ":30 today around (1020,698) in the lower-right part of the Deityland.*\n");
                            }
                            else if (DateTime.Now.Minute < 45)
                            {
                                dialog.Text("*Nether Tyrant will appear at " + DateTime.Now.Hour + ":45 today at random, central part of Deityland.*\n");
                            }
                            else if (DateTime.Now.Minute < 57)
                            {
                                dialog.Text("*Bloody Banshee will appear at " + DateTime.Now.Hour + ":57 today around (484,176) in the upper-right part of the Deityland.*\n");
                            }
                        }
                        if (map.ContainMobID(3971))
                        {
                            dialog.Option("Go~kill~the~Dragon~Wraith!", 2);
                        }
                        if (map.ContainMobID(3970))
                        {
                            dialog.Option("Go~kill~the~QueenOfEvil!", 5);
                        }
                        if (map.ContainMobID(3977))
                        {
                            dialog.Option("Go~kill~the~Chilling~Spook!", 3);

                        }
                        if (map.ContainMobID(3978))
                        {
                            dialog.Option("Go~kill~the~Nether Tyrant!", 6);

                        }
                        if (map.ContainMobID(3976))
                        {
                            dialog.Option("Go~kill~the~Bloody~Banshee!", 4);

                        }

                        dialog.Option("Learn~more~about~these~beasts.", 1).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.Text("STR_ID_tTreasureArea[20825][Text211]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text212]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text213]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text214]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text215]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text216]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text217]@@");
                        dialog.Text("STR_ID_tTreasureArea[20825][Text218]@@");
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (map.ContainMobID(3971))
                        {
                            dialog.Text("Dragon Wraith has not appeared yet. Do you want to go check the possible area, first?");
                            dialog.Option("Go~to~the~upper-left~part.", 12);
                            dialog.Option("Go~right~up~to~Dragon Wraith.~(1~CP)", 22).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (map.ContainMobID(3978))
                        {
                            dialog.Text("[Nether Tyrant] has appeared at random, central part of Deityland!\n");
                            dialog.Option("Go~to~the~upper-left~part.", 18);
                            dialog.Option("Go~right~up~to~Nether Tyrant.~(1~CP)", 28).FinalizeDialog();

                        }

                        break;
                    }
                case 3:
                    {
                        if (map.ContainMobID(3977))
                        {
                            dialog.Text("Chilling Spook has not appeared yet. Do you want to go check the possible area, first?");
                            dialog.Option("Go~to~the~lower-right~part.", 13);
                            dialog.Option("Go~right~up~to~Chilling Spook.~(1~CP)", 23).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (map.ContainMobID(3976))
                        {
                            dialog.Text("Bloody Banshee has not appeared yet. Do you want to go check the possible area, first?");
                            dialog.Option("Go~to~the~upper-right~part.", 14);
                            dialog.Option("Go~right~up~to~Bloody Banshee.~(1~CP)", 24).FinalizeDialog();

                        }

                        break;
                    }
                case 5:
                    {
                        if (map.ContainMobID(3970))
                        {
                            dialog.Text("QueenOfEvil has not appeared yet. Do you want to go check the possible area, first?");
                            dialog.Option("Go~to~the~upper-right~part.", 15);
                            dialog.Option("Go~right~up~to~QueenOfEvil.~(1~CP)", 25).FinalizeDialog();

                        }
                        break;
                    }
                case 12:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(366, 653, 10250);
                        break;
                    }
                case 18:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(872, 1135, 10250);
                        break;

                    }
                case 13:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds

                        client.Teleport(924, 819, 10250);
                        break;
                    }
                case 14:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(471, 295, 10250);
                        break;
                    }
                case 15:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        client.Teleport(640, 621, 10250);
                        break;
                    }
                case 22:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Teleport(163, 415, 10250);
                            client.Player.ConquerPoints -= 1;
                        }
                        else
                        {
                            dialog.Text("You do not have 1 conquer point.");
                            dialog.Option("Aww!", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 23:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Teleport(1020, 698, 10250);
                            client.Player.ConquerPoints -= 1;
                        }
                        else
                        {
                            dialog.Text("You do not have 1 conquer point.");
                            dialog.Option("Aww!", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 28:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Teleport(872, 1135, 10250);
                            client.Player.ConquerPoints -= 1;
                        }
                        else
                        {
                            dialog.Text("You do not have 1 conquer point.");
                            dialog.Option("Aww!", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 24:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Teleport(484, 176, 10250);
                            client.Player.ConquerPoints -= 1;
                        }
                        else
                        {
                            dialog.Text("You do not have 1 conquer point.");
                            dialog.Option("Aww!", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 25:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Teleport(644, 605, 10250);
                            client.Player.ConquerPoints -= 1;
                        }
                        else
                        {
                            dialog.Text("You do not have 1 conquer point.");
                            dialog.Option("Aww!", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GarmentsDealer)]
        public static void GarmentsDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello there, i can give you random garments to look cool.\n");
                        dialog.Text("Wanna proceed? Here are my options.\n");

                        dialog.Option("1-3 star[215 CPs].", 1);
                        dialog.Option("4-5 star[2150 CPs].", 2);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 215)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg5 = 500000; _hg5 > 0; _hg5--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.Garments.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DbGarment;
                                if (Database.CoatStorage.Garments.TryGetValue(aitem.ID, out DbGarment))
                                {
                                    if (DbGarment.Stars < 4)
                                    {
                                        if (DbGarment.Stars == 3 && Role.Core.Rate(65)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.ConquerPoints -= 215;
                                            dialog.Text("Congratulations! You've got a " + DbGarment.Stars + "-star " + aitem.Name + " garment!");

                                            dialog.Option("Try another time.", 1);
                                            dialog.Option("Thanks.", 255).FinalizeDialog();
                                        }
                                        else
                                        {
                                            dialog.Text("Hey hey! you don't have enough space in your inventory!");

                                            dialog.Option("I~see.", 255).FinalizeDialog();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg6 = 500000; _hg6 > 0; _hg6--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.Garments.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DbGarment;
                                if (Database.CoatStorage.Garments.TryGetValue(aitem.ID, out DbGarment))
                                {
                                    if (DbGarment.Stars > 3)
                                    {
                                        if (DbGarment.Stars == 5 && Role.Core.Rate(50)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.ConquerPoints -= 2150;
                                            dialog.Text("Congratulations! You've got a " + DbGarment.Stars + "-star " + aitem.Name + " garment!");

                                            dialog.Option("Try another time.", 2);
                                            dialog.Option("Thanks.", 255).FinalizeDialog();
                                        }
                                        else
                                        {
                                            dialog.Text("Hey hey! you don't have enough space in your inventory!");

                                            dialog.Option("I~see.", 255).FinalizeDialog();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountsDealer)]
        public static void MountsDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello there, i can give you random mounts to look cool.\n");
                        dialog.Text("Wanna proceed? Here are my options.\n");

                        dialog.Option("1-3 star[215 CPs].", 1);
                        dialog.Option("4-5 star[2150 CPs].", 2);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 215)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg7 = 500000; _hg7 > 0; _hg7--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.SteedMounts.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DBMount;
                                if (Database.CoatStorage.Mounts.TryGetValue(aitem.ID, out DBMount))
                                {
                                    if (DBMount.Stars < 4)
                                    {
                                        if (DBMount.Stars == 3 && Role.Core.Rate(65)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.ConquerPoints -= 215;
                                            dialog.Text("Congratulations! You've got a " + DBMount.Stars + "-star " + aitem.Name + " mount!");

                                            dialog.Option("Try another time.", 1);
                                            dialog.Option("Thanks.", 255).FinalizeDialog();
                                        }
                                        else
                                        {
                                            dialog.Text("Hey hey! you don't have enough space in your inventory!");

                                            dialog.Option("I~see.", 255).FinalizeDialog();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            Database.ItemType.DBItem aitem;
                            for (int _hg8 = 500000; _hg8 > 0; _hg8--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var array = Database.ItemType.SteedMounts.Values.ToArray();
                                aitem = array[Pool.GetRandom.Next(0, array.Length)];
                                Database.CoatStorage.StorageItem DBMount;
                                if (Database.CoatStorage.Mounts.TryGetValue(aitem.ID, out DBMount))
                                {
                                    if (DBMount.Stars > 3)
                                    {
                                        if (DBMount.Stars == 5 && Role.Core.Rate(65)) continue;
                                        if (client.Inventory.Add(stream, aitem.ID))
                                        {
                                            client.Player.ConquerPoints -= 2150;
                                            dialog.Text("Congratulations! You've got a " + DBMount.Stars + "-star " + aitem.Name + " mount!");

                                            dialog.Option("Try another time.", 2);
                                            dialog.Option("Thanks.", 255).FinalizeDialog();
                                        }
                                        else
                                        {
                                            dialog.Text("Hey hey! you don't have enough space in your inventory!");

                                            dialog.Option("I~see.", 255).FinalizeDialog();
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Beautician)]
        public static void Beautician(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = MsgServer.DialogCommands.Hairstyle,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CloudBeast)]
        public static void CloudBeast(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            switch (Option)
            {
                case 0:
                    {
                        client.SendSysMesage("You took courage to touch the Cloud Beast, and found the giant creature is quiet and gentle.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralOfDefense)]
        public static void GeneralOfDefense(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        dialog.Text("The battle against beasts in the Realm is quite fierce. We`ve almost grasped the track of the Infernal Phoenix and its 4 evil guards, and need more heroes to join us!\n");
                        dialog.Text("------------------------------------------------------------\n");
                        dialog.Text("[Beast]    [Appear Time]    [Location]    [Elimination Reward]\n");
                        dialog.Text("  <tip color=0xfff05050 desc=Iron Dragon\nSnow Spider\nGhost Devourer\nStorm Troll>4 evil guards</tip>");
                        dialog.Text("       00:00-23:59      Random       ");
                        dialog.Text("<tip color=0xfff05050 desc=Contains#1#Universal#Amulet,#1#Bright#Star#Stone,#and#a#shot#to#win#+3#Stone,#Refinery#Material#Wheel,#Weapon#Soul#Wheel,#etc.>Treasure Chest</tip>\n");
                        dialog.Text("  Infernal Phoenix          22:10        (386,386)     ");
                        dialog.Text("<tip color=0xfff05050 desc=Contains#5#Universal#Amulets,#2#Bright#Star#Stones,#500#Chi#Points,#and#a#shot#to#win#Olive#Jade,#Weapon#Soul#Wheel,#Armor#Soul#Wheel,#etc.>Infernal Phoenix`s Chest</tip>\n");
                        dialog.Text("------------------------------------------------------------\n");
                        dialog.Text("All heroes of the server can claim <tip color=0xfff05050 desc=Unlocking#an#evil#guard`s#chest:\nUniversal#Praise#Pack.\nUnlocking#Infernal#Phoenix`s#Chest:\nUniversal#Joy#Pack.>");
                        dialog.Text("a reward</tip> from the Kingdom Mission Envoy when the chest of the evil guards or Infernal Phoenix is unlocked.\n");
                        dialog.Text("Participating in the elimination may also bring you[<tip color=0xfff05050>+5 Stone</tip>], [<tip color=0xfff05050>Radiant Star Stone</tip>], [<tip color=0xfff05050>500 CPs (B)</tip>], ");
                        dialog.Text("[<tip color=0xfff05050>Hollow Pack</tip>], [<tip color=0xfff05050>special Dragon Soul</tip>], etc.");

                        dialog.Option("I`ll~join.", 255).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Autumn)]
        public static void Autumn(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        dialog.Text("<tip color=0xffffff00 desc=A#son#of#the#Hall#of#Perfection#Founder.#Though#his#delicate#health#prevents#him#from#practicing#martial#arts,");
                        dialog.Text("#Autumn#became#more#concentrated#in#studying#martial#arts#books.#At#20#years#old,#Autumn#rose#to#fame#through#a#big#exam#in#his#clan.");
                        dialog.Text("#Now,#he#is#appointed#to#establish#the#Perfection#Ranking.>Autumn</tip>:\n");
                        dialog.Text("Heroes whose equipped gears are all at least Super +8 with 2 sockets (no socket requirement on Riding Crop and Mount) can register their gears");
                        dialog.Text("~for the Perfection Ranking. However, you`re not qualified yet.");

                        dialog.Option("Perfection~Ranking?", 1);
                        dialog.Option("I`ll~talk~to~you~later.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.Text("The Perfection Ranking system is divided into 11 sections. Each section has a ranking for specific type of gear <tip color=0xfff05050 desc=The#ranking#can#automatically#update");
                        dialog.Text("#when#any#listed#gear#is#improved#on#the#Forging#interface.>with real-time updates</tip>. Honorably, each section`s top-ranking gear will");
                        dialog.Text("~be displayed in the hall with its owner`s name. Heroes, if all your equipped gears have been at least Super +8 with 2 sockets");
                        dialog.Text("~(no socket requirement on Riding Crop and Mount), come and register with me for the Perfection Ranking, up to 5 times every day.\n");

                        dialog.Option("See~you.", 255).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ArtisanZhuRong)]
        public static void ArtisanZhuRong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                       // dialog.Text("<tip color=0xffffff00 desc=He#is#an#outstanding#apprentice#in#the#school#of#the#Magic#Artisan,#");
                       // dialog.Text("and#provides#maintenance#service#for#tempered#gears.");
                        dialog.Text("Artisan Zhu Rong<:\n");
                        dialog.Text("Many friends are reporting that they changed their character name but their tempered gear is still with their old name.");
                        dialog.Text("~I`ve figure out a way to update with the new name. If you have the same problem on a gear that you tempered, just find me.");

                        dialog.Option("Update~the~owner`s~name.", 1).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        foreach (var item in client.Equipment.ClientItems.Values)
                        {
                            if (item.OwnerName != client.Player.Name && client.Player.UID == item.OwnerUID)
                            {
                                item.OwnerName = client.Player.Name;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);
                            }
                        }
                        client.CreateBoxDialog("With the help of Artisan Zhu Rong, the owner`s name displayed on your tempered gear has been updated.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Ambassador)]
        public static void Ambassador(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        dialog.Text("This Privacy Policy describes how we gather, use, ");
                        dialog.Text("manage and protect your personal information, and ");
                        dialog.Text("applies to all of the servers offered by the Product.\n");
                        dialog.Text("Please read Privacy Policy first and choose <Agree> or ");
                        dialog.Text("<Disagree> before gaming.");

                        dialog.Option("Detail", 1);
                        dialog.Option("Leave", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        dialog.AddPage("http://co.99.com/useragreement/");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Loni)]
        public static void Loni(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        dialog.Text("<tip color=0xffffff00 desc=		Loni#is#a#famous#swordsman#and#enjoy#sharing#stories#in#Jiang#Hu.>Loni</tip>:\n");
                        dialog.Text("     As the 16th anniversary approaches, CO has opened a global server.\n");
                        dialog.Text("     I`ve also made a comprehensive Wall of World Prestige listing top heroes and guilds");
                        dialog.Text("~on nobility, flowers received, Prestige Score and other aspects of all servers.\n");

                        dialog.Option("View~the~wall.", 1);
                        dialog.Option("I`ll~talk~to~you~later.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.SendSysMesage("https://wtop.zf.99.com/?lang=en", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red);
                        break;
                    }
            }
        }       
        [NpcAttribute(NpcID.AccessoriesDealer)]
        public static void AccessoriesDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello there, i can give you random accessories to look cool.\n");
                        dialog.Text("Wanna proceed? Here are my options.\n");

                        dialog.Option("Try my luck.[1500 CPs].", 1);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 1500)
                        {
                            for (int _hg9 = 500000; _hg9 > 0; _hg9--)  // [hang-fix] cap to avoid infinite spin on bad data (normal play exits in a few iterations)
                            {
                                var Accessorys = Database.ItemType.Accessorys.Values.ToArray();
                                var rand = (ushort)(Pool.GetRandom.Next() % 1000);
                                int position = Pool.GetRandom.Next(0, Accessorys.Length);

                                if (client.Inventory.Add(stream, Accessorys[position].ID))
                                {
                                    client.Player.ConquerPoints -= 1500;
                                    dialog.Text("Congratulations! You've got a " + Pool.ItemsBase[Accessorys[position].ID].Name + "!");

                                    dialog.Option("Try another time.", 1);
                                    dialog.Option("Thanks.", 255).FinalizeDialog();
                                }
                                else
                                {
                                    dialog.Text("Hey hey! you don't have enough space in your inventory!");

                                    dialog.Option("I~see.", 255).FinalizeDialog();
                                }
                                break;
                            }
                        }
                        else
                        {
                            dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            dialog.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.BeauticianDealer)]
        //public static void BeauticianDealer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.Text("Hello there, i can give you random hair/avatar to look cool.\n");
        //                dialog.Text("Wanna proceed? Here are my options.\n");

        //                dialog.Option("Hairstyle.[1000 CPs].", 1);
        //                dialog.Option("Avatar.[1000 CPs].", 2);
        //                dialog.Option("I~see.", 255).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.ConquerPoints >= 1000)
        //                {
        //                    while (true)
        //                    {
        //                        var vals = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Hairstyle && client.HairfaceStorage.CanEquip(i) && !client.HairfaceStorage.Exists(i.ID, i.Type)).ToArray();
        //                        if (vals.Length > 0)
        //                        {
        //                            var rand = (ushort)(Pool.GetRandom.Next() % 1000);
        //                            int position = Pool.GetRandom.Next(0, vals.Length);

        //                            if (vals[position].Name == "LoveHat") continue;
        //                            if (client.HairfaceStorage.Add(vals[position]))
        //                            {
        //                                client.Player.ConquerPoints -= 1000;
        //                                dialog.Text("Congratulations! You've got " + vals[position].Name + " hairstyle!");

        //                                dialog.Option("Try another time.", 1);
        //                                dialog.Option("Thanks.", 255).FinalizeDialog();
        //                                break;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            dialog.Text("You are not qualified to get any hairstyles.");

        //                            dialog.Option("I~see.", 255).FinalizeDialog();
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Player.ConquerPoints >= 1000)
        //                {
        //                    while (true)
        //                    {
        //                        var vals = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Avatar && client.HairfaceStorage.CanEquip(i) && !client.HairfaceStorage.Exists(i.ID, i.Type)).ToArray();
        //                        if (vals.Length > 0)
        //                        {
        //                            var rand = (ushort)(Pool.GetRandom.Next() % 1000);
        //                            int position = Pool.GetRandom.Next(0, vals.Length);

        //                            if (client.HairfaceStorage.Add(vals[position]))
        //                            {
        //                                client.Player.ConquerPoints -= 1000;
        //                                dialog.Text("Congratulations! You've got " + vals[position].Name + " avatar!");

        //                                dialog.Option("Try another time.", 2);
        //                                dialog.Option("Thanks.", 255).FinalizeDialog();
        //                                break;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            dialog.Text("You are not qualified to get any avatars.");

        //                            dialog.Option("I~see.", 255).FinalizeDialog();
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    dialog.Text("Hey hey! you don't have the required amount of Conquer Points!");

        //                    dialog.Option("I~see.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.HuntingArea)]
        public static void HuntingArea(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello there, i can teleport you to my hunting map.\n");
                        dialog.Text("There, many monsters as fuck, you can collect a lot of Conquer Points by hunting.\n");
                        dialog.Text("In red devil's maps, the monsters are a bit stronger and a less than pheasant's map.\n");

                        dialog.Option("Red~Devils[1000 CPs].", 1);
                        dialog.Option("Pheasants[2000 CPs].", 2);
                        dialog.Option("I~see.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 1000)
                        {
                            client.Player.ConquerPoints -= 1000;
                            client.Teleport(19, 127, 2351);
                            client.CreateBoxDialog("You've been teleported to red devil's map.");
                        }
                        else
                        {
                            dialog.Text("Hey, are you making fun on me? you don't have the required amount of Conquer Points.\n");

                            dialog.Option("My~bad.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 2000)
                        {
                            client.Player.ConquerPoints -= 2000;
                            client.Teleport(19, 127, 2352);
                            client.CreateBoxDialog("You've been teleported to pheasant's map.");
                        }
                        else
                        {
                            dialog.Text("Hey, are you making fun on me? you don't have the required amount of Conquer Points.\n");

                            dialog.Option("My~bad.", 255).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.HellConquer_US)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void HellConquer_US(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ServerInfo = Database.GroupServerList.GetServer(2);
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.OurCoqnuer_EU)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void OurCoqnuer_EU(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
#if Encore
              var ServerInfo = Database.GroupServerList.GetServer(1);
#else
            var ServerInfo = Database.GroupServerList.GetServer(3);
#endif
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.OurCoqnuer_US)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void OurCoqnuer_US(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
#if Encore
              var ServerInfo = Database.GroupServerList.GetServer(2);
#else
            var ServerInfo = Database.GroupServerList.GetServer(4);
#endif
            if (ServerInfo == null)
                return;

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the keeper of [" + ServerInfo.Name + "] server. You`ll find a keeper like me in each server.")
                            .AddText("~If you want to visit another server, you need to register with me. I can also help you")
                            .AddText("~return to your original server. Please carefully learn about the rules before taking the trip.");
                        data.AddOption("Take~me~to~server~[" + ServerInfo.Name + "].", 1)
                            .AddOption("How~to~travel~across~server?", 10)
                            .AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.OnTransform)
                        {
                            client.Player.TransformInfo.FinishTransform();
                        }
                        if (client.Player.ServerID == ServerInfo.ID)
                        {
                            client.Socket.Disconnect();
                        }
                        else
                        {
                            client.Send(stream.ConnectionInfoCreate(1, ServerInfo.ID));
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("If you want to go to another server, talk to the Cross-server Keeper in your server to enter the Realm,")
                            .AddText("~and then register with the Cross-server Keeper of the destination server in the Realm. If return,")
                            .AddText("~you`ll take the same procedures. First, go transit in the Realm. Then, find the Keeper of your original server")
                            .AddText("~to get back. Please make sure you`ve learned all rules and matters before taking a cross-server trip.");
                        data.AddOption("What~should~I~pay~attention~to?", 11)
                           .AddAvatar(7).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("While visiting a new server, your information about character level, rebirth times, equipment, nobility,")
                             .AddText("~mentor and apprentice, and bonus BP of guild and clan will also be registered in the server, however,")
                             .AddText("~only chat and PK functions are available now. The chat channels include Talk, Whisper, Team, Guild,")
                             .AddText("~Clan, Friend, Service, World and Ally. If you were killed in the Realm, you`ll also revive there.");
                        data.AddOption("What~functions~are~unavailable?", 12)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("In the Realm, you`re not able to use the functions related to money, item trade, forging,")
                             .AddText("~social contact, Arena, Mentor gains, welfare, enlightenment, sending flowers and all PvP")
                             .AddText("~matches. Things will get right after I send you back to your original server.");
                        data.AddOption("I~see.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
            }
        }
    }
}

