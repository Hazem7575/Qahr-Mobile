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

        [NpcAttribute(NpcID.MountRetailer)]
        public static void MountRetailer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MountTrainer, client.Player.Class, 702);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Wow, it seems that I need to get some more steeds for all of you guys to choose! Maybe I`ll be a millionaire. It`s just a")
                               .AddText("~matter of time.")
                              .AddOption("Yeah.~Tell~me~more.", 2)
                              .AddOption("I~want~to~buy~a~Steed.", 1)
                              .AddAvatar(15).FinalizeDialog();
                        }
                        else
                            goto case 1;

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
                case 2:
                    {
                        data.AddText("There are a few kinds of steeds here, with many more that you can get. A good steed is a pretty convenient thing to have.")
                               .AddText("~Able to take you out for relaxing rides around the country, or ready to help you when you are fighting for your life!")
                              .AddOption("Looking~forward~to~it!", 3)
                              .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("You can unite 2 mounts with different appearances to breed a new mount. In this way, you can also improve on your steed`s")
                                  .AddText("~lineage. You can talk to the SteedKeeper beside me if you want to learn more about it.")
                                 .AddOption("Sure.", 4)
                                 .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~learned~something~about~steeds~and~received~EXP~worth~1/12~of~an~EXP~Ball!");
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.bless);
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MountTrainer)]
        public static void MountTrainer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MountTrainer, client.Player.Class, 702);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level > 25)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Hey! Guys, look at me! Good Steeds! Nah, not me. I mean there are some good steeds for you to choose. Would you like to take a")
                            .AddText("~look?")
                                    // .AddOption("Steed?~Why~do~I~need~it?", 20)
                           .AddOption("learn~To~ride.", 10)
                           .AddAvatar(15).FinalizeDialog();
                            }
                            else
                            {
                                goto case 10;
                            }
                        }
                        else
                        {

                            data.AddText("I`m the best Mount Trainer in the Empire. You can learn the Mount Skills from me. It costs 50,000 silver for the Riding skill,")
                                .AddText("~100,000 silver for the Spook skill, and 1,000,000 silver for the War Cry skill. Is there something you needed?")
                                .AddOption("Learn~Riding" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding) ? "(Learnt)" : "(50,000~silver)") + "", 1)
                                .AddOption("Learn~Spook" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Spook) ? "(Learnt)" : "(100,000~silver)") + "", 3)
                                .AddOption("Learn~War~Cry" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarCry) ? "(Learnt)" : "(1,000,000~silver)") + "", 5)
                                .AddOption("ChargingVortex" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChargingVortex) ? "(Learnt)" : "(500,000~silver)") + "", 7)
                                .AddOption("Just~passing~by.", 255)
                                .AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 20:
                    {
                        data.AddText("A good steed can increase your movement and efficiency. Why not pay a visit to the")
                           .AddText("~Steed Dealer behind me to learn about the different kinds of steeds?")
                          .AddOption("As~you~wish.", 21)
                          .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        data.AddText("Pay~a~visit~to~the~Mount~Retailer.")
                             .AddOption("Yes.", 22)
                             .AddOption("No.", 255)
                             .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 22:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
                case 10:
                    {
                        data.AddText("I`m the best Mount Trainer in the Empire. You can learn the Mount Skills from me. It costs 50,000 silver for the Riding skill,")
                                                       .AddText("~100,000 silver for the Spook skill, and 1,000,000 silver for the War Cry skill. Is there something you needed?")
                                                       .AddOption("Learn~Riding" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Riding) ? "(Learnt)" : "(50,000~silver)") + "", 1)
                                                       .AddOption("Learn~Spook" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Spook) ? "(Learnt)" : "(100,000~silver)") + "", 3)
                                                       .AddOption("Learn~War~Cry" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WarCry) ? "(Learnt)" : "(1,000,000~silver)") + "", 5)
                                                       .AddOption("ChargingVortex" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChargingVortex) ? "(Learnt)" : "(500,000~silver)") + "", 7)
                                                       .AddOption("Just~passing~by.", 255)
                                                       .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        data.AddText("It`s upgradable. You`ll need to equip a two-handed weapon and be mounted to use this skill. You will attack a target area within 15 paces and cause")
                         .AddText("~70%damage on all targets within 3 paces. It costs 30 Stamina. Are you sure you want to pay 500,000 silver to learn it?")
                        .AddOption("Yes.", 8)
                        .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 30)
                        {
                            data.AddText("Sorry, but you`ll need to reach Level 30, first.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(15).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Money >= 500000)
                        {
                            var SpellID = Role.Flags.SpellID.ChargingVortex;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 500000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 500,000 silver. You are unable to learn the ChargingVortex skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("After you equip and ride your mount, you can right click the War Cry skill to dismount the players in a certain area whose")
                          .AddText("~mounts` Lineage Levels are lower than yours and force the Vigors of their mounts drop to 0. Cool down time: 60 seconds.")
                         .AddOption("Learn~War~Cry.", 6)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(15).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.Money >= 1000000)
                        {
                            var SpellID = Role.Flags.SpellID.WarCry;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 1000000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 1,000,000 silver. You are unable to learn the war cry skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("After you equip and ride your mount, you can right click the Spook skill to dismount a player whose mount`s Lineage Level is")
                          .AddText("~lower than yours and force the Vigor of his mount drop to 0. Cool down time: 15 seconds.")
                         .AddOption("Learn~Spook.", 4)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Money >= 100000)
                        {
                            var SpellID = Role.Flags.SpellID.Spook;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 100000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 100,000 silver. You are unable to learn the Spook skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        data.AddText("After the mount is equipped, you can right click the (Riding) button or drag the button into the shortcut column to ride")
                           .AddText("~your mount. The higher the Lineage Level, the higher Vigor and the less limits for your mount!")
                          .AddOption("Learn~Riding~skill.", 2)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(15).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Money >= 50000)
                        {
                            var SpellID = Role.Flags.SpellID.Riding;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(15).FinalizeDialog();
                                break;
                            }
                            client.Player.Money -= 50000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(15).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don`t have 50,000 silver. You are unable to learn the Riding skill.").AddOption("Oh,~I`ll~fetch~some~money!", 255).AddAvatar(15).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        #region NewPromate
        [NpcAttribute(NpcID.PromotionTaoist)]
        public static void PromotionTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 13000 && client.Player.Class <= 14058 || client.Player.Class == 10000 || client.Player.Class == 10001))
            {
                data.AddText("Sorry, but you are not a Taoist.").AddOption("Alright.", 255).AddAvatar(6).FinalizeDialog();
                return;
            }
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class >= 13002 && client.Player.Class <= 14058)
                        {
                            data.AddText("Every Taoist wants holy spirit, and few gain. You`re gifted in controlling inner power. Just pay more attention to details, you may be the next achiever!");
                            data.AddOption("Learn~class~skills.", 10)
                           .AddOption("Learn~Pure~Skills.", 50)
                           .AddOption("Nothing.", 255)
                           .AddAvatar(6).FinalizeDialog();
                            return;
                        }
                        data.AddText("Every Taoist wants holy spirit, and few gain. You`re gifted in controlling inner power. Just pay more attention to details, you may be the next achiever!");
                        if (client.Player.Class == 10000)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else if (client.Player.Class == 10001)
                            data.AddOption("I~want~to~get~promoted.", 3);
                        else
                            data.AddOption("I~want~to~get~promoted.", 53);
                        data.AddOption("I`ll~think~about~it.", 255);
                        data.AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("The Taoist are capable of casting devastating skills and deadly spells, and proved to be versatile. Do you want to be a Taoist, now?")
                          .AddOption("Yeah,~please!", 2)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 10000)
                        {
                            //  if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ChainBolt);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Lightning);
                            client.Player.Class++;
                            data.AddText("You`ve been promoted and learned corresponding XP skill. The system will notify you when your XP is full. Keep up with the good work!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(6).FinalizeDialog();

                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("There are two paths of the Taoist, the Fire and the Water. Which one would you choose?")
                          .AddOption("Fire~Taoist.", 4)
                          .AddOption("Water~Taoist.", 5)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        // if (!client.Inventory.HaveSpace(1))
                        // {
                        // data.AddText("Please make 1 more space in your inventory")
                        //      .AddOption("Let me check.", 255)
                        //  .AddAvatar(6).FinalizeDialog();
                        // break;
                        //  }
                        if (client.Player.Class == 10001)
                        {
                            // client.Inventory.Add(stream, 421079, 1);
                            // client.SendSysMesage("You received 1 (Super)EndBacksword.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                            client.Player.Class = 14002;
                            data.AddText("Good, you`ve finished the training as a Fire Taoist. Hope you can play the role of a saver in your groups well!")
                         .AddOption("Thank~you,~Master.", 255)
                         .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 5:
                    {
                        // if (!client.Inventory.HaveSpace(1))
                        ////  {
                        // data.AddText("Please make 1 more space in your inventory")
                        // .AddOption("Let me check.", 255)
                        // .AddAvatar(6).FinalizeDialog();
                        // break;
                        // }
                        if (client.Player.Class == 10001)
                        {
                            // client.Inventory.Add(stream, 421079, 1);
                            //  client.SendSysMesage("You received 1 (Super)EndBacksword.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                            client.Player.Class = 13002;
                            data.AddText("Good, you`ve finished the training as a Water Taoist. Hope you can play the role of a saver in your groups well!")
              .AddOption("Thank~you,~Master.", 255)
              .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 250:
                    {
                        if (client.HundredWeapons.ValidateSpells())
                        {
                            data.AddText("You've gained your missing skills.")
                                   .AddOption("Thanks.", 255)
                                   .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You don't have any skills that need to be reclaimed.")
                                   .AddOption("I~see.", 255)
                                   .AddAvatar(5).FinalizeDialog();
                        }
                        break;
                    }
                case 91:
                case 92:
                case 93:
                case 94:
                    {
                        Role.Flags.SpellID spellid = (Role.Flags.SpellID)0;
                        switch (Option)
                        {
                            case 91: { spellid = Role.Flags.SpellID.AuroraLotus; break; }
                            case 92: { spellid = Role.Flags.SpellID.BlessingTouch; break; }
                            case 93: { spellid = Role.Flags.SpellID.FlameLotus; break; }
                            case 94: { spellid = Role.Flags.SpellID.SearingTouch; break; }
                        }
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)spellid))
                        {
                            data.Text("Taoist Master has already acquired this skill. Go practice and feel its power.");

                            data.Option("Okay.", 255).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            byte spellLevel = 15;
                            if (client.Player.Level >= spellLevel)
                            {
                                if (client.Player.ConquerPoints >= 2150)
                                {
                                    client.Player.ConquerPoints -= 2150;
                                    client.MySpells.Add(stream, (ushort)spellid);
                                    data.Text("	You`ve acquired [" + spellid.ToString() + "]. Go practice and feel its power.");

                                    data.Option("Thanks!", 255).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                                    data.AddOption("Okay.");
                                    data.AddAvatar(5).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.Text("	The study of [" + spellid.ToString() + "] requires you to reach Level " + spellLevel + ".");

                                data.Option("I`ll~find~you~at~Lv." + spellLevel + "!", 255).FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 95:
                    {
                        data.AddText("Which Skill do you want to learn?");
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            data.AddOption("Aurora~Lotus [2150 CPs].", 91)
                            .AddOption("Blessing~Touch [2150 CPs].", 92);
                        }
                        else if (Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            data.AddOption("Flame~Lotus [2150 CPs].", 93)
                            .AddOption("Breaking~Touch [2150 CPs].", 94);
                        }
                        data.AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet Taoist will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 111:
                    {
                        //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        //{
                        //    client.Inventory.Add(stream, 3005160);
                        //    client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                        //}
                        data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                                    .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                        if (client.Player.Class == 10000)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else if (client.Player.Class == 10001)
                            data.AddOption("I~want~to~get~promoted.", 3);
                        else
                            data.AddOption("I~want~to~get~promoted.", 53);
                        data.AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 190:
                    {
                        data.AddText("The Great Sun King has acquired the essence of Taoism and mad a Divine Panacea.")
                           .AddText(" This mythical pill can be used to reshape the backsword, and bring out the")
                           .AddText(" endless power. However, it`s not that easy. You'll need guidance from Sun King, profound")
                           .AddText(" understanding on art of Taoism, and also lucky. Many Taoists have set out to find the way")
                           .AddText(" of forging an Epic Weapon.")
                          .AddOption("Take me to see Sun King", 191)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 191:
                    {
                        if (client.Player.Level >= 70 && client.Player.Reborn >= 2)
                        {
                            client.Teleport(84, 84, 3933);
                        }
                        else
                        {
                            data.AddText("Please come back when you will be 2nd Reborn and with a higher level than 70.")
                                                                        .AddOption("Okay.", 255)
                                                                        .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 53:
                    {
                        switch (client.Player.Class)
                        {
                            case 14002:
                            case 13002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Taoist. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(6).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 14003:
                            case 13003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Taoist. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(6).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 14004:
                            case 13004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                                            break;
                                        }
                                        if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Taoist. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(6).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                        .AddOption("Thank~you,~Master.", 255)
                                                                                                        .AddAvatar(6).FinalizeDialog();
                                    break;
                                }
                        }
                        break;
                    }
                case 50:
                    {
                        if (client.Player.Class > 10001)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddText("Only Pure Fire Taoists, who have trained as Water Taoists for all 3 lives, can learn the Pure Skills, Azure Shield. If you`ve been Reborn twice, but are not a Pure Fire Taoist, you can take some")
                                    .AddText("~Oblivion Dew to the Reincarnation Seer in the Market and be Reincarnated! That way, you can obtain a new class combination. Oh, by the way, Chain Bolt is a XP skill, and it`ll allow you to throw")
                                    .AddText("~bolts of lightning at several targets during the XP time.")
                          .AddOption("AzureShield" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AzureShield) ? "(Learnt)" : "(Lv.40)") + "", 51)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();
                            }
                            else if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                data.AddText("Only Pure Fire Taoists, who have trained as Fire Taoists for all 3 lives, can learn the Pure Skills, Heaven Blade. If you`ve been Reborn twice, but are not a Pure Fire Taoist, you can take some")
                                   .AddText("~Oblivion Dew to the Reincarnation Seer in the Market and be Reincarnated! That way, you can obtain a new class combination. Oh, by the way, Chain Bolt is a XP skill, and it`ll allow you to throw")
                                   .AddText("~bolts of lightning at several targets during the XP time.")
                         .AddOption("Learn~Heaven~Blade" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HeavenBlade) ? "(Learnt)" : "(Lv.40)") + "", 52)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(6).FinalizeDialog();
                            }
                        }


                        break;
                    }
                case 51:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            if (client.Player.Reborn < 2)
                            {
                                data.AddText("Only pure Fire Taoists, who have trained as Water Taoists for all 3 of their lives, can learn the Pure Skill, AzureShield.")
                                                                .AddOption("Ok~I~see.", 255)
                                                                .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (!Database.AtributesStatus.IsWater(client.Player.FirstClass) || !Database.AtributesStatus.IsWater(client.Player.SecoundeClass))
                            {
                                data.AddText("Only pure water Taoists, who have trained as Water Taoists for all 3 of their lives, can learn the Pure Skill, AzureShield.")
                                                                                       .AddOption("Ok~I~see.", 255)
                                                                                       .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Level < 40)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.AzureShield;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 52:
                    {
                        if (Database.AtributesStatus.IsFire(client.Player.Class))
                        {
                            if (client.Player.Reborn < 2)
                            {
                                data.AddText("Only pure Fire Taoists, who have trained as Fire Taoists for all 3 of their lives, can learn the Pure Skill, Heaven Blade.")
                                                                .AddOption("Ok~I~see.", 255)
                                                                .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (!Database.AtributesStatus.IsFire(client.Player.FirstClass) || !Database.AtributesStatus.IsFire(client.Player.SecoundeClass))
                            {
                                data.AddText("Only pure Fire Taoists, who have trained as Fire Taoists for all 3 of their lives, can learn the Pure Skill, Heaven Blade.")
                                                                                       .AddOption("Ok~I~see.", 255)
                                                                                       .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Level < 40)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.HeavenBlade;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I know many types of skills and spells for a true Taoist, like you! Which of them do you want to learn, first?")
                          .AddOption("Learn~magic.", 11)
                          .AddOption("Learn~XP~Skill.", 12)
                            .AddOption("Learn~Epic~Skills.", 95)
                            //.AddOption("Universal~skills.", 13)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        data.AddText("The Taoist`s skills are varied and powerful. Which skill do you want to learn?")
                        .AddOption("ChainBolt" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt) ? "(Learnt)" : "(Lv.1)") + "", 40)
                        .AddOption("Lightning" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning) ? "(Learnt)" : "(Lv.3)") + "", 41)
                        .AddOption("Volcano" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Vulcano) ? "(Learnt)" : "(Lv.40)") + "", 42);
                        if (client.Player.Class > 10001)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddOption("Revive" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Revive) ? "(Learnt)" : "[Lv.40]") + "", 43);
                            }
                        }
                        data.AddOption("SpeedLightning" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedLightning) ? "(Learnt)" : "[Lv.40]") + "", 44);
                        data.AddOption("I`ll~think~about~it.", 255).AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 44:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpeedLightning;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 43:
                    {
                        if (Database.AtributesStatus.IsWater(client.Player.Class))
                        {
                            if (client.Player.Level < 40)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.Revive;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Okay. , thank you.", 255)
                                                                     .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 42:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Vulcano;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (client.Player.Level < 3)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 3 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Lightning;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 40:
                    {
                        var SpellID = Role.Flags.SpellID.ChainBolt;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 58:
                    {
                        if (client.Player.Level < 93)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireofHell;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("The Taoist`s skills are varied and powerful. Which skill do you want to learn?")
                            //.AddOption("Thunder" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunder) ? "(Learnt)" : "(Lv.1)") + "", 20)
                           .AddOption("Cure" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cure) ? "(Learnt)" : "(Lv.1)") + "", 21)
                           .AddOption("Meditation" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Meditation) ? "(Learnt)" : "(Lv.44)") + "", 22);
                        if (client.Player.Class >= 10001)
                        {
                            data.AddOption("ChainBolt" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChainBolt) ? "(Learnt)" : "[Lv.1]") + "", 70)
                                .AddOption("Lightning" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Lightning) ? "(Learnt)" : "[Lv.1]") + "", 71);
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {

                                data.AddOption("Fire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fire) ? "(Learnt)" : "[Lv.40][Thunder~Level~4]") + "", 23)
                                    .AddOption("Tornado" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tornado) ? "(Learnt)" : "[Lv.92][Fire~Level~3]") + "", 24)
                                    .AddOption("FireRing" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireRing) ? "(Learnt)" : "(Lv.55)") + "", 25)
                                    .AddOption("FireMeteor" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireMeteor) ? "(Learnt)" : "(Lv.52)") + "", 26)
                                    .AddOption("FireCircle" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireCircle) ? "(Learnt)" : "(Lv.65)") + "", 27)
                                   .AddOption("Bomb" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Bomb) ? "(Learnt)" : "(Lv.82)") + "", 28)
                               .AddOption("FireofHell" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireofHell) ? "(Learnt)" : "(Lv.93)") + "", 58);
                            }
                            else if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddOption("HealingRain" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingRain) ? "(Learnt)" : "(Lv.40)") + "", 29)
                                    .AddOption("StarofAccuracy" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarofAccuracy) ? "(Learnt)" : "(Lv.45)") + "", 30)
                                    .AddOption("MagicShield" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicShield) ? "(Learnt)" : "(Lv.50)") + "", 31)
                                    .AddOption("Stigma" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Stigma) ? "(Learnt)" : "(Lv.55)") + "", 32)
                                    .AddOption("Invisibility" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Invisibility) ? "(Learnt)" : "(Lv.60)") + "", 33)
                                    .AddOption("Pray" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Pray) ? "(Learnt)" : "(Lv.70)") + "", 34)
                                    .AddOption("AdvancedCure" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AdvancedCure) ? "(Learnt)" : "(Lv.81)") + "", 35)
                                    .AddOption("Nectar" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Nectar) ? "(Learnt)" : "(Lv.94)") + "", 36);
                            }
                        }
                        data.AddOption("I`ll~think~about~it.", 255).AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 70:
                    {
                        var SpellID = Role.Flags.SpellID.ChainBolt;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 71:
                    {
                        var SpellID = Role.Flags.SpellID.Lightning;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 28:
                    {
                        if (client.Player.Level < 82)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 82 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Bomb;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 27:
                    {
                        if (client.Player.Level < 65)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 65 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireCircle;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 26:
                    {
                        if (client.Player.Level < 52)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 52 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireMeteor;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 25:
                    {
                        if (client.Player.Level < 55)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 55 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireRing;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 24:
                    {
                        if (client.Player.Level < 92)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 92 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        MsgServer.MsgSpell spell;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Fire, out spell))
                        {
                            if (spell.Level == 3)
                            {
                                var SpellID = Role.Flags.SpellID.Tornado;
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry. but you need thunder level 4.")
                                                     .AddOption("Okay.~I~see..", 255)
                                                     .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry. but you need thunder level 4.")
                                                 .AddOption("Okay.~I~see..", 255)
                                                 .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 23:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        MsgServer.MsgSpell spell;
                        if (client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Thunder, out spell))
                        {
                            if (spell.Level == 4)
                            {
                                var SpellID = Role.Flags.SpellID.Fire;
                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry. but you need thunder level 4.")
                                                     .AddOption("Okay.~I~see..", 255)
                                                     .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry. but you need thunder level 4.")
                                                 .AddOption("Okay.~I~see..", 255)
                                                 .AddAvatar(6).FinalizeDialog();
                        }


                        break;
                    }
                case 36:
                    {
                        if (client.Player.Level < 94)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 94 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Nectar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 35:
                    {
                        if (client.Player.Level < 81)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 81 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.AdvancedCure;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 34:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Pray;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        if (client.Player.Level < 60)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 60 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Invisibility;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 32:
                    {
                        if (client.Player.Level < 55)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 55 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Stigma;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.Level < 50)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 50 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MagicShield;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 30:
                    {
                        if (client.Player.Level < 45)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 45 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.StarofAccuracy;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 29:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.HealingRain;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.Level < 44)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 44 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Meditation;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        var SpellID = Role.Flags.SpellID.Cure;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();


                        break;
                    }
                case 20:
                    {
                        var SpellID = Role.Flags.SpellID.Thunder;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(6).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(6).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Taoist)]
        public static void Taoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24274][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24274][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddText("STR_ID_tNewInaugurationTask[24274][Text115]@@\n")
                                    .AddOption("STR_ID_tNewInaugurationTask[24274][Option113]@@\n", 1)
                                .AddAvatar(10).FinalizeDialog();
                                return;
                            }
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                data.AddText("STR_ID_tNewInaugurationTask[24274][Text125]@@\n")
                                    .AddOption("STR_ID_tNewInaugurationTask[24274][Option113]@@\n", 1)
                                .AddAvatar(10).FinalizeDialog();
                                return;
                            }

                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddText("STR_ID_tNewInaugurationTask[24274][Text114]@@\n")
                                    .AddOption("STR_ID_tNewInaugurationTask[24274][Option113]@@\n", 1)
                                .AddAvatar(10).FinalizeDialog();
                                return;
                            }
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                data.AddText("STR_ID_tNewInaugurationTask[24274][Text124]@@\n")
                                    .AddOption("STR_ID_tNewInaugurationTask[24274][Option113]@@\n", 1)
                                .AddAvatar(10).FinalizeDialog();
                                return;
                            }

                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            if (Database.AtributesStatus.IsWater(client.Player.Class))
                            {
                                data.AddText("STR_ID_tNewInaugurationTask[24274][Text113]@@\n")
                                    .AddOption("STR_ID_tNewInaugurationTask[24274][Option113]@@\n", 1)
                                .AddAvatar(10).FinalizeDialog();
                                return;
                            }
                            if (Database.AtributesStatus.IsFire(client.Player.Class))
                            {
                                data.AddText("STR_ID_tNewInaugurationTask[24274][Text123]@@\n")
                                    .AddOption("STR_ID_tNewInaugurationTask[24274][Option113]@@\n", 1)
                                .AddAvatar(10).FinalizeDialog();
                                return;
                            }

                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24274][Text112]@@\n")
                                  .AddText("STR_ID_tNewInaugurationTask[24274][Text122]@@\n")
                                   .AddText("STR_ID_tNewInaugurationTask[24274][Text132]@@\n")
                                    .AddText("STR_ID_tNewInaugurationTask[24274][Text142]@@\n")
                                     .AddText("STR_ID_tNewInaugurationTask[24274][Text152]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24274][Option112]@@\n", 2)
                                 .AddOption("STR_ID_tNewInaugurationTask[24274][Option122]@@\n", 3)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24274][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24274][Option111]@@\n", 1)
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
                case 2:
                    {
                        client.Player.Class = 13002;
                        break;
                    }
                case 3:
                    {
                        client.Player.Class = 14002;
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PromotionMonk)]
        public static void PromotionMonk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 6000 && client.Player.Class <= 6058))
            {
                data.AddText("Sorry, but you are not a Monk.").AddOption("Alright.", 255).AddAvatar(226).FinalizeDialog();
                return;
            }


            // var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);


            switch (Option)
            {
                case 0:
                    {
                        //if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        // //{
                        //   data.AddText("Monk and Saints have a deep understanding of both the physical and spiritual world! Through intense training, Monk and Saints have gained devastating power. What is it, my child?")
                        //.AddOption("Learn~about~Promotion.", 110);
                        //if (client.Player.Class == 6000)
                        // data.AddOption("I~want~to~get~promoted", 1);
                        // else
                        // data.AddOption("I~want~to~get~promoted", 22);
                        // data.AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(259).FinalizeDialog();
                        //  break;
                        //  }
                        // if (client.Player.Class == 6000)
                        // {
                        // data.AddText("Monk and Saints have a deep understanding of both the physical and spiritual world! Through intense training, Monk and Saints have gained devastating power. What is it, my child?")
                        // /   //.AddOption("I~want~to~get~promoted", 1)
                        /// .AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(226).FinalizeDialog();
                        //break;
                        //}
                        // else
                        {
                            data.AddText("Head Abbot:                                                                                                                Monk and Saints have a deep understanding of both the physical and spiritual")
                                .AddText(" world! Through intense training, Monk and Saints have gained devastating power. What is it, my child")
                                /// .AddOption("I~want~to~get~promoted.", 22)
                               .AddOption("Learn~class~skills.", 3)
                               .AddOption("Learn~Pure~Skills.", 20)
                                .AddOption("Learn~Epic~Skills.", 90)
                                // .AddOption("Epic~Quest (L70, 2nd-reborn).", 253)
                               .AddOption("Nothing.", 255)
                               .AddAvatar(226).FinalizeDialog();
                        }

                        break;
                    }
                case 90:
                    {
                        data.AddText("Hello there, which skill do you want me to teach you?")
                      .AddOption("InfernalEcho [2150 CPs].", 83)
                       .AddOption("WrathOfTheEmperor [2150 CPs].", 84)
                      .AddOption("GraceOfHeaven [2150 CPs].", 85)
                      .AddOption("I`ll talk you later.", 255)
                      .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 85:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            var SpellID = Role.Flags.SpellID.GraceofHeaven;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 84:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            var SpellID = Role.Flags.SpellID.WrathoftheEmperor;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 83:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;

                            var SpellID = Role.Flags.SpellID.InfernalEcho;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Sorry...");
                            data.AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet Monk will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 111:
                    {
                        // if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3005160);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                        }
                        data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                                    .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                        if (client.Player.Class == 6000)
                            data.AddOption("I~want~to~get~promoted", 1);
                        else
                            data.AddOption("I~want~to~get~promoted", 22);
                        data.AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 253:
                    {
                        data.AddText("The alleviate miseries in the mortal world, Golden Cicada is recruiting tenacious Monks and Saints to obtain")
                            .AddText(" sacred sutrs in the Western Fairyland. The journey is full of trials and tribolations, but the heroes who go")
                            .AddText(" through them will be rewarded with a [Boundless Heart] and obtain the core secret of forging Monk`s Epic Weapon.")
                           .AddOption("Head to see Golden Cicada.", 91)
                            .AddOption("Learn~Monk`s~epic~skills.", 90)
                           .AddOption("Secret is secret.", 255)
                           .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 91:
                    {
                        if (client.Player.Level >= 70 && client.Player.Reborn >= 2)
                        {
                            client.Teleport(95, 70, 3983);
                        }
                        else
                        {
                            data.AddText("Please come back when you will be 2nd Reborn and with a higher level than 70.")
                                                                        .AddOption("Okay.", 255)
                                                                        .AddAvatar(226).FinalizeDialog();
                        }


                        break;
                    }
                case 22:
                    {

                        switch (client.Player.Class)
                        {
                            case 6001:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 6002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(226).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 6003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(226).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 6004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(226).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(226).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Monk. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(226).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(226).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Only a Pure Monk/Saint can learn the skill, Soul Shackle. If you`ve been reborn twice, but are not Pure, you can take Oblivion Dew to the Reincarnation Seer in the Market")
                            .AddText("~and ask him to help you get Reincarnated. That way, you can obtain a new class combination.")
                            .AddOption("I~want~to~learn~it~now.", 21)
                            .AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("You are not a Pure Monk/Saint, You can`t learn the Pure Skills. Go find the Reincarnation Seer in the Market and get reincarnated, first.")
                                                            .AddOption("Alright.", 255)
                                                            .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsMonk(client.Player.FirstClass) || !Database.AtributesStatus.IsMonk(client.Player.SecoundeClass))
                        {
                            data.AddText("You are not a Pure Monk/Saint, You can`t learn the Pure Skills. Go find the Reincarnation Seer in the Market and get reincarnated, first.")
                                                                                   .AddOption("Alright.", 255)
                                                                                   .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 6002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SoulShackle;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("The Monk`s skills are varied and powerful. Which skill do you want to learn?")
                            .AddOption("Deflection~Aura" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FendAura) ? "(Learnt)" : "(Lv.20)") + "", 4)
                            .AddOption("Tyrant~Aura" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TyrantAura) ? "(Learnt)" : "(Lv.20)") + "", 5)
                            .AddOption("Serenity" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Serenity) ? "(Learnt)" : "(Lv.40)") + "", 6)
                            .AddOption("Radiant~Palm" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RadiantPalm) ? "(Learnt)" : "(Lv.40)") + "", 7)
                            .AddOption("Tranquility" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Tranquility) ? "(Learnt)" : "(Lv.70)") + "", 8)
                            .AddOption("Next.", 9)
                            .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        data.AddText("The Monk`s skills are varied and powerful. Which skill do you want to learn?")
                            .AddOption("Compassion" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Compassion) ? "(Learnt)" : "(Lv.100)") + "", 10)
                            .AddOption("ElementalAuraMetal" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MetalAura) ? "(Learnt)" : "(Lv.100)") + "", 11)
                            .AddOption("ElementalAuraWood" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WoodAura) ? "(Learnt)" : "(Lv.100)") + "", 12)
                            .AddOption("ElementalAuraWater" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WatherAura) ? "(Learnt)" : "(Lv.100)") + "", 13)
                            .AddOption("ElementalAuraFire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireAura) ? "(Learnt)" : "(Lv.100)") + "", 14)
                            .AddOption("ElementalAuraEarth" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthAura) ? "(Learnt)" : "(Lv.100)") + "", 15)
                            .AddOption("Next.", 70)
                            .AddOption("Previous.", 3)
                            .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 70:
                    {
                        data.AddText("The Monk`s skills are varied and powerful. Which skill do you want to learn?")
                          .AddOption("WhirlwindKick" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick) ? "(Learnt)" : "(Lv.1)") + "", 71)
                          .AddOption("TripleAttack" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack) ? "(Learnt)" : "(Lv.1)") + "", 72)
                          .AddOption("Oblivion" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion) ? "(Learnt)" : "(Lv.1)") + "", 73)
                          .AddOption("Previous.", 9)
                          .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 71:
                    {
                        var SpellID = Role.Flags.SpellID.WhirlwindKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 622439, 1);
                        client.Inventory.Add(stream, 622439, 1);
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 72:
                    {
                        var SpellID = Role.Flags.SpellID.TripleAttack;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 73:
                    {
                        var SpellID = Role.Flags.SpellID.Oblivion;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 6004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.EarthAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 6004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FireAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 6004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.WatherAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 6004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.WoodAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 6004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MetalAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 6004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Compassion;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 6003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Tranquility;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 6002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.RadiantPalm;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 6:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 6002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Serenity;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 6001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.TyrantAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 6001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FendAura;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(226).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("You`ve met the requirement for job promotion. You can get promoted from Intern to a Monk and learn `Whirlwind Kick` when you are Level 15 or above.")
                          .AddOption("Get~promoted.", 2)
                          .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(226).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 6000)
                        {
                            //  if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WhirlwindKick))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.WhirlwindKick);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleAttack))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.TripleAttack);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Oblivion))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Oblivion);
                            client.Player.Class++;
                            data.AddText("You`ve been promoted and learned corresponding XP skill. The system will notify you when your XP is full. Keep up with the good work!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(226).FinalizeDialog();

                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Monk)]
        public static void Monk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24272][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24272][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24272][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24272][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24272][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24272][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24272][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24272][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24272][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24272][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24272][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24272][Option111]@@\n", 1)
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

        [NpcAttribute(NpcID.PromotionNinja)]
        public static void PromotionNinja(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 5000 && client.Player.Class <= 5058))
            {
                data.AddText("Sorry, but you are not a Ninja.").AddOption("Alright.", 255).AddAvatar(203).FinalizeDialog();
                return;
            }


            //var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        // if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        // {
                        // data.AddText("Cloaked in shadow, mysterious and aloof, the Ninja is the fastest class, with the most devastating attacks! So, what can I do for you?")
                        //.AddOption("Learn~about~Promotion.", 110);
                        //if (client.Player.Class == 5000)
                        //  data.AddOption("I~want~to~get~promoted", 1);
                        // else
                        // data.AddOption("I~want~to~get~promoted", 20);
                        // data.AddOption("I`ll~think~about~it.", 255)
                        //.AddAvatar(203).FinalizeDialog();
                        //break;
                        // }
                        // if (client.Player.Class == 5000)
                        //   {
                        //  data.AddText("Cloaked in shadow, mysterious and aloof, the Ninja is the fastest class, with the most devastating attacks! So, what can I do for you?")
                        //.AddOption("I~want~to~get~promoted", 1)
                        //.AddOption("I`ll~think~about~it.", 255)
                        //.AddAvatar(203).FinalizeDialog();
                        // break;
                        // }
                        //else
                        {
                            data.AddText("Wind Sage:                                                                                                                       Cloaked in shadow, mysterious and aloof, the Ninja is the fastest class, with the most devastating attacks! So, what can I do for you")
                                // .AddOption("I~want~to~get~promoted.", 20)
                                //.AddOption("Learn~Katana~skills.", 6)
                                // .AddOption("Learn~Scythe~skills.", 8)
                                // .AddOption("Learn~XP~skills.", 11)
                                   .AddOption("Learn~class~skills.", 3)
                                   .AddOption("Learn~Pure~Skills.", 15)
                                    .AddOption("Learn~Epic~Spells.", 82)
                                //.AddOption("Epic~Quest.~(L70,~2nd~reborn)", 80)
                                            .AddOption("Okay. I see.", 255)
                               .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Ninja` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                //case 111:
                //{
                // if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                // {
                //   client.Inventory.Add(stream, 3005160);
                //  client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                // }
                // data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                // .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                //  if (client.Player.Class == 5000)
                //data.AddOption("I~want~to~get~promoted", 1);
                //else
                //  data.AddOption("I~want~to~get~promoted", 20);
                //data.AddAvatar(259).FinalizeDialog();

                // break;
                //  }
                case 80:
                    {
                        data.AddText("The Divine Shadow has opened the Infinite Abyss and set 3 trials for Ninjas who have reached Level 70 of the 2nd rebirth.")
                            .AddText("~If you`re able to complete the trials, you`ll acquire the legendary ninja skills, and also obtain a Life`s Eye.")
                            .AddText("~This mythical eye can be used to transform your weapon into an Epic weapon.")
                          .AddOption("Enter~the~Infinite~Abyss.", 81)
                              .AddOption("Learn~Epic~Spells.", 82)
                          .AddOption("Sounds~crazy.", 255)
                          .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 82:
                    {
                        data.AddText("You can buy Shadow Clone, Twilight Dance, Fatal Spin, skills for 2150 CPs Per Skill. Ready to buy now?")
                      .AddOption("Buy Shadow Clone For 2150 CPs.", 83)
                       .AddOption("Buy Twilight Dance For 2150 CPs.", 84)
                      .AddOption("Buy Fatal Spin For 2150 CPs.", 85)
                      .AddOption("Sounds~crazy.", 255)
                      .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 85:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            var SpellID = Role.Flags.SpellID.FatalSpin;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Okay thanks.");
                            data.AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 84:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;
                            var SpellID = Role.Flags.SpellID.TwilightDance;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Okay thanks.");
                            data.AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 83:
                    {
                        if (client.Player.ConquerPoints >= 2150)
                        {
                            client.Player.ConquerPoints -= 2150;

                            var SpellID = Role.Flags.SpellID.ShadowClone;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come~on,~you~don`t~have~2,150~CPs~with~you.");
                            data.AddOption("Okay thanks.");
                            data.AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 81:
                    {
                        if (client.Player.Reborn >= 2 && client.Player.Level >= 70)
                        {
                            //32 35
                            client.Teleport(31, 36, 3849, 0);
                        }
                        else
                        {
                            data.AddText("Only Ninjas who have reached Level 70 of their 2nd rebirth are available for the Infinite Abyss.")
                              .AddOption("Alright.", 255)
                              .AddAvatar(203).FinalizeDialog();

                        }


                        break;
                    }
                case 20:
                    {
                        switch (client.Player.Class)
                        {
                            case 5001:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 5002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 5003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 5004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(203).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Ninja. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(203).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(203).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("You are over level 15. You can now get promoted to Ninja and learn Fatal Strike (XP skill).")
                            .AddOption("Yeah,~please!", 2)
                            .AddOption("Let~me~think~it~over.", 255)
                            .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        // if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        if (client.Player.Class == 5000)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalStrike);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ToxicFog);
                            client.Player.Class++;
                            data.AddText("You`ve been promoted and learned corresponding XP skill. The system will notify you when your XP is full. Keep up with the good work!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(203).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        data.AddText("Only Pure Ninjas can learn the Pure Skill, Counter Kill. If you`ve been Reborn twice, but are not a Pure Ninja, you can take some Oblivion Dew to the Reincarnation Seer in the Market and be")
                            .AddText("~Reincarnated! That way, you can obtain a new class combination.")
                 .AddOption("I~want~to~learn~it~now.", 16).AddOption("I`ll~think~about~it.", 255)
                 .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("You are not a Pure Ninja. Why not speak with the Reincarnation Seer in the Market and obtain a new class combination, first")
                                                            .AddOption("Okay. I see.", 255)
                                                            .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsNinja(client.Player.FirstClass) || !Database.AtributesStatus.IsNinja(client.Player.SecoundeClass))
                        {
                            data.AddText("You are not a Pure Ninja. Why not speak with the Reincarnation Seer in the Market and obtain a new class combination, first")
                                                                                   .AddOption("Okay. I see.", 255)
                                                                                   .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 5002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.CounterKill;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("Which skill do you want to learn")
              .AddOption("ShurikenVortex" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenVortex) ? " [Learnt]" : " [Lv.70]") + "", 12)
                       .AddOption("FatalStrike" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalStrike) ? " [Learnt]" : " [Lv.3]") + "", 13)
              .AddOption("Okay.~I see.", 255)
              .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 3)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 3 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.FatalStrike;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 5003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ShurikenVortex;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        data.AddText("Which skill do you want to learn")
                 .AddOption("Poison~Star" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar) ? " [Learnt]" : " [Lv.70]") + "", 4)
                 .AddOption("Archer~Bane" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane) ? " [Learnt]" : " [Lv.110]") + "", 5)
                            // .AddOption("MortalDrag" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag) ? "(Learnt)" : "(Lv.20)") + "", 9)
                            // .AddOption("BloodyScythe" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe) ? "(Learnt)" : "(Lv.40)") + "", 10)
                             .AddOption("GapingWounds" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GapingWounds) ? " [Learnt]" : " [Lv.40]") + "", 87)
                             .AddOption("SuperTwofoldBlade" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade) ? " [Learnt]" : " [Lv.70]") + "", 249)
                  .AddOption("Okay.~I see.", 255)
                  .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 91:
                    {
                        data.AddText("Which skill do you want to learn")
                            // .AddOption("Poison~Star" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar) ? " [Learnt]" : " [Lv.70]") + "", 4)
                            // .AddOption("Archer~Bane" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane) ? " [Learnt]" : " [Lv.110]") + "", 5)
                            // .AddOption("MortalDrag" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalDrag) ? "(Learnt)" : "(Lv.20)") + "", 9)
                             .AddOption("BloodyScythe" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BloodyScythe) ? " [Learnt]" : " [Lv.40]") + "", 10)
                            // .AddOption("GapingWounds" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.GapingWounds) ? " [Learnt]" : " [Lv.40]") + "", 87)
                  .AddOption("Okay.~I see.", 255)
                  .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 87:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 5002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.GapingWounds;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 5002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BloodyScythe;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level < 20 || client.Player.Class < 5001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 20 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MortalDrag;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;

                    }

                case 6:
                    {
                        data.AddText("Which skill do you want to learn?")
                  .AddOption("TwofoldBlades" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwofoldBlades) ? "(Learnt)" : "(Lv.40)") + "", 7)
                  .AddOption("SuperTwofoldBlades" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperTwofoldBlade) ? "(Learnt)" : "(Lv.70)") + "", 249)
                  .AddOption("I`ll~think~about~it.", 255)
                  .AddAvatar(203).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 5002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.TwofoldBlades;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 249:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 5003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SuperTwofoldBlade;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        data.AddText("The XP skills for Ninjas are Fatal Strike and Shuriken Vortex. Fatal Strike makes flash appearance, and Shuriken Vortex turns the caster into a cyclone. Ninjas are mysterious and unpredictable. Toxic Fog frightens the enemies, while Gaping Wounds quickly puts off enemies` guard. Ninja`s Epic skills will boost stunning power in a flash, and knock down enemies by surprise. Which one do you want to learn?")
                            // .AddOption("Poison~Star" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PoisonStar) ? "(Learnt)" : "(Lv.70)") + "", 4)
                            // .AddOption("Archer~Bane" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArcherBane) ? "(Learnt)" : "(Lv.100)") + "", 5)
                            //.AddOption("ToxicFog" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ToxicFog) ? "(Learnt)" : "(Lv.15)") + "", 14)
                 .AddOption("Learn~class~skills.", 8)
                 .AddOption("Learn~Scythe~skills.", 91)
                 .AddOption("Learn~XP~skills.", 11)
                            // .AddOption("Learn~exclusive~Epic~skills.", 82)
                            // .AddOption("Okay.~I~see.", 255)
                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 15 || client.Player.Class < 5001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ToxicFog;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        if (client.Player.Level < 110 || client.Player.Class < 5005)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 110 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ArcherBane;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(203).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        if (client.Player.Reborn == 0)
                        {
                            data.AddText("Only reborn Ninja-ninja, who have spent all 2 lives as Ninja, can learn the that Skill, PoisonStar.")
                                                       .AddOption("Okay.~I~see..", 255)
                                                       .AddAvatar(203).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Reborn == 1 && Database.AtributesStatus.IsNinja(client.Player.FirstClass) || client.Player.Reborn == 2 && Database.AtributesStatus.IsNinja(client.Player.SecoundeClass))
                        {
                            if (client.Player.Level < 70 || client.Player.Class < 5003)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.PoisonStar;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(203).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Okay. , thank you.", 255)
                                                                     .AddAvatar(203).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only reborn Ninja-ninja, who have spent all 2 lives as Ninja, can learn the that Skill, PoisonStar.")
                                                     .AddOption("Okay.~I~see..", 255)
                                                     .AddAvatar(203).FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Ninja)]
        public static void Ninja(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24268][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24268][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24268][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24268][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24268][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24268][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24268][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24268][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24268][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24268][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24268][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24268][Option111]@@\n", 1)
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

        [NpcAttribute(NpcID.PromotionWarrior)]
        public static void PromotionWarrior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 2000 && client.Player.Class <= 2058))
            {
                data.AddText("Sorry, but you are not a Warrior.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                return;
            }
            //var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);
            switch (Option)
            {
                case 0:
                    {
                        // if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        // {
                        // data.AddText("To be a good Warrior, you must know your weapon, your armor be a 2nd skin, and use your shield to ward off your enemy! Remember, fortune favors the brave, and you must keep your head, even in the darkest hours. So, what can I do for you?.")
                        // .AddOption("Learn~about~Promotion.", 110);
                        // if (client.Player.Class == 2000)
                        // data.AddOption("I~want~to~get~promoted", 1);
                        // else
                        // data.AddOption("I~want~to~get~promoted", 3);
                        //data.AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(259).FinalizeDialog();
                        // break;
                        //  }
                        // if (client.Player.Class == 2000)
                        //{
                        //data.AddText("To be a good Warrior, you must know your weapon, your armor be a 2nd skin, and use your shield to ward off your enemy! Remember, fortune favors the brave, and you must keep your head, even in the darkest hours. So, what can I do for you?.")
                        //.AddOption("I~want~to~get~promoted", 1)
                        /// .AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(8).FinalizeDialog();
                        // break;
                        // }
                        // else
                        {
                            data.AddText("Warrior God:                                                                                                               Warriors destroy the enemies and shield the compatrits. But remember, violence is a means of attaining a goal. Never let yourself sink into killing.")
                                //.AddOption("I~want~to~get~promoted.", 3)
                                //.AddOption("Learn~class~skills.", 5)
                                //   Avatar= 0  There are 3 superior skills. Which skill do you want to learn?Avatar= 0  Summon~Guard~[Lvl~15] Avatar= 0  Fire~Evil~[Lvl~40]Avatar= 0  Let~me~think.


                                  .AddOption("Learn~skills~for~Fist.", 130)
                                // .AddOption("Learn~class~skills.", 5)
                               .AddOption("Learn~shield~skills.", 4)

                               .AddOption("Learn~XP~skills.", 10)
                                   .AddOption("Learn~weapon~skills.", 20)
                                       .AddOption("Learn~Pure~Skills.", 21).AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }
                case 130:
                    {
                        data.AddText("If you`re interested in the skills for Fist, I`m glad to give some guidance.")
                                     .AddText("[Wave of Blood]: Active skill for Fist. Attack targets and restore XP.\n[Backfire]: Active skill for Fist. Reflect the damage received and restore Stamina.\n[Scar of Earth]: Passive skill for Fist.")
                                     .AddText("~Attack targets and inflicts bleeding.\n [Twist of War]: Passive skill for Fist. Retrieve XP status after revival.\n[Maniac Dance]: XP skill for Fist. Immune to physical attack and inflict damage.")
                           .AddOption("Scar~of~Earth.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScarofEarth) ? "(Acquired)" : "(Lv.40)") + "", 131)
                            .AddOption("Wave~of~Blood.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.WaveofBlood) ? "(Acquired)" : "(Lv.40)") + "", 132)
                            .AddOption("Maniac~Dance.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ManiacDance) ? "(Acquired)" : "(Lv.40)") + "", 133)
                             .AddOption("Twist~of~War.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar) ? "(Acquired)" : "(Lv.70)") + "", 134)
                           .AddOption("Backfire.~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TwistofWar) ? "(Acquired)" : "(Lv.100)") + "", 135)
                             .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 46:
                    {
                        if (client.Player.Level < 15)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SummonVoltaicWarg;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 44:
                    {

                        {
                            var SpellID = 4060;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {

                                client.MySpells.Remove((ushort)SpellID, stream);
                                data.AddText("All done, the skill is now removed.")
                               .AddOption("Okay.", 255)
                               .AddAvatar(6)
                              .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Sorry, you haven't learned the skill FireEvil. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                case 135:
                    {
                        if (client.Player.Level < 100)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Backfire;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 134:
                    {
                        if (client.Player.Level < 70)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.TwistofWar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 133:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ManiacDance;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 132:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.WaveofBlood;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 131:
                    {
                        if (client.Player.Level < 40)
                        {
                            data.AddText("You`re not strong enough to learn this skill. Keep practicing, and come back when you`re qualified.").AddOption("Alright.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ScarofEarth;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Warrior` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                //case 111:
                //  {
                // if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                // {
                //  client.Inventory.Add(stream, 3005160);
                //  client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                //}
                // data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                //   .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                //  ///if (client.Player.Class == 2000)
                // data.AddOption("I~want~to~get~promoted", 1);
                //else
                // data.AddOption("I~want~to~get~promoted", 3);
                // data.AddAvatar(259).FinalizeDialog();


                // break;
                //}
                case 3:
                    {
                        switch (client.Player.Class)
                        {
                            case 2001:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 2002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(8).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 2003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(8).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 2004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(8).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(8).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(8).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(8).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("Only Warriors who`ve trained for all three lives can learn the Pure Skill, Perseverance. If you`ve been Reborn twice, but are not a Pure Warrior, you take some Oblivion Dew to the Reincarnation Seer in the Market and be")
                            .AddText("~Reincarnated! That way, you can obtain a new class combination.")
                                                       .AddOption("I~want~to~learn~it~now.", 22).AddOption("I`ll~think~about~it.", 255)
                                                       .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 22:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("Only pure Warriors, who have spent all 3 lives as a Warrior, can learn the Pure Skill, Perseverance.")
                                                            .AddOption("Ok~I~see.", 255)
                                                            .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsWarrior(client.Player.FirstClass) || !Database.AtributesStatus.IsWarrior(client.Player.SecoundeClass))
                        {
                            data.AddText("Only pure Warriors, who have spent all 3 lives as a Warrior, can learn the Pure Skill, Perseverance.")
                                                                                   .AddOption("Ok~I~see.", 255)
                                                                                   .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 2002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Perseverance;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                        {
                            data.AddText("You`ve learned Fast Blade and Scented Sword!")
                                                             .AddOption("Okay.", 255)
                                                             .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            string line = "You just have learned the skill ";
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                line += Role.Flags.SpellID.FastBlader.ToString();
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                line += "/" + Role.Flags.SpellID.ScrenSword.ToString();
                            }
                            data.AddText(line)
                                            .AddOption("Thank you..", 255)
                                            .AddAvatar(8).FinalizeDialog();
                        }


                        break;
                    }
                case 10:
                    {
                        data.AddText("Which skill do you want to learn?")
                            .AddOption("Superman" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman) ? "(Learnt)" : "(Lv.3)") + "", 11)
                            .AddOption("Accuracy" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy) ? "(Learnt)" : "(Lv.3)") + "", 12)
                            .AddOption("Roar" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar) ? "(Learnt)" : "(Lv.3)") + "", 13)
                            .AddOption("Shield" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield) ? "(Learnt)" : "(Lv.3)") + "", 14)
                              .AddOption("FlyingMoon" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FlyingMoon) ? "(Learnt)" : "(Lv.40)") + "", 15)
                            .AddOption("Let~me~think~it~over.", 255)
                            .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        var SpellID = Role.Flags.SpellID.FlyingMoon;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 14:
                    {
                        var SpellID = Role.Flags.SpellID.Shield;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        var SpellID = Role.Flags.SpellID.Roar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        var SpellID = Role.Flags.SpellID.Accuracy;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        var SpellID = Role.Flags.SpellID.Superman;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 4:
                    {
                        data.AddText("Which skill do you want to learn?")
                            .AddOption("Dash" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Dash) ? "(Learnt)" : "(Lv.40)") + "", 5)//40
                            .AddOption("Shield~Block" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShieldBlock) ? "(Learnt)" : "(Lv.40)") + "", 6)//40
                            .AddOption("Magic~Defender" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender) ? "(Learnt)" : "(Lv.70)") + "", 7)//level 70
                            .AddOption("Defensive~Stance" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance) ? "(Learnt)" : "(Lv.70)") + "", 8)//level 70
                            .AddOption("Let~me~think~it~over.", 255)
                            .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        data.AddText("There are 3 superior skills. Which skill do you want to learn")
                             .AddOption("Summon Fox [Lv.15]" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SummonFox) ? "(Learnt)" : "") + "", 46)//40
                            //.AddOption("Fire Evil [Lv.40]" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FireEvil) ? "(Learnt)" : "") + "", 6)//40
                            // .AddOption("Magic~Defender" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MagicDefender) ? "(Learnt)" : "(Lv.70)") + "", 7)//level 70
                            //.AddOption("Defensive~Stance" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DefensiveStance) ? "(Learnt)" : "(Lv.70)") + "", 8)//level 70
                             .AddOption("Let~me~think.", 255)
                             .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 2002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ShieldBlock;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 2003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MagicDefender;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 2003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DefensiveStance;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(8).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(8).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("I`m proud that you made a mature decision! The warriors are adept in the use of weapons and shields for additional defense. Do you want to get promoted, now?")
                           .AddOption("Yeah,~please!", 2)
                           .AddOption("Let~me~think~it~over.", 255)
                           .AddAvatar(8).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 2000)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Superman))
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Superman);
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Shield))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Shield);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Roar))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Roar);
                            }
                            client.Player.Class++;
                            data.AddText("You`ve learned ways of the Warrior. Now, more weapons and armors are available for you, while new skills are at your disposal. Go test your power!")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(8).FinalizeDialog();
                            //if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Warrior)]
        public static void Warrior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24267][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24267][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24267][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24267][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24267][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24267][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24267][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24267][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24267][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24267][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24267][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24267][Option111]@@\n", 1)
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

        [NpcAttribute(NpcID.PromotionWindWalker)]
        public static void PromotionWindWalker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 16000 && client.Player.Class <= 16058))
            {
                switch (Option)
                {
                    case 0:
                        {

                            data.AddText("You don`t look like a Windwalker. What`s the matter? If you were ever a Windwalker in either of the previous two lives, you can learn [Frost Gaze I] from me.");
                            if (client.Player.Reborn == 1)
                            {
                                if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass))
                                    data.AddOption("Frost~Gaze~II.", 41);
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                    data.AddOption("Frost~Gaze~I.", 40);
                                else
                                    data.AddOption("Frost~Gaze~I.", 40);
                            }
                            else if (client.Player.Reborn == 2)
                            {
                                if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                    data.AddOption("Frost~Gaze~III.", 42);
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                                    data.AddOption("Frost~Gaze~II.", 41);
                                else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                    data.AddOption("Frost~Gaze~II.", 41);
                                else
                                    data.AddOption("Frost~Gaze~I.", 40);

                            }
                            else
                                data.AddOption("Frost~Gaze~I.", 40);

                            data.AddOption("I`ll~talk~to~you~later.", 255);
                            data.AddAvatar(333).FinalizeDialog();

                            break;
                        }
                    case 42:
                        {
                            if (client.Player.Reborn >= 2
                                && Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                            {
                                var SpellID = Role.Flags.SpellID.FrostGazeIII;

                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                    data.AddOption("Okay.", 8);
                                    data.AddAvatar(333).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(333).FinalizeDialog();
                            }
                            else
                                client.CreateBoxDialog("You're not qualified to learn this skill.");
                            break;
                        }
                    case 41:
                        {
                            if (client.Player.Reborn >= 1
                                && (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass)
                                || Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                || Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                            {
                                var SpellID = Role.Flags.SpellID.FrostGazeII;

                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                    data.AddOption("Okay.", 8);
                                    data.AddAvatar(333).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(333).FinalizeDialog();
                            }
                            else
                                client.CreateBoxDialog("You're not qualified to learn this skill.");
                            break;
                        }
                    case 40:
                        {
                            if (client.Player.Reborn > 0 && (Database.AtributesStatus.IsWindWalker(client.Player.Class) || Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                            {
                                var SpellID = Role.Flags.SpellID.FrostGazeI;

                                if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                                {
                                    data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                    data.AddOption("Okay.", 8);
                                    data.AddAvatar(333).FinalizeDialog();
                                    break;
                                }
                                client.MySpells.Add(stream, (ushort)SpellID);
                                data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                         .AddOption("Ok , thank you.", 255)
                                                                         .AddAvatar(333).FinalizeDialog();
                            }
                            else
                                client.CreateBoxDialog("You're not qualified to learn this skill.");
                            break;
                        }
                }
                return;
            }

            //var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 3795);
            //var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)20032, client.Player.Class, 3802);
            switch (Option)
            {
                case 0:
                    {
                        //if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        //  {
                        // data.AddText("Hey, FellowBrother! You`re back. Are you okay? No hurry, the Chief has sent me a letter and told me the things.");
                        // data.AddOption("So~what~can~we~do~now?", 60);
                        // data.AddAvatar(333).FinalizeDialog();
                        // break;
                        //}
                        data.AddText("Windwalker Lord:                                                                                                         Our Windwalker school was founded by Saint Wan on the principle of KINDNESS and JUSTICE. Once you decide to");
                        data.AddText("~join us as a Windwalker, you take the responsibility for the security of the country against alien invaders and devils.");
                        // if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        // data.AddOption("Learn~about~Promotion.", 52);
                        // data.AddOption("I~want~to~get~promoted.", 50);
                        data.AddOption("Learn~skills.", 8);
                        data.AddOption("Switch~to~another~branch.", 1);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 60:
                    {
                        data.AddText("Our school and the devil force have been at daggers drawn for centuries, but I don`t really get their purposes for");
                        data.AddText("~attacking us and intruding the imperial mausoleum this time. Now that we`ve completed the service in the mausoleum,");
                        data.AddText("~we should return to society. FellowBrother, set off to investigate and avenge the deaths of our fellows!");
                        data.AddOption("I~see.", 61);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                //case 61:
                // {
                // if (client.Inventory.HaveSpace(2))
                //{
                // if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                // {
                // client.SendSysMesage("You`ve completed the class quest of Windwalker and received 2 Lv.40 Elite Pride Fans (B)!");
                // client.Inventory.Add(stream, 626078, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                // client.CreateBoxDialog("You`ve completed the class quest of Windwalker and received 2 Lv.40 Elite Pride Fans (B)!");
                //}
                // }
                // else
                // {
                //client.CreateBoxDialog("Please make 2 more spaces in your inventory.");
                // }
                // break;
                // }
                case 52:
                    {
                        data.AddText("You`re such a talent and so promising. The Windwalker shool will amaze the world soon.");
                        data.AddText("~Remember, you can learn class skills and get promoted here to start your life. Would you like");
                        data.AddText("~to learn something about your class promotion and quests?");
                        data.AddOption("Thanks.", 53);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 53:
                    {
                        data.AddText("Congratulations! You`ve graduated from the training quest. From now on, you are no longer a simple recruit, but a trained");
                        data.AddText("~soldier. You can sign up for your class quests with me when you`re Level 12, and get promoted at Level 15.");
                        data.AddText("~The world is filled with dangers, so be sure and take care of yourself!");
                        data.AddOption("Let~the~legend~begin!", 54);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                //case 54:
                //{
                //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                // {
                //client.Inventory.Add(stream, 3005160);
                // client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                // }
                /// data.AddText("The TC Vice Captain (TwinCity 384,251) is now damned busy. Hey, if you can give him a hand,");
                ///data.AddText("~he will remember you for the rest of his life. Hah, just kidding. Anyway, would you like to help him?");
                ///data.AddOption("Send~me~there.", 56);
                //data.AddAvatar(333).FinalizeDialog();
                //break;
                // }
                case 56:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        break;
                    }
                case 50:
                    {

                        switch (client.Player.Class)
                        {
                            case 16000:
                                {
                                    if (client.Player.Level < 15)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 15 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.15.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        client.Player.Class++;
                                        data.AddText("Excellent! You`ve learned ways of the Windwalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                          .AddOption("Thank~you,~Master.", 255)
                                                                          .AddAvatar(333).FinalizeDialog();
                                        //if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                        client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);

                                    }
                                    break;
                                }
                            case 16001:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 40 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.40.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(333).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 626079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 626079, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received (Super)Pride Fan*2.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Windwalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 16002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 70 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.70.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(5).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 101069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.Inventory.Add(stream, 150139, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)MysticWindrobe and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the WindWalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 16003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 100 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.100.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(333).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 626199, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2PrideFan 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the WindWalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 16004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("FellowBrother, you`re not qualified to be [WindwalkerGuard]. You should reach Level 110 to get promoted.");
                                        data.AddOption("I`ll~try~to~reach~Lv.110.", 255);
                                        data.AddAvatar(333).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(1))
                                            {
                                                data.AddText("Please make 1 more space in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(333).FinalizeDialog();
                                                break;
                                            }
                                            if (client.Player.Reborn == 0)
                                            {
                                                client.Inventory.Add(stream, 626219, 1, 3, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received a (Super)+3PrideFan 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the WindWalker. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(333).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(333).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("Our Windwalker school is divided into two branches: Chaser and Stomper. Chaser focuses on");
                        data.AddText("~ranged combat, while Stomper excels in melee combat. I`m here representing the Chief to");
                        data.AddText("~impart distinctive skills to the Windwalkers.");
                        data.AddOption("Learn~skills~of~Stomper.", 9);
                        data.AddOption("Learn~skills~of~Chaser.", 12);
                        data.AddOption("Learn~rebirth~skill.", 13);
                        data.AddOption("Learn~universal~skills.", 14);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Frost Gaze is an exclusive rebirth skill for Windwalker. The heroes who got reborn to be Windwalker can learn Frost Gaze I. If both of your precious andd");
                        data.AddText("~current Class are Windwalker, you can learn Frost Gaze II. While for Windwalkers who were also a Windwalker in previous two lives, they can learn Frost Gaze III.");
                        if (client.Player.Reborn == 1)
                        {
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass))
                                data.AddOption("Frost~Gaze~II.", 41);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                data.AddOption("Frost~Gaze~I.", 40);
                            else
                                data.AddOption("Frost~Gaze~I.", 40);
                        }
                        else if (client.Player.Reborn == 2)
                        {
                            if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                data.AddOption("Frost~Gaze~III.", 42);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                                data.AddOption("Frost~Gaze~II.", 41);
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                                data.AddOption("Frost~Gaze~II.", 41);
                            else
                                data.AddOption("Frost~Gaze~I.", 40);

                        }
                        else
                            data.AddOption("Frost~Gaze~I.", 40);

                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (client.Player.Reborn >= 2
                            && Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.FrostGazeIII;

                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                data.AddOption("Okay.", 8);
                                data.AddAvatar(333).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                        }
                        else
                            client.CreateBoxDialog("You're not qualified to learn this skill.");
                        break;
                    }
                case 41:
                    {
                        if (client.Player.Reborn >= 1
                            && (Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass)
                            || Database.AtributesStatus.IsWindWalker(client.Player.Class) && (Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass))
                            || Database.AtributesStatus.IsWindWalker(client.Player.Class) && Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) && Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                        {
                            var SpellID = Role.Flags.SpellID.FrostGazeII;

                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                data.AddOption("Okay.", 8);
                                data.AddAvatar(333).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                        }
                        else
                            client.CreateBoxDialog("You're not qualified to learn this skill.");
                        break;
                    }
                case 40:
                    {
                        if (client.Player.Reborn > 0 && (Database.AtributesStatus.IsWindWalker(client.Player.Class) || Database.AtributesStatus.IsWindWalker(client.Player.FirstClass) || Database.AtributesStatus.IsWindWalker(client.Player.SecoundeClass)))
                        {
                            var SpellID = Role.Flags.SpellID.FrostGazeI;

                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                                data.AddOption("Okay.", 8);
                                data.AddAvatar(333).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(333).FinalizeDialog();
                        }
                        else
                            client.CreateBoxDialog("You're not qualified to learn this skill.");
                        break;
                    }
                case 14:
                    {
                        data.AddText("Omnipotence and Justice Chant are basic skills for Windwalker, which are available to both Chaser and Stomper learners. I`ll teach you if you want.");
                        data.AddOption("Omnipotence~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Omnipotence) ? "(Learned)" : "(Lv.3)") + "", 35);
                        data.AddOption("JusticeChant~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.JusticeChant) ? "(Learned)" : "(Lv.3)") + "", 36);
                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        var SpellID = Role.Flags.SpellID.JusticeChant;
                        if (client.Player.Level < 3)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 3.");
                            data.AddOption("I`ll~see~you~at~Lv.3!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 35:
                    {
                        var SpellID = Role.Flags.SpellID.Omnipotence;
                        if (client.Player.Level < 3)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 3.");
                            data.AddOption("I`ll~see~you~at~Lv.3!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        data.AddText("The Chaser branch is famous for its ability to control wind and outstanding ranged attacks.");
                        data.AddText("~FellowBrother, which skill would you like to learn?");
                        data.AddOption("SwirlingStorm~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SwirlingStorm) ? "(Learned)" : "(Lv.15)") + "", 30);
                        data.AddOption("ShadowofChaser~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShadowofChaser) ? "(Learned)" : "(Lv.15)") + "", 31);
                        data.AddOption("TripleBlasts~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.TripleBlasts) ? "(Learned)" : "(Lv.40)") + "", 32);
                        data.AddOption("Thundercloud~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thundercloud) ? "(Learned)" : "(Lv.70)") + "", 33);
                        data.AddOption("Thunderbolt~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Thunderbolt) ? "(Learned)" : "(Lv.100)") + "", 34);
                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        var SpellID = Role.Flags.SpellID.Thunderbolt;
                        if (client.Player.Level < 100)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 100.");
                            data.AddOption("I`ll~see~you~at~Lv.100!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 33:
                    {
                        var SpellID = Role.Flags.SpellID.Thundercloud;
                        if (client.Player.Level < 70)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 70.");
                            data.AddOption("I`ll~see~you~at~Lv.70!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        var SpellID = Role.Flags.SpellID.TripleBlasts;
                        if (client.Player.Level < 40)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 40.");
                            data.AddOption("I`ll~see~you~at~Lv.40!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        var SpellID = Role.Flags.SpellID.ShadowofChaser;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 30:
                    {
                        var SpellID = Role.Flags.SpellID.SwirlingStorm;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        data.AddText("The Stomper branch has developed overwhelming melee skills which enable the caster to disable an enemy with swift and flexible fighting moves.");
                        data.AddText("~FellowBrother, which skill would you like to learn?");
                        data.AddOption("AngerofStomper~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AngerofStomper) ? "(Learned)" : "(Lv.1)") + "", 20);
                        data.AddOption("BurntFrost~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BurntFrost) ? "(Learned)" : "(Lv.15)") + "", 21);
                        data.AddOption("HealingSnow~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HealingSnow) ? "(Learned)" : "(Lv.15)") + "", 22);
                        data.AddOption("RageofWar~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RageofWar) ? "(Learned)" : "(Lv.40)") + "", 23);
                        data.AddOption("HorrorofStomper~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.HorrorofStomper) ? "(Learned)" : "(Lv.40)") + "", 24);
                        data.AddOption("ChillingSnow~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ChillingSnow) ? "(Learned)" : "(Lv.70)") + "", 25);
                        data.AddOption("PeaceofStomper~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PeaceofStomper) ? "(Learned)" : "(Lv.70)") + "", 26);
                        data.AddOption("Next.", 10);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("The Stomper branch has developed overwhelming melee skills which enable the caster to disable an enemy with swift and flexible fighting moves.");
                        data.AddText("~FellowBrother, which skill would you like to learn?");
                        data.AddOption("RevengeTail~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RevengeTail) ? "(Learned)" : "(Lv.100)") + "", 27);
                        data.AddOption("FreezingPelter~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FreezingPelter) ? "(Learned)" : "(Lv.100)") + "", 28);
                        data.AddOption("I~want~to~learn~other~skills.", 8);
                        data.AddOption("I`ll~talk~to~you~later.", 255);
                        data.AddOption("Previous.", 9);
                        data.AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 28:
                    {
                        var SpellID = Role.Flags.SpellID.FreezingPelter;
                        if (client.Player.Level < 100)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 100.");
                            data.AddOption("I`ll~see~you~at~Lv.100!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 27:
                    {
                        var SpellID = Role.Flags.SpellID.RevengeTail;
                        if (client.Player.Level < 100)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 100.");
                            data.AddOption("I`ll~see~you~at~Lv.100!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 26:
                    {
                        var SpellID = Role.Flags.SpellID.PeaceofStomper;
                        if (client.Player.Level < 70)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 70.");
                            data.AddOption("I`ll~see~you~at~Lv.70!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        var SpellID = Role.Flags.SpellID.ChillingSnow;
                        if (client.Player.Level < 70)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 70.");
                            data.AddOption("I`ll~see~you~at~Lv.70!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 24:
                    {
                        var SpellID = Role.Flags.SpellID.HorrorofStomper;
                        if (client.Player.Level < 40)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 40.");
                            data.AddOption("I`ll~see~you~at~Lv.40!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        var SpellID = Role.Flags.SpellID.RageofWar;
                        if (client.Player.Level < 40)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 40.");
                            data.AddOption("I`ll~see~you~at~Lv.40!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        var SpellID = Role.Flags.SpellID.HealingSnow;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        var SpellID = Role.Flags.SpellID.BurntFrost;
                        if (client.Player.Level < 15)
                        {
                            data.AddText("The study of [" + SpellID.ToString() + "] requires FellowBrother to reach a certain level. FellowBrother, you can find me when you reach Level 15.");
                            data.AddOption("I`ll~see~you~at~Lv.15!", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        var SpellID = Role.Flags.SpellID.AngerofStomper;

                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("FellowBrother has already acquired this skill. Go practice and feel its power.");
                            data.AddOption("Okay.", 8);
                            data.AddAvatar(333).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(333).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (DateTime.Now > client.Player.CanChangeWindWalkerFree)
                        {
                            data.AddText("										*Branches of Windwalker School*");
                            data.AddText("Ã‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚Âª");
                            data.AddText("Stomper: Excels in melee combat with excellent attack range and remarkable recovery ability. Stomper`s exclusive skill");
                            data.AddText("		   can freeze enemies` blood to extend their skill cooldown time and response time.");
                            data.AddText("Chaser: Excels in ranged combat. With a unique focusing skill, Chaser is able to control the power of wind to attack");
                            data.AddText("		  enemies far away.");
                            data.AddText("Ã‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚Âª");
                            data.AddText("(You`re currently on the [RangedChaser] branch. Each Windwalker can switch his/her branch once every 30 days, free of charge.)");
                            if ((client.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack)
                                data.AddOption("Switch~to~[MeleeStomper]~branch.", 2);
                            else
                                data.AddOption("Switch~to~[RangedStomper]~branch.", 3);
                            data.AddOption("I`ll~think~about~it.", 255);
                            data.AddAvatar(333).FinalizeDialog();

                        }
                        else
                        {
                            string myStomper = "MeleeStomper";
                            if ((client.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack)
                                myStomper = "RangedStomper";

                            data.AddText("										*Branches of Windwalker School*");
                            data.AddText("Ã‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚Âª");
                            data.AddText("Stomper: Excels in melee combat with excellent attack range and remarkable recovery ability. Stomper`s exclusive skill");
                            data.AddText("		   can freeze enemies` blood to extend their skill cooldown time and response time.");
                            data.AddText("Chaser: Excels in ranged combat. With a unique focusing skill, Chaser is able to control the power of wind to attack");
                            data.AddText("		  enemies far away.");
                            data.AddText("Ã‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚ÂªÃ‚Â¡Ã‚Âª");
                            data.AddText("(You`re currently on the [" + myStomper + "] branch.)");
                            if ((client.Player.MainFlag & Role.Player.MainFlagType.OnMeleeAttack) != Role.Player.MainFlagType.OnMeleeAttack)
                            {
                                data.AddOption("Switch~to~[MeleeStomper]~branch.~(1WindCore)", 4);
                                data.AddOption("Switch~to~[MeleeStomper]~branch.~(27~CPs)", 5);
                            }
                            else
                            {
                                data.AddOption("Switch~to~[RangedChaser]~branch.~(1WindCore)", 6);
                                data.AddOption("Switch~to~[RangedChaser]~branch.~(27~CPs)", 7);
                            }
                            data.AddOption("I`ll~think~about~it.", 255);
                            data.AddAvatar(333).FinalizeDialog();

                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3301705, 1))
                        {
                            client.Player.SwitchWingWalkerAttack(stream);
                            client.Inventory.Remove(3301705, 1, stream);
                            data.AddText("Okay, I`ve changed your branch to [RangedChaser].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I can`t help you switch your Windwalker branch if you don`t have a Wind Core. You can exchange for one in the Horse Racing Store.");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            if (client.InTrade) return;
                            client.Player.SwitchWingWalkerAttack(stream);
                            client.Player.ConquerPoints -= 27;
                            data.AddText("Okay, I`ve changed your branch to [RangedChaser].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you switch the branch if you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.Contain(3301705, 1))
                        {
                            client.Player.SwitchWingWalkerAttack(stream);
                            client.Inventory.Remove(3301705, 1, stream);
                            data.AddText("Okay, I`ve changed your branch to [MeleeStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I can`t help you switch your Windwalker branch if you don`t have a Wind Core. You can exchange for one in the Horse Racing Store.");
                            data.AddOption("I~see.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 27)
                        {
                            if (client.InTrade) return;
                            client.Player.SwitchWingWalkerAttack(stream);
                            client.Player.ConquerPoints -= 27;
                            data.AddText("Okay, I`ve changed your branch to [MeleeStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`m afraid I can`t help you switch the branch if you don`t have enough CPs.");
                            data.AddOption("Alright.", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (DateTime.Now > client.Player.CanChangeWindWalkerFree)
                        {
                            client.Player.CanChangeWindWalkerFree = DateTime.Now.AddDays(30);
                            client.Player.SwitchWingWalkerAttack(stream);
                            data.AddText("Okay, I`ve changed your branch to [RangedStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (DateTime.Now > client.Player.CanChangeWindWalkerFree)
                        {
                            client.Player.CanChangeWindWalkerFree = DateTime.Now.AddDays(30);
                            client.Player.SwitchWingWalkerAttack(stream);
                            data.AddText("Okay, I`ve changed your branch to [MeleeStomper].");
                            data.AddOption("Thanks!", 255);
                            data.AddAvatar(333).FinalizeDialog();
                        }
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.WindWalker)]
        public static void WindWalker(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24269][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24269][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24269][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24269][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24269][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24269][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24269][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24269][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24269][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24269][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24269][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24269][Option111]@@\n", 1)
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

        [NpcAttribute(NpcID.PromotionTrojan)]
        public static void PromotionTrojan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 1000 && client.Player.Class <= 1058))
            {
                data.AddText("Trojans do not share their secrets of battle with others. I shall not teach you.").AddOption("Alright.", 255).AddAvatar(5).FinalizeDialog();
                return;
            }

            //var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);

            switch (Option)
            {
                case 0:
                    {
                        //  if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        //{
                        // data.AddText("Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe courage is the secret of victory. So, what can I do for you?")
                        //    .AddOption("Learn~about~Promotion.", 110);
                        //  if (client.Player.Class == 1000)
                        //   data.AddOption("I~want~to~get~promoted", 1);
                        // else
                        //   data.AddOption("I~want~to~get~promoted", 3);
                        //  data.AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(5).FinalizeDialog();
                        //  break;
                        // }

                        // if (client.Player.Class == 1000)
                        // {
                        //data.AddText("Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe courage is the secret of victory. So, what can I do for you?")
                        ////.AddOption("I~want~to~get~promoted.", 1)
                        // .AddAvatar(5).FinalizeDialog();
                        // break;
                        //  }
                        // else
                        {
                            data.AddText("Trojan Star:                                                                                                           Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe courage is the secret of victory. So, what can I do for you")
                               .AddOption("Learn~class~and~weapon~skills.", 4)
                               .AddOption("Claim Weapon Archives Skills.", 255)
                               .AddOption("Pure~skills~for~Trojans.", 30)
                                .AddOption("Learn~about~the~Epic~quest.", 5)
                               .AddOption("Okay.~I~see.", 255)
                               .AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Trojan` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                // case 111:
                // {
                //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                //  {
                //client.Inventory.Add(stream, 3005160);
                // client.SendSysMesage("You received 100 minute(s) of EXP.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                //  }
                // data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                //       .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                //  if (client.Player.Class == 1000)
                // data.AddOption("I~want~to~get~promoted", 1);
                //  else
                //  data.AddOption("I~want~to~get~promoted", 3);
                // data.AddAvatar(5).FinalizeDialog();

                // break;
                // }
                case 30:
                    {
                        data.AddText("The Dragon Whirl is a Pure skill, which means that only players who`ve been a Trojan on their two past lives and their current life as well can learn it. Though you`ve been Reborn twice, You are not a Pure Trojan.")
                                                 .AddText("~Why not take an Oblivion Dew and speak with the Reincarnation Seer  Through the power of Reincarnation, you can obtain new class combinations and make yourself a Pure Trojan!")
                                                 .AddOption("Learn~Dragon~Whirl~(Lv~40).", 31)
                                                     .AddOption("I`ll~think~about~it.", 255)
                                                     .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("After your 2nd Rebirth, you still have to be Reincarnated and become a Pure Trojan, to be able to learn the Dragon Whirl skill.")
                                                            .AddOption("Ok~I~see.", 255)
                                                            .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (!Database.AtributesStatus.IsTrojan(client.Player.FirstClass) || !Database.AtributesStatus.IsTrojan(client.Player.SecoundeClass))
                        {
                            data.AddText("You don'thave the combo ( Tro - Tro - Tro ) in your status.")
                                                 .AddOption("Okay.~I~see..", 255)
                                                 .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonWhirl;
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
                case 5:
                    {
                        data.AddText("300 years ago, General Pak was ordered to fight against the Flame Devastator. However, a traitor in his army led to Pak`s unexpected death.")
                                                    .AddText("~and the annihilation of his army. Pak`s Ghost (Twin City 267,194) refused to rest in peace, waiting for a chosen Trojan to change the history.")
                                                    .AddText("~The Trojan who successfully changes the history will earn a fabulous Epic Weapon.")
                                                    .AddText("\n\n    (Level 120+ or reborn Trojans will receive a Trojan Epic Weapon for free by completing a specified Trojan Epic Quest every day, and a series of Epic Quests in 90 days.)")
                                                    .AddOption("Head~to~see~General~Pak.", 101)
                                                    .AddOption("I see.", 255)
                                                        .AddAvatar(5).FinalizeDialog();



                        break;
                    }
                case 101:
                    {
                        data.AddText("Sorry, This Quest is Currently Closed and Will Be UP and Running Soon.")
                                                      .AddOption("Okay thanks.", 255)
                                                          .AddAvatar(5).FinalizeDialog();
                        break;
                    }
                //  case 101:
                // {
                //   client.Teleport(427, 298, 1002);
                //  client.Player.QuestGUI.SendAutoPatcher(1002, 427, 298, (uint)NpcID.PaksGhost);

                //  break;
                // }
                case 3:
                    {
                        switch (client.Player.Class)
                        {
                            case 1001:
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
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 1002:
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
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 1003:
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
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(5).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 1004:
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
                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
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
                case 4:
                    {
                        data.AddText("The XP skills for Trojans are Accuracy, Cyclone and Golem. Accuracy can triple your hit rate; Cyclone can increase your speed; By using the Golem skill, you can transform")
                                                        .AddText("~into a fearful Golem! Trojans are always fearless. Hercules makes them powerful and swift, while the mysterious Epic skills enable Trojans to be much fiercer than ever.")
                                                        .AddText("~So, which one did you want to learn?")
                                                        .AddOption("Trojan`s~skills.", 6)
                                                            .AddOption("Trojan`s~Epic~skills.", 7)
                                                            .AddOption("Let~me~see.", 255)
                                                            .AddAvatar(5).FinalizeDialog();


                        break;
                    }
                case 7:
                    {
                        data.AddText("Fierce as hurricane, Fatal Cross will crash all blocking the way. While the Mortal Strike holds a devastated power that can cause terrifying damage.")
                                                     .AddText("\n[Fatal Cross]: Epic active skill; Ranged attack.")
                                                     .AddText("\n[Mortal Strike]: Epic passive skill; Inflicts heavy damage within a fan-shape range.")
                                                     .AddOption("Fatal~Cross.~[5,000,000 Money].", 16)
                                                         .AddOption("Mortal~Strike.~[5,000,000 Money].", 17)
                                                         .AddOption("I`ll~think~about~it.", 255)
                                                         .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        if (client.Equipment.UseEpicTrojanWeapon())
                        {
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalStrike))
                            {
                                data.AddText("You`ve learned MortalStrike!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Money >= 5000000)
                            {
                                client.Player.Money -= 5000000;
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.MortalStrike);
                                data.AddText("You~successfully~acquired~the~skill,~[Mortal~Strike].~Equip~2~Trojan`s~Epic~Weapons~to~cast.~While~attacking,~the~caster~has~a~chance~to~cause~severe~damage~on~human~targets,");
                                data.AddText("~and~more~violent~to~monsters,~within~a~certain~range.");
                                data.AddOption("Thanks!").AddAvatar(5).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Come~on,~you~don`t~have~5,000,000~Money~with~you.");
                                data.AddOption("Sorry...");
                                data.AddAvatar(5).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You need to equip a Trojan`s Epic Weapon to learn [MortalStrike].")
                                         .AddOption("Ah ok.", 255)
                                         .AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 16:
                    {
                        if (client.Equipment.UseEpicTrojanWeapon())
                        {
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FatalCross))
                            {
                                data.AddText("You`ve~already~acquired~this~skill.").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            if (client.Player.Money >= 5000000)
                            {
                                client.Player.Money -= 5000000;
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FatalCross);
                                data.AddText("You~successfully~acquired~the~skill,~[Fatal~Cross].~This~active~skill~is~for~Trojan`s~Epic~Weapon~only.");
                                data.AddText("~Equip~2~Epic~Weapons~and~throw~the~Cross~Saber~in~target~direction,~inflicting~damage~on~all~the~targets");
                                data.AddText("~within~2~paces~along~its~attack~path.~Cost:~25~Stamina.");
                                data.AddOption("Thanks!").AddAvatar(5).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Come~on,~you~don`t~have~5,000,000~Money~with~you.");
                                data.AddOption("Sorry...");
                                data.AddAvatar(5).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You need to equip a Trojan`s Epic Weapon to learn [Fatal Cross].")
                                         .AddOption("Ah ok.", 255)
                                         .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 251:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MonsterHunter;
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
                case 252:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.UndyingWill;
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
                        data.AddText("The XP skills for Trojans are Accuracy, Cyclone and Golem. Accuracy can triple your hit rate, Cyclone can increase your attack speed, and by using the Golem skill, you can transform")
                                                   .AddText("~into a fearful Golem! Trojans are always fearless. Hercules makes them powerful and swift, and the Breath Focus recovers the Stamina in time.")
                                                   .AddText("When the Super Cyclone sweeps, no one can stand. Which one would you like to learn?")
                                                     .AddOption("New~Skill:~Monster~Hunter~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MonsterHunter) ? "(Learnt)" : "(Lv.40)") + "", 251)
                                                   .AddOption("New~Skill:~Undying~Will~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.UndyingWill) ? "(Learnt)" : "(Lv.40)") + "", 252)
                                                 .AddOption("New:~Super~Cyclone~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone) ? "(Learnt)" : "(Lv.40)") + "", 8)
                                                     .AddOption("Accuracy~and~Cyclone~" + ((client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy)) ? "(Learnt)" : "(Lv.40)") + ".", 9)
                                                       .AddOption("Golem~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Golem) ? "(Learnt)" : "(Lv.40)") + ".", 10)
                                                         .AddOption("Hercules~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Hercules) ? "(Learnt)" : "(Lv.40)") + ".", 11)
                                                             .AddOption("Spirit~Healing~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritHealing) ? "(Learnt)" : "(Lv.40)") + ".", 12)
                                                                 .AddOption("New:~Breath~Focus~" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus) ? "(Learnt)" : "(Lv.40)") + ".", 13)
                                                       .AddOption("Superior~skills.", 14)
                                                       .AddOption("Learn~weapon~skills.", 15)
                                                               .AddOption("I`ll~think~about~it.", 255)
                                                       .AddAvatar(5).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Reborn > 1)
                        {
                            data.AddText("Please choose the spell you want to learn.")
                                  .AddOption("CruelShade(L40).", 31).AddOption("I`ll~think~about~it.", 255)
                                  .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, only reborn players can learn this skill.")
                                                 .AddOption("Okay.~I~see.", 255)
                                                 .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 32:
                    {
                        if (client.Player.Reborn > 1)
                        {
                            if (client.Player.Level < 40 || client.Player.Class < 1002)
                            {
                                data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                      .AddOption("Okay.~I~see..", 255)
                                                      .AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            var SpellID = Role.Flags.SpellID.CruelShade;
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

                        break;
                    }
                case 8:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SuperCyclone;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("You~successfully~acquired~the~skill,~[Super~Cyclone].~This~XP~skill~exclusive~for~Trojans~boosts~the~caster`s~movement~and~attack~speed,");
                        data.AddText("~and~also~reduces~the~cost~of~Stamina~required~by~other~skills~by~half,~however,~the~caster~cannot~recover~XP~while~in~use~or~hunting~monsters.");
                        data.AddText("~Last~for~40~seconds.~");
                        data.AddOption("Thanks!");
                        data.AddAvatar(5);
                        data.FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
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
                case 11:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Hercules;
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
                case 12:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpiritHealing;
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
                        if (client.Player.Level < 90 || client.Player.Class < 1003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 90 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BreathFocus;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("You~successfully~acquired~the~skill,~[Breath~Focus]!~This~passive~skill~exclusive~for~Trojans");
                        data.AddText("~gives~a~100%~chance~to~recover~a~certain~amount~of~Stamina~if~fails~to~hit~a~target~with~a~random~active~skill");
                        data.AddText("~(excludes~Reborn~skills).");
                        data.AddOption("Thanks!");
                        data.AddAvatar(5);
                        data.FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 1002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                        {
                            data.AddText("You`ve learned Accuracy and Cyclone!")
                                                             .AddOption("Okay.", 255)
                                                             .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            string line = "You just have learned the skill ";
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Accuracy))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Accuracy);
                                line += Role.Flags.SpellID.Accuracy.ToString();
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Cyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.Cyclone);
                                line += "/" + Role.Flags.SpellID.Cyclone.ToString();
                            }
                            data.AddText(line)
                                            .AddOption("Thank you..", 255)
                                            .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 15:
                    {
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader) && client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                        {
                            data.AddText("You`ve learned Fast Blade and Scented Sword!")
                                                             .AddOption("Okay.", 255)
                                                             .AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            string line = "You just have learned the skill ";
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                                line += Role.Flags.SpellID.FastBlader.ToString();
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                                line += "/" + Role.Flags.SpellID.ScrenSword.ToString();
                            }
                            data.AddText(line)
                                            .AddOption("Thank you..", 255)
                                            .AddAvatar(5).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 1000)
                        {
                            data.AddText("The Trojans are all-out offensive melee combat fighters. Do you want to be a qualified Trojan, now?.")
                                                             .AddOption("Yeah,~please!", 2)
                                                             .AddAvatar(5).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only the powerful deserve to be trained. You are still a mere fledgeling.")
                                                              .AddOption("Oh,~I~see..", 255)
                                                              .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }
                case 2:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 1000)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.FastBlader))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.FastBlader);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScrenSword))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ScrenSword);
                            }
                            client.Player.Class++;
                            data.AddText("Excellent! You`ve learned ways of the Trojan. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                //.AddText("~For a limited time, you can buy special Super and Elite Kits that are just right for you! Only at the Kit Merchant! One more thing, the Vice Captain in Twin City (511,360) has a quest to test you. You won`t say NO, right?")
                                                              .AddOption("Thank~you,~Master.", 255)
                                                              .AddAvatar(5).FinalizeDialog();

                            // if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }
                        else
                        {
                            data.AddText("Only the powerful deserve to be trained. You are still a mere fledgeling.")
                                                                 .AddOption("Oh,~I~see..", 255)
                                                                 .AddAvatar(5).FinalizeDialog();
                        }


                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.Trojan)]
        public static void Trojan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {
                        if (client.HundredWeapons.Unlocked == false)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text1011]@@\n")
                                   .AddText("STR_ID_tNewInaugurationTask[24271][Text1012]@@\n")
                                   .AddText("STR_ID_tNewInaugurationTask[24271][Text1013]@@\n")
                                   .AddOption("STR_ID_tNewInaugurationTask[24271][Option1011]@@\n", 3)
                               .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24271][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24271][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24271][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24271][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text1011]@@\n")
                                 .AddText("STR_ID_tNewInaugurationTask[24271][Text1012]@@\n")
                                 .AddText("STR_ID_tNewInaugurationTask[24271][Text1013]@@\n")
                                 .AddOption("STR_ID_tNewInaugurationTask[24271][Option1011]@@\n", 3)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24271][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24271][Option111]@@\n", 2)
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
                case 2:
                    {
                        ActionQuery action = new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            dwParam = 456,
                            Fascing = 2,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y,

                        };
                        client.Send(stream.ActionCreate(action));

                        break;
                    }
                case 3:
                    {
                        data.AddText("STR_ID_tNewInaugurationTask[24271][Text2011]@@\n")
                                .AddText("STR_ID_tNewInaugurationTask[24271][Text2012]@@\n")
                                .AddText("STR_ID_tNewInaugurationTask[24271][Text2013]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24271][Option2011]@@\n", 4)
                            .AddAvatar(10).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.HundredWeapons.Unlocked = true;
                        client.Send(stream.CreateHundredWeaponsInfo(client, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                        client.Send(stream.ActionCreate(new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = client.Player.UID,
                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),
                            dwParam = DialogCommands.TrojanWeaponArchive,
                            PositionX = client.Player.X,
                            PositionY = client.Player.Y
                        }));
                        client.HundredWeapons.LoadPowerFoucs(client);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.PromotionArcher)]
        public static void PromotionArcher(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 4000 && client.Player.Class <= 4058))
            {
                data.AddText("Sorry, but you are not a Archer.")
                                                           .AddOption("Alright.", 255)
                                                           .AddAvatar(10).FinalizeDialog();
                return;
            }


            //var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TCCaptain, client.Player.Class, 0, p => p.FinishNpcId.Map == 1004);

            switch (Option)
            {
                case 0:
                    {
                        // if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        // {
                        // data.AddText("Born with sharp insight and swift skill, the Archers are expert in shooting arrows and wielding throwing knives to deadly purpose.")
                        //  .AddOption("Learn~about~Promotion.", 110);
                        //  if (client.Player.Class == 4000)
                        //data.AddOption("I~want~to~get~promoted", 1);
                        //   else
                        //  data.AddOption("I~want~to~get~promoted", 33);
                        // data.AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(259).FinalizeDialog();
                        //break;
                        //}
                        // if (client.Player.Class == 4000)
                        // {
                        ///data.AddText("Born with sharp insight and swift skill, the Archers are expert in shooting arrows and wielding throwing knives to deadly purpose.")
                        // .AddOption("I~want~to~get~promoted", 1)
                        //.AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(10).FinalizeDialog();
                        //  break;
                        //  }
                        // else
                        {
                            data.AddText("Storm Master:\n");
                            data.AddText("Born with sharp insight and swift skill, the Archers are expert in shooting arrows and wielding throwing knives to deadly purpose.")
                                //  .AddOption("I~want~to~get~promoted", 33)
                           .AddOption("Learn~the~Class~Skill.", 5)
                           .AddOption("Learn~the~Pure~Skill.", 30)
                                //.AddOption("Epic Weapon Quest.", 119)

                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
                // case 177:
                // {
                //  client.Teleport(34, 54, 11257);
                // client.SendSysMesage("You found the Elder Power sitting in the tomb tunnel. It seems he has been seriously injured.");
                //  client.Player.QuestGUI.SendAutoPatcher("Mo Chu is waiting for you over there. Go talk to him.", 11257, 37, 53, (uint)MsgNpc.NpcID.MoChuEpicArcherQuest);
                //client.Player.QuestGUI.SendAutoPatcher(11568, 407, 456, (uint)NpcID.QingyangTaoist2);
                //  break;
                // }
                case 119:
                    {
                        if (client.Player.Reborn < 2)
                        {
                            data.AddText("Storm Master:\n");
                            data.AddText("Sorry, you haven`t reached 2nd-reborn and level 70 yet, to Go and do the mission of Epic Archer`s.")
                       .AddOption("Alright.", 255).AddAvatar(10).FinalizeDialog();
                        }
                        else if (client.Player.Level < 70)
                        {
                            data.AddText("Storm Master:\n");
                            data.AddText("Sorry, you haven`t reached 2nd-reborn and level 70 yet, to Go and do the mission of Epic Archer`s.")
                       .AddOption("Alright.", 255).AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Storm Master:\n")
                            .AddText("The trial ground of Moism is open to Archers of 2nd rebirth level 70! \nIf you can pass the trial, you`ll get the precious <tip color=0xfff05050 desc=This#encyclopedia#contains#a#wide#range#of#technical#issues.#Use#it#to#convert#Archer`s#throwing#knife#into#an#epic#weapon.>Tiangong Kaiwu</tip>, which will\n")
                                      .AddText("convert your throwing knife into an epic weapon, and you`ll also be able to learn Archer`s epic skills.\n")
                                       .AddText("<tip color=0xff00fa00 desc=>After equipping the epic weapon, Archer`s normal attack damage will be decreased as the attack range increases.</tip>")
                            .AddOption("Enter trial ground.", 177)
                            .AddAvatar(10).FinalizeDialog();
                        }
                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Archer` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                //case 111:
                //  {
                //  if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                // {
                //  client.Inventory.Add(stream, 3005160);
                //  client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                // }
                //data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                //  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.");
                ///  if (client.Player.Class == 4000)
                // data.AddOption("I~want~to~get~promoted", 1);
                //else
                // data.AddOption("I~want~to~get~promoted", 33);
                //data.AddAvatar(259).FinalizeDialog();

                //  break;
                //  }
                case 33:
                    {
                        if (client.Player.Level >= 15 && client.Player.Level < 40 && client.Player.Class == 4001)
                        {
                            data.AddText("Sorry,~you~cannot~get~promoted~before~you~reach~level~40.~Please~train~harder.")
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(234).FinalizeDialog();
                            return;
                        }


                        switch (client.Player.Class)
                        {
                            case 4001:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1072031, 5))
                                        {
                                            data.AddText("I'm sorry but you need to have 5 EuxeniteOre in your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {

                                            client.Inventory.Remove(1072031, 5, stream);

                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(5).FinalizeDialog();
                                                    break;
                                                }
                                                //500069 ScarletBow
                                                client.Inventory.Add(stream, 500069, 1);
                                                client.SendSysMesage("You received 1 (Super)ScarletBow.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 4002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(2))
                                                {
                                                    data.AddText("Please make 2 more spaces in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(10).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 150139, 1);//150139 PearlRing
                                                client.Inventory.Add(stream, 133049, 1);//133049 ApeCoat
                                                client.SendSysMesage("You received 1 (Super)ApeCoat and 1 (Super)PearlRing.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            case 4003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (client.Player.Reborn == 0)
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                                   .AddOption("Let me check.", 255)
                                                                   .AddAvatar(10).FinalizeDialog();
                                                    break;
                                                }
                                                client.Inventory.Add(stream, 500189, 1, 2, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket, true);
                                                client.SendSysMesage("You received 1 (Super)+2RosewoodBow 2 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 4004://133089 EagleCoat
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(10).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            if (!client.Inventory.HaveSpace(2))
                                            {
                                                data.AddText("Please make 2 more spaces in your inventory")
                                                               .AddOption("Let me check.", 255)
                                                               .AddAvatar(10).FinalizeDialog();
                                                break;
                                            }
                                            if (client.Player.Reborn == 0)
                                            {
                                                client.Inventory.Add(stream, 133089, 1, 3, 1, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.NoSocket, true);
                                                client.SendSysMesage("You received 1 (Super)+3EagleCoat 1 socket.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                            }
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received 1 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Archer. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(10).FinalizeDialog();
                                        }
                                    }


                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                        .AddOption("Thank~you,~Master.", 255)
                                                                                                        .AddAvatar(10).FinalizeDialog();


                                    break;
                                }
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("Flight, ranged attack and maneuverability are powers all possessed by the archer.")
                           .AddOption("I~want~to~get~promoted", 2)
                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.Class == 4000)
                        {
                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.XpFly);
                            client.Player.Class++;
                            data.AddText("Good, you have trained hard, and completed my lesson this day. You have gained power and skills.")
                       .AddOption("Thank~you,~Master.", 255)
                       .AddAvatar(10).FinalizeDialog();
                            // if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }


                        break;
                    }
                case 30:
                    {
                        data.AddText("Only Pure Archers can learn the Pure Skill, Star Arrow. If you`ve been Reborn twice, but are not a Pure Archer, you can take some Oblivion Dew to the Reincarnation Seer in the Market and be")
                     .AddText("~Reincarnated. That way, you can obtain a new class combination.")
                            .AddOption("I~want~to~learn~it~now.", 31)
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(10).FinalizeDialog();



                        break;
                    }
                case 31:
                    {
                        if (client.Player.Reborn == 2 && Database.AtributesStatus.IsArcher(client.Player.FirstClass) && Database.AtributesStatus.IsArcher(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.StarArrow;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(10).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only pure Archers, whose classes for their past two lives and their current life are all Archers, can learn the pure skill Star Arrow.")
                      .AddOption("Okay.~I~see.", 255)
                      .AddAvatar(10).FinalizeDialog();
                        }


                        break;
                    }
                case 5:
                    {
                        data.AddText("The powerful Archers are expert in shooting clustered demons to death. When transforming into Oriental Assassins, they are able to wield the Throwing Knives to deadly purpose.")
                             .AddOption("Learn~Archer~skills.", 10)
                             .AddOption("Learn~Assassin~skills.", 11)
                           .AddOption("Learn~Epic~skills.", 12)
                             .AddOption("Leave.", 255)
                             .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        data.AddText("After you get promoted as an Eagle Archer at Level 40, you can equip a Throwing Knife and use skill `Path Of Shadow` to disguise as an Assassin! The skills will cause fatal damage to the opponent!\n")
                           .AddText("Tip: Use Mortal Wound under Stigma state, to inflict high damage!")
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.PathOfShadow) ? "(Learnt)" : "[Lv.40]:") + "~Path~Of~Shadow~(Disguise)", 20)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BladeFlurry) ? "(Learnt)" : "[Lv.40]:") + "~Blade~Flurry~(XP)", 21)
                        .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.MortalWound) ? "(Learnt)" : "[Lv.40]:") + "~Mortal~Wound~(Offensive)", 22)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.KineticSpark) ? "(Learnt)" : "[Lv.50]:") + "~Kinetic~Spark~(Passive)", 23)
                       .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BlisteringWave) ? "(Learnt)" : "[Lv.70]:") + "~Blistering~Wave~(Mass)", 24)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpiritFocus) ? "(Learnt)" : "[Lv.90]:") + "~Spirit~Focus~(Boost)", 25)
                      .AddOption("" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DaggerStorm) ? "(Learnt)" : "[Lv.100]:") + "~Dagger~Storm~(Mass)", 26)
                      .AddOption("Leave.", 255)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        data.AddText("STR_ID_tArcherEpicTask1[400][Text111]@@")
                           .AddText("STR_ID_tArcherEpicTask1[400][Text211]@@")
                            .AddText("STR_ID_tArcherEpicTask1[400][Text212]@@")
                             .AddText("STR_ID_tArcherEpicTask1[400][Text213]@@")
                              .AddText("STR_ID_tArcherEpicTask1[400][Text214]@@")
                               .AddText("STR_ID_tArcherEpicTask1[400][Text215]@@")
                      .AddOption("~Nebulous~Hunt.[Lv.15]~", 50)
                       .AddOption("Starburst~Arrows.~[Lv.40]~", 51)
                        .AddOption("~Crescent~Shadow.~[Lv.70]~", 52)

                      .AddOption("Leave.", 255)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 50:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem) || client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {

                            if (Database.ItemType.IsArcherEpicWeapon(GameItem.ITEM_ID))
                            {
                                if (client.Player.Level < 15)
                                {
                                    data.AddText("STR_ID_tArcherEpicTask1[400][Text611]@@.")
                                                          .AddOption("Okay.", 255)
                                                          .AddAvatar(10).FinalizeDialog();
                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.NebulousHunt))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.NebulousHunt, 1, 0, 0, 0);
                                }
                                else
                                {
                                    var SpellID = Role.Flags.SpellID.NebulousHunt;
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                                    break;
                                }
                            }
                            else
                            {
                                data.AddText("STR_ID_tArcherEpicTask1[400][Text411]@@")
                                                  .AddOption("Okay.", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.AddText("STR_ID_tArcherEpicTask1[400][Text411]@@")
                                              .AddOption("Okay.", 255)
                                              .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        break;
                    }
                case 51:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem) || client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {

                            if (Database.ItemType.IsArcherEpicWeapon(GameItem.ITEM_ID))
                            {
                                if (client.Player.Level < 40)
                                {
                                    data.AddText("STR_ID_tArcherEpicTask1[400][Text611]@@.")
                                                          .AddOption("Okay.", 255)
                                                          .AddAvatar(10).FinalizeDialog();
                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StarburstArrows))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.StarburstArrows, 1, 0, 0, 0);
                                }
                                else
                                {
                                    var SpellID = Role.Flags.SpellID.StarburstArrows;
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                                    break;
                                }
                            }
                            else
                            {
                                data.AddText("STR_ID_tArcherEpicTask1[400][Text411]@@")
                                                  .AddOption("Okay.", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.AddText("STR_ID_tArcherEpicTask1[400][Text411]@@")
                                              .AddOption("Okay.", 255)
                                              .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        break;
                    }
                case 52:
                    {
                        MsgGameItem GameItem;
                        if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out GameItem) || client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out GameItem))
                        {

                            if (Database.ItemType.IsArcherEpicWeapon(GameItem.ITEM_ID))
                            {
                                if (client.Player.Level < 70)
                                {
                                    data.AddText("STR_ID_tArcherEpicTask1[400][Text611]@@.")
                                                          .AddOption("Okay.", 255)
                                                          .AddAvatar(10).FinalizeDialog();
                                    break;
                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrescentShadow))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.CrescentShadow, 1, 0, 0, 0);
                                }
                                else
                                {
                                    var SpellID = Role.Flags.SpellID.CrescentShadow;
                                    data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                                    break;
                                }
                            }
                            else
                            {
                                data.AddText("STR_ID_tArcherEpicTask1[400][Text411]@@")
                                                  .AddOption("Okay.", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                                break;
                            }
                        }
                        else
                        {
                            data.AddText("STR_ID_tArcherEpicTask1[400][Text411]@@")
                                              .AddOption("Okay.", 255)
                                              .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        break;
                    }
                case 26:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 4004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DaggerStorm;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 25:
                    {
                        if (client.Player.Level < 90 || client.Player.Class < 4003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 90 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpiritFocus;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 24:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 4003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BlisteringWave;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 23:
                    {
                        if (client.Player.Level < 50 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 50 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.KineticSpark;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.MortalWound;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 21:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.BladeFlurry;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();



                        break;
                    }
                case 20:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.PathOfShadow;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        data.AddText("After equipping bow and arrows, Archers can fly and attack enemies in the air!")
                      .AddOption("XP~Fly~[Lv.1]", 70)
                      .AddOption("Rapid~Fire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.RapidFire) ? "(Learnt)" : " [Lv.46]") + "", 13)
                      .AddOption("Improved~Fly" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Fly) ? "(Learnt)" : " [Lv.70]") + "", 14)
                      .AddOption("Arrow~Rain" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ArrowRain) ? "(Learnt)" : " [Lv.70]") + "", 15)
                      .AddOption("Intensify" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Intensify) ? "(Learnt)" : " [Lv.71]") + "", 16)
                      .AddOption("Scatter~Fire" + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ScatterFire) ? "(Learnt)" : " [Lv.23]") + "", 17)
                      .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 70:
                    {
                        var SpellID = Role.Flags.SpellID.XpFly;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 17:
                    {
                        if (client.Player.Level < 23 || client.Player.Class < 4001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 23 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ScatterFire;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        if (client.Player.Level < 71 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 71 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Intensify;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Okay. , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ArrowRain;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 4002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.Fly;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 46 || client.Player.Class < 4001)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 46 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.RapidFire;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(10).FinalizeDialog();


                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Archer)]
        public static void Archer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class % 100 >= 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24265][Text117]@@\n")
                               .AddOption("STR_ID_tNewInaugurationTask[24265][Option117]@@\n", 255)
                           .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 110 || client.Player.Class % 100 == 5)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24265][Text115]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24265][Option115]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 100 || client.Player.Class % 100 == 4)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24265][Text114]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24265][Option114]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 70 || client.Player.Class % 100 == 3)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24265][Text113]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24265][Option113]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 40 || client.Player.Class % 100 == 2)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24265][Text112]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24265][Option112]@@\n", 1)
                            .AddAvatar(10).FinalizeDialog();
                            return;
                        }
                        if (client.Player.Level >= 15 || client.Player.Class % 100 == 1)
                        {
                            data.AddText("STR_ID_tNewInaugurationTask[24265][Text111]@@\n")
                                .AddOption("STR_ID_tNewInaugurationTask[24265][Option111]@@\n", 1)
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

        [NpcAttribute(NpcID.PromotionLeeLong)]
        public static void PromotionLeeLong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            if (!(client.Player.Class >= 8000 && client.Player.Class <= 8058))
            {
                data.AddText("Sorry, but you are not a Dragon-Warrior.")
                                                           .AddOption("Alright.", 255)
                                                           .AddAvatar(259).FinalizeDialog();
                return;
            }


            //var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BlackSmith, client.Player.Class, 6426);

            switch (Option)
            {
                case 0:
                    {
                        //if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                        // {
                        // data.AddText("Dragon-Warrior is a kung fu star who fights fast, accurately and ruthlessly, as well as a group training martial arts on the basis")
                        //.AddText("~of practicality. The Dragon Warrior masters martial arts of various schools, and also excels in wielding many kinds of weapons, such as nunchaku.")
                        //.AddOption("Learn~about~Promotion.", 110)
                        // .AddOption("I~want~to~get~promoted", 3)
                        //  .AddOption("I`ll~think~about~it.", 255)
                        // .AddAvatar(259).FinalizeDialog();
                        //  break;
                        // }
                        // if (client.Player.Class == 8000)
                        /// {

                        //data.AddText("Dragon-Warrior is a kung fu star who fights fast, accurately and ruthlessly, as well as a group training martial arts on the basis")
                        //  .AddText("~of practicality. The Dragon Warrior masters martial arts of various schools, and also excels in wielding many kinds of weapons, such as nunchaku.")
                        // .AddOption("I~want~to~get~promoted", 3)
                        //.AddOption("I`ll~think~about~it.", 255)
                        //.AddAvatar(259).FinalizeDialog();
                        //break;
                        //  }
                        //  else
                        {
                            data.AddText("Kungfu King:                                                                                                   Dragon-Warrior is a kung fu star who fights fast, accurately and ruthlessly, as well as a group training martial arts on the basis")
                             .AddText("~of practicality. The Dragon Warrior masters martial arts of various schools, and also excels in wielding many kinds of weapons, such as nunchaku.")
                                //  .AddOption("I~want~to~get~promoted", 4)
                               .AddOption("Class skills.", 5)
                               .AddOption("Pure~skill.", 30)
                               .AddOption("Sounds~great.", 255)
                               .AddAvatar(259).FinalizeDialog();
                        }

                        break;
                    }
                case 110:
                    {
                        data.AddText("Ah, you look promising. I bet `Dragon-Warrior` will be a big name in a short time. Listen, you can learn class skills and")
                                  .AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                        .AddOption("Thanks.", 111)
                        .AddAvatar(259).FinalizeDialog();
                        break;
                    }
                //case 111:
                // {
                //if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                // {
                // client.Inventory.Add(stream, 3005160);
                // client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                // }
                // data.AddText("Wow, look at you, you`ll be another hero in the world of Conquer! Here, you can learn some skills and get promoted! Do you want to learn more?")
                //.AddText("~get promoted here to start your life. Would you like to learn something about your class promotion and quests?.")
                //.AddOption("Job~Promotion~2.", 3)
                //.AddAvatar(259).FinalizeDialog();

                // break;
                // }
                case 4:
                    {
                        if (client.Player.Level >= 15 && client.Player.Level < 40 && client.Player.Class == 8001)
                        {
                            data.AddText("`Dragon-Warrior` can get promoted to be `Expert Dragon-Warrior` at Level 40.")
                                .AddText("You should have reached Level 40 to get promoted. Keep practicing!.")
                      .AddOption("I`ll~think~about~it.", 255)
                      .AddAvatar(259).FinalizeDialog();
                        }


                        switch (client.Player.Class)
                        {
                            case 8001:
                                {
                                    if (client.Player.Level < 40)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 40+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 8002:
                                {
                                    if (client.Player.Level < 70)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 70+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(1080001, 1) && !client.Inventory.Contain(1080001, 1, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Emerald at your inventory , to get one go and kill some HillMonsters ( Desert City ) to get one .")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(1080001, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 8003:
                                {
                                    if (client.Player.Level < 100)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 100+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.Meteor, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 Meteor at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            case 8004:
                                {
                                    if (client.Player.Level < 110)
                                    {
                                        data.AddText("There is much that you must learn before you can accept my training. Slaughter a legion of monsters first! [Level 110+]")
                                                                         .AddOption("I~see.", 255)
                                                                         .AddAvatar(259).FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (!client.Inventory.Contain(Database.ItemType.MoonBox, 1))
                                        {
                                            data.AddText("I'm sorry but you need to have 1 MoonBox at your inventory.")
                                                                     .AddOption("I~see.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                                            break;
                                        }
                                        //if (Database.DataCore.AtributeStatus.CheckStatus(client.Player))
                                        {
                                            client.Inventory.Add(stream, Database.ItemType.DragonBall, 1);
                                            client.SendSysMesage("You received 1 DragonBall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

                                            client.Inventory.Remove(Database.ItemType.MoonBox, 1, stream);
                                            client.Player.Class++;
                                            data.AddText("Excellent! You`ve learned ways of the Dragon-Warrior. Do you feel a different energy in your body? That`s stronger power and battle prowess!")
                                                                              .AddOption("Thank~you,~Master.", 255)
                                                                              .AddAvatar(259).FinalizeDialog();
                                        }
                                    }

                                    break;
                                }
                            default:
                                {
                                    data.AddText("You cannot be promoted anymore. You have mastered your class")
                                                                                                         .AddOption("Thank~you,~Master.", 255)
                                                                                                         .AddAvatar(259).FinalizeDialog();

                                    break;
                                }
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("The pure skill, [Dragon Fury], is exclusive for Dragon-Warrior who were also Dragon-Warrior in previous two lives.")
                            .AddText("~If you`ve been 2nd-reborn but not a pure Dragon-Warrior, go find the Rebirth Master for reincarnation to get a new class combination.")
                         .AddOption("Learn~[Dragon~Fury].", 31)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (client.Player.Reborn >= 2 && Database.AtributesStatus.IsLee(client.Player.FirstClass) && Database.AtributesStatus.IsLee(client.Player.SecoundeClass))
                        {
                            var SpellID = Role.Flags.SpellID.DragonFury;
                            if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                            {
                                data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                                break;
                            }
                            client.MySpells.Add(stream, (ushort)SpellID);
                            data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                     .AddOption("Ok , thank you.", 255)
                                                                     .AddAvatar(259).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("To learn [Dragon Fury], you should be Dragon-Warrior for three lives in a row and reach at least Level 70.")
                    .AddOption("OK", 255)
                    .AddAvatar(259).FinalizeDialog();
                        }


                        break;
                    }

                case 5:
                    {
                        data.AddText("Dragon-Warrior enjoys unique and brilliant combo skills. Typically, the striking [Dragon Kicks], destructive [Dragon Swipe]")
                            .AddText("~and overwhelming [Dragon Strides] enable the caster to get close to enemies at a lightning speed")
                            .AddText("~and kill the target in a second")
                          .AddOption("Dragon~Punch. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch) ? "(Learnt)" : "(Lv.15)") + "", 6)
                          .AddOption("Dragon~Cyclone. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone) ? "(Learnt)" : "(Lv.15)") + "", 7)
                          .AddOption("AirKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick) ? "(Learnt)" : "(Lv.15)") + "", 8)
                          .AddOption("AirRaid. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid) ? "(Learnt)" : "(Lv.15)") + "", 9)
                          .AddOption("AirSweep. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep) ? "(Learnt)" : "(Lv.15)") + "", 10)
                          .AddOption("Dragon~Flow. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow) ? "(Learnt)" : "(Lv.15)") + "", 11)
                          .AddOption("Cracking~Swipe. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.CrackingSwipe) ? "(Learnt)" : "(Lv.70)") + "", 12)
                          .AddOption("More.", 15)
                          .AddOption("I`ll talk to you later.", 255)
                          .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 15:
                    {
                        data.AddText("Dragon-Warrior enjoys unique and brilliant combo skills. Typically, the striking [Dragon Kicks], destructive [Dragon Swipe]")
                            .AddText("~and overwhelming [Dragon Strides] enable the caster to get close to enemies at a lightning speed")
                            .AddText("~and kill the target in a second")
                        .AddOption("Splitting~Swipe. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SplittingSwipe) ? "(Learnt)" : "(Lv.100)") + "", 13)
                        .AddOption("Dragon~Slash. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSlash) ? "(Learnt)" : "(Lv.100)") + "", 14)
                        .AddOption("Dragon~Roar. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonRoar) ? "(Learnt)" : "(Lv.70)") + "", 16)
                        .AddOption("Dragon~Swing. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonSwing) ? "(Learnt)" : "(Lv.70)") + "", 17)
                        .AddOption("SpeedKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SpeedKick) ? "(Learnt)" : "(Lv.40)") + "", 18)
                        .AddOption("ViolentKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ViolentKick) ? "(Learnt)" : "(Lv.40)") + "", 19)

                        .AddOption("Previous.", 5).AddOption("More.", 120)
                        .AddOption("I`ll talk to you later.", 255)
                          .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 120:
                    {
                        data.AddText("Dragon-Warrior enjoys unique and brilliant combo skills. Typically, the striking [Dragon Kicks], destructive [Dragon Swipe]")
                            .AddText("~and overwhelming [Dragon Strides] enable the caster to get close to enemies at a lightning speed")
                            .AddText("~and kill the target in a second")
                        .AddOption("AirStrike. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirStrike) ? "(Learnt)" : "(Lv.1)") + "", 121)
                        .AddOption("EarthSweep. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.EarthSweep) ? "(Learnt)" : "(Lv.1)") + "", 122)
                        .AddOption("Kick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.Kick) ? "(Learnt)" : "(Lv.1)") + "", 123)
                        .AddOption("StormKick. " + (client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.StormKick) ? "(Learnt)" : "(Lv.40)") + "", 20)
                        .AddOption("Previous.", 15).AddOption("I`ll talk to you later.", 255)
                          .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 121:
                    {
                        var SpellID = Role.Flags.SpellID.AirStrike;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 122:
                    {
                        var SpellID = Role.Flags.SpellID.EarthSweep;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;

                    }
                case 123:
                    {

                        var SpellID = Role.Flags.SpellID.Kick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;

                    }
                case 20:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 8002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.StormKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 19:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 8002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.ViolentKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 18:
                    {
                        if (client.Player.Level < 40 || client.Player.Class < 8002)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 40 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SpeedKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 17:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 8003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonSwing;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 16:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 8003)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonRoar;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 14:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 8004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.DragonSlash;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 13:
                    {
                        if (client.Player.Level < 100 || client.Player.Class < 8004)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 100 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.SplittingSwipe;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        if (client.Player.Level < 70 || client.Player.Class < 8000)
                        {
                            data.AddText("Sorry. I can`t teach you this skill until you reach level 70 and get promoted.")
                                                  .AddOption("Okay.~I~see..", 255)
                                                  .AddAvatar(259).FinalizeDialog();
                            break;
                        }
                        var SpellID = Role.Flags.SpellID.CrackingSwipe;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        var SpellID = Role.Flags.SpellID.DragonFlow;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        var SpellID = Role.Flags.SpellID.AirSweep;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();


                        break;
                    }
                case 9:
                    {
                        var SpellID = Role.Flags.SpellID.AirRaid;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        var SpellID = Role.Flags.SpellID.AirKick;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        var SpellID = Role.Flags.SpellID.DragonCyclone;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        var SpellID = Role.Flags.SpellID.DragonPunch;
                        if (client.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
                        {
                            data.AddText("You`ve learned " + SpellID.ToString() + "!").AddOption("Okay.", 255).AddAvatar(5).FinalizeDialog();
                            break;
                        }
                        client.MySpells.Add(stream, (ushort)SpellID);
                        data.AddText("Congratulation! You just have learned the skill " + SpellID.ToString() + "")
                                                                 .AddOption("Ok , thank you.", 255)
                                                                 .AddAvatar(259).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.Level >= 15 && client.Player.Class == 8000)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonPunch))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonPunch);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonCyclone))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonCyclone);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.DragonFlow))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.DragonFlow);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirRaid))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirRaid);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirSweep))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirSweep);
                            }
                            if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.AirKick))
                            {
                                client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.AirKick);
                            }
                            client.Player.Class++;
                            data.AddText("You`ve been promoted!")
                            .AddOption("Great,~thanks!", 255)
                            .AddAvatar(259).FinalizeDialog();//7991 384 251
                            //  if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.SendAutoPatcher(1002, 384, 251, 7991);
                        }
                        else
                        {
                            data.AddText("Sorry, you need level 15.")
                      .AddOption("Ah ok.", 255)
                      .AddAvatar(259).FinalizeDialog();
                        }


                        break;
                    }

            }
        }
        #endregion
    }
}

