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
        [NpcAttribute(NpcID.NameRegister)]
        public static void NameRegister(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Good day! If you want to turn a new page in your life, you can pay me 810 CPs to get a new name. Your new name will be registered, immediately.")
                   .AddOption("Okay,~here`s~my~CPs!", 1)
                   .AddOption("Reset edit times.", 10)
                   .AddOption("I`ll~think~about~it.", 255)
                   .AddAvatar(31).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to reset your name for 1,000 CPs?")
                        .AddOption("Yes.", 11)
                       .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(31).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (client.Player.ConquerPoints >= 1000)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 1000;
                            client.Player.NameEditCount = 0;
                            data.AddText("Your name reset was finished succesfully!")
                       .AddOption("thank you.", 255)
                      .AddAvatar(31).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                       .AddOption("Okay.", 255)
                      .AddAvatar(31).FinalizeDialog();
                        }
                        break;
                    }

                case 1:
                    {
                        if (client.Player.ConquerPoints >= 810)
                        {
                            ActionQuery action = new ActionQuery()
                            {
                                Type = ActionType.OpenDialog,
                                ObjId = client.Player.UID,
                                dwParam = MsgServer.DialogCommands.ChangeName,
                                PositionX = client.Player.X,
                                PositionY = client.Player.Y,

                            };
                            client.Send(stream.ActionCreate(action));
                            client.Send(stream.NameChangeCreate(MsgNameChange.ActionID.DialogInfo, client.Player.NameEditCount, 5, client.Player.Name));
                        }
                        else
                        {
                            data.AddText("At your service! Oh, no... I`m sorry. You`d better check your bag, again. You don`t have enough CPs to rename yourself. It takes 810 CPs, and I`m not allowed to offer any discounts.")
                      .AddOption("Rules~are~rules.~I`ll~wait.", 255)
                      .AddAvatar(31).FinalizeDialog();
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.Revenant1)]
        public static void Revenant1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();



                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant2)]
        public static void Revenant2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant3)]
        public static void Revenant3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant4)]
        public static void Revenant4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Revenant5)]
        public static void Revenant5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 0, 0, 1) == false)
                            {
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                                data.AddText("Thank you very much! I can rest in peace now!")
                           .AddOption("Close~your~eyes,~soldier!", 255)
                           .AddAvatar(30).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("Charge! I will kill you all!")
                           .AddOption("Oh~MY!", 255)
                           .AddAvatar(30).FinalizeDialog();


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CrystalRiddler)]
        public static void CrystalRiddler(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DisCityMain, client.Player.Class, 1313);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1) == false)
                            {
                                client.Inventory.Remove(711354, 1, stream);
                                client.Inventory.Add(stream, 711355);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 1);
                                client.SendSysMesage("The Divine Calix was filled with the fountain water.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.StoneApe)]
        public static void StoneApe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneApe, client.Player.Class, 1312);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DisCityMain, client.Player.Class, 1313);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StoneApe, client.Player.Class, 1314);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I come from the stone, and so I am the Stone Ape. The other apes regard me as a freak, but I`m not! Please, go and ask the Solar Saint what I am!")
                               .AddText("~I must know what I truly am, or I fear I shall go bananas!");
                            data.AddOption("Okay!", 1);
                            data.AddOption("Sorry,~not~today!", 255);
                            data.AddAvatar(53).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I was born from the rocks used to restore cracks in the sky? I can`t believe my ears! And what is this? A Divine Calix? Solar Saint told you that I can")
                              .AddText("~become an immortal after I drink the fountain water in the calix? That`s great news! Thank you very much for your help!");
                            data.AddOption("You~are~welcome!", 2);
                            data.AddOption("See~you~then!", 255);
                            data.AddAvatar(53).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 3))
                            {
                                data.AddText("Thank you! I feel much more powerful now! I will work hard to become a worthy immortal!")
                              .AddOption("You~will!", 10)
                              .AddOption("See~you~then!", 255)
                              .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Where is the Divine Calix? I want to drink the water inside!")
                                 .AddOption("I~will~grab~it~back~for~you!", 255)
                                 .AddAvatar(53).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Go and ask the Solar Saint what I am!")
                           .AddOption("Okay!", 255)
                           .AddAvatar(53).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        data.AddText("Go and ask the Solar Saint what I am!")
                                 .AddOption("Okay!", 255)
                                 .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(711355, 1, stream);
                            client.SendSysMesage("You received 485,000 points of EXP, 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 485000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 728,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 728000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, Database.ItemType.DoubleExp);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.JackDaniel)]
        public static void JackDaniel(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1315);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JackDaniel, client.Player.Class, 1316);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("No problem! I will do everything I can to help Carpenter Jack fix the wooden bridge! Why not take a short break? Or better yet, I have something else for you to do!")
                          .AddOption("As~you~wish!", 1)
                          .AddOption("Sorry,~not~today!", 255)
                          .AddAvatar(53).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            goto case 2;
                        }
                        else
                        {
                            data.AddText("Please don`t stand and in the way! The transport column will pass through here!")
                       .AddOption("Sorry!", 255)
                       .AddAvatar(53).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 495,000 points of EXP, 1000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 495000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        goto case 2;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Go slay 50 Giant Ape and protect the wood transport column.")
                         .AddOption("Okay!", 255)
                         .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("I can prepare the wood needed by the carpenter, but I`m worried about the Giant Apes. Slay 50 Giant Apes for me in the Love Canyon, in case they attack the wood transports.")
                          .AddOption("No~problem!", 3)
                          .AddOption("Sorry,~not~today!", 255)
                          .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CliffFlower1)]
        public static void CliffFlower1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1317);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.ActivePick)
                        {
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                            {
                                client.Player.AddPick(stream, "Collect~Cliff~Flower", 5);
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CliffFlower2)]
        public static void CliffFlower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1317);
            switch (Option)
            {
                case 0:
                    {
                        if (!client.Player.ActivePick)
                        {
                            if (client.Player.QuestGUI.IsActiveQuest(ActiveQuest.MissionId))
                            {
                                client.Player.AddPick(stream, "Collect~Cliff~Flower", 5);
                            }
                        }

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.KeYulunsFollower)]
        public static void KeYulunsFollower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1463);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            {
                                data.AddText("The Sand Monsters L68 are too strong for me. Please go kill them and protect me!")
                    .AddOption("I`m~on~my~way.", 255)
                     .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Help, help! It`s so good to see you here. The Sand Monsters L68 blocked my way home. Please, I don`t want to die here. Could you kill them for me?")
                    .AddOption("No~problem!", 1)
                    .AddOption("Monsters?~It`s~terrible.", 255)
                     .AddAvatar(53).FinalizeDialog();
                            }

                            break;
                        }

                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.DCViceGenera)]
        public static void DCViceGenera(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1452);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1453);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1454);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1455);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("There are many dangerous creatures in the desert. Once you step through here, I won`t be able to protect you anymore. Take care.");



                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Danger~1.", 1);

                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Danger~2.", 4);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hidden~Danger~3.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Hidden~Danger~4.", 20);
                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(1, 1, 1, 1, 1))
                        {
                            data.AddText("The Sandbag is open! We did it!")
                                .AddOption("Yeah,~yeah!", 21)
                                 .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("You`re going to collect some water from the source in the city and spread the water over the city gates.")
                              .AddText("~After that, the Sand Monsters will be unable to go through the gates or enter the city.")
                                .AddOption("I~see.", 255)
                                 .AddAvatar(51).FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                  .AddOption("Let me check.", 255)
                                   .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(720939, 1, stream);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.Inventory.Add(stream, 720173);
                            client.CreateBoxDialog("ou've~received~some~EXP,~3,000~Silver,~a~Small~Joy~Stone~and~a~+1~Stone~(B)!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.QuestGUI.SendAutoPatcher(1000, 559, 795, (ushort)NpcID.KeYulun);
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1, 1))
                            {

                                data.AddText("A lot of yellow sand? That`s weird. This information is important. Thanks for you help!")
                                    .AddOption("Okay.", 3)
                                    .AddAvatar(51).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you killed the Sand Monsters, yet? Go patrol outside the city gates. You`ll find plenty of them roaming around.")
                                    .AddOption("Okay.", 255)
                                    .AddAvatar(51).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("Have you killed the Sand Monsters, yet? Go patrol outside the city gates. You`ll find plenty of them roaming around.")

                             .AddOption("I`ll~go~right~away.", 2)
                         .AddOption("Later.", 255)
                         .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Head~out~and~patrol~around~the~city~gates.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~1,500~Silver!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        goto case 4;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711338, 1))
                            {
                                data.AddText("The Sand Monsters want to use the Sandbags to attack the city? Thanks for the information. Here`s your reward!")
                   .AddOption("Thanks.", 6)
                   .AddAvatar(51).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Sand Monsters must have something to do with the weird yellow sand. I need you to turn to the Sand Monsters for some clues.")
                      .AddOption("Alright.", 255)
                      .AddAvatar(51).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Sand Monsters must have something to do with the weird yellow sand. I need you to return to the Sand Monsters to look for clues.")
                                   .AddOption("I~won`t~let~you~down.", 5)
                                   .AddOption("Later.", 255)
                                   .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 6:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711338, 1, stream);
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

                            client.Inventory.Add(stream, ItemID);

                            client.CreateBoxDialog("You~received~some~EXP,~3,000~Silver~and~1~Refined~Gem!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        goto case 10;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("These Sandbags can`t be opened somehow. I`ve tried many ways, and even the a blade doesn`t work. Can you deliver it to Taoist Yu? Maybe he can figure that out.")

                                .AddOption("I`m~on~my~way.", 255)
                                .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        data.AddText("These Sandbags can`t be opened, no matter what I do. I`ve tried many ways, and even a blade won`t work. Can you take it to Taoist Yu? Maybe he can figure it out.")
                                   .AddOption("You~can~count~on~me.", 11)
                                   .AddOption("Later.", 255)
                                   .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~deliver~the~Sandbag~to~Taoist~Yu.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.SnakemanLeader)]
        public static void SnakemanLeader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1332);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SnakemanLeader, client.Player.Class, 1359);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Why you are here? Speak!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            data.AddOption("Quest:~Snakeman~Leader(2).", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Race~Hatred(1).", 10);

                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Good job! Take your reward!")
                          .AddOption("Thanks!", 12)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(10).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kill 50 Heresy Snakemen for me! They are my enemies!")
                            .AddOption("Okay!", 255)
                            .AddAvatar(10).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("The Heresy Snakemen are enemies to my race! I want you to kill 50 Heresy Snakemen for me!")
                                   .AddOption("Okay!", 11)
                                   .AddOption("Sorry,~not~today!", 255)
                                   .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh? A Catholicon? Give it to me!")
                                .AddOption("Eat~it!", 2)
                                .AddOption("See~you~then!", 255)
                                .AddAvatar(10).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 1,205,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1205000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                        client.SendSysMesage("The Snakeman Leader ate the Catlholicon and become very sock! Go report back to Moon Maiden!", MsgMessage.ChatMode.System);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CaptorCooke)]
        public static void CaptorCooke(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1344);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1336);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1339);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1340);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1342);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1345);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1346);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1347);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1450);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("No criminal can escape from my hands! Here`s a letter for the DC Vice Captain. I need you to head to the Desert and deliver this letter to him.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Back~Home(1).", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Back~Home(2).", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Back~Home(5).", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Pick~a~Fight(1).", 30);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Pick~a~Fight(3).", 40);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Strife(1).", 50);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Strife(2).", 60);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Strife(3).", 70);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("You~can~count~on~me.", 90);

                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 90:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~deliver~this~letter~to~the~DC~Vice~Captain!", ActiveQuest9.FinishNpcId.Map, ActiveQuest9.FinishNpcId.X, ActiveQuest9.FinishNpcId.Y, ActiveQuest9.FinishNpcId.ID);

                        break;
                    }
                case 70:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest8.MissionId, 1, 1))
                        {
                            data.AddText("Thank you for all you have done. Your name will be remembered by us!")
                 .AddOption("You~are~welcome.", 72)
                 .AddOption("See~you~then!", 255)
                 .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Listen, this maybe a little complicated. Take these two Corpse Bags and these two Smell Bags of the two snake races. Use them in the appointed place to entice the elders of the 2 races from their lair and kill them. After the elders are killed!")
                 .AddText(" ~The 2 races will be furious at each other! And we can just sit down to watch them fight.")
                            .AddOption("No~problem!", 71)
                      .AddOption("Sorry,~not~today", 255)
                      .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 72:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Inventory.Remove(720861, 1, stream);
                            client.Inventory.Remove(720862, 1, stream);
                            if (client.Player.Class > 10000)
                                client.Inventory.Add(stream, 121128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            else
                                client.Inventory.Add(stream, 120128, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.SendSysMesage("You received 1,863,000 EXP, an Elite Necklace and 3,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1863000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 71:
                    {

                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720861, 1))
                            client.Inventory.Add(stream, 720861);
                        if (!client.Inventory.Contain(720862, 1))
                            client.Inventory.Add(stream, 720862);
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        client.SendSysMesage("You can used the Corpse Bag(Heresy Bnake) and the Small Bag(Heresy Snake). You enticed the Heresy Snakeman Eider out from its fair", MsgMessage.ChatMode.System);

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 40, 40))
                        {
                            data.AddText("My scout reported that the two snake races are at war! Thank you for your help! Now we can deliver the final blow!")
               .AddOption("Tell~me~what!", 62)
               .AddOption("See~you~then!", 255)
               .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("Now, carry these 2 Smell Bags and slay 40 Snakemen and 40 Heresy Snakemen. The smell of the other race will be left on the dead body of the Snakemen and Heresy Snakemen. Let`s make their hatred greater!")
                        .AddOption("No~problem!", 61)
                        .AddOption("Sorry,~not~today", 255)
                        .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 62:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You received 1,739,000 EXP, and 2,700 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1739000, questReward: true);
                            client.Player.Money += 2700;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
                case 61:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 20, 20))
                            {
                                data.AddText("It is great to have you help me! Now, listen carefully to my plan!")
                     .AddOption("Okay!", 52)
                     .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 20 Snakemen and 20 Heresy Snakemen and use these 2 Smell bags to collect their smell.")
                       .AddOption("See~you~then!", 255)
                       .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Snakeman and the Heresy Snakeman are two major snake races in Ape City. I have a secret plan to stir up a fight between them! Take these 2 Smell Bags, and go slay 20 Snakemen and 20 Heresy Snakemen to collect their smell.")
                        .AddOption("No~problem!", 51)
                        .AddOption("Sorry,~not~today", 255)
                        .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 52:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.SendSysMesage("You received 1,553,000 EXP, 2,500 Silver, and 5 Panaceas.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1553000, questReward: true);
                            client.Player.Money += 2500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 51:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        data.AddText("Good job! This plan will not succeed without your help! Here are your rewards!")
                           .AddOption("Thank~you~very~much!", 41)
                           .AddOption("See~you~then!", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You received 1,835,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1835000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 30:
                    {
                        data.AddText("The Snakeman and the Heresy Snakeman are the two major creatures, here. They usually stir up trouble for us, but I have a plan to pick up a fight between them. Go speak with Harmony Taoist to learn more about the plan!")
                           .AddOption("No~problem!", 31)
                           .AddOption("Sorry,~not~today", 255)
                           .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        break;
                    }
                case 20:
                    {
                        data.AddText("Wonderful! With the help of the Cloud Riding, I can bring the criminal back to Ape City and have him punished by law safely! Here are your rewards!")
                          .AddOption("Thank~you!", 21)
                          .AddOption("Sorry,~not~today", 255)
                          .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                      .AddOption("Let me check.", 255)
                      .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Add(stream, 160118);
                            client.SendSysMesage("You received 475,000 EXP, a pair of Elite Shoes and 1500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 475000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("I heard that there is a famous Harmony Taoist here. He must know a shortcut to the Ape City. Go ask him for me! You know, I need to stay here watch this criminal.")
                         .AddOption("No~problem!", 11)
                         .AddOption("Sorry,~not~today", 255)
                         .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                            {
                                data.AddText("It is all cliff around here. I need to return to the Ape City as soon as possible! What can I do?")
                         .AddOption("You~have~my~help!", 3)
                         .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Can you find a shortcut for me? I need to stay here watch this criminal.")
                            .AddOption("Okay!", 255)
                            .AddAvatar(7).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I just captured a wanted criminal! But I am afraid that his friends will come to rescue him. I need to return to Ape City as fast as possible! Can you find a shortcut for me? I need to stay here and watch this ruffian!")
                             .AddOption("No~problem!", 2)
                             .AddOption("Sorry,~not~today", 255)
                             .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 890,000 EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 890000);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HeresySnakemanLeader)]
        public static void HeresySnakemanLeader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1342);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeresySnakemanLeader, client.Player.Class, 1360);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You are not a normal person! I can tell it from your face!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Pick~a~Fight(3).", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Race~Hatread(2).", 10);
                        data.AddOption("Really?", 255);

                        data.AddAvatar(10).FinalizeDialog();


                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 60))
                        {
                            data.AddText("Thank you very much for slaying my enemies! Here are your rewards!")
         .AddOption("Thanks!", 12)
      .AddOption("See~you~then!", 255)
           .AddAvatar(10).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Snakemen are my enemy! I will wipe them all out! Slay 60 Snakemen for me!")
               .AddOption("No~problem!", 11)
            .AddOption("Sorry,~I~am~not~interested.", 255)
                 .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 1,205,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1205000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher(1020, 211, 445, 8298);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HarmonyTaoist)]
        public static void HarmonyTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1335);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1343);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1336);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1337);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1338);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1339);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1340);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1341);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HarmonyTaoist, client.Player.Class, 1342);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You are not a normal person! I can tell it from your face!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Back~Home(2).", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Back~Home(3).", 30);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Back~Home(4).", 40);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Back~Home(5).", 60);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Pick~a~Fight(1).", 70);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {

                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Pick~a~Fight(2).", 80);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Quest:~Pick~a~Fight(3).", 90);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Eradicate~Snakeman.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snakeman~L63.", 10);
                        data.AddOption("Continue Quests.", 50).AddOption("See~you~then!", 255);

                        data.AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 90:
                    {
                        data.AddText("I have cast my magic on the Holy Bead and the Heresy Snake Gall! Use the Holy Bead to transform into a Snakeman and challenge the Heresy Leader. Use the Heresy Snake Gall to transform into a Heresy Snakeman to challenge the Snakeman Leader.")
                                 .AddText("~By doing this, we can arose their hatred against each other and see both sides jump on each other without getting involved in it!")
                     .AddOption("No~problem!", 91)
                  .AddOption("Sorry,~I~am~not~interested.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 91:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 441, 451, 30105);
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        break;
                    }
                case 80:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720857, 1)
                                && client.Inventory.Contain(720858, 1))
                            {
                                data.AddText("Good. I will cast my magic spell on the Holy Bead and the Heresy Snake Gall! Then you can use it to transform into a Snakeman or a Heresy Snakeman to continue our plan.")
                      .AddOption("Great!", 82)
                      .AddOption("See~you~then!", 255)
                           .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Get a Holy Bead from the Snakeman and a Heresy Snake Gall from the Heresy Snakeman. We need them to pick a fight between the two snake races.")
                      .AddOption("Okay!", 255)
                           .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Listen! We have a plan to pick a fight between the two snake races. Get a Holy Bead from the Snakeman and a Heresy Snake Gall from the Heresy Snakeman. These two items are the key to carry out the plan!")
                     .AddOption("No~problem!", 81)
                  .AddOption("Sorry,~I~am~not~interested.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 82:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.SendSysMesage("You received 1,205,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1205000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 81:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);
                        break;
                    }
                case 70:
                    {
                        data.AddText("I have discussed this with Captor Cooke. This will be a big plan!")
                         .AddOption("Tell~me~what!", 71)
                      .AddOption("Sorry,~I~am~not~interested.", 255)
                           .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 71:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.SendSysMesage("You received 301,000 EXP, and 500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 301000, questReward: true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Teach Captor Cooke Cloud Riding! We have to help him send the criminals back to Ape City to be punished by the courts!")
                        .AddOption("Okay!", 255)
                             .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Since you have learned the secret of Cloud Riding, you should go teach Captor Cooke how to ride the clouds. We need to help him send the criminals back to Ape City to be punished by the courts!")
                           .AddOption("No~problem!", 61)
                        .AddOption("See~you~then!", 255)
                             .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 61:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                            {
                                data.AddText("You are not a normal person! I knew it at the time when I saw you! See, you mastered the Cloud Riding!")
                  .AddOption("Thank~you.", 42)
                  .AddOption("See~you~then!", 255)
                       .AddAvatar(67).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Go to the cliff closest to Ape City, and chant `Kuh Limb Mah, Kuh Limb Mah` to use the skill! Don`t forget to put away your weapons, first!")
                      .AddOption("Okay.", 255)
                           .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("According to the Scroll, you need a pure mind to fly over the cliff. Now, put down your weapon. Go to the cliff closest to Ape City, and chant `Kuh Limb Mah, Kuh Limb Mah` to use the skill!")
                          .AddOption("No~problem!", 41)
                       .AddOption("See~you~then!", 255)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                             .AddOption("Let me check.", 255)
                             .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.SendSysMesage("You received 1,187,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1870000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 41:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(720856, 1))
                        {
                            data.AddText("That is it! The Cloud Riding Scroll. It`s written in the Snakemen`s language... Give it to me. I will translate it to you!")
                 .AddOption("Okay!", 32)
                 .AddOption("See~you~then!", 255)
                 .AddAvatar(67).FinalizeDialog();
                            break;
                        }


                        data.AddText("Snakeman has a secret skill of Cloud Riding. As the name suggested, you can raid the cloud flying over the cliffs. Get a Cloud Riding Scroll from the Snakeman and you will know how to fly!")
                       .AddOption("No~problem!", 31)
                       .AddOption("See~you~then!", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 32:
                    {
                        client.Inventory.Remove(720856, 1, stream);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.SendSysMesage("You received 1,290,000 EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1290000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("A shortcut to Ape City? There is one. Fly over the cliff, and there you go! But you`ll need the Cloud Riding Skill to help you!")
                         .AddOption("What~is~it?", 22)
                         .AddOption("See~you~then!", 255)
                         .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 890,000 EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 890000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 50:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 211, 445, 8298);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("I can`t believe it! You made it! Here are your rewards!")
                          .AddOption("Thank~you!", 12)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 50 Snakemen L63 for me! Be careful! They are very aggressive!")
                              .AddOption("Okay!", 255)
                              .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Snakeman L63 is the most aggressive snake race! I want you to slay 50 Snakemen L63 for me!")
                                 .AddOption("No~problem!", 11)
                                 .AddOption("Sorry,~not~today!", 255)
                                 .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 1,223,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1223000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 60))
                            {
                                data.AddText("Thank you very much for slaying the Snakeman for me. Here are your rewards!")
                          .AddOption("Thank~you!", 3)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 60 Snakeman to return peace to Ape City!")
                              .AddOption("Okay!", 255)
                              .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Snakeman is a bane on Ape City! I want you to slay 60 Snakeman and return peace to Ape City!")
                               .AddOption("No~problem!", 2)
                               .AddOption("Sorry,~not~today!", 255)
                               .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 1,170,000 EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1170000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MoonMaiden)]
        public static void MoonMaiden(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1331);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1332);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1333);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MoonMaiden, client.Player.Class, 1334);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Poisonous snakes are dangerous to people!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Snakeman~Leader(1).", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snakeman~Leader(2).", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snake~Gall.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snake~Fang.", 30);

                        data.AddOption("Continue Quests.", 50).AddOption("See~you~then!", 255);

                        data.AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 50:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 265, 429, 8297);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720855, 5))
                            {
                                data.AddText("Thank you very much for punishing the Snakeman. Here are your rewards!")
                                   .AddOption("Thank~you!", 32)
                                   .AddOption("See~you~then!", 255)
                                   .AddAvatar(175).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go slay the Snakeman and collect 5 Snake Fangs for me! This is will be a warning!")
                                    .AddOption("Okay!", 255)
                                    .AddAvatar(175).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Many villagers were wounded by the Snakemen. They deserve to be punished! Go slay the Snakemen and collect 5 Snake Fangs for me! This is will be a warning to them!")
                    .AddOption("No~problem!", 31)
                    .AddOption("Sorry,~not~today!", 255)
                    .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(720855, 5, stream);
                            client.Inventory.AddItemWitchStack(723340, 0, 1, stream);
                            client.SendSysMesage("You received 1,223,000 points of EXP, 2 Diligence Book, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1223000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

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
                            if (client.Inventory.Contain(720854, 10))
                            {
                                data.AddText("Thank you very much for the Snake Gall. Here are your rewards!")
                             .AddOption("You~are~welcome.", 22)
                             .AddOption("See~you~then!", 255)
                             .AddAvatar(175).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Snake Gall of the Snakeman is a rare medicine, but a necessary ingredient in making the drug for snakebites. Collect 10 Snake Galls for me from the Snakeman!")
                             .AddOption("Okay!", 255)
                             .AddAvatar(175).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Snake Gall of the Snakeman is a rare medicine, but a necessary ingredient in making the drug for snakebites. Collect 10 Snake Galls for me! I can use them to save lives.")
                          .AddOption("No~problem!", 21)
                          .AddOption("Sorry,~not~today!", 255)
                          .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(720854, 10, stream);
                            client.Inventory.AddItemWitchStack(720172, 0, 2, stream);
                            client.SendSysMesage("You received 1,170,000 points of EXP, 2 EXPBallScrap, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1170000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            data.AddText("Well done! The Snakeman Leader must have learnt a lesson this time!")
                             .AddOption("Thank~you.", 12)
                             .AddOption("See~you~then!", 255)
                             .AddAvatar(175).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Snakeman Leader is an enthusiast in alchemy who likes to eat the bolus produced by the alchemists to strength his power! I have some Catholicon, it some kind of drug for snakebites which is very effective in dealing with the snakes.")
                      .AddText("~Take it to the Snakeman Leader persuade him to eat it! Then the Breeze Pupil will have his revenge!")
                            .AddOption("No~problem!", 11)
                            .AddOption("Sorry,~not~today!", 255)
                            .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(175).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.SendSysMesage("You received 702000 EXP, a Meteor and 1200 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 702000, questReward: true);
                            client.Player.Money += 1200;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("You want me to punish the Snakeman Leader? that is easy!")
                                 .AddOption("Tell~me~how?", 2)
                                 .AddOption("Just~kidding!", 255)
                                 .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.SendSysMesage("You received 292,000 points of EXP, and 5,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 292000, questReward: true);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TempestTaoist)]
        public static void TempestTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1326);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TempestTaoist, client.Player.Class, 1327);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TempestTaoist, client.Player.Class, 1328);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I want to know the limits of man. That is the purpose of my practice, and my goal in life.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Medicine~Supply.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Stone.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Rage~Stone.", 20);

                        data.AddOption("Open another Quest.", 100).AddOption("See~you~then!", 255);

                        data.AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 100:
                    {

                        client.Player.QuestGUI.SendAutoPatcher(1020, 356, 315, 8295);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720850, 3))
                            {
                                data.AddText("I will do some research on these Rage Stones. Thank you very much for your help!")
                               .AddOption("You~are~welcome!", 22)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {


                                data.AddText("Collect 3 Rage Stones for me from the Thunder Ape L58.")
                                             .AddOption("Okay!", 255)
                                             .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Rage Stones of the Thunder Apes L58 can really boost their strength! I hope you can collect 3 Rage Stones for me.")
                                     .AddOption("No~problem!", 21)
                                     .AddOption("See~you~then!", 255)
                                     .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        client.Inventory.Remove(720850, 3, stream);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.SendSysMesage("You received 1,134,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1134000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711353, 3))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                                      .AddOption("Thank~you!", 12)
                                      .AddOption("See~you~then!", 255)
                                      .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {


                                data.AddText("Please collect 3 Thunder Stones for me from the Thunder Apes. I need to do a research on them!")
                                             .AddOption("Okay!", 255)
                                             .AddAvatar(35).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Thunder Apes usually carry the Thunder Stones. I guess these strange stones give them special power! Will you collect 3 Thunder Stones for me, for further research?")
                                .AddOption("No~problem!", 11)
                                           .AddOption("Sorry,~not~today!", 255)
                                           .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        client.Inventory.Remove(711353, 3, stream);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 720172);
                            client.SendSysMesage("You received 1,250,000 points of EXP, 1 EXP Ball Scrap, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1250000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Thank you very much for your medicine! We can use them to enter the lair of the apes and find out the way to deal with them.")
                             .AddOption("You~are~welcome!", 2)
                                        .AddOption("Sorry,~not~today!", 255)
                                        .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(1000030, 1) && client.Inventory.Contain(1001020, 1))
                        {
                            client.Inventory.Remove(1001020, 1, stream);
                            client.Inventory.Remove(1000030, 1, stream);
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.SendSysMesage("You received 850,000 points of EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                client.IncreaseExperience(stream, 850000, questReward: true);
                                client.Player.Money += 1500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            }
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ArdenteTaoist)]
        public static void ArdenteTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1324);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1325);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArdenteTaoist, client.Player.Class, 1326);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The goal of the Taoist practice is to achieve harmony in both the mental and physical planes.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Ape.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Ape~L58.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Medicine~Supply.", 20);
                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        data.AddText("My brother, the Tempest Taoist, and I decided to enter the lair of the Giant Apes, but we can`t afford the necessary medicine. Please, buy 2 Amrita and 2 Pearl Ointments for my brother and I. I will give you all I have in return.")
                                 .AddOption("No~problem!", 21)
                                            .AddOption("Sorry,~not~today!", 255)
                                            .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 2);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                        {
                            data.AddText("Thank you very much for wiping out the Thunder Apes L58 for me! Here are your rewards!")
                                     .AddOption("Thank~you!", 12)
                                     .AddOption("See~you~then!", 255)
                                     .AddAvatar(35).FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Slay 50 Thunder Apes L58 for me! Be quick!")
                                     .AddOption("Okay!", 255)
                                     .AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            data.AddText("Why can`t they stop bothering me! Go slay 50 Thunder Apes L58 for me! I need a quiet place for my practice!")
                               .AddOption("No~problem!", 11)
                                          .AddOption("Sorry,~not~today!", 255)
                                          .AddAvatar(35).FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                            client.SendSysMesage("You received 1,134,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 1134000, questReward: true);

                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 60))
                        {

                            data.AddText("Thank you very much for wiping out the Thunder Apes for me! Here are your rewards!")
                          .AddOption("Thank~you!", 3)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I hope you can slay 60 Thunder Apes for me. Maybe then I can continue with my studies and practice.")
                                 .AddOption("Sorry,~not~today!", 255)
                                 .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        data.AddText("The Thunder Apes are such a nuisance to me! They keep on interrupting my practice. Take this Soulfire Token and slay 60 Thunder Apes for me!")
                          .AddOption("No~problem!", 2)
                                     .AddOption("Okay!", 255)
                                     .AddAvatar(35).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.SendSysMesage("You received 1,079,000 points of EXP, 1 Small Joy Stone, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.Inventory.Add(stream, 720173);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.IncreaseExperience(stream, 1079000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BreezePupil)]
        public static void BreezePupil(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1329);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1330);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BreezePupil, client.Player.Class, 1331);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I will become a great taoist in the future!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Blaze~Pill.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Capture~the~Apes.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Snakeman~Leader(1).", 20);
                        data.AddOption("See~you~then!", 255);

                        data.AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please find the Moon Maiden for me!")
                                 .AddOption("See~you~then!", 255)
                                 .AddAvatar(186).FinalizeDialog();
                            break;
                        }
                        data.AddText("Everytime I entered the habitation of the Snakemen, I was bullied by the Snakeman Leader. I heard that the Moon Maiden is an expert in dealing with the Snakemen and making drugs for snakebite. Will you find her and ask her to help me?")
                            .AddText("~I~want~my~revenge!")
                            .AddOption("No~problem!", 21)
                                       .AddOption("Sorry,~not~today!", 255)
                                       .AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1020, 272, 438, 8296);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5, 5))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                                      .AddOption("You~are~welcome!", 12)
                                      .AddOption("See~you~then!", 255)
                                      .AddAvatar(186).FinalizeDialog();
                            }
                            else
                            {
                                if (!client.Inventory.Contain(720852, 1))
                                    client.Inventory.Add(stream, 720852);
                                data.AddText("Capture 5 Thunder Apes and 5 Thunder Apes L58 for me.")
                                      .AddOption("Okay!", 255)
                                      .AddAvatar(186).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("My master gave me a Ape Gourd. He told me that I can use it to capture some Apes. Use it to Capture 5 Thunder Apes and 5 Thunder Apes L58 for me.")
                                  .AddOption("No~problem!", 11)
                                        .AddOption("Sorry,~not~today!", 255)
                                        .AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        client.Inventory.Remove(720852, 1, stream);
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more space in your inventory")
                                    .AddOption("Let me check.", 255)
                                  .AddAvatar(186).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.SendSysMesage("You received 1,116,000 points of EXP, 5 Amritas, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1116000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                        }

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory")
                                    .AddOption("Let me check.", 255)
                                  .AddAvatar(186).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720852, 1))
                            client.Inventory.Add(stream, 720852);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720851, 1))
                            {
                                data.AddText("Thank you very much for retrieving my Blaze Pill! Here are your rewards!")
                                      .AddOption("You~are~welcome!", 3)
                                      .AddOption("See~you~then!", 255)
                                    .AddAvatar(186).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need your help. Please bring the Blaze Pill back to me!")
                                         .AddOption("Okay!", 255)
                                       .AddAvatar(186).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("My master gave me a Blaze Pill. But the Thunder Ape grabbed it away. If my master knows that I lost the pill, he will be mad at me. Please find the pill back for me.")
                                  .AddOption("No~problem!", 2)
                                        .AddOption("Sorry,~not~today!", 255)
                                        .AddAvatar(186).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Inventory.Remove(720851, 1, stream);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.SendSysMesage("You received 1,079,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1079000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PoisonMaster)]
        public static void PoisonMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1348);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1349);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1350);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1351);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1352);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1353);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1354);
            var ActiveQuest8 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1355);
            var ActiveQuest9 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1356);
            var ActiveQuest10 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1357);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The purpose of studying poison is to save people`s lives!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Deliver~Medicine.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Thunder~Ginseng.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Courage~Test(1).", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Courage~Test(2).", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Courage~Test(3).", 40);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(1).", 50);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(2).", 60);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest8.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(3).", 70);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest9.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(4).", 80);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest10.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Enervation~Pill(5).", 90);

                        data.AddOption("Open another Quest.", 100).AddOption("Good!", 255);

                        data.AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 100:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 366, 304, 8293);
                        break;
                    }
                case 90:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest10.MissionId, 5))
                        {
                            data.AddText("Thank you very much for your help! The ape problem will be solved! Here are your rewards!")
                      .AddOption("Thank~you!", 92).AddOption("See~you~then!", 255)
                      .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I have a plan to deal with the Apes. They`re very curious creatures. I am sure they will eat the Enervation Pill when they find it. I will give you a bottle of Enervation Pill. Spread the pills in the required position. After the Apes eat the pills,")
                                .AddText("~I am sure they will become less aggressive.")
                                .AddOption("No~problem!", 91)
                                      .AddOption("Sorry,~not~today!", 255)
                                      .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 92:
                    {
                        client.Inventory.Remove(720846, 1, stream);
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest10.MissionId))
                        {
                            client.Inventory.Add(stream, 150118, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.SendSysMesage("You received 518,000 points of EXP,a level 60 Elite Ring and 900 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 518000, questReward: true);
                            client.Player.Money += 900;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 91:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720846, 1))
                            client.Inventory.Add(stream, 720846);
                        client.Player.AtiveQuestApe = 0;
                        client.Player.QuestGUI.Accept(ActiveQuest10, 0);

                        break;
                    }
                case 80:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest9.MissionId, 1))
                        {
                            data.AddText("Thank you very much for your help! Here are your rewards!")
                      .AddOption("Thank~you!", 82).AddOption("See~you~then!", 255)
                      .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Now, I want to put the Enervation Pill in the really tough battle. There is a Raging Ape in the wild. Feed the Enervation Pill to him and kill it!")
                                       .AddOption("No~problem!", 81)
                                      .AddOption("Sorry,~not~today!", 255)
                                      .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 81:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest9, 0);
                        if (client.Player.View.ContainMobInScreen("RagingApe") == false)
                            Server.AddMapMonster(stream, client.Map, 14336, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        else
                        {
                            data.AddText("There is a Raging Ape in the wild. Feed the Enervation Pill to him and kill it!")
                                     .AddOption("Okay!", 255)
                                     .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 82:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest9.MissionId))
                        {
                            client.SendSysMesage("You received 1,152,000 points of EXP, and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1152000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 70:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest8.MissionId, 1))
                        {
                            data.AddText("Thank you very much for testing the Enervation Pill for me! Here are your rewards!")
                       .AddOption("Thank~you!", 72).AddOption("See~you~then!", 255)
                       .AddAvatar(56).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            if (client.Inventory.Contain(720845, 1))
                            {
                                data.AddText("Go and find a Thunder Ape L58 and feed it the Enervation Pill and kill it!")
                                    .AddOption("Okay!", 255)
                                    .AddAvatar(56).FinalizeDialog();
                                break;
                            }
                            data.AddText("This time, I want to test the Enervation Pill on a real ape. Go find a Thunder Ape L58 and feed it the Enervation Pill and kill it! Don`t forget to tell me its response.")
                                   .AddOption("No~problem!", 71)
                                  .AddOption("Sorry,~not~today!", 255)
                                  .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 72:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest8.MissionId))
                        {
                            client.Inventory.Remove(720845, 1, stream);
                            client.SendSysMesage("You received 864,000 points of EXP, and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 864000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
                case 71:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720845, 1))
                            client.Inventory.Add(stream, 720845);
                        client.Player.QuestGUI.Accept(ActiveQuest8, 0);

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 1))
                        {
                            data.AddText("Thank you very much for helping me test the poison! Here are your rewards!")
                       .AddOption("Thank~you!", 62).AddOption("See~you~then!", 255)
                       .AddAvatar(56).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            data.AddText("Okay! This is the medicine I made! I call it Enervation Pill. Take Transmute Pill to transform into an Ape and then take the Enervation Pill to see what will happen on you!")

                                   .AddOption("No~problem!", 61)
                                  .AddOption("Sorry,~not~today!", 255)
                                  .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 62:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Inventory.Remove(720845, 1, stream);
                            client.SendSysMesage("You received 461,000 points of EXP, and 800 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 461000, questReward: true);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720845, 1))
                            client.Inventory.Add(stream, 720845);
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);


                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("Yes, that is the Transmute Pill. Let`s go to the next step.")
                       .AddOption("Let`s~begin!", 52).AddOption("See~you~then!", 255)
                       .AddAvatar(56).FinalizeDialog();

                            break;
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Go get the Transmute Pill from my old friend, the Apothecary!")
                         .AddOption("Okay!", 255)
                         .AddAvatar(56).FinalizeDialog();
                                break;
                            }
                            data.AddText("I have a good news to share with you! I made a new medicine! It can weaken the apes, but it does no harm to humans. So I can`t use my body to test its potency. Can you get a Transmute Pill from the Apothecary to transform into an Ape")
                             .AddText("~and then take my medicine to test it?")
                                .AddOption("No~problem!", 51)
                               .AddOption("Sorry,~not~today!", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 52:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.SendSysMesage("You received 288,000 points of EXp, and 500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 288000, questReward: true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 51:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                        {
                            data.AddText("I am quite impressed by your courage! Here are your rewards!")
                             .AddOption("Thank~you!", 42)
                             .AddOption("See~you~then!", 255)
                             .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Here is the final test! Jump off the cliff.  If you are a man of dignity, righteousness and courage, you will be lifted up by the clouds! Dare you try this?")
                              .AddOption("No~problem!", 41)
                              .AddOption("Never!", 255)
                              .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.SendSysMesage("You received 1,116,000 points of EXp, 1 Meteor and 2,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 1116000);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                        }

                        break;
                    }
                case 41:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("I am quite impressed by your courage! Here are your rewards!")
                          .AddOption("Thank~you~very~much!", 32)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(56).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Kill the Thunder Ape King to prove your courage to me!")
                            .AddOption("Okay!", 255)
                            .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("You passed one of my Courage Test, but it is only a beginning. Now, kill the Thunder Ape King!")
                              .AddOption("No~problem!", 31)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.SendSysMesage("You received 558,000 points of EXp, and 1000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 558000);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 31:
                    {
                        client.SendSysMesage("The Thunder Ape King appeared in front of your kill it!", MsgMessage.ChatMode.System);
                        if (client.Player.View.ContainMobInScreen("ApeTyrant") == false)
                            Server.AddMapMonster(stream, client.Map, 678000, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                            {
                                if (!client.Inventory.Contain(720870, 1, 0))
                                {
                                    data.AddText("What? You lost the Toxicant? Maybe I should ask someone else to take my courage test!")
                          .AddOption("Thank~you~very~much!", 22)
                          .AddAvatar(56).FinalizeDialog();
                                    break;
                                }
                                data.AddText("I am quite impressed by your courage! Here are your rewards!")
                       .AddOption("Never", 255)
                       .AddAvatar(56).FinalizeDialog();

                            }
                            else
                            {
                                if (!client.Inventory.Contain(720870, 1, 0))
                                {
                                    data.AddText("What? You lost the Toxicant? Maybe I should ask someone else to take my courage test!")
                          .AddOption("Thank~you~very~much!", 22)
                          .AddAvatar(56).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("Dare you take this Toxicant?")
                        .AddOption("Never", 255)
                        .AddAvatar(56).FinalizeDialog();
                                }
                            }

                            break;
                        }
                        data.AddText("People told me that you are brave. But I don`t believe it! You need to prove it to me! I made a new Toxicant! Dare you take it to test its power like me?")
                             .AddOption("No~problem!", 21)
                             .AddOption("Never!", 255)
                             .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 279,000 points of EXp, 1 EXP Ball Scrap, and 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 279000);
                            client.Inventory.Add(stream, 720172);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 720870);

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                       && client.Inventory.Contain(720869, 1, 0))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                            {
                                data.AddText("Thank you very much for the Thunder Ginseng you collected! Here are your rewards!")
                        .AddOption("You~are~welcome!", 12)
                        .AddOption("See~you~then!", 255)
                        .AddAvatar(56).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Use the Sickle to collect 10 Thunder Ginsengs for me!")
                           .AddOption("Okay!", 255)
                           .AddAvatar(56).FinalizeDialog();
                            }
                            break;
                        }
                        else
                        {
                            data.AddText("I need 10 Thunder Ginsengs to make an antidote to a rare poison. Take this Sickle and collect 10 Thunder Ginsengs for me!")
                            .AddOption("No~problem!", 11)
                            .AddOption("Sorry,~not~today!", 255)
                            .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                     .AddOption("Let me check.", 255)
                     .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                            client.SendSysMesage("You received 809,000 points of EXp, 5 Amritas and 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.Inventory.Remove(720869, 1, stream);
                            client.Inventory.Remove(720868, 10, stream);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.IncreaseExperience(stream, 809000);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.Inventory.Add(stream, 720869);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {

                        data.AddText("Thank you. I can use this Bezoar to purge the remaining poison from my body!")
                        .AddOption("You~are~welcome!", 2)
                        .AddOption("See~you~then!", 255)
                        .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711362, 1, stream);
                            client.SendSysMesage("You received 247,000 points of EXp, and 800 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 247000, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 800;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Apothecary)]
        public static void Apothecary(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1363);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1364);

            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1348);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PoisonMaster, client.Player.Class, 1353);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I have been doctored in Ape City for decades. Saving people is my job!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Collect~Medicine.", 50);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Collect~Medicine.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Weary~Pill.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Deliver~Medicine.", 30);
                        data.AddOption("Great~doctor!", 255);
                        data.AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 50:
                    {


                        data.AddText("Poison Master asked you to get a Transmute Pill from me? My friend must have an interesting plan! Ok, this is the Transmute Pill you need!")
                        .AddOption("Thanks.", 51)
                        .AddAvatar(64).FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720871, 1))
                        {
                            client.Inventory.Add(stream, 720871);
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest5.MissionId, 1);
                        }

                        break;
                    }
                case 30:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && client.Inventory.Contain(711362, 1, 0))
                        {
                            data.AddText("Please send the Quality Bezoar to my friend Poison Master.")
                         .AddOption("Okay.", 255)
                         .AddAvatar(64).FinalizeDialog();
                            break;
                        }

                        data.AddText("My friend Poison Master dedicated his life to the research of poison. He even use himself to try the poison and find out the way of detoxification. I am worrying about his body! After so many years of trial, many toxin must have remained in his body!")
                           .AddText("~I have a Quality Bezoar. It can help him clear up the remained toxin in his body! Will you send the Quality Bezoar to him?")
                            .AddOption("My~pleasure!", 31)
                             .AddOption("Sorry,~not~today!", 255)
                             .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.Inventory.Add(stream, 711362, 1);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.TransformationID == 356)
                        {
                            data.AddText("Thank you very much for testing the Weary Pill! It seems that it is safe to mankind! Here are your rewards!")
                          .AddOption("You~are~welcome!", 12)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        data.AddText("After years of study! I developed a Weary Pill that can deal with the apes in Ape City! But I need someone to test it whether it`s harmful to human beings. Will you try this Weary Pill? Don`t worry, I am a doctor, I can save you if anything bad happens.")
                                 .AddOption("I~can~risk~it!", 11)
                                   .AddOption("Sorry,~not~today!", 255)
                                   .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            client.Inventory.AddItemWitchStack(1000030, 0, 4, stream);
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                            .AddOption("Let me check.", 255)
                            .AddAvatar(64).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720865, 1, 0))
                        {
                            client.Inventory.Add(stream, 720865);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                            {
                                data.AddText("These are what I have been looking for! Thank you!")
                         .AddOption("You~are~welcome!", 3)
                         .AddOption("See~you~then!", 255)
                         .AddAvatar(64).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The wounded soldiers are waiting for medicine. Collect 1 Ape Claw, 1 Ape Bone, and 1 Ape Skin for me to make a medicine to treat them!")
                            .AddOption("Okay!", 255)
                            .AddAvatar(64).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Many soldiers were wounded by the monkeys. Collect 1 Ape Claw, 1 Ape Bone, and 1 Ape Skin for me! I can will make a medicine to treat the wounded soldiers.")
                                .AddOption("No~problem!", 2)
                                  .AddOption("Sorry,~not~today!", 255)
                                  .AddAvatar(7).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711357, 1, stream);
                            client.Inventory.Remove(711358, 1, stream);
                            client.Inventory.Remove(711359, 1, stream);
                            client.CreateBoxDialog("You~received~a~Meteor.~You~can~use~Meteors~to~upgrade~equipment~level~at~the~Equipment~Blacksmith~(Market~178,199).");
                            client.Inventory.Add(stream, 720172);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                        }

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.CarpenterJack)]
        public static void CarpenterJack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1308);//The Carpenter
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1315);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.JackDaniel, client.Player.Class, 1316);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.CarpenterJack, client.Player.Class, 1317);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I am the only carpenter in Ape City.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~The~Carpenter.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Wood~Shortage(1).", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~Wood~Shortage(2).", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Cliff~Flower.", 30);
                        data.AddOption("The~city~needs~you!", 255)
                            .AddOption("Continue Quests.", 50);
                        data.AddAvatar(9).FinalizeDialog();

                        break;
                    }
                case 50:
                    {
                        var nextquest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Apothecary, client.Player.Class, 1363);
                        client.Player.QuestGUI.SendAutoPatcher(nextquest.FinishNpcId.Map, nextquest.FinishNpcId.X, nextquest.FinishNpcId.Y, nextquest.FinishNpcId.ID);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 20))
                        {
                            data.AddText("Yes! That is the flower I have been looking for! It can help us to consolidate the bridges! Here are your rewards.")
                            .AddOption("Great!", 32)
                              .AddOption("See~you~then!", 255)
                              .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("My master told me that the juice of the Cliff Flower can be painted on the wood to prevent it from decaying and make it stronger.")
                            .AddText("~I hope you can collect 20 Cliff Flowers for me, so I can use them to fix the bridge outside Ape City.")
                            .AddOption("No~problem!", 31)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                            .AddOption("let me check.", 255)
                            .AddAvatar(9).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Remove(711356, 20, stream);
                            client.SendSysMesage("You received 247,000 points of EXP, 1 Ape City Gage and 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 247000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 1060022);

                        }

                        break;
                    }
                case 31:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 50))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                            .AddOption("let me check.", 255)
                            .AddAvatar(9).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.SendSysMesage("You received 743,000 points of Exp, 1,500 Silver, 1 Meteor.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                client.IncreaseExperience(stream, 743000, Role.Flags.ExperienceEffect.angelwing, true);
                                client.Player.Money += 1500;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.Inventory.Add(stream, Database.ItemType.Meteor);
                                data.AddText("The wood is arriving? Good! I will have enough materials to fix the wooden bridge! Thank you very much!")
                           .AddOption("You~are~welcome!", 255)
                           .AddAvatar(9).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Go slay 50 Giant Ape and protect the wood transport column.")
                       .AddOption("Okay!", 255)
                       .AddAvatar(9).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please go visit Jack Daniel.")
                           .AddOption("I~will~visit~him~right~away!", 11)
                           .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("The wooden bridge outside Ape City need to be fixed, but I don`t have enough wood. Speak with Jack Daniel to get some wood for me!")
                            .AddOption("No~problem!", 11)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(9).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 247,000 points of EXP, 500 Silver, and 1 Diligence Book.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 247000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Remove(711351, 1, stream);
                            client.Inventory.Add(stream, 723340);
                            data.AddText("Thank you very much! This bottle of Bamboo Liquid can be a nice, little refreshment for me!")
                       .AddOption("You~are~welcome!", 255)
                       .AddAvatar(9).FinalizeDialog();
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ACCastellan)]
        public static void ACCastellan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1306);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1307);//a waring
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ACCastellan, client.Player.Class, 1308);//The Carpenter
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It`s difficult to manage a city. There are too many things you need to think about!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Vulture~Feather.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~A~Warning.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~The~Carpenter.", 20);
                        data.AddOption("You~need~a~break!", 255);
                        data.AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(711351, 1))
                                client.Inventory.Add(stream, 711351);
                            data.AddText("Please go visit Carpenter Jack, and bring him the Bamboo Liquid.")
                        .AddOption("I~will~visit~him~right~away!", 255)
                        .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        data.AddText("Carpenter Jack helped us a lot by fixing the wooden bridge outside the city. I want you to visit him and bring him this bottle of Bamboo Liquid, to express our gratitude.")
                            .AddOption("My~pleasure!", 21)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.Inventory.Add(stream, 711351);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("let me check.", 255)
                        .AddAvatar(7).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.SendSysMesage("You received 374,000 points of EXP, 800 Silver, and 1 Apec City Gate.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                    client.IncreaseExperience(stream, 374000, Role.Flags.ExperienceEffect.angelwing, true);
                                    client.Player.Money += 800;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 1060022);
                                }
                                data.AddText("You brought peace back to the city! Thank you very much for your help. Here are your rewards!")
                           .AddOption("Thanks.", 255)
                           .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                if (!client.Inventory.Contain(720844, 1))
                                    client.Inventory.Add(stream, 720844);
                                data.AddText("The Macaques are doing increasing damage to farms. We need to do something to drive them away! Have you ever heard of the story of beating the dog before the lion? I want to do the same thing this time. Here is a bag of Pheasants.")
                            .AddText("~Release the Pheasant and kill it before the Macaques. It will be a warning to the Macaques. Go do it for me!")
                            .AddOption("I~will~do~it~right~away!", 255)
                            .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Macaques are doing increasing damage to farms. We need to do something to drive them away! Have you ever heard of the story of beating the dog before the lion? I want to do the same thing this time. Here is a bag of Pheasants.")
                           .AddText("~Release the Pheasant and kill it before the Macaques. It will be a warning to the Macaques. Go do it for me!")
                            .AddOption("My~pleasure!", 11)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.Contain(720844, 1))
                            client.Inventory.Add(stream, 720844);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1020, 648, 634, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {

                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.SendSysMesage("You received 674,000 points of Exp, 1,500 Silver and 1 Exp Ball Scrap.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                                    client.IncreaseExperience(stream, 674000, Role.Flags.ExperienceEffect.angelwing, true);
                                    client.Player.Money += 1500;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, 720172);
                                }
                                data.AddText("You brought peace to the city! Thank you very much for your help. Here are your rewards!")
                           .AddOption("Thanks.", 255)
                           .AddAvatar(7).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go get a Vulture Feather and burn it in the city. The other Vultures will never come near Ape City, after that!")
                            .AddOption("I~will~do~it~right~away!", 255)
                            .AddAvatar(7).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("We are quite annoyed by the Vultures outside Ape City, but I have an effective way to drive them far away from the city! If we get a Vulture Feather and burn it in the city, the other Vultures won`t dare come back!")
                           .AddText("~Don`t ask why, I know what I`m doing! Would you like to do this job for me?")
                            .AddOption("My~pleasure!", 2)
                              .AddOption("Sorry,~not~today!", 255)
                              .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Lydia)]
        public static void Lydia(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Lydia, client.Player.Class, 1311);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("The price of gold will soar! Believe me!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Retrieve~Jewelry.", 1);
                        data.AddOption("I~don`t~believe~it!", 255);
                        data.AddAvatar(1).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 20))
                            {
                                data.AddText("These are my Jewelry Boxes. Thank you very much for helping me! Here are some rewards for you!")
                               .AddOption("Thanks.", 3)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(1).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please! Please retrieve 20 Jewelry Boxes for me from the Macaques!")
                               .AddOption("I~will~do~it~right~away!", 255)
                               .AddAvatar(1).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The Macaques broke in the house and ran away with 20 of my Jewelry Boxes! They`re all I have! Will you bring them back for me?")
               .AddText("~I need to sell them to feed my family!")
               .AddOption("Don`t~worry,~I~will~help~you!", 2)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(1).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(1).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711352, 20, stream);
                            client.SendSysMesage("You received 701,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 701000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.CreateBoxDialog("You~received~a~+1~Stone.~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.AC_Lieutenant)]
        public static void AC_Lieutenant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.MilitiaCaptain, client.Player.Class, 1300);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1301);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1302);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1303);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1304);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.AC_Lieutenant, client.Player.Class, 1305);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("What would be the proof of a hero? A true hero never falters, even when the enemy is at the gates!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Quest:~The~Ape~City.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Clear~the~Way.", 2);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Appease~Dead~Souls.", 5);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Macaques(1).", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Macaques(2).", 15);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Macaques(3).", 20);
                        data.AddOption("You~are~a~great~hero!", 255);
                        data.AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                            {
                                data.AddText("Good job! Here are your rewards! Thank you very much for your help!")
                               .AddOption("Thanks.", 22)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 1 Monkey King to return peace to Ape City.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("This time, our target is the Monkey King. Slay the Monkey King and bring back its tail. It will help inspire the people in Ape City,")
               .AddText("~and should lift their spirits a bit.")
               .AddOption("My~pleasure.", 21)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Inventory.HaveSpace(2) == false)
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.Remove(720863, 1, stream);
                            client.SendSysMesage("You received 701,000 points of EXP, 1,500 Silver, 1 emerald and level50 EliteBoots.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 701000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 1080001);
                            client.Inventory.Add(stream, 160098);
                        }

                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 15:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 30))
                            {
                                data.AddText("You gave the Macaques a heavy blow. I think they will think twice before coming, again. Thank you very much for your help. Here are your rewards!")
                               .AddOption("Thanks.", 17)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 30 MacaquesL48 to return peace to Ape City.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("According to my observations, the Macaques L48 are the upper level forces of the apes. I need you to slay 30 Macaques L48 for me, to weaken them.")
                  .AddOption("My~pleasure.", 16)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 17:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.SendSysMesage("You received 701,000 points of EXP, 1,500 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 701000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 50))
                            {
                                data.AddText("I am quite impressed by your performance! Here are your rewards!")
                               .AddOption("Thanks.", 12)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Slay 50 Macaques to return peace to Ape City.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The naughty Macaques has been making trouble in the city for too long! We are tired of the constant noise, and don`t get")
                          .AddText("~me started on the smell! Go slay 50 Macaques for me!")
                  .AddOption("My~pleasure.", 11)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.SendSysMesage("You received 467,000 points of EXP, 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 467000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1, 1, 1, 1))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                               .AddOption("Thanks.", 7)
                               .AddOption("See~you~then!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please go appease the souls of my fallen soldiers on the battlefield.")
                               .AddOption("Okay!", 255)
                               .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I lost 5 of my best men in a decisive battle. They appeared to me one night, in a vision. Their souls are still wandering")
                            .AddText("~the battlefield, tortured and lost. I hope you can go to find their bodies and appease their souls for me.")
                    .AddOption("My~pleasure.", 6)
                    .AddOption("Sorry,~not~today!", 255)
                   .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You received 674,000 points of EXP, 1,000 Silver.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 674000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            break;
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.SendSysMesage("You received 179,000 points of EXP, 400 Silver, and 5 Amritas.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 179000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 400;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.AddItemWitchStack(1000030, 0, 5, stream);
                            break;
                        }
                        data.AddText("Nice to see you, hero! It is great that you came to Ape City. We could use the help. You need to keep your eyes open, while you`re here.")
                             .AddText("~This is not a safe place to just wander around.")
                     .AddOption("Okay!", 255)
                    .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Thank you very much for your help! Here are your rewards!")
                .AddOption("Thank~you!", 4)
                .AddOption("See~you~then!", 255)
               .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("For the safety of the town, you must wipe out 50 Vultures gathered outside of Ape City!")
                  .AddOption("Okay!", 255)
                 .AddAvatar(85).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Many Vultures appeared at the gates of Ape City. For the safety of the town, I hope you can help us slay 50 Vultures.")
                            .AddOption("No~problem!", 3)
                  .AddOption("Sorry,~not~today!", 255)
                 .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 4:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You received 674,000 points of EXP, 1,500 Silver, and 1 +1Stone.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white);
                            client.IncreaseExperience(stream, 674000, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B).~You~can~use~Stones~to~compose~equipment~in~the~Wuxing~Oven~(Market~178,204).");
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GreatMerchant)]
        public static void GreatMerchant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GreatMerchant, client.Player.Class, 706);
            switch (Option)
            {
                case 0:
                    {

                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("We have various kinds of stores in the market. But if you are interested in some rare items with special effect, I will be the")
                                .AddText("~perfect one to help you with your needs.")
             .AddOption("Well,~impress~me.", 1)
                        .AddOption("Purchase~Item.", 20)
                       .AddAvatar(174).FinalizeDialog();
                        }
                        else
                            goto case 20;

                        break;
                    }
                case 20:
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
                case 1:
                    {
                        if (client.Inventory.Contain(722700, 1) && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) || client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                data.AddText("How do you like it? I have lots of those items. When you need them, don`t hesitate to let me know!")
                           .AddOption("Brilliant.", 5)
                           .AddAvatar(174).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Just right click this EXP Potion to activate a 10 minute double EXP bonus. Talk to me again when you`re done.")
                             .AddOption("Okay.", 255)
                             .AddAvatar(174).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("You can buy some unusual items in the Shopping Mall. To enter it, talk to the Great Merchant")
                              .AddText("~in the Market, or click the Shopping Mall Button on the lower left of your screen when you are level 70 or have some CPs on you")
           .AddOption("CPs?", 2)
                     .AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("I guess you don`t know what that is yet, right?")
                                .AddText("~CP is short for Conquer Points, which are used to purchase special items from the Shopping Mall.")
                            .AddOption("I~see.", 3)
                            .AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Please take this EXP Potion as a present, you will obtain a 10 mins double EXP bonus when using it.")
                              .AddOption("Thank~you!", 4)
                              .AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(174).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 722700);
                        client.CreateBoxDialog("You`ve~received~an~EXP~Potion.~Right~click~it~to~use~it.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.CreateBoxDialog("You`ve~learned~about~CPs~and~the~Shopping~Mall,~and~received~EXP~worth~1/12~of~an~EXP~Ball!");
                        }

                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.SuperGuildWar)] // [OfficialFix 2026-10-14] private-server war NPC (SobNpcs 25015, no map): its claim paid 2,000,000,000 CPs + Lord Token; handler disabled
        public static void SuperGuildWar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Super Guild War Starts Wednesday At Hour 21.00 Ended 22.00")
              .AddOption("Enter~the~Super~Guild~Area.", 6)
                         .AddOption("Claim The SuperGuildWar Reward.", 7)
                            .AddOption("Just~passing~by.", 255)
                        .AddAvatar(6).FinalizeDialog();


                        break;
                    }

                case 6:
                    {
                        client.Player.ProtectAttack(5 * 1000); // 5 Seconds


                        if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Alive)
                        {
                            client.Teleport(348, 339, 1138);
                        }
                        else
                        {
                            data.AddText("The tournament is offline at the moment.")
            .AddOption("Thank you.", 255).AddAvatar(110).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID && client.Player.MyGuild != null && MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {

                            if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.LeaderReward > 0
                                 && !MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(2))
                                {
                                    data.AddText("Please make one more space in your inventory.")
                  .AddOption("let me check", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }

                                MsgTournaments.MsgSchedules.SuperGuildWar.RewardLeader.Add(client.Player.UID);

                                MsgTournaments.MsgSchedules.SuperGuildWar.Winner.LeaderReward -= 1;
                                // حفظ حالة الاستلام فورًا (منع الاستلام مرتين لو وقع)
                                try { MsgTournaments.MsgSchedules.SuperGuildWar.Save(); } catch (Exception exGwSave) { MyConsole.WriteLine(exGwSave.ToString()); }

                                client.Inventory.Add(stream, 723467, 1);

                                client.Player.ConquerPoints += 2000000000; // 2B CPs

                                client.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.WingsofSolarDra, stream);
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSuperGuildWarFiveStars, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                Server.SendGlobalPacket(new MsgServer.MsgMessage("[ " + client.Player.Name + " ] , GuildLeader from " + client.Player.MyGuild.GuildName + " was rewarded with 2.000.000.000 CPs - 1x Lord Token for the domination the Super Guild War.", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                            }
                            else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader && MsgTournaments.MsgSchedules.SuperGuildWar.Winner.DeputiLeaderReward > 0
                                && !MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                            {
                                MsgTournaments.MsgSchedules.SuperGuildWar.Winner.DeputiLeaderReward -= 1;
                                MsgTournaments.MsgSchedules.SuperGuildWar.RewardDeputiLeader.Add(client.Player.UID);
                                try { MsgTournaments.MsgSchedules.SuperGuildWar.Save(); } catch (Exception exGwSave) { MyConsole.WriteLine(exGwSave.ToString()); }
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSuperGuildWarThreeStars, Role.StatusFlagsBigVector32.PermanentFlag, false);
                            }
                            else
                            {
                                data.AddText("Just the GuildLeader or DeputiLeader can claim the reward!")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your~guild~has~not~dominated~super~guild~area.~Super~Guild~war~will~be~held~on~every~Tuesday,~lasting~2~days.")
                        .AddOption("Ok~thank~you.", 255).AddAvatar(110).FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        //[NpcAttribute(NpcID.GuildOfficer)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void GuildOfficer(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {
                            data.AddText("Guild War starts on Friday at 21:00 and ends at 22:00 and the winning guild gets 1.000.000 CPs - 5.000.000.000 Money - 1x Lord Token and Title (Conqueror).")
                               .AddOption("I would like to claim it.", 1)
                           .AddOption("Just passing by.", 255).AddAvatar(110).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Guild War starts on Friday at 21:00 and ends at 22:00 and the winning guild gets 1.000.000 CPs - 5.000.000.000 Money - 1x Lord Token and Title (Conqueror).")
                                .AddOption("I would like to claim it.", 1)
                           .AddOption("Just passing by.", 255).AddAvatar(110).FinalizeDialog();
                        }


                        break;
                    }
                case 150:
                    {
                        data.AddText("Guild Leaders and Deputy Leaders whose guild wins in this Guild War can get an Honor Halo, which will last for 1 week.");
                        data.AddOption("Claim an Honor Halo.", 160).AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 160:
                    {
                        data.AddText("Your guild has not dominated guild area. Guild war will be held on every Saturday, lasting 2 days.The winner can seize the guild area and gain the funds after that.");
                        data.AddOption("Ok, I see.", 255).AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.GuildID == MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID && client.Player.MyGuild != null && MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                        {

                            if (client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader && MsgTournaments.MsgSchedules.GuildWar.Winner.LeaderReward > 0
                                 && !MsgTournaments.MsgSchedules.GuildWar.RewardLeader.Contains(client.Player.UID))
                            {
                                if (!client.Inventory.HaveSpace(2))
                                {
                                    data.AddText("Please make 2 more spaces in your inventory.").AddOption("Let me check.", 255).AddAvatar(110).FinalizeDialog();
                                    break;
                                }

                                client.Player.AddSpecialTitle(MsgTitleStorage.TitleType.Conqueror, stream);
                                client.SendSysMesage("You received a title (Conqueror).", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);

                                MsgTournaments.MsgSchedules.GuildWar.RewardLeader.Add(client.Player.UID);

                                MsgTournaments.MsgSchedules.GuildWar.Winner.LeaderReward = 0;
                                // حفظ حالة الاستلام فورًا في GuildWarInfo.ini (منع الاستلام مرتين لو وقع)
                                try { MsgTournaments.MsgSchedules.GuildWar.Save(); } catch (Exception exGwSave) { MyConsole.WriteLine(exGwSave.ToString()); }

                                client.Inventory.Add(stream, 723467, 1); // 1x Lord Token

                                client.Player.ConquerPoints += 1000000;

                                client.Player.Money += 5000000000;

                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopGuildLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                                Server.SendGlobalPacket(new MsgServer.MsgMessage(" Congratulations, " + client.Player.Name + " Guild Leader [ " + client.Player.MyGuild.GuildName + " ] He Won The Guild War and Received 1.000.000 CPs - 5.000.000.000 Money - x1 Lord Token ", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                            }
                            else if (client.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader && MsgTournaments.MsgSchedules.GuildWar.Winner.DeputiLeaderReward > 0
                                && !MsgTournaments.MsgSchedules.GuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                            {
                                MsgTournaments.MsgSchedules.GuildWar.Winner.DeputiLeaderReward -= 1;
                                MsgTournaments.MsgSchedules.GuildWar.RewardDeputiLeader.Add(client.Player.UID);
                                try { MsgTournaments.MsgSchedules.GuildWar.Save(); } catch (Exception exGwSave) { MyConsole.WriteLine(exGwSave.ToString()); }
                                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopDeputyLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                            }
                            else
                            {
                                data.AddText("Just the Guild Leader or Deputi Leader can claim the reward!")
                        .AddOption("Okay.", 255).AddAvatar(110).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Your guild has not dominated guild area. Guild war will be held on every Saturday, lasting 2 days.The winner can seize the guild area and gain the funds after that.")
                        .AddOption("Ok, I see.", 255).AddAvatar(110).FinalizeDialog();
                        }


                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.ChangeLordTokensNPC)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled // The Npc Edit By [ Mohy ElDin - Ã™â€¦Ã˜Â­Ã™Å  Ã˜Â§Ã™â€žÃ˜Â¯Ã™Å Ã™â€  ]
        public static void ChangeLordTokensNPC(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText(" Hello, You Can Exchange \n (3) Lord Token for a [Gold Prize] \n (9) Lord Tokens for a [Holy Grail].")
                          .AddOption("(3) Lord Token = [Gold Prize].", 2)
                          .AddOption("(9) Lord Token = [Holy Grail].", 3)
                          .AddOption("I~will~come~later.", 255).AddAvatar(3129).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(723467, 3) && client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Remove(723467, 3, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                            client.Inventory.Add(stream, 2100075, 1);
                            data.AddText("Congratulations, You Have Exchanged 3 (Lord Tokens) for [Gold Prize].");
                            data.AddOption("Thanks.", 255).AddAvatar(3129).FinalizeDialog();
                        }
                        else
                        {
                            client.CreateBoxDialog("Sorry, You Don`t Have Any Lord Tokens !");
                        }
                        break;

                    }
                case 3:
                    {
                        if (client.Inventory.Contain(723467, 9) && client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Remove(723467, 9, stream);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, id, true, "firemagic");
                            client.Inventory.Add(stream, 2100095, 1);
                            data.AddText("Congratulations, You Have Exchanged 9 (Lord Tokens) for [Holy Grail].");
                            data.AddOption("Thanks.", 255).AddAvatar(3129).FinalizeDialog();
                        }
                        else
                        {
                            client.CreateBoxDialog("Sorry, You Don`t Have Any Lord Tokens !");
                        }
                        break;

                    }               
            }
        }
        //[NpcAttribute(NpcID.ExchangeDB)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled // The NPC Edit By Mohy ElDin ...
        public static void ExchangeDB(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
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

                        data.AddText("I can exchange your DragonBalls and DBScroll for CPs.\n");
                        data.AddText("Exchange DragonBall for 215 CPs.\n");
                        data.AddText("Exchange DBScroll for 2150 CPs.\n");
                        data.AddText("Choose what you want to exchange.\n");
                        data.AddOption("Exchange DragonBall (215 CPs)", 9);
                        data.AddOption("Exchange DBScroll (2150 CPs)", 10);
                        data.AddOption("Just passing by. ", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("Are you sure you want to exchange a Dragon Ball for 215 CPs")
                      .AddOption("Yep!", 9)
                        .AddOption("Nope.", 255).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                       
                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        data.AddText("Are you sure you want to exchange a DBScroll for 2150 CPs")
                      .AddOption("Yep!", 10)
                        .AddOption("Nope.", 255).AddAvatar(3497).FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);
                            client.Player.ConquerPoints += 215;

                            Database.ServerDatabase.LoginQueue.Enqueue(
        "Check Dragon Balls [Exchange] " +
        "Username Player : [ " + client.Player.Name + " ] " +
        "UID Player : [ " + client.Player.UID + " ] " +
        "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ] " +
        "Item : [ Dragon Ball ] " +
        "Amount : [ 1 ] " +
        "CPs Received : [ 215 ] " +
        "All CPs Player : [ " + client.Player.ConquerPoints + " ] " +
        "Map : [" + client.Player.Map + "] " +
        "X : [" + client.Player.X + "] " +
        "Y : [" + client.Player.Y + "]");

                        }
                        else
                        {
                            data.AddText("You don't have a DragonBall.")
                          .AddOption("Let~me~think.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
                case 10:
                    {
                       

                        if (client.IsVendor)
                            return;

                        if (client.InTrade)
                            return;

                        if (client.Inventory.Contain(Database.ItemType.DBScroll, 1))
                        {
                            client.Inventory.Remove(Database.ItemType.DBScroll, 1, stream);
                            client.Player.ConquerPoints += 2150;
                            Database.ServerDatabase.LoginQueue.Enqueue(
        "Check Dragon Balls [Exchange] " +
        "Username Player : [ " + client.Player.Name + " ] " +
        "UID Player : [ " + client.Player.UID + " ] " +
        "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ] " +
        "Item : [ DB Scroll ] " +
        "Amount : [ 1 ] " +
        "CPs Received : [ 2150 ] " +
        "All CPs Player : [ " + client.Player.ConquerPoints + " ] " +
        "Map : [" + client.Player.Map + "] " +
        "X : [" + client.Player.X + "] " +
        "Y : [" + client.Player.Y + "]");

                        }
                        else
                        {
                            data.AddText("You don't have a DBScroll.")
                          .AddOption("Let~me~think.", 255).AddAvatar(3497).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        //[NpcAttribute(NpcID.Blessed1)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void Blessed1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Just paying 5.000 CPs, you`re able to activate the Wuxing Oven and attach 1 blessed attribute on specific equipment. It`s different from that attributes upgraded with Tortoise Gems.")
                            .AddText("~So, which type of equipment would you like to bless?")
                       .AddOption("Garment.", 2)
                       .AddOption("Mount~Armor.", 4)
                       .AddOption("Star~Tower.", 6)
                       .AddOption("Heaven~Fan.", 8)
                       .AddOption("Riding~Crop.", 10)
                       .AddOption("Gourd/Magic~Bottle.", 12)
                       .AddOption("Wing.", 50)
                        .AddOption("RightWeapon.", 70)
                      .AddOption("LeftWeapon.", 144)
                       .AddOption("#Thnx.", 255).FinalizeDialog();
                        break;

                    }
                case 19:
                    {
                        data.AddText("You can obtain Composition Points from minor items and make your main items reach the next bonus level on the Forging interface.")
                      .AddOption("How~to~refine~equipment?", 20)
                      .FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        data.AddText("There are two ways to refine items. \n1 .Right click the refined items and click the equipment with the same type to refine.")
                          .AddText("\n 2.Refine the equipment on the Forging interface.")
                    .AddOption("Okay.", 255)
                    .FinalizeDialog();

                        break;
                    }
                case 14:
                    {

                        data.AddText("You may promote the bonus level of items on the Forging interface, namely compose +n items. +n items have +n stats which")
                            .AddText("~can make your items much more powerful.")
                      .AddOption("I~see.", 255)
                      .FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        data.AddText("The enchanted weapon can increase up to 255 HP. The higher quality gem will enchant higher HP. The enchanted effects vary with different gems.")
                                 .AddOption("How~to~enchant?", 16)
                                 .AddOption("I~can~enchat~it~repeatedly?", 17)
                                 .AddOption("What~gems~do~I~need?", 18)
                                 .FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        data.AddText("You can promote the bonus level of weapon on the Forging interface. Put the weapon and the enchanted materials into the designated places, then begin to enchant.")
                            .AddOption("Okay.", 255)
                            .FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        data.AddText("Sure. You can re-enchant your weapon on the Forging interface. If the new attribute is higher, it will replace the old one, otherwise, it won`t replace.")
                              .AddOption("Okay.", 255)
                              .FinalizeDialog();

                        break;
                    }
                case 18:
                    {
                        data.AddText("They can be Dragon, Phoenix, Tortoise, Rainbow, Moon, Violet, Fury, or Kylin. The enchanted effects vary with the gem quality.")
                              .AddOption("Okay.", 255)
                              .FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Just paying 5.000 CPs, you`re able to activate the Wuxing Oven and attach 1 blessed attribute on specific equipment. It`s different from that attributes upgraded with Tortoise Gems.")
                            .AddText("~So, which type of equipment would you like to bless?")
                       .AddOption("Garment.", 2)
                       .AddOption("Mount~Armor.", 4)
                       .AddOption("Star~Tower.", 6)
                       .AddOption("Heaven~Fan.", 8)
                       .AddOption("Riding~Crop.", 10)
                       .AddOption("Gourd/Magic~Bottle.", 12)
                       .AddOption("Wing.", 50)
                        .AddOption("RightWeapon.", 70)
                      .AddOption("LeftWeapon.", 144)
                       .AddOption("#Thnx.", 255).FinalizeDialog();
                        break;

                    }
                case 70:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1% blessed attribute to the RightWeapon?")
                        .AddOption("Yes.", 71)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 144:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1% blessed attribute to the LeftWeapon?")
                        .AddOption("Yes.", 145)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 71:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.RightWeaponAccessory))
                        {
                            data.AddText("You need to equip Accessory, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeaponAccessory, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Accessory.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Accessory.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 145:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.LeftWeaponAccessory))
                        {
                            data.AddText("You need to equip Accessory, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeaponAccessory, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Accessory.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Accessory.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 50:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the Wing?")
                        .AddOption("Yes.", 51)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Wing))
                        {
                            data.AddText("You need to equip Wing, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Wing, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Wing.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Wing.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 12:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the Gourd?")
                        .AddOption("Yes.", 13)
                        .AddOption("No,~I~changed~my~mind.", 1)
                       .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Bottle))
                        {
                            data.AddText("You need to equip Gourd, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Bottle, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Gourd.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Gourd.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the Riding Crop?")
                       .AddOption("Yes.", 11)
                       .AddOption("No,~I~changed~my~mind.", 1)
                       .AddOption("Thanks.", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.RidingCrop))
                        {
                            data.AddText("You need to equip Riding Crop, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RidingCrop, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Riding Crop.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Riding Crop.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 8:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the Heaven Fan?")
                        .AddOption("Yes.", 9)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Fan))
                        {
                            data.AddText("You need to equip Heaven Fan, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Fan, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Heaven Fan.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Heaven Fan.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the Star Tower?")
                       .AddOption("Yes.", 7)
                       .AddOption("No,~I~changed~my~mind.", 1)
                      .AddOption("Thanks.", 255)
                       .FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Tower))
                        {
                            data.AddText("You need to equip Star Tower, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Tower, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Star Tower.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Star Tower.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the Mount Armor?")
                        .AddOption("Yes.", 5)
                        .AddOption("No,~I~changed~my~mind.", 1)
                        .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.SteedMount))
                        {
                            data.AddText("You need to equip Mount Armor, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.SteedMount, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Mount Armor.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the Mount Armor.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure you want to pay 5000 CPs to add 1 blessed attribute to the garment?")
                        .AddOption("Yes.", 3)
                        .AddOption("No,~I~changed~my~mind.", 1)
                       .AddOption("Thanks.", 255)
                        .FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Garment))
                        {
                            data.AddText("You need to equip Garment, first.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints > 5000)
                        {
                            Game.MsgServer.MsgGameItem item;
                            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.Garment, out item))
                            {
                                if (item.Bless > 0)
                                {
                                    data.AddText("You already have 1 blessed attribute in Garment.")
                       .AddOption("I~see.", 255)
                       .FinalizeDialog();
                                    break;
                                }
                                item.Bless = 1;
                                item.Mode = Role.Flags.ItemMode.Update;
                                item.Send(client, stream);

                                data.AddText("Congratulation! You've added 1 blessed attribute to the garment.")
                   .AddOption("Thank you.", 255)
                   .FinalizeDialog();
                                client.Player.ConquerPoints -= 5000;
                                client.Status.ItemBless += 1;
                            }
                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to attach blessed attribute to the equipment.")
                       .AddOption("Alright.", 255)
                       .FinalizeDialog();
                        }

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PokerCpsCasino)]
        public static void PokerCpsCasino(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Greeting, I`m glad to be at your service. Win or lose is not that important. To win is not the ultimate aim of the game, happiness is what we have been pursuing. Here, I even more like to wish you a good health than a good luck!")
                       .AddOption("I~have~no~interest~in~it.", 1)
                       .AddOption("Enter~Poker~Room.", 2)
                       .AddOption("I~still~want~to~play.", 255)
                       .AddAvatar(175).FinalizeDialog();

                        break;
                    }
                case 1:
                    {

                        client.SendSysMesage("You are teleported to Twin City.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(Places.TwinCity);

                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You are teleported to the Poker Room.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(157, 119, 3053);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.PokerCasinoHostess)]
        public static void PokerCasinoHostess(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.Text("Greeting, I`m glad to be at your service. Win or lose is not that important. To win is not the ultimate aim of the game, happiness is what we have been pursuing. Here, I even more like to wish you a good health than a good luck!");
                        data.Option("I~have~no~interest~in~it.", 1);
                        data.Option("Enter~Omaha~Room.", 2);
                        data.Option("Enter~VIP~Hall.", 3);
                        data.Option("I~still~want~to~play.", 255);
                        data.Avatar(175);
                        data.FinalizeDialog();
                        break;

                    }
                case 1:
                    {
                        client.SendSysMesage("You are teleported to Twin City.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(Places.TwinCity);

                        break;
                    }
                case 2:
                    {
                        client.SendSysMesage("You are teleported to Twin City.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(267, 257, 3053);

                        break;
                    }
                case 3:
                    {
                        client.SendSysMesage("You arrived on the VIP Hall.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.white);
                        client.Teleport(55, 62, 1860);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.FullInnerPower)]
        public static void InnerPower(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Reborn == 2)
                        {
                            if (client.Player.InnerPower.Stages[0].UnLocked == false)
                            {
                                data.AddText("To concentrate on the study of humankind`s potential, I completely")
                                    .AddText(" isolated myself from the outside world 100 years ago. Finally, I achieved the essence of Inner Power.")
                            .AddOption("How~to~acquire~Inner~Power?", 1)
                            .AddOption("Not~interested.", 255)
                            .AddAvatar(6).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Good to see you again. If you've finished the studying the Inner Power Book I gave")
                                   .AddText(" you last time, you can claim the next book.")
                           .AddOption("Claim the next book.", 10)
                           .AddOption("I'm just passing by.", 255)
                           .AddAvatar(6).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Kid, you`re not strong enough to study my Inner Power Books. The unusual power may hurt you instead.")
                                   .AddText("If you`re really interested, find me when you reach at least Level 15 of 2nd-reborn.")
                           .AddOption("Alright.", 255)
                           .AddAvatar(6).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        Role.Instance.InnerPower.Stage stage;
                        if (client.Player.InnerPower.GetLastStage(out stage))
                        {
                            if (stage.Complete)
                            {
                                if (stage.ID < Database.InnerPowerTable.Count)
                                {
                                    if (!client.Inventory.HaveSpace(1))
                                    {
                                        data.AddText("Please make 1 more space in your inventory")
                                           .AddOption("Let me check.", 255).AddAvatar(6)
                                          .FinalizeDialog();
                                        break;
                                    }
                                    var next_stage = Database.InnerPowerTable.Stages[stage.ID];
                                    var DbNeiGong = next_stage.NeiGongAtributes[0];
                                    var DbItem = Pool.ItemsBase[DbNeiGong.ItemID];
                                    if (client.Inventory.Contain(DbItem.ID, 1))
                                    {
                                        data.AddText("Kid, you need to train the power of " + DbItem.Name + " to the perfect level before")
                      .AddText(" you can claim the next book. Be patient, okay?")
                       .AddOption("Okay.", 255)
                      .AddAvatar(6).FinalizeDialog();
                                        break;
                                    }
                                    client.Inventory.Add(stream, DbItem.ID);
                                    client.SendSysMesage("You received an Inner Power Book, " + DbItem.Name + ". Hurry and go training your Inner Power with it!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                    data.AddText("Meeting you was fate. I`d like present you a " + DbItem.Name + ". When you acquire its essence and reach the perfect level, you can")
                           .AddText(" find me to claim the next book.")
                            .AddOption("Thanks~a~lot!", 255)
                           .AddAvatar(6).FinalizeDialog();
                                }
                                else
                                {
                                    //You've completed the inner power concept .
                                    data.AddText("You've completed the inner power concept .")
                                         .AddOption("Thanks~a~lot.", 255).AddAvatar(6)
                                        .FinalizeDialog();
                                    //completed.
                                }
                            }
                            else
                            {
                                Role.Instance.InnerPower.Stage.NeiGong nei_gong;
                                if (stage.GetLastNeiGang(out nei_gong))
                                {
                                    if (nei_gong.Complete)
                                    {
                                        var db_stage = Database.InnerPowerTable.Stages[stage.ID - 1];
                                        foreach (var dbnei in db_stage.NeiGongAtributes)
                                        {
                                            if (dbnei.ID == (byte)(nei_gong.ID + 1))
                                            {
                                                if (!client.Inventory.HaveSpace(1))
                                                {
                                                    data.AddText("Please make 1 more space in your inventory")
                                                       .AddOption("Let me check.", 255).AddAvatar(6)
                                                      .FinalizeDialog();
                                                    break;
                                                }
                                                var DbItem = Pool.ItemsBase[dbnei.ItemID];
                                                if (client.Inventory.Contain(DbItem.ID, 1))
                                                {
                                                    data.AddText("Kid, you need to train the power of " + DbItem.Name + " to the perfect level before")
                                  .AddText(" you can claim the next book. Be patient, okay?")
                                   .AddOption("Okay.", 255)
                                  .AddAvatar(6).FinalizeDialog();
                                                    break;
                                                }
                                                client.SendSysMesage("You received an Inner Power Book, " + DbItem.Name + ". Hurry and go training your Inner Power with it!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                                                client.Inventory.Add(stream, DbItem.ID);
                                                data.AddText("Meeting you was fate. I`d like present you a " + DbItem.Name + ". When you acquire its essence and reach the perfect level, you can")
                                       .AddText(" find me to claim the next book.")
                                        .AddOption("Thanks~a~lot!", 255)
                                       .AddAvatar(6).FinalizeDialog();
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var db_stage = Database.InnerPowerTable.Stages[stage.ID - 1];
                                        foreach (var dbnei in db_stage.NeiGongAtributes)
                                        {
                                            if (dbnei.ID == (byte)(nei_gong.ID))
                                            {
                                                var DbItem = Pool.ItemsBase[dbnei.ItemID];
                                                data.AddText("Kid, you need to train the power of " + DbItem.Name + " to the perfect level before")
                                   .AddText(" you can claim the next book. Be patient, okay?")
                                    .AddOption("Okay.", 255)
                                   .AddAvatar(6).FinalizeDialog();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("During the 100 years, I created numerous concepts, but found it too complicated. Therefore, I embodies these concepts through books.")
                  .AddText("~It also requires Potency Points to make breakthrough while training the Inner Power. You can collect Potency Points from the Kingdom Missions.")
                            .AddOption("How~can~I~get~the~books?", 2)
                    .AddAvatar(7).FinalizeDialog();

                        break;
                    }
                case 2:
                    {

                        var db_stage = Database.InnerPowerTable.Stages[0];
                        client.Inventory.Add(stream, db_stage.NeiGongAtributes[0].ItemID);

                        data.AddText("Meeting you was fate. I`d like present you a Universal Concept (A). When you acquire its essence and reach the perfect level, you can")
                            .AddText(" find me to claim the next book.")
                             .AddOption("Thanks~a~lot!", 255)
                            .AddAvatar(7).FinalizeDialog();

                        break;
                    }

            }
        }
        public static int CpsInnerPowerStage(ushort ID)
        {
            switch (ID)
            {
                case 1:
                    return 400;
                case 2:
                    return 600;
                case 3:
                    return 800;
                case 4:
                    return 1000;
                case 5:
                    return 1200;
                case 6:
                    return 1400;
                case 7:
                    return 5000;
                case 8:
                    return 10000;
                case 9:
                    return 15000;
                case 10:
                    return 20000;
                case 11:
                    return 25000;
                case 12:
                    return 30000;

            }
            return 0;
        }
        //[NpcAttribute(NpcID.InnerPowerNpc)]
        //public static void InnerPowerNpc(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    var Count = client.Inventory.GetCountItem(3005412);
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {

        //                if (client.Player.Reborn == 2)
        //                {
        //                    data.AddText("STR_ID_tWork[23021][Text111]@@\n");
        //                    data.AddText("STR_ID_tWork[23021][Text112]@@\n");
        //                    data.AddText("STR_ID_tWork[23021][Text113]@@\n");
        //                    data.AddText("Reward: <tip color=0xfff05050 desc=Dragon#Tactics		Boundless#Heart\nDoctrine#of#Deity			Puzzle#of#Life>Inner Power Books</tip>,");
        //                       data.AddText("STR_ID_tWork[23021][Text115]@@\n");
        //                    if (client.Player.InnerPower.Stages[0].UnLocked == false)
        //                    {
        //                        data.AddOption("How~to~acquire~Inner~Power?", 15);
        //                    }
        //                    data.AddOption("STR_ID_tWork[23021][Option3]@@\n", 2);
        //                    data.AddOption("STR_ID_tWork[23021][Option124]@@\n", 4);
        //                data.AddAvatar(6).FinalizeDialog();


        //                }
        //                else
        //                {
        //                    data.AddText("Kid, you`re not strong enough to study my Inner Power Books. The unusual power may hurt you instead.")
        //                           .AddText("If you`re really interested, find me when you reach at least Level 15 of 2nd-reborn.")
        //                   .AddOption("Alright.", 255)
        //                   .AddAvatar(6).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 15:
        //            {
        //                data.AddText("During the 100 years, I created numerous concepts, but found it too complicated. Therefore, I embodies these concepts through books.")
        //          .AddText("~It also requires Potency Points to make breakthrough while training the Inner Power. You can collect Potency Points from the Kingdom Missions.")
        //                    .AddOption("How~can~I~get~the~books?", 16)
        //            .AddAvatar(7).FinalizeDialog();

        //                break;
        //            }
        //        case 16:
        //            {

        //                if (client.Player.ConquerPoints >= 200)
        //                {
        //                    client.Player.ConquerPoints -= 200;
        //                    var db_stage = Database.InnerPowerTable.Stages[0];
        //                    client.Inventory.Add(stream, db_stage.NeiGongAtributes[0].ItemID);

        //                    data.AddText("Meeting you was fate. I`d like present you a Universal Concept (A). When you acquire its essence and reach the perfect level, you can")
        //                        .AddText(" find me to claim the next book.")
        //                         .AddOption("Thanks~a~lot!", 255)
        //                        .AddAvatar(7).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("you don't have 200 CPs")
        //                       .AddOption("Thanks~a~lot!", 255)
        //                      .AddAvatar(7).FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 1:
        //            {
        //                data.AddText("STR_ID_tWork[23021][Text111]@@\n")
        //                              .AddText("STR_ID_tWork[23021][Text211]@@\n")
        //                               .AddText("STR_ID_tWork[23021][Text212]@@\n")
        //                                .AddText("STR_ID_tWork[23021][Text213]@@\n")
        //                                 .AddText("STR_ID_tWork[23021][Text214]@@\n")
        //                             .AddOption("STR_ID_tWork[23021][Option4]@@\n", 255)
        //                      .AddAvatar(6).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                if (client.Inventory.Contain(3005412, 1))
        //                {
        //                    data.AddText("STR_ID_tWork[23021][Text411]@@\n")
        //                       .AddText("STR_ID_tWork[23021][Text412]@@\n")
        //                       .AddOption("PowerEraser.~(10000~Potency~Pts)", 7)
        //                       .AddOption("STR_ID_tWork[23021][Option11]@@\n", 255)
        //                        .AddAvatar(6).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    client.CreateBoxDialog("STR_ID_tWork[23021][NoAllItem]@@");
        //                }

        //                break;
        //            }

        //        case 4:
        //            {
        //                client.MyExchangeShop.Open(stream, 3008253, 169);


        //                break;
        //            }
        //        case 5:
        //            {
        //                data.AddText("STR_ID_tWork[23021][Text111]@@\n")
        //                .AddText("STR_ID_tWork[23021][Text821]@@\n")
        //                                .AddText("STR_ID_tWork[23021][Text822]@@\n")
        //                                 .AddText("STR_ID_tWork[23021][Text823]@@\n")
        //                                  .AddText("STR_ID_tWork[23021][Text824]@@\n")
        //                                  .AddOption("STR_ID_tWork[23021][Option821]@@\n", 6)
        //                               .AddOption("STR_ID_tWork[23021][Option822]@@\n", 255)
        //                        .AddAvatar(6).FinalizeDialog();
        //                break;
        //            }
        //        case 7:
        //            {

        //                data.AddText("There are " + Count.ToString() + " PowerEraser in your inventory, each for 10000 Potency Points.\n")
        //                    .AddOption("STR_ID_tWork[23021][Option18]@@\n", 8)
        //                               .AddOption("STR_ID_tWork[23021][Option19]@@\n", 9)
        //                                 .AddOption("STR_ID_tWork[23021][Option14]@@\n", 10)
        //                                 .AddOption("STR_ID_tWork[23021][Option15]@@\n", 255)
        //                    .AddAvatar(6).FinalizeDialog();
        //                break;
        //            }
        //        case 8:
        //            {
        //                data.AddText("Are you sure you want to swap 1 PowerEraser?.\n")
        //                  .AddOption("STR_ID_tWork[23021][Option16]@@\n", 11)
        //                               .AddOption("STR_ID_tWork[23021][Option17]@@\n", 255)
        //               .AddAvatar(6).FinalizeDialog();

        //                break;
        //            }
        //        case 9:
        //            {
        //                data.AddText("Are you sure you want to swap 5 PowerEraser?.\n")
        //                     .AddOption("STR_ID_tWork[23021][Option16]@@\n", 12)
        //                               .AddOption("STR_ID_tWork[23021][Option17]@@\n", 255)
        //                .AddAvatar(6).FinalizeDialog();

        //                break;
        //            }
        //        case 10:
        //            {
        //                data.AddText("Are you sure you want to swap " + Count.ToString() + " PowerEraser?.\n")
        //                 .AddOption("STR_ID_tWork[23021][Option16]@@\n", 13)
        //                               .AddOption("STR_ID_tWork[23021][Option17]@@\n", 255)
        //                .AddAvatar(6).FinalizeDialog();

        //                break;
        //            }
        //        case 11:
        //            {
        //                if (client.Inventory.Contain(3005412, 1) || client.Inventory.Contain(3005412, 1, 1))
        //                {
        //                    client.Inventory.Remove(3005412, 1, stream);
        //                    client.Player.InnerPower.AddPotency(stream, client, 10000);
        //                }
        //                break;
        //            }
        //        case 12:
        //            {
        //                if (client.Inventory.Contain(3005412, 5) || client.Inventory.Contain(3005412, 5, 1))
        //                {
        //                    client.Inventory.Remove(3005412, 5, stream);
        //                    client.Player.InnerPower.AddPotency(stream, client, 50000);
        //                }
        //                break;
        //            }
        //        case 13:
        //            {
        //                if (client.Inventory.Contain(3005412, (ushort)Count) || client.Inventory.Contain(3005412, (ushort)Count, 1))
        //                {
        //                    client.Inventory.Remove(3005412, (ushort)Count, stream);
        //                    client.Player.InnerPower.AddPotency(stream, client, 10000 * (ushort)Count);
        //                }
        //                break;
        //            }


        //    }
        [NpcAttribute(NpcID.PeachTree)]
        public static void PeachTree(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Player.ActivePick)
            {
                if (client.Player.QuestGUI.IsActiveQuest(1011))
                {
                    client.Player.AddPick(stream, "Pick", 5);
                }
            }
        }
        [NpcAttribute(NpcID.PeachTree2)]
        public static void PeachTree2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            if (!client.Player.ActivePick)
            {
                if (client.Player.QuestGUI.IsActiveQuest(1011))
                {
                    client.Player.AddPick(stream, "Pick", 5);
                }
            }
        }
        public static string GetQuestModes(Client.GameClient user, uint UID, byte Level)
        {
            string text = "Level~" + Level.ToString();
            if (user.Player.QuestGUI.CheckQuest(UID, MsgQuestList.QuestListItem.QuestStatus.Finished))
                text = "Completed";
            else if (user.Player.QuestGUI.CheckQuest(UID, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                text = "Accepted";
            return text;
        }
        [NpcAttribute(NpcID.DailyItem1)]
        public static void DailyItem1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 1:
                    {
                        client.Teleport(746, 665, 1015);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
            }
        }

        public static uint DailiHeavenCpsCost(uint Stage)
        {
            if (Stage == 1)
                return 5;
            if (Stage == 2)
                return 20;
            if (Stage == 3)
                return 35;
            if (Stage == 4)
                return 100;
            return 0;
        }
        //ThiefChen
        [NpcAttribute(NpcID.ThiefChen)]
        public static void ThiefChen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1719);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefChen, client.Player.Class, 1720);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefChen, client.Player.Class, 1721);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Beware of the bandits. They`re dangerous.");
                            data.AddOption("Are~you~the~Connector?", 1)
                                .AddOption("Thanks~for~reminding.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                            {
                                data.AddText("Beware of the bandits. They`re dangerous.");
                                data.AddOption("Hi!~Connector!", 4)
                                    .AddOption("Thanks~for~reminding.", 255)
                                   .AddAvatar(72).FinalizeDialog();
                                break;
                            }
                            data.AddText("Beware of the bandits. They`re dangerous.");
                            data.AddOption("I~came~for~Thief~Wong.", 2)
                                .AddOption("Thanks~for~reminding.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("Yes! Here`s a letter that should help. Take it to Wong. I bet he will pay you when he sees what I have to say.");
                            data.AddOption("I~won`t~let~you~down.", 5)
                                .AddOption("I`m~busy.", 255)
                               .AddAvatar(72).FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("Beware of the bandits. They`re dangerous.");
                            data.AddOption("Thanks~for~reminding.", 255)
                               .AddAvatar(72).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("You said you came from Twin City and were looking for Thief Chen? What`s the problem?");
                        data.AddOption("Thief~Wong~sent~me~here.", 2)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver.", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Well, I`m still going to need you to prove yourself. Go kill 5 Bandits, first!");
                        data.AddOption("Piece~of~cake.", 3).AddOption("I`m~leaving.", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Thief~Chen~ran~away~and~hid.~Now~you`re~going~to~find~and~kill~5~Bandits.");

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 1600, Role.Flags.ExperienceEffect.angelwing, true);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver and 1600 EXP.", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Fine. I guess I can trust you, after all.");
                        data.AddOption("Good.", 6)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("Yes! Here`s a letter that should help. Take it to Wong. I bet he will pay you when he sees what I have to say.");
                        data.AddOption("I~won`t~let~you~down.", 5)
                            .AddOption("I`m~busy.", 255)
                           .AddAvatar(72).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why are you still here? Deliver the letter to Wong, right away!");
                            data.AddOption("I~lost~the~letter.", 7)
                                .AddOption("I`m~on~my~way.", 255)
                               .AddAvatar(72).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 729942);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~received~the~Connector~Letter.~Hurry~and~deliver~it~to~Thief~Wong~(347,228).", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(729942, 1))
                        {
                            data.AddText("Hey, friend! Check your bag. The letter is right there!");
                            data.AddOption("Oh.", 255)
                               .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                client.Inventory.Add(stream, 729942);
                                client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                                client.Player.QuestGUI.SendAutoPatcher("You`ve~received~the~Connector~Letter.~Hurry~and~deliver~it~to~Thief~Wong~(347,228).", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ThiefWong)]
        public static void ThiefWong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1718);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1719);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefChen, client.Player.Class, 1721);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ThiefWong, client.Player.Class, 1722);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Here`s~a~letter~for~you!", 10)
                                .AddOption("Good.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Yeah!~Count~me~in!", 1)
                                .AddOption("Leave.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) &&
                            !client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Let~me~help~you!", 3)
                                .AddOption("Leave.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) &&
                            client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && !client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Need~my~help?", 12)
                                .AddOption("Good.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("The poor have always been squeezed by those devious land owners. I can`t just stand by and let it happen!");
                            data.AddOption("Good.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("To tell you the truth, I stole some jewelry from those slum lords, and want to give the proceeds back to the poor in Maple Forest.");
                        data.AddOption("That~would~be~great!", 2)
                            .AddOption("Leave.", 255)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255)
                               .AddAvatar(56).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            data.AddText("To tell you the truth, I stole some jewelry from those slum lords, and want to give the proceeds back to the poor in Maple Forest.");
                            data.AddOption("That~would~be~great!", 3)
                                .AddOption("Leave.", 255)
                               .AddAvatar(56).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("The problem is, I was wounded by the guards as I escaped. Can you take a message to my connector, Thief Chen?")
                            .AddText("~He should be somewhere on the road between Phoenix Castle and Twin City.");
                        data.AddOption("Sure.", 4)
                            .AddOption("Sorry,~I~can`t~help.", 255)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~find~Thief~Chen~on~the~road~between~Phoenix~Castle~and~Twin~City.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
                case 10:
                    {
                        client.Inventory.Remove(729942, 1, stream);
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, Role.Flags.ExperienceEffect.bless.ToString());
                            client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for the letter!");
                        data.AddOption("You`re~welcome.", 11)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        data.AddText("Here`s your reward. Thank you again!");
                        data.AddOption("You`re~welcome.", 12)
                           .AddAvatar(56).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~claim~your~reward~from~Taskmaster~Chang!~It`s~a~necklace!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ZhaoJian)]
        public static void ZhaoJian(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1725);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1))
                            {
                                data.AddText("I am the best at disguise work. As long as you pay, I can disguise you as almost anything! Young, old, pretty, ugly...you name it.")
                                 .AddOption("Whatever.", 255)
                                .AddAvatar(33).FinalizeDialog();
                                break;
                            }
                            data.AddText("I am the best at disguise work. As long as you pay, I can disguise you as almost anything! Young, old, pretty, ugly...you name it.");
                            data.AddOption("I~need~your~help.", 1)
                                .AddOption("Whatever.", 255)
                               .AddAvatar(33).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I am the best at disguise work. As long as you pay, I can disguise you as almost anything! Young, old, pretty, ugly...you name it.")
                                .AddOption("Whatever.", 255)
                               .AddAvatar(33).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, 817, 224, (int)60, 8213);
                        client.Player.QuestGUI.SendAutoPatcher("You~are~disguised~as~a~ravishing~beauty!", 1011, 100, 430, 0);
                        data.AddText("I usually charge a fortune for this work, but as you`re going to rid us of Cloud the Lustful... I suppose I can make an exception.")
                              .AddOption("Thank~you.", 255)
                             .AddAvatar(33).FinalizeDialog();

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.VeteranHong)]
        public static void VeteranHong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1723);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1724);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1725);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.VeteranHong, client.Player.Class, 1726);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                                .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                            data.AddOption("Taskmaster~Chang~sent~me~here.", 1)
                                .AddOption("You~bet!", 255)
                               .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                               .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                                data.AddOption("Anything~I~can~help~you~with?", 4)
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                         .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                                data.AddOption("Talk~about~the~lost~armor.", 10)
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                         .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                                data.AddOption("I`ve~got~something~for~you..", 13)
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                         .AddText("~Now that I am old, I can still fight as well as I could when I was young.")
                                    .AddOption("You~bet!", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                     .AddText("~Now that I am old, I can still fight as well as I could when I was young.")
                                .AddOption("You~bet!", 255)
                               .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes! I`m in trouble now, and I could use your help. Someone has stolen my Spun Gold Armor. Were this found out by the Emperor,")
                               .AddText("~I would be sentenced to death! Please, you must help me! You should find some clues from those Bandits who roam outside the city.");
                        data.AddOption("Got~it.", 2)
                            .AddOption("Spun~Gold~Armor?", 255)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver.", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Yes! I guess it was those bandits who stole my armor. You should find them and try to recover any clues you can. Feel free to use force.");
                        data.AddOption("You~can~count~on~me!", 3)
                            //   .AddOption("Spun~Gold~Armor?", 10)
                            .AddOption("I`m~busy.", 255)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                        {
                            data.AddText("Rumor has it that those bandits who roam outside the city have something to do with this. Go find them and see what they know!");
                            data.AddOption("Dirty~thief!", 6)
                                .AddOption("Who`s~Cloud~the~Lustful?", 7)
                               .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("This is an invitation to the Treasure Appraisal Meeting. Now I`m positive the Spun Gold Armor was stolen by Cloud the Lustful!");
                        data.AddOption("Okay.", 3)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(729971, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);

                            data.AddText("I miss those good old days. We were so valiant, and managed to conquer the whole country. We were legends! Feared and respected!")
                            .AddText("~Now that I am old, I can still fight as well as I could when I was young.");
                            data.AddOption("Talk~about~the~lost~armor.", 10)
                                .AddOption("You~bet!", 255)
                               .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        data.AddText("Cloud the Lustful is the head of those bandits. They are a motley crew... Notorious and capable of committing")
                           .AddText("~every kind of crime conceivable. Now they even have the gall to steal my Spun Gold Armor! Those nasty rogues!");
                        data.AddOption("Indeed.", 255)
                           .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Go~find~some~clues~from~the~Bandits.");

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Player.Money += 1000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, Role.Flags.ExperienceEffect.bless.ToString());
                                client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,000 Silver!", MsgMessage.ChatMode.System);
                            }
                            data.AddText("Thank you! I thought I would never regain my precious armor. Please, let me repay you for your heroic feat.");
                            data.AddOption("My~pleasure.", 13)
                               .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("He usually shows up around a small altar in the southwest of Phoenix Castle. He is a randy old man, and actually quite disgusting.")
                            .AddText("~The only way to attract the attention of Cloud the Lustful is beautiful girls. Please take these offerings and find Zhao Jian,")
                            .AddText("~the Disguiser (200,325). He will disguise you as an astounding beauty.");
                        //data.AddOption("I`m~setting~off.", 11)
                        data.AddOption("Gimme~some~offerings.", 12)
                        .AddOption("It`s~crazy.", 255)
                       .AddAvatar(91).FinalizeDialog();

                        break;
                    }

                case 12:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(729972, 2))
                        {
                            data.AddText("Are you kidding me? You`ve already got the offering!");
                            data.AddOption("Let~me~see.", 255).AddAvatar(91).FinalizeDialog();
                            //Are you kidding me? You`ve already got the offering!
                            break;
                        }
                        client.CreateBoxDialog("You`ve~got~the~offering.~Now~go~visit~Disguiser~Zhao~(200,325)~for~help.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 729972);

                        break;
                    }
                case 13:
                    {
                        data.AddText("Thank you for retrieving my Spun Gold Armor. Now you can speak with Taskmaster Chang, to claim your reward.");
                        data.AddOption("Great!", 255)
                           .AddAvatar(91).FinalizeDialog();
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~claim~your~reward~from~Taskmaster~Chang!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ScholarWu)]
        public static void ScholarWu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1727);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1728);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1731);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1732);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                            data.AddOption("Did~you~go~to~see~a~doctor?", 1)
                                .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                    .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("Maybe~I~can~help?", 3)
                                    .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                    .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("I`ll~try~my~best~to~help~you!", 7)
                                    .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                    .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("I`ll~try~my~best~to~help~you!", 8)
                                    .AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                                   .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                                data.AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Here it is, the Phoenix Castle. Well, this asthma... it has been on and off for years... Terrible, I`m telling you... They")
                               .AddText("~said I could find some good pills, here... I do hope so... it`s killing me...");
                            data.AddOption("Good~luck,~buddy.", 255).AddAvatar(40).FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        data.AddText("Hats off to your valor! You saved my life! Please accept this, with my thanks.");
                        data.AddOption("Thank~you.", 8).AddAvatar(40).FinalizeDialog();


                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(200, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You've received EXP worth 1/3 EXP Ball and 1,500 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you! You saved me! Now go claim your reward from Taskmaster Chang!");
                        data.AddOption("I`m~on~the~way.", 9).AddOption("Later.", 255).AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~and~claim~your~reward~from~Taskmaster~Chang!~It`s~a~ring!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes. But sigh... I still haven`t gotten over it...");
                        data.AddOption("Sorry~to~hear~that.", 2).AddAvatar(40).FinalizeDialog();


                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);
                        }

                        goto case 3;
                    }
                case 3:
                    {
                        data.AddText("Thank you. I heard there is a rat called the Fire Rat L38 outside the castle... Its eye and meat could be used to make a powerful")
                            .AddText("~pill, called the Azure Pill. But those rats are ferocious... Will you be so kind as to help me with this?");
                        data.AddOption("Sure!", 4).AddOption("I`m~afraid~I~can`t~help.", 255).AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 4:
                    {

                        data.AddText("Well, you are so generous... thank you. The recipe for the Azure Pill belongs to Pharmacist Mu Mu (182,241). Please go there")
                            .AddText("~immediately... Oh, my throat...");
                        data.AddOption("I`m~off~right~now.", 5).AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Head~to~visit~Pharmacist~Mu~Mu~(182,241).", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BianQing)]
        public static void BianQing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1729);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 2) && client.Inventory.Contain(721697, 2))
                            {
                                data.AddText("Oh... look who`s coming? I have anything you want. Just name it.");
                                data.AddOption("You~bet!", 255).AddAvatar(55).FinalizeDialog();
                                break;
                            }
                            data.AddText("Oh... look who`s coming? I have anything you want. Just name it.");
                            data.AddOption("You~knew~the~Azure~Pill?", 1)
                                .AddOption("You~bet!", 255).AddAvatar(55).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Oh... look who`s coming? I have anything you want. Just name it.");
                            data.AddOption("You~bet!", 255).AddAvatar(55).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes, sure. So my old friend Mu Mu sent you here? Let me see... you need to collect 1 Ratling Eye and 3 Ratling Meat, yourself,")
                            .AddText("~but I will give you the 2 Herb Packs for free. He must need them to make some medicine. Here you are.");
                        data.AddOption("Thanks.", 2).AddAvatar(55).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.");
                            data.AddOption("Let me check.", 255).AddAvatar(55).FinalizeDialog();
                            break;
                        }
                        client.Inventory.AddItemWitchStack(721697, 0, 2, stream);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 2);
                        client.CreateBoxDialog("You~receive~2~Herb~Packs.~You~still~need~1~Ratling~Eye~and~3~Ratling~Meat!");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PharmacistMuMu)]
        public static void PharmacistMuMu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ScholarWu, client.Player.Class, 1728);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1729);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1730);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistMuMu, client.Player.Class, 1731);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("You~knew~Scholar~Wu?", 1)
                                .AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                                data.AddOption("What~about~the~Azure~Pill?", 4)
                                    .AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                                break;
                            }
                            data.AddText("Scholar Wu is suffering from a rare asthma. Only the Azure Pill can cure it. I need 1 Ratling Eye, 3 Ratling Meat and")
                                .AddText("~2 Herb Packs. Kill Ratlings and you`ll have a chance to get what I need. You can ask Bian Qing (Twin City 342,232) for the packs.");
                            data.AddOption("I`m~on~my~way.", 3)
                                .AddOption("None~of~my~business.", 255).AddAvatar(62).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("It will take a while to make the Azure Pill. Be patient.");
                                data.AddOption("Alright.", 6).AddAvatar(62).FinalizeDialog();
                            }
                            else
                            {
                                if (DateTime.Now > client.Player.AzurePillStamp)
                                {
                                    data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                                    data.AddOption("How`s~going~with~the~pill?", 9).AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                                    data.AddOption("How`s~going~with~the~pill?", 8).AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                                }

                            }
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("Any~news~about~the~Azure~Pill?", 14).AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`ve spent most of my life travelling around, you know. Collecting medicine formulas and curing people. Trust me, I`m the best.");
                            data.AddOption("Indeed.", 255).AddAvatar(62).FinalizeDialog();
                        }

                        break;
                    }
                case 14:
                    {
                        data.AddText("Finally, the Azure Pill is done. Hurry up, bring it to Scholar Wu.");
                        data.AddOption("I`ll~set~off~right~away.", 15).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.Contain(721698, 1))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.");
                                data.AddOption("Let me check.", 255).AddAvatar(62).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721698);
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~got~the~Azure~Pill!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest4.MissionId, 1);

                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.");
                            data.AddOption("Let me check.", 255).AddAvatar(62).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                        data.AddText("Haha, the Azure Pill is done!");
                        data.AddOption("Perfect!", 10).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        data.AddText("Finally, the Azure Pill is done. Hurry up, bring it to Scholar Wu");
                        data.AddOption("I`ll~set~off~right~away.", 11).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.Inventory.Add(stream, 721698);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~got~the~Azure~Pill!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest4.MissionId, 1);

                        break;
                    }
                case 8:
                    {
                        data.AddText("Don`t bother me, right now. Get some manners and be patient, OK?");
                        data.AddOption("I`ll~wait.", 255).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("Oh, Scholar Wu`s disease... It`s difficult to cure. I`ll try my best, but I`ll need your help gathering things I need.");
                        data.AddOption("Go~ahead.", 2).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Scholar Wu is suffering from a rare asthma. Only the Azure Pill can cure it. I need 1 Ratling Eye, 3 Ratling Meat and")
                                .AddText("~2 Herb Packs. Kill Ratlings and you`ll have a chance to get what I need. You can ask Bian Qing (Twin City 342,232) for the packs.");
                        data.AddOption("I`m~on~my~way.", 3)
                            .AddOption("None~of~my~business.", 255).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Hurry~to~collect~1~Ratling~Eye,~3~Ratling~Meat,~and~buy~2~Herb~Packs~from~Bian~Qing~(Twin~City~342,232)!");

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 3, 2))
                        {
                            data.AddText("Excellent! All of the ingredients are here. Now leave me alone for a while so I can combine them into an Azure Pill.");
                            data.AddOption("Cool.", 5).AddAvatar(62).FinalizeDialog();


                            break;
                        }
                        data.AddText("Now, bring me 1 Ratling Eye and 3 Ratling Meat. Make sure to get the 2 Herb Packs from Bian Qing (342,232).");
                        data.AddOption("I~see", 255).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(721695, 1, stream);
                            client.Inventory.Remove(721696, 3, stream);
                            client.Inventory.Remove(721697, 2, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("It will take a while to make the Azure Pill. Be patient.");
                        data.AddOption("Alright.", 6).AddAvatar(62).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.AzurePillStamp = DateTime.Now.AddSeconds(30);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Just~give~me~30~seconds!");

                        ActionQuery action = new ActionQuery();
                        action.ObjId = client.Player.UID;
                        action.Type = ActionType.CountDown;
                        action.dwParam = (uint)30;
                        client.Send(stream.ActionCreate(action));

                        break;
                    }
            }
        }
        //Level43UniqueRingPack
        [NpcAttribute(NpcID.Level43UniqueRingPack)]
        public static void Level43UniqueRingPack(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            client.Inventory.Remove(728238, 1, stream);
            switch (Option)
            {
                case 1:
                    {
                        client.Inventory.Add(stream, 151077);
                        client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Ivory~Heavy~Ring!");

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Add(stream, 150077);
                        client.CreateBoxDialog("You~opened~the~pack~and~received~a~Unique~Amethyst~Ring!");

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ApprenticeLuo)]
        public static void ApprenticeLuo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1733);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ApprenticeLuo, client.Player.Class, 1734);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Ever wonder who makes the best arrows in the world? It`s my dad!")
                                .AddText("~Those arrows... So delicate, but they can fly so steadily...");
                            data.AddOption("Your~dad?", 1).AddOption("You~are~adorable.", 255).AddAvatar(68).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you met my father on the Wind Plain (442,431)? I`m sure he`ll")
                                    .AddText("~bring back lots of beautiful feathers.");
                                data.AddOption("Yeah.", 255).AddAvatar(68).FinalizeDialog();
                                break;
                            }
                            data.AddText("Have you ever met my father, Artisan Luo? He makes arrows, and has been off at the Wind Plain Bridge")
                            .AddText("~(442,431) collecting feathers. It usually only takes him one or two days, but it`s been almost a week")
                            .AddText("~since he left, this time. My mother and I are so very worried. I hope he`s alright...");
                            data.AddOption("River?", 2).AddOption("Sorry,~I~can`t~help.", 255).AddAvatar(68).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Have you met my father on the Wind Plain (442,431)? I`m sure he`ll")
                                   .AddText("~bring back lots of beautiful feathers.");
                            data.AddOption("Yeah.", 255).AddAvatar(68).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Have you ever met my father, Artisan Luo? He makes arrows, and has been off at the Wind Plain Bridge")
                              .AddText("~(442,431) collecting feathers. It usually only takes him one or two days, but it`s been almost a week")
                              .AddText("~since he left, this time. My mother and I are so very worried. I hope he`s alright...");
                        data.AddOption("River?", 2).AddOption("Sorry,~I~can`t~help.", 255).AddAvatar(68).FinalizeDialog();

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
                        data.AddText("He should be near the river between the Peach Blossom Garden and Wind Plain. There are many birds roaming around.")
                             .AddText("~My dad used to take me there to collect feathers, so I`m sure that`s where he`ll be.");
                        data.AddOption("Got~it.", 3).AddOption("Sorry,~I~can`t~help.", 255).AddAvatar(68).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~the~Riverside~(442,431)!", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ArtisanLuo)]
        public static void ArtisanLuo(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1010);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ApprenticeLuo, client.Player.Class, 1734);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1735);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1736);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArtisanLuo, client.Player.Class, 1737);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                  .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                                .AddOption("I~came~for~your~son.", 10)
                                .AddOption("Oh~no,~thanks.", 255)
                                .AddAvatar(51).FinalizeDialog();

                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                              .AddOption("I`ve~got~the~feathers.", 11)
                              .AddOption("Oh~no,~thanks.", 255)
                              .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                               .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                             .AddOption("I`ve~got~the~Vulture~Feather.", 13)
                             .AddOption("Oh~no,~thanks.", 255)
                             .AddAvatar(51).FinalizeDialog();


                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                               .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                             .AddOption("Let~me~know~if~you~need~help.", 16)
                             .AddOption("Oh~no,~thanks.", 255)
                             .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")

                              .AddOption("Oh~no,~thanks.", 255)
                              .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 10))
                            {
                                data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                    .AddText(" ~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                              .AddOption("Feather~Collection.", 4)
                              .AddOption(" Oh~no,~thanks.", 255)
                              .AddAvatar(51).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText(" I ran out of the materials I use to make arrows. Can you hunt the Robins and collect 10 Robin Feathers for me?")
                               .AddOption("Here~you~are!", 3)
                               .AddOption("I`ll~go~now.", 255)
                               .AddAvatar(51).FinalizeDialog();
                            }
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I am an artisan from Twin City. It`s my job to make exquisite arrows for hunters. The best arrow is a")
                                .AddText("~perfect combination of a sharp arrowhead, solid shaft and brilliant feathers.")
                                .AddOption("Feather~Collection.", 1)
                                .AddOption("Oh~no,~thanks.", 255)
                                .AddAvatar(51).FinalizeDialog();
                            break;
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Oh, my son is looking for me? Alright, I`ll be able to head home as soon as I can gather enough feathers.")
                          .AddOption("You~crazy.", 11)
                          .AddOption("Fine.", 255)
                          .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 100))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                            {
                                client.Player.Money += 100;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                            }
                            data.AddText("That`s very kind of you! Thank you!")
                                .AddOption("No~problem.", 13)
                                .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        data.AddText("Ouch...my foot! I accidentally sprained my ankle while I was trapping a gigantic bird, so it`s hard for")
                            .AddText("~me to keep collecting feathers. Would you be so kind as to help me collect 100 feathers?")
                            .AddText("~I would really appreciate your help.")
                         .AddOption("My~pleasure.", 12)
                         .AddOption("Sorry,~I~can`t~help.", 255)
                         .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 13:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            //You`ve~found~a~Vulture~Feather.
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("Thank you for helping me! Take this as your reward. You deserve it.")
                                    .AddOption("My~pleasure.", 16)
                            .AddAvatar(51).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.AddText("The Vultures are nesting in the Love Canyon, but be careful! They`re very dangerous.").AddOption("I~see.", 255)
                              .AddAvatar(51).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("One last thing... I`ve been dreaming of getting my hands on some of the native vulture`s feathers.")
                            .AddText("~Perhaps you could help me out one more time?")
                                  .AddOption("I`ll~try~my~best.", 14).AddOption("I`m~busy,~you~know.", 255)
                                  .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 16:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(729976, 1, stream);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,500 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Thank you for what you`ve done for me! Please pay a visit to Taskmaster Chang and claim your reward.")
                               .AddOption("I~will.", 255)
                       .AddAvatar(51).FinalizeDialog();
                            break;
                        }
                        data.AddText("Thank you for helping me. Now you can go pick a reward from Taskmaster Chang.")
                                .AddOption("Really?~I`m~on~my~way.", 17)
                                .AddOption("Got~it.", 255)
                        .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~claim~your~reward~from~Taskmaster~Chang.~It`s~a~weapon!", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 14:
                    {
                        data.AddText("You certainly deserve your good reputation! I heard the strange birds from the Love Canyon can cast magic spells")
                         .AddText("~and turn human beings into monkeys! I could never risk going, but maybe you could? Do you think you could collect")
                         .AddText("~a feather from one of the vultures for me?")
                               .AddOption("You~can~count~on~me.", 15).AddOption("I`m~busy,~you~know.", 255)
                               .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 15:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~the~Love~Canyon~to~hunt~the~Vultures.", 1020, 586, 771, 0);

                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Hurry~to~collect~100~feathers!");

                        break;
                    }
                case 4:
                    {
                        data.AddText("I ran out of the materials I use to make arrows. Can you hunt the Robins and collect 10 Robin Feathers for me?")
                                 .AddOption("Here~you~are!", 5)
                                 .AddOption("I`ll~go~now.", 255)
                                 .AddAvatar(51).FinalizeDialog();


                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You~finished~the~quest~Feather~Collection~and~received~400~silver~and~EXP~worth~1/15~of~an~EXP~Ball.");
                            client.Player.Money += 400;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(10, true, Role.Flags.ExperienceEffect.angelwing);
                            data.AddText("Thank you very much for collecting the Robin Feathers for me! I can make good arrows with it!")
                               .AddOption("You~are~welcome.", 255)
                               .AddAvatar(51).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Hey, I don`t see any feathers on you. Don`t cheat me!")
                               .AddOption("I`ll~go~now.", 255)
                               .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText(" Hi! I`m the fletcher of Twin City. It`s my job to make exquisite arrows. Now, I`m out of the material I need to make arrows. Can you")
                              .AddText("~hunt some Robins and collect 10 Robin Feathers for me?")
                              .AddText("~I need those feathers, or I can`t make any arrows!")
                              .AddOption("No~problem.", 2)
                              .AddOption("I~have~other~things~to~do.", 255)
                              .AddAvatar(51).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~hunt~Robins~and~collect~10~Robin~Feathers.", 1002, 577, 718, 0);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ShenJunior)]
        public static void ShenJunior(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1742);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("So my father asked you to visit me? It is very kind of you. I really miss him.")
                               .AddOption("You~need~a~Gate~scroll?", 1)
                               .AddOption("Never~mind.", 255)
                               .AddAvatar(27).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("So my father asked you to visit me? It is very kind of you. I really miss him.")
                             .AddOption("Never~mind.", 255)
                             .AddAvatar(27).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Thank you for your help. This means a lot to me, since I`ll finally be able to go home and get a break!")
                             .AddOption("Take~care.", 2)
                             .AddAvatar(27).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Inventory.Remove(721710, 1, stream);
                        client.Player.QuestGUI.SendAutoPatcher(1011, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DealerShen)]
        public static void DealerShen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1739);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1740);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1741);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1742);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1743);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                               .AddOption("Are~you~Dealer~Shen?", 1)
                               .AddOption("That`s~interesting.", 255)
                               .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("I`ve~got~what~you~need.", 3)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                      && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("I~finished~the~Ratlings.", 5)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                     && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("I`m~always~at~your~service.", 7)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                  && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                             .AddOption("Your~son~is~back.", 10)
                             .AddOption("That`s~interesting.", 255)
                             .AddAvatar(85).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Only the most glorious treasures will stand the test of time. Don`t laugh, young man. I`ve set my mind to unveiling their beauty!")
                          .AddOption("That`s~interesting.", 255)
                          .AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Yes! Nice to see you! I love collecting antiques. They`ve been my biggest passion in life.")
                              .AddOption("I~want~your~Antique~Ring.", 2)
                              .AddAvatar(85).FinalizeDialog();

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
                        data.AddText("Oh, I could have helped you... But... you know... the Antique Ring... that`s valuable...")
                         .AddOption("What~do~you~want?", 3)
                         .AddOption("Never~mind.", 255)
                         .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 12, 12))
                        {
                            data.AddText("Thank you. I`ll get them to take their medicine, now.")
                        .AddOption("Take~your~time.", 5)
                        .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        data.AddText("You`re smart. To be honest with you, I could use some help. I need 12 Stanchers and 12 Agrypnotics to help with my family`s illness.")
                            .AddText("~You can buy them from the Pharmacist, but it`s too far away. I need to stay close to my family, right now.")
                      .AddOption("I`ll~be~right~back.", 4)
                      .AddOption("Later.", 255)
                      .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            foreach (var item in client.Inventory.ClientItems.Values)
                            {
                                if (item.ITEM_ID == 1000000)
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 1);
                                else if (item.ITEM_ID == 1001000)
                                    client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 0, 1);
                            }
                        }
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~buy~12~Stanchers~and~12~Agrypnotics!", 1011, 189, 252, 10014);

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(1000000, 12, stream);
                            client.Inventory.Remove(1001000, 12, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 20))
                            {
                                data.AddText("Thank you for what you`ve done for the village. I won`t soon forget your heroism!")
                                   .AddOption("You`re~welcome.", 7)
                                   .AddAvatar(85).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("How come you didn`t kill enough Ratlings?")
                                    .AddOption("Sorry,~I`ll~try~my~best.", 255)
                                    .AddAvatar(85).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("Oh, thank you ,thank you... but... Will you please kill 20 Ratlings, too? They are really quite annoying, nibbling our food all the time.")
                       .AddOption("Okay.", 6)
                       .AddOption("It`s~boring.", 255)
                       .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~kill~20~Ratlings!", 1011, 610, 429, 0);

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("My son is back! How can I thank you enough!")
                   .AddOption("You`re~welcome.", 10)
                   .AddAvatar(85).FinalizeDialog();
                                break;
                            }
                            data.AddText("Have you heard the news? Recently, many people were robbed by the Bandits who hide in Maple Forest.")
                           .AddText("~I`m worried about my son. He`s working inside the Mine Cave (116,164), so he doesn`t get much news from outside.")
                   .AddOption("I`ll~take~him~home.", 255)
                   .AddOption("I~lost~the~scroll.", 9)
                   .AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        data.AddText("Can you please head into Maple Forest and deliver this Castle Gate Scroll to my son?")
                            .AddText("~His name is Shen Junior, and he`s doing excavation work inside the Mine Cave (116,164).")
                    .AddOption("Sure.", 8)
                    .AddOption("Not~now.", 255)
                    .AddAvatar(85).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721710, 1))
                            client.Inventory.Add(stream, 721710);

                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Deliver~the~Castle~Gate~Scroll~to~Shen~Junior!", 1011, 920, 552, 4502);

                        break;
                    }
                case 9:
                    {
                        if (!client.Inventory.Contain(721710, 1))
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721710);
                            client.CreateBoxDialog("Deliver~the~Castle~Gate~Scroll~to~Shen~Junior!");
                        }
                        else
                        {
                            data.AddText("You have the Scroll. Hurry up and deliver it to my son.");
                            data.AddOption("Okay.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("With my family finally together and safe, the ring is not important to me, anymore. Please, accept it as your reward.");
                        data.AddOption("Thank~you.", 11)
                            .AddOption("I`ll~take~it~later.", 255).AddAvatar(85).FinalizeDialog();


                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721708, 1))
                            client.Inventory.Add(stream, 721708);
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~got~the~Antique~Ring.~Hurry~and~bring~it~to~the~Minstrel.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.Minstrel)]
        public static void Minstrel(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1738);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1739);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DealerShen, client.Player.Class, 1743);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Minstrel, client.Player.Class, 1744);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                               .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                               .AddOption("Taskmaster~Chang~sent~me~here.", 1)
                               .AddOption("Uh...~Are~you~okay?", 255)
                               .AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                               .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                               .AddOption("Anything~I~can~help~with?", 2)
                               .AddOption("Uh...~Are~you~okay?", 255)
                               .AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                        .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                        .AddOption("What~about~the~Mystic~Ring?", 4)
                        .AddOption("Interesting.", 255)
                        .AddAvatar(40).FinalizeDialog();

                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                               .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                               .AddOption("Anything~I~can~help~with?", 7)
                               .AddOption("Uh...~Are~you~okay?", 255)
                               .AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Well, what a wonderful world! So many intricate treasures, unbreakable weapons, a panacea for immortality... I`ve had some ups and")
                            .AddText("~downs in my life, you know, but I have never stopped loving what I do. How can anyone resist their natural interests?")
                            .AddOption("Uh...~Are~you~okay?", 255)
                            .AddAvatar(40).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        data.AddText("I heard Antique Dealer Shen has an Antique Ring which could be converted into Mystic Ring. But seems that I can`t even see it...")
                              .AddOption("What~a~pity!", 2)
                              .AddAvatar(40).FinalizeDialog();

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
                        data.AddText("A little bird told me an antique dealer named Shen (779,477) is living in a village near Phoenix Castle, and has an Antique Ring. His")
                            .AddText("~ring matches the description of the Mystic Ring! I must satisfy my curiosity and find out if it`s real! Will you check it out for me?")
                             .AddOption("No~problem.", 3)
                             .AddOption("Why~me?", 255)
                             .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Ask~Dealer~Shen~(Phoenix~Castle~779,477)~about~the~Antique~Ring.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
                case 4:
                    {

                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1))
                            {
                                data.AddText("Oh gosh, the Mystic Ring... it`s no different than an ordinary one. But you truly did a great thing.")
                                    .AddText("~So, my hero, will you wear this ring and show me its power?");
                                data.AddOption("Of~course.", 7).AddAvatar(40).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please use the AntiqueRing.");
                                data.AddOption("Let me check.", 255).AddOption("I~lost~the~Broken~Ring.", 8).AddAvatar(40).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Finally, the Mystic Ring! Hats off to your gallantry and valor! Now, will you")
                        .AddText("~do me another favor and try it on? I want to see its legendary power unleashed!")
                        .AddOption("Sure.", 5)
                        .AddOption("Sorry,~I~have~to~go.", 255)
                        .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("Finally, the Mystic Ring! I can`t wait to see its magic! Please, wear it and show me its power! Right now!")
                       .AddOption("Fine.", 6)
                       .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(40).FinalizeDialog();
                            break;
                        }
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Inventory.Remove(721708, 1, stream);
                        client.Inventory.Add(stream, 721707);
                        client.CreateBoxDialog("You~found~the~ring~is~broken!");

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(100, true, Role.Flags.ExperienceEffect.angelwing);
                            client.SendSysMesage("You've received EXP worth 1/6 EXP Ball and 1,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for what you`ve done for me! Now you can report to Taskmaster Chang and claim your reward.")
                            .AddOption("I`ll~go~right~away.", 9)
                            .AddAvatar(40).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (!client.Inventory.Contain(721707, 1))
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(85).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721707);
                            client.CreateBoxDialog("You~found~the~ring~is~broken!");
                        }
                        else
                        {
                            data.AddText("You have the ring.");
                            data.AddOption("Okay.", 255).AddAvatar(85).FinalizeDialog();
                        }

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hurry~to~claim~your~reward~from~Taskmaster~Chang.~It`s~some~headgear!", ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.DoctorLi)]
        public static void DoctorLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1747);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorLi, client.Player.Class, 1748);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorLi, client.Player.Class, 1749);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                               .AddOption("You~know~the~frantic~monkeys?", 1)
                               .AddOption("I`ll~leave~you~alone.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                             .AddOption("What~shall~I~do~now?", 4)
                             .AddOption("I`ll~leave~you~alone.", 255)
                             .AddAvatar(67).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                             .AddOption("I`m~back.", 7)
                             .AddOption("I`ll~leave~you~alone.", 255)
                             .AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("What are you doing here? Don`t you ever mess around with me... I`m old, but not foolish...")
                          .AddOption("I`ll~leave~you~alone.", 255)
                          .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("What? Monkeys are attacking people! I`ve never heard that before!")
                             .AddOption("But~it`s~real.", 2)
                             .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are you sure it`s monkey`s heart? As far as I know, it`s posioned!")
                              .AddOption("Any~solution?", 3)
                              .AddAvatar(67).FinalizeDialog();


                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        client.Inventory.Remove(721699, 1, stream);
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721700, 1))
                            client.Inventory.Add(stream, 721700);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Doctor~Li~gave~you~a~bottle~of~poison.");

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                        {
                            data.AddText("You did go shockingly frantic! Well, your syndrome reminded me of the bites by Venomous Apes.")
                            .AddOption("I~see.", 7)
                            .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Look at this poison...I need someone to test it...")
                            .AddText("~Will you do me this favor?")
                                .AddOption("Sure.", 5)
                                .AddOption("I~lost~the~posion.", 6)
                                .AddOption("Not~me...", 255)
                                .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.OnTransform == false)
                        {
                            client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                            client.Player.TransformInfo.CreateTransform(stream, 817, 356, (int)60, 8213);
                        }
                        else
                        {
                            client.CreateBoxDialog("You~turned~yourself~into~a~gigantic~monkey.~You~can~take~the~poison~now.");
                        }

                        break;
                    }
                case 6:
                    {
                        if (!client.Inventory.Contain(721700, 1))
                        {
                            if (client.Inventory.HaveSpace(1) == false)
                            {
                                data.AddText("Please make 1 more space in your inventory");
                                data.AddOption("Let me check.", 255).AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            client.Inventory.Add(stream, 721700);
                            client.CreateBoxDialog("You~turned~yourself~into~a~gigantic~monkey.~You~can~take~the~poison~now.");
                        }
                        else
                        {
                            data.AddText("Hey! You still have the poison!");
                            data.AddOption("Okay.", 255).AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("The Venomous Apes are harassing people in Love Canyon (795,471) on the outskirts of Ape City.")
                            .AddText("~Please take this Metamorphic Vial along. It`s made of a secret recipe. Use it when you arrive there.")
                           .AddOption("I`m~on~my~way.", 8)
                           .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(721701, 1))
                            client.Inventory.Add(stream, 721701);
                        data.AddText("Venomous Apes live in Love Canyon (795,471). People will turn frantic after being bitten by them. Search there for clues.")
                        .AddOption("I~will.", 9)
                        .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1020, 795, 471, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.OfficerBao)]
        public static void OfficerBao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1745);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1746);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1747);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DoctorLi, client.Player.Class, 1749);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.OfficerBao, client.Player.Class, 1750);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                               .AddText("~What a peaceful view... Or is it?...")
                               .AddOption("Taskmaster~Chang~sent~me~here.", 1)
                               .AddOption("You~really~did~a~good~job.", 255)
                               .AddAvatar(93).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                           .AddText("~What a peaceful view... Or is it?...")
                           .AddOption("About~the~Decayed~Heart.", 3)
                           .AddOption("You~really~did~a~good~job.", 255)
                           .AddAvatar(93).FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                         && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                           .AddText("~What a peaceful view... Or is it?...")
                           .AddOption("How~to~deal~with~the~heart?", 4)
                           .AddOption("You~really~did~a~good~job.", 255)
                           .AddAvatar(93).FinalizeDialog();

                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                         .AddText("~What a peaceful view... Or is it?...")
                         .AddOption("I~found~this~sword.", 7)
                         .AddOption("You~really~did~a~good~job.", 255)
                         .AddAvatar(93).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                       && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                           .AddText("~What a peaceful view... Or is it?...")
                           .AddOption("Was~there~anything~else?", 8)
                           .AddOption("You~really~did~a~good~job.", 255)
                           .AddAvatar(93).FinalizeDialog();

                        }
                        else
                        {
                            data.AddText("I`m the Officer of Ape City. It is well situated... Both picturesque and thriving... People are working very hard...")
                         .AddText("~What a peaceful view... Or is it?...")
                         .AddOption("You~really~did~a~good~job.", 255)
                         .AddAvatar(93).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("It`s weird that the monkeys outside the city strangely went frantic and began to attack villagers...")
                              .AddOption("Really?", 2)
                              .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Yes. You may have noticed those monkeys outside our city. They used to be benign, though sometimes a little bit naughty.")
                            .AddText("~But now they are attacking people... every one of them... Will you give me a hand and see what`s going on there?")
                           .AddOption("My~pleasure.", 3)
                           .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                data.AddText("Thank you for what you`ve done for the people!")
                      .AddOption("Never~mind.", 4)
                      .AddAvatar(93).FinalizeDialog();
                                break;
                            }
                            else
                            {
                                data.AddText("Only a genuine brave heart can conquer all. Please go and find out what`s driving the monkeys berserk. Quick!")
                       .AddOption("I`ll~be~right~back.", 255)
                       .AddAvatar(93).FinalizeDialog();
                            }
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Nothing~ventured,~nothing~gained.~Go~find~reasons~why~the~monkeys~turned~frantic~so~suddenly.", 1020, 668, 673, 0);


                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Dear me! What a horrible heart! It`s decayed. I`ve never seen this kind of thing before. Will you go visit Royal Doctor Li")
                            .AddText("~in Twin City (296,243)? He`s an accomplished doctor. He must be of some help.")
                    .AddOption(" Good~idea.", 5)
                    .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Hand~the~Decayed~Heart~to~Royal~Doctor~Li~in~Twin~City~(296,243).", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 7:
                    {
                        data.AddText("You got the Devilish Sword! I`m really impressed. This is your reward.")
                            .AddOption("Thank~you.", 8)
                            .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've received EXP worth 1/2 EXP Ball and 3,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you for restoring peace to our city. Now you can go claim your reward from Taskmaster Chang.")
                           .AddOption("I`m~on~the~way.", 9)
                           .AddAvatar(93).FinalizeDialog();

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`re~heading~to~see~Taskmaster~Chang.", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralAmber)]
        public static void GeneralAmber(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1751);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1752);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1753);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1754);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1755);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuardLi, client.Player.Class, 1756);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("Was~there~anything~else?", 20)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                               .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                               .AddOption("Taskmaster~Chang~sent~me~here.", 1)
                               .AddOption("Marvellous!", 255)
                               .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                              .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                              .AddOption("Let~me~help~you.", 3)
                              .AddOption("Marvellous!", 255)
                              .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("I~can~help~you.", 5)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("You~said~the~portrait?", 8)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                        && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                            .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                            .AddOption("Here`s~the~portrait.", 10)
                            .AddOption("Marvellous!", 255)
                            .AddAvatar(91).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You are too young to know of me. We weathered the greatest storm. We had our finest hour. Oh, yes, our grand alliance...")
                         .AddText("~When the hinges of fate came, we were closed in a ring of destruction... But a true fighter shall never be vanquished!")
                         .AddOption("Marvellous!", 255)
                         .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest6.MissionId, 1))
                        {
                            data.AddText("You`ve got the Paper Figure. Hurry to find General Zhao!")
                              .AddOption("I`m~on~my~way.", 255)
                              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("Haha! Thank you for allowing me to control you like a pawn! I could not have taken my revenge without you. General Zhao")
                            .AddText("~is now captivated by my enchantment... He is now under my spell! My revenge is complete, all thanks to you!")
                            .AddOption("Stop~it!~Release~him!", 21)
                            .AddOption("Awful.", 255)
                            .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = id,
                            Type = ActionType.RemoveEntity
                        };
                        client.Send(stream.ActionCreate(action));
                        if (client.Player.View.ContainMobInScreen("JadePhantom") == false)
                            Server.AddMapMonster(stream, client.Map, 33212, client.Player.X, client.Player.Y, 8, 8, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        client.CreateBoxDialog("General~Amber~vanished~into~thin~air.~A~vicious~Jade~Phantom~appeared...");

                        break;
                    }
                case 1:
                    {
                        data.AddText("My soul could not rest in peace. I still have some unfinished wishes. Oh, how they are torturing me...")
                            .AddText("~I do need your help.")
                            .AddOption("What~is~it?", 2)
                            .AddOption("Why~me?", 255)
                            .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Find General Zhao (262,247) in Twin City. We used to be best friends. But we had a big fight over some rift")
                         .AddText("~and swore never to meet again. But now... I miss him. Will you take a tuft of Hair of him for me as a token of peace?")
                         .AddOption("Okay.", 3)
                         .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                data.AddText("Oh, my, General Zhao`s Hair... This really reminds me of those days... when we were fighting side by side...")
                                    .AddText("~Will you help me complete my other wishes, too?")
                                  .AddOption("Not~right~now.", 5)
                                   .AddAvatar(91).FinalizeDialog();
                                break;
                            }
                            data.AddText("Pity! I can`t go visit General Zhao (262,247) myself. I do hope everything is fine with him. Will you please bring me")
                                .AddText("~a tuft of his Hair?")
                               .AddOption("Okay.", 255)
                                .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("My soul could not rest in peace. I still have some unfinished wishes. Oh, how they are torturing me...")
                    .AddText("~I do need your help.")
                     .AddOption("I`d~like~to.", 4)
                     .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(721703, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I need 1 Snake Man, 10 Rock Monsters (Level 42) and 10 Giant Apes (Level 53) Smells. Urr... Don`t ask me why. It`s like a")
             .AddText("~BIG surprise! But I will sure repay you handsomely... So, is it a deal?")
              .AddOption("I~need~a~Silk~Pack~for~this.", 7)
              .AddOption("You~crazy.", 255)
              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        data.AddText("I need 1 Snake Man, 10 Rock Monsters (Level 42) and 10 Giant Apes (Level 53) Smells. Urr... Don`t ask me why. It`s like a")
                .AddText("~BIG surprise! But I will sure repay you handsomely... So, is it a deal?")
                 .AddOption("Deal.", 6)
                 .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 7:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 10, 10))
                        {
                            data.AddText("I didn`t expect you would return so soon. This is your reward. Please accept it.")
                               .AddOption("Okay.", 8)
                               .AddAvatar(91).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Hey you, one Silk Pack is enough.")
                                .AddOption("Got~it.", 255)
                                .AddAvatar(91).FinalizeDialog();

                        }

                        break;
                    }
                case 8:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(721704, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                //721705
                                data.AddText("Fantastic! So true to life! But he doesn`t look so well does he? General Zhao looks unhappy... Would you pay him a")
                                    .AddText("~visit for me? I am so worried...")
                                  .AddOption("Okay.", 10)
                                  .AddOption("Later.", 255)
                                  .AddAvatar(91).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Only the Painter Feng Kang (556,571) in Ape City can paint the best portrait. No more waste of time, please.")
                                   .AddOption("All~right.", 255)
                                   .AddAvatar(91).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Ahhh, excellent. You have done well, and I commend you. But, if it`s not too much too ask, I still have one more wish. Will")
                            .AddText("~you find Painter Feng Kang (556,571) in Ape City and ask him to paint a portrait of General Zhao?")
                            .AddText("~I`d love to have such a painting...")
                               .AddOption("Well...~okay~then.", 9)
                               .AddOption("No~way.", 255)
                               .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(721705, 1, stream);
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);
                            goto case 11;
                        }
                        else
                        {
                            data.AddText("Fantastic! So true to life! But he doesn`t look so well does he? General Zhao looks unhappy... Would you pay him a")
                                    .AddText("~visit for me? I am so worried...")
                                  .AddOption("Okay.", 11)
                                  .AddOption("Later.", 255)
                                  .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You~have~agreeed~to~pay~a~visit~to~General~Zhao~for~General~Amber.", 1002, 262, 247, 4488);

                        break;
                    }
                case 9:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Find~Painter~Feng~Kang~(556,571)~in~Ape~City~and~paint~a~full~length~Portrait.", 1020, 556, 571, 4489);

                        //Find~Painter~Feng~Kang~(556,571)~in~Ape~City~and~paint~a~full~length~Portrait.
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(91).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 721704);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("Kill 1 Snake Man, 10 Rock Monsters (Level 42) and 10 Giant Apes (Level 53).");


                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Find~General~Zhao~(262,247)~in~Twin~City~and~take~back~a~tuft~of~his~hair~to~General~Amber.", 1002, 262, 247, 4488);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GuardLi)]
        public static void GuardLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1755);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuardLi, client.Player.Class, 1756);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will ward off any danger away from General Zhao.")
                               .AddOption("About~General~Zhao.", 1)
                               .AddOption("Good~for~you.", 255)
                               .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                  && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I will ward off any danger away from General Zhao.")
                       .AddOption("What`s~wrong~with~Zhao?", 1)
                       .AddOption("Good~for~you.", 255)
                       .AddAvatar(30).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I will ward off any danger away from General Zhao.")
                               .AddOption("Good~for~you.", 255)
                               .AddAvatar(30).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }

                        data.AddText("Wake up! Don`t you realize now? The hair, the silk pack, and the portait...they are all for an enchantment! General Zhao has been")
                            .AddText("~bewitched! How could you take them for granted?! The man you helped is not real General Amber! You must return and kill that horrid phantom!")
                             .AddOption("I`ll~try~my~best.", 2)
                             .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~back~to~kill~the~phantom!", 1020, 865, 540, 4487);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PainterFengKang)]
        public static void PainterFengKang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1754);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Art is not only for art`s sake.")
                               .AddOption("I~came~for~a~portrait.", 1)
                               .AddOption("True.", 255)
                               .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Art is not only for art`s sake.")
                               .AddOption("True.", 255)
                               .AddAvatar(74).FinalizeDialog();
                        }


                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(721705, 1))
                        {
                            data.AddText("Why not send the Portrait to General Amber right now? What`s been dragging you?")
                                     .AddOption("I`ll~hurry~up.", 255)
                                     .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        data.AddText("The revenant of General Amber has sent you here for a Portrait of General Zhao? Ok, I will try my best... But this sure")
                            .AddText("~does sound suspicious!")
                                  .AddOption("Thank~you.", 2)
                                  .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.Inventory.Add(stream, 721705);
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1) == false)
                            client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("You~obtain~a~Portrait.~Hurry~up~to~give~it~to~General~Amber!", ActiveQuest.FinishNpcId.Map, ActiveQuest.FinishNpcId.X, ActiveQuest.FinishNpcId.Y, ActiveQuest.FinishNpcId.ID);


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KungfuMaster)]
        public static void KungfuMaster(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1752);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralAmber, client.Player.Class, 1755);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuardLi, client.Player.Class, 1756);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KungfuMaster, client.Player.Class, 1757);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh, yes. God, my head hurts. Ahhhh, my head!")
                               .AddOption("What`s~wrong~with~you?.", 15)
                               .AddOption("C`est~la~vie.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Oh, yes. God, my head hurts. Ahhhh, my head!")
                               .AddOption("You~must~be~crazy.", 13)
                               .AddOption("C`est~la~vie.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                               .AddOption("I~have~a~request.", 10)
                               .AddOption("C`est~la~vie.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Oh, yes. God, my head hurts. Ahhhh, my head!")
                              .AddOption("Are~you~OK~now?.", 16)
                              .AddOption("C`est~la~vie.", 255)
                              .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.IsActiveQuest(802))
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                                .AddOption("Tell~me~about~the~team~system.", 1)
                                .AddOption("See~you~then!", 255)
                                .AddAvatar(104).FinalizeDialog();
                        }
                        else if (!client.Player.QuestGUI.CheckQuest(807, MsgServer.MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                          .AddOption("Tell~me~about~the~team~system.", 2)
                          .AddOption("See~you~then!", 255)
                          .AddAvatar(104).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                                .AddOption("C`est~la~vie.", 255)
                                .AddAvatar(104).FinalizeDialog();
                        }

                        break;
                    }
                case 15:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                        {
                            data.AddText("What`s wrong with me? It seemed I`ve had a lot of nightmares these days... I see, it`s a spell... an enchantment... You")
                          .AddText("~have defeated Jade Phantom and it turned into a Paper Figure? I really appreciate your help. Thank you, and please take this.")
                              .AddOption("My~pleasure.", 16)
                              .AddOption("Hmm...", 255)
                              .AddAvatar(104).FinalizeDialog();
                        }
                        else
                        {
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.QuestGUI.SendAutoPatcher("Go~back~to~kill~the~phantom!", 1020, 865, 540, 4487);
                        }

                        break;
                    }
                case 16:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(721706, 1, stream);
                            client.Player.Money += 4000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've received EXP worth 2/3 EXP Ball and 4,000 Silver!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Thank you, for saving my life! You can go claim your reward from Taskmaster Chang!")
                         .AddText("~have defeated Jade Phantom and it turned into a Paper Figure? I really appreciate your help. Thank you, and please take this.")
                             .AddOption("I~will", 17)
                             .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 17:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("You`ve~completed~the~quest!~Hurry~to~claim~your~reward~from~Taskmaster~Chang!", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);


                        break;
                    }
                case 13:
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("General~Zhao~is~delirious.~Maybe~Guard~Li~(261,243)~will~know~why.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(721703, 1))
                        {
                            data.AddText("Didn`t I just give you my Hair?")
                               .AddOption("Oh,~yeah.", 255)
                               .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        data.AddText("Oh... General Amber missed me... I`m surprised... He always hated me so much. What would make him change his mind?")
                            .AddText("~Take this tuft of Hair back to him, as a token of trust.")
                                .AddOption("Thank~you.", 11)
                                .AddOption("Hmm...", 255)
                                .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Inventory.HaveSpace(1) == false)
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                              .AddOption("Let me check.", 255)
                              .AddAvatar(104).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 721703);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest2.MissionId, 1);
                        client.Player.QuestGUI.SendAutoPatcher("You~got~General~Zhao`s~Hair.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);


                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(802))
                            client.GainExpBall(200);
                        data.AddText("I`m telling you! Don`t you ever put all your eggs in one basket. Life is so unpredictable..")
                                .AddOption("Tell~me~about~the~team~system.", 2)
                                .AddOption("See~you~then!", 255)
                                .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("You can`t just venture out alone in this cruel world, the team system will help you to find some companions to help you! There are two methods to help you to find companions. The first one is to click team button to create a team by yourself and invite")
                            .AddText("~other people to join.")
                            .AddOption("What~about~the~other~one?", 3)
                            .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("The second method is to click team button and click join to apply to join a team. You can join the team if there are less than 5 members in it, after the team leader agrees.")
                           .AddOption("What~are~the~benefits?", 4)
                           .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("You will receive bonus experience when slaying monsters in a team. If there is a new player in the team, the team leader will receive Virtue Points when that player levels up. Furthermore, some special quests require players to join as a team.")
                             .AddOption("Great!", 5)
                             .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        data.AddText("There are many things for you to learn in this world! Um, I guess you want to learn more, huh? Let me see. What about equipment and armors? That will be useful! Now speak with the Armor in Twin City, he will tell you something about the armors.")
                       .AddOption("Wonderful!", 6)
                       .AddOption("Other~things!", 255)
                       .AddAvatar(104).FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        var next_quest = Database.QuestInfo.GetFinishQuest((uint)id, client.Player.Class);
                        if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                            next_quest = Database.QuestInfo.GetFinishQuest((uint)2001, client.Player.Class, 3750);
                        client.Player.QuestGUI.Accept(next_quest, 0);
                        client.Player.QuestGUI.SendAutoPatcher("General~Zhao~asked~you~to~speak~with~the~Armorer~in~Twin~City!", next_quest.FinishNpcId.Map, next_quest.FinishNpcId.X, next_quest.FinishNpcId.Y, next_quest.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PurplePowder)]
        public static void PurplePowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729989, 1))
                            {
                                client.Inventory.Add(stream, 729989);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Indigo~Purple~from~this~purple~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an purple halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an purple halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.IndigoPowder)]
        public static void IndigoPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729988, 1))
                            {
                                client.Inventory.Add(stream, 729988);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Indigo~Powder~from~this~indigo~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an indigo halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an indigo halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BluePowder)]
        public static void BluePowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729987, 1))
                            {
                                client.Inventory.Add(stream, 729987);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Blue~Powder~from~this~blue~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an blue halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an blue halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GreenPowder)]
        public static void GreenPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729986, 1))
                            {
                                client.Inventory.Add(stream, 729986);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Green~Powder~from~this~green~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an yellow halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an green halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.YellowPowder)]
        public static void YellowPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729985, 1))
                            {
                                client.Inventory.Add(stream, 729985);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Yellow~Powder~from~this~yellow~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an yellow halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an red yellow around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OrangePowder)]
        public static void OrangePowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729984, 1))
                            {
                                client.Inventory.Add(stream, 729984);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }
                                client.CreateBoxDialog("You`ve~collected~Orange~Powder~from~this~orange~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an orange halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an red orange around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.PharmacistDong)]
        public static void PharmacistDong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 1784);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I need someone to kill those monsters. They must pay for their sins.")
                               .AddOption("Snake~Men!", 1)
                               .AddOption("Not~me.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I need someone to kill those monsters. They must pay for their sins.")
                                .AddOption("About~the~Rainbow~Powder.", 2)
                                .AddOption("Not~interested.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("I need someone to kill those monsters. They must pay for their sins.")
                                .AddOption("Not~interested.", 255)
                                .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("Snake Men? Yes, I`ve heard of it. People get lunatic after being bitten by them. It may have been because of their")
                            .AddText("~Venomous Fang. But a true doctor never makes wild guesses. Wait a moment. Let me extract the toxin from the")
                            .AddText("~fang and see what that is.")
                              .AddOption("Good.~A~real~diagnosis.", 2)
                              .AddOption("About~these~Snake~Men...", 255)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(3))
                            {
                                data.AddText("Please make 3 more spaces in your inventory.")
                                    .AddOption("Let me chieck.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729979, 1))
                                client.Inventory.Add(stream, 729979);
                            if (!client.Inventory.Contain(729980, 1))
                                client.Inventory.Add(stream, 729980);
                            if (!client.Inventory.Contain(729981, 1))
                                client.Inventory.Add(stream, 729981);

                            data.AddText("Herbs have a variety of uses, including culinary, medicinal, or in some cases even spiritual meditation.")
                              .AddText("~Nobody can truly master them, even after years of cultivation. Nature`s secrets are even more obscure than I could possibly imagine.")
                                .AddOption("You~bet.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("Now we need Rainbow Powder to relieve Du San`s syndrome. It`s made of powder of World different colors.")
                            .AddText("~I believe these can be found in this Kun Lun place... I`ll give you 3 toxins, and you can use them to reach Kun Lun.")
                            .AddText("~So, what`s your opinion?")
                              .AddOption("I`ll~do~it!", 3)
                              .AddOption("It`s~dangerous.", 255)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Let me chieck.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        data.AddText("No more delays. Take the toxins now. Pick up two of them and right click to use. If your choice is right, you will arrive in Kun Lun and")
                               .AddText("~find those rock powders... They must be some type of sedimentary rock... Go collect the powders and be careful!")
                                 .AddOption("Alright.", 255)
                                 .AddAvatar(67).FinalizeDialog();

                        client.Inventory.Add(stream, 729979);
                        client.Inventory.Add(stream, 729980);
                        client.Inventory.Add(stream, 729981);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Pharmacist~Dong~gave~you~3~toxins.");

                        //Pharmacist~Dong~gave~you~3~toxins.
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.RedPowder)]
        public static void RedPowder(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255).FinalizeDialog();
                                break;
                            }
                            if (!client.Inventory.Contain(729983, 1))
                            {
                                client.Inventory.Add(stream, 729983);
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                if (client.Inventory.Contain(729983, 1) && client.Inventory.Contain(729984, 1) && client.Inventory.Contain(729985, 1)
                                    && client.Inventory.Contain(729986, 1) && client.Inventory.Contain(729987, 1) && client.Inventory.Contain(729988, 1)
                                    && client.Inventory.Contain(729989, 1))
                                {
                                    client.CreateBoxDialog("You`ve~collected~all~the~powers~needed.~Now~open~your~inventory~to~compose~them~into~the~Rainbow~Power.");
                                    break;
                                }

                                client.CreateBoxDialog("You`ve~collected~Red~Powder~from~this~red~rock.");
                            }
                            else
                            {
                                data.AddText("The rock has an red halo around it.").FinalizeDialog();
                            }
                            break;
                        }
                        else
                            data.AddText("The rock has an red halo around it.").FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralJudd)]
        public static void GeneralJudd(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1782);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 1783);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 1784);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralJudd, client.Player.Class, 710);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                                .AddText("~country needs you.")
                               .AddOption("I~have~no~time~to~play.", 2)
                               .AddOption("Brilliant.", 255)
                               .AddAvatar(95).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                              .AddText("~country needs you.")
                             .AddOption("News~about~Du~San.", 2)
                             .AddOption("Brilliant.", 255)
                             .AddAvatar(95).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
             && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                              .AddText("~country needs you.")
                             .AddOption("I`ve~got~the~fang.", 4)
                             .AddOption("Brilliant.", 255)
                             .AddAvatar(95).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("Don`t forget your country and people when you have established your name. This is what a responsible person should do. Your")
                             .AddText("~country needs you.");
                            if (client.Player.Level >= 70 && !client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("How~to~devote~myself?", 20);
                            data.AddOption("Brilliant.", 255)
                            .AddAvatar(95).FinalizeDialog();
                        }

                        break;
                    }
                case 20:
                    {
                        data.AddText("The Emperor keeps track of all those who make donations to his empire. When you have reached level 70, you are able to donate");
                        data.AddText("~money to the Emperor for your nobility rank, which can increase your battle power. Your title is determined by the amount of");
                        data.AddText("~money you have donated.");
                        data.AddOption("How~to~donate~money?", 21);
                        data.AddAvatar(95);
                        data.FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        data.AddText("Click the Nobility Button, input the amount of money you want to donate in the box, and confirm your donation. If you don`t");
                        data.AddText("~have enough silver, you can exchange your CPs for silver to use.");
                        data.AddOption("I~see.", 22);
                        data.AddAvatar(95);
                        data.FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        data.AddText("Here is a question for you. How much Battle Power can be added to King/ Queen ranked players?");
                        data.AddInput("(Answer:~12)", 23);
                        data.AddAvatar(95);
                        data.FinalizeDialog();
                        break;
                    }
                case 23:
                    {
                        if (Input == "12")
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                            {
                                client.CreateBoxDialog("You`ve~learned~something~about~the~Nobility~Rankings~and~received~EXP~worth~1/6~of~an~EXP~Ball!");
                                client.GainExpBall(100);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                            }
                        }
                        else
                        {
                            client.CreateBoxDialog("Sorry,~but~you`ve~given~the~incorrect~answer.");
                        }
                        break;
                    }
                case 1:
                    {
                        data.AddText("You mean you saw a potpourri bag just like this before? It must be my nephew, Du San. That poor child. He lost his parents")
                              .AddText("~when he was young, so I raised him up. But when he was in his twenties, he suddenly lost his mind...")
                              .AddText("~I guess I just couldn`t be there for him all the time...")
                             .AddOption("What~happened?", 2)
                             .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1))
                            {
                                data.AddText("Really? You`ve got some clues?")
                                .AddOption("Look~at~this~Venomous~Fang.", 4)
                                .AddAvatar(95).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need some one to kill those Snake Men. They must pay for their sins.")
                                 .AddOption("I`m~in.", 255)
                                 .AddAvatar(95).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("He was in Ape City, learning Kungfu. Then our servants reported that he was missing. Several days later,")
                               .AddText("~someone caught him on the street of Ape City, totally unkempt, babbling strange words... They found snake bites all")
                               .AddText("~over his body... Snake Men got to him. Damn those snake vermin! But nobody has made them pay for it.")
                              .AddOption("I`ll~avenge~him.", 3)
                              .AddOption("I`ll~think~about~it.", 255)
                              .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Snake~Men~are~in~the~west~of~Ape~City.~Kill~some~of~them~and~find~out~why~Du~San~went~lunatic.", 1020, 255, 504, 0);

                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("Really! It`s about time! It must be those snake bites that made Du San go crazy! Please send this Venomous Fang to")
                            .AddText("~Pharmacist Dong (258,277) for diagnosis. We`ll find out for sure, then take revenge!")
                               .AddOption("Got~it.", 5)
                               .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Speack~with~Pharmacist~Dong~(258,277)~for~diagnosis.", ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DuSan)]
        public static void DuSan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaskmasterChang, client.Player.Class, 1781);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1782);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PharmacistDong, client.Player.Class, 1785);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DuSan, client.Player.Class, 1786);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {

                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                             .AddOption("I~have~no~time~to~play.", 10)
                             .AddOption("What~are~you~talking~about?", 255)
                             .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
               && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                              .AddOption("I~have~no~time~to~play.", 13)
                              .AddOption("What~are~you~talking~about?", 255)
                              .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                               .AddOption("I~have~no~time~to~play.", 1)
                               .AddOption("What~are~you~talking~about?", 255)
                               .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                               .AddOption("I~have~no~time~to~play.", 1)
                               .AddOption("What~are~you~talking~about?", 255)
                               .AddAvatar(77).FinalizeDialog();
                            break;
                        }
                        else
                        {
                            data.AddText("I will pass that exam! For the students, passing this exam means a guaranteed position and great credit to their family.")
                             .AddOption("What~are~you~talking~about?", 255)
                             .AddAvatar(77).FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("History has forgotten those glorious celestials... So aloof and dreamy...")
                             .AddText("~Oh, Kun Lun, the Land of the Immortality... Why do I only have this potpourri to remember you by?")
                             .AddOption("Let~me~help~you.", 11)
                            .AddOption("Brilliant.", 255)
                            .AddAvatar(95).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 0, 0, 0, 0, 0, 0, 0, 1))
                        {
                            data.AddText("Snakes! Help!")
                          .AddOption("Take~this~Rainbow~Powder.", 13)
                         .AddAvatar(95).FinalizeDialog();

                            break;
                        }
                        data.AddText("What`d happened to me?")
                           .AddOption("~I forget everthing...", 12)
                          .AddOption("The~GameServer~is~over.", 255)
                          .AddAvatar(95).FinalizeDialog();


                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("You took the Red Toxin and Blue Toxin in turn, and felt very dizzy.");

                        break;
                    }
                case 13:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(729982, 1, stream);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.GainExpBall(500, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You`ve~received~EXP~worth~5/6~EXP~Ball~and~5,000~Silver!~Claim~your~reward~from~Taskmaster~Chang!!", MsgMessage.ChatMode.System);
                        }
                        data.AddText("Yes! Thank you! My uncle has given your reward to Taskmaster Chang. Go claim it there!")
                                .AddOption("OK.~Thank~you.", 14)
                                .AddOption("I`ll~go~later.", 255)
                                .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 14:
                    {
                        data.AddText("Where am I? I remembered I was in Kun Lun... then I was in a dismal abyss... Oh, my head...")
                            .AddText("~Oh, it was you! You saved me! Thank you! I thought I`d never escape the darkness.... Please, take this...")
                             .AddOption("Welcome.", 255)
                             .AddAvatar(77).FinalizeDialog();

                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher("Go~claim~your~reward~from~Taskmaster~Chang!", ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 1:
                    {
                        data.AddText("History has forgotten those glorious celestials... So aloof and dreamy...")
                            .AddText("~Oh, Kun Lun, the Land of the Immortality... Why do I only have this potpourri to remember you by?")
                                 .AddOption("Taskmaster~Chang~sent~me~here.", 2)
                                 .AddOption("Bye.", 255)
                                 .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.Money += 100;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "recovery");
                            client.SendSysMesage("You've completed the quest and received 100 Silver!", MsgMessage.ChatMode.System);

                        }
                        data.AddText("For what? The story of Kun Lun?")
                                   .AddText("~I don`t remember anything except the potpourri bag...")
                                        .AddOption("How~did~you~get~it?", 3)
                                        .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText(" Oh, this? Well, it was given by a celestial when I visited Kun Lun. Isn`t it elegant?")
                                         .AddOption("Sounds~interesting.", 4)
                                         .AddAvatar(77).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher("The~potpourri~bag~looks~similar~to~General~Judd`s~(397,235)~in~Twin~City.~Visit~him~to~investigate.", ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);

                        break;
                    }
            }
        }

        public static string GetStrTask(Client.GameClient user, Database.QuestInfo.DBQuest dbquest)
        {
            if (user.Player.Level < dbquest.Lv_min)
                return "Level~" + dbquest.Lv_min + "";
            if (user.Player.QuestGUI.CheckQuest(dbquest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                return "Underway";
            if (user.Player.QuestGUI.CheckQuest(dbquest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                return "Complete";
            if (user.Player.Level > dbquest.Lv_min)
                return "Acceptable";
            return "Level~" + dbquest.Lv_min;
        }
        [NpcAttribute(NpcID.MysterCheast1)]
        public static void MysterCheast1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            client.Inventory.Remove(3002308, 1, stream);
                            if (client.Player.MyJiangHu != null)
                            {
                                client.Player.MyJiangHu.Talent = (byte)(Math.Min(client.Player.MyJiangHu.Talent + 1, 4));
                                client.Player.MyJiangHu.SendInfo(client, MsgJiangHuInfo.JiangMode.UpdateTalent, true, client.Player.UID.ToString(), client.Player.MyJiangHu.Talent.ToString());
                            }
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);

                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {

                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(Places.TwinCity);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }
                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            if (client.InTrade) return;
                            int Amount = (int)cps;
                            if (Amount < 0)
                                return;
                            client.Player.ConquerPoints -= (uint)cps;

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(Places.TwinCity);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MysterCheast2)]
        public static void MysterCheast2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DailyQuestEnvoy, client.Player.Class, 6390);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.DailyHeavenChance == 0)
                        {
                            data.AddText("Are you ready to harvest fortune from the Mystery Chest? Waste no time, and go for it!")
                               .AddText("After opening the chest, you`ll receive 60 minutes of EXP, 10 Study Points, 1 Chi Token and 1 Heaven Key Scrap. If you`re a Jiang Hu fighter, you`ll also get 1 Talent point.");
                            data.AddOption("Open~the~chest.", 1).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance > 0)
                        {
                            data.AddText("Are you ready to embrace wealth coming from the Mystery Chests? Waste no time, and go for it!");
                            data.AddOption("Open~the~chest.", 2).AddOption("Wait,~I`m~not~ready~yet!", 255).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory");
                            data.AddOption("Let me check.", 255).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Player.DailyHeavenChance += 1;
                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.SubClass.AddStudyPoints(client, 10, stream);
                            client.Inventory.AddItemWitchStack(729304, 0, 1, stream);
                            client.Inventory.AddItemWitchStack(3002307, 0, 1, stream);
                            client.CreateBoxDialog("You~received~60~minutes~of~EXP,~10~Study~Points,~1~Chi~Token,~1~Heaven~Key~Scrap,~and~1~Talent~point!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }

                        break;
                    }

                case 2:
                    {
                        if (client.Player.DailyHeavenChance == 1)
                        {
                            data.AddText("Well, it`s the 2nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP and 30 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 2)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 100 Study Points and 60 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Free Course and 1 Talent point.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 3)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 150 Study Points and 100 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 1 Super Protection Pill and 1 Free Course.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }
                        else if (client.Player.DailyHeavenChance == 4)
                        {
                            data.AddText("Well, it`s the 3nd time you want to open a Mystery Chest. You need to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs or use the Heaven Key to open the chest.")
                                .AddText("You`ll receive 60 minutes of EXP, 200 Study Points and 150 Chi Points from the chest. If you`re a Jiang Hu fighter, you`ll also receive 1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses.");
                            data.AddOption("Open~the~chest~with~" + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + "~CPs.", 3).AddOption("Use~the~Heaven~Key.", 5).FinalizeDialog();
                        }

                        break;
                    }
                case 3:
                    {
                        data.AddText("Are you sure you want to pay " + DailiHeavenCpsCost(client.Player.DailyHeavenChance) + " CPs to open the Mystery Chest?");
                        data.AddOption("Yes.", 4).AddOption("No.", 255).FinalizeDialog();

                        break;
                    }
                case 5:
                    {
                        if (client.Inventory.Contain(3002308, 1))
                        {
                            client.Inventory.Remove(3002308, 1, stream);

                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(Places.TwinCity);
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                        }
                        else
                        {
                            data.AddText("You don`t have a Heaven Key to open the Mystery Chest.");
                            data.AddOption("I~see.", 255).FinalizeDialog();

                        }

                        break;
                    }
                case 4:
                    {
                        uint cps = DailiHeavenCpsCost(client.Player.DailyHeavenChance);
                        if (client.Player.ConquerPoints >= cps)
                        {
                            if (client.InTrade) return;
                            int Amount = (int)cps;
                            if (Amount < 0)
                                return;
                            client.Player.ConquerPoints -= (uint)cps;


                            client.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                            switch (client.Player.DailyHeavenChance)
                            {
                                case 1:
                                    {
                                        client.Player.MyChi.ChiPoints += 30;
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 30 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 2:
                                    {
                                        client.Player.MyChi.ChiPoints += 60;
                                        client.Player.SubClass.AddStudyPoints(client, 100, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 100 Study Points and 60 Chi Points,~and~1 Talent point and 1 Free Course!");

                                        break;
                                    }
                                case 3:
                                    {
                                        client.Player.MyChi.ChiPoints += 100;
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 150 Study Points and 100 Chi Points,~and~1 Talent point, 1 Super Protection Pill and 1 Free Cours!");

                                        break;
                                    }
                                case 4:
                                    {
                                        client.Player.MyChi.ChiPoints += 150;
                                        client.Player.SubClass.AddStudyPoints(client, 200, stream);
                                        client.CreateBoxDialog("You~received~60 minutes of EXP, 200 Study Points and 150 Chi Points,~and~1 Talent point, 5 Protection Pills, 4 Super Protection Pills and 2 Free Courses");

                                        break;
                                    }
                            }
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                            client.Player.DailyHeavenChance += 1;
                            if (client.Player.DailyHeavenChance == 5)
                            {
                                client.Teleport(Places.TwinCity);
                            }

                        }
                        else
                        {
                            data.AddText("You don`t have enough CPs to open the Mystery Chest. Make sure you bring enough money, next time!");
                            data.AddOption("Alright.", 255).FinalizeDialog();

                        }
                        break;
                    }
            }
        }
    }
}

