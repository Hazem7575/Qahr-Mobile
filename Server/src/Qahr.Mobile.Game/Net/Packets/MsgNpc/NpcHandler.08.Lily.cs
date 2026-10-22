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
        [NpcAttribute(NpcID.Lily)]
        public static void Lily(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711440, 1))
                {
                    client.SendSysMesage("You've already got a Lily!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "Lily", 3);
                }
            }
        }

        [NpcAttribute(NpcID.WillowLeaf)]
        public static void WillowLeaf(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711443, 1))
                {
                    client.SendSysMesage("You've already got a Willow Leaf!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "Willow Leaf", 3);
                }
            }
        }

        [NpcAttribute(NpcID.Jasmine)]
        public static void Jasmine(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711442, 1))
                {
                    client.SendSysMesage("You've already got a Jasmine!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "Jasmine", 3);
                }
            }
        }

        [NpcAttribute(NpcID.WhiteChrysanthemum)]
        public static void WhiteChrysanthemum(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (!client.Inventory.HaveSpace(1))
                {
                    Dialog data = new Dialog(client, stream);
                    data.AddText("Please make 1 more space in your inventory.")
                          .AddOption("Let me check.", 255).FinalizeDialog();
                    return;
                }
                if (client.Inventory.Contain(711441, 1))
                {
                    client.SendSysMesage("You've already got a White Chrysanthemum!", MsgMessage.ChatMode.System);
                    return;
                }
                else
                {
                    client.Player.AddPick(stream, "WhiteChrysanthemum", 3);
                }
            }
        }
        [NpcAttribute(NpcID.XimenQing)]
        public static void XimenQing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1600);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1601);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1602);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1603);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1594);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1599);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I like flowers. They have their own language. For example, red roses are given as a symbol of love, beauty and passion...")
                            .AddText("~Hmm, are you coming for a quest or what?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711461, 1))
                            data.AddOption("Elemental~Circle.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Strange~Box~1.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Strange~Box~2.", 10);
                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                data.AddOption("Strange~Box~3.", 20);

                            else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            {
                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                    data.AddOption("Strange~Box~4.", 30);
                            }
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Flower~Arrangement.", 40);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Bird~Beaks.", 50);

                        data.AddOption("Nothing.", 255);
                        data.AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 60:
                    {
                        data.AddText("My wood sword can help defeat the HawKings? Great! Here you are. I hope you`ll succeed, soon!")
                                    .AddOption("Thanks.", 61)
                                    .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~received~the~Wood~Sword~from~the~Playboy~(wood~element)!");
                        client.Inventory.Add(stream, 711461);
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711445, 5))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                                {
                                    client.Inventory.Remove(711445, 5, stream);
                                    client.Inventory.AddItemWitchStack(720173, 0, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~1~Small~Joy~Stone!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Well done! You deserve the reward!")
.AddOption("Thank~you!", 255)
.AddAvatar(74).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("I need you to kill L88 Birdmen and bring me 5 of their Bird Beaks.")
  .AddOption("Deal", 255)
  .AddAvatar(74).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Damn it! The L88 Birdmen ruined my garden! Please, I need you to kill them and bring back 5 of their Bird Beaks.~I`ll pay you!")
                                .AddOption("Deal!", 51)
                                .AddOption("Not~interested.", 255)
                                .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Please~kill~L88~Birdmen~to~collect~5~Bird~Beaks.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711440, 1) && client.Inventory.Contain(711443, 1)
                                && client.Inventory.Contain(711441, 1) && client.Inventory.Contain(711442, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                                {
                                    client.Inventory.Remove(711440, 1, stream);
                                    client.Inventory.Remove(711443, 1, stream);
                                    client.Inventory.Remove(711442, 1, stream);
                                    client.Inventory.Remove(711441, 1, stream);
                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);

                                    client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Great! I didn`t expect you would come back so sonn! Thank you!")
                                  .AddOption("You`re~welcome.", 255)
                                  .AddAvatar(74).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need you to collect a Lily, White Chrysanthemum, Jasmine and a Willow Leaf for me.")
                                    .AddOption("I`m~on~my~way.", 255)
                                    .AddAvatar(74).FinalizeDialog();

                            }

                            break;
                        }

                        data.AddText("Yep, I`m studying flower arranging, these days. Can you help me collect a Lily, White Chrysanthemum, Jasmine and a Willow Leaf?")
                             .AddOption("Okay.", 41)
                             .AddOption("Leave.", 255)
                             .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("Now~go~collect~a~Lily~for~Ximen~Qing!");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(711448, 1))
                        {
                            data.AddText("This is a Phoenix Jade. Please put it inside the Strange Box. I want to give it to the fairy, as a gift.")
                                .AddOption("I`m~on~my~way.", 255)
                                .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        data.AddText("You can`t imagine how excited I am! This is a Phoenix Jade. Put it inside the Strange Box, okay?")
                            .AddText("~I want to give it to the fairy, as a gift.")
                                                        .AddOption("Okay.", 31)
                                                        .AddOption("Later.", 255)
                                                        .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(74).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711448);
                        client.CreateBoxDialog("Please~put~the~Phoenix~Jade~in~the~Strange~Box!");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(711447, 1, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("That is amazing! This letter is written by a fairy! She wants to thank me for taking care of the flowers in the island.")
                                                      .AddOption("Oh,~I~see.", 255)
                                                      .AddAvatar(74).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("In my opinion, maybe we can use a L88 Birdman`s Shap Claw to open this strange box.")
                                                 .AddOption("I`m~on~the~way.", 255)
                                                 .AddAvatar(74).FinalizeDialog();
                            break;
                        }

                        data.AddText("You mean the red box? I`m curious, too. Maybe you can open it yourself.")
                                                   .AddOption("How?", 11)
                                                   .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("I don`t know what`s inside, either. It just somehow appeared here, the other day.")
                       .AddText("~I did try to open the box but failed. In my opinion, maybe we can use L88 Birdman`s Shap Claw to open this strange box.")
                                                .AddOption("Sounds~reasonable.", 12)
                                                .AddOption("I~don`t~think~so.", 255)
                                                .AddAvatar(74).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Please~kill~L88~Birdman~to~get~their~Sharp~Claw.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {


                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("You mean the red box? I`m curious, too. Maybe you can open it yourself.")
                                                      .AddOption("How?", 11)
                                                      .AddAvatar(74).FinalizeDialog();

                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.StrangeBox)]
        public static void StrangeBox(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1600);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1601);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.StrangeBox, client.Player.Class, 1602);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XimenQing, client.Player.Class, 1603);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This is a strange box.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Interesting.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Strange~Box~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Strange~Box~3.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Strange~Box~4.", 30);
                        data.AddOption("Leave.", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(711448, 1, stream);
                            client.CreateBoxDialog("You~saw~a~mysterious~halo~and~found~yourself~energized!");
                            client.IncreaseExperience(stream, 3719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 20:
                    {
                        data.AddText("Are you sure you want to use the claw to open this strange box?")
                                                  .AddOption("Yes.", 21)
                                                  .AddOption("No.", 255)
                                                  .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~opened~the~box~and~found~a~letter~inside.~Deliver~it~to~Ximen~Qing!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 711447);
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(711446, 1))
                        {
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {

                                client.Inventory.Remove(711446, 1, stream);
                                client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                                client.IncreaseExperience(stream, 3719000, questReward: true);
                                client.Player.Money += 3000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                            data.AddText("Are you sure you want to use the claw to open this strange box?")
                                                   .AddOption("Yes.", 21)
                                                   .AddOption("No.", 255)
                                                   .AddAvatar(67).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("As Ximen Qing said, you`d better go get a L88 Birdman`s Sharp Claw to open the box.")

                                                     .AddOption("Alright.cre", 255)
                                                     .AddAvatar(67).FinalizeDialog();
                        }

                        break;
                    }
                case 1:
                    {
                        data.AddText("You seem curious about the box. Would you like to open it?")
                                                      .AddOption("Yeah,~yeah.", 2)
                                                      .AddOption("No~need.", 255)
                                                      .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        data.AddText("The box is locked and you can`t open it. Why not ask Ximen Qing? Maybe he knows something.")
                                                      .AddOption("Okay.", 255)
                                                      .AddAvatar(67).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.CityGeneral)]
        public static void CityGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CityGeneral, client.Player.Class, 1597);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Do not make trouble, here. If you find anything abnormal or need my help with a quest, come talk to me.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Birdman~Feathers.", 1);


                        data.AddOption("I~see.", 255);
                        data.AddAvatar(30).FinalizeDialog();
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

                                    client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                                    client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thank you for what you`ve done!")
                                                          .AddOption("Never~mind.", 255)//10
                                                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("We don`t have enough soldiers to guard the West Gate, and the Birdmen just keep robbing the travelers outside the gate.")
                          .AddText("~I need you to teach them a lesson! Go collect their feathers and burn them at the West Gate!")
                          .AddText("~That will keep them from being so bold.")
                                 .AddOption("You~can~count~on~me.", 255)
                                 .AddAvatar(30).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("We don`t have enough soldiers to guard the West Gate, and the Birdmen just keep robbing the travelers outside the gate.")
                            .AddText("~I need you to teach them a lesson! Go collect their feathers and burn them at the West Gate!")
                            .AddText("~That will keep them from being so bold.")
                                   .AddOption("You~can~count~on~me.", 2)
                                   .AddOption("None~of~my~business.", 255)
                                   .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~collect~some~Birdman~Feathers~and~burn~them~at~the~West~Gate.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WindTaoist)]
        public static void WindTaoist(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindTaoist, client.Player.Class, 1595);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WindTaoist, client.Player.Class, 1596);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Bird Island is really a wonderful place. I believe that some hero will appear here, soon.");
                        data.AddText("~Will you be that hero?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Of~course.", 1);
                        //   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        //     data.AddOption("What`s~next?", 10);

                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.Contain(720985, 1))
                                client.Inventory.Add(stream, 720985);
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 5))
                            {

                            }
                            else
                            {
                                data.AddText("Use the Transformation Amulet to transform and kill 5 Birdmen. Good luck!")
           .AddOption("Ok.", 255)
           .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("The amulet is finished! Haha. Find 5 Birdmen, and use the amulet to transform them into the Pheasants.")
                            .AddText("~Then you can kill them easily. Remember, the amulet can used once per minute. What`s more, you`ll")
                            .AddText("~have the chance to transform yourself into a Pheasant for 10 seconds. Good luck!")
                                            .AddOption("Ok.", 11)
                                            .AddOption("Sorry,~but~I`m~busy.", 255)
                                            .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720985, 1))
                            client.Inventory.Add(stream, 720985);
                        client.CreateBoxDialog("Use~the~Transformation~Amulet~to~kill~5~Birdmen.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(1001020, 1)
                                && client.Inventory.Contain(1002000, 1)
                                && client.Inventory.Contain(711444, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.Inventory.Remove(711444, 1, stream);
                                    client.Inventory.Remove(1002000, 1, stream);
                                    client.Inventory.Remove(1001020, 1, stream);

                                    client.CreateBoxDialog("Congratulations!~You~received~3,270,000~Exp~and~2,000~silver!");
                                    client.IncreaseExperience(stream, 3719000, questReward: true);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Great! With all these materials, I can make the amulet! Thank you, brave hero!")
                                                          .AddOption("Good~luck.", 255)//10
                                                          .AddAvatar(67).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Hurry up! We should teach a lesson to the Birdmen as soon as possible!")
                                           .AddOption("On~my~way!", 255)
                                           .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Have you heard about the evil Birdmen? They`re gathering outside of the city and often attack travelers.")
                            .AddText("~I know a kind of amulet that can transform them into the weaker Pheasants. Could you please buy a Pearl Ointment")
                            .AddText("~and Panacea from the Pharmacist, and a piece of Rice Paper from the Storekeeper?")
                                                 .AddOption("Ok.", 2)
                                                 .AddOption("Sorry,~but~I`m~busy.", 255)
                                                 .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Help~the~Wind~Taoist~collect~the~materials~he~needs.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WealthyWan)]
        public static void WealthyWan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWansWife, client.Player.Class, 1604);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1605);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1606);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1607);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MysteryTaoist, client.Player.Class, 1626);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("It has been my dream to be an aviator since I was a little boy! I believe that one day, I can fly high in the blue sky!");
                        data.AddText("~Higher than anyone else has ever gone! YEAH!!!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711463, 1))
                            data.AddOption("Elemental~Circle.", 40);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Just~be~safe.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Aviator~1.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Aviator~2.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Aviator~3.", 30);

                        data.AddOption("I`m~just~passing~by.", 255);
                        data.AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        data.AddText("You need some gunpowder? I`ve prepared a lot for my aircraft. Here you are! I hope we can eradicate")
                            .AddText("~the HawKings as soon as possible!")
                                  .AddOption("Thanks.", 41)
                                  .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(35).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~received~some~Powder~from~Wealthy~Wan~(fire~element)!");
                        client.Inventory.Add(stream, 711463);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {

                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~a~Level~90~Unique~weapon!");
                                    client.IncreaseExperience(stream, 4719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                        client.Inventory.Add(stream, 410177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//leafBlade
                                    else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                        client.Inventory.Add(stream, 480177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//club
                                    else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                        client.Inventory.Add(stream, 500167, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BambooBow
                                    else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                        client.Inventory.Add(stream, 601177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//iron katana
                                    else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                        client.Inventory.Add(stream, 610177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BambooPrayerBeads
                                    else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                        client.Inventory.Add(stream, 611177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//recruitRapier
                                    else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                        client.Inventory.Add(stream, 421177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//SpellBacksword 
                                    else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                        client.Inventory.Add(stream, 617177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//WhiteWaxNunchaku 
                                    else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                        client.Inventory.Add(stream, 626177, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                                }
                                data.AddText("It has been my dream to be an aviator since I was a little boy! I believe that one day, I can fly high in the blue sky!")
                                    .AddText("~Higher than anyone else has ever gone! YEAH!!!")
                                                          .AddOption("I`m~just~passing~by.", 255)
                                                          .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("We`ll start to fly, now. Are you ready?")
                                             .AddOption("Yes.", 33)
                                             .AddOption("I`ll~think~about~it.", 255)
                                             .AddAvatar(35).FinalizeDialog();
                            }

                            break;
                        }

                        data.AddText("Armorer Yu is so kind! Now we can go out and test my craft. Thanks for your help, and I hope you can complete the first flight.")
                                                .AddOption("I`m~looking~forward~to~it.", 31)
                                                .AddOption("Not~interested.", 255)
                                                .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        client.Teleport(764, 550, 1015);
                        client.CreateBoxDialog("The~kite~broke~right~after~rising~into~the~air.~You~fell~from~the~sky~and~were~hurt!");
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "M_Fire1");
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest4.MissionId, 1);
                        break;
                    }
                case 31:
                    {
                        data.AddText("I knew you wouldn`t be afraid of it. Take a break first, and I`ll complete the preparation work.")
                                                .AddOption("Ok.", 32)
                                                .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711450, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                                {
                                    client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp~and~3,000~silver!");
                                    client.IncreaseExperience(stream, 4719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Armorer Yu is so kind! Now we can go out and test my craft. Thanks for your help, and I hope you can complete the first flight.")
                                                          .AddOption("I`m~looking~forward~to~it.", 30)
                                                          .AddOption("Not~interested.", 255)
                                                          .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please bring back some powder from Armorer Yu.")
                                                          .AddOption("Ok.", 255)
                                                          .AddAvatar(35).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("The last material I need is some gunpowder. Could you please find Armorer Yu and get some for me?")
                                                       .AddOption("Ok.", 21)
                                                       .AddOption("Sorry,~but~I`m~busy.", 255)
                                                       .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Speak~with~Armorer~Yu~and~get~some~Gunpowder.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711449, 5))
                            {

                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711449, 5, stream);
                                    client.CreateBoxDialog("You~received~30~minutes~of~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 4719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thanks for your help! When everything is done, please come back and join me in a trip to the sky! Haha!")
                                                          .AddOption("Good~luck!", 20)
                                                          .AddAvatar(35).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Could you please kill the BirdmanL88 and collect 5 feathers?")
                                        .AddOption("Ok.", 255)
                                        .AddAvatar(35).FinalizeDialog();

                            }

                            break;
                        }

                        data.AddText("Haha, I think I`ll realize my dream, very soon. Let me tell you, I want to make a huge kite and use a large rocket to launch it.")
                            .AddText("~The first thing I need to do is finish the kite. Could you please collect 5 feathers from the BirdmenL88 that are outside?")
                                        .AddOption("Ok.", 11)
                                        .AddOption("I`ll~think~about.", 255)
                                        .AddAvatar(35).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~the~BirdmanL88~to~collect~5~feathers.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;

                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 20))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255)
                                      .AddAvatar(35).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                            {
                                client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 1, stream);
                                client.CreateBoxDialog("Congratulations!~You~received~30~minutes~of~Exp,~3,000~silver~and~an~Exp~Potion!");
                                client.IncreaseExperience(stream, 4719000, questReward: true);
                                client.Player.Money += 3000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                            data.AddText("My wife is always being too careful... And I thought my mother was overprotective! Anyway, thanks for your help!")
                                                      .AddOption("You`re~welcome.", 255)
                                                      .AddAvatar(35).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Didn`t I ask you to go outside of the city and kill 20 Birdmen L88?")
                                                               .AddOption("I~see.", 255)
                                                               .AddAvatar(35).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.WealthyWansWife)]
        public static void WealthyWansWife(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWansWife, client.Player.Class, 1604);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Sigh, my husband wanted to become an aviator for a long time. Maybe having a dream is good for a man... Well, at least I want to make sure he is safe.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("I~can~help.", 1);


                        data.AddOption("You`re~a~good~wife.", 255);
                        data.AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Didn`t I ask you to go outside of the city and kill 20 Birdmen L88?")
                                    .AddOption("I~see.", 255)
                                    .AddAvatar(110).FinalizeDialog();
                            break;
                        }
                        data.AddText("Thanks! He is always working on his machine outside, but there are so many BirdmenL88 outside of the city. I`m really")
                            .AddText("~worried about him. Could you please go and kill 20 BirdmenL88? Show them no mercy!")
                                     .AddOption("Alright.", 2)
                                     .AddOption("Sorry,~but~I`m~busy.", 255)
                                     .AddAvatar(110).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~outside~of~the~city~and~kill~20~BirdmenL88.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.LittleBen)]
        public static void LittleBen(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArmorerYu, client.Player.Class, 6129);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Nobody likes me, even my father only cares about his boring job. Maybe I should just disappear...");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Your~dad`s~worried~about~you!", 1);


                        data.AddOption("You~naughty~boy!", 255);
                        data.AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("Really? So, my father sent you here? He may remember me at times, so what? Nothing will change, and he will still fix his sights on his work again, tomorrow.")
                            .AddText("~Look, how about we play some Rock-Paper-Scissors? If you win, I`ll go home. If not... Haha, I`ll push you off the roof! The rules are easy. `Rock` beats `Scissors`,")
                            .AddText("~`Scissors` beats `Paper`, and `Paper` beats `Rock`. You got it?")
                                      .AddOption("Yeah,~let`s~start!", 2)
                                      .AddOption("I~need~more~time~to~remember.", 255)
                                      .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Rock-Paper-Scissors...Shoot!")
                                      .AddOption("Rock!", 3)
                                      .AddOption("Scissors!", 4)
                                      .AddOption("Paper!", 3)
                                      .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("Wh...A tie? I want to win! Let`s play again. Rock-Paper-Scissors...Shoot!")
                                      .AddOption("Rock!", 3)
                                      .AddOption("Scissors!", 4)
                                      .AddOption("Paper!", 3)
                                      .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 4:
                    {

                        data.AddText("Alright, it looks like I lose. Whew, I guess I`ll go home, then. See you around!")
                                         .AddOption("You`re~a~good~boy.", 5)
                                         .AddAvatar(68).FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                            client.IncreaseExperience(stream, 4719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = (uint)NpcID.LittleBen,
                            Type = ActionType.RemoveEntity
                        };
                        client.Send(stream.ActionCreate(action));
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ArmorerYu)]
        public static void ArmorerYu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArmorerYu, client.Player.Class, 1592);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ArmorerYu, client.Player.Class, 6129);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.WealthyWan, client.Player.Class, 1606);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("My father used to be a famous armorer, and I want to work as well as him. The most important thing")
                            .AddText("~is to forge an ultimate weapon.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted)
                            && !client.Inventory.Contain(711450, 1))
                            data.AddOption("The~Aviator~2.", 30);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Can~I~help~you?", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Quest:~Lonely~Ben.", 20);

                        data.AddOption("You`ll~make~it.", 255);
                        data.AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        /*sa..
alchemist..
black
circus
china !
*/
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        data.AddText("Wealthy Wan is still insisting on his foolish dream... Well, if you want to get some gunpowder from me, you`ll need")
                            .AddText("~need to answer 5 questions correctly!")
                                       .AddOption("Go~ahead.", 31)
                                       .AddOption("Sorry,~but~I`m~busy.", 255)
                                       .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("1. Gunpowder is actually a mixture of sulfur, charcoal and what?")
                                .AddOption("Saltpeter.", 32)
                               .AddOption("Saltpeter.", 39)
                                .AddOption("Saltpeter.", 39)
                                .AddOption("Saltpeter.", 39)
                                .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        data.AddText("2. The powder was first made by who?")
                              .AddOption("Doctor.", 39)
                             .AddOption("Alchemist.", 33)
                              .AddOption("Craftsman.", 39)
                              .AddOption("Hunter.", 39)
                              .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("3. In the early days, gunpowder was called?")
                            .AddOption("Yellow~powder.", 39)
                           .AddOption("Green~powder.", 39)
                            .AddOption("Black~powder.", 34)
                            .AddOption("Red~powder.", 39)
                            .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        data.AddText("4. At first, gunpowder was widely used in which area?")
                           .AddOption("War.", 39)
                          .AddOption("Circus.", 35)
                           .AddOption("Medicine.", 39)
                           .AddOption("Cultivation.", 39)
                           .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 35:
                    {
                        data.AddText("5. Gunpowder was first made in which country?")
                             .AddOption("America.", 39)
                            .AddOption("Britain.", 39)
                             .AddOption("India.", 39)
                             .AddOption("China.", 36)
                             .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 39:
                    {
                        data.AddText("Sorry, but you`ll need to go study up a little more.")
                               .AddOption("Ok.", 255)
                               .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 36:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        client.CreateBoxDialog("You~can~report~back~to~Wealthy~Wan,~now.");
                        client.Inventory.Add(stream, 711450);
                        data.AddText("It seems these questions are easy for you. Great! Open your inventory, and I`ll give you some gunpowder!")
                                 .AddOption("Thanks.", 255)
                                 .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Alas... It`s all my fault. I should have paid more attention to Little Ben. If anything happpens to him, let me know. I`m so worried about him.")
                                        .AddOption("He~will~be~fine.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hey! Did you see my son, Ben? I was so absorbed in my work that I didn`t notice when he walked away. I hope he isn`t on someone`s roof, again. If you see Little Ben")
                            .AddText("~somewhere, please bring him home.")
                                       .AddOption("Okay.~I~will.", 21)
                                       .AddOption("Maybe~he~has~returned~home.", 255)
                                       .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(1015, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711438, 5))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                          .AddOption("Let me check.", 255)
                                          .AddAvatar(47).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.Inventory.Remove(711438, 5, stream);
                                    client.Inventory.AddItemWitchStack(720172, 0, 1, stream);
                                    client.CreateBoxDialog("Congratulations!~You~received~4,850,000~Exp,~3,000~silver~and~an~EXP~Ball~Scrap!");
                                    client.IncreaseExperience(stream, 4719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Just as I expected, these arms are really solid. I`ll forge them into a powerful weapon! Thanks!")
                                        .AddOption("You`re~welcome.", 255)
                                        .AddAvatar(30).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Go and kill some Birdmen to collect 5 arms for me. Of course, you`ll be handsomely rewarded!")
                                            .AddOption("Alright.", 255)
                                            .AddAvatar(47).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("Actually, I know how to make the weapon. The Birdman Arm is the key material. Could you please kill")
                            .AddText("~them and collect 5 arms for me?")
                                             .AddOption("Ok.", 11)
                                             .AddOption("Sorry,~but~I`m~busy.", 255)
                                             .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~the~Birdmen~and~collect~their~arms.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ViceGeneral)]
        public static void ViceGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IslandGeneral, client.Player.Class, 1620);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneral, client.Player.Class, 1621);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`m guarding the island. Was there a quest can I help you with?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Time~to~Unite.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~L93~Hawks.", 10);

                        data.AddOption("Nope.", 255);
                        data.AddAvatar(66).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("It`s a good start. Thank you very much!")
                           .AddOption("Not~at~all.", 255)
                           .AddAvatar(66).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Have you killed 30 L93 Hawks?")
                            .AddOption("Not~yet!", 255)
                            .AddAvatar(66).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The first plan is to kill 30 L93 Hawks. I hope you can help me with that.")
                            .AddOption("No~problem.", 11)
                            .AddOption("It`s~dangerous.", 255)
                            .AddAvatar(66).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Go~kill~30~L93~Hawks!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711455, 1, stream);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("The general asked you to deliver a letter? Great! Let me read it...")
                              .AddOption("Good~luck~for~you.", 255)
                              .AddAvatar(66).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.IslandGeneral)]
        public static void IslandGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {

            Dialog data = new Dialog(client, stream);

            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1608);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IslandGeneral, client.Player.Class, 1611);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IslandGeneral, client.Player.Class, 1620);

            switch (Option)
            {
                case 0:
                    {

                        data.AddText("I`m guarding the island. If you find anything special, please report to me.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Military~Order.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~HawKings.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Time~to~Unite.", 20);

                        data.AddOption("Bye.", 255);
                        data.AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(711455, 1))
                        {
                            data.AddText("It`s really urgent. Please inform the Vice General as soon as possible!")
                                .AddOption("Copy~that.", 255)
                                .AddAvatar(30).FinalizeDialog();
                            break;
                        }

                        data.AddText("I`ve sent the Vice General to guard the northeastern island. Now, it`s the time to unite to eliminate the Hawk Bosses.")
                            .AddText("~He`ll know the detailed plans when he reads this letter.")
                                      .AddOption("You~can~count~on~me.", 21)
                                      .AddOption("Sorry,~I`m~busy.", 255)
                                      .AddAvatar(30).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(30).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(711455, 1))
                            client.Inventory.Add(stream, 711455);
                        client.CreateBoxDialog("Please~give~this~letter~to~the~Vice~General.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                          .AddOption("Let me check.", 255)
                                          .AddAvatar(30).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1060024, 0, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~2/3~EXP~Ball,~4,000~Silver~and~1~Bird~Island~Gate!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thank you for your help! I bet they won`t be that bold from now on!")
                                        .AddOption("I~hope~so.", 255)
                                        .AddAvatar(30).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need your help. Go kill 50 HawKings! I will see the ground run red with their blood!")
                                .AddOption("Got~it.", 255)
                                .AddAvatar(30).FinalizeDialog();
                                break;
                            }

                            break;
                        }
                        data.AddText("We`re going to slay the HawKings and need your help.")
                            .AddText("~Please go kill 50 of them!")
                                .AddOption("I`m~in.", 11)
                                .AddOption("No,~I~won`t.", 255)
                                .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Go~kill~50~HawKings!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711451, 1, stream);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                        }
                        data.AddText("Okay, I got it. Thanks for your information.")
                                .AddOption("You`re~welcome.", 255)
                                .AddAvatar(30).FinalizeDialog();
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.BIViceCaptain)]
        public static void BIViceCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1591);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1598);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BIViceCaptain, client.Player.Class, 1608);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Welcome to the Bird Island. Are you coming for a quest?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Birdman.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hunting~L88~Birdmen.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Military~Order.", 20);

                        data.AddOption("Nothing.", 255);
                        data.AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(711451, 1))
                        {
                            data.AddText("I just received the report that the HawKings were gathering and planning to attack the city.")
                        .AddText("~Please deliver this General Token to the island General and ask him to be on alert.")
                                     .AddOption("Okay.", 255)
                                     .AddAvatar(47).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("I just received the report that the HawKings were gathering and planning to attack the city.")
                          .AddText("~Please deliver this General Token to the island General and ask him to be on alert.")
                                       .AddOption("I`m~on~my~way.", 21)
                                       .AddOption("Why~me?!", 255)
                                       .AddAvatar(47).FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                  .AddOption("Let me check.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(711451, 1) == false)
                            client.Inventory.Add(stream, 711451);
                        client.CreateBoxDialog("Hurry~to~take~this~General~Token~to~the~Island~General.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in your inventory.")
                                      .AddOption("Let me check.", 255)
                                      .AddAvatar(47).FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.AddItemWitchStack(1060024, 0, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~1~Bird~Island~Gate!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("Thank you very much! We`ve removed the nearby dangers!")
                                        .AddOption("My~pleasure.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Many villagers were killed by the L88 Birdmen! We have to do something. Please go kill 30 of them. We must protect the island!")
                                        .AddOption("I~won`t~disappoint~you.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Things have become worse... Many villagers were killed by the L88 Birdmen!")
                         .AddText("~We must stop them! Please go kill 30 of them. We must protect the island!")
                                      .AddOption("I~won`t~disappoint~you.", 11)
                                      .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Please~kill~30~L88~Birdmen.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 50))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                                {
                                    client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                }
                                data.AddText("That`s very kind of you! Thank you very much!")
                                        .AddOption("No~problem.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Birdmen keep trying to attack the city to grab the treasures we have stored. Please go kill 50 Birdmen!")
                                        .AddOption("I`m~setting~off.", 255)
                                        .AddAvatar(47).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("You must have noticed that many Birdmen are gathering outside the city. I need you to kill 50 of them, to protect the city!")
                                        .AddOption("I~won`t~let~you~down.", 2)
                                        .AddOption("Why~me?!", 255)
                                        .AddAvatar(47).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~kill~50~Birdmen!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.XuFan)]
        public static void XuFan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1590);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.XuFan, client.Player.Class, 1593);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey! What can I do for you? Are you coming to repair items or maybe for a job?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Inform~the~Artisan.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~Tool~Bag.", 10);


                        data.AddOption("Nothing.", 255);
                        data.AddAvatar(53).FinalizeDialog();
                        break;
                    }

                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711439, 1))
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                        .AddOption("Let me check.", 255)
                                        .AddAvatar(53).FinalizeDialog();
                                    break;
                                }
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Add(stream, 720173);
                                    client.Inventory.Remove(711439, 1, stream);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~a~Small~Joy~Stone!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Yes, that`s it! Thank you very much!")
                                      .AddOption("Never~mind.", 13)
                                      .AddAvatar(53).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("My tool bag was stolen by a Birdman, the other day. Can you please retrieve it for me?")
                          .AddOption("No~problem.", 255)
                          .AddAvatar(53).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("My tool bag was stolen by a Birdman, the other day. Can you please retrieve it for me?")
                           .AddOption("My~pleasure!", 11)
                           .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Hey! What can I do for you? Are you coming to repair items or maybe for a job?")
                             .AddOption("Nothing.", 255)
                             .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 720173);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~1~Defense~Pot~(20m)!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Really?! Thank you for what you`ve done for the city!")
                              .AddOption("My~pleasure!", 255)
                              .AddAvatar(53).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Xi_er)]
        public static void Xi_er(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1609);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Xi_er, client.Player.Class, 1610);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I was begging my father for a long time before he agreed to let me play outside. I had such a good time these days!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("~Everyone should be so free!", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Old~Book.", 10);


                        data.AddOption("Nice.", 255);
                        data.AddAvatar(135).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711452, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.Inventory.Remove(711452, 1, stream);
                                    client.Inventory.Add(stream, 720172);
                                    client.CreateBoxDialog("You`ve~received~EXP~worth~2/3~EXP~Ball,~4,000~Silver~and~1~EXP~Ball~Scrap!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 4000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Great! That`s the Old Book! Thank you very much!")
                                        .AddText("~I`ll bring the book home soon!")
                                      .AddOption("Okay.", 255)
                                      .AddAvatar(135).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Thank you! The book has been shred into separate parts. Please kill the HawKings and try your best to retrieve all of them!")
                                  .AddOption("I~will.", 255)
                                  .AddAvatar(135).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I`m wondering if you can do me a favor. I took an old book with me when I left home, unfortunately, it was shred into parts and taken by the HawKings!")
                           .AddText("~Please, you have to help me. It`s my father`s favorite book...")
                            .AddOption("I`ll~try~my~best.", 11)
                            .AddOption("That`s~your~fault.", 255)
                               .AddAvatar(135).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Kill~HawKings~to~retrieve~the~Old~Book.");
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(135).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 723340);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Study~Points!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Yes, I`m Castellan`s daughter, Xi`er. My father is worried about me? I guess it`s my fault... I stayed gone a long time.")
                              .AddOption("Thank~god~you`re~fine.", 255)
                              .AddAvatar(135).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.BICastellan)]
        public static void BICastellan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1660);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1661);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1590);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BICastellan, client.Player.Class, 1609);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the castellan of Bird Island. Are you coming for any jobs we have available?");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Head~to~Bird~Island.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Patrol~the~City.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == true
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inform~the~Artisan.", 20);
                        else if (client.Player.Level > 90)
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Look~for~Daughter.", 30);

                        data.AddOption("Hail~to~you!", 255);
                        data.AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            data.AddText("I`m worried about my daughter, Xi`er. She went out 3 days ago, but hasn`t come back, yet!")
                           .AddText("~Can you please search the island for her?")
                                   .AddOption("I~will.", 255)
                                   .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        data.AddText("I`m worried about my daughter, Xi`er. She went out 3 days ago, but hasn`t come back, yet!")
                            .AddText("~Can you please search the island for her?")
                                    .AddOption("Sure.", 31)
                                    .AddOption("I`m~busy.", 255)
                                    .AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Please~serch~the~Island~for~Xi`er.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please ask Xu Fan to repair the bridge without any delays!")
                                     .AddOption("Got~it!", 255)
                                     .AddAvatar(18).FinalizeDialog();

                            break;
                        }
                        data.AddText("The bridge is badly damaged? That`s a big problem! We must do something, before any disasters happen.")
                            .AddText("~Can you inform the artisan, Xu Fan, about this? Please ask him to repair the bridge as soon as possible.")
                                      .AddOption("Sure!", 21)
                                      .AddOption("Later.", 255)
                                      .AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Please~head~to~visit~the~artisan,~Xu~Fan!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                                {
                                    client.CreateBoxDialog("You~received~20~minutes~of~EXP~and~2,000~Silver!");
                                    client.IncreaseExperience(stream, 2719000, questReward: true);
                                    client.Player.Money += 2000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    data.AddText("Really?! Thank you for what you`ve done for the city!")
                                      .AddOption("My~pleasure!", 20)
                                      .AddAvatar(18).FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Please patrol the city to check if there`s any potential danger!")
                                  .AddOption("I`m~leaving.", 255)
                                  .AddAvatar(18).FinalizeDialog();

                            }
                            break;
                        }
                        data.AddText("We are always shorthanded. I`m wondering if you can patrol the city for me, to find out if there`s any potential danger.")
                               .AddOption("Glad~to~help.", 11)
                               .AddOption("None~of~my~business.", 255)
                               .AddAvatar(18).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Please~go~patrol~the~east~of~the~island.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(18).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711437, 1, stream);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            data.AddText("Desert City Vice Captain asked you to deliver this Official Letter? He must be very busy. Thanks for your help!")
                              .AddOption("Welcome.", 255)
                              .AddAvatar(18).FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.Lotus)]
        public static void Lotus(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.Lotus, client.Player.Class, 1456);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This place used to be a thick forest. Years ago, there came a group of monsters that destroyed the forest and turned it into a barren desert.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~1.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Withered~Trees~4.", 10);

                        data.AddOption("Leave.", 255);
                        data.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        data.AddText("Really? The Withered Trees are alive, now? Thank you a lot!")
                           .AddOption("My~pleasure.", 11).AddAvatar(159)
             .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(159).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                                client.Inventory.Add(stream, 118069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                                client.Inventory.Add(stream, 111069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                                client.Inventory.Add(stream, 113029, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                                client.Inventory.Add(stream, 112069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                                client.Inventory.Add(stream, 143069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                                client.Inventory.Add(stream, 144069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsLee(client.Player.Class))
                                client.Inventory.Add(stream, 148069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                                client.Inventory.Add(stream, 114069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true, Role.Flags.ItemEffect.None, true, "~and~an~EXP~Ball!");
                            else if (Database.AtributesStatus.IsWindWalker(client.Player.Class))
                                client.Inventory.Add(stream, 101069, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Super~Hat!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Taoist Yu is said to be wise man. Perhaps he has a way to save the dying trees.")
                            .AddOption("Fine.", 255).AddAvatar(159)
              .FinalizeDialog();

                            break;
                        }

                        data.AddText("The sand is about to swallow the whole area. People left here one after the other... Before I leave, I wish I could save the Withered Trees which that I planted last year.")
        .AddText("~Taoist Yu is said to be wise man. Perhaps he has a way to save the dying trees?")
                            .AddOption("I~won`t~let~you~down.", 2)
                .AddOption("Leave.", 255).AddAvatar(159)
                .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Head~to~ask~Taoist~Yu~for~solution.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DCViceCaptain)]
        public static void DCViceCaptain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.CaptorCooke, client.Player.Class, 1450);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1451);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1460);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1465);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SculptorHe, client.Player.Class, 1496);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1511);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1660);
            switch (Option)
            {

                case 0:
                    {

                        data.AddText("I have been guarding here for years. I miss my wife and kids...");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Inscription~1.", 40);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Elder~Jiang~4.", 50);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Head~to~Desert~City.", 1);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Sand~Monsters.", 10);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~L68~Sand~Monsters.", 20);


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Speedy~Rescue.", 30);
                        if (client.Player.Level >= 87)
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                                data.AddOption("Head~to~Bird~Island.", 60);

                        data.AddOption("See~you~then!", 255);
                        data.AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Please deliver this Official Letter to the Bird Island Castellan!")
                                   .AddOption("I`m~on~my~way.", 255)
                                   .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Hello, there! I need you to do me a favor. You see, I`m really busy these days...")
                            .AddText("~Can you head to the Bird Island and deliver this letter to the Castellan?")
                          .AddOption("You~can~count~on~me.", 61)
                        .AddOption("I`m~busy,~too!", 255)
                          .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(53).FinalizeDialog();

                            break;
                        }



                        client.CreateBoxDialog("Please~visit~the~Castellan~in~the~Bird~Island.");

                        client.Inventory.Add(stream, 711437);
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        break;
                    }
                case 50:
                    {
                        data.AddText("What? A straight hook and above the water? That`s interesting. I`ll report that to the Captain and I`m sure he will visit the strange old man.")
                           .AddOption("Bye.", 51)
                           .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {

                            data.AddText("Please make 3 more spaces in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(53).FinalizeDialog();

                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
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
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.SendSysMesage("You`ve~received~some~EXP,~2,000~Silver,~a~random~Gem~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 40:
                    {
                        data.AddText("Inscription? Hurry to deliver it to Sculptor He!")
                           .AddOption("I`m~on~my~way.", 41)
                           .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                    .AddOption("Let me check.", 255)
                                    .AddAvatar(53).FinalizeDialog();

                            break;
                        }
                        if (!client.Inventory.Contain(711408, 1))
                        {
                            client.Inventory.Add(stream, 711408);

                            client.CreateBoxDialog("Hurry~to~deliver~the~Inscription~to~Sculptor~He.");

                        }
                        break;
                    }

                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You came at the right time! A refugee camp in Desert was surrounded by a group of Hill Monsters.")
                      .AddText("~Go kill 20 of them, and report back to General ZhuGe!")
                       .AddOption("Okay.", 255)
                       .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        data.AddText("Gulp! Those villagers in the east of Desert haven`t recovered from the sandstorm, and another threat from the Hill Monsters came.")
                       .AddText("~Go kill 20 of the Hill Monsters, and report back to General ZhuGe!")
                            .AddOption("I~won`t~let~you~down.", 31)
                        .AddOption("I`m~busy.", 255)
                        .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~20~Hill~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 30))
                            {
                                data.AddText("Good job! Here`s your reward!")
                      .AddOption("Thanks.", 22)
                      .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you killed the Sand Monsters, yet? Go patrol outside the city gates. You`ll find plenty of them roaming around.")
                         .AddOption("Roger.", 255)
                         .AddAvatar(53).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Misfortune never comes alone. This time, I need you to kill 30 Level 68 Sand Monsters!")
                          .AddOption("Got~it.", 21)
                          .AddOption("Not~interested.", 255)
                          .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~30~Level~68~Sand~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Thanks for what you`ve done!")
                      .AddOption("My~pleasure.", 12)
                      .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The cruel Sand Monsters often attack the travellers who pass through here. I need you to kill 50 of them!")
                         .AddOption("I`ll~set~out~right~away.", 255)
                         .AddAvatar(53).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The cruel Sand Monsters often attack the travellers. I need you to kill 50 of them!")
                          .AddOption("I~will.", 11)
                          .AddOption("Why~me?", 255)
                          .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You~received~2,519,000~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 2519000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~50~Sand~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {
                        data.AddText("Captor Cooke sent you here? Thanks for delivering the letter.")
                            .AddOption("You`re~welcome.", 2)
                            .AddAvatar(53).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
         .AddOption("Let me check.", 255)
         .AddAvatar(53).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~1,500~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 1739000, questReward: true);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
            }

        }

        [NpcAttribute(NpcID.WitheredTree1)]
        public static void WitheredTree1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is a withered tree.")
                           .AddOption("Use~Healing~Water.", 1).AddOption("Leave.", 255)
             .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                            {
                                client.CreateBoxDialog("You`ve~watered~this~tree~already.");
                            }
                            else
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    client.CreateBoxDialog("You`ve~watered~all~the~Withered~Trees.~Go~report~to~Lotus.");
                                else
                                    client.CreateBoxDialog("You`ve~watered~the~tree~with~the~Healing~Water.~Instantly,~it~becomes~green~and~full~of~life!");
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WitheredTree2)]
        public static void WitheredTree2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is a withered tree.")
                           .AddOption("Use~Healing~Water.", 1).AddOption("Leave.", 255)
             .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                            {
                                client.CreateBoxDialog("You`ve~watered~this~tree~already.");
                            }
                            else
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 1);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    client.CreateBoxDialog("You`ve~watered~all~the~Withered~Trees.~Go~report~to~Lotus.");
                                else
                                    client.CreateBoxDialog("You`ve~watered~the~tree~with~the~Healing~Water.~Instantly,~it~becomes~green~and~full~of~life!");
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.WitheredTree3)]
        public static void WitheredTree3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This is a withered tree.")
                           .AddOption("Use~Healing~Water.", 1).AddOption("Leave.", 255)
             .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                            {
                                client.CreateBoxDialog("You`ve~watered~this~tree~already.");
                            }
                            else
                            {
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "relive1");
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 0, 0, 1);
                                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                                    client.CreateBoxDialog("You`ve~watered~all~the~Withered~Trees.~Go~report~to~Lotus.");
                                else
                                    client.CreateBoxDialog("You`ve~watered~the~tree~with~the~Healing~Water.~Instantly,~it~becomes~green~and~full~of~life!");
                            }
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.TaoistYu)]
        public static void TaoistYu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1461);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceGenera, client.Player.Class, 1454);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1455);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.Lotus, client.Player.Class, 1456);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1457);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1458);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistYu, client.Player.Class, 1459);
            switch (Option)
            {
                case 0:
                    {


                        data.AddText("The desert seems to be dead, without anything moving around. But I think treasures are always hidden under a barren land. That`s why I`m here! I`m looking for a talisman!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Withered~Trees~1.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~2.", 40);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                          && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~3.", 50);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                && client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Withered~Trees~4.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Collecting~Sand~Essence.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Hidden~Danger~3.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Hidden~Danger~4.", 20);
                        }
                        data.AddOption("Leave.", 255);
                        data.AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 60:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("OK. The Healing Water is done! You`ll need to water the 5 Withered Trees. After that, go tell Lotus about it.")
                         .AddOption("I~will.", 255)
                         .AddAvatar(67).FinalizeDialog();

                            break;
                        }
                        data.AddText("Here`s an Emerald Bottle. You`re going to head to the Moon Spring and collect some water.")
                          .AddOption("You~can~count~on~me.", 61)
                          .AddOption("Is~it~dangerous?", 255)
                          .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 61:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 711340);
                        client.CreateBoxDialog("You`ve~received~the~Healing~Water!");
                        client.Player.QuestGUI.Accept(ActiveQuest7, 0);
                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711341, 1))
                            {
                                data.AddText("I`ll use the Moon Spring and Sand Blocker to make the Healing Water. Please wait a moment.")
                          .AddOption("Take~your~time.", 53)
                          .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To save the withered trees, we also need to collect the Sand Blocker. It`s carried by the Sand Monsters. Would you like to do me the favor?")
                                 .AddOption("Sure.", 255)
                                 .AddAvatar(67).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("To save the withered trees, we also need to collect the Sand Blocker. It`s carried by the Sand Monsters. Would you like to do me this favor?")
                               .AddOption("Sure.", 52)
                               .AddOption("I`ll~think~about~it.", 255)
                               .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 53:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
                        {

                            client.Inventory.Add(stream, Database.ItemType.Meteor, 3);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.SendSysMesage("You've~received~some~EXP~3,000~Silver,~5~Panaceas,~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1))
                            {
                                data.AddText("Good job! I didn`t expect you would come back so soon.")
                              .AddOption("It`s~a~piece~of~cake.", 42)
                              .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {

                                data.AddText("Here`s an Emerald Bottle. You`re going to head to the Moon Spring and collect some water.")
                       .AddText("~Beware of the Sand Monsters, as they`re drawn to the magic of the bottle and may try to steal it!")
                               .AddOption("OK.~I~will.", 255)
                               .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Here`s an Emerald Bottle. You`re going to need to head to the Moon Spring and collect some water.")
                            .AddText("~Beware of the Sand Monsters, as they`re drawn to the magic of the bottle and may try to steal it!")
                                    .AddOption("OK.~I~will.", 41)
                                    .AddAvatar(67).FinalizeDialog();
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
                        client.Inventory.Add(stream, 720938);
                        client.CreateBoxDialog("You`ve~received~an~Emerald~Bottle.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        data.AddText("Save the withered trees? Let me see...")
                                   .AddOption("Take~your~time.", 31)
                                   .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("To save the withered trees, we also need to collect the Sand Blocker. It`s carried by the Sand Monsters. Would you like to do me this favor?")
                                  .AddOption("Sure.", 52)
                                  .AddOption("I`ll~think~about~it.", 255)
                                  .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 52:
                    {
                        client.CreateBoxDialog("Go~fetch~a~Sand~Blocker~from~the~Sand~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(720173, 0, 2, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~1,000~Silver~and~2~SmallJoyStone!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Here`s an Emerald Bottle. You`re going to need to head to the Moon Spring and collect some water.")
                          .AddText("~Beware of the Sand Monsters, as they`re drawn to the magic of the bottle and may try to steal it!")
                                  .AddOption("OK.~I~will.", 41)
                                  .AddAvatar(67).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Here`s a Jade Flask. You`re going to collect some water from the source in the city and spread the water over the city gates.")
                            .AddText("~After that, the Sand Monsters will be unable to go through the gates or enter the city. Don`t forget to inform the DC Vice General of the news.")

                           .AddOption("I`ll~do~right~away.", 22)
                            .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.Contain(720939, 1))
                            client.Inventory.Add(stream, 720939);
                        client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest3.MissionId, 1);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You`ve~received~a~Jade~Flask.~Hurry~to~collect~some~water!");

                        break;
                    }
                case 10:
                    {
                        data.AddText("You were sent by the DC Vice General? Anything I can help you with?")
                              .AddOption("Check~this~Sandbag.", 11)
                              .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~1,000~Silver!");
                            client.IncreaseExperience(stream, 2719000, questReward: true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711343, 10))
                            {
                                data.AddText("You really impressed me! Thank you very much!")
                                  .AddOption("You`re~welcome.", 3)
                                  .AddAvatar(67).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you ever heard of the Sand Essence? It`s the perfect material to forge a talisman. As far as I know, some Level 68 Sand Monsters are carrying the Sand Essence.")
                                    .AddText("~Can you go collect 10 for me?")
                                    .AddOption("Sure.", 255)
                                    .AddAvatar(67).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Have you ever heard of the Sand Essence? It`s a perfect material to forge a talisman. As far as I know, some Level 68 Sand Monsters are carrying the Sand Essence.")
                      .AddText("~Can you go collect 10 for me?")
                           .AddOption("Glad~to~help.", 2)
                       .AddOption("None~of~my~business.", 255)
                       .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~go~collect~10~Sand~Essences!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;

                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                                .AddOption("Let me check.", 255)
                                .AddAvatar(67).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711343, 10, stream);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.SendSysMesage("You've~received~some~EXP,~3,000~Silver,~an~EXP~Ball~Scrap~and~a~+1~Stone~(B)!", MsgMessage.ChatMode.System);

                            client.Inventory.Add(stream, 720172);
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 2719000);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }


                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SpringSoldierZhao)]
        public static void SpringSoldierZhao(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1504);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1505);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1506);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1507);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Let`s wait and see. The backup will arrive, soon. We are going to remove all the monsters!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~3.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~4.", 30);
                        data.AddOption("Bye.", 255);
                        data.AddAvatar(92).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest4.MissionId, 1))
                            {
                                data.AddText("Well done! Please accept these rewards.")
             .AddOption("Thanks.", 32).AddAvatar(92)
             .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Why are you still here? I need you to put the Antidote into the source of the Moon Spring, right away!")
                .AddOption("I`m~on~my~way.", 255).AddAvatar(92)
                .FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("I need you to put the Antidote into the source of the Moon Spring, right away! Beware of the Level 83 Blade Ghosts, on the way.")
                    .AddOption("I~will.", 31)
                  .AddOption("I`m~busy.", 255).AddAvatar(92)
                  .FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(92)
                           .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Add(stream, 730002, 1, 2);
                            client.Inventory.AddItemWitchStack(Database.ItemType.DoubleExp, 0, 0, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You've~received~some~EXP~3,000~Silver,~and~EXP~Postion~and~a~+2~Stone~(B)!", MsgMessage.ChatMode.System);
                            client.CreateBoxDialog("You~received~a~+2~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Inventory.Contain(720968, 1) == false)
                            client.Inventory.Add(stream, 720968);

                        client.CreateBoxDialog("You`ve~received~an~Antidote.~Hurry~to~put~it~into~the~Moon~Spring.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(720968, 1))
                        {
                            data.AddText("Thanks for the antidote! Here`s your reward!")
                          .AddOption("Okay.", 21).AddAvatar(92)
                          .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Come back when you will have Antidote.")
                        .AddOption("Okay.", 255).AddAvatar(92)
                        .FinalizeDialog();
                        }
                        break;
                    }
                case 21:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                        }
                        data.AddText("I need you to put the Antidote into the source of the Moon Spring, right away! Beware of the Level 83 Blade Ghosts, on the way.")
                            .AddOption("I~will.", 31)
                          .AddOption("I`m~busy.", 255).AddAvatar(92)
                          .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Does General ZhuGe have a solution?")
                          .AddOption("I`m~on~my~way.", 255).AddAvatar(92)
                          .FinalizeDialog();
                            break;
                        }
                        data.AddText("The Moon Spring was poisoned! I need you to pay a visit to General ZhuGe and ask him how to solve this problem!")
                            .AddOption("I`ll~be~back~soon.", 11)
                            .AddOption("Later.", 255).AddAvatar(92)
                            .FinalizeDialog();

                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~head~to~see~General~ZhuGe.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                        {
                            data.AddText("So the Moon Spring is poisoned! Thank you very much for your help.")
                                .AddOption("My~pleasure.", 3).AddAvatar(92)
                                .FinalizeDialog();

                            break;//finish
                        }
                        if (client.Inventory.Contain(720967, 1))
                        {
                            data.AddText("Did you take the Moon Spring I gave to you?")
                .AddOption("Nope.", 255).AddAvatar(92)
                .FinalizeDialog();
                            break;
                        }
                        data.AddText("Recently, many villagers were poisoned by drinking the water from the Moon Spring. Could you test if the water is toxic?")
                .AddOption("Yes,~I`d~love~to.", 2)
                .AddOption("Of~course~not.", 255).AddAvatar(92)
                .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            //  client.Inventory.Add(stream, 120159, 1,0,0,0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket,true);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball~and~2,000~Silver!");
                        }
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(92)
                           .FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720967);
                        client.CreateBoxDialog("You`ve~received~the~Moon~Spring.~Hurry~to~test~if~it~is~poisonous.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TaoistSpring)]
        public static void TaoistSpring(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistSpring, client.Player.Class, 1501);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistSpring, client.Player.Class, 1502);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.TaoistSpring, client.Player.Class, 1503);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("People believe the Moon Spring is blessed by the god of water. We should be grateful for every drop of water he provides!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Worship~the~Water~God~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Worship~the~Water~God~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Worship~the~Water~God~3.", 20);
                        data.AddOption("Leave.", 255);
                        data.AddAvatar(43).FinalizeDialog();
                        break;



                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                        {
                            data.AddText("Thank you very much. The god will bless the Moon Spring and the whole village!")
            .AddOption("Great!", 22).AddAvatar(43)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Inventory.Contain(720966, 1))
                        {
                            data.AddText("What`s up? You`re going to find the grids where 1 and 6 are located, and then sacrifice the offering to god.")
                  .AddOption("I~see.", 255).AddAvatar(43)
                  .FinalizeDialog();
                            break;
                        }

                        data.AddText("Do you still remember where the number 1 and 6 are? Take Blade Ghost`s Ear and worship the god of water there.")
              .AddOption("Okay.", 21)
              .AddOption("Later.", 255).AddAvatar(43)
              .FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(43)
                           .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Add(stream, 120159, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver,~a~Super~Necklace!");
                        }
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(43)
                           .FinalizeDialog();
                            break;
                        }
                        client.Inventory.Add(stream, 720966);
                        client.CreateBoxDialog("You`ve~received~an~offering.~Hurry~go~to~the~worship~ceremony.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711410, 1) && client.Inventory.Contain(711411, 1)
                                && client.Inventory.Contain(711412, 1) && client.Inventory.Contain(711413, 1))
                            {
                                data.AddText("Let me see. Sand Pill, Hill Monster`s Heart, Rock Monster`s Tail and Blade Ghost`s Ear. Yup! They`re what I`m looking for.")
               .AddOption("Good.", 4).AddAvatar(43)
               .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need Sand Pill, Hill Monster`s Heart, Rock Monster`s Tail and Blade Ghost`s Ear. Have you collected all of them?")
               .AddOption("Not~yet.", 255).AddAvatar(43)
               .FinalizeDialog();
                            }

                            break;
                        }

                        data.AddText("We`re going to hold a worship ceremony. I need you to collect some offerings for me.")
                  .AddOption("Go~ahead.", 2)
                  .AddOption("Sorry,~I~have~to~leave.", 255).AddAvatar(43)
                  .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Water: 1 (north) and 6 (northwest). Fire: 7 (west) and 2 (southwest). Gold: 9 (south) and 4 (southeast). Wood: 3 (east) and 8 (northeast). Earth: 5 (center).")
          .AddOption("I`m~ready.", 13).AddAvatar(43)
          .FinalizeDialog();
                            break;
                        }
                        data.AddText("There`re 9 squares marked on the platform. You must learn the rules before you can make a sacrifice.")
           .AddOption("What`re~the~rules?", 11)
           .AddOption("Later.", 255).AddAvatar(43)
           .FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        data.AddText("Where is number 5?")
                          .AddOption("A.~In~the~center.", 14)
                          .AddOption("B.~In~the~southeast.", 255).AddAvatar(43)
                          .FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        data.AddText("Bingo! You gave the correct answer! Good job!")
                            .AddOption("Great.", 15).AddAvatar(43)
                            .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                           .AddOption("Let me check.", 255).AddAvatar(43)
                           .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                        }
                        break;
                    }
                case 11:
                    {
                        data.AddText("Listen. 2, 9 and 4 (from left to right) are in the first line. 3, 5 and 7 are in the middle line. 8, 1 and 6 are in the bottom line.")
                            .AddOption("I~got~it.", 12).AddAvatar(43)
                            .FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Talk~to~Taoist~Spring~to~answer~his~questions.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 4:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711410, 1, stream);
                            client.Inventory.Remove(711411, 1, stream);
                            client.Inventory.Remove(711412, 1, stream);
                            client.Inventory.Remove(711413, 1, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                        }
                        data.AddText("There`re 9 squares marked on the platform. You must learn the rules before you can make a sacrifice.")
              .AddOption("What`re~the~rules?", 11)
              .AddOption("Later.", 255).AddAvatar(43)
              .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        data.AddText("Go kill the monsters and gather a Sand Pill, Hill Monster`s Heart, Rock Monster`s Tail and Blade Ghost`s Ear.")
                 .AddOption("I~see.", 3).AddAvatar(43)
                 .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Hurry~to~help~Taoist~Spring~to~collect~the~offerings.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.ElderJiang)]
        public static void ElderJiang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1508);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1509);// [QuestFix 2026-10-13] was 12022 (no such mission -> null -> dialog crash)
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1510);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1511);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Stop asking me about the Straight Hook. You will get it, someday.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Elder~Jiang~3.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~4.", 30);
                        data.AddOption("I`ll~leave~you~alone.", 255);
                        data.AddAvatar(63).FinalizeDialog();
                        break;



                    }
                case 30:
                    {
                        data.AddText("I`m waiting for someone who is in a higher position.")
                   .AddOption("Roger.", 31)
                   .AddOption("Bye.", 255).AddAvatar(63)
                   .FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("Spring General Xu? I`m waiting for someone who is in a higher position.")
                  .AddOption("Oh.", 21).AddAvatar(63)
                  .FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/6~EXP~Ball~and~1,000~Silver!");
                        }
                        data.AddText("I`m waiting for someone who is in a higher position.")
                        .AddOption("Roger.", 31)
                        .AddOption("Bye.", 255).AddAvatar(63)
                        .FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("You~should~report~it~to~the~DC~Vice~Captain.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why a straight hook? Well, I`m not interested in the fish. I`m waiting for the nobles.")
                     .AddOption("Visit~Spring~General~Xu.", 255).AddAvatar(63)
                     .FinalizeDialog();
                            break;
                        }
                        data.AddText("You want to know why a straight hook? Well, I`m not interested in the fish. I`m waiting for the nobles.")
                       .AddOption("Aha.", 11)
                       .AddOption("See~ya.", 255).AddAvatar(63)
                       .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~report~it~to~Spring~General~Xu.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711414, 1))
                            {
                                data.AddText("Great! That`s the Straight Hook! Why a straight hook? Haha, you`ll know that soon enough!")
                     .AddOption("Can`t~wait.", 3).AddAvatar(63)
                     .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you got a Straight Hook? Go kill the Blade Ghosts to grab one!")
                         .AddOption("Got~it.", 255).AddAvatar(63)
                         .FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Look at the Server. It`s full of fish! Can you go get a Straight Hook from the Blade Ghost?")
                            .AddOption("Yes,~I~can.", 2)
                            .AddOption("Sorry,~I`m~busy.", 255).AddAvatar(63)
                            .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711414, 1, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball~and~2,000~Silver!");
                        }
                        data.AddText("You want to know why a straight hook? Well, I`m not interested in the fish. I`m waiting for the nobles.")
                        .AddOption("Aha.", 11)
                        .AddOption("See~ya.", 255).AddAvatar(63)
                        .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Blade~Ghosts~to~get~a~Straight~Hook.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ConvoyViceLeader)]
        public static void ConvoyViceLeader(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1485);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1486);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1487);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1488);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1489);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1490);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Leave me alone. I`m busy.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Prove~Myself.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~1.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~2.", 20);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~3.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~4.", 40);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Cactus~5.", 50);

                        data.AddOption("Alright.", 255);
                        data.AddAvatar(95).FinalizeDialog();
                        break;

                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("We have planted cactus on the way to the Moon Spring. It`ll prevent the monsters from getting close. Please inform Vice General Dong of the good news.")
.AddOption("Alright.", 255).AddAvatar(95)
.FinalizeDialog();
                            break;
                        }
                        data.AddText("We have planted cactus on the way to the Moon Spring. It`ll prevent the monsters from getting close. Please inform Vice General Dong of the good news.")
.AddOption("OK.", 51)
.AddOption("Later.", 255).AddAvatar(95)
.FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Hurry~to~head~to~see~Vice~General~Dong.");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);

                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest5.MissionId, 1, 1, 1))
                        {
                            data.AddText("You`d been a great help! Here`s your reward!")
.AddOption("Thanks.", 42)
.AddOption("I`m~busy.", 255).AddAvatar(95)
.FinalizeDialog();
                            break;
                        }
                        data.AddText("I hope you can head to the Moon Spring and plant 3 cactus seeds there.")
.AddOption("You~can~count~on~me.", 41)
.AddOption("I`m~busy.", 255).AddAvatar(95)
.FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                        }
                        data.AddText("We have planted cactus on the way to the Moon Spring. It`ll prevent the monsters from getting close. Please inform Vice General Dong of the good news.")
.AddOption("OK.", 51)
.AddOption("Later.", 255).AddAvatar(95)
.FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        if (!client.Inventory.Contain(720965, 1))
                            client.Inventory.Add(stream, 720965);
                        client.CreateBoxDialog("You`ve~received~some~cactus~seeds.~Hurry~to~go~plant~them.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(720965, 1))
                            {
                                data.AddText("Good job! So we must hurry to plant them.")
 .AddOption("I~agree.", 32).AddAvatar(95)
 .FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("I have an idea. If we plant this cactus outside the village, the monsters won`t dare get close! I hear Wan Ying has the cactus seeds. Would you like to borrow some?")
  .AddOption("Sure.", 255).AddAvatar(95)
  .FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("I have an idea. If we plant this cactus outside the village, the monsters won`t dare get close! I hear Wan Ying has the cactus seeds.")
   .AddOption("I`ll~go~borrow~some.", 31)
   .AddOption("None~of~my~business.", 255).AddAvatar(95)
   .FinalizeDialog();
                        break;
                    }
                case 32:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {


                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                        }
                        data.AddText("I hope you can head to the Moon Spring and plant 3 cactus seeds there.")
.AddOption("You~can~count~on~me.", 41)
.AddOption("I`m~busy.", 255).AddAvatar(95)
.FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        client.CreateBoxDialog("Hurry~to~ask~Wan~Ying~for~some~cactus~seeds.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(720963, 1))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1))
                            {
                                data.AddText("They are afraid of cactus? Great! Now, the real question is why....")
        .AddOption("Yup.", 22).AddAvatar(95)
        .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Go test if the Level 78 Rock Monsters are afraid of the cactus.")
            .AddOption("Okay!", 255).AddAvatar(95)
            .FinalizeDialog();
                                client.CreateBoxDialog("Use~the~Cactus~on~the~Level~78~Rock~Monsters.");
                            }
                            break;
                        }
                        data.AddText("I need you to do me a favor. Wave the cactus in front of the Level 78 Rock Monsters and watch their reaction.")
                         .AddOption("Interesting.", 21)
                        .AddOption("I`m~busy.", 255)
                        .AddAvatar(95)
                        .FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(711403, 1, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                        }
                        data.AddText("I have an idea. If we plant this cactus outside the village, the monsters won`t dare get close! I hear Wan Ying has the cactus seeds.")
      .AddOption("I`ll~go~borrow~some.", 31)
      .AddOption("None~of~my~business.", 255).AddAvatar(95)
      .FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        client.CreateBoxDialog("You`ve~received~a~cactus.~Go~test~if~the~Level~78~Rock~Monsters~are~afraid~of~it.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Inventory.Add(stream, 720963);
                        break;
                    }

                case 10:
                    {
                        if (client.Inventory.Contain(720961, 1))
                        {
                            if (client.Inventory.Contain(711403, 1))
                            {
                                data.AddText("Well done! I`ll work out the secret!")
           .AddOption("Good~luck.", 14).AddAvatar(95)
           .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Level 78 Rock Monsters never get close to the cactus. That`s interesting. Would you like to fetch me a cactus?")
            .AddOption("Sure.", 255).AddAvatar(95)
            .FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Listen, I noticed an interesting thing. The Level 78 Rock Monsters never get close to the cactus. Would you like to fetch me one of those cactus?")
                        .AddText("~I`d like to give it a good once over and see if we can use this to our advantage...")
                            .AddOption("Of~course.", 11)
                           .AddOption("Nope.", 255)
                           .AddAvatar(95)
                           .FinalizeDialog();
                        break;
                    }

                case 14:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(720961, 1, stream);
                            client.Inventory.Remove(711403, 1, stream);
                            client.IncreaseExperience(stream, 2952000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2,952,000~EXP~and~2,000~Silver!");
                        }

                        data.AddText("I need you to do me a favor. Wave the cactus in front of the Level 78 Rock Monsters and watch their reaction.")
                        .AddOption("Interesting.", 21)
                       .AddOption("I`m~busy.", 255)
                       .AddAvatar(95)
                       .FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        data.AddText("Here`s a Sickle. You need to wait 10 seconds each time you use it, or else it could break.")
                            .AddOption("Roger.", 12)
                           .AddAvatar(95)
                           .FinalizeDialog();
                        break;
                    }

                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720961);
                        client.CreateBoxDialog("You`ve~received~a~Sickle.~Go~fetch~a~Cactus!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711402, 1))
                            {
                                data.AddText("Good job! You`re welcome to join us and help protect the village!")
                    .AddOption("Great.", 4)
                    .AddAvatar(95)
                    .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Are you scared? Go get a Monster Tail, then.")
                      .AddOption("I`m~on~my~way.", 255)
                      .AddAvatar(95)
                      .FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("You want to join us? Well, you`ll have to prove yourself, first.")
                            .AddOption("How?", 2)
                            .AddOption("Forget~it.", 255)
                            .AddAvatar(95)
                            .FinalizeDialog();

                        break;
                    }
                case 4:
                    {

                        if (!client.Inventory.HaveSpace(3))
                        {

                            data.AddText("Please make 3 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(95)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711402, 1, stream);
                            client.IncreaseExperience(stream, 711348);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.Inventory.Add(stream, 720173);

                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.SendSysMesage("You`ve~received~some~EXP~3,000~Silver,~a~Small~Joy~Stone~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("Tell you what. Why don`t you go fetch a Monster Tail from the Level 78 Rock Monsters, then.")
                        .AddOption("I`ll~come~back~soon.", 3)
                        .AddAvatar(95)
                        .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Go~fetch~a~Monster~Tail~from~the~Level~78~Rock~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SpringViceGeneralOu)]
        public static void SpringViceGeneralOu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 1494);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 1500);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 6128);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I enjoy studying about smithing and forging powerful weapons for our soldiers!");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Sharp~Blade.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Avenger.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Deliver~the~Water~(Daily).", 20);

                        data.AddOption("Bye.", 255);
                        data.AddAvatar(107).FinalizeDialog();
                        break;

                    }

                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            client.Player.QuestGUI.SendAutoPatcher(1000, 490, 326, (ushort)NpcID.SoldierMu);
                            data.AddText("Hurry to deliver the water to the villagers!")
                      .AddOption("I`ll~go~right~away.", 255)
                      .AddAvatar(107).FinalizeDialog();
                            break;
                        }
                        data.AddText("These days, villagers are too scared because of the Blade Ghosts. I need some volunteers to disguise as Blade Ghosts and go fetch water for the villagers.")
                        .AddOption("Count~me~in.", 21)
                        .AddOption("Not~interested.", 255)
                        .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        client.Player.TransformInfo = new Role.ClientTransform(client.Player);
                        client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, 240, 150, 1360);
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.CreateBoxDialog("You`ve~got~the~water.~Hurry~to~deliver~it~to~Soldier~Mu.");
                        client.Inventory.Add(stream, 711417);
                        client.Player.QuestGUI.SendAutoPatcher(1000, 490, 326, (ushort)NpcID.SoldierMu);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711407, 5))
                            {
                                data.AddText("Thank you! You deserve the reward.")
            .AddOption("My~pleasure.", 13)
            .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need you to kill the Level 83 Blade Ghosts and cut off their ears. Bring me back 5 ears, and I`ll have more work for you.")
                 .AddOption("Okay.", 255)
                 .AddAvatar(107).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I was injured by the Level 83 Blade Ghosts! I need you to give them a lesson!")
                         .AddOption("Go~ahead.", 11)
                         .AddOption("Sorry~to~hear~that.", 255)
                         .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 13:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(711407, 5, stream);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.AddItemWitchStack(720172, 0, 3, stream);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~3~EXPBallScrap!.");
                        }
                        break;
                    }
                case 11:
                    {
                        data.AddText("I need you kill them and cut their ears. What about bringing me 5 ears?")
                        .AddOption("No~problem.", 12)
                        .AddAvatar(107).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        client.CreateBoxDialog("Hurry~to~collect~5~ears~from~the~Level~89~Blade~Ghosts.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }

                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711406, 5))
                            {
                                data.AddText("That`s perfect. Thank you! Please, take your reward!")
            .AddOption("Thanks.", 4)
            .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I need you to gather 5 Sharp Blades from the Blade Ghosts.")
                 .AddOption("I~won`t~disappoint~you.", 255)
                 .AddAvatar(95).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I`m quite interested in the Blade Ghost`s weapon, the Sharp Blade.")
                         .AddOption("I`ll~go~get~some~for~you.", 2)
                         .AddOption("Leave.", 255)
                         .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711406, 5, stream);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Meteor!");


                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("The Blade Ghosts out there are carrying the Sharp Blades. I need 5, if you think you can get them.")
                        .AddOption("I`ll~be~back~soon.", 3)
                        .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.CreateBoxDialog("Go~kill~the~Blade~Ghosts~to~collect~5~Sharp~Blades.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.SculptorHe)]
        public static void SculptorHe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SculptorHe, client.Player.Class, 1496);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SculptorHe, client.Player.Class, 1497);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SculptorHe, client.Player.Class, 1498);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, I`m Sculptor He. Here, I have many of my different sculptures and busts. Any interest in purchasing, perhaps?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inscription~1.", 1);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inscription~2.", 10);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Inscription~3.", 20);
                        data.AddOption("Good.", 255);
                        data.AddAvatar(63).FinalizeDialog();
                        break;

                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 3))
                            {
                                data.AddText("I really appreciate your help. Here`s your reward!")
                         .AddOption("Thanks.", 22)
                         .AddAvatar(63).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The noise I make when I hit the stone has enraged the Blade Ghosts. I hope you can protect me while I`m working, and hold back all the Angry Blade Ghosts.")
                                    .AddOption("Glad~to~help.", 21)
                                    .AddOption("I`ll~think~about~it.", 255)
                                    .AddAvatar(63).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("The noise I make when I hit the stone has enraged the Blade Ghosts. I hope you can protect me while I`m working, and hold back all the Angry Blade Ghosts.")
                           .AddOption("Glad~to~help.", 21)
                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(63)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.Add(stream, 150159, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);

                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~a~Super~Ring!");


                        }
                        break;
                    }
                case 21:
                    {
                        bool accept = true;
                        foreach (var mob in client.Player.View.Roles(Role.MapObjectType.Monster))
                        {
                            if ((mob as Game.MsgMonster.MonsterRole).Family.ID == 55434417)
                            {
                                accept = false;
                                break;
                            }
                        }
                        if (accept)
                        {
                            Server.AddMapMonster(stream, client.Map, 55434417, client.Player.X, client.Player.Y, 7, 7, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                            Server.AddMapMonster(stream, client.Map, 55434417, client.Player.X, client.Player.Y, 7, 7, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                            Server.AddMapMonster(stream, client.Map, 55434417, client.Player.X, client.Player.Y, 7, 7, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                            client.CreateBoxDialog("The~noise~brought~3~Angry~Blade~Ghosts.~Kill~them,~all!");
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711409, 1))
                            {
                                data.AddText("Great! I`m working on the inscription, right now.")
                         .AddOption("Take~your~time.", 12)
                         .AddAvatar(63).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What are you waiting for? I need you to get a Sharp Axe from the Blade Ghosts!")
                           .AddOption("OK.", 255)
                           .AddAvatar(63).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("Oh, my! The chisel is broken! Listen, you must help me. I need you to get a Sharp Axe from the Blade Ghosts!")
                           .AddOption("I~won`t~disappoint~you.", 11)
                           .AddOption("I`ll~think~about~it.", 255)
                           .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.Remove(711409, 1, stream);

                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball~and~3,000~Silver!");


                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Blade~Ghosts~to~gain~a~Sharp~Axe.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711408, 1))
                            {
                                data.AddText("Yup, that`s the Inscription. Thank you very much.")
                         .AddOption("You`re~welcome.", 3)
                         .AddAvatar(63).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What happened? I need you to go fetch the Inscription from the DC Vice Captain.")
                           .AddOption("I`ll~go~right~away.", 255)
                           .AddAvatar(63).FinalizeDialog();
                            }
                            break;
                        }

                        data.AddText("I need you to do me a favor. Please go visit the DC Vice Captain and fetch the Inscription. I`ll make some changes.")
                           .AddOption("You~can~count~on~me.", 2)
                           .AddOption("Sorry,~I`m~busy.", 255)
                           .AddAvatar(63).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Head~to~see~the~DC~Vice~Captain~to~fetch~the~Inscription.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(63)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.Remove(711408, 1, stream);
                            client.Inventory.AddSteed(stream, 300000, 1, 1, true, 255, 0, 150);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~a~+1~Steed(B)!");


                        }
                        break;
                    }

            }

        }
        [NpcAttribute(NpcID.SpringGeneralXu)]
        public static void SpringGeneralXu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1492);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1493);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1499);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ElderJiang, client.Player.Class, 1509);// [QuestFix 2026-10-13] was 12022 (no such mission -> null -> dialog crash)
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringGeneralXu, client.Player.Class, 1510);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My job is to guard the Moon Spring.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Elder~Jiang~2.", 30);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Elder~Jiang~3.", 40);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Visit~Spring~General.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Protect~the~Moon~Spring.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Blade~Ghosts~(L83).", 20);

                        data.AddOption("Good.", 255);
                        data.AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 40:
                    {
                        data.AddText("I really want to pay a visit to the elder. Please go ask if he is free to talk.")
                       .AddOption("I`d~love~to..", 41)
                       .AddOption("Why~not~go~there~yourself?", 255)
                       .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 41:
                    {
                        client.CreateBoxDialog("Hurry~to~visit~Elder~Jiang.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        data.AddText("What? Catch fish on a straight hook and above the water? That`s interesting.")
                      .AddOption("Yeah..", 31)
                      .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(107)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.IncreaseExperience(stream, 2711348, questReward: true);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/6~EXP~Ball,~1,000~Silver~and~5~Panaceas!");
                        }
                        data.AddText("I really want to pay a visit to the elder. Please go ask if he is free to talk.")
                        .AddOption("I`d~love~to..", 41)
                        .AddOption("Why~not~go~there~yourself?", 255)
                        .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 30))
                            {
                                data.AddText("Thanks for what you`ve done for us.")
                              .AddOption("You`re~welcome.", 22)
                              .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Have you killed 30 Level 83 Blade Ghosts, already?")
                                .AddOption("Not~yet.", 255)
                                .AddAvatar(107).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Have you ever seen the Level 83 Blade Ghosts? They`re smaller than Blade Ghosts, but much crueler. Well, I need you to go kill 30 of them!")
                                  .AddOption("I~won`t~let~you~down.", 21)
                                  .AddOption("Bye.", 255)
                                  .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(104)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);

                            client.Inventory.Add(stream, 730002, 1, 1);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+2~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.SendSysMesage("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver,~and~a~+2~Stone~(B)!", MsgMessage.ChatMode.System);

                        }
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~30~Level~83~Blade~Ghosts.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 50))
                            {
                                data.AddText("Keep up the good job. Here`s your reward!")
                                   .AddOption("Thanks.", 12)
                                   .AddAvatar(107).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What are you waiting for? Go kill 50 Blade Ghosts!")
                                    .AddOption("I`m~on~my~way.", 255)
                                    .AddAvatar(107).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I don`t know why, but more and more Blade Ghosts are gathering outside the village. I need you to kill 50 of them!")
                                   .AddOption("You~can~count~on~me.", 11)
                                   .AddOption("Just~passing~by.", 255)
                                   .AddAvatar(107).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {

                            data.AddText("Please make 3 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(104)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.SendSysMesage("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver,~5~Panaceas~and~2~Meteors!", MsgMessage.ChatMode.System);

                        }
                        break;
                    }
                case 11:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.CreateBoxDialog("Hurry~to~kill~50~Blade~Ghosts.");
                        break;
                    }
                case 1:
                    {
                        data.AddText("It`s a good idea! I`ll ask the villagers to plant some cactus!")
                                     .AddOption("That~would~be~great!", 2)
                                     .AddAvatar(107).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.Add(stream, 720172);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball,~2,000~Silver~and~an~EXP~Ball~Scrap!");


                        }
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.ConvoyLeaderGu)]
        public static void ConvoyLeaderGu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierMu, client.Player.Class, 1483);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1484);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1491);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1492);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Nice to meet you. I`m new here.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Visit~the~Convoy~Leader.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Leader`s~Trouble.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Thunder~Stone.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Visit~Spring~General.", 30);

                        data.AddOption("Nice~to~meet~you,~too.", 255);
                        data.AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        data.AddText("Spring General Xu is guarding the Moon Spring. He needs your help.")
                                      .AddOption("I`ll~go~visit~him.", 31)
                                      .AddAvatar(104).FinalizeDialog();
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
                            if (client.Inventory.Contain(711405, 1))
                            {
                                client.Player.QuestGUI.SendAutoPatcher(1000, 482, 339, (ushort)NpcID.IronsmithLi);
                            }
                            else
                            {
                                data.AddText("Do you know anything about the Thunder Stone? I saw the Level 78 Rock Monsters carrying them. Please go grab one and deliver it to Ironsmith Li.")
                                          .AddOption("I`ll~do~right~away.", 255)
                                          .AddAvatar(104).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("Do you know anything about the Thunder Stone? I saw the Level 78 Rock Monsters carrying them. Please go grab one and deliver it to Ironsmith Li.")
                                       .AddOption("You~can~count~on~me.", 21)
                                       .AddOption("I`m~afraid~I~can`t~help.", 255)
                                       .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Level~78~Rock~Monsters~to~grab~a~Thunder~Stone.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                data.AddText("Thanks for your help! Please accept this reward!")
                                          .AddOption("Okay.", 12)
                                          .AddAvatar(104).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Level 78 Rock Monsters keep making troubles. They either attack villagers or steal the water. I need you to kill 30 of them!")
                                            .AddOption("I`m~on~my~way.", 255)
                                            .AddAvatar(104).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("The Level 78 Rock Monsters keep making trouble. They either attack villagers or steal any water they can find. I need you to kill 30 of them!")
                           .AddOption("You~can~count~on~me.", 11)
                           .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");


                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Go~kill~30~Level~78~Rock~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Thanks. I`ll turn to you if I need your help.")
                            .AddOption("Yup.", 2)
                            .AddAvatar(104).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(104)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(720011, 0, 5, stream);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~2,000~Silver~and~5~Panaceas!");


                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.SoldierMu)]
        public static void SoldierMu(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierMu, client.Player.Class, 1480);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SoldierMu, client.Player.Class, 1483);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringViceGeneralOu, client.Player.Class, 6128);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`ll give my life to guard this village. It`s my sworn duty!");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Deliver~the~Water~(Daily).", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Rebuild~the~Village.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Visit~the~Convoy~Leader.", 10);

                        data.AddOption("Bye.", 255);
                        data.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 20:
                    {
                        data.AddText("You brought the spring! Thank you!")
.AddOption("My~pleasure.", 21)
.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 21:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(711417, 1, stream);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/3~EXP~Ball~and~2,000~Silver!");
                            client.Player.QuestGUI.SendAutoPatcher(1000, 339, 244, (ushort)NpcID.ConvoyViceLeader);

                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            data.AddText("A team of soldiers were sent to protect the way to Moon Spring. Go visit Convoy Leader Gu and see if he needs any help.")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(101).FinalizeDialog();
                            break;
                        }
                        data.AddText("A team of soldiers were sent to protect the way to Moon Spring. Go visit Convoy Leader Gu and see if he needs any help.")
.AddOption("No~problem.", 11)
.AddOption("Later.", 255)
.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~visit~Convoy~Leader~Gu.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                        break;
                    }
                case 1:
                    {

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711349, 5))
                            {
                                data.AddText("Ah, so many Colorful Stones! Thank you! You`d been a great help!")
.AddOption("Welcome~to~Conquer~Online.", 3)
.AddAvatar(101).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("I`m gathering some Colorful Stones to rebuild the village. Please kill the Rock Monsters to collect 5 Colorful Stones for me.")
.AddOption("I`m~on~my~way.", 2)
.AddAvatar(101).FinalizeDialog();
                            }
                            break;
                        }


                        data.AddText("The Rock Monsters are carrying these Colorful Stones. Can you collect 5 for me?")
.AddOption("Sure!", 2)
.AddAvatar(101).FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Go~kill~the~Rock~Monsters~to~collect~5~Colorful~Stones!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(101)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711349, 5, stream);
                            client.Inventory.Add(stream, Database.ItemType.DoubleExp, 1, 1);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~ExpPotion!");


                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.HanCheng)]
        public static void HanCheng(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1475);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HanCheng, client.Player.Class, 1476);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Be on alert while passing through the desert.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Water~Delivery.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Water~Delivery.", 10);


                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(60).FinalizeDialog();
                        break;
                    }

                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 40, 10))
                            {
                                data.AddText("Good job! Here`s your reward!")
.AddOption("Thanks.", 12)
.AddAvatar(60).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Those Hill Monsters are really annoying. They often tear up trees in the desert.")
                                    .AddText("~Go kill 40 Hill Monsters and 10 Hill Monsters L73!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(60).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("I notice that the Hill Monsters often tear up these trees. We must stop them!")
                            .AddText(" ~I need you to kill 40 Hill Monsters and 10 Hill Monsters L73!")
.AddOption("I`ll~give~them~a~lesson.", 11)
.AddOption("Let~me~think~about~it.", 255)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {

                            data.AddText("Please make 2 more spaces in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
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

                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Hill~Monsters~and~Level~73~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        data.AddText("I really appreciate your help.")
.AddOption("You`re~welcome.", 2)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 2:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711416, 1, stream);
                            client.IncreaseExperience(stream, 711348, questReward: true);
                            client.Inventory.Add(stream, 723340);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Diligence~Book!");

                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DCWanYing)]
        public static void DCWanYing(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1470);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1473);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1475);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.HanCheng, client.Player.Class, 1476);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1488);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("My husband, Han Cheng, and I have lived here since we were born.")
                            .AddText("~The village was kind of beautiful, but all has been destroyed by the sands");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Cactus~3.", 50);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Green~Seeds~1.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Green~Seeds~1.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hill~Monster`s~Armor.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Water~Delivery.", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                            data.AddOption("The~Desert~Herb.", 40);


                        data.AddOption("I~agree.", 255);
                        data.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 50:
                    {
                        if (client.Inventory.Contain(720965, 1))
                        {
                            data.AddText("You have already got the cactus seeds, haven`t you?")
.AddOption("Oh,~yup.", 255)
.AddAvatar(159).FinalizeDialog();
                            break;
                        }
                        data.AddText("You want to buy cactus seeds? Yes, I have some, but each bag costs 500 Silver.")
.AddOption("I`ll~buy.", 51)
.AddOption("Forget~it.", 255)
.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720965);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711401, 1))
                            {
                                data.AddText("Thank you! You saved my husband!")
.AddOption("My~pleasure.", 43)
.AddAvatar(60).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Please take the Desert Shovel and dig for a Desert Herb in the desert. Notice that you`ll need to wait 1 minute every time before you use the Desert Shovel.")
.AddOption("Roger.", 255)
.AddAvatar(60).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("My husband was wounded the other day. The doctor said only the Desert Herb could cure him. Please, I need your help to pick some.")
.AddOption("Where~to~find~it?", 41)
.AddOption("Let~me~think~about~it.", 255)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 43:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.Inventory.Add(stream, 730001, 1);
                            client.Inventory.Remove(720962, 1, stream);
                            client.Inventory.Remove(711401, 1, stream);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");

                        }
                        break;
                    }
                case 41:
                    {
                        data.AddText("I`ll give you a Desert Shovel. Please go to the desert and dig for a Desert Herb.")
.AddOption("I`ll~be~back~soon.", 42)
.AddAvatar(60).FinalizeDialog();
                        break;
                    }
                case 42:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720962);
                        client.CreateBoxDialog("You`ve~received~a~Desert~Shovel.~Hurry~to~dig~for~a~Desert~Herb.");
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        break;
                    }
                case 30:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Inventory.Contain(711348, 1))
                            {
                                if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                                {
                                    client.Inventory.Remove(711348, 1, stream);
                                    client.IncreaseExperience(stream, 2919000, questReward: true);
                                    client.Player.Money += 3000;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");

                                }
                            }
                            else
                            {
                                data.AddText("My husband, Han Cheng, must be patrolling the desert now.")
                               .AddText("~Could you fetch some water from the Server and take it to my husband?")
    .AddOption("I`m~on~my~way.", 255)
    .AddAvatar(159).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("My husband, Han Cheng, should be out patrolling the desert right now, but he forgot to take his water.")
                            .AddText("~Can you fetch some water from the Server and take it to my husband?")
.AddOption("No~problem.", 31)
.AddOption("I`m~busy.", 255)
.AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720974);
                        client.CreateBoxDialog("You`ve~received~a~bucket.~Hurry~to~fetch~some~water.");
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        break;
                    }

                case 20:
                    {
                        if (client.Inventory.Contain(711416, 1))
                        {
                            data.AddText("Is this really the Hill Monster`s Armor? Thank you! I`ll give it to my husband, right away!")
                                .AddOption("Nice.", 22)
                                .AddAvatar(159).FinalizeDialog();

                            break;
                        }
                        data.AddText("People say Level 73 Hill Monster`s armor can protect against wind and sand better than anything. I wish my husband had one!")
.AddOption("Let~me~help.", 21)
.AddOption("None~of~my~business.", 255)
.AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(711416, 1, stream);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");

                        }
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Go~get~an~armor~from~the~Level~73~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 1, 1))
                            {
                                data.AddText("Amazing! You`d been a great help!")
.AddOption("Never~mind.", 12)
.AddAvatar(159).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Now, let`s plant the tree seeds near the water sources. Looks like these places are all inside Hill Monsters territory, though. Would you like to go plant the seeds there?")
.AddOption("I`d~love~to.", 255)
.AddAvatar(159).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Now, let`s plant the tree seeds near the water sources. Looks like these places are all inside Hill Monsters territory, though. Would you like to go plant the seeds there?")
.AddOption("Sure.", 11)
.AddOption("Not~interested.", 255)
.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Remove(720972, 1, stream);
                            client.Inventory.Add(stream, Database.ItemType.DoubleExp);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~3,000~Silver~and~an~EXP~Potion!");

                        }
                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {

                            data.AddText("Please make 1 more space in your inventory.")
            .AddOption("Let me check.", 255).AddAvatar(159)
            .FinalizeDialog();
                            return;
                        }
                        client.Inventory.Add(stream, 720972);
                        client.CreateBoxDialog("Go~plant~the~tree~seeds~near~the~water~sources.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                            {
                                data.AddText("Wow, the Tree Seeds you brought are wonderful! Thanks for your help!")
.AddOption("You`re~welcome.", 3)
.AddAvatar(159).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Few plants can live in the desert, but there are several green trees near the portal leading to the Wind Plain. Can you go there and bring me 3 Tree Seeds?")
.AddOption("Sure.", 255)
.AddAvatar(159).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Things wouldn`t have been so terrible if there were more green plants and flowers.")
                            .AddText("~Could you go to the portal leading to Wind Plain, and bring me 3 Tree Seeds?")
.AddOption("Sure.", 2)
.AddOption("I`m~afraid~not.", 255)
.AddAvatar(159).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(720971, 3, stream);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                            goto case 10;
                        }
                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~head~to~the~Green~Trees~and~fetch~3~Tree~Seeds.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.st1TreeSeed)]
        public static void st1TreeSeed(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                //client.Player.QuestGUI.IncreaseQuestObjectives(
                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1))
                {
                    data.AddText("You`ve already collected a seed from this tree.")
.AddOption("Alright.", 255)
.FinalizeDialog();
                }
                else
                {
                    if (!client.Inventory.HaveSpace(1))
                    {

                        data.AddText("Please make 1 more space in your inventory.")
        .AddOption("Let me check.", 255)
        .FinalizeDialog();
                        return;
                    }

                    client.Player.AddPick(stream, "Collect", 4);
                }
            }
        }
        [NpcAttribute(NpcID.nd2TreeSeed)]
        public static void nd2TreeSeed(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);
            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 1))
                {
                    data.AddText("You`ve already collected a seed from this tree.")
.AddOption("Alright.", 255)
.FinalizeDialog();
                }
                else
                {
                    if (!client.Inventory.HaveSpace(1))
                    {

                        data.AddText("Please make 1 more space in your inventory.")
        .AddOption("Let me check.", 255)
        .FinalizeDialog();
                        return;
                    }
                    client.Player.AddPick(stream, "Collect", 4);
                }
            }
        }
        [NpcAttribute(NpcID.rd3TreeSeed)]
        public static void rd3TreeSeed(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCWanYing, client.Player.Class, 1469);

            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 0, 0, 1))
                {
                    data.AddText("You`ve already collected a seed from this tree.")
.AddOption("Alright.", 255)
.FinalizeDialog();
                }
                else
                {
                    if (!client.Inventory.HaveSpace(1))
                    {

                        data.AddText("Please make 1 more space in your inventory.")
        .AddOption("Let me check.", 255)
        .FinalizeDialog();
                        return;
                    }
                    client.Player.AddPick(stream, "Collect", 4);
                }
            }
        }
        [NpcAttribute(NpcID.GeneralZhuGe)]
        public static void GeneralZhuGe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.DCViceCaptain, client.Player.Class, 1465);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1466);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1471);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1479);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1477);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SpringSoldierZhao, client.Player.Class, 1505);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralZhuGe, client.Player.Class, 1506);

            switch (Option)
            {
                case 0:
                    {


                        data.AddText("It was a nice village in Desert, but turned into a refuge overnight after the sandstorm. Well, at least the villagers haven`t lost their hearts.");

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Speedy~Rescue.", 60);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Poisonous~Water~2.", 40);
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                           && client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Poisonous~Water~3.", 50);

                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Hill~Monster`s~Threat.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Kill~the~Hill~Monsters~(L73).", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Retrieve~the~Water~Source.", 20);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            if (client.Inventory.Contain(720975, 1))
                                data.AddOption("Thank you for the letter.", 30);

                        data.AddOption("They`ll~get~better.", 255);
                        data.AddAvatar(54).FinalizeDialog();


                        break;
                    }
                case 50:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You`ve got the Antidote? Hurry to deliver it to Spring Soldier Zhao.")
                      .AddOption("Okay.", 255)
                      .AddAvatar(54).FinalizeDialog();
                            break;
                        }
                        data.AddText("All clues are indicating the Level Blade Ghosts! I need you to kill them to get the Antidote and take it to Spring Soldier Zhao.")
                        .AddOption("No~problem.", 51)
                        .AddOption("None~of~my~business.", 255)
                        .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 51:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Level~83~Blade~Ghosts!");
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        break;
                    }
                case 40:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
                        {
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~2,000~Silver!");
                        }
                        data.AddText("All clues are indicating the Level Blade Ghosts! I need you to kill them to get the Antidote and take it to Spring Soldier Zhao.")
                            .AddOption("No~problem.", 51)
                            .AddOption("None~of~my~business.", 255)
                            .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 41:
                    {

                        data.AddText("Villagers are poisoned by the spring! I`ll figure that out!")
                        .AddOption("Great.", 41)
                        .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 30:
                    {

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(720975, 1, stream);
                            client.Inventory.Add(stream, 1060021);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Desert~City~Gate!");
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 50))
                            {
                                data.AddText("Thank you! The villagers will be able to go out and fetch water, again.")
.AddOption("Great.", 22)
.AddAvatar(54).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("What are you waiting for? Go kill 50 Rock Monsters!")
.AddOption("I`m~on~my~way.", 255)
.AddAvatar(54).FinalizeDialog();

                            }

                            break;
                        }
                        data.AddText("Oh-oh! The Rock Monsters on the west of the camp blocked the way to the Moon Spring! That`s the most important water source in the desert.")
                        .AddText("~I need you to kill 50 of them!")
.AddOption("You~can~count~on~me.", 21)
.AddOption("Bye.", 255)
.AddAvatar(54).FinalizeDialog();

                        break;
                    }
                case 22:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP~and~3,000~Silver!");
                        }

                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~50~Rock~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);


                        break;

                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 30))
                            {
                                data.AddText("Great! That may be enough to keep them from being so bold.")

.AddOption("I~hope~so.", 12)
.AddAvatar(54).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("A horde of Hill Monsters L73 are going to attack the camp.")
                          .AddText("~To weaken their power, I need you to kill 30 of them!")
.AddOption("I`m~on~my~way.", 255)
 .AddAvatar(54).FinalizeDialog();
                            }


                            break;
                        }
                        data.AddText("It`s reported that some Hill Monsters L73 are going to attack the camp, tonight!")
                            .AddText("~A good offense is the best defense! Go kill 30 Hill Monsters L73, right away!")
  .AddOption("I~won`t~disappoint~you.", 11)
  .AddOption("Why~me?", 255)
   .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.Inventory.Add(stream, 720172);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~an~EXP~Ball~Scrap!");
                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~30~Level~73~Hill~Monsters!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 50))
                            {
                                data.AddText("Well done! Please take these gifts, as your rewards.")
.AddOption("Thanks.", 3)
.AddAvatar(54).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("We should do something to save the camp. Go kill 50 of the Hill Monsters, and cut down their power!")
.AddOption("I~won`t~disappoint~you.", 255)
 .AddAvatar(54).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("More and more Hill Monsters are gathering outside the camp! It`s really dangerous.")
                            .AddText("~I need you to kill 50 of them!")
  .AddOption("I~won`t~disappoint~you.", 2)
  .AddOption("Why~not~do~that~yourself?", 255)
   .AddAvatar(54).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {

                            client.Inventory.Add(stream, 723340);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~Diligence~Book!");
                        }

                        break;
                    }
                case 2:
                    {

                        client.CreateBoxDialog("Hurry~to~kill~50~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        break;
                    }
                case 60:
                    {

                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest7.MissionId, 20))
                            {
                                data.AddText("Really? You`ve cleared the way? Good job!")
                       .AddOption("Never~mind.", 62)
                       .AddAvatar(54).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("You came at the right time! A refugee camp in Desert was surrounded by a group of Hill Monsters.")
                         .AddText("~Go kill 20 of them, and report back to General ZhuGe!")
                          .AddOption("Okay.", 255)
                          .AddAvatar(54).FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 62:
                    {

                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 mores space in your inventory.")
         .AddOption("Let me check.", 255)
         .AddAvatar(53).FinalizeDialog();
                            break;
                        }

                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest7.MissionId))
                        {
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 1739000, questReward: true);
                            client.SendSysMesage("You've~received~some~EXP,~2,000~Silver,~5~Panaceas~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.Player.Money += 1500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                    }
                    break;
            }

        }

        [NpcAttribute(NpcID.IronsmithLi)]
        public static void IronsmithLi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.IronsmithLi, client.Player.Class, 1467);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IronsmithLi, client.Player.Class, 1472);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.IronsmithLi, client.Player.Class, 1481);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyLeaderGu, client.Player.Class, 1491);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Hey, I`m an ironsmith. What can I do for you? You need any smithing done, by chance?");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("Stolen~Grindstone.", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Ironsmith`s~Request.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~New~Weapon.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Stolen~Grindstone.", 20);

                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 30:
                    {
                        if (client.Inventory.Contain(711405, 1))
                        {
                            data.AddText("So is it a Thunder Stone? I`ll reveal the secret.")
                                     .AddOption("Good~luck.", 31)
                                     .AddAvatar(53).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Do you know anything about the Thunder Stone? I saw the Level 78 Rock Monsters carrying them. Please go grab one and deliver it to Ironsmith Li.")
                                      .AddOption("I`ll~do~right~away.", 255)
                                      .AddAvatar(53).FinalizeDialog();
                        }
                        break;
                    }
                case 31:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(711405, 1, stream);
                            client.Inventory.Add(stream, 1060021);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~a~Desert~City~Gate!");
                        }
                        break;
                    }
                case 20:
                    {

                        if (client.Inventory.Contain(711399, 1))
                        {
                            data.AddText("Yes! That`s my grindstone! Thank you!")
.AddOption("No~problem.", 22)
.AddAvatar(53).FinalizeDialog();

                            break;
                        }


                        data.AddText("My grindstone was stolen by the Rock Monsters! It`s useless for them but to me, it`s important. Please, go retrieve it!")
      .AddOption("You~can~count~on~me.", 21)
      .AddOption("I`ll~think~about~it.", 255)
       .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.Inventory.Remove(711399, 1, stream);
                            client.Inventory.Add(stream, 720172);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You`ve~received~EXP~worth~1/2~EXP~Ball,~3,000~Silver~and~an~EXP~Ball~Scrap!");
                        }
                        break;
                    }
                case 21:
                    {
                        client.CreateBoxDialog("Hurry~to~kill~the~Rock~Monsters~to~retrieve~the~grindstone.");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 20))
                            {
                                data.AddText("Great! I`ll go report to the General. Here`s your reward, by the way.")

    .AddOption("Thank~you.", 12)
     .AddAvatar(53).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("This is my Newly Made Hammer. I need you to test it.")
                          .AddText("~Just bring it with you and kill 20 Hill Monsters L73!")
         .AddOption("I`m~leaving.", 255)
          .AddAvatar(53).FinalizeDialog();
                            }

                            break;
                        }
                        data.AddText("Haha, this is my Newly Made Hammer. I need you to test it out.")
                          .AddText("~Just bring it with you and kill 20 Hill Monsters L73!")
         .AddOption("I`ll~try!", 11)
         .AddOption("I`m~going~anywhere.", 255)
          .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {

                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~a~+1~Stone~(B)!");
                        }
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("You`ve~received~a~Newly~Made~Hammer.~Go~kill~20~Level~73~Hill~Monsters.");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {

                            if (client.Inventory.Contain(711344, 5, 0))
                            {
                                data.AddText("See? These hammers are great! Thank you very much!")
           .AddOption("My~pleasure.", 3)
            .AddAvatar(53).FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("With the help of Hill Monster`s hammers, I`ll make better weapons.")
           .AddOption("I`ll~go~get~5~for~you.", 255)
            .AddAvatar(53).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("To be frank, I`m quite interested in the Hill Monster`s hammer. Could you collect some for me?")
            .AddOption("OK.~I`ll~go~get~5~for~you.", 2)
            .AddOption("Sorry,~I~can`t~help.", 255)
             .AddAvatar(53).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(711344, 5, stream);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.Inventory.Add(stream, 720173);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silvers,~a~Small~Joy~Stone~and~2~Meteors!");
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.CreateBoxDialog("Hurry~to~kill~the~Hill~Monsters~to~collect~5~Hammers!");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.ViceGeneralDong)]
        public static void ViceGeneralDong(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneralDong, client.Player.Class, 1468);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneralDong, client.Player.Class, 1474);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ViceGeneralDong, client.Player.Class, 1478);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ConvoyViceLeader, client.Player.Class, 1490);

            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m the Vice General, you can call me Dong. Watching the refugee camp is my duty.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            data.AddOption("The~Cactus~5.", 30);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("Go~Deep~into~the~Lair.", 1);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Supply~Line.", 10);
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                            data.AddOption("The~Sand-proof~Wall.", 20);


                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 30:
                    {

                        data.AddText("Good idea! I`ll ask the villagers to plant more of the cactus. Thanks for your advice!")
               .AddOption("You`re~welcome.", 31)
                .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Player.QuestGUI.SendAutoPatcher(1000, 308, 456, (ushort)NpcID.ElderJiang);

                            client.Inventory.Add(stream, 160157, 1);
                            client.Inventory.Add(stream, 730001, 1, 1);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.SendSysMesage("You`ve~received~some~EXP,~2,000~Silver,~Tiger~Boots~and~a~+1~Stone~(B)!", MsgMessage.ChatMode.System);
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 1, 1, 1, 1))
                            {
                                data.AddText("I`m the Vice General, you can call me Dong. Watching the refugee camp is my duty.")
               .AddOption("The~Slabstones.", 25)
               .AddOption("Nice~to~see~you.", 255)
                .AddAvatar(59).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The Rock Monsters are known for carrying fine Slabstones. You can collect some, and use them to build a wall around the Server in 4 directions.")
                .AddOption("I`ll~come~back~soon.", 255)
                 .AddAvatar(59).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("The sand is swallowing the water Server. We need to collect some slabstones to build a sand-proof wall around the Server.")
                       .AddOption("Ready~to~help.", 21)
                       .AddOption("I`ll~think~about~it.", 255)
                        .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 25:
                    {
                        data.AddText("You`ve built the wall! You`ve been such a big help!")
               .AddOption("My~pleasure.", 26)
                .AddAvatar(59).FinalizeDialog();

                        break;
                    }
                case 26:
                    {
                        if (!client.Inventory.HaveSpace(3))
                        {
                            data.AddText("Please make 3 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {

                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.SendSysMesage("You`ve~received~by~the~Vice~General~Dong~with~EXP~worth~1/2~EXP~Ball,~3,000~Silver,~and~5~Panaceas,~for~building~the~sand~prof~wall!", MsgMessage.ChatMode.System);
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                        }
                        break;
                    }
                case 21:
                    {
                        data.AddText("You`re going to turn to the Rock Monsters for the Slabstones. After that, go build a wall around the water Server in 4 directions.")
                     .AddOption("I`ll~come~back~soon.", 22)
                      .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(4))
                        {
                            data.AddText("Please make 4 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        client.Inventory.AddItemWitchStack(720970, 0, 4, stream);
                        client.CreateBoxDialog("Go~kill~the~Rock~Monsters,~and~gather~some~Slabstones!");
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(711415, 5))
                        {
                            data.AddText("Really? You`ve gathered enough Heavy Axes? Thank you!")
                      .AddOption("Welcome.", 12)
                       .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        data.AddText("Emergency! Our armory is running low on weapons and the supply lines are cut off!")
                            .AddText("~I need you to get 5 Heavy Axes from the Hill Monsters or Hill Monsters L73!")
                         .AddOption("Got~it.", 11)
                         .AddOption("I`ll~think~about~it.", 255)
                          .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 11:
                    {
                        client.CreateBoxDialog("Kill~the~Hill~Monsters~and~Level~73~Hill~Monsters~to~get~5~Heavy~Axes!");
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {

                            client.Inventory.Remove(711415, 5, stream);
                            client.Inventory.AddItemWitchStack(1002000, 0, 5, stream);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~3,000~Silver~and~5~Panaceas!");
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 1:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest.MissionId, 1, 1, 1))
                        {
                            data.AddText("You made it? Were they scared? Haha!")
                       .AddOption("Yeah,~yeah.", 3)
                        .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        data.AddText("We have a plan. We need a brave man, like you, to go deep into the Hill Monster`s lair and set a fire. That should smoke them out!")
                        .AddOption("Count~me~in.", 2)
                        .AddOption("I`ll~think~about~it.", 255)
                         .AddAvatar(59).FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(720980, 1, stream);
                            client.SendSysMesage("You`ve~received~some~EXP,~2,000~Silvers~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 2500;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        break;
                    }
                case 2:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(59).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            break;
                        }
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        client.Inventory.Add(stream, 720980);
                        client.CreateBoxDialog("You`ve~got~a~torch.~Hurry~to~set~a~fire~and~threaten~the~Hill~Monsters!");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.KeYulun)]
        public static void KeYulun(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1462);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1463);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.KeYulun, client.Player.Class, 1464);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This desert is often called the throat of the Silk Road. Merchants from all over the world came here, as well as evildoers.");
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished)
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Clear~the~Path.", 20);
                        }
                        else
                        {
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Ginseng~Delivery.", 1);
                            if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                data.AddOption("Save~the~Company.", 10);
                        }
                        data.AddOption("Thanks.", 255);
                        data.AddAvatar(47).FinalizeDialog();

                        break;
                    }
                case 20:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest3.MissionId, 40, 10))
                        {
                            data.AddText("Great! We`re going home! Thanks a million!")
                          .AddOption("Have~a~good~journey.", 22)
                           .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        data.AddText("My followers and I are planning to go home right away. But the problem is, the path is blocked by the bloody monsters, again!")
                            .AddText("~I`m wondering if you can help us kill 40 Sand Monsters and 10 Level 68 Sand Monsters.")
                            .AddOption("You~can~count~on~me.", 21)
                            .AddOption("Sorry,~I`m~busy.", 255)
                             .AddAvatar(47).FinalizeDialog();
                        break;
                    }
                case 22:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
                        {
                            client.SendSysMesage("You`ve~received~some~EXP,~3,000~Silvers,~Unique~Snakeskin~Boots~and~a~+1~Stone~(B)!", MsgMessage.ChatMode.System);
                            client.Inventory.Add(stream, 160137, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.Inventory.Add(stream, 730001, 1, 1, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            client.CreateBoxDialog("You~received~a~+1~Stone~(B)!~Find~the~Socketsmith~in~the~market~(178,204)~to~compose~your~equipment!");
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.QuestGUI.SendAutoPatcher(1000, 486, 346, 8439);
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
                            if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 1, 7))
                            {
                                data.AddText("Thanks for what you`ve done for us! Please accept these rewards!")
                             .AddOption("My~pleasure.", 12)
                              .AddAvatar(47).FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("While going through the desert, we were hit by a sand storm and my follower was kidnapped by the Level 68 Sand Monsters.")
                            .AddText("~Please save the poor guy!")
                                .AddOption("I`ll~try~my~best.", 255)
                                 .AddAvatar(47).FinalizeDialog();
                            }
                            break;
                        }
                        data.AddText("While going through the desert, we were hit by a sand storm and my follower was kidnapped by the Level 68 Sand Monsters.")
                            .AddText("~Please save the poor guy!")
                                .AddOption("I`ll~try~my~best.", 11)
                                .AddOption("Leave.", 255)
                                 .AddAvatar(47).FinalizeDialog();

                        break;
                    }
                case 12:
                    {
                        if (!client.Inventory.HaveSpace(2))
                        {
                            data.AddText("Please make 2 more spaces in your inventory.")
                        .AddOption("Let me check.", 255)
                         .AddAvatar(47).FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                        {
                            client.SendSysMesage("You've~received~some~EXP,~3,000~Silver~and~2~Meteors!", MsgMessage.ChatMode.System);
                            client.Inventory.Add(stream, Database.ItemType.Meteor, 2);
                            client.CreateBoxDialog("You~received~2~Meteors!~Find~the~Magic~Artisan~in~the~market~(259,248)~to~level~up~your~equipment!");
                            client.IncreaseExperience(stream, 2919000, questReward: true);
                            client.Player.Money += 3000;
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
                        if (client.Inventory.Contain(1060021, 1))
                        {
                            data.AddText("Tell me how to thank you enough! You saved my life!")
                              .AddOption("My~pleasure.", 3)
                               .AddAvatar(47).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Thank God you came! I ran into a sand storm on my way here, and was badly injured. Can you bring me a Ginseng, please?")
                                 .AddOption("I`ll~be~back~soon.", 2)
                                 .AddOption("Leave.", 255)
                                  .AddAvatar(47).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        client.CreateBoxDialog("Hurry~to~fetch~a~Ginseng!");
                        client.Player.QuestGUI.Accept(ActiveQuest, 0);

                        break;
                    }
                case 3:
                    {
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Remove(1060021, 1, stream);
                            client.Inventory.Add(stream, 1060021, 1);
                            client.CreateBoxDialog("You`ve~received~some~EXP,~2,000~Silver~and~a~Desert~City~Gate!");
                            client.IncreaseExperience(stream, 1719000, questReward: true);
                            client.Player.Money += 2000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SkillEraser)]
        public static void SkillEraser(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);


            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You have to give up something in roder to get something. I can help you to forget the skills you don't need anymore.")
                       .AddOption("Forget skills in this life.", 1)
                       .AddOption("I'll think about it.", 255)
                       .AddAvatar(100)
                       .FinalizeDialog();
                        break;
                    }
                #region I want to ask about something else
                case 99:
                    {
                        data.AddText("You have to give up something in roder to get something. I can help you to forget the skills you don't need anymore.")
                       .AddOption("Forget skills in this life.", 1)
                       .AddOption("I'll think about it.", 255)
                        .AddAvatar(100)
                      .FinalizeDialog();
                        break;
                    }
                #endregion

                case 1:
                    {


                        data.AddText("If you don't need some skills anymore, i can help you forget them. So what kind of skill would you like to forget?")
                       .AddOption("Summon skills.", 2)
                       .AddOption("General weapon skills.", 3)  //done
                       .AddOption("Transform skills.", 4)  //done
                       .AddOption("Class-based skills.", 5) //done
                       .AddOption("I want to ask about something else.", 99)
                        .AddAvatar(100)
                       .FinalizeDialog();
                        break;
                    }

                #region Class-based skills.
                case 5: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Spirit Healing.", 6)
                       .AddOption("Serenity.", 7)
                       .AddOption("Flying Moon.", 8)
                       .AddOption("Next.", 9) //ok
                       .AddOption("Cruel Shade.", 10)
                       .AddOption("Compassion.", 11)
                       .AddOption("Thunder.", 12)
                       .AddOption("I want to ask something else..", 1)
                         .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }



                #region SpiritHealing
                case 6:
                    {

                        {
                            var SpellID = 1190;
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
                                data.AddText("Sorry, you haven't learned the skill SpiritHealing. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }

                #endregion

                #region Serenity
                case 7:
                    {

                        {
                            var SpellID = 10400;
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
                                data.AddText("Sorry, you haven't learned the skill Serenity. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region FlyingMoon
                case 8:
                    {

                        {
                            var SpellID = 1320;
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
                                data.AddText("Sorry, you haven't learned the skill FlyingMoon. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region CruelShade
                case 10:
                    {

                        {
                            var SpellID = 3050;
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
                                data.AddText("Sorry, you haven't learned the skill CruelShade. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Compassion
                case 11:
                    {

                        {
                            var SpellID = 10430;
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
                                data.AddText("Sorry, you haven't learned the skill Compassion. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Thunder
                case 12:
                    {

                        {
                            var SpellID = 1000;
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
                                data.AddText("Sorry, you haven't learned the skill Thunder. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region Invisibility
                case 13:
                    {

                        {
                            var SpellID = 1075;
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
                                data.AddText("Sorry, you haven't learned the skill Invisibility. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Fire
                case 14:
                    {

                        {
                            var SpellID = 1001;
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
                                data.AddText("Sorry, you haven't learned the skill Fire. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Kraken'sRevenge
                case 15:
                    {

                        {
                            var SpellID = 11100;
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
                                data.AddText("Sorry, you haven't learned the skill Kraken'sRevenge. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Pervade
                case 16:
                    {

                        {
                            var SpellID = 3306;
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
                                data.AddText("Sorry, you haven't learned the skill Pervade. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region FireCircle
                case 17:
                    {

                        {
                            var SpellID = 1120;
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
                                data.AddText("Sorry, you haven't learned the skill FireCircle. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #endregion

                #region Transform skills.
                case 4: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Night Devil.", 18)
                       .AddOption("Water Elf.", 19)
                       .AddOption("Golem.", 20)
                       .AddOption("Divine Hare.", 22)
                       .AddOption("I want to ask something else..", 1)
                      .AddAvatar(100)
                    .FinalizeDialog();
                        break;

                    }


                #region DivineHare
                case 22:
                    {

                        {
                            var SpellID = 10522;
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
                                data.AddText("Sorry, you haven't learned the skill DivineHare. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Golem
                case 20:
                    {

                        {
                            var SpellID = 1270;
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
                                data.AddText("Sorry, you haven't learned the skill Golem. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region WaterElf
                case 19:
                    {

                        {
                            var SpellID = 1280;
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
                                data.AddText("Sorry, you haven't learned the skill WaterElf. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region NightDevil
                case 18:
                    {

                        {
                            var SpellID = 1360;
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
                                data.AddText("Sorry, you haven't learned the skill NightDevil. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #endregion

                #region General weapon skills.
                case 3: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget");
                        data.AddOption("FastBlade.", 23);
                        data.AddOption("Phoenix.", 24);
                        data.AddOption("Wide Strike.", 25);
                        data.AddOption("Next.", 26); //done
                        data.AddOption("Scent Sword.", 27);
                        data.AddOption("Speed Gun.", 28);
                        data.AddOption("Penetration.", 29);
                        data.AddOption("I want to ask something else..", 1)
                        .AddAvatar(100)
                        .FinalizeDialog();
                        break;

                    }


                case 26: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Boreas.", 30)
                       .AddOption("Snow.", 31) //aici am ramas
                       .AddOption("Boom.", 32)
                       .AddOption("Next.", 33) //done
                       .AddOption("Stranded Monster.", 34)
                       .AddOption("Halt.", 35)
                       .AddOption("Previous.", 3)
                       .AddOption("I want to ask something else..", 1)
                        .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }
                case 33: //       
                    {
                        data.AddText("After you forget the skills you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Seizer.", 36)
                       .AddOption("Rage.", 37)
                       .AddOption("Roamer.", 38)
                       .AddOption("Next.", 39) //done
                       .AddOption("EarthQuake.", 40)
                       .AddOption("Celestial.", 41)
                       .AddOption("Previous.", 26) //done
                       .AddOption("I want to ask something else..", 1)
                       .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }


                case 39: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Dragon Tail.", 42)
                       .AddOption("Viper Fang.", 69)
                       .AddOption("Previous.", 33) //done
                       .AddOption("I want to ask something else..", 1)
                          .AddAvatar(100)
                     .FinalizeDialog();
                        break;

                    }
                #region DragonTail
                case 42:
                    {

                        {
                            var SpellID = 11000;
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
                                data.AddText("Sorry, you haven't learned the skill DragonTail. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region ViperFang
                case 69:
                    {

                        {
                            var SpellID = 11005;
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
                                data.AddText("Sorry, you haven't learned the skill ViperFang. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Seizer
                case 36:
                    {

                        {
                            var SpellID = 7109;
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
                                data.AddText("Sorry, you haven't learned the skill Seizer. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Rage
                case 37:
                    {

                        {
                            var SpellID = 7020;
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
                                data.AddText("Sorry, you haven't learned the skill Rage. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Roamer
                case 38:
                    {

                        {
                            var SpellID = 7040;
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
                                data.AddText("Sorry, you haven't learned the skill Roamer. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion



                #region EartQuake
                case 40:
                    {

                        {
                            var SpellID = 7010;
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
                                data.AddText("Sorry, you haven't learned the skill EartQuake. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Celestial
                case 41:
                    {

                        {
                            var SpellID = 7030;
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
                                data.AddText("Sorry, you haven't learned the skill Celestial. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Boreas
                case 30:
                    {

                        {
                            var SpellID = 5050;
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
                                data.AddText("Sorry, you haven't learned the skill Boreas. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Snow
                case 31:
                    {

                        {
                            var SpellID = 5010;
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
                                data.AddText("Sorry, you haven't learned the skill Snow. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Boom
                case 32:
                    {

                        {
                            var SpellID = 5040;
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
                                data.AddText("Sorry, you haven't learned the skill Boom. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region StrandedMonster
                case 34:
                    {

                        {
                            var SpellID = 5020;
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
                                data.AddText("Sorry, you haven't learned the skill StrandedMonster. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Halt
                case 35:
                    {

                        {
                            var SpellID = 1300;
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
                                data.AddText("Sorry, you haven't learned the skill Halt. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region FastBlade
                case 23:
                    {

                        {
                            var SpellID = 1045;
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
                                data.AddText("Sorry, you haven't learned the skill FastBlade. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #region Phoenix
                case 24:
                    {

                        {
                            var SpellID = 1420;
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
                                data.AddText("Sorry, you haven't learned the skill Phoenix. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region WideStrike
                case 25:
                    {

                        {
                            var SpellID = 1250;
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
                                data.AddText("Sorry, you haven't learned the skill WideStrike. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region ScentSword
                case 27:
                    {

                        {
                            var SpellID = 1046;
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
                                data.AddText("Sorry, you haven't learned the skill ScentSword. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region SpeedGun
                case 28:
                    {

                        {
                            var SpellID = 1260;
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
                                data.AddText("Sorry, you haven't learned the skill SpeedGun. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Penetration
                case 29:
                    {

                        {
                            var SpellID = 1290;
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
                                data.AddText("Sorry, you haven't learned the skill Penetration. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion
                #endregion

                #region Summon Skills

                #region Summon skills.
                case 2: //       
                    {
                        data.AddText("After you forget the skilsl you don't need, you can conecentrate on the study of new skills.Which skill would you like to forget")
                       .AddOption("Bloody Bat.", 49)
                       .AddOption("Fire Evil.", 44)
                       .AddOption("Summon Bat Boss.", 45)
                       .AddOption("Summon Guard.", 46)
                       .AddOption("Skeleton.", 47)
                       .AddOption("Summon Bat.", 43)
                       .AddOption("I want to ask something else..", 1)
                         .AddAvatar(100)
                        .FinalizeDialog();
                        break;

                    }
                #endregion
                #region Summon Bat.
                case 43:
                    {

                        {
                            var SpellID = 4010;
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
                                data.AddText("Sorry, you haven't learned the skill Summon Bat. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Skeleton.
                case 47:
                    {

                        {
                            var SpellID = 4070;
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
                                data.AddText("Sorry, you haven't learned the skill Skeleton. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Guard.
                case 46:
                    {

                        {
                            var SpellID = 4000;
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
                                data.AddText("Sorry, you haven't learned the skill Guard. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region Bat.
                case 45:
                    {

                        {
                            var SpellID = 4000;
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
                                data.AddText("Sorry, you haven't learned the skill Guard. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion

                #region FireEvil.
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
                #endregion

                #region BloodyBat.
                case 49:
                    {

                        {
                            var SpellID = 8262;
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
                                data.AddText("Sorry, you haven't learned the skill BloodyBat. How can you forget it?")
                                 .AddOption("Okay.", 1)
                                 .AddAvatar(6)
                              .FinalizeDialog();
                            }
                        }


                        break;
                    }
                #endregion


                #endregion








                case 100:
                    {



                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.FrozenGrottoGeneral)]
        public static void FrozenGrottoGeneral(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Level < 28)
                        {
                            data.AddText("You can get the monster hunter quest from the TC Captain now! It`s a great place for you to level up. Here is a steed to");
                            data.AddText("~take you there!");
                            data.AddOption("Okay,~thanks.", 1);
                            data.AddOption("I~have~no~interest.");
                            data.AddAvatar(57);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.Level < 48)
                            {
                                data.AddText("You are able to take the quest from the PC Captain now! You can level up more quickly there. Here is a steed to take you there!");
                                data.AddOption("Okay,~thanks.", 2);
                                data.AddOption("I~have~no~interest.");
                                data.AddAvatar(57);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.Level < 68)
                                {
                                    data.AddText("You`d better take the quests from the AC Captain. It`s the right place for you to level up. Here is a steed to take you");
                                    data.AddText("~there!");
                                    data.AddOption("Okay,~thanks.", 3);
                                    data.AddOption("I~have~no~interest.");
                                    data.AddAvatar(57);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    if (client.Player.Level < 88)
                                    {
                                        data.AddText("You are powerful enough to take the quests from the DC Captain now! Here is a steed to take you there!");
                                        data.AddOption("Okay,~thanks.", 4);
                                        data.AddOption("I~have~no~interest.");
                                        data.AddAvatar(57);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        if (client.Player.Level < 103)
                                        {
                                            data.AddText("You are powerful now, and you are able to take the quests from the BI Captain! He is in need of your help! Here is a steed to");
                                            data.AddText("~take you there!");
                                            data.AddOption("Okay,~thanks.", 5);
                                            data.AddOption("I~have~no~interest.");
                                            data.AddAvatar(57);
                                            data.FinalizeDialog();
                                        }
                                        else
                                        {
                                            if (client.Player.Level < 120)
                                            {
                                                data.AddText("You are strong enough to kill the monsters in Mystic Castle. Why not take a quest from MC Captain to level up quickly? Here");
                                                data.AddText("~is a steed to take you there.");
                                                data.AddOption("Okay,~thanks.", 6);
                                                data.AddOption("I~have~no~interest.");
                                                data.AddAvatar(57);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddText("Cloud Saint in Market (291,224) is looking for the heroes to help him exterminate the monsters. If you can finish the quest,");
                                                data.AddText("~you will receive 5 EXP Balls as rewards!");
                                                data.AddOption("I~see.");
                                                data.AddAvatar(57);
                                                data.FinalizeDialog();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 6:
                    {
                        client.Teleport(085, 321, 1000);
                        break;
                    }
                case 5:
                    {
                        client.Teleport(792, 571, 1015);
                        break;
                    }
                case 4:
                    {
                        client.Teleport(483, 634, 1000);
                        break;
                    }
                case 3:
                    {
                        client.Teleport(571, 621, 1020);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(233, 260, 1011);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(410, 354, 1002);
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.KitMerchant)]
        public static void KitMerchant(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        {
                            data.AddText("Well,hello there! I'm here selling special kits for all the different classes.See anything you like? We've got Elite kits, cheap and sturdy, or Super kits for the more afluent adventurer.Which kind of kit were you interested in? The Super or Elite kits?")
                        .AddOption("Super~kits.", 1)
                        .AddOption("Elite~kits.", 2)
                        .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                case 105:
                    {

                        {
                            data.AddText("Well,hello there! I'm here selling special kits for all the different classes.See anything you like? We've got Elite kits, cheap and sturdy, or Super kits for the more afluent adventurer.Which kind of kit were you interested in? The Super or Elite kits?")
                        .AddOption("Super~kits.", 1)
                        .AddOption("Elite~kits.", 2)
                        .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }


                        break;
                    }
                #region Elite Kits
                case 2:
                    {
                        data.AddText("There are some options.");
                        if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            data.AddOption("Elite~Monk~Outfit.", 60); //done
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            data.AddOption("Elite~Ninja~Outfit.", 61); //done
                        else if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            data.AddOption("Elite~Trojan~Outfit.", 62); //done
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            data.AddOption("Elite~Warrior~Outfit.", 63); //done
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            data.AddOption("Elite~Archer~Outfit.", 64); //done
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            data.AddOption("Elite~Pirate~Outfit.", 65); //done
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            data.AddOption("Elite~Long~Lee~Outfit.", 66); //DONE
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddOption("Elite~Water-Fire~Outfit.", 67);
                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddOption("Common~Elite~outfit~B.", 49);
                        else
                            data.AddOption("Common~Elite~outfit~A.", 68);

                        data.AddOption("Anything~else?", 105)
                        .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Water
                case 67:
                    {
                        data.AddText("This pack contains an~Elite~L70~Great~Backsword,~an~Elite~L70~Crane~Vestment,~an~Elite~L67~Shark~Cap~and~3~+1~Stones.")
                     .AddOption("Yes.", 40)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 40:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720710, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteTaoistKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Long-lee
                case 66:
                    {
                        data.AddText("This pack contains 2 (Elite)HeartNunchaku , 1 (Elite) Level 70 Combat Suit,1 (Elite) level 70 WindHood and~1~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 41)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 41:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 617138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 617138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 138068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //CombatSuit
                                client.Inventory.Add(stream, 148068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //WindHood
                                client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                                client.SendSysMesage("You received  2 (Elite)HeartNunchaku , 1 (Elite) Level 70 Combat Suit,1 (Elite) level 70 WindHood and~1~+1~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Pirate
                case 65:
                    {
                        data.AddText("This pack contains 1 (Elite)MisteryRapier ,1 (Elite)GlowerPistol, 1 (Elite) Level 70 Tide Coat,1 (Elite) level 70 NightMareHood and~1~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 42)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 42:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 611138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //MisteryRapier
                                client.Inventory.Add(stream, 612138, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //GlowPistol
                                client.Inventory.Add(stream, 139068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //TideCoat
                                client.Inventory.Add(stream, 145068, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //NightMareHood
                                client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                                client.SendSysMesage("You received 1 (Elite)MisteryRapier ,1 (Elite)GlowerPistol, 1 (Elite) Level 70 Tide Coat,1 (Elite) level 70 NightMareHood and~1~+1~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Archer
                case 64:
                    {
                        data.AddText("This pack contains an~Elite~L70~Goose~Bow,~an~Elite~L67~Ape~Coat,~an~Elite~L72~Ape~Hat~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 43)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 43:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720708, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteArcherKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Warrior
                case 63:
                    {
                        data.AddText("This pack contains ~L70~Iron~Wand,~an~Elite~L70~Light~Armor,~an~Elite~L67~Heart~of~Ocean~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 44)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 44:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720706, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteWarriorKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Trojan
                case 62:
                    {
                        data.AddText("This pack contains an~Elite~L70~Shark~Sword,~an~Elite~L70~Rage~Armor,~an~Elite~L67~War~Coronet~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 45)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 45:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720707, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteTrojanKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Ninja
                case 61:
                    {
                        data.AddText("This pack contains an~Elite~L70~Rain~Katana,~an~Elite~L70~TigerVest,~an~Elite~L67~BloodVeil~and~3~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 46)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 46:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720709, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteNinjaKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Monk
                case 60:
                    {
                        data.AddText("This pack contains  2 Elite level 90 Beads of Eternity, Elite Level 87 Tough Frock,Elite Level 82 Martyr's Headband and~1~+1~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 47)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 47:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 610178, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 610178, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 136078, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ToughFrock
                                client.Inventory.Add(stream, 143078, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //Martyr`sHeadband
                                client.Inventory.AddItemWitchStack(723712, 0, 1, stream);
                                client.SendSysMesage("You received 2 (Elite)Beads of Eternity, 1 (Elite) Level 87 Tough Frock,1 (Elite) level 82 Martyr`sHeadband and~1~+1~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                #region UniversalKit
                case 68: //sexi
                    {

                        data.AddText("There are some options.");
                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddText("The Elite Universal! Kit A contains an~Elite~L65~Bone~Bracelet,~an~Elite~L67~Ambergris~Bag,~an~Elite~L70~Snakeskin~Boots~and~3~+1~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");

                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddOption("Yeah!", 49);
                        else
                            data.AddText("The Elite Universal! Kit A contains an~Elite~L67~Platina~Necklace,~an~Elite~L70~Pearl~Ring,~an~Elite~L70~Snakeskin~Boots~and~3~+1~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");
                        data.AddOption("Yeah!", 50);
                        data.AddOption("Let~me~think.", 1)
                                   .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 49: //wateri
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 749;

                                client.Inventory.AddItemWitchStack(720712, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteUniversalKitB.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }

                case 50:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 749;

                                client.Inventory.AddItemWitchStack(720711, 0, 1, stream);
                                client.SendSysMesage("You received 1 EliteUniversalKitA.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                ////
                #region Super Kits
                case 1:
                    {
                        data.AddText("There are some options.");
                        if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            data.AddOption("Super~Monk~Outfit.", 10); //done
                        else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            data.AddOption("Super~Ninja~Outfit.", 11); //done
                        else if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            data.AddOption("Super~Trojan~Outfit.", 12); //done
                        else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            data.AddOption("Super~Warrior~Outfit.", 13); //done
                        else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            data.AddOption("Super~Archer~Outfit.", 14); //done
                        else if (Database.AtributesStatus.IsPirate(client.Player.Class))
                            data.AddOption("Super~Pirate~Outfit.", 15); //done
                        else if (Database.AtributesStatus.IsLee(client.Player.Class))
                            data.AddOption("Super~Long~Lee~Outfit.", 16); //DONE
                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddOption("Super~Water-Fire~Outfit.", 17);
                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddOption("Common~Super~outfit~B.", 86);
                        else
                            data.AddOption("Common~Super~outfit~A.", 30);
                        data.AddOption("Anything~else?", 105)
                             .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                #endregion
                #region Water
                case 17:
                    {
                        data.AddText("This pack contains a~Super~L90~War~Backsword,~a~Super~L87~Full~Frock,~a~Super~L82~Dragon~Cap~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 21)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 21:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720703, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperTaoistKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Long-lee
                case 16:
                    {
                        data.AddText("This pack contains 2 (Super)HeartNunchaku , 1 (Super) Level 70 Combat Suit,1 (Super) level 70 WindHood and~3~+2~Stones ! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 9)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 9:
                    {
                        if (client.Inventory.HaveSpace(4))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 617139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 617139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //HeatNunchaku
                                client.Inventory.Add(stream, 138069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //CombatSuit
                                client.Inventory.Add(stream, 148069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //WindHood
                                client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                                client.SendSysMesage("You received  2 (Super)HeartNunchaku , 1 (Super) Level 70 Combat Suit,1 (Super) level 70 WindHood and~3~+2~Stones .", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 4 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Pirate
                case 15:
                    {
                        data.AddText("This pack contains 1 (Super)MisteryRapier ,1 (Super)GlowerPistol, 1 (Super) Level 70 Tide Coat,1 (Super) level 70 NightMareHood and~3~+2~Stones ! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 8)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 8:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 611139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //MisteryRapier
                                client.Inventory.Add(stream, 612139, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //GlowPistol
                                client.Inventory.Add(stream, 139069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //TideCoat
                                client.Inventory.Add(stream, 145069, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //NightMareHood
                                client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                                client.SendSysMesage("You received 1 (Super)MisteryRapier ,1 (Super)GlowerPistol, 1 (Super) Level 70 Tide Coat,1 (Super) level 70 NightMareHood and~3~+2~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Archer
                case 14:
                    {
                        data.AddText("This pack contains a~Super~L90~Star~Bow,~a~Super~L87~Shark~Coat,~a~Super~L82~Marten~Hat~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 7)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 7:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;
                                client.Inventory.AddItemWitchStack(720701, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperArcherKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Warrior
                case 13:
                    {
                        data.AddText("This pack contains a~Super~L90~Exorcising~Wand,~a~Super~L87~Lion~Armor,~a~Super~L82~Dragon~Helmet~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 6)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 6:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.AddItemWitchStack(720699, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperWarriorKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Trojan
                case 12:
                    {
                        data.AddText("This pack contains a~Super~L90~Exorcising~Wand,~a~Super~L87~Lion~Armor,~a~Super~L82~Dragon~Helmet~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 5)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 5:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;
                                client.Inventory.AddItemWitchStack(720699, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperWarriorKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Ninja
                case 11:
                    {
                        data.AddText("This pack contains a~Super~L90~Ise~Katana,~a~Super~L87~Bear~Vest,~a~Super~L82~Stealth~Veil~and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 4)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 4:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;


                                client.Inventory.AddItemWitchStack(720702, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperNinjaKit.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }


                #endregion
                #region Monk
                case 10:
                    {
                        data.AddText("This pack contains  2 Super level 90 Beads of Eternity, Super Level 87 Tough Frock,Super Level 82 Martyr's Headband and~3~+2~Stones! Are you sure you want to pay 699 CPs to pruchase it?.")
                     .AddOption("Yes.", 3)
                     .AddOption("Let~me~think.", 1)
                     .AddAvatar(63).FinalizeDialog();
                        break;
                    }

                case 3:
                    {
                        if (client.Inventory.HaveSpace(5))
                        {
                            if (client.Player.ConquerPoints >= 699)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 699;

                                client.Inventory.Add(stream, 610179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 610179, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //BeadsofEternity
                                client.Inventory.Add(stream, 136079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //ToughFrock
                                client.Inventory.Add(stream, 143079, 0, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false); //Martyr`sHeadband
                                client.Inventory.AddItemWitchStack(3005654, 0, 2, stream);
                                client.SendSysMesage("You received 2 (Super)Beads of Eternity, 1 (Super) Level 87 Tough Frock,1 (Super) level 82 Martyr`sHeadband and~3~+2~Stones.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 699 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 5 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion

                #region Univeral Kit
                case 30:
                    {

                        data.AddText("There are some options.");
                        if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddText("The Super Universal! Kit A contains a~Super~L85~Jade~Bracelet,~a~Super~L82~Jade~Bag,~a~Super~L90~Leopard~Boots~and~3~+2~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");

                        else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            data.AddOption("Yeah!", 86);
                        else
                            data.AddText("The Super Universal! Kit A contains a~Super~L90~Crystal~Ring,~a~Super~L82~Basalt~Necklace,~a~Super~L90~Leopard~Boots~and~3~+2~Stones.It costs 749 CPs, and is an incredible bargain! Would you like to buy one?");
                        data.AddOption("Yeah!", 99);
                        data.AddOption("Let~me~think.", 1)
                                   .AddAvatar(6).FinalizeDialog();
                        break;
                    }
                case 86:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 749;

                                client.Inventory.AddItemWitchStack(720705, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperUniversalKitB.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);



                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }

                case 99: //normal
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Player.ConquerPoints >= 749)
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 749;
                                client.Inventory.AddItemWitchStack(720704, 0, 1, stream);
                                client.SendSysMesage("You received 1 SuperUniversalKitA.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);


                            }
                            else
                            {
                                data.AddText("This pack costs 749 CPs.Since you don't have enough CPs, i can't give it to you!")
                  .AddOption("Alright.", 255)
                 .AddAvatar(63).FinalizeDialog();
                            }
                        }
                        else
                        {

                            data.AddText("Please make 1 space in your inventory.")
                         .AddOption("Let~me~think.", 255)
                        .AddAvatar(63).FinalizeDialog();
                        }
                        break;
                    }
                #endregion
            }
        }




        //[NpcAttribute(NpcID.PkReset)] // [NpcOfficial 2026-10-14] custom (non-official) NPC removed from the data; handler disabled
        public static void PkReset(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Hello, You Can Reduce Your PK Points Through Me. Do You Want To Reduce It Now?")
                           .AddOption("Reduce 10 PK Points \nFor (500 CPs).", 1)
                           .AddOption("Reduce 50 PK Points \nFor (2.500 CPs).", 2)
                           .AddOption("Reduce 100 PK Points \nFor (5.000 CPs).", 3)
                           .AddOption("Reduce 250 PK Points \nFor (12.500 CPs).", 4)
                           .AddOption("Reduce 500 PK Points \nFor (25.000 CPs).", 5)
                           .AddOption("Reduce 1.000 PK Points \nFor (50.000 CPs).", 6)
                            .AddOption("I will come later.", 255).AddAvatar(174).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.ConquerPoints >= 500)
                        {
                            if (client.Player.PKPoints >= 10)
                            {
                                if (client.InTrade) return;
                                client.Player.PKPoints -= 10;
                                client.Player.ConquerPoints -= 500;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("10 PK Points Have Been Successfully Removed From Your Account.");
                            }
                            else
                            {
                                data.AddText("You Can Not Use This Option For Now Since Your PK Points Are Less Than 10 !")
                               .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                            .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 2500)
                        {
                            if (client.Player.PKPoints >= 50)
                            {
                                if (client.InTrade) return;
                                client.Player.PKPoints -= 50;
                                client.Player.ConquerPoints -= 2500;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("50 PK Points Have Been Successfully Removed From Your Account.");
                            }
                            else
                            {
                                data.AddText("You Can Not Use This Option For Now Since Your PK Points Are Less Than 50 !")
                               .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                            .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 5000)
                        {
                            if (client.Player.PKPoints >= 100)
                            {
                                if (client.InTrade) return;
                                client.Player.PKPoints -= 100;
                                client.Player.ConquerPoints -= 5000;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("100 PK Points Have Been Successfully Removed From Your Account.");
                            }
                            else
                            {
                                data.AddText("You Can Not Use This Option For Now Since Your PK Points Are Less Than 100 !")
                               .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                            .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.ConquerPoints >= 12500)
                        {
                            if (client.Player.PKPoints >= 250)
                            {
                                if (client.InTrade) return;
                                client.Player.PKPoints -= 250;
                                client.Player.ConquerPoints -= 12500;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("250 PK Points Have Been Successfully Removed From Your Account.");
                            }
                            else
                            {
                                data.AddText("You Can Not Use This Option For Now Since Your PK Points Are Less Than 250 !")
                               .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                            .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.ConquerPoints >= 25000)
                        {
                            if (client.Player.PKPoints >= 500)
                            {
                                if (client.InTrade) return;
                                client.Player.PKPoints -= 500;
                                client.Player.ConquerPoints -= 25000;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("500 PK Points Have Been Successfully Removed From Your Account.");
                            }
                            else
                            {
                                data.AddText("You Can Not Use This Option For Now Since Your PK Points Are Less Than 500 !")
                               .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                            .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.ConquerPoints >= 50000)
                        {
                            if (client.Player.PKPoints >= 1000)
                            {
                                if (client.InTrade) return;
                                client.Player.PKPoints -= 1000;
                                client.Player.ConquerPoints -= 50000;
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                                client.CreateBoxDialog("1.000 PK Points Have Been Successfully Removed From Your Account.");
                            }
                            else
                            {
                                data.AddText("You Can Not Use This Option For Now Since Your PK Points Are Less Than 1.000 !")
                               .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Sorry, but you don`t have enough CPs.")
                            .AddOption("Okay.", 255).AddAvatar(174).FinalizeDialog();
                        }
                        break;
                    }
            }
        }




//        [NpcAttribute(NpcID.VipQuest)]
//        public static void VipQuest(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
//        {
//            Dialog data = new Dialog(client, stream);
//            switch (Option)
//            {
//                case 0:
//                    {
//                        data.AddText("I'm pretty sure you are looking for a way to get VIP 6, good thing there is a way but it wont be that easy.")
//                   .AddOption("No problem.", 1)
//                    .AddOption("I`ll~think~it~over.", 255)
//                    .AddAvatar(105)
//                    .FinalizeDialog();

//                        break;
//                    }
//                case 1:
//                    {
//                        client.Teleport(608, 632, 1700);
//                        break;
//                    }
//            }
//        }
//#if Encore
//                [NpcAttribute(NpcID.Arthur)]
//        public static void Arthur(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
//        {
//            Dialog data = new Dialog(client, stream);
//            switch (Option)
//            {
//                case 0:
//                    {
//#if Arabic
//                       {
//                          data.AddText("This place can be quite dangerous. If you want to leave, just come talk to me.")
//                                .AddText("Well.. i can help you to get vip(Level 6) token. \n")
//                                .AddText("There are 4 stages for you to complete the Vip6 Quest: \n")
//                            .AddText("Stage 1. Change 3 Moss, 3 SoulAroma and 3 DreamGras for an GhostHorn. \n")
//                            .AddText("Stage 2. Change 3xGhostHorn for an EvilTooth. \n")
//                            .AddText("Stage 3. Change 5xEvilTooth for an ImpureVigor. \n")
//                            .AddText("Stage 4. Change 5xImpureVigor for an 1DayVIP token. \n")
//                                .AddOption("I have (Moss,SoulAroma,DreamGras)", 1)
//                                 .AddOption("I have 3xGhostHorn to trade.", 2)
//                                 .AddOption("I have 5xEvilTooth to trade.", 3)
//                                 .AddOption("I have 5xImpureVigor to trade", 4)
//                                 .AddOption("Teleport~to~Twin~City", 5)
//                       .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();
//                       }
//#else
//                        {
//                            data.AddText("This place can be quite dangerous. If you want to leave, just come talk to me.")
//                                .AddText("Well.. i can help you to get vip(Level 6) token. \n")
//                                .AddText("There are 4 stages for you to complete the Vip6 Quest: \n")
//                            .AddText("Stage 1. Change 3 Moss, 3 SoulAroma and 3 DreamGras for an GhostHorn. \n")
//                            .AddText("Stage 2. Change 3xGhostHorn for an EvilTooth. \n")
//                            .AddText("Stage 3. Change 4xEvilTooth for an ImpureVigor. \n")
//                            .AddText("Stage 4. Change 5xImpureVigor for an 1DayVIP token. \n")
//                                .AddOption("I have (Moss,SoulAroma,DreamGras)", 1)
//                                 .AddOption("I have 3xGhostHorn to trade.", 2)
//                                 .AddOption("I have 4xEvilTooth to trade.", 3)
//                                 .AddOption("I have 2xImpureVigor to trade", 4)
//                                 .AddOption("Teleport~to~Twin~City", 5)
//                       .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();
//                        }
//#endif
//                        break;

//                    }
//                case 4:
//                    {
//#if Arabic
//                       {
//                           if (!client.Inventory.HaveSpace(1))
//                       {
//                           data.AddText("Please make 1 more space in your inventory.")
//                  .AddOption("Let me check.", 255).FinalizeDialog();
//                           break;
//                       }
//                       if (client.Inventory.Contain(722730, 5, 0))
//                       {
//                           client.Inventory.Remove(722730, 5, stream);
//                           client.Inventory.Add(stream, 780001);
//                           Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Vip6 Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
//                           data.AddText("You successfully exchange the ImpureVigor for an 1DayVIP token!")
//                      .AddOption("Thank you.", 255).FinalizeDialog();
//                       }
//                       else
//                       {
//                           data.AddText("I'm sorry, you need at least 5 ImpureVigor in order for me to help you.")
//                     .AddOption("I see.", 255).FinalizeDialog();
//                       }
//                       }
//#else
//                        {
//                            if (!client.Inventory.HaveSpace(1))
//                            {
//                                data.AddText("Please make 1 more space in your inventory.")
//                       .AddOption("Let me check.", 255).FinalizeDialog();
//                                break;
//                            }
//                            if (client.Inventory.Contain(722730, 2, 0))
//                            {
//                                client.Inventory.Remove(722730, 2, stream);
//                                client.Inventory.Add(stream, 780001);
//                                Server.SendGlobalPacket(new MsgMessage("Congratulation! " + client.Player.Name + " has finished the Vip6 Quest.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
//                                data.AddText("You successfully exchange the ImpureVigor for an 1DayVIP token!")
//                           .AddOption("Thank you.", 255).FinalizeDialog();
//                            }
//                            else
//                            {
//                                data.AddText("I'm sorry, you need at least 2 ImpureVigor in order for me to help you.")
//                          .AddOption("I see.", 255).FinalizeDialog();
//                            }
//                        }
//#endif

//                        break;
//                    }
//                case 3:
//                    {
//#if Arabic 
//                       {
//                           if (!client.Inventory.HaveSpace(1))
//                       {
//                           data.AddText("Please make 1 more space in your inventory.")
//                  .AddOption("Let me check.", 255).FinalizeDialog();
//                           break;
//                       }
//                       if (client.Inventory.Contain(722721, 4, 0))
//                       {
//                           client.Inventory.Remove(722721, 4, stream);
//                           client.Inventory.Add(stream, 722730);
//                           data.AddText("You successfully exchange the EvilTooth for an ImpureVigor!")
//                      .AddOption("Thank you.", 255).FinalizeDialog();
//                       }
//                       else
//                       {
//                           data.AddText("I'm sorry, you need at least 3 EvilTooth in order for me to help you.")
//                     .AddOption("I see.", 255).FinalizeDialog();
//                       }
//                       }
//#else
//                        {
//                            if (!client.Inventory.HaveSpace(1))
//                            {
//                                data.AddText("Please make 1 more space in your inventory.")
//                       .AddOption("Let me check.", 255).FinalizeDialog();
//                                break;
//                            }
//                            if (client.Inventory.Contain(722721,4, 0))
//                            {
//                                client.Inventory.Remove(722721,4, stream);
//                                client.Inventory.Add(stream, 722730);
//                                data.AddText("You successfully exchange the EvilTooth for an ImpureVigor!")
//                           .AddOption("Thank you.", 255).FinalizeDialog();
//                            }
//                            else
//                            {
//                                data.AddText("I'm sorry, you need at least 5 EvilTooth in order for me to help you.")
//                          .AddOption("I see.", 255).FinalizeDialog();
//                            }
//                        }
//#endif
//                        break;
//                    }
//                case 2:
//                    {
//#if Arabic
//                       {
//                           if (!client.Inventory.HaveSpace(1))
//                       {
//                           data.AddText("Please make 1 more space in your inventory.")
//                  .AddOption("Let me check.", 255).FinalizeDialog();
//                           break;
//                       }
//                       if (client.Inventory.Contain(722722, 3, 0))
//                       {
//                           client.Inventory.Remove(722722, 3, stream);
//                           client.Inventory.Add(stream, 722721);
//                           data.AddText("You successfully exchange the GhostHorn for an EvilTooth!")
//                      .AddOption("Thank you.", 255).FinalizeDialog();
//                       }
//                       else
//                       {
//                           data.AddText("I'm sorry, you need at least 3 GhostHorn in order for me to help you.")
//                     .AddOption("I see.", 255).FinalizeDialog();
//                       }
//                       }
//#else
//                        {
//                            if (!client.Inventory.HaveSpace(1))
//                            {
//                                data.AddText("Please make 1 more space in your inventory.")
//                       .AddOption("Let me check.", 255).FinalizeDialog();
//                                break;
//                            }
//                            if (client.Inventory.Contain(722722,3, 0))
//                            {
//                                client.Inventory.Remove(722722, 3, stream);
//                                client.Inventory.Add(stream, 722721);
//                                data.AddText("You successfully exchange the GhostHorn for an EvilTooth!")
//                           .AddOption("Thank you.", 255).FinalizeDialog();
//                            }
//                            else
//                            {
//                                data.AddText("I'm sorry, you need at least 5 GhostHorn in order for me to help you.")
//                          .AddOption("I see.", 255).FinalizeDialog();
//                            }
//                        }
//#endif
//                        break;
//                    }
//                case 1:
//                    {
//#if Arabic
//                       {
//                           if (!client.Inventory.HaveSpace(1))
//                       {
//                           data.AddText("Please make 1 more space in your inventory.")
//                  .AddOption("Let me check.", 255).FinalizeDialog();
//                           break;
//                       }
//                       if (client.Inventory.Contain(722723, 3, 0)
//                           && client.Inventory.Contain(722724, 3, 0)
//                           && client.Inventory.Contain(722725, 3, 0))
//                       {
//                           client.Inventory.Remove(722723, 3, stream);
//                           client.Inventory.Remove(722724, 3, stream);
//                           client.Inventory.Remove(722725, 3, stream);
//                           client.Inventory.Add(stream, 722722);
//                           data.AddText("You successfully exchange the (Moss,SoulAroma and DreamGras) for an GhostHorn!")
//                      .AddOption("Thank you.", 255).FinalizeDialog();
//                       }
//                       else
//                       {
//                           data.AddText("I'm sorry, you need at least 3xMoss, 3xSoulAroma and 3xDreamGras in order for me to help you.")
//                     .AddOption("I see.", 255).FinalizeDialog();
//                       }
//                       }
//#else
//                        {
//                            if (!client.Inventory.HaveSpace(1))
//                            {
//                                data.AddText("Please make 1 more space in your inventory.")
//                       .AddOption("Let me check.", 255).FinalizeDialog();
//                                break;
//                            }
//                            if (client.Inventory.Contain(722723, 3, 0)
//                                && client.Inventory.Contain(722724, 3, 0)
//                                && client.Inventory.Contain(722725, 3, 0))
//                            {
//                                client.Inventory.Remove(722723, 3, stream);
//                                client.Inventory.Remove(722724, 3, stream);
//                                client.Inventory.Remove(722725, 3, stream);
//                                client.Inventory.Add(stream, 722722);
//                                data.AddText("You successfully exchange the (Moss,SoulAroma and DreamGras) for an GhostHorn!")
//                           .AddOption("Thank you.", 255).FinalizeDialog();
//                            }
//                            else
//                            {
//                                data.AddText("I'm sorry, you need at least 3xMoss, 3xSoulAroma and 3xDreamGras in order for me to help you.")
//                          .AddOption("I see.", 255).FinalizeDialog();
//                            }
//                        }
//#endif
//                        break;
//                    }
//                case 5:
//                    {
        //                        client.Teleport(410, 354, 1002);
//                        break;
//                    }

//            }
//        }
//#else

//#endif

        [NpcAttribute(NpcID.Norbert)]
        public static void Norbert(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Inventory.Contain(721262, 5) && client.Inventory.Contain(721263, 1))
                        {
                            data.AddText("Hi, you have got the meterial i want. Do you come for the Bomb?")
                 .AddOption("Yes", 10)
                 .AddOption("Just passing by.", 255)
                 .AddAvatar(67).FinalizeDialog();

                            break;
                        }
                        data.AddText("Five~pieces~of~Saltpeter~and~one~piece~of~Sulphur~are~required~to~make~the~Bomb.~And~I~charge~a~fee~of~30,000~silvers.")
                   .AddOption("What~is~the~Bomb?", 1)
                   .AddOption("I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        data.AddText("It`s used to blow down the guild gate! I participated in a guild war and witnessed lots of people lost their lives when attacking the gate.")
                            .AddText("~Since then, I have been working on inventing powerful bombs which can destroy the guild gate. Finally, I make it!")
                   .AddOption("Can~you~make~one~for~me?", 2)
                   .AddOption("Oh.~I~see.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 2:
                    {
                        data.AddText("No problem. But you know such bombs are powerful and dangerous. Whoever uses it to destroy a guild gate will be killed by the bomb, too!")
                  .AddOption("Thanks.~I~need~it~badly.", 3)
                  .AddOption("Then~forget~it.", 255)
                  .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("Good. You`re going to collect 5 Saltpeter and a Sulphur. Besides, you`ll need to pay 30,000 Silver as fees.")
                   .AddOption("Where~do~I~get~the~materials?", 4)
                   .AddOption("I`ll~give~up.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 4:
                    {
                        data.AddText("You~can~get~Saltpeter~from~my~friend~OldQuarrier~in~Ape~City~and~get~Sulphur~from~the~Bandits~nearby.")
                   .AddOption("Okay,~I`ll~be~back~soon.", 255)
                   .AddAvatar(67).FinalizeDialog();

                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(721262, 5) && client.Inventory.Contain(721263, 1))
                        {
                            client.Inventory.Remove(721262, 5, stream);
                            client.Inventory.Remove(721263, 1, stream);
                            client.Inventory.Add(stream, 721261);
                            data.AddText("You can take the Bomb away. Remember that you`ll be killed once you use it to destroy the guild gate!")
                   .AddOption("I see. Thanks for the remind.", 255)
                   .AddAvatar(67).FinalizeDialog();
                        }
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.OldQuarrier)]
        public static void OldQuarrier(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Let me guess. It`s Norbert who asked you to come here. Am I right?")
                  .AddOption("Yeah,~I~need~a~Saltpeter.", 1)
                  .AddOption("Nope.", 2)
                  .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Inventory.Add(stream, 721262);
                            data.AddText("My~pleasure.~Here~you~are.")
             .AddOption("Thank~you.", 255)
             .AddAvatar(91).FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("Please make 1 more space in your inventory.")
              .AddOption("Let me check.", 255)
              .AddAvatar(91).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("Are~you~coming~for~Saltpeter?~Since~Norbert~invented~Bomb,~I~have~been~kept~busy~with~supplying~Saltpeter~for~him.")
                            .AddOption("What~is~the~Bomb?", 3)
                            .AddOption("Sorry~to~bother~you.", 255)
                            .AddAvatar(91).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("It~is~powerful~and~used~to~bomb~out~City~Gate.~If~you`re~interested~in~it,~you~can~visit~Norbert~ for the details. He lives in~the~small~village~of~the~Maple Forest")
                             .AddOption("I~see.", 255)
                             .AddAvatar(91).FinalizeDialog();

                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DrYinYang)]
        public static void DrYinYang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("If you want to change your gender for a new life, I can help you. Before making the")
                            .AddText("~magic of gender reassignment works, you need to pay 1075 CPs, and make sure you`ve understood")
                            .AddText("~the rules and procedures about the reassignment.")
                   .AddOption("Please~change~my~gender,~now.", 1)
                   .AddOption("What~else~should~I~know?", 2)
                   .AddOption("Not~interested.", 255)
                   .AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 3:
                    {
                        data.AddText("If you want to change your gender for a new life, I can help you. Before making the")
                            .AddText("~magic of gender reassignment works, you need to pay 1075 CPs, and make sure you`ve understood")
                            .AddText("~the rules and procedures about the reassignment.")
                   .AddOption("Please~change~my~gender,~now.", 1)
                   .AddOption("What~else~should~I~know?", 2)
                   .AddOption("Not~interested.", 255)
                   .AddAvatar(159).FinalizeDialog();

                        break;
                    }
                case 1:
                    {
                        if (client.Player.Spouse != "None")
                        {
                            data.AddText("You`re married. To change your gender, you need to get a divorce, first.")
                .AddOption("Okay.", 255)
                .AddAvatar(159).FinalizeDialog();
                            break;
                        }
                        if (client.Player.ConquerPoints >= 1075)
                        {
                            if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.Garment) == false
                           || !client.Equipment.FreeEquip(Role.Flags.ConquerItem.AleternanteGarment))
                            {
                                data.AddText("Please remove your garment.")
                   .AddOption("Okay.", 255)
                   .AddAvatar(159).FinalizeDialog();
                                break;
                            }
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 1075;
                            if (Role.Core.IsBoy(client.Player.Body))
                            {
                                client.Player.Body = 2007;
                                client.Player.SendUpdate(stream, client.Player.Mesh, MsgUpdate.DataType.Mesh);
                                client.Player.View.ReSendView(stream);

                            }
                            else
                            {
                                client.Player.Body = 1008;
                                client.Player.SendUpdate(stream, client.Player.Mesh, MsgUpdate.DataType.Mesh);
                                client.Player.View.ReSendView(stream);
                            }
                            Database.DataCore.SetCharacterSides(client.Player);
                            if (client.Player.Body == 1008)
                                client.Player.Face = (ushort)Pool.GetRandom.Next(1, 50);
                            else
                                client.Player.Face = (ushort)Pool.GetRandom.Next(201, 250);

                            client.HairfaceStorage.Add(Database.HairfaceStorageTable.Hairfaces.Where(i => i.ID == client.Player.Face && i.Type == MsgHairfaceStorage.Type.Avatar).FirstOrDefault(), false);
                            if (client.Player.Nobility != null)
                            {
                                client.Player.Nobility.Gender = client.Player.GetGender;
                                client.Player.Nobility.Mesh = client.Player.Mesh;
                            }
                            if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
                                client.Player.MyGuildMember.Mesh = client.Player.Mesh;
                            if (client.ArenaStatistic != null)
                                client.ArenaStatistic.Mesh = client.Player.Mesh;

                            MsgMessage messaj = new MsgMessage(client.Player.Name + ", Changed He's/Hers mesh into male / female.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);
                            if (client.Player.Flowers.FreeFlowers > 0)
                            {
                                client.Send(stream.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                                    ? Game.MsgServer.MsgFlower.FlowerAction.FlowerSender
                                    : Game.MsgServer.MsgFlower.FlowerAction.Flower
                                    , 0, 0, client.Player.Flowers.FreeFlowers));
                            }
                            client.Socket.Disconnect();


                            Server.SendGlobalPacket(messaj.GetArray(stream));

                        }
                        else
                        {
                            data.AddText("Gender reassignment requires 1,075 CPs. Make sure you have enough money with you.")
                 .AddOption("Okay.", 255)
                 .AddAvatar(159).FinalizeDialog();
                        }

                        break;
                    }
                case 2:
                    {
                        data.AddText("To make gender reassignment as simple as possible, I want you to be single, not married. Make sure you`ve")
                            .AddText("~properly handled with your clan and guild affairs. After the reassignment, your name will be removed")
                            .AddText("~from the Charm ranking, and your garment will be put into your inventory. All in all, you need to carefully")
                   .AddOption("I~want~to~change~my~gender.", 3)
                   .AddOption("I`ll~think~about~it.", 255)
                   .AddAvatar(159).FinalizeDialog();

                        break;
                    }

            }
        }
    }
}

