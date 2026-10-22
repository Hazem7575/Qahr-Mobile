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
        private static string Class(int Class)
        {
            if (Class >= 1000 && Class <= 1058) { return "Trojan"; }
            if (Class >= 2000 && Class <= 2058) { return "Warrior"; }
            if (Class >= 3000 && Class <= 3057) { return "DuneWanderer"; }
            if (Class >= 4000 && Class <= 4058) { return "Archer"; }
            if (Class >= 5000 && Class <= 5058) { return "Ninja"; }
            if (Class >= 6000 && Class <= 6058) { return "Monk"; }
            if (Class >= 7000 && Class <= 7058) { return "Pirate"; }
            if (Class >= 8000 && Class <= 8058) { return "Dragon-Warrior"; }
            if (Class >= 16000 && Class <= 16058) { return "Wind-Walker"; }
            if (Class >= 10000 && Class <= 14058) { return "WaterTaoist"; }
            if (Class >= 14002 && Class <= 14058) { return "FireTaoist"; }
            if (Class >= 9000 && Class <= 9058) { return "ThunderStriker"; }
            if (Class == 0) { return "Not selected"; }
            return "Not selected";
        }
        private static string ATTKs(int ATTKs)
        {
            if (ATTKs == 201) { return "[MaxHP]+[P-Attack]+[PStrike]+[Immunity]"; }
            if (ATTKs == 202) { return "[MaxHP]+[P-Attack]+[Break]+[Immunity]"; }
            if (ATTKs == 203) { return "[MaxHP]+[M-Attack]+[M-Strike]+[Immunity]"; }
            if (ATTKs == 204) { return "[MaxHP]+[P-Damge]+[AntiBreak]+[Immunity]"; }
            if (ATTKs == 0) { return "Not selected"; }
            return "Not selected";

        }


        // ------------------------------------------------------------------
        // End User Agreement (EULA) shown automatically on a character's first
        // login (see MsgDataPacket.SetLocation). The player must choose Agree to
        // continue; Disagree disconnects them. Acceptance is stored persistently
        // by UserAgreementDB so it is only ever shown once per character.
        // ------------------------------------------------------------------
        // ------------------------------------------------------------------
        // "@tool" menu (owner account only): lists the ScriptHazem tools found in
        // Sourse	ools\*\publish and opens the chosen one on the server PC.
        // ------------------------------------------------------------------
        [NpcAttribute(NpcID.ToolsMenu)]
        public static void ToolsMenuDialog(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.IsOwner)
            {
                client.SendSysMesage("Insufficient privileges for this command.");
                return;
            }
            var tools = Qahr.Mobile.ServerTools.ToolLauncher.List();
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                if (tools.Count == 0)
                {
                    data.AddText("No tools found on the server PC.~Put Folder=<tools path> in tools.ini (DB folder).");
                    data.AddOption("Close.", 255);
                }
                else
                {
                    data.AddText("~ScriptHazem Tools~");
                    data.AddText("Pick a tool to open it on the server PC.");
                    for (int i = 0; i < tools.Count && i < 9; i++)
                        data.AddOption(tools[i].Name + " - " + tools[i].Info, (byte)(i + 1));
                    data.AddOption("Close.", 255);
                }
                data.AddAvatar(30);
                data.FinalizeDialog();
                return;
            }
            if (Option >= 1 && Option <= tools.Count)
                client.SendSysMesage(Qahr.Mobile.ServerTools.ToolLauncher.Open(tools[Option - 1]));
        }

        [NpcAttribute(NpcID.UserAgreement)]
        public static void UserAgreementDialog(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("~END USER AGREEMENT~");
                        data.AddText("Please read this Agreement and choose Agree or Disagree before playing.");
                        data.AddText(" ");
                        data.AddText("1. By playing on this server you agree to follow the Rules of Conduct.");
                        data.AddText("2. Cheating, hacking, botting or exploiting bugs is strictly forbidden.");
                        data.AddText("3. Buying, selling or sharing accounts is at your own risk.");
                        data.AddText("4. The staff may suspend or ban any account that breaks these rules.");
                        data.AddText("5. Your game data may be reset or adjusted for maintenance/balance.");
                        data.AddText(" ");
                        data.AddText("By clicking \"I Agree\" you accept these terms. If you do not agree,");
                        data.AddText("click \"I Disagree\" and you will be disconnected.");
                        data.AddOption("I Agree.", 1);
                        data.AddOption("I Disagree.", 2);
                        data.AddAvatar(30);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        UserAgreementDB.SetAgreed(client.Player.UID);
                        client.CreateBoxDialog("Thank you for accepting the Agreement. Enjoy the game!");
                        break;
                    }
                case 2:
                    {
                        client.Socket.Disconnect();
                        break;
                    }
            }
        }

        //public static void PeachJade(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    if (!client.Inventory.Contain(3316793, 1))
        //    {
        //        client.SendSysMesage("Check your inventory!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
        //        return; // ( Done )
        //    }
        //    Dialog dialog = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.AddText("You have to go to Fairy Maria to exchange PeachJade. Do you want to go now?");
        //                dialog.AddOption("Find~Fairy~Maria.", 1);
        //                dialog.AddAvatar(3051);//done
        //                dialog.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {

        //                client.Player.QuestGUI.SendAutoPatcher(1002, 340, 398, (uint)NpcID.FairyMaria);
        //            }
        //            break;
        //    }
        //}


        //[NpcAttribute(NpcID.FairyMaria)] // Edit By Mohy ElDin ...
        //public static void FairyMaria(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Fairy Maria:\n");
        //                data.AddText("     Dear hero, I have many rich treasures here. if you have PeachJade, you can use PeachJade to exchange for massive rewards with me.")
        //                 .AddOption("Go~to~Exchange~Store.", 1)
        //              .AddAvatar(118)
        //       .AddOption("Leave.", 255).FinalizeDialog();


        //                break;
        //            }
        //        case 1:
        //            {
        //                {
        //                    MsgExchangeShopGoodsInfo obj = new MsgExchangeShopGoodsInfo();
        //                    obj.Type = 196;
        //                    obj.NpcID = 1;
        //                    obj.ShopID = client.Player.UID;

        //                    DateTime now64 = DateTime.Now;
        //                    int hours = now64.Hour;
        //                    int minutes = now64.Minute;
        //                    int secounds = now64.Second;

        //                    int lefthours = 23 - hours;
        //                    int leftminutes = 60 - minutes;
        //                    int leftsecounds = 60 - secounds;

        //                    TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
        //                    TimeSpan nextday = new TimeSpan(DateTime.Now.AddHours(lefthours).AddMinutes(leftminutes).AddSeconds(leftsecounds).Ticks);
        //                    client.Send(stream);

        //                    client.MyExchangeShop.Open(stream, 3316793, 196);

        //                }

        //                break;
        //            }
        //    }
        //}

        #region ArchiveDune
        //[NpcAttribute(NpcID.OpenArchivess)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void OpenArchivess(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("Hello, Through Me You Can Open The Dune Wanderer Archive For Free . \n");

                        data.Option("Open [Conception] .", 1);
                        data.Option("Open [Gobernador] .", 2);
                        data.Option("Open [Belt] .", 3);
                        data.Option("I~will~come~later.", 255);
                        data.AddAvatar(118).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.Conception, 1, 0, 1, 0);

                        break;
                    }
                case 2:
                    {
                        client.MyArchives.AddItem(Archives.TypeID.Gobernador, 1, 0, 0, 0);
                        break;
                    }
                case 3:
                    {
                        client.MyArchives.AddItem(Archives.TypeID.Belt, 1, 0, 0, 0);
                        break;
                    }



            }

        }

        [NpcAttribute(NpcID.NpcAllCollections)]
        public static void NpcAllCollections(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        dialog.Text("Hello, You Can Get All The Old and New Collections Through Me \n 1x Collection = [ 100 ] Scraps.");
                        dialog.Option("(100) Pistil Aroma Scrap. ", 3);
                        dialog.Option("(100) Lotus Dew Scrap. ", 4);
                        dialog.Option("(100) Begonia Frost Scrap. ", 5);
                        dialog.Option("(100) Snowsweet Melody Scrap. ", 6);
                        dialog.Option("(100) Peach Bloom Scrap. ", 7);
                        dialog.Option("(100) Ethereal Luster Scrap. ", 8);
                        dialog.Option("(100) Fancy Carrier Scrap. ", 9);
                        dialog.Option("(100) Droplet Glow Scrap. ", 10);
                        dialog.Option("(100) Moonlight Whispers Scrap. ", 11);
                        dialog.Option("(100) Celestial Melody Scrap. ", 12);
                        dialog.Option("(100) Fox Dream Scrap. ", 13);
                        dialog.Option("(100) Panda Ponpon Scrap. ", 14);
                        dialog.Option("(100) Dragon Ruyi Scrap. ", 15);
                        dialog.Option("Next Page. ", 2);
                        dialog.Option("I`m just passing by.", 255).AddAvatar(3688).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.InTrade) 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        dialog.Text("Hello, You Can Get All The Old and New Collections Through Me \n 1x Collection = [ 100 ] Scraps.");
                        dialog.Option("(100) Pistil Aroma Scrap. ", 3);
                        dialog.Option("(100) Lotus Dew Scrap. ", 4);
                        dialog.Option("(100) Begonia Frost Scrap. ", 5);
                        dialog.Option("(100) Snowsweet Melody Scrap. ", 6);
                        dialog.Option("(100) Peach Bloom Scrap. ", 7);
                        dialog.Option("(100) Ethereal Luster Scrap. ", 8);
                        dialog.Option("(100) Fancy Carrier Scrap. ", 9);
                        dialog.Option("(100) Droplet Glow Scrap. ", 10);
                        dialog.Option("(100) Moonlight Whispers Scrap. ", 11);
                        dialog.Option("(100) Celestial Melody Scrap. ", 12);
                        dialog.Option("(100) Fox Dream Scrap. ", 13);
                        dialog.Option("(100) Panda Ponpon Scrap. ", 14);
                        dialog.Option("(100) Dragon Ruyi Scrap. ", 15);
                        dialog.Option("Next Page. ", 2);
                        dialog.Option("I`m just passing by.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3326259, 100))
                        {
                            client.Inventory.AddItemWitchStack(4300000, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Pistil Aroma Scraps and He Got 1x Pistil Aroma [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3326259, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Pistil Aroma Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3326260, 100))
                        {
                            client.Inventory.AddItemWitchStack(4300001, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Lotus Dew Scraps and He Got 1x Lotus Dew [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3326260, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Lotus Dew Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3326261, 100))
                        {
                            client.Inventory.AddItemWitchStack(4300002, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Begonia Frost Scraps and He Got 1x Begonia Frost [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3326261, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Begonia Frost Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3326262, 100))
                        {
                            client.Inventory.AddItemWitchStack(4300003, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Snowsweet Melody Scrap and He Got 1x Snowsweet Melody [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3326262, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Snowsweet Melody Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3326263, 100))
                        {
                            client.Inventory.AddItemWitchStack(4300004, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Peach Bloom Scrap and He Got 1x Peach Bloom [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3326263, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Peach Bloom Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3336144, 100))
                        {
                            client.Inventory.AddItemWitchStack(4310000, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Ethereal Luster Scrap and He Got 1x Ethereal Luster [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3336144, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Ethereal Luster Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3336145, 100))
                        {
                            client.Inventory.AddItemWitchStack(4310001, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Fancy Carrier Scrap and He Got 1x Fancy Carrier [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3336145, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Fancy Carrier Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3336146, 100))
                        {
                            client.Inventory.AddItemWitchStack(4310002, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Droplet Glow Scrap and He Got 1x Droplet Glow [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3336146, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Droplet Glow Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3341899, 100))
                        {
                            client.Inventory.AddItemWitchStack(4320000, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Moonlight Whispers Scrap and He Got 1x Moonlight Whispers [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3341899, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Moonlight Whispers Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3341900, 100))
                        {
                            client.Inventory.AddItemWitchStack(4320001, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Celestial Melody Scrap and He Got 1x Celestial Melody [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3341900, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Celestial Melody Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3344551, 100))
                        {
                            client.Inventory.AddItemWitchStack(4320002, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Fox Dream Scrap and He Got 1x Fox Dream [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3344551, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Fox Dream Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3342835, 100))
                        {
                            client.Inventory.AddItemWitchStack(4330000, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Panda Ponpon Scrap and He Got 1x Panda Ponpon [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3342835, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Panda Ponpon Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 15:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3347144, 100))
                        {
                            client.Inventory.AddItemWitchStack(4330001, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Dragon Ruyi Scrap and He Got 1x Dragon Ruyi [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3347144, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Dragon Ruyi Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 16:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3348179, 100))
                        {
                            client.Inventory.AddItemWitchStack(4330002, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Doggy Pudding Scrap and He Got 1x Doggy Pudding [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3348179, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Doggy Pudding Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 17:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3351450, 100))
                        {
                            client.Inventory.AddItemWitchStack(4330003, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Merry Meow Fragment and He Got 1x Merry Meow [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3351450, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Merry Meow Fragment In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 18:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3352547, 100))
                        {
                            client.Inventory.AddItemWitchStack(4330004, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Wukong Fragment and He Got 1x Wukong [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3352547, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Wukong Fragment In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 19:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3346022, 100))
                        {
                            client.Inventory.AddItemWitchStack(4331000, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Stellar Canopy Fragment and He Got 1x Stellar Canopy [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3346022, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Stellar Canopy Fragment In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3349734, 100))
                        {
                            client.Inventory.AddItemWitchStack(4332000, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Morning Star Scraps and He Got 1x Morning Star [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3349734, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Morning Star Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
                        }
                        break;
                    }
                case 100:
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
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(3354664, 100))
                        {
                            client.Inventory.AddItemWitchStack(4330005, 0, 1, stream, false);
                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage("Congratulations, [ " + client.Player.Name + " ] Exchange [100] Papet Scraps and He Got 1x Papet [Collection] " + "", "", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            {
                                client.Inventory.Remove(3354664, 100, stream);
                            }
                        }
                        else
                        {
                            dialog.Text("Sorry, You Do Not Have (100) Papet Scraps In Your Inventory.");
                            dialog.Option("Okay Thanks.", 255).AddAvatar(3688).FinalizeDialog();
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
                        dialog.Text("Hello, You Can Get All The Old and New Collections Through Me \n 1x Collection = [ 100 ] Scraps.");
                        dialog.Option("(100) Doggy Pudding Scrap. ", 16);
                        dialog.Option("(100) Merry Meow Fragment. ", 17);
                        dialog.Option("(100) Wukong Fragment. ", 18);
                        dialog.Option("(100) Stellar Canopy Fragment. ", 19);
                        dialog.Option("(100) Morning Star Scrap. ", 20);
                        dialog.Option("(100) Papet Scrap. ", 100);
                        dialog.Option("Previous Page. ", 1);
                        dialog.Option("I`m just passing by.", 255).AddAvatar(3688).FinalizeDialog();
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.P7SacredTreasuresEnvoy)]
        public static void P7SacredTreasuresEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("There`s a mysterious treasure sealed since the ancient times. With a Savage Bone, you can explore and have a chance to get Sacred refinery materials, rare")
                            .AddText("~weapon accessories, and even the legendary P7 Dragon Soul.")
                            .AddOption("I~wanna~explore.", 1)
                      // .AddOption("Special~exploration.", 2)
                    .AddOption("Alright.", 255)
                    .FinalizeDialog();


                        break;
                    }
                case 1:
                    {
                        data.AddText("The Nemesis Tyrant has the Savage Bones which are required to explore the mysterious treasures. As I know, Nemesis Tyrant is now serving the Queen of Evil on the Dragon Island.")
                           .AddOption("I`ve got a Savage Bone.", 10)
                   .AddOption("Alright.", 255)
                   .FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3004181, 1))
                        {
                            client.Inventory.Remove(3004181, 1, stream);
                            uint ItemID = 0;
                            if (client.Inventory.HaveSpace(1))
                            {
                                uint[] Items = new uint[]
                                {
                                    3004243,3004244,3004245,723713,723714,723715,723716,723717
                                };
                                uint mohe = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                                //client.Inventory.Add(stream, mohe, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                                client.Inventory.AddItemWitchStack((uint)mohe, 0, 1, stream);
                            }
                            else
                                client.CreateBoxDialog("You Don't Have Much Space");
                            break;
                        }
                        else
                        {
                            data.AddText("Sorry, you don`t have a Savage Bone.")
         .AddOption("My~bad.", 255)
         .FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        data.AddText("If you don`t have a Savage Bone, you can also pay 37 or 370 CPs to explore. Of course, the more you pay, the better you get.")
                           .AddText("~How much would you like to pay?")
                           .AddOption("37~CPs.", 3)
                      .AddOption("370~CPs.", 5)
                   .AddOption("Alright.", 255)
                   .FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to use 37 CPs to explored the P7 Sacred Treasures?")
                          .AddOption("Yes.", 4)
                  .AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Are you sure you want to use 370 CPs to explored the P7 Sacred Treasures?")
                          .AddOption("Yes.", 6)
                  .AddOption("No.", 255).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(10))
                        {
                            if (client.Player.ConquerPoints >= 370)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 370;
                                for (int x = 0; x < 20; x++)
                                {
                                    uint[] array_Items = new uint[]
                       {
                               0,0, 3004250, 3004256, 0, 3004247,0 , 0, 0,0,3004251,0,0, 0,3004252,0,0,0,0,0,3004248,3004253,0,0,0,0,3004253 , 0, 0, 0, 0 ,3004250,3004254,3004255,0,0,0,3004254,3004249,
                               //0,0, 3004250, 3004250, 0, 3004251,0 , 0, 0,0,3004251,0,0, 0,3004252,0,0,0,0,0,3004253,3004253,0,0,0,0,3004254 , 0, 0, 0, 0 ,3004254,3004254,3004255,0,0,0,3004255,3004254,
                              // 0,0, 3004256, 3004250, 0, 3004247,0 , 0, 0,0,3004251,0,0, 0,3004252,0,0,0,0,0,3004253,3004253,0,0,0,0,3004248 , 0, 0, 0, 0 ,3004254,3004254,3004255,0,0,0,3004255,3004255
                       };
                                    int index = (int)Pool.GetRandom.Next(0, array_Items.Length);
                                    uint ItemId = array_Items[index];
                                    if (ItemId != 0)
                                    {
                                        var item = Pool.ItemsBase[ItemId];
                                        client.Inventory.AddItemWitchStack((uint)ItemId, 0, 1, stream);
                                        //  data.AddText("You`re ran the oven " + client.Player.EpicQuestChance + " time(s). Received " + item.Name + ". \n");
                                    }
                                    else
                                        // data.AddText("You`re ran the oven " + client.Player.EpicQuestChance + " time(s). Received nothing. \n");
                                        //client.Player.EpicQuestChance += 1;
                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "angelwing");
                                }
                                // data.AddOption("Head to The Unique Pure Oven.", 15).AddOption("Run 10 times more (270 CPs)", 12).AddOption("I see.", 255).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you don`t have enough CPs.")
                        .AddOption("Alright.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You Don`t Have Space in your inventory.")
                                                       .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 37)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 37;

                                uint ItemID = 0;
                                {
                                    uint[] Items = new uint[]
                                {
                                    3004256,3004250,3004251,3004252,3004253,3004254,3004255,3004247,3004248,3004249,
                                };
                                    uint mohe = Items[(uint)Pool.GetRandom.Next(0, Items.Length)];
                                    //client.Inventory.Add(stream, mohe, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false, Role.Flags.ItemEffect.None, true);
                                    client.Inventory.AddItemWitchStack((uint)mohe, 0, 1, stream);
                                }
                            }
                            else
                            {
                                data.AddText("Sorry, you don`t have enough CPs.")
                        .AddOption("Alright.", 255).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You Don`t Have Space in your inventory.")
                                                       .AddOption("Let me check.", 255)
                                                       .FinalizeDialog();
                        }
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.YeGuzhou)]
        public static void YeGuzhou(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {

                case 0:
                    {
                        data.AddText("STR_ID_tNewInaugurationTask[34720][Text111]@@")
                        .AddText("STR_ID_tNewInaugurationTask[34720][Text112]@@")
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option111]@@", 1)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option112]@@", 2)
                        .AddAvatar(3801).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("STR_ID_tNewInaugurationTask[34720][Text111]@@")
                       .AddText("STR_ID_tNewInaugurationTask[34720][Text211]@@")
                       .AddOption("STR_ID_tNewInaugurationTask[34720][Option211]@@", 3)
                       .AddOption("STR_ID_tNewInaugurationTask[34720][Option212]@@", 4)
                        .AddAvatar(3801).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("STR_ID_tNewInaugurationTask[34720][Text111]@@")
                        .AddText("STR_ID_tNewInaugurationTask[34720][Text611]@@")
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option611]@@", 15)
                        .AddAvatar(3801).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        var SpellID = Role.Flags.SpellIDDune.FleetingShadow;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("STR_ID_tNewInaugurationTask[34720][Text111]@@")
                        .AddText("STR_ID_tNewInaugurationTask[34720][Text311]@@")
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option312]@@", 5)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option318]@@", 6)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option313]@@", 7)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option317]@@", 8)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option319]@@", 9)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option314]@@", 10)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option321]@@", 11)
                        .AddOption("STR_ID_tLuaRes[10006]@@", 12)
                        .AddAvatar(3801).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        var SpellID = Role.Flags.SpellIDDune.TempestStrike;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        var SpellID = Role.Flags.SpellIDDune.CliffCrusher;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        var SpellID = Role.Flags.SpellIDDune.MoonwardLeap;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        var SpellID = Role.Flags.SpellIDDune.SwallowDive;

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);

                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        var SpellID = Role.Flags.SpellIDDune.FinalStand;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        var SpellID = Role.Flags.SpellIDDune.LonelyBattle;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        var SpellID = Role.Flags.SpellIDDune.SkyStep;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("STR_ID_tNewInaugurationTask[34720][Text111]@@")
                        .AddText("STR_ID_tNewInaugurationTask[34720][Text311]@@")
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option315]@@", 13)
                        .AddOption("STR_ID_tNewInaugurationTask[34720][Option320]@@", 14)
                        .AddOption("STR_ID_tLuaRes[10005]@@", 3)
                        .AddAvatar(3801).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        var SpellID = Role.Flags.SpellIDDune.SheathParry;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        var SpellID = Role.Flags.SpellIDDune.HeroicHeart;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(3801).FinalizeDialog();

                        break;
                    }
            }
        }
        #endregion 


        //[NpcAttribute(NpcID.NpcSha7nGame)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void NpcSha7nGame(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
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
                        data.AddText("Number Godzilla GM [ 01147354849 - 01104470517 ]");
                        data.AddOption("VIP Level 6 (1 Day) [50 L.E]", 255);
                        data.AddOption("VIP Level 6 (7 Days) [150 L.E]", 255);
                        data.AddOption("VIP Level 6 (15 Days) [300 L.E]", 255);
                        data.AddOption("VIP Level 6 (30 Days) [500 L.E]", 255);
                        data.AddOption("VIP Level 7 (1 Day) [100 L.E]", 255);
                        data.AddOption("VIP Level 7 (7 Days) [300 L.E]", 255);
                        data.AddOption("VIP Level 7 (15 Days) [600 L.E]", 255);
                        data.AddOption("VIP Level 7 (30 Days) [1.000 L.E]", 255);
                        data.AddOption("x1 ZADICO Trophy [400 L.E]", 255);
                        data.AddOption("x1 Godzilla Cup [800 L.E]", 255);
                        data.AddOption("x1 Ideal Rune [350 L.E]", 255);
                        data.AddOption("x1 PaperDance [250 L.E]", 255);
                        data.AddOption("x1 HellVortex [250 L.E]", 255);
                        data.AddOption("x1 InfiniteMist [250 L.E]", 255);
                        data.AddOption("x1 Heaven`sWonder [250 L.E]", 255);
                        data.AddOption("x1 BonePulse [250 L.E]", 255);
                        data.AddOption("x1 Relic Full [1.000 L.E]", 255);
                        data.AddOption("x1 Anima (P18) [2.500 L.E]", 255);
                        data.AddOption("x1 Anima (P17) [2.000 L.E]", 255);
                        data.AddOption("x1 Anima (P16) [1.500 L.E]", 255);
                        data.AddOption("x1 Anima (P15) [1.000 L.E]", 255);
                        data.AddOption("x1 Anima (P14) [850 L.E]", 255);
                        data.AddOption("x1 Anima (P13) [650 L.E]", 255);
                        data.AddOption("x1 Anima (P12) [400 L.E]", 255);
                        data.AddOption("x1 Wild Cleave (+9) [500 L.E]", 255);
                        data.AddOption("x1 StrikeBooster(+9) [500 L.E]", 255);
                        data.AddOption("1.000.000 CPs [500 L.E]", 255);
                        data.AddOption("1.000.000.000 Money [50 L.E]", 255);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.SusanooArchiveNinja)]
        public static void SusanooArchiveNinja(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            // [OfficialFix 2026-10-14] NPC 25850 is the official Saizo (Ninja Awakening, Questinfo 4301-4342). The old script
            // set Susanoo to max + both Susanoo skills for free at 60000 Ninpo score: not official. Officially Susanoo is raised
            // only by resonating Arcane Essences in the Ninja Archive (client STR_NINJA_XZNH_*, gouyu_xuzuo_type.txt; server
            // MsgGouYuAptitude ResonanceSusanoo). Quest dialogs come from the QuestEngine first; this is his idle line (client strings).
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tNinjaQuest[25850][Text111]@@");
                        data.Text("STR_ID_tNinjaQuest[25850][Text112]@@");
                        data.Text("STR_ID_tNinjaQuest[25850][Text113]@@");
                        data.Option("STR_ID_tNinjaQuest[25850][Option111]@@", 255);
                        data.AddAvatar(3497).FinalizeDialog();
                        break;
                    }
            }
        }

       
        //[NpcAttribute(NpcID.AllTournamentsInfinity)]
        //public static void AllTournamentsInfinity(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    DateTime timer = DateTime.Now;
        //    DateTime Now64 = DateTime.Now;
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Are you ready to battle on the Infinity Server! \n");
        //                data.AddText("Shadow Castle Start Every Hour At Minute x00 Ended x05 \n");
        //                data.AddText("Infinity War Start Every Hour At Minute x06 Ended x10 \n");
        //                data.AddText("Dark Flag Base Start Every Hour At Minute x10 Ended x15 \n");
        //                data.AddText("Lucky Golden Boxes Start Every Hour At Minute x20 Ended x25 \n");
        //                data.AddText("Flags War Start Every Hour At Minute x30 Ended x35 \n");
        //                data.AddText("Titan War Start Every Hour At Minute x36 Ended x40 \n");
        //                data.AddText("Phantom War Start Every Hour At Minute x41 Ended x45 \n");
        //                data.AddText("Octopus Start Every Hour At Minute x50 Ended x55 \n");
        //                data.AddText("You Have [ " + client.Player.FlagsPointss + " ] Flags Points \n");
        //                if (DateTime.Now.Minute >= 00 && DateTime.Now.Minute < 05)
        //                    data.Option("Shadow Castle Event [Open].", 1);
        //                else
        //                    data.Option("Shadow Castle Event [Closed].", 255);

        //                if (DateTime.Now.Minute >= 06 && DateTime.Now.Minute < 10)
        //                    data.Option("Infinity War [Open].", 2);
        //                else
        //                    data.Option("Infinity War [Closed].", 255);

        //                if (Game.MsgTournaments.MsgWarOfPlayers.Proces == ProcesType.Alive)
        //                    data.Option("Dark Flag Base [Open].", 3);
        //                else
        //                    data.Option("Dark Flag Base [Claim Reward].", 4);

        //                if (DateTime.Now.Minute >= 20 && DateTime.Now.Minute < 25)
        //                    data.Option("Lucky Golden Boxes [Open].", 5);
        //                else
        //                    data.Option("Lucky Golden Boxes [Closed].", 255);

        //                if (DateTime.Now.Minute >= 30 && DateTime.Now.Minute < 35)
        //                    data.Option("Flags War [Started].", 6);
        //                else
        //                    data.Option("Flags War [Ended].", 255);
        //                data.AddOption("Exchange [Flags Points].", 150);

        //                if (DateTime.Now.Minute >= 36 && DateTime.Now.Minute < 40)
        //                    data.Option("Titan War [Open].", 7);
        //                else
        //                    data.Option("Titan War [Closed].", 255);

        //                if (DateTime.Now.Minute >= 41 && DateTime.Now.Minute < 45)
        //                    data.Option("Phantom War [Open].", 8);
        //                else
        //                    data.Option("Phantom War Event [Closed].", 255);

        //                if (MsgSchedules.SquidwardOctopus.Process == ProcesType.Alive)
        //                {
        //                    data.Option("Squidward Octopus [Open].", 9);
        //                    data.Option("Change Coins.", 170);
        //                }
        //                else
        //                {
        //                    data.Option("Squidward Octopus [Closed].", 255);
        //                    data.Option("Change Coins.", 170);
        //                }

        //                data.Option("Just passing by.", 255).AddAvatar(3660).FinalizeDialog();
        //                break;
        //            }               
        //        case 1:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                if (MsgTournaments.MsgSchedules.CpsCastle.Process == MsgTournaments.ProcesType.Alive)
        //                {
        //                    switch (Pool.GetRandom.Next(0, 6))
        //                    {
        //                        case 0:
        //                            {
        //                                client.Teleport(221, 225, 10133);
        //                                break;
        //                            }
        //                        case 1:
        //                            {
        //                                client.Teleport(232, 438, 10133);
        //                                break;
        //                            }
        //                        case 2:
        //                            {
        //                                client.Teleport(461, 223, 10133);
        //                                break;
        //                            }
        //                        case 3:
        //                            {
        //                                client.Teleport(308, 296, 10133);
        //                                break;
        //                            }
        //                        case 4:
        //                            {
        //                                client.Teleport(401, 400, 10133);
        //                                break;
        //                            }
        //                        case 5:
        //                            {
        //                                client.Teleport(389, 255, 10133);
        //                                break;
        //                            }
        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Player.ProtectAttack(5 * 1000); // 5 Seconds

        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                if (DateTime.Now.Minute >= 06 && DateTime.Now.Minute < 10)
        //                {
        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 11);
        //                    if (Nr == 1) client.Teleport(100, 102, 12546); // Done 1
        //                    if (Nr == 2) client.Teleport(99, 118, 12546); // Done 2
        //                    if (Nr == 3) client.Teleport(82, 119, 12546); // Done 3
        //                    if (Nr == 4) client.Teleport(66, 100, 12546); // Done 4
        //                    if (Nr == 5) client.Teleport(69, 114, 12546); // Done 5
        //                    if (Nr == 6) client.Teleport(80, 130, 12546); // Done 6
        //                    if (Nr == 7) client.Teleport(127, 137, 12546); // Done 7
        //                    if (Nr == 8) client.Teleport(133, 106, 12546); // Done 8
        //                    if (Nr == 9) client.Teleport(116, 79, 12546); // Done 9
        //                    if (Nr == 10) client.Teleport(83, 71, 12546); // Done 10

        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }

        //        case 3:
        //            {
        //                client.Player.ProtectAttack(5 * 1000); // 5 Seconds

        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);


        //                if (Game.MsgTournaments.MsgWarOfPlayers.Proces == ProcesType.Alive)
        //                {
        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 6);
        //                    if (Nr == 1) client.Teleport(164, 177, 11222);  // Done 1
        //                    if (Nr == 2) client.Teleport(181, 164, 11222);  // Done 2
        //                    if (Nr == 3) client.Teleport(200, 179, 11222);  // Done 3
        //                    if (Nr == 4) client.Teleport(149, 170, 11222);  // Done 4
        //                    if (Nr == 5) client.Teleport(164, 167, 11222);  // Done 5           
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }

        //        case 4:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (Game.MsgTournaments.MsgWarOfPlayers.Proces == ProcesType.Dead)
        //                {
        //                    if (Game.MsgTournaments.MsgWarOfPlayers.RoundOwner != null)
        //                    {
        //                        if (Game.MsgTournaments.MsgWarOfPlayers.RoundOwner.UID == client.Player.UID)
        //                        {
    
        //                            client.Player.ConquerPoints += 10000;
        //                            Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(" [ " + client.Player.Name + " ] He Won [Dark Flag Base] AnD Got 10.000 CPs ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
        //                            Game.MsgTournaments.MsgWarOfPlayers.RoundOwner = null;
        //                        }
        //                    }
        //                }
        //                break;
        //            }

        //        case 5:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (MsgTournaments.MsgSchedules.GoldBoxWar.Process == MsgTournaments.ProcesType.Alive)
        //                {

        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                    #region Skills Trojan
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.CelestialDance)) // Skill Trojan (Scepter)
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.CelestialDance); // Skill Trojan (Scepter)
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.Cyclone)) // Skill XP Trojan
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.Cyclone);  // Skill XP Trojan
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.SuperCyclone)) // Skill XP Trojan
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.SuperCyclone); // Skill XP Trojan
        //                    #endregion
        //                    #region Skills XP Ninja
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike)) // Skill XP Ninja
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.FatalStrike); // Skill XP Ninja
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex)) // Skill XP Ninja
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.ShurikenVortex); // Skill XP Ninja
        //                    #endregion
        //                    #region Skill XP Wind Walker
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.Omnipotence)) // Skill XP Wind Walker
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.Omnipotence); // Skill XP Wind Walker
        //                    #endregion
        //                    #region Skill XP Thunder Striker
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage)) // Skill XP Thunder Striker
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.ThunderRampage); // Skill XP Thunder Striker
        //                    #endregion
        //                    #region Skill XP Dragon Warrior
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone)) // Skill XP Dragon Warrior
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.DragonCyclone); // Skill XP Dragon Warrior
        //                    #endregion
        //                    #region Skill XP Monk
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.Oblivion)) // Skill XP Monk
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.Oblivion); // Skill XP Monk
        //                    #endregion
        //                    #region Skill XP Warrior
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.Superman)) // Skill XP Warrior
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.Superman); // Skill XP Warrior
        //                    if (client.Player.ContainFlag(MsgUpdate.Flags.ManiacDance)) // Skill XP Warrior
        //                        client.Player.RemoveFlag(MsgUpdate.Flags.ManiacDance); // Skill XP Warrior
        //                    #endregion


        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 9);
        //                    if (Nr == 1) client.Teleport(239, 215, 10741);
        //                    if (Nr == 2) client.Teleport(268, 212, 10741);
        //                    if (Nr == 3) client.Teleport(230, 152, 10741);
        //                    if (Nr == 4) client.Teleport(223, 121, 10741);
        //                    if (Nr == 5) client.Teleport(285, 122, 10741);
        //                    if (Nr == 6) client.Teleport(133, 165, 10741);
        //                    if (Nr == 7) client.Teleport(176, 274, 10741);
        //                    if (Nr == 8) client.Teleport(236, 310, 10741);
        //                    //client.SendWhisper("Player Verified : [ " + client.Player.Name + " ] Successfully .", "Bots Verification System", client.Player.Name);
        //                    break;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }

        //        case 150:
        //            {
        //                byte rand = GetBalancedFlagsReward();
        //                switch (rand)
        //                {
        //                    case 1: // Done 1
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }

        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                Random random = new Random();
        //                                uint randomCPs = (uint)random.Next(500, 2001);

        //                                client.Player.ConquerPoints += randomCPs;
        //                                client.Player.FlagsPointss -= 20;

        //                                MsgSchedules.SendSysMesage(
        //                                    "[ " + client.Player.Name + " ] received " + randomCPs +
        //                                    " CPs For 20 (Flags Points) In [Flags War]",
        //                                    MsgServer.MsgMessage.ChatMode.World,
        //                                    MsgServer.MsgMessage.MsgColor.yellow
        //                                );
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }

        //                            break;
        //                        }

        //                    case 2: // Done 2
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }

        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                Random random = new Random();
        //                                int randomLevel = random.Next(1, 6);

        //                                uint animaID;

        //                                switch (randomLevel)
        //                                {
        //                                    case 1:
        //                                        animaID = 4200001;
        //                                        break;

        //                                    case 2:
        //                                        animaID = 4200002;
        //                                        break;

        //                                    case 3:
        //                                        animaID = 4200003;
        //                                        break;

        //                                    case 4:
        //                                        animaID = 4200004;
        //                                        break;

        //                                    default:
        //                                        animaID = 4200005;
        //                                        break;
        //                                }

        //                                client.Inventory.AddItemWitchStack(animaID, 0, 1, stream, false);

        //                                MsgSchedules.SendSysMesage(
        //                                    "[ " + client.Player.Name + " ] received x1 Anima Level " + randomLevel +
        //                                    " For 20 (Flags Points) In [Flags War]",
        //                                    MsgServer.MsgMessage.ChatMode.World,
        //                                    MsgServer.MsgMessage.MsgColor.yellow
        //                                );

        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }

        //                            break;
        //                        }
        //                    case 3: // Done 3
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                client.Inventory.AddItemWitchStack(722057, 0, 1, stream, false);
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 PowerEXPBall For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                    case 4: // Done 4
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }

        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                Random random = new Random();

        //                                ulong randomMoney = (ulong)random.Next(5000000, 25000001);

        //                                client.Player.Money += randomMoney;

        //                                MsgSchedules.SendSysMesage(
        //                                    "[ " + client.Player.Name + " ] received " + randomMoney.ToString("N0") +
        //                                    " Money For 20 (Flags Points) In [Flags War]",
        //                                    MsgServer.MsgMessage.ChatMode.World,
        //                                    MsgServer.MsgMessage.MsgColor.yellow
        //                                );

        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }

        //                            break;
        //                        }                            

        //                    case 5: // Done 5
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }

        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                Random random = new Random();
        //                                uint randomCPs = (uint)random.Next(500, 2001);

        //                                client.Player.ConquerPoints += randomCPs;
        //                                client.Player.FlagsPointss -= 20;

        //                                MsgSchedules.SendSysMesage(
        //                                    "[ " + client.Player.Name + " ] received " + randomCPs +
        //                                    " CPs For 20 (Flags Points) In [Flags War]",
        //                                    MsgServer.MsgMessage.ChatMode.World,
        //                                    MsgServer.MsgMessage.MsgColor.yellow
        //                                );
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }

        //                            break;
        //                        }

        //                    case 6: // Done 6
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }

        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                Random random = new Random();
        //                                int randomLevel = random.Next(1, 4);

        //                                uint MythSoulRandom;

        //                                switch (randomLevel)
        //                                {
        //                                    case 1:
        //                                        MythSoulRandom = 3325756;
        //                                        break;

        //                                    case 2:
        //                                        MythSoulRandom = 3325980;
        //                                        break;                                                                                   

        //                                    default:
        //                                        MythSoulRandom = 3325989;
        //                                        break;
        //                                }

        //                                client.Inventory.AddItemWitchStack(MythSoulRandom, 0, 1, stream, false);

        //                                MsgSchedules.SendSysMesage(
        //                                    "[ " + client.Player.Name + " ] received x1 MythSoul Level " + randomLevel +
        //                                    " For 20 (Flags Points) In [Flags War]",
        //                                    MsgServer.MsgMessage.ChatMode.World,
        //                                    MsgServer.MsgMessage.MsgColor.yellow
        //                                );

        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }

        //                            break;
        //                        }
        //                    case 7: // Done 7
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                client.Inventory.AddItemWitchStack(3324226, 0, 1, stream, false);
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 Cosmos Hammer For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                    case 8: // Done 8
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                client.Inventory.AddItemWitchStack(3322497, 0, 1, stream, false);
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 3000PotencyPointsPack For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                    case 9: // Done 9
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Player.FlagsPointss >= 20)
        //                            {
        //                                client.Inventory.AddItemWitchStack(3314915, 0, 1, stream, false);
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 LuxuryRelicBox For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
        //                                client.Player.FlagsPointss -= 20;
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
        //                                data.AddOption("Okay thanks.", 255).AddAvatar(3660).FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                }
        //                break;
        //            }
        //        case 6:
        //            {                        
        //                if (MsgTournaments.MsgSchedules.FlagsWar.Process == MsgTournaments.ProcesType.Alive)
        //                {
        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 14);
        //                    if (Nr == 1) client.Teleport(50, 132, 2071); // Done 1
        //                    if (Nr == 2) client.Teleport(40, 149, 2071); // Done 2
        //                    if (Nr == 3) client.Teleport(54, 150, 2071); // Done 3
        //                    if (Nr == 4) client.Teleport(82, 158, 2071); // Done 4
        //                    if (Nr == 5) client.Teleport(92, 148, 2071); // Done 5
        //                    if (Nr == 6) client.Teleport(125, 176, 2071); // Done 6
        //                    if (Nr == 7) client.Teleport(167, 187, 2071); // Done 7
        //                    if (Nr == 8) client.Teleport(198, 167, 2071); // Done 8
        //                    if (Nr == 9) client.Teleport(179, 133, 2071); // Done 9
        //                    if (Nr == 10) client.Teleport(169, 89, 2071); // Done 10
        //                    if (Nr == 11) client.Teleport(100, 59, 2071); // Done 11
        //                    if (Nr == 12) client.Teleport(73, 85, 2071); // Done 12 
        //                    if (Nr == 13) client.Teleport(55, 105, 2071); // Done 13                  
        //                }
        //                else
        //                    data.AddText("I`m sorry, but it is not yet time to enter the Flags War.")
        //    .AddOption("I see. ", 255)
        //    .AddAvatar(3660).FinalizeDialog();
        //                break;
        //            }

        //        case 7:
        //            {
        //                client.Player.ProtectAttack(5 * 1000); // 5 Seconds

        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                if (DateTime.Now.Minute >= 36 && DateTime.Now.Minute < 40)
        //                {
        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 7);
        //                    if (Nr == 1) client.Teleport(136, 155, 12602);  // Done 1
        //                    if (Nr == 2) client.Teleport(151, 135, 12602);  // Done 2
        //                    if (Nr == 3) client.Teleport(117, 135, 12602);  // Done 3
        //                    if (Nr == 4) client.Teleport(104, 149, 12602);  // Done 4
        //                    if (Nr == 5) client.Teleport(135, 169, 12602);  // Done 5         
        //                    if (Nr == 6) client.Teleport(166, 150, 12602);  // Done 6 

        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }


        //        case 8:
        //            {
        //                client.Player.ProtectAttack(5 * 1000); // 5 Seconds

        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                if (DateTime.Now.Minute >= 41 && DateTime.Now.Minute < 45)
        //                {
        //                    Random R = new Random();
        //                    int Nr = R.Next(1, 8);
        //                    if (Nr == 1) client.Teleport(153, 172, 12605);  // Done 1
        //                    if (Nr == 2) client.Teleport(108, 162, 12605);  // Done 2
        //                    if (Nr == 3) client.Teleport(79, 137, 12605);  // Done 3
        //                    if (Nr == 4) client.Teleport(107, 115, 12605);  // Done 4
        //                    if (Nr == 5) client.Teleport(129, 77, 12605);  // Done 5         
        //                    if (Nr == 6) client.Teleport(160, 108, 12605);  // Done 6       
        //                    if (Nr == 7) client.Teleport(144, 154, 12605);  // Done 7

        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }               

        //        case 9:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
        //                    client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

        //                if (MsgTournaments.MsgSchedules.SquidwardOctopus.Process == MsgTournaments.ProcesType.Alive)
        //                {
        //                    switch (Program.GetRandom.Next(0, 6))
        //                    {
        //                        case 0:
        //                            {
        //                                client.Teleport(236, 214, 3071);
        //                                break;
        //                            }
        //                        case 1:
        //                            {
        //                                client.Teleport(370, 241, 3071);
        //                                break;
        //                            }
        //                        case 2:
        //                            {
        //                                client.Teleport(252, 139, 3071);
        //                                break;
        //                            }
        //                        case 3:
        //                            {
        //                                client.Teleport(156, 132, 3071);
        //                                break;
        //                            }
        //                        case 4:
        //                            {
        //                                client.Teleport(94, 117, 3071);
        //                                break;
        //                            }
        //                        case 5:
        //                            {
        //                                client.Teleport(273, 319, 3071);
        //                                break;
        //                            }
        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("The Octopus Event has not started yet. Come back at minute :50 to join the event!");
        //                    data.AddOption("Alright.", 255).AddAvatar(3660).FinalizeDialog();
        //                }
        //                break;
        //            }

        //        case 10: // Copper Coins
        //            {

        //                if (client.IsVendor)
        //                    return;

        //                if (client.InTrade)
        //                    return;

        //                byte rand = (byte)Program.GetRandom.Next(1, 4);
        //                switch (rand)
        //                {
        //                    case 1:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711611, 1))
        //                            {
        //                                client.Inventory.Remove(711611, 1, stream);
        //                                client.Player.ConquerPoints += 500;
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 500 Cps for delivering Copper Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
        //                                data.AddOption("Sorry,~bye!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }

        //                    case 2:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711611, 1))
        //                            {
        //                                client.Inventory.Remove(711611, 1, stream);
        //                                client.Player.Money += 5000000;
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 5.000.000 Money for delivering Copper Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

        //                            }
        //                            else
        //                            {
        //                                data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
        //                                data.AddOption("Sorry,~bye!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                    case 3:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711611, 1))
        //                            {
        //                                client.Inventory.Remove(711611, 1, stream);
        //                                client.Inventory.AddItemWitchStack(722136, 0, 20, stream, false); // x20 ExpBall(Event) 
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got X20 ExpBall(Event) for delivering Copper Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

        //                            }
        //                            else
        //                            {
        //                                data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
        //                                data.AddOption("Sorry,~bye!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                }
        //                break;
        //            }




        //        case 11: // Silver Coins
        //            {

        //                if (client.IsVendor)
        //                    return;

        //                if (client.InTrade)
        //                    return;

        //                byte rand = (byte)Program.GetRandom.Next(1, 4);
        //                switch (rand)
        //                {
        //                    case 1:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711610, 1))
        //                            {
        //                                client.Inventory.Remove(711610, 1, stream);
        //                                client.Player.ConquerPoints += 1000;
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 1000 Cps for delivering Silver Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
        //                                data.AddOption("Oh,~no!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }

        //                    case 2:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711610, 1))
        //                            {
        //                                client.Inventory.Remove(711610, 1, stream);
        //                                client.Player.Money += 10000000;
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 10.000.000 Money for delivering Silver Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
        //                                data.AddOption("Oh,~no!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                    case 3:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711610, 1))
        //                            {
        //                                client.Inventory.Remove(711610, 1, stream);
        //                                client.Inventory.AddItemWitchStack(722136, 0, 40, stream, false); // x40 ExpBall(Event) 
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got X40 ExpBall(Event) for delivering Silver Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

        //                            }
        //                            else
        //                            {
        //                                data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
        //                                data.AddOption("Oh,~no!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                }
        //                break;
        //            }


        //        case 12: // Gold Coins
        //            {

        //                if (client.IsVendor)
        //                    return;

        //                if (client.InTrade)
        //                    return;

        //                byte rand = (byte)Program.GetRandom.Next(1, 4);
        //                switch (rand)
        //                {
        //                    case 1:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711609, 1))
        //                            {
        //                                client.Inventory.Remove(711609, 1, stream);
        //                                client.Player.ConquerPoints += 1500;
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 1500 Cps for delivering Gold Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
        //                            }
        //                            else
        //                            {
        //                                data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
        //                                data.AddOption("Sorry,~bye!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }

        //                    case 2:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711609, 1))
        //                            {
        //                                client.Inventory.Remove(711609, 1, stream);
        //                                client.Player.Money += 20000000;
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 20.000.000 Money for delivering Gold Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

        //                            }
        //                            else
        //                            {
        //                                data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
        //                                data.AddOption("Sorry,~bye!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }
        //                    case 3:
        //                        {
        //                            if (!client.Inventory.HaveSpace(1))
        //                            {
        //                                data.AddText("Please make 1 more space in inventory.");
        //                                data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
        //                                break;
        //                            }
        //                            if (client.Inventory.Contain(711609, 1))
        //                            {
        //                                client.Inventory.Remove(711609, 1, stream);
        //                                client.Inventory.AddItemWitchStack(722057, 0, 1, stream, false); // PowerEXPBall
        //                                MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got X1 PowerEXPBall for delivering Gold Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

        //                            }
        //                            else
        //                            {
        //                                data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
        //                                data.AddOption("Sorry,~bye!", 255);
        //                                data.AddAvatar(3660);
        //                                data.FinalizeDialog();
        //                            }
        //                            break;
        //                        }                            
        //                }
        //                break;
        //            }
        //        //case 150:
        //        //    {
        //        //        data.AddText("Do You Want to Exchange Your Gold Coins Now !");
        //        //        data.AddOption("Yeah,~please!", 12);
        //        //        data.AddOption("Sorry,~wait!~Wait!", 255);
        //        //        data.AddAvatar(45);
        //        //        data.FinalizeDialog();
        //        //        break;
        //        //    }
        //        //case 151:
        //        //    {
        //        //        data.AddText("Do You Want to Exchange Your Silver Coins Now !");
        //        //        data.AddOption("Yeah,~please!", 11);
        //        //        data.AddOption("No, not now!", 255);
        //        //        data.AddAvatar(45);
        //        //        data.FinalizeDialog();
        //        //        break;
        //        //    }
        //        //case 152:
        //        //    {
        //        //        data.AddText("Do You Want to Exchange Your Copper Coins Now !");
        //        //        data.AddOption("Yeah,~please!", 10);
        //        //        data.AddOption("Can I say `NO`?", 255);
        //        //        data.AddAvatar(45);
        //        //        data.FinalizeDialog();
        //        //        break;
        //        //    }
        //        case 170:
        //            {

        //                if (client.IsVendor)
        //                    return;

        //                if (client.InTrade)
        //                    return;

        //                data.AddText("Welcome to the Prize Center! Did you bring the treasure we want? So, which kind of coins you want to exchange, the Gold, Silver or the Copper?\n");
        //                data.Option("Gold Coin (Unlimited)", 12);
        //                data.Option("Silver Coin (Unlimited)", 11);
        //                data.Option("Copper Coin (Unlimited)", 10);
        //                data.Option("I`ll think it over.", 255);
        //                data.AddAvatar(3660);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //    }
        //}

        [NpcAttribute(NpcID.ShadowCastleEvent)]
        public static void ShadowCastleEvent(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Shadow Castle Event] Starts Every Hour At Minute x00 and Ended At Minute x05 \n");

                        if (DateTime.Now.Minute >= 00 && DateTime.Now.Minute < 05)

                            data.Option("Shadow Castle Event [Open].", 1);
                        else
                            data.Option("Shadow Castle Event [Closed].", 255);

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                        if (MsgTournaments.MsgSchedules.CpsCastle.Process == MsgTournaments.ProcesType.Alive)
                        {
                            switch (Pool.GetRandom.Next(0, 6))
                            {
                                case 0:
                                    {
                                        client.Teleport(221, 225, 10133);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(232, 438, 10133);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(461, 223, 10133);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(308, 296, 10133);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(401, 400, 10133);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(389, 255, 10133);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
                            data.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
            }
        }



        [NpcAttribute(NpcID.InfinityWar)]
        public static void InfinityWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Infinity War] Starts Every Hour At Minute x06 and Ended At Minute x10 \n");

                        if (DateTime.Now.Minute >= 06 && DateTime.Now.Minute < 10)
                            data.Option("Infinity War [Open].", 1);
                        else
                            data.Option("Infinity War [Closed].", 255);

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds

                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                        if (DateTime.Now.Minute >= 06 && DateTime.Now.Minute < 10)
                        {
                            Random R = new Random();
                            int Nr = R.Next(1, 11);
                            if (Nr == 1) client.Teleport(100, 102, 12546); // Done 1
                            if (Nr == 2) client.Teleport(99, 118, 12546); // Done 2
                            if (Nr == 3) client.Teleport(82, 119, 12546); // Done 3
                            if (Nr == 4) client.Teleport(66, 100, 12546); // Done 4
                            if (Nr == 5) client.Teleport(69, 114, 12546); // Done 5
                            if (Nr == 6) client.Teleport(80, 130, 12546); // Done 6
                            if (Nr == 7) client.Teleport(127, 137, 12546); // Done 7
                            if (Nr == 8) client.Teleport(133, 106, 12546); // Done 8
                            if (Nr == 9) client.Teleport(116, 79, 12546); // Done 9
                            if (Nr == 10) client.Teleport(83, 71, 12546); // Done 10

                        }
                        else
                        {
                            data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
                            data.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DarkFlagBase)]
        public static void DarkFlagBase(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Dark Flag Base] Starts Every Hour At Minute x10 and Ended At Minute x15 \n");

                        if (Game.MsgTournaments.MsgWarOfPlayers.Proces == ProcesType.Alive)
                            data.Option("Dark Flag Base [Open].", 1);
                        else
                            data.Option("Dark Flag Base [Claim Reward].", 2);

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds

                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);


                        if (Game.MsgTournaments.MsgWarOfPlayers.Proces == ProcesType.Alive)
                        {
                            Random R = new Random();
                            int Nr = R.Next(1, 6);
                            if (Nr == 1) client.Teleport(164, 177, 11222);  // Done 1
                            if (Nr == 2) client.Teleport(181, 164, 11222);  // Done 2
                            if (Nr == 3) client.Teleport(200, 179, 11222);  // Done 3
                            if (Nr == 4) client.Teleport(149, 170, 11222);  // Done 4
                            if (Nr == 5) client.Teleport(164, 167, 11222);  // Done 5           
                        }
                        else
                        {
                            data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
                            data.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }

                case 2:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (Game.MsgTournaments.MsgWarOfPlayers.Proces == ProcesType.Dead)
                        {
                            if (Game.MsgTournaments.MsgWarOfPlayers.RoundOwner != null)
                            {
                                if (Game.MsgTournaments.MsgWarOfPlayers.RoundOwner.UID == client.Player.UID)
                                {

                                    client.Player.ConquerPoints += 5000;
                                    Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(" [ " + client.Player.Name + " ] He Won [Dark Flag Base] AnD Got 5.000 CPs ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    Game.MsgTournaments.MsgWarOfPlayers.RoundOwner = null;
                                }
                            }
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.LuckyGoldenBoxes)]
        public static void LuckyGoldenBoxes(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Lucky Golden Boxes] Starts Every Hour At Minute x20 and Ended At Minute x25 \n");

                        if (DateTime.Now.Minute >= 20 && DateTime.Now.Minute < 25)
                            data.Option("Lucky Golden Boxes [Open].", 1);
                        else
                            data.Option("Lucky Golden Boxes [Closed].", 255);

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (MsgTournaments.MsgSchedules.GoldBoxWar.Process == MsgTournaments.ProcesType.Alive)
                        {

                            if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            #region Skills Trojan
                            if (client.Player.ContainFlag(MsgUpdate.Flags.CelestialDance)) // Skill Trojan (Scepter)
                                client.Player.RemoveFlag(MsgUpdate.Flags.CelestialDance); // Skill Trojan (Scepter)
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Cyclone)) // Skill XP Trojan
                                client.Player.RemoveFlag(MsgUpdate.Flags.Cyclone);  // Skill XP Trojan
                            if (client.Player.ContainFlag(MsgUpdate.Flags.SuperCyclone)) // Skill XP Trojan
                                client.Player.RemoveFlag(MsgUpdate.Flags.SuperCyclone); // Skill XP Trojan
                            #endregion
                            #region Skills XP Ninja
                            if (client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike)) // Skill XP Ninja
                                client.Player.RemoveFlag(MsgUpdate.Flags.FatalStrike); // Skill XP Ninja
                            if (client.Player.ContainFlag(MsgUpdate.Flags.ShurikenVortex)) // Skill XP Ninja
                                client.Player.RemoveFlag(MsgUpdate.Flags.ShurikenVortex); // Skill XP Ninja
                            #endregion
                            #region Skill XP Wind Walker
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Omnipotence)) // Skill XP Wind Walker
                                client.Player.RemoveFlag(MsgUpdate.Flags.Omnipotence); // Skill XP Wind Walker
                            #endregion
                            #region Skill XP Thunder Striker
                            if (client.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage)) // Skill XP Thunder Striker
                                client.Player.RemoveFlag(MsgUpdate.Flags.ThunderRampage); // Skill XP Thunder Striker
                            #endregion
                            #region Skill XP Dragon Warrior
                            if (client.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone)) // Skill XP Dragon Warrior
                                client.Player.RemoveFlag(MsgUpdate.Flags.DragonCyclone); // Skill XP Dragon Warrior
                            #endregion
                            #region Skill XP Monk
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Oblivion)) // Skill XP Monk
                                client.Player.RemoveFlag(MsgUpdate.Flags.Oblivion); // Skill XP Monk
                            #endregion
                            #region Skill XP Warrior
                            if (client.Player.ContainFlag(MsgUpdate.Flags.Superman)) // Skill XP Warrior
                                client.Player.RemoveFlag(MsgUpdate.Flags.Superman); // Skill XP Warrior
                            if (client.Player.ContainFlag(MsgUpdate.Flags.ManiacDance)) // Skill XP Warrior
                                client.Player.RemoveFlag(MsgUpdate.Flags.ManiacDance); // Skill XP Warrior
                            #endregion


                            Random R = new Random();
                            int Nr = R.Next(1, 9);
                            if (Nr == 1) client.Teleport(239, 215, 10741);
                            if (Nr == 2) client.Teleport(268, 212, 10741);
                            if (Nr == 3) client.Teleport(230, 152, 10741);
                            if (Nr == 4) client.Teleport(223, 121, 10741);
                            if (Nr == 5) client.Teleport(285, 122, 10741);
                            if (Nr == 6) client.Teleport(133, 165, 10741);
                            if (Nr == 7) client.Teleport(176, 274, 10741);
                            if (Nr == 8) client.Teleport(236, 310, 10741);
                            //client.SendWhisper("Player Verified : [ " + client.Player.Name + " ] Successfully .", "Bots Verification System", client.Player.Name);
                            break;
                        }
                        else
                        {
                            data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
                            data.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        private static List<byte> FlagsRewards = new List<byte>();
        private static byte GetBalancedFlagsReward()
        {
            if (FlagsRewards.Count == 0)
            {
                FlagsRewards.Add(1);
                FlagsRewards.Add(2);
                FlagsRewards.Add(3);
                FlagsRewards.Add(4);
                FlagsRewards.Add(5);
                FlagsRewards.Add(6);
                FlagsRewards.Add(7);
                FlagsRewards.Add(8);
                FlagsRewards.Add(9);

                // Shuffle
                for (int i = FlagsRewards.Count - 1; i > 0; i--)
                {
                    int j = Pool.GetRandom.Next(0, i + 1);

                    byte temp = FlagsRewards[i];
                    FlagsRewards[i] = FlagsRewards[j];
                    FlagsRewards[j] = temp;
                }
            }

            byte reward = FlagsRewards[0];
            FlagsRewards.RemoveAt(0);

            return reward;
        }

        [NpcAttribute(NpcID.FlagsWar)]
        public static void FlagsWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Flags War] Starts Every Hour At Minute x30 and Ended At Minute x35 \n");

                        if (DateTime.Now.Minute >= 30 && DateTime.Now.Minute < 35)
                            data.Option("Flags War [Started].", 1);
                        else
                            data.Option("Flags War [Ended].", 255);
                        data.AddOption("Exchange [Flags Points].", 2);

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (MsgTournaments.MsgSchedules.FlagsWar.Process == MsgTournaments.ProcesType.Alive)
                        {
                            Random R = new Random();
                            int Nr = R.Next(1, 14);
                            if (Nr == 1) client.Teleport(50, 132, 2071); // Done 1
                            if (Nr == 2) client.Teleport(40, 149, 2071); // Done 2
                            if (Nr == 3) client.Teleport(54, 150, 2071); // Done 3
                            if (Nr == 4) client.Teleport(82, 158, 2071); // Done 4
                            if (Nr == 5) client.Teleport(92, 148, 2071); // Done 5
                            if (Nr == 6) client.Teleport(125, 176, 2071); // Done 6
                            if (Nr == 7) client.Teleport(167, 187, 2071); // Done 7
                            if (Nr == 8) client.Teleport(198, 167, 2071); // Done 8
                            if (Nr == 9) client.Teleport(179, 133, 2071); // Done 9
                            if (Nr == 10) client.Teleport(169, 89, 2071); // Done 10
                            if (Nr == 11) client.Teleport(100, 59, 2071); // Done 11
                            if (Nr == 12) client.Teleport(73, 85, 2071); // Done 12 
                            if (Nr == 13) client.Teleport(55, 105, 2071); // Done 13                  
                        }
                        else
                            data.AddText("I`m sorry, but it is not yet time to enter the Flags War.")
            .AddOption("I see. ", 255)
            .AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        byte rand = GetBalancedFlagsReward();
                        switch (rand)
                        {
                            case 1: // Done 1
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
                                        break;
                                    }

                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        Random random = new Random();
                                        uint randomCPs = (uint)random.Next(500, 2001);

                                        client.Player.ConquerPoints += randomCPs;
                                        client.Player.FlagsPointss -= 20;

                                        MsgSchedules.SendSysMesage(
                                            "[ " + client.Player.Name + " ] received " + randomCPs +
                                            " CPs For 20 (Flags Points) In [Flags War]",
                                            MsgServer.MsgMessage.ChatMode.World,
                                            MsgServer.MsgMessage.MsgColor.yellow
                                        );
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }

                                    break;
                                }

                            case 2: // Done 2
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }

                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        Random random = new Random();
                                        int randomLevel = random.Next(1, 6);

                                        uint animaID;

                                        switch (randomLevel)
                                        {
                                            case 1:
                                                animaID = 4200001;
                                                break;

                                            case 2:
                                                animaID = 4200002;
                                                break;

                                            case 3:
                                                animaID = 4200003;
                                                break;

                                            case 4:
                                                animaID = 4200004;
                                                break;

                                            default:
                                                animaID = 4200005;
                                                break;
                                        }

                                        client.Inventory.AddItemWitchStack(animaID, 0, 1, stream, false);

                                        MsgSchedules.SendSysMesage(
                                            "[ " + client.Player.Name + " ] received x1 Anima Level " + randomLevel +
                                            " For 20 (Flags Points) In [Flags War]",
                                            MsgServer.MsgMessage.ChatMode.World,
                                            MsgServer.MsgMessage.MsgColor.yellow
                                        );

                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }

                                    break;
                                }
                            case 3: // Done 3
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        client.Inventory.AddItemWitchStack(722057, 0, 1, stream, false);
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 PowerEXPBall For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }
                                    break;
                                }
                            case 4: // Done 4
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }

                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        Random random = new Random();

                                        ulong randomMoney = (ulong)random.Next(5000000, 25000001);

                                        client.Player.Money += randomMoney;

                                        MsgSchedules.SendSysMesage(
                                            "[ " + client.Player.Name + " ] received " + randomMoney.ToString("N0") +
                                            " Money For 20 (Flags Points) In [Flags War]",
                                            MsgServer.MsgMessage.ChatMode.World,
                                            MsgServer.MsgMessage.MsgColor.yellow
                                        );

                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }

                                    break;
                                }

                            case 5: // Done 5
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }

                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        Random random = new Random();
                                        uint randomCPs = (uint)random.Next(500, 2001);

                                        client.Player.ConquerPoints += randomCPs;
                                        client.Player.FlagsPointss -= 20;

                                        MsgSchedules.SendSysMesage(
                                            "[ " + client.Player.Name + " ] received " + randomCPs +
                                            " CPs For 20 (Flags Points) In [Flags War]",
                                            MsgServer.MsgMessage.ChatMode.World,
                                            MsgServer.MsgMessage.MsgColor.yellow
                                        );
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }

                                    break;
                                }

                            case 6: // Done 6
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }

                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        Random random = new Random();
                                        int randomLevel = random.Next(1, 4);

                                        uint MythSoulRandom;

                                        switch (randomLevel)
                                        {
                                            case 1:
                                                MythSoulRandom = 3325756;
                                                break;

                                            case 2:
                                                MythSoulRandom = 3325980;
                                                break;

                                            default:
                                                MythSoulRandom = 3325989;
                                                break;
                                        }

                                        client.Inventory.AddItemWitchStack(MythSoulRandom, 0, 1, stream, false);

                                        MsgSchedules.SendSysMesage(
                                            "[ " + client.Player.Name + " ] received x1 MythSoul Level " + randomLevel +
                                            " For 20 (Flags Points) In [Flags War]",
                                            MsgServer.MsgMessage.ChatMode.World,
                                            MsgServer.MsgMessage.MsgColor.yellow
                                        );

                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }

                                    break;
                                }
                            case 7: // Done 7
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        client.Inventory.AddItemWitchStack(3324226, 0, 1, stream, false);
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 Cosmos Hammer For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }
                                    break;
                                }
                            case 8: // Done 8
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        client.Inventory.AddItemWitchStack(3322497, 0, 1, stream, false);
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 3000PotencyPointsPack For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }
                                    break;
                                }
                            case 9: // Done 9
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Player.FlagsPointss >= 20)
                                    {
                                        client.Inventory.AddItemWitchStack(3314915, 0, 1, stream, false);
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] received x1 LuxuryRelicBox For 20 (Flags Points) In [Flags War]", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                                        client.Player.FlagsPointss -= 20;
                                    }
                                    else
                                    {
                                        data.AddText("Sorry, You do not have (20) Flags Points to Exchange for Prizes.");
                                        data.AddOption("Okay thanks.", 255).AddAvatar(3497).FinalizeDialog();
                                    }
                                    break;
                                }
                        }
                        break;
                    }              
            }
        }       


        [NpcAttribute(NpcID.MortalKnightsWar)]
        public static void MortalKnightsWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Mortal Knights War] Starts Every Hour At Minute x41 and Ended At Minute x45 \n");

                        if (DateTime.Now.Minute >= 41 && DateTime.Now.Minute < 45)
                            data.Option("Mortal Knights War [Open].", 1);
                        else
                            data.Option("Mortal Knights War [Closed].", 255);

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds

                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                        if (DateTime.Now.Minute >= 41 && DateTime.Now.Minute < 45)
                        {
                            Random R = new Random();
                            int Nr = R.Next(1, 7);
                            if (Nr == 1) client.Teleport(111, 102, 12275);  // Done 1
                            if (Nr == 2) client.Teleport(129, 88, 12275);  // Done 2
                            if (Nr == 3) client.Teleport(68, 84, 12275);  // Done 3
                            if (Nr == 4) client.Teleport(79, 128, 12275);  // Done 4
                            if (Nr == 5) client.Teleport(127, 137, 12275);  // Done 5         
                            if (Nr == 6) client.Teleport(138, 104, 12275);  // Done 6       

                        }
                        else
                        {
                            data.AddText("Sorry, but you cannot enter the war now. Please try again when the war starts at the scheduled time.");
                            data.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    } 
            }
        }


        [NpcAttribute(NpcID.SquidwardOctopus)]
        public static void SquidwardOctopus(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("[Squidward Octopus] Starts Every Hour At Minute x50 and Ended At Minute x55 \n");

                        if (MsgSchedules.SquidwardOctopus.Process == ProcesType.Alive)
                        {
                            data.Option("Squidward Octopus [Open].", 1);
                            data.Option("Change Coins.", 2);
                        }
                        else
                        {
                            data.Option("Squidward Octopus [Closed].", 255);
                            data.Option("Change Coins.", 2);
                        }

                        data.Option("Just passing by.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            client.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                        if (MsgTournaments.MsgSchedules.SquidwardOctopus.Process == MsgTournaments.ProcesType.Alive)
                        {
                            switch (Program.GetRandom.Next(0, 6))
                            {
                                case 0:
                                    {
                                        client.Teleport(236, 214, 3071);
                                        break;
                                    }
                                case 1:
                                    {
                                        client.Teleport(370, 241, 3071);
                                        break;
                                    }
                                case 2:
                                    {
                                        client.Teleport(252, 139, 3071);
                                        break;
                                    }
                                case 3:
                                    {
                                        client.Teleport(156, 132, 3071);
                                        break;
                                    }
                                case 4:
                                    {
                                        client.Teleport(94, 117, 3071);
                                        break;
                                    }
                                case 5:
                                    {
                                        client.Teleport(273, 319, 3071);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            data.AddText("The Octopus Event has not started yet. Come back at minute :50 to join the event!");
                            data.AddOption("Alright.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("Welcome to the Prize Center! Did you bring the treasure we want? So, which kind of coins you want to exchange, the Gold, Silver or the Copper?\n");
                        data.Option("Gold Coin (Unlimited)", 5);
                        data.Option("Silver Coin (Unlimited)", 4);
                        data.Option("Copper Coin (Unlimited)", 3);
                        data.Option("I`ll think it over.", 255);
                        data.AddAvatar(3497);
                        data.FinalizeDialog();
                        break;
                    }
                case 3: // Copper Coins
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        byte rand = (byte)Program.GetRandom.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711611, 1))
                                    {
                                        client.Inventory.Remove(711611, 1, stream);
                                        client.Player.ConquerPoints += 500;
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 500 Cps for delivering Copper Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
                                        data.AddOption("Sorry,~bye!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }

                            case 2:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3660).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711611, 1))
                                    {
                                        client.Inventory.Remove(711611, 1, stream);
                                        client.Player.Money += 5000000;
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 5.000.000 Money for delivering Copper Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                    }
                                    else
                                    {
                                        data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
                                        data.AddOption("Sorry,~bye!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711611, 1))
                                    {
                                        client.Inventory.Remove(711611, 1, stream);
                                        client.Inventory.AddItemWitchStack(722136, 0, 20, stream, false); // x20 ExpBall(Event) 
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got X20 ExpBall(Event) for delivering Copper Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                    }
                                    else
                                    {
                                        data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
                                        data.AddOption("Sorry,~bye!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 4: // Silver Coins
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        byte rand = (byte)Program.GetRandom.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711610, 1))
                                    {
                                        client.Inventory.Remove(711610, 1, stream);
                                        client.Player.ConquerPoints += 1000;
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 1000 Cps for delivering Silver Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
                                        data.AddOption("Oh,~no!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }

                            case 2:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711610, 1))
                                    {
                                        client.Inventory.Remove(711610, 1, stream);
                                        client.Player.Money += 10000000;
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 10.000.000 Money for delivering Silver Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                    }
                                    else
                                    {
                                        data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
                                        data.AddOption("Oh,~no!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711610, 1))
                                    {
                                        client.Inventory.Remove(711610, 1, stream);
                                        client.Inventory.AddItemWitchStack(722136, 0, 40, stream, false); // x40 ExpBall(Event) 
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got X40 ExpBall(Event) for delivering Silver Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                    }
                                    else
                                    {
                                        data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
                                        data.AddOption("Oh,~no!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 5: // Gold Coins
                    {

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        byte rand = (byte)Program.GetRandom.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711609, 1))
                                    {
                                        client.Inventory.Remove(711609, 1, stream);
                                        client.Player.ConquerPoints += 1500;
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 1500 Cps for delivering Gold Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                    }
                                    else
                                    {
                                        data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
                                        data.AddOption("Sorry,~bye!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }

                            case 2:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711609, 1))
                                    {
                                        client.Inventory.Remove(711609, 1, stream);
                                        client.Player.Money += 20000000;
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got 20.000.000 Money for delivering Gold Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                    }
                                    else
                                    {
                                        data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
                                        data.AddOption("Sorry,~bye!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in inventory.");
                                        data.AddOption("Okay.", 255).AddAvatar(3497).FinalizeDialog();
                                        break;
                                    }
                                    if (client.Inventory.Contain(711609, 1))
                                    {
                                        client.Inventory.Remove(711609, 1, stream);
                                        client.Inventory.AddItemWitchStack(722057, 0, 1, stream, false); // PowerEXPBall
                                        MsgSchedules.SendSysMesage("[ " + client.Player.Name + " ] got X1 PowerEXPBall for delivering Gold Coin!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                    }
                                    else
                                    {
                                        data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
                                        data.AddOption("Sorry,~bye!", 255);
                                        data.AddAvatar(3497);
                                        data.FinalizeDialog();
                                    }
                                    break;
                                }
                        }
                        break;
                    }               
            }
        }
        //[NpcAttribute(NpcID.InfinityWarPrize)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void InfinityWarPrize(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        data.AddText("[Infinity War] Ended At Minute [ x10 ] \n");
                        data.AddOption("Claim~Reward.", 1);
                        data.AddOption("I~will~come~later.", 255).FinalizeDialog();
                        break;
                    }
                #region Infinity War Prize
                case 1:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        int alive = 0;
                        DateTime Now64 = DateTime.Now;
                        foreach (Client.GameClient Client in Pool.GamePoll.Values)
                            if (Client.Player.Map == 12546 && Client.Player.Alive && client.Player.HitPoints >= 1)
                                alive++;
                        if (DateTime.Now.Minute >= 10)
                        {
                            if (alive == 1)
                            {
                                client.Player.ConquerPoints += 5000;
                                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(" [ " + client.Player.Name + " ] He Won [Infinity War] AnD Got 5.000 CPs ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.TeleportTC();
                            }
                            else
                            {
                                data.AddText("You Can Only Claim The Reward If You Kill Everyone In The War First.\n");
                                data.AddText("All Players In The War [ " + alive + " ]");
                                data.AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, You Can Only Claim The Reward When The War Ends First.");
                            data.AddOption("Okay Thanks.", 255).FinalizeDialog();
                            break;
                        }
                        break;
                    }
                #endregion
            }
        }

        //[NpcAttribute(NpcID.TitanWarPrize)]
        //public static void TitanWarPrize(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                data.AddText("[Titan War] Ended At Minute [ x40 ] \n");
        //                data.AddOption("Claim~Reward.", 1);
        //                data.AddOption("I~will~come~later.", 255).FinalizeDialog();
        //                break;
        //            }
        //        #region Titan War Prize
        //        case 1:
        //            {
        //                if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
        //                {
        //                    client.SendSysMesage("Sorry, You are dead.");
        //                    break;
        //                }

        //                int alive = 0;
        //                DateTime Now64 = DateTime.Now;
        //                foreach (Client.GameClient Client in Pool.GamePoll.Values)
        //                    if (Client.Player.Map == 12602 && Client.Player.Alive && client.Player.HitPoints >= 1)
        //                        alive++;
        //                if (DateTime.Now.Minute >= 40)
        //                {
        //                    if (alive == 1)
        //                    {
        //                        client.Player.ConquerPoints += 5000;
        //                        Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(" [ " + client.Player.Name + " ] He Won [Titan War] AnD Got 5.000 CPs ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
        //                        client.TeleportTC();
        //                    }
        //                    else
        //                    {
        //                        data.AddText("You Can Only Claim The Reward If You Kill Everyone In The War First.\n");
        //                        data.AddText("All Players In The War [ " + alive + " ]");
        //                        data.AddOption("Okay.", 255).FinalizeDialog();
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, You Can Only Claim The Reward When The War Ends First.");
        //                    data.AddOption("Okay Thanks.", 255).FinalizeDialog();
        //                    break;
        //                }
        //                break;
        //            }
        //        #endregion
        //    }
        //}

        //[NpcAttribute(NpcID.MortalKnightsWarPrize)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void MortalKnightsWarPrize(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        data.AddText("[Mortal Knights War] Ended At Minute [ x45 ] \n");
                        data.AddOption("Claim~Reward.", 1);
                        data.AddOption("I~will~come~later.", 255).FinalizeDialog();
                        break;
                    }
                #region Mortal Knights War Prize
                case 1:
                    {
                        if (client.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Dead))
                        {
                            client.SendSysMesage("Sorry, You are dead.");
                            break;
                        }

                        int alive = 0;
                        DateTime Now64 = DateTime.Now;
                        foreach (Client.GameClient Client in Pool.GamePoll.Values)
                            if (Client.Player.Map == 12275 && Client.Player.Alive && client.Player.HitPoints >= 1)
                                alive++;
                        if (DateTime.Now.Minute >= 45)
                        {
                            if (alive == 1)
                            {
                                client.Player.ConquerPoints += 5000;
                                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(" [ " + client.Player.Name + " ] He Won [Mortal Knights War] AnD Got 5.000 CPs ", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                client.TeleportTC();
                            }
                            else
                            {
                                data.AddText("You Can Only Claim The Reward If You Kill Everyone In The War First.\n");
                                data.AddText("All Players In The War [ " + alive + " ]");
                                data.AddOption("Okay.", 255).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, You Can Only Claim The Reward When The War Ends First.");
                            data.AddOption("Okay Thanks.", 255).FinalizeDialog();
                            break;
                        }
                        break;
                    }
                #endregion
            }
        }

        [NpcAttribute(NpcID.PromotionFull)]
        public static void PromotionFull(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey , Iam Selling Full Class Promotion For 10.000 CPs.");
                        data.AddOption("I want Full Promotion [Trojan]. ", 1);
                        data.AddOption("I want Full Promotion [Warrior]. ", 2);
                        data.AddOption("I want Full Promotion [Archer]. ", 3);
                        data.AddOption("I want Full Promotion [Ninja]. ", 4);
                        data.AddOption("I want Full Promotion [Monk]. ", 5);
                        data.AddOption("I want Full Promotion [Pirate]. ", 6);
                        data.AddOption("I want Full Promotion [DragonWarrior]. ", 7);
                        data.AddOption("I want Full Promotion [ThunderStrike]. ", 8);
                        data.AddOption("I want Full Promotion [Water]. ", 9);
                        data.AddOption("I want Full Promotion [Fire]. ", 10);
                        data.AddOption("I want Full Promotion [WindWalker]. ", 11);
                        data.AddOption("I want Full Promotion [Dune]. ", 12);
                        data.AddOption("I don`t want to buy now.");
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                #region Trojan
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                if (client.Player.Class == 1057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 1057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Trojan.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Warrior
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                if (client.Player.Class == 2057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 2057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Warrior.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Archer
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            {
                                if (client.Player.Class == 4057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 4057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Archer.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Ninja
                case 4:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                if (client.Player.Class == 5057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 5057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Ninja.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Monk
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                if (client.Player.Class == 6057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 6057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Monk.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Pirate
                case 6:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            {
                                if (client.Player.Class == 7057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 7057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Pirate.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region DragonWarrior
                case 7:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsLee(client.Player.Class))
                            {
                                if (client.Player.Class == 8057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 8057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Dragon Warrior.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region ThunderStrike
                case 8:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                            {
                                if (client.Player.Class == 9057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 9057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Thunder Strike.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Water
                case 9:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                if (client.Player.Class == 13057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 13057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Water.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Fire
                case 10:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                if (client.Player.Class == 14057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 14057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Fire.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Windwalker
                case 11:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            {
                                if (client.Player.Class == 16057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 16057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Wind Walker.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region Dune
                case 12:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            if (Database.AtributesStatus.IsDune(client.Player.Class))
                            {
                                if (client.Player.Class == 3057)
                                {
                                    client.CreateBoxDialog("You already have the higher promotion class.");
                                    return;
                                }
                                else
                                {
                                    client.Player.ConquerPoints -= 10000;
                                    client.Player.Class = 3057;
                                    client.Player.SendUpdate(stream, client.Player.Class, MsgUpdate.DataType.Class);
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante, false);
                                }
                            }
                            else
                            {
                                data.Text("Sorry you not Dune Wanderere.");
                                data.AddOption("THX .", 255);
                                data.AddAvatar(67).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.Text("Please check your CPS ...");

                            data.Option("I~See.", 255).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
            }
        }
        // ===== Security helpers for account-credential NPC inputs =====
        // Restrict secrets to a safe character set. This blocks quote/backslash/CR/LF injection
        // (defence in depth on top of MySqlEscape) and keeps the value safe to store in INI files.
        private static bool IsSafeSecret(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            foreach (char c in value)
            {
                bool ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9');
                if (!ok)
                    return false;
            }
            return true;
        }

        private static bool IsSafeEmailName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            foreach (char c in value)
            {
                bool ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')
                          || c == '.' || c == '_' || c == '-';
                if (!ok)
                    return false;
            }
            return true;
        }

        // [sqli] Parameterized access to `accounts` for the HelpDesk password/email flow (was the
        // string-building DBFunctionality.MySqlCommand builder). `column` is always a constant from this file.
        // Returns false when no row matches; a NULL column reads as "" (same as MySqlReader.ReadString).
        private static bool ReadAccountColumnByEntityId(uint entityId, string column, out string value)
        {
            value = "";
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = new MySqlConnector.MySqlCommand("SELECT * FROM `accounts` WHERE `EntityID` = @eid", conn))
            {
                cmd.Parameters.AddWithValue("@eid", entityId);
                cmd.CommandTimeout = 30;
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                        return false;
                    object v = rdr[column];
                    value = (v == null || v is DBNull) ? "" : v.ToString();
                    return true;
                }
            }
        }

        private static int UpdateAccountColumnByEntityId(uint entityId, string column, string value)
        {
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = new MySqlConnector.MySqlCommand("UPDATE `accounts` SET `" + column + "` = @val WHERE `EntityID` = @eid", conn))
            {
                cmd.Parameters.AddWithValue("@val", value ?? "");
                cmd.Parameters.AddWithValue("@eid", entityId);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // [sqli] SecurityAnima forget/remove pin: does an `accounts` row with this ID and Email exist?
        private static bool AccountExistsByIdAndEmail(uint id, string email)
        {
            using (var conn = DataHolder.MySqlConnection)
            using (var cmd = new MySqlConnector.MySqlCommand("SELECT * FROM `accounts` WHERE `ID` = @id AND `Email` = @email", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@email", email ?? "");
                cmd.CommandTimeout = 30;
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                    return rdr.Read();
            }
        }

        //[NpcAttribute(NpcID.HelpDeskInfinity)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void HelpDeskInfinity(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Welcome to Server Infinity How Can I Help You !");
                        dialog.AddOption("Change Password.", 30);
                        dialog.AddOption("Change Email.", 33);
                      //  dialog.AddOption("Website Game.", 100);
                      //  dialog.AddOption("Create New Account.", 106);
                       // dialog.AddOption("Group Facebook.", 101);
                       // dialog.AddOption("Page Facebook.", 104);
                      //  dialog.AddOption("Channel YouTube", 105);
                        dialog.AddOption("I~will~come~later.", 255);
                        dialog.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 100: // Link Website Game
                    {
                        client.SendSysMesage("https://conquergodzila.com/", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 101: // Group Facebook Game
                    {
                        client.SendSysMesage("https://www.facebook.com/share/g/1DdZFb3waV/", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }                                             
                case 104: // Link Page Facebook
                    {
                        client.SendSysMesage("https://www.facebook.com/share/1cetdc6aKU/", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 105: // Link Page Channel Youtube
                    {
                        client.SendSysMesage("https://youtube.com/@godzilla_conquer?si=bEfPSUVYlUa2lw8i", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                case 106: // Link Create New Account
                    {
                        client.SendSysMesage("https://conquergodzila.com/register", MsgMessage.ChatMode.WebSite, MsgMessage.MsgColor.red, false);
                        break;
                    }
                #region Change Password
                case 30:
                    {
                        // Start of the change-password flow: clear any previous verification.
                        client.PwdChangeVerified = false;
                        dialog.AddText("Write Your old password.");
                        dialog.AddInput("Old Password", 31);
                        dialog.AddOption("Later.", 255);
                        dialog.AddAvatar(0).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        try
                        {
                            client.PwdChangeVerified = false;
                            string oldpass = Input;
                            if (oldpass.Length < 6 || oldpass.Length > 16)
                            {
                                dialog.AddText("Password must be between 6 and 16 characters.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                                break;
                            }
                            string dbPassword;
                            {
                                {
                                    if (ReadAccountColumnByEntityId(client.Player.UID, "Password", out dbPassword))
                                    {
                                        if (oldpass == dbPassword)
                                        {
                                            // Old password confirmed on this session: allow the write step.
                                            client.PwdChangeVerified = true;
                                            dialog.AddText("Write Your new password.");
                                            dialog.AddInput("New Password", 32);
                                            dialog.AddOption("Later.", 255);
                                            dialog.AddAvatar(0).FinalizeDialog();
                                        }
                                        else
                                        {
                                            dialog.AddText("Wrong old password.");
                                            dialog.AddOption("Ok", 255);
                                            dialog.AddAvatar(0).FinalizeDialog();
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception)
                        {
                            client.Player.MessageBox("Something went wrong", null, null);
                        }
                        break;
                    }
                case 32:
                    {
                        try
                        {
                            // Refuse the write unless the old password was verified in option 31
                            // during this session (the client controls the option number).
                            if (!client.PwdChangeVerified)
                            {
                                client.PwdChangeVerified = false;
                                dialog.AddText("Please verify your old password first.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                                break;
                            }

                            string newpass = Input;

                            if (newpass.Length < 6 || newpass.Length > 16 || !IsSafeSecret(newpass))
                            {
                                dialog.AddText("Password must be 6-16 letters or digits.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                                break;
                            }
                            // Single-use: consume the verification immediately.
                            client.PwdChangeVerified = false;
                            if (UpdateAccountColumnByEntityId(client.Player.UID, "Password", newpass) > 0)
                            {
                                dialog.AddText("Password changed successfully.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                            }
                            else
                            {
                                client.Player.MessageBox("Something went wrong", null, null);
                            }
                        }
                        catch (Exception)
                        {
                            client.Player.MessageBox("Something went wrong", null, null);
                        }
                        break;
                    }
                case 33:
                    {
                        // Start of the change-email flow: clear any previous verification.
                        client.EmailChangeVerified = false;
                        dialog.AddText("Write Your old Email Name.");
                        dialog.AddInput("Old Email Name", 34);
                        dialog.AddOption("Later.", 255);
                        dialog.AddAvatar(0).FinalizeDialog();
                        break;
                    }

                case 34:
                    {
                        try
                        {
                            client.EmailChangeVerified = false;
                            string oldpass = Input;
                            if (oldpass.Length < 3 || oldpass.Length > 16)
                            {
                                dialog.AddText("Email name length invalid.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                                break;
                            }

                            string dbEmail;
                            {
                                {
                                    if (ReadAccountColumnByEntityId(client.Player.UID, "Email", out dbEmail))
                                    {
                                        string emailName = dbEmail.Split('@')[0];

                                        if (oldpass == emailName)
                                        {
                                            // Old email confirmed on this session: allow the write step.
                                            client.EmailChangeVerified = true;
                                            dialog.AddText("Write Your new Email Name.");
                                            dialog.AddInput("New Email", 35);
                                            dialog.AddOption("Later.", 255);
                                            dialog.AddAvatar(0).FinalizeDialog();
                                        }
                                        else
                                        {
                                            dialog.AddText("Wrong old email.");
                                            dialog.AddOption("Ok", 255);
                                            dialog.AddAvatar(0).FinalizeDialog();
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            client.Player.MessageBox("Something went wrong", null, null);
                        }
                        break;
                    }

                case 35:
                    {
                        try
                        {
                            // Refuse the write unless the old email was verified in option 34 this session.
                            if (!client.EmailChangeVerified)
                            {
                                client.EmailChangeVerified = false;
                                dialog.AddText("Please verify your old email first.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                                break;
                            }

                            string newEmail = Input;
                            if (newEmail.Length < 3 || newEmail.Length > 16 || !IsSafeEmailName(newEmail))
                            {
                                dialog.AddText("Email name must be 3-16 letters, digits, '.', '_' or '-'.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                                break;
                            }

                            // Single-use: consume the verification immediately.
                            client.EmailChangeVerified = false;
                            if (UpdateAccountColumnByEntityId(client.Player.UID, "Email", newEmail) > 0)
                            {
                                dialog.AddText("Email changed successfully.");
                                dialog.AddOption("Ok", 255);
                                dialog.AddAvatar(0).FinalizeDialog();
                            }
                            else
                            {
                                client.Player.MessageBox("Something went wrong", null, null);
                            }
                        }
                        catch
                        {
                            client.Player.MessageBox("Something went wrong", null, null);
                        }
                        break;
                    }
                #endregion
            }
        }
        [NpcAttribute(NpcID.OpenEonspiritSystem)]
        public static void OpenEonspiritSystem(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Reborn < 2 || client.Player.Level < 120)
                        {
                            data.AddText("You must be Second Reborn and at least Level 120 to unlock the Eonspirit System.")
                                .AddOption("Alright.", 255)
                                .AddAvatar(63).FinalizeDialog();
                            return;
                        }
                        data.AddText("Hello, You can unlock the Eonspirit System here for free. Would you like to unlock it now? \n");
                        data.Option("Yeap, Unlock [Eonspirit Advent].", 1);
                        data.Option("No, I~will~come~later.", 255);
                        data.AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.UnLockedSystem == false)
                        {
                            client.Player.UnLockedSystem = true;
                            MsgYuanshen.RemvedGUI(client);
                            var FirstItem = new MsgGameItem();
                            FirstItem.UID = Pool.ITEM_Counter.Next;
                            FirstItem.ITEM_ID = 10020101;
                            var DB = Pool.ItemsBase[FirstItem.ITEM_ID];
                            FirstItem.Durability = FirstItem.MaximDurability = DB.Durability;
                            FirstItem.Bound = 1;
                            client.EonspiritSystem.Add(FirstItem.UID, FirstItem);
                            FirstItem.Mode = Role.Flags.ItemMode.AddItem;
                            FirstItem.Position = (ushort)Role.Enums.EonspiritPosition.EonspiritInventory;
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.AddOrRemoveEonspirit, FirstItem.UID, 0, 0, 0, 0, 0, null, 0));
                            FirstItem.Send(client, stream);
                            var SecondItem = new MsgGameItem();
                            SecondItem.UID = Pool.ITEM_Counter.Next;
                            SecondItem.ITEM_ID = 10040101;
                            DB = Pool.ItemsBase[SecondItem.ITEM_ID];
                            SecondItem.Durability = SecondItem.MaximDurability = DB.Durability;
                            SecondItem.Bound = 1;
                            client.EonspiritSystem.Add(SecondItem.UID, SecondItem);
                            SecondItem.Mode = Role.Flags.ItemMode.AddItem;
                            SecondItem.Position = (ushort)Role.Enums.EonspiritPosition.EonspiritInventory;
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.AddOrRemoveEonspirit, SecondItem.UID, 0, 0, 0, 0, 0, null, 0));
                            SecondItem.Send(client, stream);
                            var ThirdItem = new MsgGameItem();
                            ThirdItem.UID = Pool.ITEM_Counter.Next;
                            ThirdItem.ITEM_ID = 10100101;
                            DB = Pool.ItemsBase[ThirdItem.ITEM_ID];
                            ThirdItem.Durability = ThirdItem.MaximDurability = DB.Durability;
                            ThirdItem.Bound = 1;
                            client.EonspiritSystem.Add(ThirdItem.UID, ThirdItem);
                            ThirdItem.Mode = Role.Flags.ItemMode.AddItem;
                            ThirdItem.Position = (ushort)Role.Enums.EonspiritPosition.EonspiritInventory;
                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.AddOrRemoveEonspirit, ThirdItem.UID, 0, 0, 0, 0, 0, null, 0));
                            ThirdItem.Send(client, stream);
                            var Eonspirit333 = new MsgYuanshen();
                            Eonspirit333.Type = (byte)MsgYuanshen.ActionID.Flag;
                            Eonspirit333.EonspiritItem = new uint[1] { 15 };
                            client.Send(Eonspirit333);
                            MsgYuanshen.OpenGUI(client);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HuntingMaps)]
        public static void HuntingMaps(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Where would you like to hunt today?")
                         .AddOption("Enter Hunting Map 1", 1)
                         .AddOption("Enter Hunting Map 2", 2)
                         .AddOption("Enter Hunting Map 3", 3)
                         .AddOption("Enter Hunting Map 4", 4)
                         .AddOption("I'll stay here.", 255)
                         .AddAvatar(3642).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        switch (Program.GetRandom.Next(0, 6))
                        {
                            case 0:
                                client.Teleport(221, 225, 31254);
                                break;

                            case 1:
                                client.Teleport(232, 438, 31254);
                                break;

                            case 2:
                                client.Teleport(461, 223, 31254);
                                break;

                            case 3:
                                client.Teleport(308, 296, 31254);
                                break;

                            case 4:
                                client.Teleport(401, 400, 31254);
                                break;

                            case 5:
                                client.Teleport(389, 255, 31254);
                                break;
                        }

                        break;
                    }
                case 2:
                    {
                        switch (Program.GetRandom.Next(0, 6))
                        {
                            case 0:
                                client.Teleport(221, 225, 31255);
                                break;

                            case 1:
                                client.Teleport(232, 438, 31255);
                                break;

                            case 2:
                                client.Teleport(461, 223, 31255);
                                break;

                            case 3:
                                client.Teleport(308, 296, 31255);
                                break;

                            case 4:
                                client.Teleport(401, 400, 31255);
                                break;

                            case 5:
                                client.Teleport(389, 255, 31255);
                                break;
                        }
                        break;
                    }
                case 3:
                    {
                        switch (Program.GetRandom.Next(0, 6))
                        {
                            case 0:
                                client.Teleport(221, 225, 31256);
                                break;

                            case 1:
                                client.Teleport(232, 438, 31256);
                                break;

                            case 2:
                                client.Teleport(461, 223, 31256);
                                break;

                            case 3:
                                client.Teleport(308, 296, 31256);
                                break;

                            case 4:
                                client.Teleport(401, 400, 31256);
                                break;

                            case 5:
                                client.Teleport(389, 255, 31256);
                                break;
                        }
                        break;
                    }
                case 4:
                    {
                        switch (Program.GetRandom.Next(0, 6))
                        {
                            case 0:
                                client.Teleport(221, 225, 31257);
                                break;

                            case 1:
                                client.Teleport(232, 438, 31257);
                                break;

                            case 2:
                                client.Teleport(461, 223, 31257);
                                break;

                            case 3:
                                client.Teleport(308, 296, 31257);
                                break;

                            case 4:
                                client.Teleport(401, 400, 31257);
                                break;

                            case 5:
                                client.Teleport(389, 255, 31257);
                                break;
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ThunderGloryGem)]
        public static void ThunderGloryGem(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3311693, 1))
            {
                client.SendSysMesage("Sorry, you don`t have a Optional Super Gem Pack in your inventory.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Open to select one from Super GloryGem or Super ThunderGem.");
                        dialog.Option("Super Thunder Gem.", 1);
                        dialog.Option("Super Glory Gem.", 2);
                        dialog.Option("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3311693, 1, stream);
                        client.Inventory.Add(stream, 700103, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3311693, 1, stream);
                        client.Inventory.Add(stream, 700123, 1);
                        break;
                    }

            }
        }
        public static void DragonTacticsAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3321817, 1))
            {
                client.SendSysMesage("Sorry, you don`t have a Dragon Tactics Pack in your inventory.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Open to get a Dragon Tactics (A) or a Dragon Tactics (B).");
                        dialog.Option("Dragon~Tactics~(A).", 1);
                        dialog.Option("Dragon~Tactics~(B).", 2);
                        dialog.Option("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3321817, 1, stream);
                        client.Inventory.Add(stream, 3005397, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3321817, 1, stream);
                        client.Inventory.Add(stream, 3005398, 1);
                        break;
                    }

            }
        }
        public static void BoundlessHeartAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3321818, 1))
            {
                client.SendSysMesage("Sorry, you don`t have a Boundless Heart Pack in your inventory.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Open to get a Boundless Heart (A) or a Boundless Heart (B) or a Boundless Heart (C).");
                        dialog.Option("Boundless~Heart~(A).", 1);
                        dialog.Option("Boundless~Heart~(B).", 2);
                        dialog.Option("Boundless~Heart~(C).", 3);
                        dialog.Option("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3321818, 1, stream);
                        client.Inventory.Add(stream, 3007115, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3321818, 1, stream);
                        client.Inventory.Add(stream, 3007116, 1);
                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3321818, 1, stream);
                        client.Inventory.Add(stream, 3007117, 1);
                        break;
                    }

            }
        }
        public static void DoctrineofDeityAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3321819, 1))
            {
                client.SendSysMesage("Sorry, you don`t have a Doctrine of Deity Pack in your inventory.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Open to get a Doctrine of Deity (A) or a Doctrine of Deity (B) or a Doctrine of Deity (C).");
                        dialog.Option("Doctrine~of~Deity~(A).", 1);
                        dialog.Option("Doctrine~of~Deity~(B).", 2);
                        dialog.Option("Doctrine~of~Deity~(C).", 3);
                        dialog.Option("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3321819, 1, stream);
                        client.Inventory.Add(stream, 3005399, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3321819, 1, stream);
                        client.Inventory.Add(stream, 3005400, 1);
                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3321819, 1, stream);
                        client.Inventory.Add(stream, 3005401, 1);
                        break;
                    }

            }
        }
        public static void PuzzleofLifeAB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3321820, 1))
            {
                client.SendSysMesage("Sorry, you don`t have a Puzzle of Life Pack in your inventory.");
                return;
            }

            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Open to get a Puzzle of Life (A) or a Puzzle of Life (B) or a Puzzle of Life (C).");
                        dialog.Option("Puzzle~of~Life~(A).", 1);
                        dialog.Option("Puzzle~of~Life~(B).", 2);
                        dialog.Option("Puzzle~of~Life~(C).", 3);
                        dialog.Option("Leave.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Inventory.Remove(3321820, 1, stream);
                        client.Inventory.Add(stream, 3007230, 1);
                        break;
                    }
                case 2:
                    {
                        client.Inventory.Remove(3321820, 1, stream);
                        client.Inventory.Add(stream, 3007231, 1);
                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(3321820, 1, stream);
                        client.Inventory.Add(stream, 3007232, 1);
                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.ArchivesMonk)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void ArchivesMonk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hello, Sir I will Help You To Open Your Monk Archive\n");
                        //  data.Option("OpenGolden", 4);
                        data.Option("Open Heavenl", 1);
                        data.Option("Open Mighty", 2);
                        data.Option("Open Cosmic", 3);

                        data.Option("i,See!", 255);

                        data.AddAvatar(118).FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.HeavenlyTiger, 1, 0, 1, 0);
                        break;

                    }
                case 2:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.MightyDragon, 1, 0, 1, 0);
                        break;

                    }
                case 3:
                    {

                        client.MyArchives.AddItem(Archives.TypeID.CosmicRoc, 1, 0, 1, 0);
                        break;

                    }


            }
        }
        [NpcAttribute(NpcID.Relic)]
        public static void Relic(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Please choose one from the following Relic ")

                      .AddOption("Full~Relic~MaxHP", 1)
                      .AddOption("Full~Relic~PAttack", 2)
                      .AddOption("Full~Relic~Break", 3)
                      .AddOption("Full~Relic~Strike", 4)
                      .AddOption("Full~Relic~AntiBreak", 5)
                       .AddOption("Full~Relic~Immunity", 6)
                          .AddOption("Full~Relic~MAttack", 7)
                          .AddOption("Full~Relic~MStrike", 8)
                          .AddOption("Full~Relic~LuckyStike", 9)
                        .AddAvatar(288)

                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 6, 6, 6, 6, 6);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 7, 7, 7, 7, 7);
                            client.Inventory.Remove(3314913, 1, stream);
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 4, 4, 4, 4, 4);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 1, 1, 1, 1, 1);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 5, 5, 5, 5, 5);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;
                    }

                case 6:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 3, 3, 3, 3, 3);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 8, 8, 8, 8, 8);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;

                    }
                case 8:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 2, 2, 2, 2, 2);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;

                    }
                case 9:
                    {
                        if (client.Inventory.Contain(3314913, 1, 0))
                        {
                            client.Inventory.Relice(stream, false, true, 14, 14, 14, 14, 14);
                            client.Inventory.Remove(3314913, 1, stream);
                        }
                        break;

                    }
            }
        }              
        //[NpcAttribute(NpcID.FullAccount)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void FullAccount(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            // Security fix: this is a debug/giveaway NPC that hands out ConquerPoints, items and
            // max-perfection gear with no cost. Gate the ENTIRE handler behind an authoritative
            // staff check so no normal client can trigger any case. AccountState comes from the
            // encrypted login token and cannot be forged by the client.
            if (!client.IsStaffAccount && !client.ProjectManager)
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey , From here, you can buy Some Things \n");

                        if (client.Player.Level >= 100 && client.Player.Reborn >= 2)
                        {
                            if (client.Player.InnerPower.Stages[0].UnLocked == false)
                            {
                                data.AddOption("InnerPower", 19);
                            }
                        }
                        data.AddOption("NewRun", 111);
                        data.AddOption("RedBodhi[40]", 16);
                        //data.AddOption("Anima[P18]", 17);
                        data.AddOption("RedRunes", 12);
                        data.AddOption("YellowRunes", 14);
                        data.AddOption("BlueRunes", 13);
                        data.AddOption("RandomRelic", 112);
                        data.AddOption("InnerPower", 19);
                        data.AddOption("SupClass", 22);
                        data.AddOption("MythSoul", 140);
                        data.AddOption("chilevel", 186);
                        data.AddOption("SacredClass", 187);
                        //data.AddOption("Cps", 40);
                        //data.AddOption("Money", 60);
                        data.AddOption("Vip", 151);
                        data.AddOption("LordToken", 45);
                        data.AddOption("Cosmos Hammer", 141);
                        data.AddOption("RedBodhiBean", 110);
                        data.AddOption("Add Perfectionlevel All Items", 30);
                        data.AddOption("Just passing by..");
                        data.AddAvatar(263).FinalizeDialog();
                        break;
                    }
                #region MythSoul

                case 140:
                    {
                        client.Inventory.AddItemWitchStack(3326003, 0, 50, stream);

                        break;
                    }
                #endregion
                #region MythSoul

                case 111:
                    {
                        client.Inventory.AddItemWitchStack(4038009, 0, 1, stream);

                        break;
                    }
                #endregion

                #region Cosmos Hammer

                case 141:
                    {
                        client.Inventory.AddItemWitchStack(3324226, 0, 500, stream);

                        break;
                    }
                #endregion
                #region SacredClass

                case 187:
                    {
                        client.Player.ClassExperience += 1500000;

                        break;
                    }
                #endregion
                #region chilevel
                case 186:
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
                #endregion
                #region FullRelic
                case 112:
                    {
                        client.Inventory.Add(stream, 3314913, 1);

                        break;
                    }
                #endregion
                #region RedBodhiBean
                case 110:
                    {
                        client.Inventory.AddItemWitchStack(3009104, 0, 9888, stream);

                        break;
                    }
                #endregion
                #region ConquerPoints
                case 40:
                    {
                        client.Player.ConquerPoints += 1000000000;


                        break;
                    }
                #endregion
                #region VipLevel [ 7 ]
                case 151:
                    {
                        if (client.Player.Money >= 1)
                        {
                            if (client.Player.VipLevel >= 0 && client.Player.VipLevel <= 6)
                            {
                                client.Player.Money -= 1;
                                client.Player.ExpireVip = DateTime.Now.AddHours(12);

                                client.Player.VipLevel = 7;
                                client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                                client.Player.UpdateVip(stream);
                                client.SendSysMesage("You have 12 hour(s) of VIP service remaining.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);

                            }

                        }

                        break;
                    }
                #endregion
                //#region Money
                //case 60:
                //    {
                //        client.Player.Money += 7000000000;
                //        break;
                //    }
                //#endregion
                #region LordToken
                case 45:
                    {
                        client.Inventory.Add(stream, 723467, 1);

                        break;
                    }
                #endregion
                case 30:
                    {
                        MsgGameItem item1;
                        MsgGameItem item2;
                        MsgGameItem item3;
                        MsgGameItem item4;
                        MsgGameItem item5;
                        MsgGameItem item6;
                        MsgGameItem item7;
                        MsgGameItem item8;
                        MsgGameItem item9;
                        MsgGameItem item10;
                        MsgGameItem item11;
                        MsgGameItem item12;
                        item1 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Necklace);
                        item2 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Armor);
                        item3 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Ring);
                        item4 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Boots);
                        item5 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Head);
                        item6 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Tower);
                        item7 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Fan);
                        item8 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RidingCrop);
                        item9 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Steed);
                        item10 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon);
                        item11 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon);
                        item12 = client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Wing);

                        if (item1 != null)
                        {
                            item1.PerfectionProgress = 0;
                            item1.PerfectionLevel = 54;
                            item1.OwnerName = client.Player.Name;
                            item1.OwnerUID = client.Player.UID;
                            item1.Mode = Role.Flags.ItemMode.Update;
                            item1.Send(client, stream);
                        }

                        if (item2 != null)
                        {
                            item2.PerfectionProgress = 0;
                            item2.PerfectionLevel = 54;
                            item2.OwnerName = client.Player.Name;
                            item2.OwnerUID = client.Player.UID;
                            item2.Mode = Role.Flags.ItemMode.Update;
                            item2.Send(client, stream);
                        }
                        if (item3 != null)
                        {
                            item3.PerfectionProgress = 0;
                            item3.PerfectionLevel = 54;
                            item3.OwnerName = client.Player.Name;
                            item3.OwnerUID = client.Player.UID;
                            item3.Mode = Role.Flags.ItemMode.Update;
                            item3.Send(client, stream);
                        }
                        if (item4 != null)
                        {
                            item4.PerfectionProgress = 0;
                            item4.PerfectionLevel = 54;
                            item4.OwnerName = client.Player.Name;
                            item4.OwnerUID = client.Player.UID;
                            item4.Mode = Role.Flags.ItemMode.Update;
                            item4.Send(client, stream);
                        }
                        if (item5 != null)
                        {
                            item5.PerfectionProgress = 0;
                            item5.PerfectionLevel = 54;
                            item5.OwnerName = client.Player.Name;
                            item5.OwnerUID = client.Player.UID;
                            item5.Mode = Role.Flags.ItemMode.Update;
                            item5.Send(client, stream);
                        }

                        if (item6 != null)
                        {
                            item6.PerfectionProgress = 0;
                            item6.PerfectionLevel = 54;
                            item6.OwnerName = client.Player.Name;
                            item6.OwnerUID = client.Player.UID;
                            item6.Mode = Role.Flags.ItemMode.Update;
                            item6.Send(client, stream);
                        }
                        if (item7 != null)
                        {
                            item7.PerfectionProgress = 0;
                            item7.PerfectionLevel = 54;
                            item7.OwnerName = client.Player.Name;
                            item7.OwnerUID = client.Player.UID;
                            item7.Mode = Role.Flags.ItemMode.Update;
                            item7.Send(client, stream);
                        }

                        if (item8 != null)
                        {
                            item8.PerfectionProgress = 0;
                            item8.PerfectionLevel = 54;
                            item8.OwnerName = client.Player.Name;
                            item8.OwnerUID = client.Player.UID;
                            item8.Mode = Role.Flags.ItemMode.Update;
                            item8.Send(client, stream);
                        }
                        if (item9 != null)
                        {
                            item9.PerfectionProgress = 0;
                            item9.PerfectionLevel = 54;
                            item9.OwnerName = client.Player.Name;
                            item9.OwnerUID = client.Player.UID;
                            item9.Mode = Role.Flags.ItemMode.Update;
                            item9.Send(client, stream);
                        }
                        if (item10 != null)
                        {
                            item10.PerfectionProgress = 0;
                            item10.PerfectionLevel = 54;
                            item10.OwnerName = client.Player.Name;
                            item10.OwnerUID = client.Player.UID;
                            item10.Mode = Role.Flags.ItemMode.Update;
                            item10.Send(client, stream);
                        }
                        if (item11 != null)
                        {
                            item11.PerfectionProgress = 0;
                            item11.PerfectionLevel = 54;
                            item11.OwnerName = client.Player.Name;
                            item11.OwnerUID = client.Player.UID;
                            item11.Mode = Role.Flags.ItemMode.Update;
                            item11.Send(client, stream);
                        }
                        if (item12 != null)
                        {
                            item12.PerfectionProgress = 0;
                            item12.PerfectionLevel = 54;
                            item12.OwnerName = client.Player.Name;
                            item12.OwnerUID = client.Player.UID;
                            item12.Mode = Role.Flags.ItemMode.Update;
                            item12.Send(client, stream);
                        }
                        client.UpdatePerfectionLevel(stream);
                        client.Equipment.QueryEquipment(client.Equipment.Alternante, true);
                        break;
                    }
                #region PackRunes
                case 22:
                    {
                        if (client.Player.ConquerPoints >= 10000)
                        {
                            client.Player.ConquerPoints -= 10000;
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.MartialArtist, client, stream);
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.Warlock, client, stream);
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.ChiMaster, client, stream);
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.Sage, client, stream);
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.Apothecary, client, stream);
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.Wrangler, client, stream);
                            client.Player.SubClass.LearnFixedSubClass(Database.DBLevExp.Sort.Performer, client, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e300");
                        }
                        else client.SendSysMesage("You don`t have 10,000 ConquerPoints .");
                        break;
                    }
                case 20:
                    {
                        var action = new ActionQuery
                        {
                            ObjId = client.Player.UID,
                            Type = Game.MsgServer.ActionType.SetMapColor,
                            dwParam = 0x595959,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
                case 21:
                    {
                        var action = new ActionQuery
                        {
                            ObjId = client.Player.UID,
                            Type = Game.MsgServer.ActionType.SetMapColor,
                            dwParam = 0,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
                case 19:
                    {

                        if (client.Player.ConquerPoints >= 10000)
                        {
                            client.Player.ConquerPoints -= 10000;

                            foreach (var m_stage in client.Player.InnerPower.Stages)
                            {
                                foreach (var m_gong in m_stage.NeiGongs)
                                {
                                    m_stage.UnLocked = m_gong.Unlocked = true;
                                    m_gong.Score = 100;
                                    Database.InnerPowerTable.Stage DBStage = null;
                                    Database.InnerPowerTable.Stage.NeiGong DBGong = null;
                                    if (Database.InnerPowerTable.GetDBInfo(m_gong.ID, out DBStage, out DBGong))
                                        m_gong.level = DBGong.MaxLevel;
                                   // MsgSchedules.SendSysMesage("Congratulations ! [ " + client.Player.Name + " ] Has fixed his [InnerPower] For [ 10,000 ] ConquerPoints Enjoy With [ " + Program.ServerConfig.ServerName + " ] !.", MsgServer.MsgMessage.ChatMode.Talk, MsgServer.MsgMessage.MsgColor.white);

                                }
                            }
                        }
                        else
                        {
                            data.AddText("You do not have 10,000 ConquerPoints with you.")
                           .AddOption("I'm just passing by.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 16:
                    {
                        if (!client.Inventory.HaveSpace(20))
                        {
                            client.CreateBoxDialog("Please make 20 more space in your inventory.");
                            break;
                        }
                        // client.Inventory.Add(stream, 3009104, 20);
                        // client.Inventory.AddItemWitchStack(3009104,0, 20, stream);
                        client.Inventory.AddItemWitchStack(3009104, 0, 40, stream);
                        break;
                    }
                case 17:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                            break;
                        }
                        break;
                    }
                //case 18:
                //    {
                //        if (client.Player.ConquerPoints >= 199999999)
                //        {
                //            client.CreateBoxDialog("Error...");
                //            break;
                //        }
                //        client.Player.ConquerPoints += 20000000;
                //        break;
                //    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(12))
                        {
                            client.CreateBoxDialog("Please make 12 more space in your inventory.");
                            break;
                        }
                        foreach (var item in Pool.ItemsBase.Values)
                        {
                            if (Database.ItemType.isRedRune(item.ID))
                            {
                                if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.Add(rec.GetStream(), item.ID);
                            }
                        }
                        break;
                    }
                case 13:
                    {
                        if (!client.Inventory.HaveSpace(22))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        foreach (var item in Pool.ItemsBase.Values)
                        {
                            if (Database.ItemType.isBlueRune(item.ID))
                            {
                                if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.Add(rec.GetStream(), item.ID);
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        if (!client.Inventory.HaveSpace(36))
                        {
                            client.CreateBoxDialog("Please make 36 more space in your inventory.");
                            break;
                        }
                        foreach (var item in Pool.ItemsBase.Values)
                        {
                            if (Database.ItemType.isYellowRune(item.ID))
                            {
                                if (Database.ItemType.MaxRuneLevel(item.ID) == item.ID % 100)
                                    using (var rec = new ServerSockets.RecycledPacket())
                                        client.Inventory.Add(rec.GetStream(), item.ID);
                            }
                        }
                        break;
                    }
                //case 15:
                //    {
                //        client.Inventory.AddRandomRelic(new ServerSockets.RecycledPacket().GetStream(), false, false);
                //        break;
                //    }
                case 1:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsWarrior(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);

                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 2005;//warrior
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                        client.Socket.Disconnect();
                        break;
                    }
                case 2:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsTrojan(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 1005;//trojan
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);

                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                        client.Socket.Disconnect();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 13005;//water
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                        client.Socket.Disconnect();
                        break;
                    }
                case 4:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 14005;//fire
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Thunder);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cure);
                        client.Socket.Disconnect();
                        break;
                    }
                case 5:
                    {

                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsArcher(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 4005;//archer
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                        client.Socket.Disconnect();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsNinja(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 5005;//ninja
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperTwofoldBlade);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TwilightDance);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalStrike);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                        client.Socket.Disconnect();
                        break;
                    }
                case 7:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsMonk(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 6005;//monk
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Compassion);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WhirlwindKick);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleAttack);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Oblivion);
                        client.Socket.Disconnect();
                        break;
                    }
                case 8:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsPirate(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 7005; //pirate
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CannonBarrage);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeTempest);
                        client.Socket.Disconnect();
                        break;
                    }
                case 9:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsLee(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 8005;//dr
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);

                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonPunch);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonCyclone);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFlow);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirRaid);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirSweep);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirKick);

                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirStrike);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.EarthSweep);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Kick);
                        client.Socket.Disconnect();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsWindWalker(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 16005;//windwallker
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AngerofStomper);

                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.JusticeChant);


                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Circle);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Rectangle);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Sector);
                        client.Socket.Disconnect();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn != 0 && client.Player.Level != 1 || Database.AtributesStatus.IsThunderStriker(client.Player.Class))
                        {
                            client.CreateBoxDialog("Error..!");
                            break;
                        }
                        client.Player.Atributes += 900;
                        client.Player.Agility = 0;
                        client.Player.Strength = 0;
                        client.Player.Vitality = 1;
                        client.Player.Spirit = 0;
                        client.Player.SendUpdate(stream, client.Player.Agility, Game.MsgServer.MsgUpdate.DataType.Agility);
                        client.Player.SendUpdate(stream, client.Player.Strength, Game.MsgServer.MsgUpdate.DataType.Strength);
                        client.Player.SendUpdate(stream, client.Player.Vitality, Game.MsgServer.MsgUpdate.DataType.Vitality);
                        client.Player.SendUpdate(stream, client.Player.Spirit, Game.MsgServer.MsgUpdate.DataType.Spirit);
                        client.Player.SendUpdate(stream, client.Player.Atributes, Game.MsgServer.MsgUpdate.DataType.Atributes);
                        var spells = client.MySpells.ClientSpells.Keys.ToArray();

                        foreach (var spell in spells)
                            using (var rec = new ServerSockets.RecycledPacket())
                                client.MySpells.Remove(spell, rec.GetStream());
                        client.UpdateLevel(stream, 140);
                        client.Player.Reborn = 2;
                        client.Player.Class = 9005;//thunderstrike
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Stigma);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MagicShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.LightningShield);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WindstormBattleaxe);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DevouringStrike);

                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SkyFall);

                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrackingShock);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalAttack1);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalAttack2);
                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NormalAttack3);
                        client.Socket.Disconnect();
                        break;
                    }
                #endregion
            }
        }
        [NpcAttribute(NpcID.CoatWarehouseSale)]
        private static void CoatWarehouseSale(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text111]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text112]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text113]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text114]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text135]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text136]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text137]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text138]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text139]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text1310]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text1311]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text1312]@@");
                        data.Text("STR_ID_tCoatWarehouse_Sale[32173][Text1313]@@");
                        data.Option("STR_ID_tCoatWarehouse_Sale[32173][Option131]@@", 1);

                        data.AddAvatar(100).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var OpenFashion = new ActionQuery() { ObjId = client.Player.UID, dwParam = 1266, Type = 126, Fascing = 2, PositionX = client.Player.X, PositionY = client.Player.Y };
                        client.Send(stream.ActionCreate(OpenFashion));
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Caesar)]
        private static void Caesar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text111]@@");
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text112]@@");
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text113]@@");
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text114]@@");
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text115]@@");
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text116]@@");
                        data.Text("STR_ID_tSummerSalesPromotion[31306][Text117]@@");
                        data.Option("STR_ID_tSummerSalesPromotion[31306][Option141]@@", 1);

                        data.AddAvatar(100).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var OpenFashion = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = 3653,
                            NpcID = 31306,
                            Type = 126,
                            Fascing = 1,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        };
                        client.Send(stream.ActionCreate(OpenFashion));
                        break;
                    }
            }
        }
        public static string RechargeOptionInfo(Client.GameClient user, Role.Player.RechargeType type, uint Cost)
        {
            if (user.Player.ContainRechargeType(type))
                return "(claimed).";

            return "(" + Cost + "Points).";
        }
        [NpcAttribute(NpcID.StudyPoints)]
        public static void StudyPoints(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Hello you can exchange 1k conquer points for 50k study points.");
                        data.AddOption("Exchange", 1)
                            .AddOption("Let me think.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 1000)
                        {
                            client.Player.ConquerPoints -= 1000;

                            client.Player.SubClass.AddStudyPoints(client, 50000, stream);


                            client.CreateBoxDialog("You received 50000 StudyPoints.");
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough ConquerPoints.")
                        .AddOption("Okay.", 255).AddAvatar(7)
                        .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.ExchangeMoneyandCPs)]
        //public static void ExchangeMoneyandCPs(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);

        //    switch (Option)
        //    {

        //        case 0:
        //            {
        //                if (client.IsVendor)
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.InTrade)
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First.");
        //                    return;
        //                }
        //                data.AddText("Hello, You Can Exchange 100.000.000 Gold For 100.000.000 CPs - 100.000.000 CPs For 100.000.000 Gold");
        //                data.AddOption("Exchange 100.000.000 Gold.", 1);
        //                data.AddOption("Exchange 100.000.000 CPs.", 2);
        //                data.AddOption("Let me think.", 255);
        //                data.FinalizeDialog();

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.IsVendor)
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.InTrade)
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.Player.Money >= 100000000)
        //                {
        //                    client.Player.Money -= 100000000;

        //                    client.Player.ConquerPoints += 100000000;


        //                    client.CreateBoxDialog("You Have Successfully Exchanged 100.000.000 Gold For 100.000.000 CPs.");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you don`t have enough Money.");
        //                    data.AddOption("Okay Thanks.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.IsVendor)
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
        //                    return;
        //                }
        //                if (client.InTrade)
        //                {
        //                    client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
        //                    return;
        //                }
        //                if (client.Player.ConquerPoints >= 100000000)
        //                {
        //                    client.Player.ConquerPoints -= 100000000;

        //                    client.Player.Money += 100000000;


        //                    client.CreateBoxDialog("You Have Successfully Exchanged 100.000.000 CPs For 100.000.000 Gold");
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry, but you don`t have enough CPs.");
        //                    data.AddOption("Okay Thanks.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.OpenHalo)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void OpenHalo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Hello, You Can Open Through Me \n [ Wings - Titles - Hallos - Actions - FootPrints - Duo Action - Medals ] All For Free. ");
                        data.AddOption("[Open All Wings].", 6);
                        data.AddOption("[Open All Titles].", 5);
                        data.AddOption("[Open All Hallos].", 1);
                        data.AddOption("[Open All Actions].", 12);
                        data.AddOption("[Open All FootPrints].", 9);
                        data.AddOption("[Open Duo Action].", 253);
                        data.AddOption("[Open All Medals].", 170);
                        data.AddOption("[Open All Avatars]", 234);
                        data.AddOption("[Open All HairStyles]", 235);
                        data.AddOption("[Open All Profile Cards]", 236);
                        data.AddOption("I~want~come~later.", 255);
                        data.FinalizeDialog();

                        break;
                    }
                case 234:
                    {
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.WarDropeFull = 1;
                            client.Player.ConquerPoints -= 1;
                            var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Avatar).ToArray();

                            foreach (var AddAllFrame in AllFrame)
                            {
                                client.HairfaceStorage.Add(AddAllFrame, true);
                            }

                        }
                        else
                        {
                            data.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            data.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 235:
                    {
                        if (client.Player.ConquerPoints >= 1)
                        {
                            client.Player.ConquerPoints -= 1;
                            var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Hairstyle).ToArray();

                            foreach (var AddAllFrame in AllFrame)
                            {
                                client.HairfaceStorage.Add(AddAllFrame, true);
                            }
                        }
                        else
                        {
                            data.Text("Hey hey! you don't have the required amount of Conquer Points!");

                            data.Option("I~see.", 255).FinalizeDialog();
                        }
                        break;
                    }
                case 236:
                    {
                        uint[] ProfileCards =
    {
        100101,
        100102,
        400101,
        600101
    };

                        foreach (uint ID in ProfileCards)
                        {
                            var ProfileCard = Database.HairfaceStorageTable.Hairfaces
                                .FirstOrDefault(i =>
                                    i.Type == MsgHairfaceStorage.Type.ProfileCard &&
                                    i.ID == ID);

                            if (ProfileCard != null)
                            {
                                client.HairfaceStorage.Add(ProfileCard, true);
                            }
                        }

                        break;
                    }

                case 170:
                    {
                        data.AddText("Hello You Can Open All Medals For Free.");
                        data.AddOption("Open Medel 1", 100);
                        data.AddOption("Open Medel 2", 101);
                        data.AddOption("Open Medel 3", 102);
                        data.AddOption("Open Medel 4", 103);
                        data.AddOption("Open Medel 5", 104);
                        data.AddOption("Open Medel 6", 105);
                        data.AddOption("Open Medel 7", 106);
                        data.AddOption("Open Medel 8", 107);
                        data.AddOption("Open Medel 9", 108);
                        data.AddOption("Open Medel 10", 109);
                        data.AddOption("Open Medel 11", 110);
                        data.AddOption("Open Medel 12", 111);
                        data.AddOption("Let me think.", 255).FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        client.MedalStorage.Create(1, (365 * 24 * 60 * 60));
                        break;
                    }
                case 101:
                    {
                        client.MedalStorage.Create(2, (365 * 24 * 60 * 60));
                        break;
                    }
                case 102:
                    {
                        client.MedalStorage.Create(3, (365 * 24 * 60 * 60));
                        break;
                    }
                case 103:
                    {
                        client.MedalStorage.Create(4, (365 * 24 * 60 * 60));
                        break;
                    }
                case 104:
                    {
                        client.MedalStorage.Create(5, (365 * 24 * 60 * 60));
                        break;
                    }
                case 105:
                    {
                        client.MedalStorage.Create(6, (365 * 24 * 60 * 60));
                        break;
                    }
                case 106:
                    {
                        client.MedalStorage.Create(7, (365 * 24 * 60 * 60));
                        break;
                    }
                case 107:
                    {
                        client.MedalStorage.Create(8, (365 * 24 * 60 * 60));
                        break;
                    }
                case 108:
                    {
                        client.MedalStorage.Create(9, (365 * 24 * 60 * 60));
                        break;
                    }
                case 109:
                    {
                        client.MedalStorage.Create(10, (365 * 24 * 60 * 60));
                        break;
                    }
                case 110:
                    {
                        client.MedalStorage.Create(11, (365 * 24 * 60 * 60));
                        break;
                    }
                case 111:
                    {
                        client.MedalStorage.Create(12, (365 * 24 * 60 * 60));
                        break;
                    }
                case 1:
                    {
                        var Halo = Database.TitleStorage.Titles.Values.Where(p => p.SubID > 6126).ToArray();
                        foreach (var info in Halo)
                        {
                            if (!client.Player.HaloTitles.Contains((MsgTitleStorage.HaloType)info.SubID))
                            {
                                client.Player.AddSpecialHalo((Game.MsgServer.MsgTitleStorage.HaloType)info.SubID, stream);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.WarDropeFull = 1;
                        var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Avatar).ToArray();
                        foreach (var AddAllFrame in AllFrame)
                        {
                            client.HairfaceStorage.Add(AddAllFrame, true);
                        }
                        break;
                    }
                case 4:
                    {
                        var AllFrame = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Hairstyle).ToArray();
                        foreach (var AddAllFrame in AllFrame)
                        {
                            client.HairfaceStorage.Add(AddAllFrame, true);
                        }
                        break;
                    }
                case 5:
                    {
                        client.Player.WarDropeFull = 2;
                        var Title = Database.TitleStorage.Titles.Values.Where(p => p.ID >= 1 && p.ID <= 2456).ToArray();
                        foreach (var info in Title)
                        {
                            if (!client.Player.SpecialTitles.Contains((MsgTitleStorage.TitleType)info.SubID))
                            {
                                client.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)info.SubID, stream);
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        client.Player.WarDropeFull = 2;
                        var Wings = Database.TitleStorage.Titles.Values.Where(p => p.ID >= 4001 && p.ID <= 6137).ToArray();
                        foreach (var info in Wings)
                        {
                            if (!client.Player.WingsTitles.Contains((MsgTitleStorage.WingsType)info.SubID))
                            {
                                client.Player.AddSpecialWings((Game.MsgServer.MsgTitleStorage.WingsType)info.SubID, stream);
                            }
                        }
                        break;
                    }
                case 9:
                    {
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.BlazesFootprint), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.BloodwingSpecter), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.LotusGrace), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.LuckyStarshine), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.SakuraGrace), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.ZookosFootprint), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.BlazesFootprint1), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.LotusGrace1), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.FireflyFlit), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.PhoenixGrace), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.DancingPetal), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.AuspiciousGlow), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.SnowbloomStep), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.MoonKiss), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.Bloomjump), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.RadiantReturn), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.AdoringWaltz), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.FushengBurningRipple), stream);
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.StarryFeatherfall), stream);
                        break;
                    }
                case 12:
                    {
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.SupremeAlone2), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.BlazesAction3), stream);
                        client.Player.AddSpecialHaloAction(Game.MsgServer.MsgTitleStorage.HaloAction.Sandsong, stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.BlazesAction), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.FallingSakura), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.MaleficMoon), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.SupremeAlone), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.ZookosAction), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.PhoenixBloom), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.MelodiousStroll), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.LegacyBreeze), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.ZADICOAction), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.FushengLurkingAbyss), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.HeatwaveCheer), stream);
                        client.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.VibrantSummer), stream);
                        break;
                    }
                case 14:
                    {
                        var AllEmoj = Database.HairfaceStorageTable.Hairfaces.Where(i => i.Type == MsgHairfaceStorage.Type.Emoji).ToArray();
                        foreach (var AddAllEmoj in AllEmoj)
                        {
                            client.HairfaceStorage.Add(AddAllEmoj, true);
                        }
                        break;
                    }
                case 253:
                    {
                        client.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.AdoringWaltz), stream);
                    }
                    break;
            }
        }
        [NpcAttribute(NpcID.HuntLevelAndCps)]
        public static void HuntLevelAndCps(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("hey do you want me to send u to The HuntingMaps?")
                        .AddOption("Floor2.", 2)
                       .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();
                        break;
                    }

                case 2:
                    {
                        client.Teleport(140, 140, 15758);
                        break;
                    }


            }
        }
        [NpcAttribute(NpcID.HuntLevelAndCps2)]
        public static void HuntLevelAndCps2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("hey do you want me to send u to The HuntingMaps?")
                        .AddOption("Floor1.", 1)
                       .AddOption("Just~passing~by.", 255).AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(140, 140, 15757);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PointsChange)]
        public static void PointsChange(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Hello i can help you with some extra rewards, you can claim it just 1 time on your life.")
                            .AddText("You can get points from our store (by donation) or you can get points by voting our server once at 12 hours. \n")
                            .AddText("At the moment you have " + client.Player.VotePoints + " Vote Points.");
                        data.AddOption("VIP 4 (7Day) (Bound)." + RechargeOptionInfo(client, Role.Player.RechargeType.Vip6Tokens, 25) + "", 1);
                        data.AddOption("VIP 6 (7Day) (Bound)." + RechargeOptionInfo(client, Role.Player.RechargeType.Vip6Tokens, 50) + "", 2)
                            .AddOption("Let me think.", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.VotePoints >= 25)
                        {
                            if (client.Player.ContainRechargeType(Role.Player.RechargeType.FourVipTokens))
                            {
                                data.AddText("You already claimed VipToken.")
                          .AddOption("Okay.", 255).AddAvatar(7)
                          .FinalizeDialog();
                                break;
                            }
                            client.Player.VotePoints -= 25;
                            client.Inventory.Add(stream, 780010, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You received VIP 4 (7Day).");
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.FourVipTokens);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Vote Points.")
                        .AddOption("Okay.", 255).AddAvatar(7)
                        .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.VotePoints >= 50)
                        {
                            if (client.Player.ContainRechargeType(Role.Player.RechargeType.Vip6Tokens))
                            {
                                data.AddText("You already claimed VipToken.")
                          .AddOption("Okay.", 255).AddAvatar(7)
                          .FinalizeDialog();
                                break;
                            }
                            client.Player.VotePoints -= 50;
                            client.Inventory.Add(stream, 3306220, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You received VIP 6 (7Day).");
                            client.Player.AddRechargeFlag(Role.Player.RechargeType.Vip6Tokens);
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough Vote Points.")
                        .AddOption("Okay.", 255).AddAvatar(7)
                        .FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.StarDragonBall)]
        //public static void StarDragonBall(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("The StarDragonBall has begun! Go and check what you like!.")
        //                    .AddText("You Can Change 1-StarDragonBall to 7-StarDragonBall.")
        //                    .AddOption("ClaimPrize.", 1)//13
        //                    .AddOption("I`ll~think~about~it.", 255)//14
        //                    .AddAvatar(3).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                var SameHWID = Pool.GamePoll.Values.Where(p => p.OnLogin.HWID == client.OnLogin.HWID).ToArray();
        //                var CantClaim = SameHWID.Where(p => p.Player.CanClaim == true).ToArray();
        //                if (CantClaim.Length > 0)
        //                {
        //                    data.AddText("You have already claimed your reward for Prize. You may only vote once every 12 hour.")
        //                  .AddOption("I~see.", 255)
        //                 .AddAvatar(323)
        //                 .FinalizeDialog();
        //                    break;

        //                }
        //                if (Database.StarDragonBall.CanClaim(client))
        //                {
        //                    client.Player.CanClaim = true;
        //                    Database.StarDragonBall.CheckUp(client);
        //                    break;
        //                }
        //                else
        //                {
        //                    data.Text("you already Have a Prize  maybe other account").AddOption("i~see", byte.MaxValue).FinalizeDialog();
        //                }

        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.ConquerLeter)]
        //public static void ConquerLeter(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("The ConquerLeter has begun! Go and check what you like!")
        //                    .AddText("You Can Change C,O,N,Q,U,E,R or Leter14 ")
        //                    .AddOption("Claim C,O,N,Q,U,E,R.", 1)//13
        //                                              .AddOption("Claim Leter14.", 2)//13
        //                    .AddOption("I`ll~think~about~it.", 255)//14
        //                    .AddAvatar(3).FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {

        //                if (client.Inventory.Contain(3303124, 1)
        //                      && client.Inventory.Contain(3303125, 1)
        //                      && client.Inventory.Contain(3303126, 1)
        //                     && client.Inventory.Contain(3303127, 1)
        //                     && client.Inventory.Contain(3303128, 1)
        //                     && client.Inventory.Contain(3303129, 1)
        //                     && client.Inventory.Contain(3303130, 1))
        //                {
        //                    client.Inventory.RemoveStackItem(3303124, 1, stream);
        //                    client.Inventory.RemoveStackItem(3303125, 1, stream);
        //                    client.Inventory.RemoveStackItem(3303126, 1, stream);
        //                    client.Inventory.RemoveStackItem(3303127, 1, stream);
        //                    client.Inventory.RemoveStackItem(3303128, 1, stream);
        //                    client.Inventory.RemoveStackItem(3303129, 1, stream);
        //                    client.Inventory.RemoveStackItem(3303130, 1, stream);
        //                    client.Player.BoundConquerPoints += 5000;
        //                    client.CreateBoxDialog("You Claim 5k BoundCPs");
        //                }

        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Inventory.Contain(3303131, 1))
        //                {
        //                    client.Inventory.RemoveStackItem(3303131, 1, stream);
        //                    client.Player.BoundConquerPoints += 10000;
        //                    client.CreateBoxDialog("You Claim 10k BoundCPs");
        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.MerchantClerk)]
        private static void MerchantClerk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("5003101_0101@@");
                        dialog.AddText("5003102_0101@@");
                        dialog.AddText("5003104_0101@@");
                        dialog.AddText("5003108_0101@@");
                        dialog.AddOption("5003109_0102@@", 1);
                        dialog.AddOption("Let me think it over.", 255);
                        dialog.AddAvatar(6).FinalizeDialog();
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.EgyptCoEvents)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        //[NpcAttribute((NpcID)6525)]
        public static void EgyptCoEvents(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            DateTime timer = DateTime.Now;
            DateTime Now64 = DateTime.Now;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hii.. Here events time and reward Enjoy Guys .");

                        data.AddOption("TimeOfEvent", 29);
                        data.AddOption("RewardEvent", 38);
                        data.AddOption("Just passing by..");
                        data.AddAvatar(277).FinalizeDialog();
                        break;
                    }
                #region Infermotaion Events
                case 29:
                    {

                        data.AddText("SuperGuildWar    [Tuesday] 21.00 && 22.00 PM\n");
                        data.AddText("GuildWar         [SunDay ] 21.00 && 22.00 PM\n");
                        data.AddText("ElitePk          [Friday] 21.45\n");
                        data.AddText("Teampk           [Tuesday]21.45\n");
                        data.AddText("SkillpK          [Wednesday]21.45\n");
                        data.AddText("CaptureTheFlag   [Saturday]21.00 && 20.00\n");
                        data.AddText("ClanWar          [Wednesday && Monday && Friday] 00.00 && xx.55\n");
                        data.AddText("WekklyPk         [Monday] 20.00 && xx.15\n");
                        data.AddText("ClassPk          [Saturday] 20.00 && xx.15\n");
                        data.AddOption("Oky,Thanks", 255);
                        data.AddAvatar(265).FinalizeDialog();
                        break;
                    }
                case 38:
                    {

                        data.AddText("SuperGuildWar     1M ConquerPoints && 7 day Holy Zokoo\n");
                        data.AddText("GuildWar          2BB Money \n");
                        data.AddText("ElitePk           Rank 1 == 300k cps && anima p 15 ^^ Rank 2 == 200k cps && anima p 14 ^^Rank 3 == 100k cps && anima p 13\n");
                        data.AddText("Teampk            Rank 1 == 300k cps && anima p 15 ^^ Rank 2 == 200k cps && anima p 14 ^^Rank 3 == 100k cps && anima p 13\n");
                        data.AddText("SkillpK           Rank 1 == 300k cps && anima p 15 ^^ Rank 2 == 200k cps && anima p 14 ^^Rank 3 == 100k cps && anima p 13\n");
                        data.AddText("CaptureTheFlag    Rank 1 == 500k cps && anima p 16 ^^ Rank 2 == 250k cps && anima p 15 ^^Rank 3 == 100k cps && anima p 14\n");
                        data.AddText("ClanWar           50k cps and token to change for anima p 16\n");
                        data.AddText("WekklyPk          300000 k cps and vip token 7 (7day)\n");
                        data.AddText("ClassPk           300000 k cps and vip token 6 (7day)\n");
                        data.AddOption("Oky,Thanks", 255);
                        data.AddAvatar(270).FinalizeDialog();
                        break;
                    }
                #endregion

            }
        }
        #region The NPC Edit By Mohy ElDin ...
        //[NpcAttribute(NpcID.updateanima)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void updateanima(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hello, From Here You Can Upgrade All Anima");
                        data.AddOption("Upgrade 2x [P1Anima].", 1);
                        data.AddOption("Upgrade 2x [P2Anima].", 2);
                        data.AddOption("Upgrade 2x [P3Anima].", 3);
                        data.AddOption("Upgrade 2x [P4Anima].", 4);
                        data.AddOption("Upgrade 2x [P5Anima].", 5);
                        data.AddOption("Upgrade 2x [P6Anima].", 6);
                        data.AddOption("Upgrade 2x [P7Anima].", 7);
                        data.AddOption("Upgrade 2x [P8Anima].", 8);
                        data.AddOption("Upgrade 2x [P9Anima].", 9);
                        data.AddOption("Upgrade 2x [P10Anima].", 10);
                        data.AddOption("Upgrade 2x [P11Anima].", 11);
                        data.AddOption("Upgrade 2x [P12Anima].", 12);
                        data.AddOption("Upgrade 2x [P13Anima].", 13);
                        data.AddOption("Upgrade 2x [P14Anima].", 14);
                        data.AddOption("Upgrade 2x [P15Anima].", 15);
                        data.AddOption("Upgrade 2x [P16Anima].", 16);
                        data.AddOption("Upgrade 2x [P17Anima].", 17);
                        data.AddOption("Just passing by.");
                        data.AddAvatar(263).FinalizeDialog();
                        break;
                    }

                case 1:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200001, 2))
                        {

                            client.Inventory.Remove(4200001, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200002, 1);
                                client.SendWhisper("Congratulations, You received x1 [P2Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P1Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.InTrade) 
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200002, 2))
                        {

                            client.Inventory.Remove(4200002, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200003, 1);
                                client.SendWhisper("Congratulations, You received x1 [P3Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P2Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200003, 2))
                        {

                            client.Inventory.Remove(4200003, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200004, 1);
                                client.SendWhisper("Congratulations, You received x1 [P4Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P3Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200004, 2))
                        {

                            client.Inventory.Remove(4200004, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200005, 1);
                                client.SendWhisper("Congratulations, You received x1 [P5Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P4Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200005, 2))
                        {

                            client.Inventory.Remove(4200005, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200006, 1);
                                client.SendWhisper("Congratulations, You received x1 [P6Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P5Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200006, 2))
                        {

                            client.Inventory.Remove(4200006, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200007, 1);
                                client.SendWhisper("Congratulations, You received x1 [P7Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P6Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200007, 2))
                        {

                            client.Inventory.Remove(4200007, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200008, 1);
                                client.SendWhisper("Congratulations, You received x1 [P8Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P7Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 8:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200008, 2))
                        {

                            client.Inventory.Remove(4200008, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200009, 1);
                                client.SendWhisper("Congratulations, You received x1 [P9Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P8Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 9:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200009, 2))
                        {

                            client.Inventory.Remove(4200009, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200010, 1);
                                client.SendWhisper("Congratulations, You received x1 [P10Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P9Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200010, 2))
                        {

                            client.Inventory.Remove(4200010, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200011, 1);
                                client.SendWhisper("Congratulations, You received x1 [P11Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P10Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 11:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200011, 2))
                        {

                            client.Inventory.Remove(4200011, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200012, 1);
                                client.SendWhisper("Congratulations, You received x1 [P12Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P11Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 12:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200012, 2))
                        {

                            client.Inventory.Remove(4200012, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200013, 1);
                                client.SendWhisper("Congratulations, You received x1 [P13Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P12Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 13:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200013, 2))
                        {

                            client.Inventory.Remove(4200013, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200014, 1);
                                client.SendWhisper("Congratulations, You received x1 [P14Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P13Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 14:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200014, 2))
                        {

                            client.Inventory.Remove(4200014, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200015, 1);
                                client.SendWhisper("Congratulations, You received x1 [P15Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P14Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 15:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200015, 2))
                        {

                            client.Inventory.Remove(4200015, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200016, 1);
                                client.SendWhisper("Congratulations, You received x1 [P16Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P15Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 16:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200016, 2))
                        {

                            client.Inventory.Remove(4200016, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200017, 1);
                                client.SendWhisper("Congratulations, You received x1 [P17Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P16Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
                case 17:
                    {
                        if (client.InTrade)
                        {
                            client.CreateBoxDialog("Sorry, You Must Close The Trade First .");
                            return;
                        }
                        if (client.IsVendor)
                        {
                            client.CreateBoxDialog("Sorry, You Must Cancel Your Shop First .");
                            return;
                        }
                        if (!client.Inventory.HaveSpace(2))
                        {
                            client.CreateBoxDialog("Please make 2 more space in your inventory.");
                            break;
                        }
                        if (client.Inventory.Contain(4200017, 2))
                        {

                            client.Inventory.Remove(4200017, 2, stream);
                            if (Role.Core.Rate(50))
                            {
                                client.Inventory.Add(stream, 4200018, 1);
                                client.SendWhisper("Congratulations, You received x1 [P18Anima].", "Anima System Upgrade", client.Player.Name);
                            }
                            else
                            {
                                client.CreateBoxDialog("Your Anima Upgrade Failed Good Luck Next Time.");
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[] { "zf2-e128" });
                        }
                        else
                        {
                            data.Text("Sorry, You Do Not Have 2x P17Anima To Upgrade .");

                            data.Option("Okay Thanks.", 255).AddAvatar(263).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        #endregion
        [NpcAttribute(NpcID.UpdateLevelStuff)]
        private static void UpdateLevelStuff(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.AddText("Hi. What i do is upgrade your equipment for meteors. How about it?")

 .AddOption("Upgrade level", 30)
                         .AddOption("Just passing by.")
                         .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        dialog.AddText("I see. Upgrading an item`s level makes it more powerful, but also harder to use, increasing the item level prerequisite.")
                        .AddOption("Helmet,~Earring", 31)
                        .AddOption("Upgrade~necklace~level.", 32)
                        .AddOption("Upgrade~armor~level.", 33)
                        .AddOption("Upgrade~Right~weapon~level.", 34)
                        .AddOption("Upgrade~Left~weapon~level.", 35)
                        .AddOption("Upgrade~ring,~heavy~ring~level.", 36)
                        .AddOption("Upgrade~boots~level.", 38)
                        .AddOption("Upgrade~shield~level.", 35)
                        .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 38:
                    {
                        MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)Option - 30, out item))
                        {
                            if (item.Durability < item.MaximDurability)
                            {
                                dialog.AddText("Sorry, this item has been damaged. Please get it repaired first.")
                                .AddOption("Oh,~I~see.~Farewell.")
                                .AddAvatar(6).FinalizeDialog();
                                break;
                            }

                            dialog.AddText("You have to give me " + 10000 + " ConquerPoints to upgrade this item's level.")
                       .AddOption("Sure.", (byte)(Option + 10))
                       .AddOption("Never~mind.")
                       .AddAvatar(6).FinalizeDialog();
                        }
                        else
                        {
                            dialog.AddText("Sorry, please wear the item you want to upgrade first.")
                            .AddOption("Oh,~I~see.~Farewell.")
                            .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 48:
                    {
                        MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)Option - 40, out item))
                        {
                            if (item.Durability < item.MaximDurability)
                            {
                                dialog.AddText("Sorry, this item has been damaged. Please get it repaired first.")
                                .AddOption("Oh,~I~see.~Farewell.")
                                .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var lvl = Pool.ItemsBase[item.ITEM_ID].Level;
                            if (lvl > 5)
                            {
                                var cost = 10000;
                                if (client.Player.ConquerPoints >= cost)
                                {

                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(item.ITEM_ID, out DBItem))
                                    {
                                        bool succesed = false;

                                        uint nextItemId = Pool.ItemsBase.UpdateItem(item.ITEM_ID, out succesed);

                                        if ((Database.ItemType.Equipable(nextItemId, client) == false) || nextItemId == item.ITEM_ID)
                                        {
                                            client.CreateBoxDialog("You can`t update this item.");
                                        }
                                        else
                                        {
                                            item.ITEM_ID = Pool.ItemsBase.UpdateItem(item.ITEM_ID, out succesed);
                                            item.Mode = Role.Flags.ItemMode.Update;
                                            item.Send(client, stream);

                                            client.Inventory.Remove(Database.ItemType.DragonBall, (byte)cost, stream);
                                            if (item.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);

                                            dialog.AddText("Your item has been upgraded. Look and behold my marvelous upgrading skill! Isn`t it amazing?")
                                            .AddOption("Thanks~a~lot!")
                                            .AddAvatar(6).FinalizeDialog();
                                        }
                                    }
                                }

                            }

                        }
                        else
                        {
                            dialog.AddText("Sorry, please wear the item you want to upgrade first.")
                            .AddOption("Oh,~I~see.~Farewell.")
                            .AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TeleportBosses)]
        public static void TeleportBosses(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello , " + client.Player.Name + " , I Can Teleport Bosses.");

                        dialog.Option("Go,Nemiss(DraonIsland),Time[xx15].", 1);
                        dialog.Option("Go,Banshe(DragonIsland),Time[xx27].", 2);
                        dialog.Option("Go,Nemiss(DeityIsland),Time[xx45].", 3);
                        dialog.Option("Go,Banshe(DeityIsland),Time[xx57].", 4);
                        dialog.Option("Go,Dragon(DeityIsland),Time[xx05].", 5);
                        dialog.Option("Go,Spook(DeityIsland),Time[xx30].", 6);
                        dialog.Option("Go,QueenEvil(DeityIsland),Time[20:10 && 13:10].", 7);

                        dialog.Option("thx.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(568, 377, 10137);

                        break;
                    }
                case 2:
                    {
                        client.Teleport(658, 710, 10137);

                        break;
                    }
                case 3:
                    {
                        client.Teleport(917, 1177, 10250);

                        break;
                    }
                case 4:
                    {
                        client.Teleport(484, 172, 10250);

                        break;
                    }
                case 5:
                    {
                        client.Teleport(163, 410, 10250);

                        break;
                    }
                case 6:
                    {
                        client.Teleport(1004, 700, 10250);

                        break;
                    }
                case 7:
                    {
                        client.Teleport(625, 617, 10250);

                        break;
                    }


            }
        }

        //[NpcAttribute(NpcID.Teleport)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void Teleport(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Hello , " + client.Player.Name + " , I Can Teleport U EveryWhere.");

                        dialog.Option("Go,Tops.", 1);
                        dialog.Option("Go,Shops.", 2);
                        dialog.Option("Go,Rebirth.", 3);
                        dialog.Option("Go,OnlinePoints.", 4);
                        dialog.Option("Go,Nobility.", 5);
                        dialog.Option("Go,EGW.", 6);
                        dialog.Option("Go,Elite,Skill,TeamPk.", 7);
                        dialog.Option("Go,Arena.", 8);
                        dialog.Option("Go,Stuff,Chi,Jiang.", 9);
                        dialog.Option("Go,OpenJiang.", 10);

                        dialog.Option("thx.", 255).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 2:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 3:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 4:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 5:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 6:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 7:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 8:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 9:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }
                case 10:
                    {
                        client.Teleport(410, 354, 1002);

                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.ClanWarTwin)]
        //public static void ClanWarTwin(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    dialog.AddAvatar(36);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.AddText("Hi " + client.Player.Name + ", I'm the ClanWarTwin manager .. How can i help? \n");
        //                dialog.AddText("Prize Clan War Leader 20.000.000 CPs - 1x P15Anima \n");
        //                dialog.AddText("Prize Clan War Member 10.000.000 CPs - 1x P14Anima \n");
        //                if (MsgSchedules.ClanTwin.Proces == ProcesType.Alive)
        //                    dialog.AddOption("Enter!", 1);
        //                else
        //                dialog.AddOption("Okay.~I~see..", 255);
        //                dialog.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.MyClan == null)
        //                {
        //                    dialog.AddText("Hi " + client.Player.Name + ", Join to Any Clan");
        //                    dialog.AddOption("Okay.~I~see..", 255);
        //                    dialog.FinalizeDialog();
        //                    break;
        //                }
        //                if (MsgSchedules.ClanTwin.Proces == ProcesType.Alive)
        //                {
        //                    client.Teleport(165, 198, 12020);
        //                }
        //                else
        //                    dialog.AddText("The ClanWarTwin is not started yet.").AddOption("Alright.").FinalizeDialog();
        //                break;
        //            }


        //    }
        //}
        //[NpcAttribute(NpcID.ClanWarPhoienx)]
        //public static void ClanWarPhoienx(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    dialog.AddAvatar(36);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.AddText("Hello, Clan War starts on Thursday at 21:00 and ends at 22:00\n");
        //                dialog.AddText("Prize Clan War Leader 4.000.000.000 CPs - 5.000 Online Points \n");
        //                if (MsgSchedules.ClanPhoenix.Proces == ProcesType.Alive)
        //                    dialog.AddOption("Enter!", 1);
        //                else
        //                    dialog.AddOption("Okay.~I~see..", 255);
        //                dialog.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                client.Player.ProtectAttack(5 * 1000); // 5 Seconds


        //                if (client.Player.MyClan == null)
        //                {
        //                    dialog.AddText("Hi " + client.Player.Name + ", Join to Any Clan");
        //                    dialog.AddOption("Okay.~I~see..", 255);
        //                    dialog.FinalizeDialog();
        //                    break;
        //                }
        //                if (MsgSchedules.ClanPhoenix.Proces == ProcesType.Alive)
        //                {
        //                    client.Teleport(375, 292, 11996);
        //                }
        //                else
        //                    dialog.AddText("The Clan War is not started yet.").AddOption("Alright.").FinalizeDialog();
        //                break;
        //            }

        //    }
        //}

        //[NpcAttribute(NpcID.ClanWarApe)]
        //public static void ClanWarApe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    dialog.AddAvatar(36);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.AddText("Hi " + client.Player.Name + ", I'm the ClanWarApe manager .. How can i help? \n");
        //                dialog.AddText("Prize Clan War Leader 20.000.000 CPs - 1x P15Anima \n");
        //                dialog.AddText("Prize Clan War Member 10.000.000 CPs - 1x P14Anima \n");
        //                if (MsgSchedules.ClanApe.Proces == ProcesType.Alive)
        //                    dialog.AddOption("Enter!", 1);
        //                else
        //                    dialog.AddOption("Claim~reward.", 2);
        //                dialog.AddOption("Okay.~I~see..", 255);
        //                dialog.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.MyClan == null)
        //                {
        //                    dialog.AddText("Hi " + client.Player.Name + ", Join to Any Clan");
        //                    dialog.AddOption("Okay.~I~see..", 255);
        //                    dialog.FinalizeDialog();
        //                    break;
        //                }
        //                if (MsgSchedules.ClanApe.Proces == ProcesType.Alive)
        //                {

        //                    client.Teleport(569, 575, 12066);
        //                }
        //                else
        //                    dialog.AddText("The ClanWarTwin is not started yet.").AddOption("Alright.").FinalizeDialog();
        //                break;
        //            }


        //    }
        //}

        //[NpcAttribute(NpcID.ClanWarDesert)]
        //public static void ClanWarDesert(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    dialog.AddAvatar(36);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.AddText("Hi " + client.Player.Name + ", I'm the ClanWarDesert manager .. How can i help? \n");
        //                dialog.AddText("Prize Clan War Leader 20.000.000 CPs - 1x P15Anima \n");
        //                dialog.AddText("Prize Clan War Member 10.000.000 CPs - 1x P14Anima \n");
        //                if (MsgSchedules.ClanDesert.Proces == ProcesType.Alive)
        //                    dialog.AddOption("Enter!", 1);
        //                else
        //                    dialog.AddOption("Claim~reward.", 2);
        //                dialog.AddOption("Okay.~I~see..", 255);
        //                dialog.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.MyClan == null)
        //                {
        //                    dialog.AddText("Hi " + client.Player.Name + ", Join to Any Clan");
        //                    dialog.AddOption("Okay.~I~see..", 255);
        //                    dialog.FinalizeDialog();
        //                    break;
        //                }
        //                if (MsgSchedules.ClanDesert.Proces == ProcesType.Alive)
        //                {
        //                    client.Teleport(502, 655, 12067);
        //                }
        //                else
        //                    dialog.AddText("The ClanWarTwin is not started yet.").AddOption("Alright.").FinalizeDialog();
        //                break;
        //            }


        //    }
        //}


        //[NpcAttribute(NpcID.ClanWarBird)]
        //public static void ClanWarBird(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog dialog = new Dialog(client, stream);
        //    dialog.AddAvatar(36);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                dialog.AddText("Hi " + client.Player.Name + ", I'm the ClanWarBird manager .. How can i help? \n");
        //                dialog.AddText("Prize Clan War Leader 20.000.000 CPs - 1x P15Anima \n");
        //                dialog.AddText("Prize Clan War Member 10.000.000 CPs - 1x P14Anima \n");
        //                if (MsgSchedules.ClanBird.Proces == ProcesType.Alive)
        //                    dialog.AddOption("Enter!", 1);
        //                else
        //                    dialog.AddOption("Claim~reward.", 2);
        //                dialog.AddOption("Okay.~I~see..", 255);
        //                dialog.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                if (client.Player.MyClan == null)
        //                {
        //                    dialog.AddText("Hi " + client.Player.Name + ", Join to Any Clan");
        //                    dialog.AddOption("Okay.~I~see..", 255);
        //                    dialog.FinalizeDialog();
        //                    break;
        //                }
        //                if (MsgSchedules.ClanBird.Proces == ProcesType.Alive)
        //                {
        //                    client.Teleport(736, 548, 12025);
        //                }
        //                else
        //                    dialog.AddText("The ClanWarTwin is not started yet.").AddOption("Alright.").FinalizeDialog();
        //                break;
        //            }

        //    }
        //}

        #region NewDemonBox
        [NpcAttribute(NpcID.DemonNewBox)]
        public static void DemonNewBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hello,Sir You Can Join To This Map Until Use Demon Box");
                        data.Option("Join~Map", 1);
                        data.Option("isee.", 255);
                        data.Avatar(86).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(384, 284, 11568, client.Player.UID);
                        Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                        np.UID = (uint)23081;
                        np.NpcType = Role.Flags.NpcType.Shop;
                        np.Mesh = 2550;
                        np.Map = client.Player.Map;
                        np.DynamicID = client.Player.UID;
                        np.X = 376;
                        np.Y = 273;
                        client.Map.AddNpc(np);
                        np.Send(stream);
                        client.Player.View.Role(false);

                        Game.MsgNpc.Npc np1 = Game.MsgNpc.Npc.Create();
                        np1.UID = (uint)20000;
                        np1.NpcType = Role.Flags.NpcType.Talker;
                        np1.Mesh = 43780;
                        np1.Map = client.Player.Map;
                        np1.DynamicID = client.Player.UID;
                        np1.X = 376;
                        np1.Y = 294;
                        client.Map.AddNpc(np1);
                        np1.Send(stream);
                        client.Player.View.Role(false);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DemonNewBoxOut)]
        public static void DemonNewBoxOut(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hello,Sir You Can Join To This Map Until Use Demon Box");
                        data.Option("Out~Map", 1);
                        data.Option("isee.", 255);
                        data.Avatar(86).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(410, 354, 1002);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TaiChiDemonBox)]
        public static void TaiChiDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(3303365, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the TaiChiDemonBox, you will have a chance to get a PilgrimCPPack Pack (1380000 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {


                            client.Inventory.Remove(3303365, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 4355, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the TaiChiDemonBox Box and a TaiChiDemonBox was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;

                    }
            }
        }
        [NpcAttribute(NpcID.AuroraDemonBox)]
        public static void AuroraDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720674, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you open the Aurora Demon Box, a demon will be released! After you kill it, you`ll have a chance to get a Pilgrim CP Pack (690000 CPs)! I");
                        data.AddText("~suggest you open it in a secret place to keep from being robbed. Are you sure you want to open it and release the demon now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            client.Inventory.Remove(720674, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2438, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Aurora Demon Box and a demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SacredDemonBox)]
        public static void SacredDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720673, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you open the Sacred Demon Box, a demon will be released! After you kill it, you`ll have a chance to get a Kylin CP Pack (276000 CPs)! I");
                        data.AddText("~suggest you open it somewhere safe from prying eyes. Are you sure you want to open it and release the demon now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {

                            client.Inventory.Remove(720673, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2437, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Sacred Demon Box and a Sacred Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ChaosDemonBox)]
        public static void ChaosDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720672, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Chaos Demon, you will have a chance to get a Nimbus CP Pack (138000 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {


                            client.Inventory.Remove(720672, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2436, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Chaos Demon Box and a Chaos Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;

                    }
            }
        }
        [NpcAttribute(NpcID.HeavenDemonBox)]
        public static void HeavenDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720671, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Heaven Demon, you will have a chance to get a Frost CP Pack (69000 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {


                            client.Inventory.Remove(720671, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2435, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Heaven Demon Box and a Heaven Demon was released! Hurry and kill it to get the rewards!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.FloodDemonBox)]
        public static void FloodDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720652, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Flood Demon, you will have a chance to get a Fantasy CP Pack (13800 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {

                            client.Inventory.Remove(720652, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2422, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Flood Demon Box and a Flood Demon was released! Hurry and kill it to get your reward!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AncientDemonBox)]
        public static void AncientDemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720651, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Ancient Demon, you will have a chance to get a Mystic CP Pack (6900~CPs). Are you sure you want to release it now?");
                        data.AddText("~now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {

                            client.Inventory.Remove(720651, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2421, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Ancient Demon Box and an Ancient Demon was released! Hurry and kill it to get your reward!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.DemonBox)]
        public static void DemonBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Inventory.Contain(720650, 1))
                return;
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You should open your Demon Box in a secret place far away from the guards and other players! Do not open it at the border of the map!");
                        data.AddText("~By killing the Demon, you will have a chance to get a Joy CP Pack (1380 CPs). Are you sure you want to release it now?");
                        data.AddOption("Yes!", 1);
                        data.AddOption("Wait...");
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {

                            client.Inventory.Remove(720650, 1, stream);
                            Server.AddMapMonster(stream, client.Map, 2420, client.Player.X, client.Player.Y, 0, 0, 1, client.Player.DynamicID, true);
                            client.SendSysMesage("You opened the Demon Box and a demon was released! Hurry and kill it to get the reward!", MsgMessage.ChatMode.System);
                        }
                        else
                        {
                            client.SendSysMesage("The Demon Box is available use in the DemonBoxCity.", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
            }
        }





        #endregion
        //[NpcAttribute(NpcID.UnLockAnimaLocked)]
        //public static void UnLockAnimaLocked(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{

        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.Text("Hello Sir I Can Help You To UnLock Timer Anima But You Should Have Yahoo or gamil ");
        //                if (client.Player.AnimaLockedTimer)
        //                    data.Option("UnLock All Anima In Inventory", 1);
        //                else
        //                    data.Option("Active Yahoo Or Gamil", 2);
        //                data.Option("isee.", 255);
        //                   data.Avatar(86).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                data.AddText("Plaese Choose [Gmail.com] Or [Yahoo.com]").AddOption("Yahoo", 3).AddOption("Gmail", 4).AddOption("Just passing by.").FinalizeDialog();
        //                break;
        //            }
        //        case 3:
        //            {
        //                data.AddText("Please input your email .. @yahoo.Com will input automatic.")
        //                    .AddInput("Yahoo: ", 5)
        //                    .AddOption("Just passing by.");
        //                       data.Avatar(86).FinalizeDialog();
        //                break;
        //            }
        //        case 4:
        //            {
        //                data.AddText("Please input your email .. @gmail.Com will input automatic.")
        //                    .AddInput("gmail: ", 6)
        //                    .AddOption("Just passing by.");
        //                data.Avatar(86).FinalizeDialog();
        //                break;
        //            }
        //        case 5:
        //            {
        //                if (Input != "")
        //                {
        //                    string mail = Input.Contains("@") ? "" : "@yahoo.com";
        //                    client.ChangePassword.Email = Input + mail;
        //                    bool exist = false;
        //                    using (var cmd = new DBFunctionality.MySqlCommand(DBFunctionality.MySqlCommandType.SELECT).Select("accounts").Where("ID", client.Player.UID).And("Email", client.ChangePassword.Email))
        //                    using (var reader = new DBFunctionality.MySqlReader(cmd))
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            exist = true;
        //                        }
        //                    }
        //                    if (exist)
        //                    {
        //                        client.Player.AnimaLockedTimer = true;
        //                        client.CreateBoxDialog("You Can UnLock Anima Now but if you logoff you will Active Agian");
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Please input your data again.");
        //                        data.AddOption("Alrihgt.");
        //                        data.Avatar(86).FinalizeDialog();
        //                    }
        //                }
        //                break;
        //            }
        //        case 6:
        //            {
        //                if (Input != "")
        //                {
        //                    string mail = Input.Contains("@") ? "" : "@gmail.com";
        //                    client.ChangePassword.Email = Input + mail;
        //                    bool exist = false;
        //                    using (var cmd = new DBFunctionality.MySqlCommand(DBFunctionality.MySqlCommandType.SELECT).Select("accounts").Where("ID", client.Player.UID).And("Email", client.ChangePassword.Email))
        //                    using (var reader = new DBFunctionality.MySqlReader(cmd))
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            exist = true;
        //                        }
        //                    }
        //                    if (exist)
        //                    {
        //                        client.Player.AnimaLockedTimer = true;
        //                        client.CreateBoxDialog("You Can UnLock Anima Now but if you logoff you will Active Agian");
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Please input your data again.");
        //                        data.AddOption("Alrihgt.");
        //                        data.Avatar(86).FinalizeDialog();
        //                    }
        //                }
        //                break;
        //            }
        //        case 1:
        //            {
        //                data.Text("Hello Sir I Can Help You To UnLock Timer Anima But You Should Have Yahoo or gamil ");
        //                if (client.Inventory.ContainAnimaLocked(4200018))
        //                    data.Option("" + Pool.ItemsBase[4200018].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200018) + "]", 2);

        //                if (client.Inventory.ContainAnimaLocked(4200017))
        //                    data.Option("" + Pool.ItemsBase[4200017].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200017) + "]", 3);

        //                if (client.Inventory.ContainAnimaLocked(4200016))
        //                    data.Option("" + Pool.ItemsBase[4200016].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200016) + "]", 4);

        //                if (client.Inventory.ContainAnimaLocked(4200015))
        //                    data.Option("" + Pool.ItemsBase[4200015].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200015) + "]", 5);

        //                if (client.Inventory.ContainAnimaLocked(4200014))
        //                    data.Option("" + Pool.ItemsBase[4200014].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200014) + "]", 6);

        //                if (client.Inventory.ContainAnimaLocked(4200013))
        //                    data.Option("" + Pool.ItemsBase[4200013].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200013) + "]", 7);

        //                if (client.Inventory.ContainAnimaLocked(4200012))
        //                    data.Option("" + Pool.ItemsBase[4200012].Name.ToString() + " StackSize [" + client.Inventory.GetCountItemAnima(4200012) + "]", 8);

        //                data.Option("isee.", 255);
        //                   data.Avatar(86).FinalizeDialog();
        //                break;
        //            }
        //        default:
        //            {
        //                switch (Option)
        //                {
        //                    case 2:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP18 StackSize [" + client.Inventory.GetCountItemAnima(4200018) + "]");
        //                            var AnimaP18 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200018 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP18)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                    case 3:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP17 StackSize [" + client.Inventory.GetCountItemAnima(4200017) + "]");

        //                            var AnimaP17 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200017 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP17)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                    case 4:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP16 StackSize [" + client.Inventory.GetCountItemAnima(4200016) + "]");

        //                            var AnimaP16 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200016 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP16)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                    case 5:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP15 StackSize [" + client.Inventory.GetCountItemAnima(4200015) + "]");

        //                            var AnimaP15 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200015 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP15)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                    case 6:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP14 StackSize [" + client.Inventory.GetCountItemAnima(4200014) + "]");

        //                            var AnimaP14 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200014 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP14)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                    case 7:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP13 StackSize [" + client.Inventory.GetCountItemAnima(4200013) + "]");

        //                            var AnimaP13 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200013 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP13)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                    case 8:
        //                        {
        //                            client.CreateBoxDialog("You UnLock Timer AnimaP12 StackSize [" + client.Inventory.GetCountItemAnima(4200012) + "]");

        //                            var AnimaP12 = client.Inventory.ClientItems.Values.Where(p => p.ITEM_ID == 4200012 && p.Locked == 2).ToArray();
        //                            foreach (var UnItem in AnimaP12)
        //                            {

        //                                UnItem.Locked = 2;
        //                                UnItem.UnLockTimer = 0;
        //                                UnItem.Mode = Role.Flags.ItemMode.Update;
        //                                UnItem.Send(client, stream);
        //                            }
        //                            break;
        //                        }
        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.BotJail)]
        public static void BotJail(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.Text("Hello Sir I Can Help You To UnLock Out You From This  Jail");
                        if (Database.BotJail.CanOutBot(client))
                            data.Option("You~Can~Out~Bot~Jail For Cps", 1);
                        else
                            data.Option("You Have Bot Jail Permanent", 2);
                        data.Option("isee.", 255);
                        data.Avatar(86).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (Database.BotJail.CanOutBot(client))
                        {
                            Database.BotJail.CheckUp(client);

                        }
                        else
                        {
                            data.Option("You Cant out  Bot Jail Because You Dont Have 2M Cps", 2);
                            data.Option("isee.", 255);
                            data.Avatar(86).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.RecoverSkill)]
        //public static void RecoverSkill(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog obj = new Dialog(client, stream);

        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                obj.AddText("Class Skill Teleporter");
        //                obj.AddText("Every Class has distinctive skills. If you forget a skill after your 1st rebirth and want to regain it, I can send you to see your Class tutor.");
        //                obj.Option("Recover~Skill.", 140);
        //                obj.AddAvatar(47);
        //                obj.FinalizeDialog();
        //                break;
        //            }
        //        case 140:
        //            {
        //                obj.AddText("Class Skill Teleporter");
        //                obj.AddText("Every Class has distinctive skills. If you forget a skill after your 1st rebirth and want to regain it, I can send you to see your Class tutor.");
        //                var Spell = client.MySpells.RecoverSkill.Values.ToArray();
        //                for (int x = 0; x < Spell.Length; x++)
        //                {
        //                    obj.Option("Recover [" + Pool.Magic[Spell[x].ID][Spell[x].Level].Name + "]", (byte)((int)x + 1));

        //                }
        //                obj.Option("isee.", 255);
        //                obj.AddAvatar(47);
        //                obj.FinalizeDialog();
        //                break;
        //            }

        //        default:
        //            {
        //                var Spell = client.MySpells.RecoverSkill.Values.ToArray();
        //                for (int x = 0; x < Spell.Length; x++)
        //                {
        //                    if (x == 0 && Option == 1)
        //                    {
        //                        client.MySpells.Add(stream, Spell[0].ID, Spell[0].Level);
        //                        client.MySpells.RecoverSkill.Remove((ushort)Spell[0].ID);
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        client.MySpells.Add(stream, Spell[x].ID, Spell[x].Level);
        //                        client.MySpells.RecoverSkill.Remove((ushort)Spell[x].ID);
        //                        break;
        //                    }


        //                }
        //                break;
        //            }

        //    }
        //}

        //[NpcAttribute(NpcID.CrystalMerchant)]
        //public static void CrystalMerchant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("18 ferocious devils are locked in the FiendÃ˜Å’Ã‚Â¯s Lair. Kill them and you will have chance to gain the stolen treasures. \n")
        //                    .AddText("~Besides, some devils from Floor 11 to Floor 18 carry the crystals which concentrate their feeling of resentment. \n")
        //                    .AddText("~Once you get one, you can swap the crystal for incredibly good reward here. \n")
        //                    .AddText("~You can also buy FiendÃ˜Å’Ã‚Â¯s Lair Permits from Shopping Mall. With the help of these permits, you can reach the corresponding floors directly (Floor 11 to Floor 18).")
        //                    .AddOption("Fiend~Lair~Permit?", 1)
        //                    .AddOption("I`ve~got~a~crystal.", 49)
        //                    .AddOption("I~see.", 255).AddAvatar(237).FinalizeDialog();
        //                break;
        //            }
        //        case 49:
        //            {
        //                data.AddText("You can directly use the crystals to obtain huge EXP or swap them for incredibly good reward here.")
        //                    .AddText("~So, which kind of crystal did you get?")
        //                    .AddOption("Frost~Crystal.", 50)//720282
        //                    .AddOption("Fire~Crystal.", 51)//721003
        //                    .AddOption("Soul~Crystal.", 52)//721004
        //                    .AddOption("Dread~Crystal.", 53)//721005
        //                    .AddOption("Next~page", 54)
        //                    .AddOption("Not~yet.", 255).AddAvatar(237).FinalizeDialog();
        //                break;
        //            }
        //        case 53:
        //            {
        //                if (!client.Inventory.Contain(721005, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721005, 1, stream);
        //                client.Player.ConquerPoints += 27600;
        //                client.SendSysMesage("Dread~Crystal Exchanged.");
        //                break;
        //            }
        //        case 52:
        //            {
        //                if (!client.Inventory.Contain(721004, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721004, 1, stream);
        //                client.Player.ConquerPoints += 13800;
        //                client.SendSysMesage("Soul~Crystal Exchanged.");
        //                break;
        //            }
        //        case 51:
        //            {
        //                if (!client.Inventory.Contain(721003, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721003, 1, stream);
        //                client.Player.ConquerPoints += 6900;
        //                client.SendSysMesage("Fire~Crystal Exchanged.");
        //                break;
        //            }
        //        case 50:
        //            {
        //                if (!client.Inventory.Contain(720282, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(720282, 1, stream);
        //                client.Player.ConquerPoints += 1380;
        //                client.SendSysMesage("Frost~Crystal Exchanged.");
        //                break;
        //            }
        //        case 54:
        //            {
        //                data.AddText("You can directly use the crystals to obtain huge EXP or swap them for incredibly good reward here.")
        //                    .AddText("~So, which kind of crystal did you get?")
        //                    .AddOption("Bloody~Crystal.", 55)//721006
        //                    .AddOption("Dark~Crystal.", 56)//721007
        //                    .AddOption("Chaos~Crystal.", 57)//721008
        //                    .AddOption("Moon~Crystal.", 58)//721009
        //                    .AddOption("Previous.", 49)
        //                    .AddOption("Not~yet.", 255).AddAvatar(237).FinalizeDialog();
        //                break;
        //            }
        //        case 58:
        //            {
        //                if (!client.Inventory.Contain(721009, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721009, 1, stream);
        //                client.Player.ConquerPoints += 690000;
        //                client.SendSysMesage("Moon~Crystal Exchanged.");
        //                break;
        //            }
        //        case 57:
        //            {
        //                if (!client.Inventory.Contain(721008, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721008, 1, stream);
        //                client.Player.ConquerPoints += 276000;
        //                client.SendSysMesage("Chaos~Crystal Exchanged.");
        //                break;
        //            }
        //        case 56:
        //            {
        //                if (!client.Inventory.Contain(721007, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721007, 1, stream);
        //                client.Player.ConquerPoints += 138000;
        //                client.SendSysMesage("Dark~Crystal Exchanged.");
        //                break;
        //            }
        //        case 55:
        //            {
        //                if (!client.Inventory.Contain(721006, 1))
        //                {
        //                    data.AddText("What? Where is your crystal? Don`t bother me unless you get one.")
        //                        .AddText("~Why not go down to the Fiend`s Lair and show me your bravery!")
        //                        .AddOption("I~won`t~let~you~down.", 255)
        //                        .AddOption("I~wanna~sell~one.", 49)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721006, 1, stream);
        //                client.Player.ConquerPoints += 69000;
        //                client.SendSysMesage("Bloody~Crystal Exchanged.");
        //                break;
        //            }
        //        case 1:
        //            {
        //                data.AddText("The 8 Fiend Lair Permits (Floor 11 to 18) are made by rare and precious materials. If you use")
        //                    .AddText("~this FiendÃ˜Å’Ã‚Â¯s Lair Permit, you will be sent to that level of the Lair directly, by using them. Do ")
        //                    .AddText("~you want to buy one from the Shopping Mall? Or sell me your permit?")
        //                    .AddOption("I~won`t~let~you~down.", 255)
        //                    .AddOption("I~wanna~sell~one.", 2)
        //                    .AddAvatar(237)
        //                    .FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                data.AddText("I know what you`re thinking. Don`t worry, my friend. I will buy at the original price.")
        //                    .AddText("~So which Fiend`s Lair Permit do you want to sell?")
        //                    .AddOption("Floor~11:~9~CPs.", 3)
        //                    .AddOption("Floor~12:~49~CPs.", 4)
        //                    .AddOption("Floor~13:~99~CPs.", 5)
        //                    .AddOption("Floor~14:~199~CPs.", 6)
        //                    .AddOption("Next.", 7)
        //                    .AddOption("Let~me~think~about~it.", 255)
        //                    .AddAvatar(237)
        //                    .FinalizeDialog();
        //                break;
        //            }
        //        case 6:
        //            {
        //                if (!client.Inventory.Contain(721094, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721094, 1, stream);
        //                client.Player.ConquerPoints += 199;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 5:
        //            {
        //                if (!client.Inventory.Contain(721093, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721093, 1, stream);
        //                client.Player.ConquerPoints += 99;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 4:
        //            {
        //                if (!client.Inventory.Contain(721092, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721092, 1, stream);
        //                client.Player.ConquerPoints += 49;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 3:
        //            {
        //                if (!client.Inventory.Contain(721091, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721091, 1, stream);
        //                client.Player.ConquerPoints += 9;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 7:
        //            {
        //                data.AddText("I know what you`re thinking. Don`t worry, my friend. I will buy at the original price.")
        //                    .AddText("~So which Fiend`s Lair Permit do you want to sell?")
        //                    .AddOption("Floor~15:~499~CPs", 8)
        //                    .AddOption("Floor~16:~999~CPs.", 9)
        //                    .AddOption("Floor~17:~1,999~CPs.", 10)
        //                    .AddOption("Floor~18:~4,999~CPs.", 11)
        //                    .AddOption("Previous.", 2)
        //                    .AddOption("Let~me~think~about~it.", 255)
        //                    .AddAvatar(237)
        //                    .FinalizeDialog();
        //                break;
        //            }
        //        case 11:
        //            {
        //                if (!client.Inventory.Contain(721098, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721098, 1, stream);
        //                client.Player.ConquerPoints += 4999;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 10:
        //            {
        //                if (!client.Inventory.Contain(721097, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721097, 1, stream);
        //                client.Player.ConquerPoints += 1999;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 9:
        //            {
        //                if (!client.Inventory.Contain(721096, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721096, 1, stream);
        //                client.Player.ConquerPoints += 999;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //        case 8:
        //            {
        //                if (!client.Inventory.Contain(721095, 1))
        //                {
        //                    data.AddText("You can`t be serious! So where is your Fiend`s Lair Permit?")
        //                        .AddOption("I~am~just~looking.", 255)
        //                        .AddAvatar(237)
        //                    .FinalizeDialog();
        //                    break;
        //                }
        //                client.Inventory.Remove(721095, 1, stream);
        //                client.Player.ConquerPoints += 499;
        //                client.SendSysMesage("FiendLairPermit Exchanged.");
        //                break;
        //            }
        //    }
        //}
        public static int LongineCount(uint Card)
        {
            switch (Card)
            {
                case 3321107://Trojan
                    return 9;
                case 3321098:
                    return 10;
                case 3316501:
                    return 10;

                case 3333400://Ninja
                    return 5;

                case 3333401://Warrior
                    return 16;
                case 3333402:
                    return 48;
                case 3333403:
                    return 144;

                case 3335125://Archer
                    return 5;
                case 3335126:
                    return 16;
                case 3335127:
                    return 48;
                case 3335128:
                    return 144;

                case 3328251://Taiost
                    return 5;
                case 3328252:
                    return 16;
                case 3328253:
                    return 48;
                case 3328254:
                    return 144;

                case 3329511://Pirate
                    return 5;
                case 3329512:
                    return 16;
                case 3329513:
                    return 48;
                case 3329514:
                    return 144;


                case 3339453://DragonWarrior
                    return 5;
                case 3339454:
                    return 16;
                case 3339455:
                    return 48;
                case 3339456:
                    return 144;


            }
            return 0;
        }
    }
}

