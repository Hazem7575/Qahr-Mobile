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


        [NpcAttribute(NpcID.SwordMaster)]
        public static void SwordMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip desc=People#has#forgotten#the#real#name#of#the#Swordmaster,#")
                                 .AddText("but#still#remember#he#learned#arts#of#swords#on#Mount#Su.>Swordmaster</tip>:\n")
                                 .AddText("    You must have heard that Weapon Warrior will spark stunning inspirations.\n")
                                 .AddText("    Guardian space is most impressive inspiration. Now you can enter Weapon Warrior and preview diversified Guardian status.\n")
                                  .AddOption("View~Guardian~status.", 1).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.HundredWeapons.Valid() && client.HundredWeapons.Unlocked && client.HundredWeapons.Objects.Values.Count(i => i.Level > 3) > 0)
                        {
                            client.Send(stream.ActionCreate(new ActionQuery() { Type = ActionType.OpenDialog, ObjId = client.Player.UID, Timestamp = (uint)Time32.timeGetTime().GetHashCode(), dwParam = DialogCommands.TrojanWeaponArchiveLook, PositionX = client.Player.X, PositionY = client.Player.Y }));
                        }
                        else
                        {
                            data.AddText("    This Guardian space can be opened only when you have a Weapon Spirit of 3-star or above.\n")
                                .AddText("    If you`re a Trojan reaching Lv.40, you can go find Mr. Pride in the Dragland to unlock Weapon Warrior system.")
                                 .AddOption("I~see.", 255).FinalizeDialog();
                            break;
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.SecurityAnima)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void SecurityAnima(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello, Through Me You Can Activate a Security Code That Protects Your Anima From Theft in Any Way.\n");
                        //dialog.Text("Actived = " + client.Player.TREPIN2 + "");
                        if (!client.Player.TREPIN2 && client.Player.CheckPin())
                        {
                            dialog.AddOption("Actived My Security Code.", 81);
                            dialog.AddOption("Change Security Code.", 10);
                            dialog.AddOption("I Forgot My Security Code.", 5);
                            dialog.AddOption("Remove Security Code.", 50);
                        }
                        else
                            dialog.AddOption("Create Security Code.", 1);
                        dialog.AddOption("I~will~come~later.");
                        dialog.Avatar(3685).FinalizeDialog();

                        break;
                    }
                #region Create Security Code
                case 1: // (Done)
                    {
                        dialog.Text("Please Enter The Security Code You Want.");
                        dialog.AddInput("Write Here:", 2, 6);
                        dialog.Avatar(3685).FinalizeDialog();
                        break;
                    }
                case 2: // (Done)
                    {
                        string pin = Input, PINQ = "";
                        if (pin.Length != 6)
                        {
                            client.Player.MessageBox("Sorry, Must Input 6 Number Or Letters and Not Less Than That.", null, null, 60);
                            goto case 1;
                        }
                        string Get3Numper = pin.Substring(3);
                        for (int x = 0; x < pin.Length - Get3Numper.Length; x++)
                            PINQ += "*";
                        PINQ += Get3Numper;
                        client.Player.OrginalInput2 = Input;
                        dialog.Text("Are You Sure About This Security Code [ " + PINQ + " ]");
                        dialog.Option("Yes, Activate It. ", 3);
                        dialog.Option("No. ", 255);
                        dialog.Avatar(3685).FinalizeDialog();

                        break;
                    }
                case 3: // (Done)
                    {
                        string pin = client.Player.OrginalInput2;
                        if (client.Player.PinCodeAnima.ToLower() == "" && pin.Length == 6 && pin != "")
                        {
                            client.Player.PinCodeAnima = pin.ToLower();
                            client.Player.MessageBox("Congratulations, Your Security Code Has Been Successfully Activated and He Is \n [ " + pin + " ]", null, null, 60);
                        }
                        else
                            goto case 0;
                        break;
                    }
                #endregion
                #region Change Security Code
                case 10: // (Done)
                    {
                        // Start of the change-security-code flow: clear any previous verification.
                        client.PinChangeVerified = false;
                        dialog.AddText("Please Input Your Old Security Code.")
                            .AddInput("Enter Here: ", 11, 6)
                            .AddOption("Just passing by.", 255).Avatar(3685).FinalizeDialog();
                        break;

                    }
                case 12:
                    {
                        // Only reachable after a verified old code (option 11). A client that jumps
                        // straight here gets nothing it can act on, but gate it anyway.
                        if (!client.PinChangeVerified)
                        {
                            client.Player.MessageBox("Please verify your old security code first.", null, null, 60);
                            goto case 0;
                        }
                        dialog.Text("You Have Entered The Old Security Code Correctly. Enter The New Security Code You Want.");
                        dialog.AddInput("Enter Here: ", 13, 6);
                        dialog.AddOption("Just passing by.", 255).Avatar(3685).FinalizeDialog();
                        break;

                    }
                case 13:
                    {
                        // Refuse to overwrite the PIN unless the old code was verified this session.
                        if (!client.PinChangeVerified)
                        {
                            client.PinChangeVerified = false;
                            client.Player.MessageBox("Please verify your old security code first.", null, null, 60);
                            goto case 0;
                        }
                        if (Input.Length == 6 && IsSafeSecret(Input))
                        {
                            // Single-use: consume the verification immediately.
                            client.PinChangeVerified = false;
                            client.Player.PinCodeAnima = Input.ToLower();
                            //client.Player.TREPIN = true;
                            client.Player.MessageBox("Your Security Code Has Been Changed Successfully.", null, null, 60);
                            goto case 0;
                        }
                        else
                        {
                            client.Player.MessageBox("Sorry, Must Input 6 Numbers Or Letters and Not Less Than That.", null, null, 60);
                            goto case 0;
                        }
                    }
                case 11:
                    {
                        client.PinChangeVerified = false;
                        string pin = Input, PINQ = "";
                        if (pin.Length != 6)
                        {
                            client.Player.MessageBox("Sorry, Must Input 6 Number Or Letters and Not Less Than That.", null, null, 60);
                            goto case 10;
                        }
                        string Get3Numper = pin.Substring(3);
                        for (int x = 0; x < pin.Length - Get3Numper.Length; x++)
                            PINQ += "*";
                        PINQ += Get3Numper;
                        client.Player.OrginalInput2 = PINQ;

                        if (pin.Length == 6 && client.Player.PinCodeAnima.ToLower() == pin.ToLower())
                        {
                            // Old code confirmed on this session: allow the write step (option 13).
                            client.PinChangeVerified = true;
                            goto case 12;
                        }
                        else
                        {
                            client.Player.MessageBox("Sorry, Old Security Code incorrect.", null, null, 60);
                            goto case 0;
                        }


                    }
                #endregion
                #region Forget PinCode
                case 5:
                    {
                        dialog.AddText("Please Input Your Email Without Adding @yahoo.com Will Input Automatic.")
                            .AddInput("Enter Here: ", 6)
                            .AddOption("Just passing by.").Avatar(3685).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (Input != "" && Input.Length <= 32 && IsSafeEmailName(Input.Contains("@") ? Input.Split('@')[0] : Input))
                        {
                            string mail = Input.Contains("@") ? "" : "@yahoo.com";
                            client.ChangePassword.Email = Input + mail;
                            bool exist = false;
                            if (AccountExistsByIdAndEmail(client.Player.UID, client.ChangePassword.Email)) // [sqli] parameterized
                            {
                                exist = true;
                            }
                            if (exist)
                            {
                                // SECURITY: never display the security code. Recover by clearing it so the
                                // owner (who proved the email) can set a new one; a session hijacker who
                                // does not know the email cannot read or reset it.
                                client.Player.PinCodeAnima = "";
                                client.Player.TREPIN2 = false;
                                dialog.AddText("Your Security Code has been reset. Please set a new one from the menu.");
                                dialog.AddOption("Okay thanks.", 255).Avatar(3685).FinalizeDialog();
                                // goto case 0;
                            }
                            else
                            {
                                dialog.AddText("Please Input Your Data Again.");
                                dialog.AddOption("Okay.", 255).Avatar(3685).FinalizeDialog();
                                // goto case 5;
                            }
                        }
                        break;
                    }
                #endregion
                #region RemovePinCode
                case 50:
                    {
                        dialog.AddText("Please Input Your Email Without Adding @yahoo.com Will Input Automatic.")
                            .AddInput("Enter Here: ", 60);
                        dialog.AddOption("Just passing by.").Avatar(3685).FinalizeDialog();
                        break;
                    }
                case 60:
                    {
                        if (Input != "" && Input.Length <= 32 && IsSafeEmailName(Input.Contains("@") ? Input.Split('@')[0] : Input))
                        {
                            string mail = Input.Contains("@") ? "" : "@yahoo.com";
                            client.ChangePassword.Email = Input + mail;
                            bool exist = false;
                            if (AccountExistsByIdAndEmail(client.Player.UID, client.ChangePassword.Email)) // [sqli] parameterized
                            {
                                exist = true;
                            }
                            if (exist)
                            {
                                dialog.AddText("Your Security Code Has Been Successfully Cleared.");
                                dialog.AddOption("Okay thanks.", 255);
                                dialog.Avatar(3685).FinalizeDialog();
                                client.Player.PinCodeAnima = "";
                                client.Player.TREPIN2 = false;
                                // goto case 0;
                            }
                            else
                            {
                                dialog.AddText("Please Input Your Data Again.");
                                dialog.AddOption("Okay.", 255).Avatar(3685).FinalizeDialog();
                                // goto case 5;
                            }
                        }
                        break;
                    }
                #endregion
                #region Actived Password
                case 81:
                    {
                        dialog.AddText("Enter Your Security Code.")
                           .AddInput("Enter Here: ", 80, 6)
                           .AddOption("Just passing by.", 255).Avatar(3685).FinalizeDialog();
                        break;
                    }
                case 80:
                    {
                        string pin = Input;
                        if (pin.Length != 6)
                        {
                            client.Player.MessageBox("Sorry, the security code you entered is incorrect. Please try again.", null, null, 60);
                            goto case 0;
                        }
                        if (pin.Length == 6 && client.Player.PinCodeAnima.ToLower() == pin.ToLower())
                        {
                            client.Player.TREPIN2 = true;
                            client.Player.MessageBox("Activation has been completed successfully.", null, null, 60);
                        }
                        else
                            goto case 0;
                        break;
                    }
                #endregion
            }
        }

        [NpcAttribute(NpcID.HeroOfGame)]
        public static void HeroOfGame(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Glad to see you! I take it that you haven`t heard of the HeroOfGame tournament? please allow me to introduce this exciting event for you.")

                   .AddOption("Join", 1)
                   .AddOption("I will come later. ", 255)
                   .AddAvatar(63).FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.HeroOfGame)
                        {
                            if (!Game.MsgTournaments.MsgSchedules.CurrentTournament.Join(client, stream))
                            {
                                data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
            .AddOption("I see. ", 255)
            .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("We are sorry , but u can't enter right now , try when tournament start !")
 .AddOption("I see. ", 255)
 .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.clearinventory)] // The NPC Edit By Mohy ElDin ...
        public static void clearinventory(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Hello, you can use me to clear all items in your inventory For Free. Do you want to clear all items in your inventory now !\n");
                        data.AddOption("Yeap, Clear My Items.", 1);
                        data.AddOption("No, I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        data.AddText("Important note: All items will be deleted from your inventory, regardless of their importance. Do you agree to this and want all items deleted from your inventory !\n");
                        data.AddOption("Yes, Clear All Items.", 2);
                        data.AddOption("No, I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }

                case 2:
                    {
                        data.AddText("One final important note: if anything is deleted from your inventory, you will bear all the consequences, and the game administration has no involvement in this matter.\n");
                        data.AddOption("I Agree, Clear All Items.", 3);
                        data.AddOption("No, I~will~come~later.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }

                case 3:
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                            client.Inventory.Clear(rec.GetStream());
                        data.AddText("All items have been successfully removed from your inventory.\n");
                        data.AddOption("Okay Thanks.", 255).AddAvatar(3685).FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ChangeToken)]
        public static void ChangeToken(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You~Can~Change~GoldPrize~For~This~Item ")
                      .AddOption("Change~GoldPrize", 1)
                        .AddAvatar(288)

                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3001304, 1, 0))
                        {
                            client.Inventory.Remove(3001304, 1, stream);
                            if (client.Inventory.Contain(2100075, 1, 0))
                            {
                                client.Inventory.Remove(2100075, 1, stream);

                                client.Inventory.Add(stream, 2100095, 1);
                            }
                        }
                        else
                        {
                            data.AddText("You~Dont~Have~GoldPrize");
                            data.AddOption("Just passing by..");
                            data.AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.EliteGuildWar)]
        public static void EliteGuildWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }
                        dialog.AddText("[Elite Guild War] Starts Every Hour At Minute x35 and Ended At Minute x40 \n");
                        if (MsgSchedules.EliteGuildWar.Proces == ProcesType.Alive)
                            dialog.Option("Elite Guild War [Started].", 1);
                        else
                            dialog.Option("Elite Guild War [Ended].", 255);
                        dialog.AddOption("Claim~Reward.", 2);
                        dialog.AddOption("Not interested.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1: // (Elite Guild War)
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                        if (MsgSchedules.EliteGuildWar.Proces == ProcesType.Alive)
                        {
                            Random R = new Random();
                            int Nr = R.Next(1, 8);
                            if (Nr == 1) client.Teleport(147, 237, 12816); // Done 1
                            if (Nr == 2) client.Teleport(150, 195, 12816); // Done 2
                            if (Nr == 3) client.Teleport(118, 176, 12816); // Done 3
                            if (Nr == 4) client.Teleport(168, 167, 12816); // Done 4
                            if (Nr == 5) client.Teleport(137, 157, 12816); // Done 5
                            if (Nr == 6) client.Teleport(102, 208, 12816); // Done 6
                            if (Nr == 7) client.Teleport(141, 194, 12816); // Done 7
                        }
                        else
                        {
                            dialog.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
                            dialog.AddOption("Alright.", 255).AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }
                case 2: // (Elite Guild War)
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }
                        if (MsgSchedules.EliteGuildWar.Proces != ProcesType.Alive)
                        {
                            if (MsgSchedules.EliteGuildWar.RoundOwner != null && Role.Instance.Guild.GuildPoll.ContainsKey(MsgSchedules.EliteGuildWar.RoundOwner.GuildID))
                            {
                                if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                                {
                                    if (client.Player.MyGuild.Info.GuildID == MsgSchedules.EliteGuildWar.RoundOwner.GuildID)
                                    {
                                        client.Player.ConquerPoints += 10000; // 10.000 CPs      
                                        MsgSchedules.EliteGuildWar.RoundOwner = null;
                                        Server.SendGlobalPacket(
    new Game.MsgServer.MsgMessage(
        "Congratulations, [ " + client.Player.Name + " ] has won the [Elite Guild War] and received 10.000 CPs!",
        "",
        Game.MsgServer.MsgMessage.MsgColor.red,
        Game.MsgServer.MsgMessage.ChatMode.Center
    ).GetArray(stream)
);

                                    }
                                    else
                                    {
                                        dialog.AddText("Sorry, You Dont Have Any Reward To Claim .").AddOption("Alright.").AddAvatar(3685).FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    dialog.AddText("Only The Guild Leader Can Claim The Reward .").AddOption("Alright.").AddAvatar(3685).FinalizeDialog();
                                }
                            }
                            else
                            {
                                dialog.AddText("Sorry, There Is No Any Rewards Now .").AddOption("Alright.").AddAvatar(3685).FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("Please Come Back When (White Flag War) Is Finished .").AddOption("Alright.").AddAvatar(3685).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        //[NpcAttribute(NpcID.KingsWar)]
        //public static void KingsWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }
        //                dialog.AddText("[Kings War] Starts Every Day \n At Hour <tip color=0xff00fa00 desc=> [ 18.30 ] </tip> and Ends At Hour <tip color=0xff00fa00 desc=> [ 19.00 ] </tip>\n");
        //                if (Game.MsgTournaments.MsgNewKingsWar.Proces == ProcesType.Alive)
        //                    dialog.Option("Kings War [Started].", 11);
        //                else
        //                    dialog.Option("Kings War [Ended].", 255);
        //                dialog.AddOption("Claim~Reward.", 12);
        //                dialog.AddOption("Not interested.", 255).AddAvatar(3685).FinalizeDialog();
        //                break;
        //            }
        //        case 11: // (Kings War)
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }
        //                client.Player.ProtectAttack(5 * 1000); // 5 Seconds

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                if (Game.MsgTournaments.MsgNewKingsWar.Proces == ProcesType.Alive)
        //                {
        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 8);
        //                    if (Nr == 1) client.Teleport(147, 237, 12816); // Done 1
        //                    if (Nr == 2) client.Teleport(150, 195, 12816); // Done 2
        //                    if (Nr == 3) client.Teleport(118, 176, 12816); // Done 3
        //                    if (Nr == 4) client.Teleport(168, 167, 12816); // Done 4
        //                    if (Nr == 5) client.Teleport(137, 157, 12816); // Done 5
        //                    if (Nr == 6) client.Teleport(102, 208, 12816); // Done 6
        //                    if (Nr == 7) client.Teleport(141, 194, 12816); // Done 7
        //                }
        //                else
        //                {
        //                    dialog.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    dialog.AddOption("Alright.", 255).AddAvatar(3685).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 12:
        //            {
        //                if (Game.MsgTournaments.MsgNewKingsWar.Proces == ProcesType.Dead)
        //                {
        //                    if (Game.MsgTournaments.MsgNewKingsWar.RoundOwner != null)
        //                    {
        //                        if (Game.MsgTournaments.MsgNewKingsWar.RoundOwner.UID == client.Player.UID)
        //                        {
        //                            client.Player.PayNobilitySystem.Paid((1), (Role.Instance.Nobility.NobilityRank)(12));
        //                            client.Socket.Disconnect();
        //                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(" Congratulations, [ " + client.Player.Name + " ] He Win Kings War AnD received King/Queen For (1 Day) " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
        //                            Game.MsgTournaments.MsgNewKingsWar.RoundOwner = null;
        //                        }
        //                    }
        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.ReallotPoints)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void ReallotPoints(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello friend. I can help you set your Atributes points fast .. its hard work for you.")
                            .AddOption("Vitality Points.", 5)
                            .AddOption("Agility Points.", 6)
                            .AddOption("Strength Points.", 7)
                            .AddOption("Spirit Points.", 8)
                            .AddOption("I~will~reallot~my~points.", 9)
                            .AddOption("Just~passing~by.", 255)
                            .AddAvatar(6)
                            .FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level >= 70)
                        {
                            if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                            {
                                client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);


                                client.Player.Agility = 0;
                                client.Player.Strength = 0;
                                client.Player.Vitality = 1;
                                client.Player.Spirit = 0;

                                client.CreateBoxDialog("You have successfully reloaded your attribute points.");


                                if (client.Player.Reborn == 0)
                                {
                                    client.Player.Atributes = 0;
                                    Database.DataCore.AtributeStatus.ResetStatsNonReborn(client.Player);
                                    if (Database.AtributesStatus.IsWater(client.Player.Class))
                                    {
                                        if (client.Player.Level > 110)
                                            client.Player.Atributes = (ushort)((client.Player.Level - 110) * 3 + client.Player.ExtraAtributes);
                                    }
                                    else
                                    {
                                        if (client.Player.Level > 120)
                                            client.Player.Atributes = (ushort)((client.Player.Level - 120) * 3 + client.Player.ExtraAtributes);
                                    }
                                }
                                else if (client.Player.Reborn == 1)
                                {
                                    client.Player.Atributes = (ushort)(Pool.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass)
                                        + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                }
                                else
                                {
                                    if (client.Player.SecoundeRebornLevel == 0)
                                        client.Player.SecoundeRebornLevel = 130;
                                    client.Player.Atributes = (ushort)(Pool.RebornInfo.ExtraAtributePoints(client.Player.FirstRebornLevel, client.Player.FirstClass) +
                                        Pool.RebornInfo.ExtraAtributePoints(client.Player.SecoundeRebornLevel, client.Player.SecoundeClass) + 52 + 3 * (client.Player.Level - 15) + client.Player.ExtraAtributes);
                                }
                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                            }
                            else
                            {
                                data.AddText("Sorry, you don't have one DragonBall.")
                   .AddOption("I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~cannot~reallot~your~ability~points~unless~you~are~reborn~and~above~level~70.")
                  .AddOption("I~see", 255)
                  .AddAvatar(67).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Vitality += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else client.CreateBoxDialog("You dont have: " + Input + " Atribute points.");
                        }
                        break;
                    }
                case 2:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Agility += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else client.CreateBoxDialog("You dont have: " + Input + " Atribute points.");
                        }
                        break;
                    }
                case 3:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Strength += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else client.CreateBoxDialog("You dont have: " + Input + " Atribute points.");
                        }
                        break;
                    }
                case 4:
                    {
                        ushort points;
                        if (ushort.TryParse(Input, out points))
                        {
                            if (client.Player.Atributes >= points)
                            {
                                client.Player.Spirit += points;
                                client.Player.Atributes -= points;

                                client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                                client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                                client.Equipment.QueryEquipment(client.Equipment.Alternante, false);

                            }
                            else client.CreateBoxDialog("You dont have: " + Input + " Atribute points.");
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("Enter the Vitality points.").AddInput("Vitality Points.", 1).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Enter the Agility points.").AddInput("Agility Points.", 2).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("Enter the Strength points.").AddInput("Strength Points.", 3).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("Enter the Spirit points.").AddInput("Spirit Points.", 4).AddOption("Just~passing~by.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NobilityDonor)]
        public static void NobilityDonor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            if (Option != 2)
                client.Player.NobilityCps = 0;
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Hello friend. I can help you get on the top faster! Instead of donating gold, you can give me your ConquerPoints and I will automatically do the hard work for you.")
                            .AddInput("Insert amount: CPS.", 1)
                            .AddOption("Just~passing~by.", 255)
                            .AddAvatar(6)
                            .FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        int input;

                        if (int.TryParse(Input, out input))
                        {
                            if (input < 60)
                            {
                                dialog.AddText("You need to input more or atleast 60 CPs..");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                                return;
                            }
                            if (client.Player.ConquerPoints >= input)
                            {
                                client.NpcCpsInput = input;
                                dialog.AddText("Are you sure you want to donate " + input + " CPs? That means I will donate for you " + (((ulong)input) * 1ul) + " gold.");
                                dialog.AddOption("Yes", 2);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                            else
                            {
                                dialog.AddText("You don't have that much CPs. Try again. \nThe exchange ratio is 1:1. That means you give me 1 CPs, I donate for you 1 gold. The Maxmum amount is 6,000,000 CPs.");
                                dialog.AddInput("Insert amount:", 1);
                                dialog.AddOption("No thank you.", 255);
                                dialog.AddAvatar(48).FinalizeDialog();
                            }
                        }
                        else
                        {
                            dialog.AddText("Try again. \nThe exchange ratio is 1:1. That means you give me 1 CPs, I donate for you 1000 gold. The minimum amount is 60 CPs.");
                            dialog.AddInput("Insert amount:", 1);
                            dialog.AddOption("No thank you.", 255);
                            dialog.AddAvatar(48).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= client.NpcCpsInput && client.NpcCpsInput != 0)
                        {
                            int Amount = (int)client.NpcCpsInput;
                            if (Amount < 0)
                                return;
                            client.Player.ConquerPoints -= (uint)client.NpcCpsInput;
                            client.Player.Nobility.Donation += (ulong)client.NpcCpsInput * 1ul;
                            client.Send(stream.NobilityIconCreate(client.Player.Nobility));
                            Pool.NobilityRanking.UpdateRank(client.Player.Nobility);
                            client.NpcCpsInput = 0;
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.JiangMaker)]
        public static void JiangMaker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.MyJiangHu == null)
                        {
                            data.AddText("Sorry But You Don't Have Jiang Yet.");
                            data.AddOption("I'll think about it!", 255);
                            data.FinalizeDialog();
                            return;
                        }

                        data.AddText("Hello I'M Jiang Seller.");
                        data.AddOption("Okay.~I`m~ready~to~open~Jiang!", 100);
                        for (int i = 0; i < client.Player.MyJiangHu.ArrayStages.Length; i++)
                        {
                            var stage = client.Player.MyJiangHu.ArrayStages[i];
                            if (stage.Activate)
                                data.AddOption("Stage [" + (byte)(i + 1) + "]", (byte)(i + 1));

                        }

                        data.AddOption("I'll think about it!", 255);
                        data.FinalizeDialog();
                        break;
                    }
                default:
                    {

                        int mStage = Option;

                        if (mStage >= 1 && mStage <= 9)
                        {
                            foreach (var item in Database.JianHuTable.CultivateStatus[(byte)mStage])
                            {
                                data.AddOption(((Role.Instance.JiangHu.Stage.AtributesType)item).ToString(), (byte)((item * 10) + mStage));
                            }
                            data.FinalizeDialog();
                        }
                        else
                        {

                            if (client.Player.ConquerPoints < 200000)
                            {
                                data.AddText("Sorry But You Don't Have 200,000 Cps ");
                                data.AddOption("I'll think about it!", 255);
                                data.FinalizeDialog();
                                return;
                            }
                            client.Player.ConquerPoints -= 200000;
                            var typex = mStage / 10;
                            mStage = mStage % 10;
                            byte stageno = (byte)Math.Min(9, mStage);
                            byte level = 6;
                            var type = (Role.Instance.JiangHu.Stage.AtributesType)Math.Min(15, typex);
                            if (client.Player.MyJiangHu.ArrayStages.Length >= stageno)
                            {
                                var stage = client.Player.MyJiangHu.ArrayStages[(stageno - 1)];
                                for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                {
                                    client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                    client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                    client.Player.MyJiangHu.Buckupstar.Stage = stageno;
                                    client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(stageno);
                                    client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                    client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                    client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                    client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                    client.Send(stream.JiangHuUpdatesCreate(client.Player.MyJiangHu.FreeCourse, MsgJiangHuUpdates.Action.CreateStar, i, stageno,
                                        client.Player.MyJiangHu.Buckupstar.Star.UID, (byte)client.Player.MyJiangHu.FreeTimeToday,
                                        client.Player.MyJiangHu.RoundBuyPoints, 0, 0));
                                    client.Player.MyJiangHu.ApplayNewStar(client);
                                }
                                if (client.Player.MyJiangHu != null)
                                    client.Player.MyJiangHu.SendStatus(stream, client, client);
                            }

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ChargeJiang)]
        public static void ChargeJiang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.MyJiangHu == null)
                        {
                            data.AddText("Sorry But You Don't Have Jiang Yet.");
                            data.AddOption("I'll think about it!", 255);
                            data.FinalizeDialog();
                            return;
                        }
                        data.AddText("Hello I'M Jiang Seller.");
                        for (int i = 0; i < client.Player.MyJiangHu.ArrayStages.Length; i++)
                        {
                            var stage = client.Player.MyJiangHu.ArrayStages[i];
                            if (stage.Activate)
                                data.AddOption("Stage [" + (byte)(i + 1) + "]", (byte)(i + 1));

                        }
                        data.AddOption("I'll think about it!", 255);
                        data.AddAvatar(258).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        int mStage = Option;
                        if (mStage >= 1 && mStage <= 9)
                        {
                            foreach (var item in Database.JianHuTable.CultivateStatus[(byte)mStage])
                            {
                                data.AddOption(((Role.Instance.JiangHu.Stage.AtributesType)item).ToString(), (byte)((item * 10) + mStage));
                            }
                            data.FinalizeDialog();
                        }
                        else
                        {


                            client.Inventory.Remove(728173, 1, stream);
                            var typex = mStage / 10;
                            mStage = mStage % 10;
                            byte stageno = (byte)Math.Min(9, mStage);
                            byte level = 6;
                            var type = (Role.Instance.JiangHu.Stage.AtributesType)Math.Min(15, typex);
                            if (client.Player.MyJiangHu.ArrayStages.Length >= stageno)
                            {
                                var stage = client.Player.MyJiangHu.ArrayStages[(stageno - 1)];
                                for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                {
                                    client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                    client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                    client.Player.MyJiangHu.Buckupstar.Stage = stageno;
                                    client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(stageno);
                                    client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                    client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                    client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                    client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                    client.Send(stream.JiangHuUpdatesCreate(client.Player.MyJiangHu.FreeCourse, MsgJiangHuUpdates.Action.CreateStar, i, stageno,
                                        client.Player.MyJiangHu.Buckupstar.Star.UID, (byte)client.Player.MyJiangHu.FreeTimeToday,
                                       client.Player.MyJiangHu.RoundBuyPoints, 0, 0));
                                    client.Player.MyJiangHu.ApplayNewStar(client);
                                }
                                if (client.Player.MyJiangHu != null)
                                    client.Player.MyJiangHu.SendStatus(stream, client, client);
                            }

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JiangSuper)]
        public static void JiangSuper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Reborn < 2 || client.Player.Level == 15)
                        {
                            data.AddText("Sorry, you are not in your 2nd-reborns and level 15.");
                            data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
                            return;
                        }
                        if (client.Player.MyJiangHu == null)
                        {
                            data.AddText("Sorry But You Don't Have Jiang Yet.");
                            data.AddOption("I'll think about it!", 255);
                            data.AddAvatar(3497).FinalizeDialog();
                            return;
                        }
                        data.AddText("Hello, Through Me, You Can Unlock Your Jiang (Super Full) For Free");
                        for (int i = 0; i < client.Player.MyJiangHu.ArrayStages.Length; i++)
                        {
                            var stage = client.Player.MyJiangHu.ArrayStages[i];
                            if (stage.Activate)
                                data.AddOption("Stage [" + (byte)(i + 1) + "]", (byte)(i + 1));

                        }
                        data.AddOption("I'll think about it!", 255);
                        data.AddAvatar(3660).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        int mStage = Option;
                        if (mStage >= 1 && mStage <= 9)
                        {
                            foreach (var item in Database.JianHuTable.CultivateStatus[(byte)mStage])
                            {
                                data.AddOption(((Role.Instance.JiangHu.Stage.AtributesType)item).ToString(), (byte)((item * 10) + mStage));
                            }
                            data.AddText("Choose The Attribute You Want From The Options.");
                            data.AddOption("I'll think about it!", 255);
                            data.AddAvatar(3660).FinalizeDialog();
                        }
                        else
                        {


                            var typex = mStage / 10;
                            mStage = mStage % 10;
                            byte stageno = (byte)Math.Min(9, mStage);
                            byte level = 5;
                            var type = (Role.Instance.JiangHu.Stage.AtributesType)Math.Min(15, typex);
                            if (client.Player.MyJiangHu.ArrayStages.Length >= stageno)
                            {
                                var stage = client.Player.MyJiangHu.ArrayStages[(stageno - 1)];
                                for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                {
                                    client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                    client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                    client.Player.MyJiangHu.Buckupstar.Stage = stageno;
                                    client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(stageno);
                                    client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                    client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                    client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                    client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                    client.Send(stream.JiangHuUpdatesCreate(client.Player.MyJiangHu.FreeCourse, MsgJiangHuUpdates.Action.CreateStar, i, stageno,
                                        client.Player.MyJiangHu.Buckupstar.Star.UID, (byte)client.Player.MyJiangHu.FreeTimeToday,
                                       client.Player.MyJiangHu.RoundBuyPoints, 0, 0));
                                    client.Player.MyJiangHu.ApplayNewStar(client);
                                }
                                if (client.Player.MyJiangHu != null)
                                    client.Player.MyJiangHu.SendStatus(stream, client, client);
                            }

                        }
                        break;
                    }
            }
        }



        //[NpcAttribute(NpcID.JiangFull)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void JiangFull(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Reborn < 2 || client.Player.Level == 15)
                        {
                            data.AddText("Sorry, you are not in your 2nd-reborns and level 15.");
                            data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
                            return;
                        }
                        if (client.Player.MyJiangHu == null)
                        {
                            data.AddText("Sorry But You Don't Have Jiang Yet.");
                            data.AddOption("I'll think about it!", 255);
                            data.AddAvatar(3497).FinalizeDialog();
                            return;
                        }
                        data.AddText("Hello, Through Me, You Can Unlock Your Jiang (Epic Full) For Free");
                        for (int i = 0; i < client.Player.MyJiangHu.ArrayStages.Length; i++)
                        {
                            var stage = client.Player.MyJiangHu.ArrayStages[i];
                            if (stage.Activate)
                                data.AddOption("Stage [" + (byte)(i + 1) + "]", (byte)(i + 1));

                        }
                        data.AddOption("I'll think about it!", 255);
                        data.AddAvatar(3660).FinalizeDialog();
                        break;
                    }
                default:
                    {
                        int mStage = Option;
                        if (mStage >= 1 && mStage <= 9)
                        {
                            foreach (var item in Database.JianHuTable.CultivateStatus[(byte)mStage])
                            {
                                data.AddOption(((Role.Instance.JiangHu.Stage.AtributesType)item).ToString(), (byte)((item * 10) + mStage));
                            }
                            data.AddText("Choose The Attribute You Want From The Options.");
                            data.AddOption("I'll think about it!", 255);
                            data.AddAvatar(3660).FinalizeDialog();
                        }
                        else
                        {


                            var typex = mStage / 10;
                            mStage = mStage % 10;
                            byte stageno = (byte)Math.Min(9, mStage);
                            byte level = 6;
                            var type = (Role.Instance.JiangHu.Stage.AtributesType)Math.Min(15, typex);
                            if (client.Player.MyJiangHu.ArrayStages.Length >= stageno)
                            {
                                var stage = client.Player.MyJiangHu.ArrayStages[(stageno - 1)];
                                for (byte i = 1; i < stage.ArrayStars.Length + 1; i++)
                                {
                                    client.Player.MyJiangHu.Buckupstar = new Role.Instance.JiangHu.BackUpStar();
                                    client.Player.MyJiangHu.Buckupstar.PositionStar = i;
                                    client.Player.MyJiangHu.Buckupstar.Stage = stageno;
                                    client.Player.MyJiangHu.Buckupstar.Star = new Role.Instance.JiangHu.Stage.Star(stageno);
                                    client.Player.MyJiangHu.Buckupstar.Star.Activate = true;
                                    client.Player.MyJiangHu.Buckupstar.Star.Level = level;
                                    client.Player.MyJiangHu.Buckupstar.Star.Typ = type;
                                    client.Player.MyJiangHu.Buckupstar.Star.UID = client.Player.MyJiangHu.ValueToRoll(client.Player.MyJiangHu.Buckupstar.Star.Typ, client.Player.MyJiangHu.Buckupstar.Star.Level);
                                    client.Send(stream.JiangHuUpdatesCreate(client.Player.MyJiangHu.FreeCourse, MsgJiangHuUpdates.Action.CreateStar, i, stageno,
                                        client.Player.MyJiangHu.Buckupstar.Star.UID, (byte)client.Player.MyJiangHu.FreeTimeToday,
                                       client.Player.MyJiangHu.RoundBuyPoints, 0, 0));
                                    client.Player.MyJiangHu.ApplayNewStar(client);
                                }
                                if (client.Player.MyJiangHu != null)
                                    client.Player.MyJiangHu.SendStatus(stream, client, client);
                            }

                        }
                        break;
                    }
            }
        }

        //[NpcAttribute(NpcID.ChiLevel99)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void ChiLevel99(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Through me, you can unlock your Chi to Level 99 through the four gates.");
                        data.AddOption("Open Chi Level 99", 1);
                        data.AddOption("I~will~come~later.", 255);
                        data.AddAvatar(3660).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        foreach (var user in client.Player.MyChi)
                        {
                            user.Exp = 6063000;
                        }
                        Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
                        client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Send);
                        break;
                    }
            }
        }

        //[NpcAttribute(NpcID.HelpDeskGame)]
        //public static void HelpDeskGame(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello, How Can I Help You?");
        //                data.AddOption("I Want to Change My Password.", 6);
        //                data.AddOption("I`ll talk to you later.", 255);
        //                data.AddAvatar(112).FinalizeDialog();
        //                break;
        //            }



        //        case 6:
        //            {
        //                data.AddText("Please input Your Account (Username).").AddInput("Username:", 7).AddAvatar(112).FinalizeDialog();
        //                break;
        //            }
        //        case 7:
        //            {
        //                if (Input != "")
        //                {
        //                    client.ChangePassword.Username = Input;
        //                    data.AddText("Please input your (Current Password).").AddInput("Current password: ", 8).AddAvatar(112).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 8:
        //            {
        //                if (Input != "")
        //                {
        //                    client.ChangePassword.Password = Input;
        //                    bool exist = false;
        //                    using (var cmd = new DBFunctionality.MySqlCommand(DBFunctionality.MySqlCommandType.SELECT).Select("accounts").Where("Username", client.ChangePassword.Username).And("Password", client.ChangePassword.Password))
        //                    using (var reader = new DBFunctionality.MySqlReader(cmd))
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            exist = true;
        //                        }
        //                    }
        //                    if (exist)
        //                    {
        //                        data.AddText("Please input Your (New Password).").AddInput("New Password: ", 9).AddAvatar(112).FinalizeDialog();
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Sorry, The data you entered is incorrect Please input your data again.").AddOption("Alright.", 6).AddAvatar(112).FinalizeDialog();
        //                    }
        //                }
        //                break;
        //            }
        //        case 9:
        //            {
        //                if (Input != null)
        //                {
        //                    new DBFunctionality.MySqlCommand(DBFunctionality.MySqlCommandType.UPDATE).Update("accounts").Set("Password", Input).Where("Username", client.ChangePassword.Username).Execute();
        //                    data.AddText("Your Password has been Changed successfully.");
        //                    data.AddOption("Okay thanks.", 255);
        //                    data.AddAvatar(3642).FinalizeDialog();
        //                    String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
        //           Path = "Cache\\Logs\\ChangePassword[NPC]\\",
        //           NewPath = System.IO.Path.Combine(Path, folderN);
        //                    if (!File.Exists(NewPath + folderN))
        //                    {
        //                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
        //                    }
        //                    if (!File.Exists(NewPath + "\\" + DateTime.Now.Day + ".txt"))
        //                    {
        //                        using (System.IO.FileStream fs = System.IO.File.Create(NewPath + "\\" + DateTime.Now.Day + ".txt"))
        //                        {
        //                            fs.Close();
        //                        }
        //                    }
        //                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(NewPath + "\\" + DateTime.Now.Day + ".txt", true))
        //                    {
        //                        file.WriteLine("UserName[Acc] |" + client.Player.Name + "| UIDAccount |" + client.Player.UID + " | IPAccount | " + client.Socket.RemoteIp + " | MacID |" + client.MacAddress + "|  PasswordAccount  [" + client.Player.IsCheckedPass + "] [The Clock] " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
        //                        file.Close();
        //                    }
        //                }
        //                break;
        //            }
        //    }
        //}


        [NpcAttribute(NpcID.Stuff)]
        public static void Stuff(Client.GameClient client, ServerSockets.Packet stream, byte Option, String Input, UInt32 id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello Man Iam NightMareStuff Seller For Free All Select What's Equipment U Want?")
                            .AddOption("Headgear", 1)
                            .AddOption("Armor", 2)
                            .AddOption("Necklace", 3)
                            .AddOption("Weapons", 4)
                            .AddOption("Ring", 5)
                            .AddOption("Boots", 6)
                            .AddOption("Talisman", 7)
                            .AddOption("#Thanks .", 255)
                            .AddAvatar(285)
                            .FinalizeDialog();
                        break;
                    }
                case 7:
                    {

                        break;
                    }




            }
        }


        [NpcAttribute(NpcID.L60UniqueGearPack)]
        public static void L60UniqueGearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.SendSysMesage("Please make 2 more spaces in your inventory.");
                            break;
                        }

                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 410119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 130067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 118067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 561119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 131067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 141067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            data.AddText("What~kind~of~weapon~do~you~want?");
                            data.AddOption("Throwing~Knife.", 1);
                            data.AddOption("Bow.", 2);
                            data.AddOption("Let~me~see...");
                            data.FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            data.AddText("What~kind~of~weapon~do~you~want?");
                            data.AddOption("Katana.", 3);
                            data.AddOption("Scythe.", 4);
                            data.AddOption("Let~me~see...");
                            data.FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {

                                client.Inventory.Add(stream, 610119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 136067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 143067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            data.AddText("What~kind~of~weapon~do~you~want?");
                            data.AddOption("Rapier.", 5);
                            data.AddOption("Pistol.", 6);
                            data.AddOption("Let~me~see...");
                            data.FinalizeDialog();
                        }
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 617119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 138057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 148057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 421129, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 134067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 114067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            if (client.Inventory.Remove(3000588, 1, stream))
                            {
                                client.Inventory.Add(stream, 626119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 101067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 170067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                                client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                            }
                        }
                        break;
                    }
                case 5:
                    {

                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 611119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 101057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 145067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");


                        }
                        break;
                    }
                case 6:
                    {

                        if (client.Inventory.Remove(3000588, 1, stream))
                        {

                            client.Inventory.Add(stream, 612119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 139067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 145067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");

                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 601119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 135067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 123067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 511119, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 135067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 123067, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 613109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 133047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 142037, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(3000588, 1, stream))
                        {
                            client.Inventory.Add(stream, 500109, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 133047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 142037, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            client.SendSysMesage("You~received~a~suit~of~Level~60~Unique~equipment!");
                        }

                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.DukeofHell)]
        public static void DukeofHell(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)20031, client.Player.Class, 3801);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("<tip color=0xEEFFFF00 desc=Duke#of#Hell#was#born#with#the#ability#to#control#fire.#His#birth#was#a#secret,#but#said#a#son#of#the#Flame#Devastator#who#brought#disasters#to#the#land#300#years#ago.");
                        data.AddText("~He#was#formerly#the#right-hand#man#of#the#King#of#Evil,#then#hid#in#the#central#plain#when#the#King#was#defeated.>Duke of Hell</tip>: ");
                        data.AddText("A Windwalker? Just a joke. Your `wise` Elder Power has been a loser.");
                        data.AddText("~So, you think you`re better than the loser? Well, prove yourself to me.");
                        data.AddOption("Go~to~hell!", 2);
                        data.AddAvatar(62).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Duke of Hell has appeared! Defeat him to avenge the deaths of your fellows!");
                        client.Teleport(client.Player.X, client.Player.Y, 44461, Pool.ServerMaps[44461].GenerateDynamicID());
                        Server.AddMapMonster(stream, client.Map, 452373, 336, 193, 2, 2, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        //452373
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ElderPower2)]
        public static void ElderPower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3800);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)20031, client.Player.Class, 3801);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("(The Elder Power sitting there looks like settling to meditation, but you know he will never come back to life.");
                            data.AddText("~Remember, you must kill the Duke of Hell and protect the imperial mausoleum from the crisis. Keep your promise!)");
                            data.AddOption("I~will.", 6);
                            data.AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            goto case 3;
                        }
                        else
                        {
                            data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                            data.AddText(">Elder Power</tip>: ");
                            data.AddText("Humph! If I were not injured earlier, the head of devil would not have a chance to hurt me. gattata1! You came at the right time.");
                            data.AddText("~The head of devil has run into the deep tunnel after being hit by me. (Coughing)... Don`t let the devil escape!");
                            data.AddOption("What~about~you?", 1);
                            data.AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("(Coughing)... Don`t worry about me. Look, I`ll show you how to switch the combat style and help you...");
                        data.AddText("~help you unblock the inner barriers. Promise... You should promise to defend the imperial mausoleum against the devils.");
                        data.AddOption("I~promise.", 2);
                        data.AddAvatar(67).FinalizeDialog();
                        var action = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            Type = ActionType.DrawStory,
                            dwParam = 1010

                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        //client.CreateBoxDialog("You`ve learned how to switch the combat style. Talk to the Elder Power for further instruction.");
                        client.Player.MessageBox("You`ve learned how to switch the combat style. Talk to the Elder Power for further instruction.",
                       new Action<Client.GameClient>(p =>
                       {
                           using (var recBox = new ServerSockets.RecycledPacket())
                               ElderPower2(client, recBox.GetStream(), 3, "", id);
                       }), null);
                        break;
                    }
                case 3:
                    {
                        data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                        data.AddText(">Elder Power</tip>: ");
                        data.AddText("Good. It`s now your duty to defend... defend the honor of... Windwalker school. It may be too heavy for you, but...");
                        data.AddText("~but I`m old, and I can no longer... no longer...  (The Elder spoke more and more quietly until he closed his eyes.)");
                        data.AddOption("No,~no,~my~Elder!", 4);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(client.Player.X, client.Player.Y, 44460);
                        data.AddText("(The Elder Power sitting there looks like settling to meditation, but you know he will never come back to life.");
                        data.AddText("~Remember, you must kill the Duke of Hell and protect the imperial mausoleum from the crisis. Keep your promise!)");
                        data.AddOption("I~will.", 6);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(10090, 334, 197, (uint)NpcID.DukeofHell);
                        break;

                    }
            }
        }
        [NpcAttribute(NpcID.SisterYung2)]
        public static void SisterYung2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3799);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3800);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            {
                                data.AddText("Someone must be in danger. Hurry and check whether you can help!");
                                data.AddOption("Okay.", 255);
                                data.AddAvatar(155).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.AddText("<tip color=0xEEFFFF00 desc=Yung#is#your#lovely#fellow#sister#who#is#not#interested#in#kungfu#but#has#a#gift#for#cooking.");
                                data.AddText("#Many#of#your#fellow#brothers#have#great#affection#for#her.>Sister Yung</tip>: ");
                                data.AddText("Wow, my fellow brother! You killed these devils really fast. I know you won`t let me down.");
                                data.AddOption("Thanks.~Did~you~see~the~Elder?", 1);
                                data.AddAvatar(155).FinalizeDialog();
                            }
                        }
                        else
                            goto case 1;
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Teleport(client.Player.X, client.Player.Y, 44457, Pool.ServerMaps[10088].GenerateDynamicID());
                        data.AddText("<tip color=0xEEFFFF00 desc=Yung#is#your#lovely#fellow#sister#who#is#not#interested#in#kungfu#but#has#a#gift#for#cooking.");
                        data.AddText("#Many#of#your#fellow#brothers#have#great#affection#for#her.>Sister Yung</tip>: ");
                        data.AddText("Sorry, I don`t know that. It was me who told the Elder that some devils were running toward the imperial mausoleum. What can we do now?");
                        data.AddOption("I`ll~go~find~the~Elder.", 2);
                        data.AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(10088, 128, 36, 0);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.SisterYung)]
        public static void SisterYung(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderPower, client.Player.Class, 3798);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3799);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Help! Help, my brother! These devils are quite furious. I`m not a match for them. Ow!");
                            data.AddOption("Don`t~worry.~I`m~here.", 1);
                            data.AddAvatar(155).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("My fellow brother, you should be very careful of them.");
                            data.AddOption("I`ll~take~care~of~them.", 2);
                            data.AddAvatar(155).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        data.AddText("My fellow brother, you should be very careful of them.");
                        data.AddOption("I`ll~take~care~of~them.", 2);
                        data.AddAvatar(155).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(client.Player.X, client.Player.Y, 44456, Pool.ServerMaps[10088].GenerateDynamicID());
                        Server.AddMapMonster(stream, client.Map, 657218, 148, 142, 1, 1, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Server.AddMapMonster(stream, client.Map, 657218, 154, 146, 1, 1, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Server.AddMapMonster(stream, client.Map, 657218, 162, 139, 1, 1, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Server.AddMapMonster(stream, client.Map, 657218, 157, 133, 1, 1, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Server.AddMapMonster(stream, client.Map, 657218, 146, 135, 1, 1, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.ElderPower)]
        public static void ElderPower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindwalkerGuard, client.Player.Class, 3797);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderPower, client.Player.Class, 3798);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                            data.AddText(">Elder Power</tip>: ");
                            data.AddText("" + client.Player.Name + "? So good to have you here. (Coughing)... The devils disgracefully attacked us when the Chief was not in the camp.");
                            data.AddText("~Humph, we`ll never let them go that easily.");
                            data.AddOption("Are~you~hurt?", 1);
                            data.AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("<tip color=0xEEFFFF00 desc=The#old#manager#is#valued#for#his#wisdom#and#holds#an#oversight#role#in#training#young#Windwalkers.#He#may#look#serious,#but#actually#cares#for#the#young.");
                            data.AddText(">Elder Power</tip>: ");
                            data.AddText("TheBest01? So good to have you here. (Coughing)... The devils disgracefully attacked us when the Chief was not in the camp.");
                            data.AddText("~Humph, we`ll never let them go that easily.");
                            data.AddOption("Are~you~hurt?", 1);
                            data.AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {

                        //var action = new ActionQuery()
                        //{
                        //    ObjId = client.Player.UID,
                        //    Type = ActionType.DrawStory,
                        //    dwParam = 1009

                        //};
                        //client.Send(stream.ActionCreate(action));


                        var GM = new ActionQuery();
                        GM.ObjId = client.Player.UID;
                        GM.dwParam = 0;
                        GM.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                        GM.Type = 126;
                        GM.Fascing = 0;
                        GM.PositionX = 0;
                        GM.PositionY = 0;
                        GM.ScheduleTime = 0;
                        GM.RemainTime = 0;
                        GM.Strings = new string[] { "LuaUIMgr_OpenWindow|443" };
                        client.Send(stream.ActionCreate(GM));
                        ActionQuery query = new ActionQuery()
                        {
                            Type = ActionType.DrawStory,
                            ObjId = client.Player.UID,
                            dwParam = 1009,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(query));

                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        data.AddText("Yeah, I could have dodged the attacks, but I failed. It doesn`t matter. Listen, our school was endowed with the honor to guard");
                        data.AddText("~the imperial mausoleum for a hundred years. It`s almost finished, and we can`t jinx it. Wait, did you hear that?");
                        data.AddOption("Someone~is~calling~for~help.", 2);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Teleport(client.Player.X, client.Player.Y, 44455, Pool.ServerMaps[10088].GenerateDynamicID());
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WindwalkerGuard)]
        public static void WindwalkerGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindwalkerGuard, client.Player.Class, 3797);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderPower, client.Player.Class, 3798);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3799);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SisterYung, client.Player.Class, 3800);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)20031, client.Player.Class, 3801);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)20032, client.Player.Class, 3802);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My fellow brother, you`re finally back! The devils suddenly attacked our camp. Many of our fellows");
                        data.AddText("~were... were killed... (sobbing)... Don`t worry about me. Hurry and give others some help!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddOption("I~need~to~check~around~first.", 255);
                            data.AddAvatar(13).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("I`ll~go~now.", 5);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("I`ll~go~now.", 4);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                                {
                                    data.AddOption("I`ll~go~now.", 3);
                                }
                                else
                                {
                                    data.AddOption("I`ll~go~now.", 2);
                                }
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("I`ll~go~now.", 2);
                            else
                                data.AddOption("I`ll~go~now.", 1);
                        }
                        data.AddOption("I~need~to~check~around~first.", 255);
                        data.AddAvatar(13).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.Teleport(359, 312, 44460);
                        client.SendSysMesage("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.");
                        client.Player.QuestGUI.SendAutoPatcher("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.", 10090, 350, 285, (uint)MsgNpc.NpcID.ElderPower2);
                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.SendAutoPatcher("There are too many devil claws here. Try your best to break through and find the Elder Power.", 10089, 28, 89, 0);
                        client.SendSysMesage("There are too many devil claws here. Try your best to break through and find the Elder Power.");
                        client.Teleport(134, 93, 10089);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(191, 156, 44456, Pool.ServerMaps[10088].GenerateDynamicID());
                        break;
                    }
                case 2:
                    {
                        client.Teleport(191, 156, 44455, Pool.ServerMaps[10088].GenerateDynamicID());
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.SendAutoPatcher("Many of Windwalker fellows were killed, and many of devils who attacked the camp were killed. Head to see the Elder Power.", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                            client.Teleport(191, 156, 10088, Pool.ServerMaps[10088].GenerateDynamicID());
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level67EliteHeadgearPack)]
        public static void Level67EliteHeadgearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 1049)
                        {
                            data.AddOption("Elite~War~Coronet.", 1);
                            data.AddOption("Elite~Heart~of~Ocean.", 2);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 2049)
                            {
                                data.AddOption("Elite~Gold~Helmet.", 3);
                                data.AddOption("Elite~Heart~of~Ocean.", 2);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 4049)
                                {
                                    data.AddOption("Elite~Leopard~Hat.", 4);
                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 5049)
                                    {
                                        data.AddOption("Elite~Blaze~Hood.", 5);
                                        data.AddOption("Elite~Heart~of~Ocean.", 2);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 6049)
                                        {
                                            data.AddOption("Elite~Headband~Of~Repentance.", 6);
                                            data.AddOption("Elite~Heart~of~Ocean.", 2);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 7049)
                                            {
                                                data.AddOption("Elite~Calm~Hat.", 7);
                                                data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 8049)
                                                {
                                                    data.AddOption("Elite~WindHood.", 8);
                                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 14049)
                                                {
                                                    data.AddOption("Elite~Shark~Cap.", 9);
                                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Elite~WindWalker~Cap.", 10);
                                                    data.AddOption("Elite~Heart~of~Ocean.", 2);
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 101068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Shark~Cap!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 114068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Shark~Cap!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 148068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~received~an~Elite~Wind~Hood~and~an~EXP~Ball!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 144068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Calm~Hat.");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 143068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Headband~Of~Repentance!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 123068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Blaze~Hood!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 113038, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Leopard~Hat!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 111068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Gold~Helmet!");
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
                            if (client.Inventory.Remove(728245, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 118068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~War~Coronet!");
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
                            client.Inventory.Remove(728245, 1, stream);
                            client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 117068, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Heart~of~Ocean!");
                        }
                        else
                        {
                            client.CreateBoxDialog("Please~prepare~one~slot~in~your~inventory.");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level55EliteWeaponPack)]
        public static void Level55EliteWeaponPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 1049)
                        {
                            data.AddOption("Elite~Cold~Blade.", 1);
                            data.AddOption("Elite~Mirrored~Sword.", 2);
                            data.AddOption("Elite~Battle~Club.", 3);
                            data.AddOption("Others.", 4);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 2049)
                            {
                                data.AddOption("Elite~Octagonal~Shield.", 22);
                                data.AddOption("Others.", 4);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 4049)
                                {
                                    data.AddOption("Elite~Long~Bow.", 23);
                                    data.AddOption("Elite~Solar~Knife.", 24);
                                    data.AddOption("Others.", 4);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 5049)
                                    {
                                        data.AddOption("Elite~Fierce~Scythe.", 25);
                                        data.AddOption("Elite~Night~Katana.", 26);
                                        data.AddOption("Others.", 4);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 6049)
                                        {
                                            data.AddOption("Elite~Beads~Of~Truth.", 27);
                                            data.AddOption("Others.", 4);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 7049)
                                            {
                                                data.AddOption("Elite~Blood~Pistol.", 28);
                                                data.AddOption("Elite~Melody~Rapier.", 29);
                                                data.AddOption("Others.", 4);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 8049)
                                                {
                                                    data.AddOption("Elite~Flame~Nunchaku.", 30);
                                                    data.AddOption("Others.", 4);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 14049)
                                                {
                                                    data.AddOption("Elite~Copper~Backsword.", 31);
                                                    data.AddOption("Elite~Bend~Spear.", 32);
                                                    data.AddOption("Others.", 4);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Elite~WindWalker~Fan.", 33);
                                                    data.AddOption("Others.", 4);
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
                case 33:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 626108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~WindWalker~Fan!");
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 560108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Bend~Spear!");
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 421108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Copper~Backsword!");
                        }
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 617108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~an~Elite~Flame~Nunchaku!");
                        }
                        break;
                    }
                case 29:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 611108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Melody~Rapier.");
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 612108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Blood~Pistol.");
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 610108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Beads~Of~Truth!");
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 601108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Night~Katana!");
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 511108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Fierce~Scythe(B).");
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 613098, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Solar~Knife(B).");
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 500098, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Long~Bow!");
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 900028, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Octagonal~Shield!");
                        }
                        break;
                    }

                case 4:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Elite~Cold~Blade.", 7);
                        data.AddOption("Elite~Mirrored~Sword.", 8);
                        data.AddOption("Elite~Scorpion~Hook.", 9);
                        data.AddOption("Elite~Leopard~Whip.", 10);
                        data.AddOption("Elite~War~Axe.", 11);
                        data.AddOption("Elite~Iron~Hammer.", 12);
                        data.AddOption("Elite~Battle~Club.", 13);
                        data.AddOption("Next.", 5);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Elite~Gold~Scepter.", 14);
                        data.AddOption("Elite~Steel~Dagger.", 15);
                        data.AddOption("Elite~Sharp~Glaive.", 16);
                        data.AddOption("Elite~Destiny~Poleaxe.", 17);
                        data.AddOption("Elite~Giant~Long~Hammer.", 18);
                        data.AddOption("Elite~Bend~Spear.", 19);
                        data.AddOption("Previous.", 4);
                        data.AddOption("Next.", 6);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Elite~Spring~Wand.", 20);
                        data.AddOption("Elite~Star~Halbert.", 21);
                        data.AddOption("Previous.", 5);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 580108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Star~Halbert!");
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 561108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Spring~Wand!");
                        }
                        break;
                    }
                case 19:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 560108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Bend~Spear!");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 540108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Giant~Long~Hammer!");
                        }
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 530108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Destiny~Poleaxe!");
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 510108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Sharp~Glaive!");
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 490108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Steel~Dagger!");
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 481108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Gold~Scepter!");
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 480108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Battle~Club!");
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 460108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Iron~Hammer!");
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 450108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~War~Axe!");
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 440108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Leopard~Whip!");
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 430108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Scorpion~Hook!");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 420108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Mirrored~Sword!");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 410108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Cold~Blade!");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 410108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Cold~Blade!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 420108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Mirrored~Sword!");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(728243, 1, stream))
                        {
                            client.Inventory.Add(stream, 480108, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~an~Elite~Battle~Club!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level52UniqueHeadgearPack)]
        public static void Level52UniqueHeadgearPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Unique~Thread~Earring.", 1);
                        if (client.Player.Class <= 1049)
                        {
                            data.AddOption("Unique~Gold~Coronet.", 2);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 2049)
                            {
                                data.AddOption("Unique~Pearl~Helmet.", 3);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 4049)
                                {
                                    data.AddOption("Unique~Jackal~Hat.", 4);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 5049)
                                    {
                                        data.AddOption("Unique~Demon~Hood.", 5);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 6049)
                                        {
                                            data.AddOption("Unique~Pure~Headband.", 6);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 7049)
                                            {
                                                data.AddOption("Unique~Tranquil~Hat.", 7);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 8049)
                                                {
                                                    data.AddOption("Unique~Mercy~Hood.", 8);
                                                    data.FinalizeDialog();
                                                }
                                                else if (client.Player.Class <= 14049)
                                                {
                                                    data.AddOption("Unique~Lotus~Cap.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Unique~WindWalker~Cap.", 10);
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
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 101047, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~WindWalker~Cap!");
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 117057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Thread~Earring!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 118057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Gold~Coronet!");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 111057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Pearl~Helmet!");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 113027, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Jacka~lHat!");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 123057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Demon~Hood!");
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 143057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Pure~Headband!");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 144057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Tranquil~Hat.");
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 148057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~a~Unique~MercyHood!");
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Remove(728240, 1, stream))
                        {
                            client.Inventory.Add(stream, 114057, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Lotus~Cap!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Level50UniqueWeaponPack)]
        public static void Level50UniqueWeaponPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take a good look and pick one.");
                        if (client.Player.Class <= 2049)
                        {
                            data.AddOption("Unique~Moon~Blade", 1);
                            data.AddOption("Unique~Fang~Sword.", 2);
                            data.AddOption("Unique~Antler~Hook.", 3);
                            data.AddOption("Unique~Twin~Whip.", 4);
                            data.AddOption("Unique~Peace~Axe.", 5);
                            data.AddOption("Unique~Melon~Hammer.", 6);
                            data.AddOption("Unique~War~Club.", 7);
                            data.AddOption("Next.", 10);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Class <= 4049)
                            {
                                data.AddOption("Unique~Qin~Bow.", 20);
                                data.AddOption("Unique~Sparky~Knife.", 21);
                                data.AddOption("Others.", 9);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Class <= 5049)
                                {
                                    data.AddOption("Unique~Fang~Katana.", 22);
                                    data.AddOption("Unique~Evil~Scythe.", 23);
                                    data.AddOption("Others.", 9);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Class <= 6049)
                                    {
                                        data.AddOption("Unique~Jeweled~Prayer~Beads.", 24);
                                        data.AddOption("Others.", 9);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Class <= 7049)
                                        {
                                            data.AddOption("Unique~Gold~Pistol.", 25);
                                            data.AddOption("Unique~Stream~Rapier.", 26);
                                            data.AddOption("Others.", 9);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Class <= 8049)
                                            {
                                                data.AddOption("Unique~Dragon~Nunchaku.", 27);
                                                data.AddOption("Others.", 9);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.Class <= 14049)
                                                {
                                                    data.AddOption("Unique~Iron~Backsword.", 28);
                                                    data.AddOption("Others.", 9);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("Unique~WindWalker~Fan.", 29);
                                                    data.AddOption("Others.", 9);
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
                case 29:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 626088, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~WindWalker~Fan!");
                        }
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 421087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Iron~Backsword!");
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 617097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~a~Unique~Dragon~Nunchaku!");
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 611097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Stream~Rapier.");
                        }
                        break;
                    }
                case 25:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 612097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Gold~Pistol.");
                        }
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 610097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Jeweled~Prayer~Beads!");
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 511097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Evil~Scythe(B)!");
                        }
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 601097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Fang~Katana!");
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 500087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Qin~Bow!");
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 613087, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Sparky~Knife(B)!");
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 410097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Moon~Blade!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 420097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Fang~Sword!");
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 430097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Antler~Hook!");
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 440097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Twin~Whip!");
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 450097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Peace~Axe!");
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 460097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Melon~Hammer!");
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 480097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~War~Club!");
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Unique~Shaolin~Wand.", 17);
                        data.AddOption("Unique~Gold~Halbert.", 18);
                        data.AddOption("Previous.", 10);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        data.AddOption("Unique~Moon~Blade", 1);
                        data.AddOption("Unique~Fang~Sword.", 2);
                        data.AddOption("Unique~Antler~Hook.", 3);
                        data.AddOption("Unique~Twin~Whip.", 4);
                        data.AddOption("Unique~Peace~Axe.", 5);
                        data.AddOption("Unique~Melon~Hammer.", 6);
                        data.AddOption("Unique~War~Club.", 7);
                        data.AddOption("Next.", 10);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Take a good look and pick one.");
                        data.AddOption("Unique~Wish~Scepter.", 11);
                        data.AddOption("Unique~Cao~Dagger.", 12);
                        data.AddOption("Unique~Union~Glaive.", 13);
                        data.AddOption("Unique~Twin~Poleaxe.", 14);
                        data.AddOption("Unique~Great~Long~Hammer.", 15);
                        data.AddOption("Unique~Luck~Spear", 16);
                        data.AddOption("Previous.", 9);
                        data.AddOption("Next.", 8);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 481097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Wish~Scepter!");
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 490097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Cao~Dagger!");
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 510097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Union~Glaive!");

                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 530097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Twin~Poleaxe!");
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 540097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Great~Long~Hammer!");
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 560097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Luck~Spear!");
                        }
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 561097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Shaolin~Wand!");
                        }
                        break;
                    }
                case 18:
                    {
                        if (client.Inventory.Remove(728239, 1, stream))
                        {
                            client.Inventory.Add(stream, 580097, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Gold~Halbert!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Matchmaker)]
        public static void Matchmaker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddOption("I~shall~try~my~best!", 1);
                        data.AddAvatar(2);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddOption("OK.~I~shall~visit~her.", 2);
                        data.AddOption("Just~passing~by.");
                        data.AddAvatar(4);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("You~must~have~a~meteor~tear~and~a~meteor,~then~you~can~divorce.");
                        data.AddOption("Here~you~are.", 3);
                        data.AddOption("I~changed~my~mind.");
                        data.AddAvatar(6);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {

                        data.AddOption("Sorry,~I~cannot~tell~you.");
                        data.AddAvatar(2);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Joe!~You~have~seen~him?~Where~is~he~now?~Is~everything~going~on~well~with~him?");
                        data.AddOption("Do~not~cry.", 5);
                        data.AddOption("Do~not~ruin~the~meteor!");
                        data.AddAvatar(2);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox)]
        public static void TreasureBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox1)]
        public static void TreasureBox1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox2)]
        public static void TreasureBox2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TreasureBox3)]
        public static void TreasureBox3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The treasure box is empty. The bandits just took the treasure and left.");
                        data.AddOption("I~got~a~little~late,~still.");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.YangYun2)]
        public static void YangYun2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 520);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 521);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, here we are! This is the Treasure House of our Yang family.");
                            data.AddOption("Show~me~the~Tower~Splinter.", 1);
                            data.AddOption("It`s~a~nice~place.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("I really want to help you and the Tang family, but the Tower Splinter I kept has been stolen.");
                                data.AddOption("What~should~I~do~next?", 6);
                                data.AddOption("Alright.");
                            }
                            else
                            {
                                data.AddText("I really want to help you and the Tang family, but the Tower Splinter I kept has been stolen.");
                                data.AddOption("What~should~I~do~next?", 6);
                                data.AddOption("Alright.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies. I just");
                            data.AddText("~happened to have one of the Tower Splinters. Maybe it`s the key to the ending the feud between the two clans.");
                            data.AddOption("Could~be.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 6:
                    {
                        data.AddText("I`m afraid you should go find the Tower Splinters around. If you have any problem, just find me on Bird Island. Good luck!");
                        data.AddOption("Bye!", 3);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Map.View.GetAllMapRolesCount(Role.MapObjectType.Monster, p => p.Alive == true) == 0)
                        {
                            data.AddText("I can`t believe it! They`ve stolen the Tower piece!! Please, help me get it back!");
                            data.AddOption("The~thieves~will~pay~dearly.", 2);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Geez! Where had all these bandits come from? Hero, I need you to teach them a lesson! They should stay away from my collections and the Tower Splinter.");
                            data.AddOption("I`ll~take~care~of~it.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("I`m afraid you should go find the Tower Splinters around. If you have any problem, just find me on Bird Island. Good luck!");
                            data.AddOption("Bye!", 3);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(733, 518, 1015);
                        client.CreateBoxDialog("You~are~on~Bird~Island,~now.");
                        break;
                    }
            }
        }

        //
        [NpcAttribute(NpcID.NetPeddler)]
        public static void NetPeddler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The Ruan Brothers` hut is just under those stone doors. They`re crazy rich, but I`m not so jealous.");
                            data.AddText("~I don`t care if I`m poor. Selling fishing supplies is a decent trade, and safer, too!");
                            data.AddOption("Do~you~sell~Fishing~Nets?", 1);
                            data.AddOption("As~long~as~you`re~happy.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Do you know, that Ruan Brothers have a hut under those stone doors. They`re crazy rich, but I`m not so jealous.");
                            data.AddText("~I don`t care if I`m poor. Selling fishing supplies is a decent trade, and safer, too!");
                            data.AddOption("As~long~as~you`re~happy.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Of course! You don`t need to be a genius to fish, but you will need a good net. I might have a spare around here, somewhere.");
                        data.AddOption("One~Fishing~Net,~please.", 2);
                        data.AddOption("I`ll~be~back~soon.");
                        data.AddAvatar(118);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("Make sure you bring enough money with you! It`s just 10,000 Silver!");
                            data.AddOption("Alright.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Player.Money -= 10000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, 721779, 1);
                                client.CreateBoxDialog("You~bought~a~Fishing~Net~with~10,000~Silver.~Hurry~and~use~it~around~the~water~(303,380).");

                            }
                            else
                            {
                                data.AddText("Sorry. Your inventory is full. You need to make some room.");
                                data.AddOption("Oh.~No~problem.");
                                data.AddAvatar(118);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YeSheng)]
        public static void YeSheng(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("There are quite a number of mysteries, down here. You can tell a lot from this single Iron Poise.");
                            data.AddOption("What`s~the~price?", 1);

                            data.AddOption("That~is~fake,~maybe.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There are quite a number of mysteries, down here. You can tell a lot from this single Iron Poise.");
                            data.AddOption("That~is~fake,~maybe.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Come on, now! I`m selling this Iron Poise at a cut-throat price! Three fish for one! Hey, you know how to");
                        data.AddText("~get fish? Go buy a Fishing Net from the Net Peddler (321,371) and catch fish near the Island. Good luck!");
                        data.AddOption("I`ve~got~your~fish.", 2);
                        data.AddOption("I~hope,~too.");
                        data.AddAvatar(77);
                        data.FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721778, 3))
                        {
                            if (client.Inventory.Remove(721778, 3, stream))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    client.Inventory.Add(stream, 721780, 1);
                                    client.SendSysMesage("You received the Iron Poise!");
                                    data.AddText("Wow, the fish you brought is wonderful! Take this Iron Poise! You can use it to remove the spell at the stone doors (344,403),");
                                    data.AddText("~then the path to the underground chamber will be open ahead of you.");
                                    data.AddOption("I~see.");
                                    data.AddAvatar(77);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Your inventory is full! Get rid of some stuff and you can come back for the Iron Poise.");
                                    data.AddOption("I`ll~be~back~soon.");
                                    data.AddAvatar(77);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("Are you kidding? You don`t have any fish with you! Go buy a Fishing Net from the Net Peddler (189,183)");
                            data.AddText("~and catch fish near the water. Remember, I want THREE fish for one Iron Poise!");
                            data.AddOption("Got~it.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        //
        [NpcAttribute(NpcID.Plum)]
        public static void Plum(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.RuanBrother, client.Player.Class, 524);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.Plum, client.Player.Class, 525);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Days before, a group of bandits in the Desert were killed, overnight. Nobody knows who did it or for what. So, what are you doing here?");
                            data.AddOption("Do~you~know~the~Star~Tower?", 1);
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Take the Tower piece and leave. Anything can happen in these troubled times.");
                                data.AddOption("You~bet.");
                                data.AddAvatar(118);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Actually, the Star Tower has been broken into three pieces. It`s said one of the pieces was brought to the Desert. Just days before, a group of bandits");
                                data.AddText("~in Desert were killed overnight, there.");
                                data.AddOption("Where~can~I~find~it?", 5);
                                data.AddOption("I`d~better~return~home.");
                                data.AddAvatar(118);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("False friends are worse than bitter enemies.");
                            data.AddOption("You~think?");
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        data.AddText("As I know, the third Tower Splinter is in the deep of Desert. Go to (166,226) and see what you can find out there. Be careful, though. The long-standing war has");
                        data.AddText("~ruined the landscape, leaving only homeless villagers and savage bandits in its wake...");
                        data.AddOption("I`ll~be~careful.", 3);
                        data.AddAvatar(118);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Star Tower? Yeah, but I should tell you the road ahead will be long and dangerous. Are you sure you want to continue?");
                        data.AddOption("I~can`t~be~sure~more.", 2);
                        data.AddAvatar(118);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("As I know, the third Tower Splinter is in the deep of Desert. Go to (166,226) and see what you can find out there. Be careful, though. The long-standing war has");
                            data.AddText("~ruined the landscape, leaving only homeless villagers and savage bandits in its wake...");
                            data.AddOption("I`ll~be~careful.", 3);
                            data.AddAvatar(118);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Head~to~the~deep~of~Desert~at~(166,226)~to~start~an~investigation.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RuanBrother)]
        public static void RuanBrother(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.TimeDoor, client.Player.Class, 523);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.RuanBrother, client.Player.Class, 524);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Hey, you are still too green! Keep leveling up and come back when you reach Level 100.");
                            data.AddOption("Alright.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hey, who are you?! How can you arrive here?");
                                data.AddOption("I~came~for~the~Tower~Splinter.", 1);
                                data.AddOption("Sorry,~I~got~lost.");
                                data.AddAvatar(18);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("What are you doing here? You`d better have something important to bother me.");
                                    data.AddOption("I~lost~the~Tower~Splinter.", 9);
                                    data.AddOption("Sorry,~bye!");
                                    data.AddAvatar(18);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Don`t you see I`m busy, now? You`d better have something important to bother me.");
                                    data.AddOption("I~brought~the~Shawn`s~Spear.", 5);
                                    data.AddOption("Sorry,~bye!");
                                    data.AddAvatar(18);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("What are you doing here?");
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(18);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(721777, 1))
                        {
                            data.AddText("Show some manners! You`ve already received the piece.");
                            data.AddOption("My~apologies.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I-I-I-I! Where did you lose the Tower Splinter? Do you know how important that is? Fortunately, the one I gave you is a fake one. I don`t mean to");
                            data.AddText("~cheat you. See, this is the real one. I promise this one is true.");
                            data.AddOption("I~hope~you~said~the~true~word.", 3);
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 5:
                    {
                        data.AddText("Yeah, this is that spear I want. I`m old. You`ve already made the revenge for me. So I don`t need this Tower Splinter");
                        data.AddText("~to remind me of the past. Come, take it.");
                        data.AddOption("Great.", 3);
                        data.AddAvatar(18);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        data.AddText("Tower Splinter? Did Frost Tang send you here? She is my diva, the apple of my eye, my precious little sweety...But all this was shattered by Bloody");
                        data.AddText("~Shawn. He always tried to keep us apart. If you could snatch away his spear and give it to me, I can give you the piece I have.");
                        data.AddOption("I`ve~got~it~right~here.", 2);
                        data.AddOption("I`ll~think~it~over.");
                        data.AddAvatar(18);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(721781, 1))
                        {
                            client.Inventory.Remove(721781, 1, stream);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("Yeah, this is that spear I want. I`m old. You`ve already made the revenge for me. So I don`t need this Tower Splinter");
                                data.AddText("~to remind me of the past. Come, take it.");
                                data.AddOption("Great.", 3);
                                data.AddAvatar(18);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Come on, I mean the spear that Bloody Shawn holds, not any others. Go take his spear, and I`ll consider to show you the splinter.");
                            data.AddText("~Oh, wait! Did I mention where he is? Sorry! You can probably find him in the northwest of Bird Island. Good luck!");
                            data.AddOption("Got~it.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721777, 1);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.CreateBoxDialog("You~received~the~Tower~Splinter!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. Why not tidy it up, first?");
                            data.AddOption("I`ll~be~back~soon.");
                            data.AddAvatar(18);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RuanGoodMan)]
        public static void RuanGoodMan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My brother is STILL single, you know. He`s already in his forties! Nobody knows why. It`s weird.");
                        data.AddOption("Go~back~to~Bird~Island.", 1);
                        data.AddAvatar(48);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(723, 573, 1015);
                        client.CreateBoxDialog("You~returned~to~Bird~Island.");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.RuanBetterMan)]
        public static void RuanBetterMan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Alas... My brother had a duel with the Bloody Shawn days before, and lost again. Yeah, I mean that ugly beast in the northwest of Bird Island.");
                        data.AddOption("Weak~men~will~always~lose.");
                        data.AddAvatar(25);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DreamDoor)]
        public static void DreamDoor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Take these stone doors seriously. They are cursed and can only be opened with the Iron Poise.");
                        data.AddOption("Okay.", 1);
                        data.AddOption("I`ll~pass.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721780, 1))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3) == false)
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                    client.SendSysMesage("The stone door has opened!");
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        client.Inventory.Remove(721780, 1, stream);
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        client.Inventory.Remove(721780, 1, stream);
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Take these stone doors seriously. They are cursed, and can only be opened with the Iron Poise owned by Ye Sheng (192,212) on Bird Island.");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Don`t be mesmerized by the mystic radiance around here. Be quick. The door has opened.");
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SeasonDoor)]
        public static void SeasonDoor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("These stone doors are all glowing with a strange aura. You can only break them with the Iron Poise.");
                        data.AddOption("Okay.", 1);
                        data.AddOption("I`ll~pass.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721780, 1))
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 2) == false)
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                    client.SendSysMesage("The stone door has opened!");
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        client.Inventory.Remove(721780, 1, stream);
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        client.Inventory.Remove(721780, 1, stream);
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Take these stone doors seriously. They are cursed, and can only be opened with the Iron Poise owned by Ye Sheng (192,212) on Bird Island.");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Don`t be mesmerized by the mystic radiance around here. Be quick. The door has opened.");
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.TimeDoor)]
        public static void TimeDoor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("These stone doors are cursed. They can only be opened with the Iron Poise, owned by Ye Sheng (192,212) on Bird Island.");
                        data.AddOption("I`ll~use~it~now.", 1);
                        data.AddOption("Alright.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(721780, 1))
                            {

                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                                {
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                    client.SendSysMesage("The stone door has opened!");
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        client.Inventory.Remove(721780, 1, stream);
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 3))
                                    {
                                        client.Inventory.Remove(721780, 1, stream);
                                        client.CreateBoxDialog("You`ve~opened~all~the~stone~doors~and~found~a~path~leading~underground.~Move~on~and~find~out~more.");
                                    }
                                    data.AddText("The Iron Poise released a flash of light and broke the stone door apart. Go crush the other doors!");
                                    data.AddOption("Great!");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Take these stone doors seriously. They are cursed, and can only be opened with the Iron Poise owned by Ye Sheng (192,212) on Bird Island.");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Don`t be mesmerized by the mystic radiance around here. Be quick. The door has opened.");
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YangYun1)]
        public static void YangYun1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 520);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 521);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.YangYun1, client.Player.Class, 522);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.Plum, client.Player.Class, 525);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.YangYun1, client.Player.Class, 526);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Hey, you are still too green! Keep leveling up and come back when you reach Level 100.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("I`ve rechecked the Treasure House, but found nothing useful. How about you?");
                                data.AddOption("Something~strange~happened.", 20);
                                data.AddOption("No~good~news.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies.");
                                data.AddText("~If I can find all the Tower Splinters, maybe I can end the feud between the two clans. What`s your opinion?");
                                data.AddOption("I~heard~you~have~one~piece.", 1);
                                data.AddOption("Could~be.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("How is my Treasure House? So luxurious, right? It used to have countless treasure and collections, including the Tower Splinter.");
                                    data.AddOption("Where~is~the~Tower~Splinter?", 10);
                                    data.AddOption("Not~bad.");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                                else
                                {

                                    data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies.");
                                    data.AddText("~If I can find all the Tower Splinters, maybe I can end the feud between the two clans. What`s your opinion?");
                                    data.AddOption("I~heard~you~have~one~piece.", 4);
                                    data.AddOption("Could~be.");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {

                                data.AddText("Hello, buddy! Did you find my Tower Splinter? No? So, what can I do for you?");
                                data.AddOption("Where~are~other~splinters?", 13);
                                data.AddOption("I`m~just~passing~by.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();

                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("I`ve shown the handkerchief to my brothers. Actually, that belongs to our Yang family. I don`t know how to explain it to you.");
                                    data.AddOption("Just~tell~me~all.", 23);
                                    data.AddOption("Then~forget~it.~Bye!");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("I`ve shown the handkerchief to my brothers. Actually, that belongs to our Yang family. I don`t know how to explain it to you.");
                                    data.AddOption("Just~tell~me~all.", 23);
                                    data.AddOption("Then~forget~it.~Bye!");
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Years ago, the defensive talisman, Star Tower, was broken into three pieces. For that, our Yang family and the Tang family became enemies. I just");
                                data.AddText("~happened to have one of the Tower Splinters. Maybe it`s the key to the ending the feud between the two clans.");
                                data.AddOption("Could~be.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }

                case 23:
                    {
                        data.AddText("But where did you get the Bud Handkerchief? You see the character `Yang` embroidered on it? This belongs to Yang Feng,");
                        data.AddText("~the old head of the Yang family. I`d be willing to take you there, but I`m keeping the handkerchief. Sound good?");
                        data.AddOption("Alright,~let`s~go.", 22);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(721782, 1))
                        {
                            data.AddText("What? You have one piece of the Star Tower, but it disappeared and became a handkerchief? It`s incredible! Could you show me that Bud Handkerchief?");
                            data.AddOption("Okay.~Here~you~are.", 21);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I have ever seen the third Tower Splinter in the deep of Desert. Why not return the place where the Desert Condors live? Maybe you can");
                            data.AddText("~find something special that you didn`t noticed last time.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(721782, 1))
                        {
                            client.Inventory.Remove(721782, 1, stream);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("But where did you get the Bud Handkerchief? You see the character `Yang` embroidered on it? This belongs to Yang Feng,");
                                data.AddText("~the old head of the Yang family. I`d be willing to take you there, but I`m keeping the handkerchief. Sound good?");
                                data.AddOption("Alright,~let`s~go.", 22);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I have ever seen the third Tower Splinter in the deep of Desert. Why not return the place where the Desert Condors live? Maybe you can");
                            data.AddText("~find something special that you didn`t noticed last time.");
                            data.AddOption("Alright.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 22:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Teleport(52, 50, 1783);
                        client.CreateBoxDialog("You~arrived~at~the~Treasure~House,~once~again.~Be~careful!~You~sense~trouble~in~the~air.");
                        break;
                    }
                case 13:
                    {
                        data.AddText("I`m not sure, but someone told me that the second piece is in the Hut in the north of Bird Island. That Hut belongs to the Ruan Brothers.");
                        data.AddText("~Watch out for them. They`re a little off.");
                        data.AddOption("Got~it.", 12);
                        data.AddOption("Maybe~I~should~give~up.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("You knew that, didn`t you? The Tower Splinter was stolen by those bandits! And you`ve promised to investigate that for me, right?");
                        data.AddOption("Sure.~I`ll.", 11);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("I`m not sure, but someone told me that the second piece is in the Hut in the north of Bird Island. That Hut belongs to the Ruan Brothers.");
                            data.AddText("~Watch out for them. They`re a little off.");
                            data.AddOption("Got~it.", 12);
                            data.AddOption("Maybe~I~should~give~up.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You~were~advised~to~find~the~Ruan~Brother~in~the~Hut,~in~the~north~of~Bird~Island.");
                        break;
                    }
                case 1:
                    {
                        data.AddText("Um... Yeah, I have one of the pieces. How did you know? You must have pulled quite a few strings to hear");
                        data.AddText("~about that. Well, I guess I`d better tell you where the piece is, before you wreck the place. It`s in the Treasure House.");
                        data.AddOption("I`m~doing~this~for~Sugar~Tang.", 2);
                        data.AddOption("I`m~just~passing~by.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Sugar Tang? So you are trying to end the feud, right? In that case, I`ll take you to the House myself! Thank you for your help.");
                        data.AddOption("Just~doing~my~job.", 3);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Teleport(52, 50, 1782);
                        client.CreateBoxDialog("You~arrived~at~the~Treasure~House,~but~only~found~it`s~been~ransacked~by~bandits.~Talk~about~that~with~Yang~Yun~soon.");
                        break;
                    }
                case 4:
                    {
                        data.AddText("Um... Yeah, I have one of the pieces. How did you know? You must have pulled quite a few strings to hear");
                        data.AddText("~about that. Well, I guess I`d better tell you where the piece is, before you wreck the place. It`s in the Treasure House.");
                        data.AddOption("I`m~doing~this~for~Sugar~Tang.", 5);
                        data.AddOption("I`m~just~passing~by.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Sugar Tang? So you are trying to end the feud, right? In that case, I`ll take you to the House myself! Thank you for your help.");
                        data.AddOption("Just~doing~my~job.", 6);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Teleport(52, 50, 1782);
                        client.CreateBoxDialog("You~arrived~at~the~Treasure~House,~but~only~found~it`s~been~ransacked~by~bandits.~Talk~about~that~with~Yang~Yun~soon.");

                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.GhoulKong)]
        public static void GhoulKong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.YangYun1, client.Player.Class, 526);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GhoulKong, client.Player.Class, 527);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("If only I were 10 years younger...");
                            data.AddOption("I~have~some~questions.", 1);
                            data.AddOption("In~your~dreams.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {

                            data.AddText("If only I were 10 years younger...");
                            data.AddOption("I~want~to~ask~you~something.", 6);
                            data.AddOption("In~your~dreams.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("If only I were 10 years younger...");
                            data.AddOption("In~your~dreams.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        data.AddText("Well, according to your map, you are gonna meet quite a number of obstacles. A good beginning is half done. I shall");
                        data.AddText("~escort you there. Anyway, that`s all an old man can do...");
                        data.AddOption("Thank~you.", 5);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("You found the right person. I`m just the most knowledgeable man, but the dungeon is very complicated. Without a map, no one knows where to get");
                        data.AddText("~in or how to get out. So, what is your question?");
                        data.AddOption("I~got~a~Weird~Invocation.", 3);
                        data.AddOption("How~to~smelt~the~Star~Tower?", 2);
                        data.AddOption("Eh...~I~forgot.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("The Star Tower was left by Frost Tang. It is said that it takes 4 clinkers, smelt from 4 weapons. They are the Glitter Sword,");
                        data.AddText("~Annatto Blade, Wasp Spear, and Infernal Axe. With those and the three Tower pieces together, you can restore it in the Cauldro");
                        data.AddOption("There~is~another~thing.", 1);
                        data.AddOption("Unbelievable.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }

                case 3:
                    {
                        if (client.Inventory.Contain(721786, 1))
                        {
                            data.AddText("Oh my! This isn`t an invocation! It`s the map to the Dungeon! The Dungeon, I`m telling you, is chock full of treasure...");
                            data.AddText("~But it`s also full of hidden dangers and fierce monsters...");
                            data.AddOption("No~pain,~no~gain.", 4);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Maybe Yang Yun has the map to Dungeon. What can an old man like me do? I am useless...");
                            data.AddOption("I`m~glad~you~said~it.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            // client.Inventory.Remove(721786, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Well, according to your map, you are gonna meet quite a number of obstacles. A good beginning is half done. I shall");
                            data.AddText("~escort you there. Anyway, that`s all an old man can do...");
                            data.AddOption("Thank~you.", 5);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(31, 89, 1785);
                        client.CreateBoxDialog("The~path~has~lead~you~to~Dungeon~1F.");
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.Cauldron)]
        public static void Cauldron(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GhoulKong, client.Player.Class, 527);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)Game.MsgNpc.NpcID.GhoulKong, client.Player.Class, 528);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Everyone wants to be immortal, but what is the meaning, if you are alone in the world. What do you say?");
                            data.AddOption("I~want~to~smelt~some~items.", 1);
                            data.AddOption("You`re~right.");
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Everyone wants to be immortal, but what is the meaning, if you are alone in the world. What do you say?");
                            data.AddOption("How~is~the~smelting~going?", 12);
                            data.AddOption("Indeed.");
                            data.FinalizeDialog();

                        }
                        else
                        {
                            if (client.Inventory.Contain(721785, 1))
                            {
                                data.AddText("You know you`ve done this before, right? You can only do it once. Happiness lies in contentment, you know.");
                                data.AddOption("Pity.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Love built on beauty fades all too quickly. Pity, that Frost Tang was never happy... To restore the Tower, you need to collect");
                                data.AddText("~Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe, smelt them and meld 4 clinkers with the 3 pieces in the Cauldron. So, how may I help you?");
                                data.AddOption("Smelt~weapons~into~clinkers.", 10);
                                data.AddOption("Restore~the~Star~Tower.", 4);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddOption("Bird~Island,~please.", 2);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 12:
                    {
                        data.AddText("Now that the Star Tower has been restored, you should leave this dangerous place right away. Hurry up and return to Bird Island for Sugar Tang.");
                        data.AddOption("Thanks,~bye!", 7);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721785, 1))
                        {
                            data.AddText("You know you`ve done this before, right? You can only do it once. Happiness lies in contentment, you know.");
                            data.AddOption("Pity.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Love built on beauty fades all too quickly. Pity, that Frost Tang was never happy... To restore the Tower, you need to collect");
                            data.AddText("~Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe, smelt them and meld 4 clinkers with the 3 pieces in the Cauldron. So, how may I help you?");
                            data.AddOption("Smelt~weapons~into~clinkers.", 10);
                            data.AddOption("Restore~the~Star~Tower.", 4);
                            data.AddOption("I`ll~come~back~later.");
                            data.AddOption("Bird~Island,~please.", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(721784, 1))
                        {
                            if (client.Inventory.Contain(721788, 1))
                            {
                                if (client.Inventory.Contain(721789, 1))
                                {
                                    if (client.Inventory.Contain(721790, 1))
                                    {
                                        client.Inventory.Remove(721784, 1, stream);
                                        client.Inventory.Remove(721788, 1, stream);
                                        client.Inventory.Remove(721789, 1, stream);
                                        client.Inventory.Remove(721790, 1, stream);

                                        client.Inventory.Add(stream, 721791, 1);
                                        client.Inventory.Add(stream, 721792, 1);
                                        client.Inventory.Add(stream, 721793, 1);
                                        client.Inventory.Add(stream, 721794, 1);
                                        data.AddText("You`ve got the Fire Clinker, Earth Clinker, Earth Clinker, and Wood Clinker.");
                                        data.AddOption("Great!");
                                    }
                                    else
                                    {
                                        data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                                        data.AddOption("It~will~be~done.");
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                                    data.AddOption("It~will~be~done.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                                data.AddOption("It~will~be~done.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Beat the generals and bring the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe here.");
                            data.AddOption("It~will~be~done.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(721785, 1))
                        {
                            data.AddText("You know you`ve done this before, right? You can only do it once. Happiness lies in contentment, you know.");
                            data.AddOption("Pity.");
                            data.FinalizeDialog();
                        }
                        else
                        {

                            if (client.Inventory.Contain(721791, 1))
                            {
                                if (client.Inventory.Contain(721792, 1))
                                {
                                    if (client.Inventory.Contain(721793, 1))
                                    {
                                        if (client.Inventory.Contain(721794, 1))
                                        {
                                            if (client.Inventory.Contain(721777, 3))
                                            {
                                                data.AddText("It takes a combination of the five elements of fire, earth, water, metal, and wood to restore the talisman.");
                                                data.AddText("~Wait a moment. The Star Tower is almost completely restored.");
                                                data.AddOption("Great!", 6);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddText("You will have all three Tower Splinters after defeating the ghost of Yang Feng. Don`t stop until he`s dead again!");
                                                data.AddOption("Got~it!");
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                            data.AddOption("Okay.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                        data.AddOption("Okay.");
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                    data.AddOption("Okay.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("You need to smelt the Glitter Sword, Annatto Blade, Wasp Spear, and Infernal Axe into clinkers.");
                                data.AddOption("Okay.");
                                data.FinalizeDialog();
                            }

                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(721777, 3))
                        {

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Inventory.Remove(721791, 1, stream);
                                client.Inventory.Remove(721792, 1, stream);
                                client.Inventory.Remove(721793, 1, stream);
                                client.Inventory.Remove(721794, 1, stream);
                                client.Inventory.Remove(721777, 3, stream);
                                client.Inventory.Add(stream, 721785);
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("The time is up, and the Cauldron opened. Fortunately, the Star Tower appeared. And you received 100 Silver!");
                                data.AddText("Now that the Star Tower has been restored, you should leave this dangerous place right away. Hurry up and return to Bird Island for Sugar Tang.");
                                data.AddOption("Thanks,~bye!", 7);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You will have all three Tower Splinters after defeating the ghost of Yang Feng. Don`t stop until he`s dead again!");
                            data.AddOption("Got~it!");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        client.CreateBoxDialog("Report~back~to~Sugar~Tang~with~the~restored~Star~Tower.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Teleport(717, 577, 1015);
                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to return to Bird Island, now?");
                        data.AddOption("Yes,~please.", 3);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("You`re~on~the~way~to~Bird~Island,~now.");
                        client.Teleport(717, 577, 1015);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BlockStone)]
        public static void BlockStone(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The entrance has been blocked! You need to kill 20 Vicious Rats, so you`ll be able to get out of here!");
                        data.AddOption("Damned!");
                        data.FinalizeDialog();
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.KunlunWanderer)]
        public static void KunlunWanderer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 403);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KunlunWanderer, client.Player.Class, 404);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KunlunWanderer, client.Player.Class, 405);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You must have heard about the Kunlun Stone! Guess what I found out? The stone is the Poison Fang of the Viper!");
                            data.AddText("~When dissolved in the Kunlun Liquid, it can be used to dye Tang Silk. Haha, I just happened to have the liquid.");
                            data.AddOption("I~have~the~Tang~Silk.", 1);
                            data.AddOption("Sounds~fantastic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Hey, glad to see you alive! How is your adventure in the Viper Cave? You know, to dye the Tang Silk, you should have the Sacred Water!");
                                data.AddText("~That`s made from the Poison Fang and the Kunlun Liquid.");
                                data.AddOption("Now~what~should~I~do?", 5);
                                data.AddOption("Got~it.");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("As I mentioned before, you should have the Sacred Water composed by the Kunlun Liquid and the Poison Fang to dye the Tang Silk. I`ll give you the");
                                data.AddText("~Kunlun Liquid, if you can get the Poison Fang from the Viper in the Viper Cave. So, you are going to the Viper Cave?");
                                data.AddOption("Yeah,~but~how~to~get~there?", 4);
                                data.AddOption("Viper?~I`ll~be~killed!");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Finally, we have discovered the secret of the Tang Silk! But it seems another mystery has emerged...");
                            data.AddText("~Things are far from over.");
                            data.AddOption("I`ve~dyed~the~Tang~Silk.", 10);
                            data.AddOption("Sounds~fantastic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            data.AddText("Someone must have told you something about the Kunlun Stone. But you know what it is? It`s just the Posion Fang of the Viper!");
                            data.AddText("~Combined with the Kunlun Liquid, it can be used to dye Tang Silk.");
                            data.AddOption("Sounds~fantastic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("Then, what are you doing here? Shouldn`t you send the Poem Silk to Villager Chou, now?");
                        data.AddOption("I`m~on~my~way.", 9);
                        data.AddOption("I~lost~the~Poem~Silk.", 8);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            data.AddText("Oh, Sacred Water...It can be used to dye Tang Silk... It`s said a mysterious secret will appear right away!");
                            data.AddOption("Yeah...", 7);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721804, 1))
                            {
                                data.AddText("Oh, Sacred Water...It can be used to dye Tang Silk... It`s said a mysterious secret will appear right away!");
                                data.AddOption("Yeah...");
                                data.AddAvatar(52);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Inventory.Contain(721803, 1))
                                {
                                    if (client.Inventory.Contain(721802, 1))
                                    {
                                        data.AddText("Now, merge the Poison Fang with the Kunlun Liquid. That`s how Sacred Water is made. Do it, now!");
                                        data.AddOption("Don`t~talk~while~I`m~pouring.");
                                        data.AddAvatar(52);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("So, you`ve obtained the Poison Fang? Great! Here is the Kunlun Liquid. After dissolving the fang into the liquid, you`ll receive the Sacred Water.");
                                        data.AddOption("Thanks.", 6);
                                        data.AddAvatar(52);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Look, I can send you to the Viper Cave if you pay me 10,000 Silver. I`ll be glad to share the Kunlun Liquid");
                                    data.AddText("~when you obtain the Poison Fang. So, you want to explore the Viper Cave?");
                                    data.AddOption("Yep!", 3);
                                    data.AddAvatar(52);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Then, what are you doing here? Shouldn`t you send the Poem Silk to Villager Chou, now?");
                            data.AddOption("I`m~on~my~way.", 9);
                            data.AddOption("I~lost~the~Poem~Silk.", 8);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        client.CreateBoxDialog("You~received~the~Poem~Silk.~Now,~hurry~to~find~Villager~Chou.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            data.AddText("Stop kidding me! I just saw the Poem Silk in your inventory. Hurry to take it to Villager Chou!");
                            data.AddOption("Sorry,~bye!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 721801, 1);
                                client.SendSysMesage("You received the Poem Silk.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full! You need to clean some room, first.");
                                data.AddOption("I`ll~be~back~soon!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721802, 1);
                            client.SendSysMesage("You obtained the Kunlun Liquid. Right click the Poison Fang to combine them into Sacred Water.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! You need to clean some room, first.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("Look, I can send you to the Viper Cave if you pay me 10,000 Silver. I`ll be glad to share the Kunlun Liquid");
                        data.AddText("~when you obtain the Poison Fang. So, you want to explore the Viper Cave?");
                        data.AddOption("Yep!", 3);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721798, 1))
                        {
                            data.AddText("Do you mean the Tang Silk owned by Villager Chou?");
                            data.AddOption("Yeah,~he~gave~me~the~silk.", 2);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Without Tang Silk, everything will be in vain. Find Villager Chou in Maple Forest (788,456) now. I know his grandpa quite well.");
                            data.AddOption("I`ll~visit~him,~now.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Look, I can send you to the Viper Cave if you pay me 10,000 Silver. I`ll be glad to share the Kunlun Liquid");
                            data.AddText("~when you obtain the Poison Fang. So, you want to explore the Viper Cave?");
                            data.AddOption("Yep!", 3);
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Money < 10000)
                        {
                            data.AddText("Come on! 10,000 silvers wouldn`t be so hard for you. Make sure you bring enough money with you!");
                            data.AddOption("Alright.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.Money -= 10000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Teleport(135, 103, 1794);
                            client.SendSysMesage("You arrived at Viper Cave. Now kill the Viper to get the Poison Fang.");
                            client.SendSysMesage("Watch out! Rocks are falling down!!");
                        }
                        break;

                    }
            }
        }


        [NpcAttribute(NpcID.HerbalistChou)]
        public static void HerbalistChou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 407);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("It`s a long long story...Two of my friends loved each other so much... And they wrote a book together to show their love.");
                            data.AddOption("Is~that~the~Poem~Book?", 1);
                            data.AddOption("It`s~so~romantic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("It`s a long long story...Two of my friends loved each other so much... And they wrote a book together to show their love.");
                            data.AddOption("It`s~so~romantic.");
                            data.AddAvatar(52);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("You come for the Poem Book? Aha! Finally, there is someone who know the story. It`s a long long story...They loved each other so much... Sorry,");
                        data.AddText("~but I can`t help, but talking that. The Poem Book was written by Frost Tang and Mr. Loneliness together.");
                        data.AddOption("Where~is~the~book,~now?", 2);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("The Poem Book is in the Rose Garden, but it`s hard to get through the 4 stone pillars that Mr. Loneliness has set in Bird Island. The keys");
                        data.AddText("~are 4 special items: Purple Eye, Jade Sceptre, Jadeite, and Hairpin. You need to find them and put them into the pillars to");
                        data.AddText("~open the Teleport Platform to the Rose Garden. The first two have been stolen by the Crimson Viper and Jade Courtesan! Clear?");
                        data.AddOption("I~know~my~duty~now.", 3);
                        data.AddAvatar(52);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        Server.AddMapMonster(stream, client.Map, 2169, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        Server.AddMapMonster(stream, client.Map, 2170, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");

                        client.SendSysMesage("Crimson Viper and Crimson Claws have shown up! Watch out!");

                        var npc = client.Player.View.Roles(Role.MapObjectType.Npc).Where(p => p.UID == id).First();
                        if (npc != null)
                            client.Map.RemoveNpc(npc as Npc, stream);

                        break;
                    }
            }
        }
        //
        [NpcAttribute(NpcID.JadeCourtesan)]
        public static void JadeCourtesan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ah, I really do have a gift for steal! So easy for me to get the Jade Scepter! But I still need the Purple Eye, Jadeite, and Hairpin");
                            data.AddText("~to unlock the stone pillars on Bird Island.");
                            data.AddOption("Give~me~the~Jade~Scepter!", 1);
                            data.AddOption("Steal?~Shame~on~you!");
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Have you ever heard the story of Mr. Loneliness and Frost Tang? They loved each other so much... And they wrote a book together to show their love.");
                            data.AddOption("It`s~so~romantic.");
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            data.AddText("Who...are...you...? I...");
                            data.AddOption("Important?", 2);
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You came for my Jade Sceptre? You must be dreaming! Time to show me what you are made of! Let`s go!");
                            data.AddOption("I`ll~let~you~know.", 3);
                            data.AddAvatar(145);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        var action = new ActionQuery()
                        {
                            ObjId = id,
                            Type = ActionType.RemoveEntity
                        };
                        unsafe
                        {
                            client.Send(stream.ActionCreate(action));
                        }
                        Server.AddMapMonster(stream, client.Map, 2171, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.Contain(710217, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 710217);
                                client.SendSysMesage("You received the Jade Sceptre!");
                                if (client.Inventory.Contain(710216, 1))
                                {
                                    if (client.Inventory.Contain(710217, 1))
                                    {
                                        if (client.Inventory.Contain(710218, 1))
                                        {
                                            if (client.Inventory.Contain(710219, 1))
                                            {
                                                client.SendSysMesage("You`ve collected the Purple Eye, Jade Scepter, Jadeite, and Hairpin. Go to open the Teleport Platform on Bird Island (655,693).");
                                            }
                                            else
                                            {
                                                client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                            }
                                        }
                                        else
                                        {
                                            client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                        }
                                    }
                                    else
                                    {
                                        client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                    }
                                }
                                else
                                {
                                    client.SendSysMesage("As long as you collect the Purple Eye, Jade Sceptre, Jadeite, and Hairpin, you can open the Platform (Bird Island 655,693) and go to the Rose Garden.");
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Your inventory is full!");
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.LeopardPillar)]
        public static void LeopardPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1))
                        {
                            data.AddText("The Leopard Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            client.Inventory.Remove(710219, 1, stream);
                            client.Player.AddMapEffect(stream, 698, 731, "accession");
                            client.SendSysMesage("You used the Hairpin.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                            data.AddText("The Leopard Pillar is activated!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TigerPillar)]
        public static void TigerPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1))
                        {
                            data.AddText("The Tiger Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            client.Inventory.Remove(710218, 1, stream);
                            client.SendSysMesage("You used the Jadeite.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                            client.Player.AddMapEffect(stream, 694, 735, "accession1");
                            data.AddText("The Tiger Pillar is activated!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DragonPillar)]
        public static void DragonPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                        {
                            data.AddText("The Dragon Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            client.Inventory.Remove(710217, 1, stream);
                            client.SendSysMesage("You used the Jade Sceptre.");
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                            client.Player.AddMapEffect(stream, 694, 721, "accession1");

                            data.AddText("The Dragon Pillar is activated!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SparrowPillar)]
        public static void SparrowPillar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Activate~this~pillar.", 1);
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("This stone pillar looks mysterious...");
                            data.AddOption("Yeah...");
                            data.FinalizeDialog();
                        }
                        break;

                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                        {
                            data.AddText("The Sparrow Pillar has been activated. Now, go activate the other pillars.");
                            data.AddOption("Alright.");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There`s a hole in the pillar. It seems that you can put something in it.");
                            data.AddOption("Put~in~Purple~Eye.", 5);
                            data.AddOption("Put~in~Jade~Sceptre.", 4);
                            data.AddOption("Put~in~Jadeite.", 3);
                            data.AddOption("Put~in~Hairpin.", 2);
                            data.AddOption("Not~now.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            client.Player.AddMapEffect(stream, 683, 730, "accession1");
                            client.Inventory.Remove(710216, 1, stream);
                            client.SendSysMesage("You used the Purple Eye.");

                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                            data.AddText("The Sparrow Pillar is activated!");
                            data.AddOption("Great!");
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(710217, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("The stone pillar is not activated! Guess you didn`t put in the right item! C`mon, try again!");
                            data.AddOption("Damned~pillar!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have the special items to put into the pillars. Maybe Sugar Tang in Bird Island (685,599) can help you.");
                            data.AddOption("I`ll~visit~her.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //
        [NpcAttribute(NpcID.MrLoneliness)]
        public static void MrLoneliness(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Inventory.Contain(710221, 1))
                                {
                                    data.AddText("Yeah, the Heaven Fan is mine. How did you find it? You came for the poem book, right? So many people have asked");
                                    data.AddText("~me about it, but you`re the only one who gave me the Heaven Fan. So, here`s the book. Take it to Sugar Tang, and be gone!");
                                    data.AddOption("With~pleasure!", 1);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You`re the 101st guy to come for the Poem Book. But, I won`t give it to you if you can`t get me the Heaven Fan.");
                                    data.AddOption("Heaven~Fan...");
                                    data.AddOption("Bird~Island,~please!", 2);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {

                                data.AddText("Blech! Look what evil you have brought out! Shouldn`t you sweep them out of my house?");
                                data.AddOption("Oh,~my!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You`re the 101st guy to come for the Poem Book. But, I won`t give it to you if you can`t get me the Heaven Fan.");
                            data.AddOption("Heaven~Fan...");
                            data.AddOption("Bird~Island,~please!", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Teleport(717, 577, 1015);
                        client.SendSysMesage("You`ve been sent back to Bird Island.");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(710221, 1))
                        {
                            client.Inventory.Remove(710221, 1, stream);
                            client.Inventory.Add(stream, 710222);
                            client.SendSysMesage("The Heaven Fan has been taken away. You obtained the Poem Book.");
                            client.Teleport(717, 577, 1015);
                            client.SendSysMesage("You`ve been sent back to Bird Island. Find Sugar Tang to get your reward.");
                        }
                        else
                        {
                            data.AddText("You`re the 101st guy to come for the Poem Book. But, I won`t give it to you if you can`t get me the Heaven Fan.");
                            data.AddOption("Heaven~Fan...");
                            data.AddOption("Bird~Island,~please!", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.NightBox)]
        public static void NightBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                {
                                    if (client.Inventory.Contain(710222, 1))
                                    {
                                        data.AddText("This is an empty box. Go check the other boxes.");
                                        data.AddOption("I~see.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(710221, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                            data.AddOption("Gimme!~Gimme!", 1);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    {
                                        data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                        data.AddOption("Gimme!~Gimme!", 1);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("There`s a poem engraved on the box. Open it now?");
                                            data.AddOption("Can`t~wait!", 2);
                                            data.AddOption("Not~now.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Watch your back!!");
                                data.AddOption("Oh,~my!!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("This is an empty box. Go check the other boxes.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        Server.AddMapMonster(stream, client.Map, 2172, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                        client.SendSysMesage("There`s something in the box...What?! A Poem Ghost!! Look out!");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710221);
                            data.AddText("You obtained a Heaven Fan!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.SendSysMesage("There`s not enough space in your inventory!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DarkBox)]
        public static void DarkBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                {
                                    if (client.Inventory.Contain(710222, 1))
                                    {
                                        data.AddText("This is an empty box. Go check the other boxes.");
                                        data.AddOption("I~see.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(710221, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                            data.AddOption("Gimme!~Gimme!", 1);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    {
                                        data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                        data.AddOption("Gimme!~Gimme!", 1);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("There`s a poem engraved on the box. Open it now?");
                                            data.AddOption("Can`t~wait!", 2);
                                            data.AddOption("Not~now.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Watch your back!!");
                                data.AddOption("Oh,~my!!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("This is an empty box. Go check the other boxes.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        Server.AddMapMonster(stream, client.Map, 2172, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                        client.SendSysMesage("There`s something in the box...What?! A Poem Ghost!! Look out!");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710221);
                            data.AddText("You obtained a Heaven Fan!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.SendSysMesage("There`s not enough space in your inventory!");
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DayBox)]
        public static void DayBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Map.View.GetAllMapRoles(Role.MapObjectType.Monster, p => p.Alive == true).Count() == 0)
                            {
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                {
                                    if (client.Inventory.Contain(710222, 1))
                                    {
                                        data.AddText("This is an empty box. Go check the other boxes.");
                                        data.AddOption("I~see.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(710221, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                            data.AddOption("Gimme!~Gimme!", 1);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {

                                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    {
                                        data.AddText("The box is opened. See? There`s a fan at the bottom of the box!");
                                        data.AddOption("Gimme!~Gimme!", 1);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                                        {
                                            data.AddText("This is an empty box. Go check the other boxes.");
                                            data.AddOption("I~see.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("There`s a poem engraved on the box. Open it now?");
                                            data.AddOption("Can`t~wait!", 2);
                                            data.AddOption("Not~now.");
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Watch your back!!");
                                data.AddOption("Oh,~my!!");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("This is an empty box. Go check the other boxes.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        Server.AddMapMonster(stream, client.Map, 2172, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.SendSysMesage("There`s something in the box...What?! A Poem Ghost!! Look out!");
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710221);
                            data.AddText("You obtained a Heaven Fan!");
                            data.AddOption("Great!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.SendSysMesage("There`s not enough space in your inventory!");
                        }
                        break;
                    }
            }
        }
        public static void SigilPacks(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog obj = new Dialog(client, stream);
            obj.AddAvatar(2907);
            switch (Option)
            {
                case 0:
                    {
                        obj.AddText("You Get @@<STR_ITEM_TYPE_" + client.ActiveNpc + "@@>@@");
                        obj.AddText("+1");
                        obj.AddOption("STR_ID_tNinjaPromotion[3330028][Option112]@@<STR_ITEM_TYPE_" + client.ActiveNpc + "@@>@@", 2);
                        obj.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(client.ActiveNpc, 1))
                        {
                            uint Points = 0;
                            uint Level = 0;

                            if (Option == 2)
                            {
                                Points = 10000;
                                Level = 9;
                            }
                            switch (client.ActiveNpc)
                            {

                                #region WildSigil(Rapid)
                                case 3330028:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)(uint)Role.Instance.Ninja.ItemType.WildSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.WildSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        using (var recycledPacket = new ServerSockets.RecycledPacket())
                                        {
                                            var streamm = recycledPacket.GetStream();
                                            client.Inventory.Remove(3330028, 1, streamm);
                                        }
                                        break;
                                    }
                                case 3330029:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.WildSigilBurning))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.WildSigilBurning, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);

                                        break;
                                    }
                                case 3330030:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.WildSigilRapid))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.WildSigilRapid, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion WildSigil(Rapid)

                                #region FlameSigil

                                case 3330031:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.FlameSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.FlameSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion FlameSigil

                                #region FlameSigil(Rapid)

                                case 3330032:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.FlameSigilRapid))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.FlameSigilRapid, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion FlameSigil(Rapid)

                                #region FlameSigil(Scorch)

                                case 3330033:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.FlameSigilScorch))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.FlameSigilScorch, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion FlameSigil(Scorch)

                                #region PrisonSigil

                                case 3330034:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.PrisonSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.PrisonSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion PrisonSigil

                                #region PrisonSigil(Fatigue)

                                case 3330035:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.PrisonSigilFatigue))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.PrisonSigilFatigue, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion PrisonSigil(Fatigue)

                                #region PrisonSigil(Futility)

                                case 3330036:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.PrisonSigilFutility))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.PrisonSigilFutility, Level, 0, 0, 0);
                                        }
                                        break;
                                    }

                                #endregion PrisonSigil(Futility)

                                #region DragonSigil

                                case 3330037:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.DragonSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.DragonSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion DragonSigil

                                #region DragonSigil(Billow)

                                case 3330038:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.DragonSigilBillow))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.DragonSigilBillow, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion DragonSigil(Billow)

                                #region DragonSigil(Fading)

                                case 3330039:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.DragonSigilFading))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.DragonSigilFading, Level, 0, 0, 0);
                                        }
                                        break;
                                    }

                                #endregion DragonSigil(Fading)

                                #region SickleSigil

                                case 3330040:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.SickleSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.SickleSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SickleSigil

                                #region SickleSigil(Flurry)

                                case 3330041:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.SickleSigilFlurry))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.SickleSigilFlurry, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SickleSigil(Flurry)

                                #region SickleSigil(Gallop)

                                case 3330042:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.SickleSigilGallop))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.SickleSigilGallop, Level, 0, 0, 0);
                                        }
                                        break;
                                    }

                                #endregion SickleSigil(Gallop)

                                #region WhirlSigil

                                case 3330043:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.WhirlSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.WhirlSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion WhirlSigil

                                #region WhirlSigil(Sage)

                                case 3330044:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.WhirlSigilSage))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.WhirlSigilSage, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion WhirlSigil(Sage)

                                #region WhirlSigil(Aura)

                                case 3330045:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.WhirlSigilAura))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.WhirlSigilAura, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion WhirlSigil(Aura)

                                #region KylinSigil

                                case 3330046:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.KylinSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.KylinSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion KylinSigil

                                #region KylinSigil(Burial)

                                case 3330047:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.KylinSigilBurial))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.KylinSigilBurial, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion KylinSigil(Burial)

                                #region KylinSigil(Seal)

                                case 3330048:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.KylinSigilSeal))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.KylinSigilSeal, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion KylinSigil(Seal)

                                #region SlashSeal

                                case 3330049:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.SlashSeal))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.SlashSeal, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SlashSeal

                                #region SlashSeal(Flash)

                                case 3330050:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.SlashSealFlash))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.SlashSealFlash, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SlashSeal(Flash)

                                #region SlashSeal(Prowess)

                                case 3330051:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.SlashSealProwess))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.SlashSealProwess, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SlashSeal(Prowess)

                                #region MudSigil

                                case 3330052:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.MudSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.MudSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion MudSigil

                                #region MudSigil(Impeccable)

                                case 3330053:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.MudSigilImpeccable))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.MudSigilImpeccable, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion MudSigil(Impeccable)

                                #region MudSigil(Solidity)

                                case 3330054:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.MudSigilSolidity))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.MudSigilSolidity, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion MudSigil(Solidity)

                                #region DustSigil

                                case 3330055:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.DustSigil))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.DustSigil, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion DustSigil

                                #region DustSigil(Extinction)

                                case 3330056:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.DustSigilExtinction))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.DustSigilExtinction, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion DustSigil(Extinction)

                                #region DustSigil(Stunn)

                                case 3330057:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.DustSigilStunn))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.DustSigilStunn, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion DustSigil(Stunn)

                                #region SigilofPaperDance

                                case 3330058:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.PaperDance))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.PaperDance, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SigilofPaperDance

                                #region SigilofHellVortex

                                case 3330059:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.HellVortex))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.HellVortex, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SigilofHellVortex

                                #region SigilofInfiniteMist

                                case 3330060:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.InfiniteMist))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.InfiniteMist, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SigilofInfiniteMist

                                #region SigilofHeaven`sWonder

                                case 3330061:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.HeavenSWonder))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.HeavenSWonder, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SigilofHeaven`sWonder

                                #region SigilofBonePulse

                                case 3330062:
                                    {
                                        if (!AtributesStatus.IsNinja(client.Player.Class))
                                            break;
                                        if (!client.MyNinja.Items.ContainsKey((uint)Role.Instance.Ninja.ItemType.BonePulse))
                                        {
                                            client.MyNinja.AddItem((uint)Role.Instance.Ninja.ItemType.BonePulse, Level, 0, 0, 0);
                                        }
                                        else
                                            client.MyNinja.AddPoints(Points);
                                        break;
                                    }

                                #endregion SigilofBonePulse
                            }
                            using (var recycledPacket = new ServerSockets.RecycledPacket())
                            {
                                var streamm = recycledPacket.GetStream();
                                client.Inventory.Remove(client.ActiveNpc, 1, streamm);
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Platform)]
        public static void Platform(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Rules are rules. You`ve put the special items into the four stone pillars? If not, the Teleport Platform will not activate.");
                            data.AddOption("Absolutely.~Activate~it~now!", 1);
                            data.AddOption("Er,~sorry.~I~haven`t.");
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Rules are rules. You`ve put the special items into the four stone pillars? If not, the Teleport Platform will not activate.");
                                data.AddOption("Absolutely.~Activate~it~now!", 4);
                                data.AddOption("Er,~sorry.~I~haven`t.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Teleport Platform looks mysterious...");
                                data.AddOption("Send~me~to~the~Rose~Garden.", 3);
                                data.AddOption("It~looks~so~weird.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("The Teleport Platform looks mysterious...");
                            data.AddOption("It~looks~so~weird.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        client.Player.AddMapEffect(stream, 686, 723, "bomb");
                        data.AddText("The platform is activated! Good job! Now, you wanna be teleported to the Rose Garden or not?");
                        data.AddOption("What~do~you~think?", 3);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Teleport(69, 56, 1791);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("You~will~be~teleported~to~the~Rose~Garden.");

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1, 1, 1, 1))
                        {
                            client.Player.AddMapEffect(stream, 686, 723, "bomb");
                            data.AddText("The platform is activated! Good job! Now, you wanna be teleported to the Rose Garden or not?");
                            data.AddOption("What~do~you~think?", 2);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, but if the 4 stone pillars are not activated, the Teleport Platform will just be a decoration.");
                            data.AddOption("I~see.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            client.Teleport(69, 56, 1791);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.CreateBoxDialog("You~will~be~teleported~to~the~Rose~Garden.");
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.VillagerChou)]
        public static void VillagerChou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 400);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 401);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 402);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KunlunWanderer, client.Player.Class, 405);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 406);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 407);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 408);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Hey, buddy! Want to have a drink with me? Wine is a good thing that can help you forget everything.");
                            data.AddOption("Do~you~know~Herbalist~Chou?", 1);
                            data.AddOption("I~will.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Did Sugar Tang tell you anything about the Tang Silk?");
                            data.AddOption("I`ve~got~the~answer.", 20);
                            data.AddOption("I`m~just~passing~by.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You brought me the Snake Meat and wine?");
                                data.AddOption("Yeah,~here~you~are.", 6);
                                data.AddOption("Sorry,~I~haven`t...");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("He is just in this village. Is that related with Sugar Tang? Oh, I know. Herbalist Chou was Frost Tang`s best friend. So she thought the Herbalist Chou`s grandson might know something, right?");
                                data.AddOption("Yeah,~you~guessed~correctly.", 5);
                                data.AddOption("I`m~not~sure.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                 && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Why are you still here? Take the Tang Silk to Sugar Tang (685,599) on Bird Island to learn more.");
                                data.AddOption("I`m~on~my~way.", 13);
                                data.AddOption("I~quit!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Calm down, okay? The money you spent will be worthy. See, this Tang Silk is the only thing my grandpa left.");
                                data.AddOption("What~can~I~do~with~the~silk?", 11);
                                data.AddOption("It~looks~nothing~special.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("So scary. I didn`t expect those Crimson Claws would follow me here.");
                                data.AddOption("Let`s~go~into~~the~Poem~Silk.", 26);
                                data.AddOption("I`ve~dealt~with~them.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Wait, wait... I read the poems on the silk again and find something strange.");
                                data.AddOption("What~did~you~find~out?", 25);
                                data.AddOption("Stop~day~dreaming...");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
           && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("How is my grandpa`s grave, now? I hope it hasn`t been swallowed by weeds. By the way, what did you get from the Swan Lake?");
                                data.AddOption("The~Purple~Eye.", 31);
                                data.AddOption("I~got~nothing,~till~now.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To find out the real meaning of the Poem Silk, we should find the Swan Lake where my grandpa was buried.");
                                data.AddOption("Swan~Lake?", 30);
                                data.AddOption("A~grave?~Let~me~go!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
     && client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Look, we have never been so close to the success, now. As long as we have all the 4 things, we can unlock the way to the Platform.");
                                data.AddOption("Where~are~they?", 34);
                                data.AddOption("I`m~too~tired~to~continue.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I`ve checked the Purple Eye you brought. That`s real.");
                                data.AddOption("Then,~what`s~the~next~thing?", 40);
                                data.AddOption("It~should~be.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("It`s been long before I heard from Sugar Tang (BirdIsland 685,599) last time. She has always wanted to find out the secrets of her dearly departed grandma.");
                            data.AddOption("She~must~be~fine.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 40:
                    {
                        data.AddText("Since you have the Purple Eye, I`ll give you my Hairpin. Now, we just need to find the Jade Scepter and the Jadeite to unlock the way to the Platform.");
                        data.AddOption("I~know~what~to~do~now.", 34);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }

                case 34:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        data.AddText("Hmm, you`ve retrieved the Purple Eye from the Swan Lake. I have the Hairpin, and Sugar Tang owns the Jadeite. For the Jade Scepter, it was stolen by Jade Courtesan. Hurry and collect all of them!");
                        data.AddOption("Give~me~the~Hairpin.", 36);
                        data.AddOption("Give~me~the~Purple~Eye.", 35);
                        data.AddOption("I`ll~return~soon!");
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        if (client.Inventory.Contain(710219, 1))
                        {
                            data.AddText("Isn`t that the Hairpin in your inventory? You should wear glasses to check, next time.");
                            data.AddOption("Sorry,~I~forgot.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 710219, 1);
                                client.CreateBoxDialog("You~received~the~Hairpin.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full! You need to clean some room, first.");
                                data.AddOption("Okay.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("I saw the Purple Eye is still in your inventory. Stop kidding me, okay?");
                            data.AddOption("Sorry,~I~forgot.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 710216, 1);
                                client.CreateBoxDialog("You~received~the~Purple~Eye.");
                            }
                            else
                            {
                                data.AddText("Your inventory is full, now. Why not tidy it up, first?");
                                data.AddOption("I`ll~be~back~soon!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 33:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Since you have the Purple Eye, I`ll give you my Hairpin. Now, we just need to find the Jade Scepter and the Jadeite to unlock the way to the Platform.");
                            data.AddOption("I~know~what~to~do~now.", 34);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Contain(710216, 1))
                        {
                            data.AddText("You got the Purple Eye? Wow, you are incredible powerful!");
                            data.AddOption("Thanks.", 33);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721799, 1))
                            {
                                data.AddText("It seems you haven`t found anything special from the Swan Lake. Wait, you have a stick of Incense, right? Go use it at the Swan Lake (55,109)!");
                                data.AddOption("I`m~on~my~way.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Did you lose your way? The entrance to Swan Lake is in the Maple Forest (471,844). Before approaching, you should burn the Incense at the Swan Lake.");
                                data.AddOption("Sorry,~I~lost~the~Incense.", 32);
                                data.AddOption("Thanks!");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721799, 1);
                            client.CreateBoxDialog("You~received~a~stick~of~Incense~from~Villager~Chou.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full now. Come back when you have some room. I`ll be here waiting.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("Sugar Tang`s grandma and Mr. Loneliness often dated in Swan Lake. That`s why you need to go there and investigate. The entrance");
                        data.AddText("~to Swan Lake is in the Maple Forest (471,844). Here`s an Incense for you. Burn it at Swan Lake (55,109). Don`t forget it.");
                        data.AddOption("Burn~Incense?~What?", 28);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                            client.Inventory.Add(stream, 721799, 1);
                            client.SendSysMesage("You received a stick of Incense.");
                        }
                        else
                        {
                            data.AddText("I can`t help, but find your inventory is full. Why not tidy up, first?");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Sugar Tang`s grandma and Mr. Loneliness often dated in Swan Lake. That`s why you need to go there and investigate. The entrance");
                            data.AddText("~to Swan Lake is in the Maple Forest (471,844). Here`s an Incense for you. Burn it at Swan Lake (55,109). Don`t forget it.");
                            data.AddOption("Burn~Incense?~What?", 28);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Player.View.ContainMobInScreen("CrimsonClaw") == false)
                        {
                            data.AddText("Poem Silk? Wait, I remember something! The Heaven Fan! Mr. Loneliness had done something with it! We need to");
                            data.AddText("~find it first! Now you`d better go to a place called Swan Lake where my grandpa was buried. Damn! Why do I have to be blind!");
                            data.AddOption("Relax.~I~can~deal~with~it.", 27);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Oh, my! They found me! Crimson Claw! Help! Please! Please!");
                            data.AddOption("Sigh.~Man,~I`m~here.~Relax.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 25:
                    {
                        data.AddText("Listen to me... Oh, no!");
                        data.AddOption("What`s~wrong?", 22);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            data.AddText("What... There are poems on the Tang Silk only? Are you sure that isn`t a treasure map? Anyway, thanks for your help.");
                            data.AddOption("I`m~doing~my~job.", 21);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I can`t find the Poem Silk you mentioned. Did you lose it somewhere?");
                            data.AddOption("It`s~very~possible.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Inventory.Contain(721801, 1))
                        {
                            client.Inventory.Remove(721801, 1, stream);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                data.AddText("Listen to me... Oh, no!");
                                data.AddOption("What`s~wrong?", 22);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I can`t find the Poem Silk you mentioned. Did you lose it somewhere?");
                            data.AddOption("It`s~very~possible.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 22:
                    {
                        Server.AddMapMonster(stream, client.Map, 2169, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "moveback");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(721798, 1))
                        {
                            data.AddText("If you find out the secret of this Tang Silk, don`t forget to let me know it, please!");
                            data.AddOption("No~problem.");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.Contain(721801, 1))
                            {
                                data.AddText("If you find out the secret of this Tang Silk, don`t forget to let me know it, please!");
                                data.AddOption("No~problem.");
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What? You lost it? How can you be so careless! My father only gave me two. This is the last one. Take it. Keep it safe for me,");
                                data.AddText("~will you?");
                                data.AddOption("I~will.", 14);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721798, 1);
                            client.SendSysMesage("You~received~the~Tang~Silk~from~Villager~Chou.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. Please make some room first.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 11:
                    {
                        data.AddText("Haha, it`s just a joke! Look, I have the Tang Silk my grandpa left. I want to know more about it, too. But you see,");
                        data.AddText("~I am a blind man... Well, you just take it to Sugar Tang. If you get something good, don`t forget to tell me!");
                        data.AddOption("Okay.", 10);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721798, 1);
                            client.SendSysMesage("You obtained the Tang Silk from Villager Chou. Now find Sugar Tang in Bird Island (685,599) to learn more about it.");
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }
                        else
                        {
                            data.AddText("Your inventory is full. Please make some room first.");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(721800, 1))
                        {
                            if (client.Inventory.Contain(710220, 5))
                            {
                                if (client.Inventory.Remove(710220, 5, stream))
                                {
                                    client.Inventory.Remove(721800, 1, stream);
                                    if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                    {
                                        client.Player.Money += 100;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                        client.SendSysMesage("You received 100 Silver!");
                                        data.AddText("Haha, it`s just a joke! Look, I have the Tang Silk my grandpa left. I want to know more about it, too. But you see,");
                                        data.AddText("~I am a blind man... Well, you just take it to Sugar Tang. If you get something good, don`t forget to tell me!");
                                        data.AddOption("Okay.", 10);
                                        data.AddAvatar(77);
                                        data.FinalizeDialog();

                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                                data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                                data.AddOption("I`m~on~my~way.", 7);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                            data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                            data.AddOption("I`m~on~my~way.", 7);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(721800, 1))
                        {
                            if (client.Inventory.Contain(710220, 5))
                            {
                                data.AddText("Mmm, that`s good. I do like snakes, but only after they`re in my belly. It so happens, that I am the man you`re looking for...");
                                data.AddOption("You~made~me~wait~for~that?", 9);//9564290
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                                data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                                data.AddOption("I`m~on~my~way.", 7);
                                data.AddAvatar(77);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Did you get the wine and meat I want? The answer is `NO`. My memory is bad when I am starving. Just bring me a bottle of Bamboo");
                            data.AddText("~Wine from Wine Kiddo (Phoenix Castle 182,280) and 5 pieces of Snake Meat in the northeast of Maple Forest. I await you back!");
                            data.AddOption("I`m~on~my~way.", 7);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        client.SendSysMesage("Buy the Bamboo Wine from Wine Kiddo (Phoenix Castle 182,280) and collect 5 pieces of Snake Meat from northeast of Maple Forest.");
                        break;
                    }
                case 5:
                    {
                        data.AddText("Of course, I know where he is! Sorry, I feel a little hungry. Would you mind bringing me some Bamboo Wine from");
                        data.AddText("~Wine Kiddo (Phoenix Castle 182,280)? Hey! And Snake Meat in the northeast of Maple Forest is amazing. Get me five!");
                        data.AddOption("Greedy~son~of~a..", 3);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Herbalist Chou? He`s dead. But I know his grandson.");
                        data.AddOption("Where~can~I~find~him?", 2);
                        data.AddAvatar(77);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Of course, I know where he is! Sorry, I feel a little hungry. Would you mind bringing me some Bamboo Wine from");
                            data.AddText("~Wine Kiddo (Phoenix Castle 182,280)? Hey! And Snake Meat in the northeast of Maple Forest is amazing. Get me five!");
                            data.AddOption("Greedy~son~of~a..", 3);
                            data.AddAvatar(77);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Buy~the~Bamboo~Wine~from~a~Wine~Kiddo~in~Phoenix~Castle~(182,280)~and~get~5~pieces~of~Snake~Meat~from~northeast~Maple~Forest.");
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SugarTang)]
        public static void SugarTang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 520);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GhoulKong, client.Player.Class, 528);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 400);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VillagerChou, client.Player.Class, 402);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SugarTang, client.Player.Class, 403);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Platform, client.Player.Class, 409);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("Hey, you are still too green! Keep practicing, and come back when you reach level 100.");
                            data.AddOption("Alright.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you ever heard of Frost Tang? Yeah, she was very famous in her time. Not only for her beauty, but also for her great skill. You know");
                                data.AddText("~she is my grandma and I wish I could be as good as her. That`s the reason why I should find the Star Tower she lost.");
                                data.AddOption("Star~Tower?", 10);
                                data.AddOption("I~don`t~think~so.");
                                data.AddAvatar(175);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                {
                                    if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                    {
                                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                        {
                                            data.AddText("Did you find Chou`s children? No?! Come on, Chou is my grandma`s best friend, so he must know something useful!");
                                            data.AddOption("How~can~I~find~him?", 35);
                                            data.AddOption("I`m~just~passing~by.");
                                            data.AddAvatar(175);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            data.AddText("Have you ever seen Yang Feng? He was the last head of the Yang family. Though he`s dead, he won`t give up his desire to destroy the Star Tower.");
                                            data.AddText("~Anyway, there is still something I can`t understand about my family. I heard the Heaven Fan has the answer I want.");
                                            data.AddText("~are still many things I cannot understand about my family. I heard that the attack talisman, the Heaven Fan, has the answers I want.");
                                            data.AddOption("Heaven~Fan?", 30);
                                            data.AddOption("I~see.");
                                            data.AddAvatar(175);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        data.AddText("Did you find Herbalist Chou? Or any of his descendants?");
                                        data.AddOption("He~gave~me~the~Tang~Silk.", 40);
                                        data.AddOption("Sorry,~no.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                    else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                                    && !client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                    {
                                        data.AddText("Why are you still here? You should take the Tang Silk to the Kunlun Wanderer. He may know something more about it. Was there anything else you needed?");
                                        data.AddOption("How~can~I~find~him?", 43);
                                        data.AddOption("I~see.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                    else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        data.AddText("Sorry, I didn`t recognize you, just now. How is your job going? I just made an important discovery about My grandma.");
                                        data.AddOption("I~brought~you~a~book!", 50);
                                        data.AddOption("I~see.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("I read the Poem Book my grandma wrote again, and I totally understand the love between Mr. Loneliness and her, now. Thanks for all you have done for us!");
                                        data.AddOption("You`re~welcome.");
                                        data.AddAvatar(175);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Have you ever heard of Frost Tang? Yeah, she was very famous in her time. Not only for her beauty, but also for her great skill. You know");
                                    data.AddText("~she is my grandma and I wish I could be as good as her. That`s the reason why I should find the Star Tower she lost.");
                                    data.AddOption("Star~Tower?", 1);
                                    data.AddOption("I~don`t~think~so.");
                                    data.AddAvatar(175);
                                    data.FinalizeDialog();
                                }
                            }

                        }
                        break;
                    }
                case 50:
                    {
                        if (client.Inventory.Contain(710222, 1))
                        {
                            data.AddText("Oh, this is just my grandma`s Poem Book! I`ll bury this book at her grave, then she can rest in peace... Well, my friend,");
                            data.AddText("~thanks for what you have done for my family. Here`s your reward. Can you guess what it is?");
                            data.AddOption("Better~be~the~Heaven~Fan...", 51);
                            data.AddOption("Don`t~care.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What book? I can`t find any books in your bag! By the way, my grandma did write a poem book before. If we can find that book, things may be easier to understand.");
                            data.AddOption("Alright.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 51:
                    {
                        if (client.Inventory.Contain(710222, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                            {
                                client.Inventory.Remove(710222, 1, stream);
                                client.Inventory.Add(stream, 201009, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.SendSysMesage("Sugar Tang took the Poem Book from you. In return, she gave you the Heaven Fan!");
                            }
                        }
                        else
                        {
                            data.AddText("What book? I can`t find any books in your bag! By the way, my grandma did write a poem book before. If we can find that book, things may be easier to understand.");
                            data.AddOption("Alright.");
                            data.AddOption("Don`t~care.");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 43:
                    {
                        data.AddText("Hmm... We need to refer this Tang Silk to Kunlun Wanderer. You can probably find him around (490,618) in Desert City. By the way, the Kunlun Wanderer will go on a trip. soon. So, be quick!");
                        data.AddOption("I`m~on~my~way.", 42);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }

                case 40:
                    {
                        if (client.Inventory.Contain(721798, 1))
                        {
                            data.AddText("Tang Silk? It looks so beautiful! There must be many secrets about this silk. Thanks for your help!");
                            data.AddOption("It`s~okay.", 41);
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You said that Villager Chou gave you a Tang Silk. But where is it now? Don`t tell me you have sold it for money.");
                            data.AddOption("Maybe~I~lost~it~somewhere?");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 41:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            data.AddText("Hmm... We need to refer this Tang Silk to Kunlun Wanderer. You can probably find him around (490,618) in Desert City. By the way, the Kunlun Wanderer will go on a trip. soon. So, be quick!");
                            data.AddOption("I`m~on~my~way.", 42);
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 42:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.CreateBoxDialog("Pay~a~visit~to~the~Kunlun~Wanderer~around~(490,618)~in~Desert~City.");
                        break;
                    }
                case 35:
                    {
                        if (client.Inventory.Contain(710218, 1))
                        {
                            data.AddText("You can probably find him in the southeast of Maple Forest. Well, I await your good news!");
                            data.AddOption("Okay,~bye!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("How could you be so careless to lose that? Fortunately, I have another one. Here you are! Don`t lose it again, okay?");
                            data.AddOption("Sorry,~I~won`t.", 36);
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 36:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 710218, 1);
                            client.SendSysMesage("You~received~a~Jadeite.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Why not tidy it up, first?");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("I don`t know much about the Heaven Fan, but I feel that must be related to the Poem Book written by my grandma and Mr. Loneliness, years ago.");
                        data.AddText("~You found the Star Tower, so I think it won`t be hard for you to find a book, right?");
                        data.AddOption("Well,~do~you~have~any~clue?", 31);
                        data.AddOption("I~have~more~important~things.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("Herbalist Chou is my grandma`s best friend, but he died, years ago. Could you go find his children with this Jadeite? I`m sure the family is still");
                        data.AddText("~living in the small village, in the southeast of Maple Forest.");
                        data.AddOption("Okay.~Bye!", 32);
                        data.AddOption("I~have~more~important~things.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Inventory.Add(stream, 710218, 1);
                            client.SendSysMesage("Sugar Tang gave you the Jadeite that Frost Tang left behind. You`ll need it, someday.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full! Why not tidy it up, first?");
                            data.AddOption("I`ll~be~back~soon!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("The Star Tower is a defensive talisman once possessed by my grandma. It was incredibly powerful, but she lost it and no one knows its whereabouts.");
                        data.AddText("~My hero, will you help me find it?");
                        data.AddOption("Any~clue?", 15);
                        data.AddOption("You~mean~this~talisman?", 11);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("Hm... The Star Tower has been broken into three pieces. I heard it through the grapevine.");
                        data.AddText("~Yang Yun may know something about one of the pieces. He is on Bird Island, now.");
                        data.AddOption("Then~where~is~the~second~one?", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }

                case 11:
                    {
                        if (client.Inventory.Contain(721785, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Remove(721785, 1, stream);
                                client.Inventory.Add(stream, 202008, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.SendSysMesage("You received an Elite Star Tower(B)!");
                                data.AddText("Yeah, that`s exactly what I`m looking for! My grandma`s Star Tower was known for its legendary defensive qualities. But it broke into");
                                data.AddText("~3 pieces when a man from the Yang family broke her heart. This started the long-standing family feud between our Tang and the Yang.");
                                data.AddOption("Love~is~never~easy.", 12);
                                data.AddAvatar(175);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("I don`t find any talisman in your inventory. If you really want to help me, visit Yang Yun (732,516) on Bird Island. He will definitely be able to help you.");
                            data.AddOption("Okay.~Bye!");
                            data.AddAvatar(175);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {

                        data.AddText("So the ghost of Yang Feng, the late head of the family, wants to destroy the talisman, rather than let someone else have it.");
                        data.AddText("~That`s why some weird things happened ever since...");
                        data.AddOption("Sounds~complicated.", 13);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Well, who knows? This is a crazy world. Anyway, thank you. You may keep this Star Tower as a souvenir.");
                        data.AddText("~That`s the least I can do to thank you for your help.");
                        data.AddOption("Thanks!");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("The Star Tower is a defensive talisman once possessed by my grandma. It was incredibly powerful, but she lost it and no one knows its whereabouts.");
                        data.AddText("~My hero, will you help me find it?");
                        data.AddOption("Any~clue?", 3);
                        data.AddOption("You~mean~this~talisman?", 2);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Stop kidding me! I don`t see any talismans with you. Sigh... When can I retrieve the Star Tower?");
                        data.AddOption("It~won`t~be~long.");
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Hm... The Star Tower has been broken into three pieces. I heard it through the grapevine.");
                        data.AddText("~Yang Yun may know something about one of the pieces. He is on Bird Island, now.");
                        data.AddOption("Then~where~is~the~second~one?", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("The second piece is in a Hut in the north of Bird Island. It belongs to the three Ruan Brothers. If you can find Ruan Brother, you may know more.");
                        data.AddText("~When you get two pieces, go to Desert City. There is someone, Plum, who should know about the third piece.");
                        data.AddOption("I`ll~visit~Yang~Yuan,~first.", 7);
                        data.AddOption("Ruan~Brother?", 6);
                        data.AddOption("Plum?", 5);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Plum knows where to find the third piece. You can probably find her around (472,578) in Desert City. Watch out for the bandits when you get there.");
                        data.AddOption("No~problem.");
                        data.AddOption("I~still~have~some~questions.", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Ruan Brother`s Hut is in the north of Bird Island guarded by some magical stone doors (208,216). Ye Sheng (192,212) may tell you how to break those doors.");
                        data.AddOption("Got~it.");
                        data.AddOption("I~still~have~some~questions.", 4);
                        data.AddAvatar(175);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            || client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                            client.CreateBoxDialog("You~were~advised~to~find~Yang~Yun~on~Bird~Island.");
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.MrWonder)]
        //public static void MrWonder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 200);
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
        //                {
        //                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
        //                    {
        //                        data.AddText("It`s~a~legendary~bird~of~enormous~size~and~strength.~Though~I`ve~figured~out~the~way~of~making~the~Tempest~Wings,");
        //                        data.AddText("~you~need~to~bring~me~Roc`s~Feather~from~the~Rainbow~Sky.~It~could~be~much~more~difficult~than~any~challenges");
        //                        data.AddText("~you`ve~ever~taken.~If~you`re~ready,~I`ll~send~you~there~with~a~fee~of~5~Meteor~Scrolls.");
        //                        if (client.Inventory.Contain(3006392, 1))
        //                        {
        //                            data.AddOption("Hand~in~Roc`s~Feather.", 10);//97257880
        //                            data.AddOption("I`ll~talk~to~you~later.");
        //                            data.AddAvatar(6);
        //                            data.FinalizeDialog();
        //                        }
        //                        else
        //                        {
        //                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
        //                            {
        //                                data.AddOption("Send~me~to~the~Rainbow~Sky!", 4);
        //                                data.AddOption("I`ll~talk~to~you~later.");
        //                                data.AddAvatar(6);
        //                                data.FinalizeDialog();
        //                            }
        //                            else
        //                            {
        //                                data.AddOption("Head~to~the~sky~(5~Meteor~Scrolls)", 2);
        //                                data.AddOption("I`ll~talk~to~you~later.");
        //                                data.AddAvatar(6);
        //                                data.FinalizeDialog();
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {

        //                        data.AddText("So~many~people~ask~for~the~Tempest~Wings,~but~it`s~as~hard~as~climbing~up~to~the~sky.");
        //                        data.AddText("~Let~alone~the~loss~of~the~technique,~it~requires~Roc`s~Feather~to~make~the~Tempest~Wings.");
        //                        data.AddText("~You~know~Roc?");
        //                        data.AddOption("I`m~all~ears.", 1);
        //                        data.AddAvatar(6);
        //                        data.FinalizeDialog();

        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("Roc~is~an~enormous~legendary~bird~transformed~from~a~gigantic~fish~of~terrific~strength.");
        //                    data.AddOption("Oh,~I~know~it!");
        //                    data.AddAvatar(6);
        //                    data.FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 10:
        //            {
        //                if (client.Inventory.Contain(3006392, 1) && client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
        //                {
        //                    client.Inventory.Remove(3006392, 1, stream);
        //                    client.Inventory.Add(stream, 204008, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
        //                    client.SendSysMesage("You~received~Elite~Tempest~Wings~(B)!");
        //                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
        //                }
        //                else
        //                {
        //                    data.AddText("It`s~really~hard~to~get~Roc`s~Feather.~I~know~you`ve~tried~your~best.~It~doesn`t~matter.");
        //                    data.AddOption("Sorry,~leave~me~alone.");
        //                    data.AddAvatar(6);
        //                    data.FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 1:
        //            {
        //                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
        //                data.AddText("It`s~a~legendary~bird~of~enormous~size~and~strength.~Though~I`ve~figured~out~the~way~of~making~the~Tempest~Wings,");
        //                data.AddText("~you~need~to~bring~me~Roc`s~Feather~from~the~Rainbow~Sky.~It~could~be~much~more~difficult~than~any~challenges");
        //                data.AddText("~you`ve~ever~taken.~If~you`re~ready,~I`ll~send~you~there~with~a~fee~of~5~Meteor~Scrolls.");
        //                if (client.Inventory.Contain(3006392, 1))
        //                {
        //                    data.AddOption("Hand~in~Roc`s~Feather.", 10);//97257880
        //                    data.AddOption("I`ll~talk~to~you~later.");
        //                    data.AddAvatar(6);
        //                    data.FinalizeDialog();
        //                }
        //                else
        //                {
        //                    if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
        //                    {
        //                        data.AddOption("Send~me~to~the~Rainbow~Sky!", 4);
        //                        data.AddOption("I`ll~talk~to~you~later.");
        //                        data.AddAvatar(6);
        //                        data.FinalizeDialog();
        //                    }
        //                    else
        //                    {
        //                        data.AddOption("Head~to~the~sky~(5~Meteor~Scrolls)", 2);
        //                        data.AddOption("I`ll~talk~to~you~later.");
        //                        data.AddAvatar(6);
        //                        data.FinalizeDialog();
        //                    }
        //                }
        //                break;
        //            }
        //        case 4:
        //            {
        //                if (client.Inventory.Contain(3006392, 1))
        //                {
        //                    client.SendSysMesage("You`ve~obtained~Roc`s~Feather.~No~need~to~challenge~the~Rainbow~Sky,~again.");
        //                }
        //                else
        //                {
        //                    client.Teleport(125, 99, 1109);
        //                    client.SendSysMesage("You`ve~arrived~in~the~Rainbow~Sky.~Hurry~and~defeat~the~Heaven~Roc~for~its~feather!");
        //                    Server.AddMapMonster(stream, client.Map, 7696, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
        //                }

        //                break;
        //            }
        //        case 2:
        //            {
        //                data.AddText("Are~you~sure~you~want~to~pay~5~Meteor~Scrolls~to~enter~the~Rainbow~Sky?");
        //                data.AddOption("Yes!", 3);
        //                data.AddOption("No.");
        //                data.AddAvatar(6);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 3:
        //            {
        //                if (client.Inventory.Contain(3006392, 1))
        //                {
        //                    client.SendSysMesage("You`ve~obtained~Roc`s~Feather.~No~need~to~challenge~the~Rainbow~Sky,~again.");
        //                }
        //                else
        //                {
        //                    if (client.Inventory.Contain(720027, 5) || client.Inventory.Contain(720027, 5, 1))
        //                    {
        //                        if (client.Inventory.Remove(720027, 5, stream))
        //                        {//7696
        //                            client.Teleport(125, 99, 1109);
        //                            Server.AddMapMonster(stream, client.Map, 7696, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
        //                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
        //                            client.SendSysMesage("You`ve~arrived~in~the~Rainbow~Sky.~Hurry~and~defeat~the~Heaven~Roc~for~its~feather!");

        //                        }
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Come~on,~you~don`t~have~5~Meteor~Scrolls.~It~couldn`t~be~too~much~for~you,~could~it?");
        //                        data.AddOption("No,~but...");
        //                        data.AddAvatar(6);
        //                        data.FinalizeDialog();
        //                    }
        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.NaughtyBoy)]
        public static void NaughtyBoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Naughty boys sometimes make good men.");
                        data.AddOption("You~naughty~boy!");
                        data.AddAvatar(68);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Celestine)]
        public static void Celestine(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(700001, 1) && client.Inventory.Contain(700011, 1) && client.Inventory.Contain(700021, 1)
                            && client.Inventory.Contain(700031, 1) && client.Inventory.Contain(700041, 1) && client.Inventory.Contain(700051, 1)
                            && client.Inventory.Contain(700061, 1))
                        {
                            if (client.Inventory.Contain(721258, 1))
                            {
                                client.Inventory.Add(stream, 721259);
                                data.AddText("Aha! You must come for Celestial Stone! Check your inventory! It`s there! Now, take it to Eternity (298,217), if you wish to get 1st rebirth.");
                                data.AddOption("OK.~Thank~you.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Although~you~get~World~normal~gems,~I~cannot~refine~the~CelestialStone~for~you~without~the~CleanWater.~");
                                data.AddOption("I~will~get~it~ready.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("People~are~pursuing~greater~achievement~during~their~lives,~but~none~can~make~it~due~to~the~limit~of~human~constitution.");
                            data.AddOption("What~does~it~mean?", 1);
                            data.AddOption("I~don`t~believe~it.");
                            data.AddAvatar(15);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Mortals~are~mundane.~Only~getting~rid~of~it~can~help~them~accomplish~their~aims.");
                        data.AddText("If~you~are~high~level~enough,~you~can~get~reborn~to~learn~more~and~stronger~skills.");
                        data.AddOption("I~am~satisfied.");
                        data.AddOption("How~to~get~reborn?", 2);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("It~is~difficult.~First,~you~should~reach~certain~level.~Second,~you~need~a~Celestial~Stone.");
                        data.AddOption("How~to~get~CelestialStone?", 3);
                        data.AddOption("Forget~it.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("CelestialStone~is~made~of~7~gems:~VioletGem,~KylinGem,~RainbowGem,~MoonGem,~PhoenixGem,~FuryGem,~DragonGem,~and~CleanWater.");
                        data.AddOption("What~is~Clean~Water?", 4);
                        data.AddOption("It~is~difficult.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("It~is~used~to~get~rid~of~your~mundaneness,~and~then~you~won`t~be~affected~by~the~environment.");
                        data.AddText("By~the~way,~Clean~Water~comes~from~celestial~rinsing.");
                        data.AddOption("What~are~gems~used~for?", 5);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Only~the~World~gems~can~protect~you~during~the~rebirth.");
                        data.AddOption("I~will~collect~them~now.", 6);
                        data.AddOption("I~changed~my~mind.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("It~is~easy~to~get~the~gems.~But~CleanWaterÃ‚Â¡Ã‚Â­");
                        data.AddOption("But~what?", 7);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        data.AddText("The~Adventure~island~is~the~headstream~of~CleanWater.~But~it~is~occupied~by~WaterEvil~and~he~uses~spell~to~hide~the~stream.");
                        data.AddOption("What~can~I~do?", 8);
                        data.AddOption("I~will~give~up.");
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("WaterEvilElder~will~go~to~get~the~water~every~certain~time.~If~you~defeat~him,~you~may~get~the~water.");
                        data.AddText("But~he~is~very~hard~to~deal~with.");
                        data.AddOption("I~see.~Thank~you.");
                        data.AddOption("Anything~else?", 9);
                        data.AddAvatar(15);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Inventory.Contain(700001, 1) && client.Inventory.Contain(700011, 1) && client.Inventory.Contain(700021, 1)
                           && client.Inventory.Contain(700031, 1) && client.Inventory.Contain(700041, 1) && client.Inventory.Contain(700051, 1)
                           && client.Inventory.Contain(700061, 1))
                        {
                            if (client.Inventory.Contain(721258, 1))
                            {
                                client.Inventory.Add(stream, 721259);
                                data.AddText("Aha! You must come for Celestial Stone! Check your inventory! It`s there! Now, take it to Eternity (298,217), if you wish to get 1st rebirth.");
                                data.AddOption("OK.~Thank~you.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Although~you~get~World~normal~gems,~I~cannot~refine~the~CelestialStone~for~you~without~the~CleanWater.~");
                                data.AddOption("I~will~get~it~ready.");
                                data.AddAvatar(15);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("People~are~pursuing~greater~achievement~during~their~lives,~but~none~can~make~it~due~to~the~limit~of~human~constitution.");
                            data.AddOption("What~does~it~mean?", 1);
                            data.AddOption("I~don`t~believe~it.");
                            data.AddAvatar(15);
                            data.FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PowerBook)]
        public static void PowerBook(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(1851, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh...the last page is completely blank...What`s wrong? Quick,");
                            data.AddText("~go find Old Beggar to find out why!");
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(1850, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Step 10: From Better to Best!");
                                data.AddText("~Get a Blue Bird Plume in Labyrinth 3F. Then go to Labyrinth 4F, and right click it near the body");
                                data.AddText("~within 3 seconds after you kill a Minotaur L120. You need ignite 10 bodies.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.QuestGUI.CheckQuest(1849, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    data.AddText("Step 9: From Good to Better.");
                                    data.AddText("~Kill 5 Blue Birds and 5 Fiend Bats in Labyrinth 3F.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.QuestGUI.CheckQuest(1848, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                    {
                                        data.AddText("Step 8: You Are What You Eat!");
                                        data.AddText("~Kill some Fiend Bats in Labyrinth 3F and take the Fiend Bat Wing to the fire");
                                        data.AddText("~there (100,66) and roast it. Eat the roasted Fiend Bat Wing.");
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.QuestGUI.CheckQuest(1847, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                        {
                                            data.AddText("Step 7: Halve it!");
                                            data.AddText("~Now, you can enter the 3rd floor to kill 2 Blue Birds within 20 seconds.");
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.QuestGUI.CheckQuest(1846, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                            {
                                                data.AddText("Step 6: Double practice!");
                                                data.AddText("~Now, go kill 10 Bladelings and 10 Agile Rats in Labyrinth 2F.");
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                if (client.Player.QuestGUI.CheckQuest(1845, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                {
                                                    data.AddText("Step 5: Want a Bite?");
                                                    data.AddText("~They always say, teeth for teeth...");
                                                    data.AddText("~Go kill 1 Agile Rat in Labyrinth 2F and");
                                                    data.AddText("~collect 1 Agile Rat Fang. Bite it and see if you could leave");
                                                    data.AddText("~a teeth mark on it. But be careful. Don`t destroy it.");
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    if (client.Player.QuestGUI.CheckQuest(1844, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                    {
                                                        data.AddText("Step 4: Speedy Speed!");
                                                        data.AddText("~Now, you can enter the 2nd floor to kill 2 Bladelings within 10 seconds.");
                                                        data.FinalizeDialog();
                                                    }
                                                    else
                                                    {
                                                        if (client.Player.QuestGUI.CheckQuest(1843, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                        {
                                                            data.AddText("Step 3: Practice Makes Perfect!");
                                                            data.AddText("~Well, the only way to test whether you`ve mastered the previous two is to");
                                                            data.AddText("~do it again. Now, go kill 15 Slingers and 15 Gold Ghosts. Do it quick!");
                                                            data.FinalizeDialog();
                                                        }
                                                        else
                                                        {
                                                            if (client.Player.QuestGUI.CheckQuest(1842, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                                            {
                                                                data.AddText("Step 2: Ghost Goer!");
                                                                data.AddText("~How do you feel after the warm-up exercises?");
                                                                data.AddText("~Now kill 10 Gold Ghosts after herding them to");
                                                                data.AddText("~Trial Cave 1F (77,23), (102,136) or (146,104).");
                                                                data.FinalizeDialog();
                                                            }
                                                            else
                                                            {

                                                                data.AddText("Step1: Warm Up!");
                                                                data.AddText("~A good beginning. To warm yourself up, go kill 20 Slingers in Labyrinth 1F.");
                                                                data.AddText("~Don`t take them for granted.");
                                                                data.FinalizeDialog();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OldBeggar)]
        public static void OldBeggar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1840);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1841);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1851);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OldBeggar, client.Player.Class, 1852);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                            data.AddText("~My power excites fear from all who dare to challenge me.");
                            data.AddOption("Please~help~me~get~stronger.", 1);
                            data.AddOption("Who~are~you?");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 10);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 9);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 15);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 10);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                         && !client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Who~are~you?", 21);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                                data.AddText("~My power excites fear from all who dare to challenge me.");
                                data.AddOption("Please~help~me~get~stronger.", 20);
                                data.AddOption("Who~are~you?");
                                data.AddAvatar(67);
                                data.FinalizeDialog();

                            }
                        }
                        else
                        {
                            data.AddText("You must have heard of me. I am the master of all martial arts. All OF THEM!");
                            data.AddText("~My power excites fear from all who dare to challenge me.");
                            data.AddOption("Who~are~you?", 8);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("What? You don`t know me? I`m Old Beggar, and I`m the greatest Martial Arts Master in the world!");
                        data.AddOption("Crazy.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Well, it`s just a little mistake, and I`ve admitted my fault. Now you can claim your rewards from Taskmaster Chang in Twin City.");
                        data.AddOption("Sounds~good.", 19);
                        data.AddOption("I`ll~think~about~it.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (client.Inventory.Contain(721924, 1))
                        {
                            data.AddText("Mmmmmm...you have finished the first 10 steps in the book? I am impressed.");
                            data.AddText("~Trust me, you will gain some more power after the second round.");
                            data.AddText("~Just carry on and repeat all ten steps!");
                            data.AddOption("Nonsense!", 16);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("All right, all right. I`ll give you another one. Please, don`t lose it again.");
                            data.AddOption("I~won`t.", 12);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 16:
                    {
                        data.AddText("Well. You are so smart. To tell you the truth, it`s fake. The whole book, is just a story.");
                        data.AddOption("You~make~me~sick!", 17);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        data.AddText("Please forgive me. I was just too hungry. Now you can claim your rewards from Taskmaster Chang.");
                        data.AddOption("I~forgive~you.", 18);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 18:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(721924, 1, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                            client.Player.Money += 6000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(300);
                            client.SendSysMesage("You received 1 hour EXp and 6,000 Silver!");
                            data.AddText("Well, it`s just a little mistake, and I`ve admitted my fault. Now you can claim your rewards from Taskmaster Chang in Twin City.");
                            data.AddOption("Sounds~good.", 19);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 19:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.CreateBoxDialog("You~can~claim~an~Armor~from~Taskmaster~Chang.");
                        break;
                    }

                case 10:
                    {
                        data.AddText("Hey, you have already got the Power Book! Why not go to the Trial Cave and start your practice with that book now?");
                        data.AddText("~I can teleport you to the Trial Cave, if you want. So?");
                        data.AddOption("I~lost~the~book.", 11);
                        data.AddOption("I~see.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(721924, 1))
                        {
                            data.AddText("What?! You LOST it? Why?!");
                            data.AddOption("Calm~down.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("All right, all right. I`ll give you another one. Please, don`t lose it again.");
                            data.AddOption("I~won`t.", 12);
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (!client.Inventory.Contain(721924, 1))
                                client.Inventory.Add(stream, 721924);
                            client.CreateBoxDialog("You~received~the~Power~Book~from~the~Old~Beggar!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        data.AddText("Yeah, that is the Power Book in your hands! Hurry to start your practice with that book in the Trial Cave.");
                        data.AddText("~If you want, I can teleport you to the Trial Cave. So?");
                        data.AddOption("I~see.", 7);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 8:
                    {
                        data.AddText("What? You don`t know me? I`m Old Beggar, and I`m the greatest Martial Arts Master in the world!");
                        data.AddOption("Crazy.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Wait! You don`t need to know me...But I see a genius for Martial Arts in you! Come come! I have a good book for you.");
                        data.AddOption("Impress~me.", 2);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("I spent a life time learning this. According to my experience, it won`t take a genius like you long to master it.");
                        data.AddText("~I`m sure you will be a legend.");
                        data.AddOption("Give~it~to~me.", 5);
                        data.AddOption("What~is~it~about?", 3);
                        data.AddOption("You~are~crazy.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Well...All things have a price, right? Pay me one Meteor first.");
                        data.AddOption("OK.", 6);
                        data.AddOption("Nope.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        if (client.Inventory.Contain(1088001, 1) || client.Inventory.Contain(1088001, 1, 1))
                        {
                            client.Inventory.Remove(1088001, 1, stream);
                            if (!client.Inventory.Contain(721924, 1))
                                client.Inventory.Add(stream, 721924);
                            client.SendSysMesage("You gave the Old Beggar a Meteor and received the Power Book!");

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You received 100 Silver!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "recovery");
                                data.AddText("Yeah, that is the Power Book in your hands! Hurry to start your practice with that book in the Trial Cave.");
                                data.AddText("~If you want, I can teleport you to the Trial Cave. So?");
                                data.AddOption("I~see.", 7);
                                data.AddAvatar(67);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("My hero, it`s the Power Book, understand? Millions of people will kill for it.");
                            data.AddOption("Wait~for~me~to~get~a~meteor.");
                            data.AddAvatar(67);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("You`ll~need~to~go~to~Labyrinth~and~practice~with~the~Power~Book.");
                        break;
                    }
                case 3:
                    {
                        data.AddText("Many people had tried to steal the book, but nobody ever discovered its secret");
                        data.AddOption("Why?", 4);
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.AddText("Power Book! It is the essence of all the secrets behind Chinese Martial Arts.");
                        data.AddText("~Only I can understand it.");
                        data.AddOption("Whatever.");
                        data.AddAvatar(67);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.MasterMoMo)]
        public static void MasterMoMo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1835);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1836);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1837);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1838);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MasterMoMo, client.Player.Class, 1839);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                            data.AddText("~the hallowed heights here...");
                            data.AddOption("Can~I~help~you?", 1);
                            data.AddOption("Sorry,~but~I`m~busy.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 10);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 2);
                                data.AddOption("Sorry,~but~I`m~busy.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 21);
                                data.AddOption("Too~dangerous~for~me.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 20);
                                data.AddOption("I`ll~come~back~later."); data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                 && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 30);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 27);
                                data.AddOption("Too~dangerous~for~me.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
         && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("What a wild desert!");
                                data.AddOption("See~you.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Can~I~help~you?", 35);
                                data.AddOption("I`ll~come~back~later.");
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                            data.AddText("~the hallowed heights here...");
                            data.AddOption("Sounds~familiar.", 9);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("I need the heart of the Bull Master. I need to smash it in public to let others know that");
                        data.AddText("~our Mu family will avenge whoever dares to shame us!");
                        data.AddOption("Hand~in~the~heart.", 31);
                        data.AddOption("OK.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        data.AddText("Thanks a lot! You are my hero. Now you should go back to Taskmaster Chang.");
                        data.AddOption("OK.", 33);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Contain(710260, 1))
                        {
                            data.AddText("Great! Bull Master`s heart! Thanks a lot!");
                            data.AddOption("You`re~welcome.", 32);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Don`t bother to ask if you didn`t retrieve the Bull Heart.");
                            data.AddOption("Oh.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Inventory.Contain(710260, 1))
                        {
                            client.Inventory.Remove(710260, 1, stream);

                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.Player.Money += 6000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(300);
                                client.SendSysMesage("You received 1 hour EXP and 6,000 Silver!");
                                client.CreateBoxDialog("Thanks~a~lot.~~Now~you~should~go~back~to~Taskmaster~Chang.");
                                data.AddText("Thanks a lot! You are my hero. Now you should go back to Taskmaster Chang.");
                                data.AddOption("OK.", 33);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("Don`t bother to ask if you didn`t retrieve the Bull Heart.");
                            data.AddOption("Oh.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 33:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 21:
                    {
                        data.AddText("I am amazed by your courage, my hero. Now listen carefully. Mad Bulls are now at Heaven Well (102,292) in Mystic Castle.");
                        data.AddText("~Take this Ghost Mirror. Use it near the Heaven Well and herd Mad Bulls into it and kill them. The Ghost Mirror will");
                        data.AddText("~then absorb its ghost and turn it into a ghost crystal. You`ll just need to take back 3 Bull Ghosts.");
                        data.AddOption("I`ve~taken~them~here.", 24);
                        data.AddOption("Gimme~another~Ghost~Mirror.", 22);
                        data.AddOption("I`ll~be~right~on~it.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        if (client.Inventory.Contain(710261, 3))
                        {
                            if (client.Inventory.Remove(710261, 3, stream))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.Inventory.Remove(721921, 1, stream);
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.SendSysMesage("You completed the quest and received 100 Silver!");
                                    data.AddText("You`ve got 3 ghosts! Now I will tell you a secret.");
                                    data.AddOption("OK.", 25);
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        else
                        {
                            data.AddText("You`ll need to collect 3 ghosts for me, and then I will tell you a secret.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 27:
                    {
                        data.AddText("You`ve got 3 ghosts! Now I will tell you a secret.");
                        data.AddOption("OK.", 25);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        data.AddText("It`s about my family. They all call me Master Mo Mo because I have mastered all the Kung Fu skills accumulated by Mo`s family.");
                        data.AddText("~But we were scattered by some weird spell of Bull Master at (186,359) in Mystic Castle. Will you avenge them");
                        data.AddText("~for me? Kill the Bull Master and retrieve the Bull Heart. It`s really a big favor, and I will reward you handsomely after that.");
                        data.AddOption("I`m~in.", 26);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        client.CreateBoxDialog("Go~to~kill~the~Bull~Master~in~Mystic~Castle~(186,359)~and~retrieve~the~Bull~Heart.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.Contain(721921, 1))
                        {
                            data.AddText("Sorry, but you`ve already had the mirror in your inventory.");
                            data.AddOption("I~forgot.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I have a spare one...But only this time? Don`t lost it again, please.");
                            data.AddOption("I~won`t.", 23);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 23:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721921, 1);
                            client.CreateBoxDialog("You~received~the~Ghost~Mirror~from~Master~Mo~Mo.~Don`t~let~him~down.");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Great! You`ve got 10 horns. But I still have a quest for you...");
                        data.AddOption("What~is~it?", 15);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        data.AddText("Great responsibility comes from great power.");
                        data.AddOption("Indeed.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("What took you so long? And you didn`t even bring back 10 Burnt Bull Horns? Please, go and get them there...I will pay!");
                        data.AddOption("I`ve~got~them~here.", 13);
                        data.AddOption("I~lost~the~Fervent~Pill.", 11);
                        data.AddOption("I~forgot.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(710259, 10))
                        {
                            if (client.Inventory.Remove(710259, 10, stream))
                            {
                                client.Inventory.Remove(721920, 1, stream);
                                client.SendSysMesage("You completed the quest and received 100 Silver!");
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Player.Money += 100;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great! You`ve got 10 horns. But I still have a quest for you...");
                                    data.AddOption("What~is~it?", 15);
                                    data.AddAvatar(64);
                                    data.FinalizeDialog();
                                }

                            }

                        }
                        else
                        {
                            data.AddText("I was not kidding with you! I need 10 of them! Ten! T-E-N!");
                            data.AddOption("Easy~man!~Don`t~yell.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        data.AddText("I`ve found out that it is much easier to capture the bull`s ghost than its body. Here is a Ghost Mirror. You`ll need to absorb 3 ghosts of the Mad Bull in the Mystic Castle.");
                        data.AddOption("I`ll~make~it.", 16);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 16:
                    {
                        data.AddText("I am amazed by your courage, my hero. Now listen carefully. Mad Bulls are now at Heaven Well (102,292) in Mystic Castle.");
                        data.AddText("~Take this Ghost Mirror. Use it near the Heaven Well and herd Mad Bulls into it and kill them. The Ghost Mirror will");
                        data.AddText("~then absorb its ghost and turn it into a ghost crystal. You`ll just need to take back 3 Bull Ghosts.");
                        data.AddOption("I`ll~be~right~on~it.", 17);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 17:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721921);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.CreateBoxDialog("You~received~the~Ghost~Mirror~from~Master~Mo~Mo!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.Contain(721920, 1))
                        {
                            data.AddText("Sorry, but you`ve had the Fervent Pill in your inventory.");
                            data.AddOption("Sorry.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What? You lost my Fervent Pill? What a waste...Never mind, I have a spare one. But don`t lose it again, will you?");
                            data.AddOption("I~won`t.", 12);
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721920, 1);
                            client.CreateBoxDialog("You~received~a~Fervent~Pill~from~Master~Mo~Mo!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver and a Fervent Pill! Go and collect Burnt Bull Horns in Mystic Castle.");
                            data.AddText("I`m master Mo Mo. I like making inventions.");
                            data.AddOption("Can~I~help~you?", 2);
                            data.AddOption("An~inventor.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("My Fervent Pill is quite dangerous and powerful. If you want, I can give it to you.");
                        data.AddOption("Claim~a~Fervent~Pill.", 3);
                        data.AddOption("What~is~Fervent~Pill?", 4);
                        data.AddOption("Too~dangerous~for~me.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Contain(721920, 1) == false)
                            {
                                client.Inventory.Add(stream, 721920, 1);
                                client.SendSysMesage("You received a Fervent Pill! Go and collect Burnt Bull Horns in Mystic Castle.");
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            }
                            else
                            {
                                data.AddText("Sing to me of the man, Muse, the man of twists and turns driven time and again off course, once he had plundered");
                                data.AddText("~the hallowed heights here...");
                                data.AddOption("Sounds~familiar.", 6);
                                data.AddAvatar(64);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(64);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("People always say, those Bull Monsters have the hardest Horns in the world which can`t be hurt by any human device.");
                        data.AddText("~I want to test the power of the Fervent Pill with them...If you could bring back 10 Burnt Bull Horns, I`ll do it.");
                        data.AddOption("How~to~get~them?", 5);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("Of course with my Fervent Pill! Its effect will last for 1 minute. You will be engulfed in flame. They will");
                        data.AddText("~enhance your power without scorching you. By the way, you can use it repeatedly. Now, go and get me 10 Burnt Bull Horns.");
                        data.AddOption("Deal.", 6);
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("Great responsibility comes from great power.");
                        data.AddOption("Indeed.");
                        data.AddAvatar(64);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RingPack)]
        public static void RingPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can choose a ring or a bracelet.");
                        data.AddOption("Super~Diamond~Bracelet.", 1);
                        data.AddOption("Super~Dragon~Ring.", 2);
                        data.AddOption("I`ll~think~about~it.");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Inventory.Remove(728250, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 152229, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Diamond~Bracelet!");
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
                            if (client.Inventory.Remove(728250, 1, stream))
                            {
                                client.Inventory.Add(stream, 722136, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.Inventory.Add(stream, 150219, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                client.CreateBoxDialog("You~opened~the~pack~and~received~a~Super~Dragon~Ring!");
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
        [NpcAttribute(NpcID.Shark)]
        public static void Shark(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Louis, client.Player.Class, 1830);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Shark, client.Player.Class, 1831);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Shark, client.Player.Class, 1832);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                            data.AddOption("I~love~treasures.", 1);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                                data.AddOption("I~love~treasures.", 12);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(53);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                                data.AddOption("I~love~treasures.", 10);
                                data.AddOption("I`ll~think~about~it.");
                                data.AddAvatar(53);
                                data.FinalizeDialog();
                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                    && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                            data.AddOption("I~love~treasures.", 20);
                            data.AddOption("I`ll~think~about~it.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There are a lot of priceless treasures here. Collect them and sell at a good price...That`s the only way to make a fortune!");
                            data.AddOption("Money~isn`t~everything.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(721871, 1))
                        {
                            data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                            data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                            data.AddOption("Gotcha.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("A little bird picked it up and returned it to me...Here you are. Don`t lose the portrait again.");
                            data.AddOption("Thanks.", 14);
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }

                        break;
                    }

                case 12:
                    {
                        data.AddText("What`s the matter? I`ve already given you the portrait.");
                        data.AddOption("It`s~a~fake~one.", 13);
                        data.AddOption("Nothing~wrong.");
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.Inventory.Contain(721878, 1))
                        {
                            data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                            data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                            data.AddOption("Gotcha.", 6);
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("A little bird picked it up and returned it to me...Here you are. Don`t lose the portrait again.");
                            data.AddOption("Thanks.", 14);
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721878, 1);
                            client.CreateBoxDialog("You~received~the~Victory~Portrait~from~Shark!");
                        }
                        else
                        {
                            data.AddText("Your inventory is full. You`ll need to make some room, first.");
                            data.AddOption("OK.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("What`s the matter? I`ve already given you the portrait.");
                        data.AddOption("It`s~a~fake~one.", 11);
                        data.AddOption("Nothing~wrong.");
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                        data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                        data.AddOption("Gotcha.", 6);
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        data.AddText("Calm down and listen to me. It is not fake. The portrait was specially made and can only be shown with the Bat");
                        data.AddText("~Essence. Use it and give it to Louis before the Essence runs out, got it?");
                        data.AddOption("Gotcha.", 6);
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You received 100 Silver!");
                            client.Inventory.Remove(721878, 1, stream);
                            client.Inventory.Add(stream, 721871);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("What`s the matter? I`ve already given you the portrait.");
                        data.AddOption("It`s~a~fake~one.", 5);
                        data.AddOption("Nothing~wrong.");
                        data.AddAvatar(53);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            data.AddText("You`ll just need to give a price within 1-99999 Silver and you can have it. If your price is too high or low, you will lose your silver offered and won`t receive the portrait.");
                            data.AddAvatar(53);
                            data.FinalizeDialog();
                            client.Player.AddPick(stream, "Bidding...", 3);

                        }
                        else
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Shhh...Come and take a look at this...I just obtained it from the black market...This portrait is valuable...");
                        data.AddOption("Is~it~a~Victory~Portrait?", 2);
                        data.AddAvatar(53);
                        data.FinalizeDialog();

                        break;
                    }
            }
        }
    }
}

