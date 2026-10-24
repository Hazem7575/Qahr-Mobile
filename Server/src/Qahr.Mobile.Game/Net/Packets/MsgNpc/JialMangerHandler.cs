using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {
        #region JialManger - Bot Jail Release NPC
        [NpcAttribute(NpcID.JialManger)]
        public static void JialManger(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("═══════════════════════════════════\n");
                        data.AddText("<tip color=0xffff4444 desc=>[  ⚠ JAIL WARDEN ⚠  ]</tip>\n");
                        data.AddText("═══════════════════════════════════\n\n");
                        data.AddText("Well, well, well... Look who we have here.\n");
                        data.AddText("<tip color=0xff00fa00 desc=>" + client.Player.Name + "</tip>, ");
                        data.AddText("you were caught using illegal programs!\n\n");
                        data.AddText("You think cheating has no consequences?\n");
                        data.AddText("The only way out of this cell is to pay the price.\n\n");
                        data.AddText("<tip color=0xffffd700 desc=>[ BAIL FEE: 2,000,000,000 CPs ]</tip>\n\n");
                        data.AddText("Your current CPs: <tip color=0xff00fa00 desc=>[" + client.Player.ConquerPoints.ToString("N0") + "]</tip>\n");
                        data.AddOption("<tip color=0xff00fa00 desc=>💰 Pay 2,000,000,000 CPs & Get Released</tip>", 1);
                        data.AddOption("<tip color=0xffff4444 desc=>I don't have enough... (Stay in Jail)</tip>", 2);
                        data.AddAvatar(45).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 2000000000)
                        {
                            client.Player.ConquerPoints -= 2000000000;
                            client.Player.SendUpdate(stream, client.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            client.Player.CanOut = true;

                            data.AddText("═══════════════════════════════════\n");
                            data.AddText("<tip color=0xff00fa00 desc=>[  ✅ RELEASED  ]</tip>\n");
                            data.AddText("═══════════════════════════════════\n\n");
                            data.AddText("Your bail of <tip color=0xffffd700 desc=>[2,000,000,000 CPs]</tip> has been paid.\n\n");
                            data.AddText("You are now free to go, <tip color=0xff00fa00 desc=>" + client.Player.Name + "</tip>.\n");
                            data.AddText("Let this be a lesson. Next time, play fair!\n");
                            data.AddOption("Thank you, I've learned my lesson.", 3);
                            data.AddAvatar(45).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("═══════════════════════════════════\n");
                            data.AddText("<tip color=0xffff4444 desc=>[  ❌ INSUFFICIENT FUNDS  ]</tip>\n");
                            data.AddText("═══════════════════════════════════\n\n");
                            data.AddText("Ha! You don't even have enough CPs to pay your bail!\n\n");
                            data.AddText("Required: <tip color=0xffffd700 desc=>[2,000,000,000 CPs]</tip>\n");
                            data.AddText("You have: <tip color=0xffff4444 desc=>[" + client.Player.ConquerPoints.ToString("N0") + " CPs]</tip>\n\n");
                            data.AddText("You're staying right here until you can afford it.\n");
                            data.AddText("Maybe next time you'll think twice before cheating!\n");
                            data.AddOption("I understand... (Stay in Jail)", 255);
                            data.AddAvatar(45).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("═══════════════════════════════════\n");
                        data.AddText("<tip color=0xffff4444 desc=>[  🔒 ACCESS DENIED  ]</tip>\n");
                        data.AddText("═══════════════════════════════════\n\n");
                        data.AddText("Then rot in here, <tip color=0xffff4444 desc=>" + client.Player.Name + "</tip>!\n\n");
                        data.AddText("No CPs, no freedom. That's the rule.\n");
                        data.AddText("Come back when you have <tip color=0xffffd700 desc=>[2,000,000,000 CPs]</tip>.\n");
                        data.AddOption("...", 255);
                        data.AddAvatar(45).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        // Teleport player out of jail to TwinCity (Map 10364)
                        client.Teleport(Places.TwinCity);
                        client.SendSysMesage("You have been released from jail. Play fair from now on!", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        break;
                    }
            }
        }
        #endregion
    }
}
