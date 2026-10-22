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
        #region NewPromate

        [NpcAttribute(NpcID.Dragon)]
        public static void Dragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24266][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24266][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24266][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24266][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24266][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24266][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24266][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24266][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24266][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24266][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24266][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24266][Option111]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level < 15)
                        {
                            data.AddText("You`re not strong enough for a job promotion now. Keep practicing and come back when you reach Level 15.")
                            .AddOption("Okay.", 255)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 975,
                            Fascing = 7,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PromotionThunderstriker)]
        public static void PromotionThunderstriker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog dialog = new Dialog(client, stream);

            if (!Database.AtributesStatus.IsThunderStriker(client.Player.Class))
            {
                dialog.Text("	I`ve been training Thunderstrikers since Lord Thor received a prophecy about the doom");
                dialog.Text("~of Osgard years ago. Though Thunderstrikers eventually defeated Hader who is the Lord of");
                dialog.Text("~Netherworld invading Osgard, Hader sacrificed his own life in the fulfillment of the prophecy.\n");
                dialog.Text("	Great King Austin consumed all his power to break Hader`s will. The terrific blast waves created");
                dialog.Text("~a huge rift in the sky, throwing Osgardians and Netherians into the world of Conquer. As the vow");
                dialog.Text("~tells, Thunderstrikers will fight and never flee, offering the blood of Netherians to appease the braves");
                dialog.Text("~sacrificed in the battle of doom.");

                dialog.Option("I~also~want~to~help~you!", 255).FinalizeDialog();
                return;
            }
            switch (Option)
            {
                case 0:
                    {
                        dialog.Text("Thunder General:                                                                                                                  Thunderstrikers are followers of Osgard spirit that vows to fight, not flee.      They inherit great thunder and lightning powers, wielding legendary Stormhammer and Flashaxe to launch deadly attacks against enemies.");
                        //  dialog.Text("~vows to fight, not flee. They inherit great thunder");
                        //  dialog.Text("~and lightning powers, wielding PlayDragonGate Stormhammer");
                        // dialog.Text("~and Flashaxe to launch deadly attacks against enemies.");

                        // dialog.Option("I~want~to~get~promoted.", 1);
                        dialog.Option("Learn~Class~skills.", 2);
                        dialog.Option("I`ll~talk~to~you~later.", 255).AddAvatar(31).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        switch (client.Player.Class)
                        {
                            case 9000:
                                {
                                    if (client.Player.Level < 15)
                                    {
                                        dialog.Text("	When you reach Level 15, you can get promoted to [Thunderstriker] and learn more thunder skills.");

                                        dialog.Option("I`ll~try~to~reach~Lv.15.", 255).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingShock))
                                        {
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrackingShock);
                                        }
                                        client.Player.Class++;
                                        dialog.AddText("Congratulations! You have been promoted.")
                                        .AddOption("Great,~thanks!", 255)
                                        .AddAvatar(234).FinalizeDialog();
                                    }
                                    break;
                                }
                            case 9001:
                                {

                                    if (client.Player.Level < 40)
                                    {
                                        dialog.Text("	When you reach Level 40, you can get promoted to [Elite~T.striker] and learn more thunder skills.");

                                        dialog.Option("I`ll~try~to~reach~Lv.40.", 255).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingImprinting))
                                        {
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.UndyingImprinting);
                                        }
                                        client.Player.Class++;
                                        dialog.AddText("Congratulations! You have been promoted.")
                                        .AddOption("Great,~thanks!", 255)
                                        .AddAvatar(234).FinalizeDialog();
                                    }


                                    break;
                                }
                            case 9002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        dialog.Text("	When you reach Level 70, you can get promoted to [Expert~T.striker] and learn more thunder skills.");

                                        dialog.Option("I`ll~try~to~reach~Lv.70.", 255).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            dialog.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        client.Inventory.Remove(1080001, 1, stream);
                                        if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.LightningShield))
                                        {
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.LightningShield);
                                        }
                                        client.Player.Class++;
                                        dialog.AddText("Congratulations! You have been promoted.")
                                        .AddOption("Great,~thanks!", 255)
                                        .AddAvatar(234).FinalizeDialog();
                                    }
                                    break;
                                }
                            case 9003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        dialog.Text("	When you reach Level 100, you can get promoted to [Master~T.striker] and learn more thunder skills.");

                                        dialog.Option("I`ll~try~to~reach~Lv.100.", 255).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            dialog.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                        client.Player.Class++;
                                        dialog.AddText("Congratulations! You have been promoted.")
                                        .AddOption("Great,~thanks!", 255)
                                        .AddAvatar(234).FinalizeDialog();
                                    }

                                    break;
                                }
                            case 9004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        dialog.Text("	When you reach Level 110, you can get promoted to [Divine~T.striker] and learn more thunder skills.");

                                        dialog.Option("I`ll~try~to~reach~Lv.110.", 255).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            dialog.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                        client.SendSysMesage("You received 1 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                        client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                        client.Player.Class++;
                                        dialog.AddText("Congratulations! You have been promoted.")
                                        .AddOption("Great,~thanks!", 255)
                                        .AddAvatar(234).FinalizeDialog();
                                    }


                                    break;
                                }
                            default:
                                {
                                    dialog.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(5).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 2:
                    {
                        dialog.Text("	As the agent of Lord Thor, I`ll try my best to teach every Thunderstriker to wield the power of thunder.");

                        dialog.Option("Learn~Class~skill.", 10);
                        dialog.Option("Learn~Thunderstriker`s~pure~skill.", 11);
                        dialog.Option("Learn~XP~skill.", 12).AddAvatar(31).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        dialog.Text("	You can learn Class skills here.");

                        dialog.Option("Windstorm~Battleaxe[Lv.1]" + (client.MySpells.ClientSpells.ContainsKey(15800) ? "(Acquired)" : ""), 20);
                        dialog.Option("Devouring~Strike[Lv.1]" + (client.MySpells.ClientSpells.ContainsKey(15700) ? "(Acquired)" : ""), 21);
                        dialog.Option("Sky~Fall[Lv.1]" + (client.MySpells.ClientSpells.ContainsKey(15770) ? "(Acquired)" : ""), 22);
                        dialog.Option("Cracking~Shock[Lv.15]" + (client.MySpells.ClientSpells.ContainsKey(15720) ? "(Acquired)" : ""), 23);
                        dialog.Option("Undying~Imprinting[Lv.40]" + (client.MySpells.ClientSpells.ContainsKey(15820) ? "(Acquired)" : ""), 24);
                        dialog.Option("Lightning~Shield[Lv.70]" + (client.MySpells.ClientSpells.ContainsKey(15750) ? "(Acquired)" : ""), 25);
                        dialog.Option("Thunder~Blast[Lv.100]" + (client.MySpells.ClientSpells.ContainsKey(15810) ? "(Acquired)" : ""), 26);
                        dialog.Option("Megabolt[Lv.100]" + (client.MySpells.ClientSpells.ContainsKey(15790) ? "(Acquired)" : ""), 27).AddAvatar(31).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        dialog.Text("	Only when you`re a Thunderstriker in three straight lives, including your current one,");
                        dialog.Text("~you can learn the Pure Skills [%s]. If you`ve been 2nd-reborn, you can find the Rebirth");
                        dialog.Text("~Master to get Reincarnated for new class combination.");

                        dialog.Option("Heaven`sWrath[Lv.100]", 3);
                        dialog.Option("I`ll~think~about~it.", 255).AddAvatar(31).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        dialog.Text("	[ThunderRampage] is Thunderstriker`s XP skill that will increase your moving speed by 50% and strengthen the effects");
                        dialog.Text("~of [Windstorm Battleaxe] and [Lightning Shield] for 40 seconds. When right clicking, you can release a 10-pace");
                        dialog.Text("~damage circle to inflict damage.");

                        dialog.Option("ThunderRampage[Lv.3]", 13).AddAvatar(31).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ThunderRampage))
                        {
                            dialog.Text("ThunderGeneral has already acquired this skill. Go practice and feel its power.");

                            dialog.Option("Okay.", 255).AddAvatar(31).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            if (client.Player.Level >= 3)
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ThunderRampage);
                                dialog.Text("	You`ve acquired [ThunderRampage]. Go practice and feel its power.");

                                dialog.Option("Thanks!", 255).AddAvatar(31).FinalizeDialog();
                            }
                            else
                            {
                                dialog.Text("	The study of [ThunderRampage] requires you to reach Level 3.");

                                dialog.Option("I`ll~find~you~at~Lv.3!", 255).AddAvatar(31).FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level >= 100 && Database.AtributesStatus.IsThunderStriker(client.Player.FirstClass) && Database.AtributesStatus.IsThunderStriker(client.Player.SecoundeClass) && client.Player.Reborn == 2)
                        {
                            var SpellID = Role.Flags.SpellID.HeavensWrath;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                dialog.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            dialog.Text("	You`ve acquired [" + SpellID.ToString() + "]. Go practice and feel its power.");

                            dialog.Option("Thanks!", 255).AddAvatar(31).FinalizeDialog();
                        }
                        else
                        {
                            dialog.Text("	You must be a Thunderstriker level 100 in three straight lives till now before you can learn [Heaven`sWrath].");

                            dialog.Option("Alright.", 255).AddAvatar(31).FinalizeDialog();
                        }
                        break;
                    }
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                    {
                        Role.Flags.SpellID spellid = (Role.Flags.SpellID)0;
                        switch (Option)
                        {
                            case 20: { spellid = Role.Flags.SpellID.WindstormBattleaxe; break; }
                            case 21: { spellid = Role.Flags.SpellID.DevouringStrike; break; }
                            case 22: { spellid = Role.Flags.SpellID.SkyFall; break; }
                            case 23: { spellid = Role.Flags.SpellID.CrackingShock; break; }
                            case 24: { spellid = Role.Flags.SpellID.UndyingImprinting; break; }
                            case 25: { spellid = Role.Flags.SpellID.LightningShield; break; }
                            case 26: { spellid = Role.Flags.SpellID.ThunderBlast; break; }
                            case 27: { spellid = Role.Flags.SpellID.Megabolt; break; }
                        }
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)spellid))
                        {
                            dialog.Text("ThunderGeneral has already acquired this skill. Go practice and feel its power.");

                            dialog.Option("Okay.", 255).AddAvatar(31).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            byte spellLevel = 1;
                            switch (Option)
                            {
                                case 23: { spellLevel = 15; break; }
                                case 24: { spellLevel = 40; break; }
                                case 25: { spellLevel = 70; break; }
                                case 26:
                                case 27: { spellLevel = 100; break; }
                            }
                            if (client.Player.Level >= spellLevel)
                            {
                                client.MySpells.Add(stream, (ushort)spellid);
                                dialog.Text("	You`ve acquired [" + spellid.ToString() + "]. Go practice and feel its power.");

                                dialog.Option("Thanks!", 255).AddAvatar(31).FinalizeDialog();
                            }
                            else
                            {
                                dialog.Text("	The study of [" + spellid.ToString() + "] requires you to reach Level " + spellLevel + ".");

                                dialog.Option("I`ll~find~you~at~Lv." + spellLevel + "!", 255).FinalizeDialog();
                            }
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Thunderstriker)]
        public static void Thunderstriker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24270][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24270][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24270][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24270][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24270][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24270][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24270][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24270][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24270][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24270][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24270][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24270][Option111]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level < 15)
                        {
                            data.AddText("You`re not strong enough for a job promotion now. Keep practicing and come back when you reach Level 15.")
                            .AddOption("Okay.", 255)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 975,
                            Fascing = 7,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.PromotionPirate)]
        public static void PromotionPirate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 7000 && client.Player.Class <= 7058))
            {
                data.AddText("Sorry, but you are not a Pirate.")
                                                           .AddOption("Alright.", 255)
                                                           .AddAvatar(234).FinalizeDialog();
                return;
            }

            // var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);

            switch (Option)
            {
                case 0:
                    {
                        //if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        //{
                        // data.AddText("We Pirates are always in pursuit of freedom and strength. The rapier and pistol stand for cruelty and mercilessness.")
                        // .AddOption("Learn~about~Promotion.", 110);
                        // if (client.Player.Class == 7000)
                        //  data.AddOption("I~want~to~get~promoted", 1);
                        // else
                        //  data.AddOption("I~want~to~get~promoted", 4);
                        // data.AddOption("I`ll~think~about~it.", 255)
                        //.AddAvatar(234).FinalizeDialog();
                        // break;
                        //}
                        // if (client.Player.Class == 7000)
                        //{
                        // data.AddText("We Pirates are always in pursuit of freedom and strength. The rapier and pistol stand for cruelty and mercilessness.")
                        //  .AddOption("I~want~to~get~promoted", 1)
                        // .AddOption("I`ll~think~about~it.", 255)
                        //.AddAvatar(234).FinalizeDialog();
                        //  break;
                        // }
                        // else
                        {
                            data.AddText("<tip desc=He#only#robs#profiteers#and#helps#poor#people.#As#a#pirate,#he#has#never#killed#a#single#man,#and#he`s#known#as#the#Ocean#Lord.");
                            data.AddText(">Ocean Lord</tip>:\n");
                            data.AddText("We Pirates are always in pursuit of freedom and strength. The rapler and pistol stand for cruelty and mercllessness.")
                                // .AddOption("I~want~to~get~promoted", 4)
                               .AddOption("Learn~the~Class~Skill.", 5)
                               .AddOption("Learn~the~Pure~Skill.", 20)
                                //.AddOption("Pirate Epic Quest (2nd Rebirth L70).", 194)
                               .AddOption("I`ll~think~about~it.", 255)
                               .AddAvatar(234).FinalizeDialog();
                        }


                        break;
                    }
                //case 177:
                //  {
                //client.Teleport(120, 186, 10271);
                // client.Player.QuestGUI.SendAutoPatcher("Pay a visit to Jack Sparrow.", 10271, 118, 191, (uint)MsgNpc.NpcID.JackSparrow);
                // client.SendSysMesage("You`ve arrived in the Sea of Rage. Hurry and look for the Heart of the Sea! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                // break;
                // }
                case 194:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("<tip desc=He#only#robs#profiteers#and#helps#poor#people.#As#a#pirate,#he#has#never#killed#a#single#man,#and#he`s#known#as#the#Ocean#Lord.");
                            data.AddText(">Ocean Lord</tip>:\n");
                            data.AddText("Sorry, you haven`t reached 2nd-reborn and level 70 yet, to Go and do the mission of Epic Pirate.")
                       .AddOption("Alright.", 255).AddAvatar(234).FinalizeDialog();
                        }
                        else if (client.Player.Level < 70)
                        {
                            data.AddText("<tip desc=He#only#robs#profiteers#and#helps#poor#people.#As#a#pirate,#he#has#never#killed#a#single#man,#and#he`s#known#as#the#Ocean#Lord.");
                            data.AddText(">Ocean Lord</tip>:\n");
                            data.AddText("Sorry, you haven`t reached 2nd-reborn and level 70 yet, to Go and do the mission of Epic Pirate.")
                       .AddOption("Alright.", 255).AddAvatar(234).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("<tip desc=He#only#robs#profiteers#and#helps#poor#people.#As#a#pirate,#he#has#never#killed#a#single#man,#and#he`s#known#as#the#Ocean#Lord.");
                            data.AddText(">Ocean Lord</tip>:\n");
                            data.AddText("The beautiful Para Island is now lying at the bottom of the sea. Fearless Pirates, go explore the Sea of RageBrave Pirate, go into the furious sea and find the Heart of the Sea to be blessedand get the legendary Pirate Epic Weapons. My old friend, Jake Sparrow is waiting for you!")
                               .AddOption("Head to see Jack Sparrow.", 177)
                               .AddAvatar(234).FinalizeDialog();
                        }
                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Pirate` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(234).FinalizeDialog();


                        break;
                    }
                // case 111:
                //{
                //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                // {
                //  client.Inventory.Add(stream, 3005160);
                // client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                // }
                //data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                //.//AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                // if (client.Player.Class == 7000)
                // data.AddOption("I~want~to~get~promoted", 1);
                //  else
                //  data.AddOption("I~want~to~get~promoted", 4);
                // data.AddAvatar(234).FinalizeDialog();


                //  break;
                // }
                case 4:
                    {
                        if (client.Player.Level >= 15 && client.Player.Level < 40 && client.Player.Class == 7001)
                        {
                            data.AddText("As a Pirate, you`ll need to reach Level 40 to get promoted to a Pirate Gunner. You will learn Adrenaline Rush, Eagle Eye, Blackbeard`s Rage and Black Spot after promotion.")
                                .AddText("~If you never change your class as a Pirate after the 2nd Rebirth, you can learn Scurvy Bomb.")
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(234).FinalizeDialog();


                        }
                        switch (client.Player.Class)
                        {
                            case 7001:
                                {

                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 7002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 7003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 7004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(5).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {

                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received 1 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Pirate. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(5).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("If you want to learn the Scurvy Bomb, you`ll need to be a pure Pirate, who`ve been a Pirate for 3 straight lives. You can talk to the Rebirth Master in the Market to have the Reincarnation.")
                         .AddOption("Learn~the~skill.", 21)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(234).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        if (client.Player.Reborn >= 2 && Database.AtributesStatus.IsPirate(client.Player.FirstClass) && Database.AtributesStatus.IsPirate(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.ScurvyBomb;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You are not a pure Pirate, so you can`t learn the Scurvy Bomb.")
                    .AddOption("I~see.", 255)
                    .AddAvatar(234).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        data.AddText("As an Intern Pirate, you can get promoted to a Pirate and learn Blade Tempest after you reach Level 15. When you reach Level 20, you can learn Gale Bomb.")
                        .AddOption("Get~promoted.", 3)
                        .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(234).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 7000)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CannonBarrage);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BladeTempest);
                            }
                            client.Player.Class++;
                            data.AddText("You`ve been promoted!")
                            .AddOption("Great,~thanks!", 255)
                            .AddAvatar(234).FinalizeDialog();
                            //if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }
                        else
                        {
                            data.AddText("Sorry, you need level 15.")
                      .AddOption("Ah ok.", 255)
                      .AddAvatar(234).FinalizeDialog();
                        }


                        break;
                    }
                case 250:
                    {
                        data.AddText("Which Pirate Skill do you want to learn")
                          .AddOption("Blade~Tempest" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeTempest) ? "(Learnt)" : "(Lv.15)") + "", 6)
                          .AddOption("Gale~Bomb" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GaleBomb) ? "(Learnt)" : "(Lv.20)") + "", 7)
                              .AddOption("Adrenaline~Rush" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdrenalineRush) ? "(Learnt)" : "(Lv.40)") + "", 8)
                          .AddOption("Eagle~Eye" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EagleEye) ? "(Learnt)" : "(Lv.40)") + "", 9)
                          .AddOption("Blackbeard`s~Rage" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlackbeardsRage) ? "(Learnt)" : "(Lv.40)") + "", 10)
                          .AddOption("Kraken`s~Revenge" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KrakensRevenge) ? "(Learnt)" : "(Lv.70)") + "", 11)
                          .AddOption("Next.", 15)
                          .AddAvatar(234).FinalizeDialog();
                        break;
                    }
                case 251:
                    {
                        data.AddText("It`s said Pirate`s Epic skills contains the mighty power of sea which can devastate all things on the earth.\n")
                            .AddText("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n")
                        .AddText("[Sea Burial]: active, emits laser in a rectangle range to deal deadly damage.\n")
                         .AddText("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n")
                        .AddText("[Bloom of Death]: passive, creates a phantom after activating Windstorm to attack forcefully against enemies nearby.\n")
                         .AddText("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n")
                             .AddText("[Immortal Force]: active, makes the caster immune to enemies` skill damage.")
                        .AddText("Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½")

                       .AddOption("Sea~Burial.~[" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SeaBurial) ? "Learnt" : "Lv.15") + "] (2150 CPs).", 249)
                       .AddOption("Bloom~of~Death.~[" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloomofDeath) ? "Learnt" : "Lv.40") + "] (2150 CPs).", 248)
                       .AddOption("Immortal~Force.~[" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ImmortalForce) ? "Learnt" : "Lv.70") + "] (2150 CPs).", 247)
                       .AddOption("Learn~about~others.", 5)
                       .AddAvatar(234).FinalizeDialog();
                        break;
                    }
                case 249:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 7001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SeaBurial;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }
                        break;
                    }
                case 248:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 7002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BloomofDeath;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }
                        break;
                    }
                case 247:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 7003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ImmortalForce;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {

                        data.AddText("What type of Pirate skill would you like to learn?")

                        .AddOption("Universal~skill.", 250)
                        .AddOption("Skills~for~Epic~Weapon.", 251)
                        .AddAvatar(234).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 7001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Blackspot;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        data.AddText("Which skill do you want to learn")
                       .AddOption("Black~Spot" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Blackspot) ? "(Learnt)" : "(Lv.15)") + "", 12)
                       .AddOption("Windstorm" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Windstorm) ? "(Learnt)" : "(Lv.15)") + "", 13)
                       .AddOption("Learn~the~Golem" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem) ? "(Learnt)" : "(Lv.15)") + "", 14)
                            .AddOption("CannonBarrage" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CannonBarrage) ? "(Learnt)" : "(Lv.15)") + "", 60)
                       .AddOption("Previous~page.", 250).AddAvatar(234).FinalizeDialog();

                        break;
                    }
                case 60:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 7000)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.CannonBarrage;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 7001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Golem;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 7001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Windstorm;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 7001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 15 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BladeTempest;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 7001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.GaleBomb;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 7002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.AdrenalineRush;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 7002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.EagleEye;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 7002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BlackbeardsRage;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.Reborn > 1 && Database.AtributesStatus.IsPirate(client.Player.FirstClass))
                        {
                            if (client.Player.Level < 70 || client.Player.Class < 7003)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.KrakensRevenge;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry you are not reborn, or not is pirat-pirat ")
                                                                    .AddOption("Ok , thank you.", 255)
                                                                    .AddAvatar(5).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Pirate)]
        public static void Pirate(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24273][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24273][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24273][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24273][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24273][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24273][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24273][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24273][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24273][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24273][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24273][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24273][Option111]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level < 15)
                        {
                            data.AddText("You`re not strong enough for a job promotion now. Keep practicing and come back when you reach Level 15.")
                            .AddOption("Okay.", 255)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        break;
                    }
                case 1:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 975,
                            Fascing = 7,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }
            }
        }
        #endregion
        [NpcAttribute(NpcID.JiangHuNpc)]
        public static void JiangHuNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        data.AddText("Well, the Jiang Hu is quite chaotic. I`ll test your understanding with several questions. If you pass, you`re able to name a skill of your own and start your adventure as a Jiang Hu warrior!")
                                                       .AddOption("Okay.~I`m~ready!", 1)
                                                       .AddOption("Learn~about~the~Jiang~Hu.", 2)

                                                       .AddOption("I`ll~talk~to~you~later.", 255)
                                                       .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        if (client.Player.Reborn == 2 && client.Player.MyJiangHu == null)
                        {
                            var Pharaoh = new ActionQuery();//Edit By Pharaoh
                            Pharaoh.ObjId = client.Player.UID;
                            Pharaoh.dwParam = 0;
                            Pharaoh.Timestamp = (uint)Time32.timeGetTime().GetHashCode();
                            Pharaoh.Type = 258;
                            Pharaoh.Fascing = 0;
                            Pharaoh.PositionX = 0;
                            Pharaoh.PositionY = 0;
                            Pharaoh.ScheduleTime = 0;
                            Pharaoh.RemainTime = 0;
                            Pharaoh.Strings = new string[] { "LuaUIMgr_OpenWindow|617" };
                            client.Send(stream.ActionCreate(Pharaoh));
                        }
                        else
                        {
                            data.AddText("Sorry, you not have 2nd Reborn, or you are learning Jiang Hu,")
                             .AddOption("Ah,~I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();
                        }
                        break;
                    }

                case 2:
                    {
                        data.AddText("Being a fighter of the Jiang Hu, you can train your Inner-strength after activating `Jiang` button at the character window. Each time you study will cost you 10 Study Points")
                                                     .AddText("~and 1 Free Course, and will also decrease your Talent Status by 1. Remember, you can only perform up to 10 free studies in a day. If you`re at Basic Status, you can only perform extra study with CPs.")
                                                        .AddOption("How~to~raise~my~Talent~Status?", 3)
                                                     .AddOption("How~to~collect~Free~Courses?", 4)
                                                           .AddOption("What~if~I~want~to~quit?", 5)
                                                                  .AddOption("I`ll~talk~to~you~later.", 255)
                                                     .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("You need to activate the `Jiang` mode, and defeat a player whose Talent Status is not lower than yours to raise your Status Level by 1,")
                                                   .AddText("~up to Level 4. PK in the `Jiang` mode, win or lose, will only affect your Talent Status. Different levels of your Status makes your name glow different colors.")
                                                           .AddOption("Learn~about~other~things.", 2)
                                                          .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        data.AddText("Jiang Hu fighters will gain up to 100 Free Courses by accumulating the Talent Points over time, whether online or not. The higher the Talent Status,")
                                                   .AddText("~the more the Talent Points. While stay online in Twin City, you`ll receive Talent Points, up to x30 more than received in other areas.")
                                                           .AddOption("Learn~about~other~things.", 2)
                                                          .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("If you want to leave the Jiang Hu for a while, deactivate the `Jiang` mode and wait for 10 minutes to quit.")
                                                .AddText("~After that, you won`t receive threats from the Jiang Hu fighters, however, you can`t accumulate Talent Points. Remember, you`re welcome to return at anytime you want!")
                                                        .AddOption("Learn~about~other~things.", 2)
                                                       .AddOption("I`ll~talk~to~you~later.", 255).AddAvatar(6).FinalizeDialog();

                        break;
                    }
            }
        }
    }
}

